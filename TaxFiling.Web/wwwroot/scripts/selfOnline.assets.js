$(function () {

    const baseApiUrl = $("#hndapiUrl").val(); 
    console.log("Base API URL:", baseApiUrl);
    let userId = $("#hndUserid").val();

   
    loadFinancialDetails(userId, '2024/2025');

    document.querySelectorAll(".numeric-input").forEach(function (input) {
        input.addEventListener("input", function (e) {
            // Remove everything except digits and decimal point
            let value = e.target.value.replace(/[^0-9.]/g, "");

            // Only allow one decimal point
            let parts = value.split(".");
            if (parts.length > 2) {
                value = parts[0] + "." + parts.slice(1).join("");
            }

            // Format integer part with commas
            if (parts.length > 1) {
                // Has decimals
                let integerPart = parts[0];
                let decimalPart = parts[1].substring(0, 2); // limit to 2 decimals
                integerPart = integerPart ? parseInt(integerPart, 10).toLocaleString("en-US") : "0";
                value = integerPart + "." + decimalPart;
            } else {
                // No decimals
                if (value) {
                    value = parseInt(value, 10).toLocaleString("en-US");
                }
            }

            e.target.value = value;
        });

        input.addEventListener("blur", function (e) {
            let value = e.target.value.replace(/,/g, "");
            if (value) {
                let num = parseFloat(value);
                e.target.value = num.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            }
        });

        // Prevent entering multiple dots directly
        input.addEventListener("keypress", function (e) {
            if (!/[0-9.]/.test(e.key)) {
                e.preventDefault();
            }
            // Prevent typing a second dot
            if (e.key === "." && e.target.value.includes(".")) {
                e.preventDefault();
            }
        });
    });

    async function loadFinancialDetails(userId, assessmentYear) {
        try {
            const url = `${baseApiUrl}api/UserTaxAssistedOtherAssetsDetails/GetUserOtherTaxDetails?userId=${encodeURIComponent(userId)}&assessmentYear=${encodeURIComponent(assessmentYear)}`;
            const response = await fetch(url);

            //  if (!response.ok) {
            //     alert('Failed to load data');
            //     return;
            // }
            const data = await response.json();

            if (data && data.details && data.details.length > 0) {
                document.getElementById('saveFinancialDetailsBtn').innerText = "Update"; // change to update
            } else {
                document.getElementById('saveFinancialDetailsBtn').innerText = "Save";
            }

           

            // Clear all inputs first
            document.querySelectorAll('#expensesbalances .col-md-6 input').forEach(input => {
                input.value = '';      // clear old values
                input.disabled = false; // re-enable unless docStatus disables again later
            });

            // Populate inputs by matching label text with category
            data.details.forEach(detail => {
                document.querySelectorAll('#expensesbalances .col-md-6').forEach(div => {
                    const label = div.querySelector('label.form-label');
                    const input = div.querySelector('input');
                    if (label && input) {
                        const labelText = label.innerText.replace("*", "").trim(); // remove * if present
                        if (labelText === detail.category) {
                            if (!isNaN(detail.value)) {
                                input.value = Number(detail.value).toLocaleString('en-US');
                            } else {
                                input.value = detail.value;
                            }

                            
                        } 
                    }

                });
            });
        } catch (err) {
            alert('Error loading data: ' + err.message);
        }
    }

    let currentlyImmovablePropertiesEditingRow = null;
    let currentlyMVEditingRow = null;
    let currentlySSEditingRow = null;    
    let currentlyDeclareEditingRow = null;

    //------------------ Immovable Properties 
    $(document).off("click", "#btnImmovablePropertiesSubmit").on("click", "#btnImmovablePropertiesSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collect values from form
        let selfonlinePropertyId = $("#hiddenSelfonlinePropertyId").val();
        let type = $("#ddlImmovablePropertiesType").val();
        let serialNumber = $("#txtImmPropertiesSN").val();
        let situation = $("#txtImmPropertiesSituation").val();
        let dateOfAcquisition = $("#txtImmPropertiesDate").val();
        let cost = $("#txtImmPropertiesCost").val();
        let marketValue = $("#txtImmPropertiesMarketValue").val();

        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlImmovablePropertiesType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!serialNumber.trim()) {
            $("#txtImmPropertiesSN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!cost.trim()) {
            $("#txtImmPropertiesCost").after('<div class="text-danger validation-error">Cost  is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }

        if (!isValid) {
            return;
        }

        // Build object matching your ViewModel/DTO
        var immovableProperty = {
            SelfonlinePropertyID: selfonlinePropertyId,
            TransactionType: selfonlinePropertyId ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            Situation: situation,
            DateOfAcquisition: dateOfAcquisition,
            Cost: cost,
            MarketValue: marketValue
        };

        // Ajax call to save
        $.ajax({
            url: '/SelfOnlineFlow/AddEditSelfOnlineImmovableProperty',
            type: 'POST',
            data: immovableProperty,
            success: function (response) {
                $btn.prop("disabled", false);
                showMessage(selfonlinePropertyId ? "Updated successfully." : "Saved successfully", "success");
                currentlyImmovablePropertiesEditingRow = null;
                $("html, body").animate({ scrollTop: 0 }, "smooth");
                resetImmovablePropertiesForm();
                // Refresh grid so user sees latest data
                $("#ImmovablePropertiesGrid").load("/SelfOnlineFlow/LoadAssets #ImmovablePropertiesGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });

    function resetImmovablePropertiesForm() {

        $("#hiddenSelfonlinePropertyId").val("");
        $("#ddlImmovablePropertiesType").val("");
        $("#txtImmPropertiesSN").val("");
        $("#txtImmPropertiesSituation").val("");
        $("#txtImmPropertiesDate").val("");
        $("#txtImmPropertiesCost").val("");
        $("#txtImmPropertiesMarketValue").val("");
        $("#btnImmovablePropertiesSubmit").text("Submit");

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }
    $(document).on("click", "#btnImmovablePropertiesClear", function () {

        resetImmovablePropertiesForm();
        $("#btnImmovablePropertiesSubmit").text("Submit");

    });

    $(document).off("click", ".immovableiroperty-editbtn").on("click", ".immovableiroperty-editbtn", function () {

        // Remove old validation messages
        $(".validation-error").remove();

        // Get the current row
        var $row = $(this).closest("tr");
        if (currentlyImmovablePropertiesEditingRow && currentlyImmovablePropertiesEditingRow[0] !== $row[0]) {
            return showMessage("You are already editing another row. Please update before editing a new one.", "error");
        }
        // Mark this row as currently editing
        currentlyImmovablePropertiesEditingRow = $row;
        var $deleteBtn = $row.find(".immovableiroperty-deletebtn");

        // Disable the delete button in this row
        $deleteBtn.attr("data-disabled", "true");   // for persistence
        $deleteBtn.addClass("disabled-btn");
        $deleteBtn.prop("disabled", true);



        // Read all data-* attributes from the Edit button
        var id = $(this).data("id");
        var type = $(this).data("type");
        var serialNo = $(this).data("serialno");
        var situation = $(this).data("situation");
        var date = $(this).data("date");
        var cost = $(this).data("cost");
        var market = $(this).data("market");

        // Fill form fields
        $("#hiddenSelfonlinePropertyId").val(id);
        $("#ddlImmovablePropertiesType").val(type);
        $("#txtImmPropertiesSN").val(serialNo);
        $("#txtImmPropertiesSituation").val(situation);
        $("#txtImmPropertiesDate").val(date);
        $("#txtImmPropertiesCost").val(cost);
        $("#txtImmPropertiesMarketValue").val(market);

        // Change submit button text to Update
        $("#btnImmovablePropertiesSubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });


    //------------------ Motor Vehicle

    $(document).off("click", "#btnMotorVehicleSubmit").on("click", "#btnMotorVehicleSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);


        let motorVehicleID = $("#hiddenMotorVehicleID").val();
        let type = $("#ddlMType").val();
        let serialNumber = $("#txtMotorVehicleSN").val();
        let description = $("#txtMotorVehicleDescription").val();
        let registrationNo = $("#txtMotorVehicleRegNo").val();
        let dateOfAcquisition = $("#txtMotorVehicleDate").val();
        let costMarketValue = $("#txtMotorVehicleValue").val();

        let motorVehicle = {
            SelfonlineMotorVehicleID: motorVehicleID,
            TransactionType: motorVehicleID ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            Description: description,
            RegistrationNo: registrationNo,
            DateOfAcquisition: dateOfAcquisition,
            CostMarketValue: costMarketValue
        };

        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlMType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!serialNumber.trim()) {
            $("#txtMotorVehicleSN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!costMarketValue.trim()) {
            $("#txtMotorVehicleValue").after('<div class="text-danger validation-error">Cost/Market value  is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }

        if (!isValid) {
            return;
        }
        // AJAX POST
        $.ajax({
            url: '/SelfOnlineFlow/AddEditSelfOnlineMotorVehicle', // Replace with your controller action
            type: 'POST',
            data: motorVehicle,
            success: function (response) {
                $btn.prop("disabled", false);
                if (response.success) {
                    currentlyMVEditingRow = null;
                    showMessage(motorVehicleID ? "Updated successfully." : "Saved successfully", "success");
                    $("html, body").animate({ scrollTop: 0 }, "smooth");
                    resetMotorVehicleForm();
                    // Refresh grid so user sees latest data
                    $("#MotorVehicleGrid").load("/SelfOnlineFlow/LoadAssets #MotorVehicleGrid > *");
                } else {
                    alert("Error: " + response.message);
                }
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("An error occurred while saving the Motor Vehicle.");
            }
        })
    });


    function resetMotorVehicleForm() {

        $("#hiddenMotorVehicleID").val("");
        $("#ddlMType").val("");
        $("#txtMotorVehicleSN").val("");
        $("#txtMotorVehicleDescription").val("");
        $("#txtMotorVehicleRegNo").val("");
        $("#txtMotorVehicleDate").val("");
        $("#txtMotorVehicleValue").val("");
        $("#btnMotorVehicleSubmit").text("Submit");

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }

    $(document).on("click", "#btnMotorVehicleClear", function () {

        resetMotorVehicleForm();
        $("#btnMotorVehicleSubmit").text("Submit");

    });

    $(document).off("click", ".motorVehicle-editbtn").on("click", ".motorVehicle-editbtn", function () {

        // Remove old validation messages
        $(".validation-error").remove();

        // Get the current row
        var $row = $(this).closest("tr");
        if (currentlyMVEditingRow && currentlyMVEditingRow[0] !== $row[0]) {
            return showMessage("You are already editing another row. Please update before editing a new one.", "error");
        }
        // Mark this row as currently editing
        currentlyMVEditingRow = $row;
        var $deleteBtn = $row.find(".motorVehicle-deletebtn");

        // Disable the delete button in this row
        $deleteBtn.attr("data-disabled", "true");   // for persistence
        $deleteBtn.addClass("disabled-btn");
        $deleteBtn.prop("disabled", true);

        $("#hiddenMotorVehicleID").val($(this).data("id"));
        $("#ddlMType").val($(this).data("type"));
        $("#txtMotorVehicleSN").val($(this).data("serial"));
        $("#txtMotorVehicleRegNo").val($(this).data("registration"));
        $("#txtMotorVehicleDate").val($(this).data("date"));
        $("#txtMotorVehicleValue").val($(this).data("cost"));
        $("#txtMotorVehicleDescription").val($(this).data("description"));

        $("#btnMotorVehicleSubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    //--------------  Expenses & Balances
    document.getElementById('saveFinancialDetailsBtn').addEventListener('click', async () => {
        const container = document.getElementById('expensesbalances');
        const dataArray = [];

        // Validation: Check mandatory fields
        const requiredFields = ["Cash in hand", "Living Expenses"];
        let isValid = true;
        let missingFields = [];

        requiredFields.forEach(fieldLabel => {
            const input = Array.from(container.querySelectorAll('label.form-label'))
                .find(label => label.innerText.replace(/^\*\s*/, '').trim() === fieldLabel)
                ?.closest('.col-md-6')
                ?.querySelector('input');

            if (!input || input.value.trim() === "") {
                isValid = false;
                missingFields.push(fieldLabel);
            }
        });

        if (!isValid) {
            //alert("Please fill in the required fields: " + missingFields.join(", "));
            showMessage("Please fill in the required fields: " + missingFields.join(", "), "error")
            return;
        }

        container.querySelectorAll('input').forEach(input => {
            const parentDiv = input.closest('.col-md-6');
            if (parentDiv) {
                const label = parentDiv.querySelector('label.form-label');
                if (label) {
                    const value = input.value.trim();
                    if (value !== "") {  // only add if not empty
                        dataArray.push({
                            Category: label.innerText.replace(/^\*\s*/, '').trim(), // remove leading * and spaces
                            Value: value
                        });
                    }
                }
            }
        });

        // Add any other info you want, e.g., userId, assessmentYear
        const payload = {
            UserId: userId, // dynamically set
            AssessmentYear: '2024/2025',
            Details: dataArray
        };

        try {
            const response = await fetch(`${baseApiUrl}api/UserTaxAssistedOtherAssetsDetails/SaveUserOtherTaxDetails`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload),
            });

            if (!response.ok) {
                const err = await response.text();
                alert('Error: ' + err);
                return;
            }
            currentlyExEditingRow = null;
            if (document.getElementById('saveFinancialDetailsBtn').innerText == "Save") {
                showMessage("Saved successfully!", "success");
                document.getElementById('saveFinancialDetailsBtn').innerText = "Update";
            }
            else {
                showMessage("Updated Successfully!", "success");
            }
            
            
        } catch (err) {
            alert('Failed to submit: ' + err.message);
        }
    });

    //-----------------------------Shares Stocks Securities

    $(document).off("click", "#btnSharesStocksSubmit").on("click", "#btnSharesStocksSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collect values from form
        let selfonlineSharesStocksID = $("#hiddenSharesStocksID").val();
        let type = $("#ddlSType").val();
        let serialNumber = $("#txtSSN").val();
        let companyName = $("#txtNameCompanyInstitution").val();
        let noOfSharesStocks = $("#txtNoOfShares").val();
        let dateOfAcquisition = $("#txtSSSDate").val();
        let costOfAcquisition = $("#txtSCostOfAcquisition").val();
        let netDividendIncome = $("#txtSNetDvidend").val();

        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlSType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!serialNumber.trim()) {
            $("#txtSSN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!costOfAcquisition.trim()) {
            $("#txtSCostOfAcquisition").after('<div class="text-danger validation-error">Cost of Acquisition/Market value is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }

        if (!isValid) {
            return;
        }


        // Build object matching your ViewModel/DTO
        var sharesStocks = {
            SelfonlineSharesStocksID: selfonlineSharesStocksID,
            TransactionType: selfonlineSharesStocksID ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            CompanyName: companyName,
            NoOfSharesStocks: noOfSharesStocks,
            DateOfAcquisition: dateOfAcquisition,
            CostOfAcquisition: costOfAcquisition,
            NetDividendIncome: netDividendIncome
        };

        // Ajax call to save
        $.ajax({
            url: '/SelfOnlineFlow/SaveEditSelfonlineAssetsSharesStocksSecurities',
            type: 'POST',
            data: sharesStocks,
            success: function (response) {
                $btn.prop("disabled", false);
                currentlySSEditingRow = null;
                showMessage(selfonlineSharesStocksID ? "Updated successfully." : "Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");
                resetSharesStocksForm();
                // Refresh grid so user sees latest data
                $("#SharesStocksGrid").load("/SelfOnlineFlow/LoadAssets #SharesStocksGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });

    function resetSharesStocksForm() {

        $("#hiddenSharesStocksID").val("");
        $("#ddlSType").val("");
        $("#txtSSN").val("");
        $("#txtNameCompanyInstitution").val("");
        $("#txtNoOfShares").val("");
        $("#txtSSSDate").val("");
        $("#txtSCostOfAcquisition").val("");
        $("#txtSNetDvidend").val("");
        $("#btnSharesStocksSubmit").text("Submit");

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }
    $(document).on("click", "#btnSharesStocksClear", function () {

        resetSharesStocksForm();
        $("#btnSharesStocksSubmit").text("Submit");

    });

    $(document).off("click", ".sharesStocksSecurities-editbtn").on("click", ".sharesStocksSecurities-editbtn", function () {

        // Remove old validation messages
        $(".validation-error").remove();

        // Get the current row
        var $row = $(this).closest("tr");

        if (currentlySSEditingRow && currentlySSEditingRow[0] !== $row[0]) {
            return showMessage("You are already editing another row. Please update before editing a new one.", "error");
        }
        // Mark this row as currently editing
        currentlySSEditingRow = $row;
        var $deleteBtn = $row.find(".sharesStocksSecurities-deletebtn");

        // Disable the delete button in this row
        $deleteBtn.attr("data-disabled", "true");   // for persistence
        $deleteBtn.addClass("disabled-btn");
        $deleteBtn.prop("disabled", true);

        $("#hiddenSharesStocksID").val($(this).data("id"));
        $("#ddlSType").val($(this).data("type"));
        $("#txtSSN").val($(this).data("serial"));
        $("#txtNameCompanyInstitution").val($(this).data("company"));
        $("#txtNoOfShares").val($(this).data("nosharesstocks"));
        $("#txtSSSDate").val($(this).data("date"));
        $("#txtSCostOfAcquisition").val($(this).data("cost"));
        $("#txtSNetDvidend").val($(this).data("netdividend"));

        $("#btnSharesStocksSubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    //------------------- Capital and Current Account
    $(document).off("click", "#btnDeclareCapitalSubmit").on("click", "#btnDeclareCapitalSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collect values from form
        let selfonlineBusinessAccountID = $("#hiddenDeclareCapitalID").val();
        let type = $("#ddlDeclareCapitalType").val();
        let serialNumber = $("#txtDeclareCapitalSN").val();
        let businessName = $("#txtBusinessName").val();
        let currentAccountBalance = $("#txtCurrentAccountBalance").val();
        let capitalAccountBalance = $("#txtCapitalAccountBalance").val();

        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlDeclareCapitalType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!serialNumber.trim()) {
            $("#txtDeclareCapitalSN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
       
        if (!isValid) {
            return;
        }

        // Build object matching your ViewModel/DTO
        var businessAccount = {
            SelfonlineBusinessAccountID: selfonlineBusinessAccountID,
            TransactionType: selfonlineBusinessAccountID ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            BusinessName: businessName,
            CurrentAccountBalance: currentAccountBalance,
            CapitalAccountBalance: capitalAccountBalance
        };

        // Ajax call to save
        $.ajax({
            url: '/SelfOnlineFlow/SaveEditSelfonlineAssetsCapitalCurrentAccount',
            type: 'POST',
            data: businessAccount,
            success: function (response) {
                $btn.prop("disabled", false);
                currentlyDeclareEditingRow = null;
                showMessage(selfonlineBusinessAccountID ? "Updated successfully." : "Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");
                resetBusinessAccountForm();
                // Refresh grid so user sees latest data
                $("#DeclareCapitalGrid").load("/SelfOnlineFlow/LoadAssets #DeclareCapitalGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });

    function resetBusinessAccountForm() {

        $("#hiddenDeclareCapitalID").val("");
        $("#ddlDeclareCapitalType").val("");
        $("#txtDeclareCapitalSN").val("");
        $("#txtBusinessName").val("");
        $("#txtCurrentAccountBalance").val("");
        $("#txtCapitalAccountBalance").val("");

        $("#btnDeclareCapitalSubmit").text("Submit");

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }
    $(document).on("click", "#btnDeclareCapitalClear", function () {

        resetBusinessAccountForm();
        $("#btnDeclareCapitalSubmit").text("Submit");

    });

    $(document).off("click", ".capitalCurrentAccount-editbtn").on("click", ".capitalCurrentAccount-editbtn", function () {

        // Remove old validation messages
        $(".validation-error").remove();

        // Get the current row
        var $row = $(this).closest("tr");
        if (currentlyDeclareEditingRow && currentlyDeclareEditingRow[0] !== $row[0]) {
            return showMessage("You are already editing another row. Please update before editing a new one.", "error");
        }
        // Mark this row as currently editing
        currentlyDeclareEditingRow = $row;
        var $deleteBtn = $row.find(".capitalCurrentAccount-deletebtn");

        // Disable the delete button in this row
        $deleteBtn.attr("data-disabled", "true");   // for persistence
        $deleteBtn.addClass("disabled-btn");
        $deleteBtn.prop("disabled", true);

        $("#hiddenDeclareCapitalID").val($(this).data("id"));
        $("#ddlDeclareCapitalType").val($(this).data("type"));
        $("#txtDeclareCapitalSN").val($(this).data("serial"));
        $("#txtBusinessName").val($(this).data("company"));
        $("#txtCurrentAccountBalance").val($(this).data("currentaccount"));
        $("#txtCapitalAccountBalance").val($(this).data("capitalaccount"));


        $("#btnDeclareCapitalSubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    //----------- delete record

    let deleteAssetsId = null;
    let deleteCategoryName = null;

    $(document).on("click", ".immovableiroperty-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteAssetsId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });


    $(document).on("click", ".motorVehicle-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteAssetsId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });

    $(document).on("click", ".sharesStocksSecurities-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteAssetsId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });

    $(document).on("click", ".capitalCurrentAccount-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteAssetsId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });

    $(document).on("click", "#selfonline_confirmDeleteBtn", function () {

        if (!deleteAssetsId) return;

        var deleteData = {
            deleteId: deleteAssetsId,
            categoryName: deleteCategoryName
        };


        $.ajax({
            url: '/SelfOnlineFlow/DeleteSelfOnlineAssetsLiabilitiesDetails',
            type: 'POST',
            data: deleteData,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');
                if (deleteCategoryName == "ImmovableProperty") {
                    // Refresh grid so user sees latest data
                    $("#ImmovablePropertiesGrid").load("/SelfOnlineFlow/LoadAssets #ImmovablePropertiesGrid > *");
                }
                if (deleteCategoryName == "MotorVehicle") {
                    // Refresh grid so user sees latest data
                    $("#MotorVehicleGrid").load("/SelfOnlineFlow/LoadAssets #MotorVehicleGrid > *");
                }
                if (deleteCategoryName == "SharesStocksSecurities") {
                    // Refresh grid so user sees latest data
                    $("#SharesStocksGrid").load("/SelfOnlineFlow/LoadAssets #SharesStocksGrid > *");
                }
                if (deleteCategoryName == "CapitalCurrentAccount") {
                    // Refresh grid so user sees latest data
                    $("#DeclareCapitalGrid").load("/SelfOnlineFlow/LoadAssets #DeclareCapitalGrid > *");
                }

            },
            error: function () {
                alert("Error deleting.");
            }
        });




    });


    $('#linkLiabilitiesContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadLiabilities',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
    });
    $('#linkDeductionsContinue').on('click', function () {
        $.ajax({
            url: '/SelfOnlineFlow/LoadDeductions',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
            },
            error: function () {
                alert("Error loading section content.");
            }
        });
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });
});