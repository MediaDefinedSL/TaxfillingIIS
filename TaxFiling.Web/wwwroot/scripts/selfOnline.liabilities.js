$(function () {

    
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

    //------------------ All Liabilities
    $(document).off("click", "#btnLiabilitySubmit").on("click", "#btnLiabilitySubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collect values from form
        let AllliabilitiesId = $("#hiddenAllliabilitiesId").val();
        let type = $("#ddlAllliabilitiesType").val();
        let serialNumber = $("#txtAllliabilitiesSN").val();
        let description = $("#txtAllliabilitiesDes").val();
        let security = $("#txtliabilitiesSecurity").val();
        let date = $("#txtliabilityDate").val();
        let oamount = $("#txtLiabilityOriginalAmount").val();
        let amount = $("#txtLiabilityAmount").val();
        let repamount = $("#txtLiabilityAmountRepaid").val();
        
        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlAllliabilitiesType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!serialNumber.trim()) {
            $("#txtAllliabilitiesSN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
       
        if (!isValid) {
            return;
        }

        // Build object matching your ViewModel/DTO
        var liabilities = {
            SelfonlineLiabilityID: AllliabilitiesId,
            TransactionType: AllliabilitiesId ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            Description: description,
            SecurityOnLiability: security,
            DateOfCommencement: date,
            OriginalAmount: oamount,
            AmountAsAt: amount,
            AmountRepaid: repamount
        };

        // Ajax call to save
        $.ajax({
            url: '/SelfOnlineFlow/SaveEditSelfonlineLiabilitiesAllLiabilities',
            type: 'POST',
            data: liabilities,
            success: function (response) {
                $btn.prop("disabled", false);
                showMessage(AllliabilitiesId ? "Updated successfully." : "Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");
                resetLiabilitiesForm();
                // Refresh grid so user sees latest data
                $("#LiabilityGrid").load("/SelfOnlineFlow/LoadLiabilities #LiabilityGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });

    function resetLiabilitiesForm() {

        $("#hiddenAllliabilitiesId").val("");
        $("#ddlAllliabilitiesType").val("");
        $("#txtAllliabilitiesSN").val("");
        $("#txtAllliabilitiesDes").val("");
        $("#txtliabilitiesSecurity").val("");
        $("#txtliabilityDate").val("");
        $("#txtLiabilityOriginalAmount").val("");
        $("#txtLiabilityAmount").val("");
        $("#txtLiabilityAmountRepaid").val("");

        // Remove validation error messages
        $(".validation-error").remove();

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }
    $(document).on("click", "#btnImmovablePropertiesClear", function () {

        resetLiabilitiesForm();
        $("#btnLiabilitySubmit").text("Submit");

    });

    $(document).off("click", ".allliabilities-editbtn").on("click", ".allliabilities-editbtn", function () {
     
        // Remove old validation messages
        $(".validation-error").remove();

        // Get all data attributes from Edit button
        var id = $(this).data("id");
        var type = $(this).data("type");
        var serialNo = $(this).data("serialno");
        var description = $(this).data("description");
        var security = $(this).data("security");
        var date = $(this).data("date");
        var oamount = $(this).data("oamount");
        var amount = $(this).data("amount");
        var repamount = $(this).data("repamount");

        // Fill form fields
        $("#hiddenAllliabilitiesId").val(id);
        $("#ddlAllliabilitiesType").val(type);
        $("#txtAllliabilitiesSN").val(serialNo);
        $("#txtAllliabilitiesDes").val(description);
        $("#txtliabilitiesSecurity").val(security);
        $("#txtliabilityDate").val(date);
        $("#txtLiabilityOriginalAmount").val(oamount);
        $("#txtLiabilityAmount").val(amount);
        $("#txtLiabilityAmountRepaid").val(repamount);

        // Change button text
        $("#btnLiabilitySubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    //------------------ Other Assets

    $(document).off("click", "#btnOtherAssetsSubmit").on("click", "#btnOtherAssetsSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collect values from form
        let OtherAssetsId = $("#hiddenOtherAssetsId").val();
        let type = $("#ddlASType").val();
        let serialNumber = $("#txtASSN").val();
        let description = $("#txtASDes").val();
        let acquisitionMode = $("#txtASPurchase").val();
        let dateOfAcquisition = $("#txtASDate").val();
        let costMarketValue = $("#txtASCost").val();

        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlASType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }
        if (!serialNumber.trim()) {
            $("#txtASSN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
            $btn.prop("disabled", false);
        }

        if (!isValid) {
            return;
        }

        // Build object matching your ViewModel/DTO
        var otherAssets = {
            SelfonlineAssetsGiftsID: OtherAssetsId,
            TransactionType: OtherAssetsId ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            Description: description,
            AcquisitionMode: acquisitionMode,
            DateOfAcquisition: dateOfAcquisition,
            CostMarketValue: costMarketValue
        };

        // Ajax call to save
        $.ajax({
            url: '/SelfOnlineFlow/SaveEditSelfonlineLiabilitiesOtherAssetsGifts',
            type: 'POST',
            data: otherAssets,
            success: function (response) {
                $btn.prop("disabled", false);
                showMessage(OtherAssetsId ? "Updated successfully." : "Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");
                resetOtherAssetsForm();
                // Refresh grid so user sees latest data
                $("#OtherAssetsGrid").load("/SelfOnlineFlow/LoadLiabilities #OtherAssetsGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });

    function resetOtherAssetsForm() {

        $("#hiddenOtherAssetsId").val("");
        $("#ddlASType").val("");
        $("#txtASSN").val("");
        $("#txtASDes").val("");
        $("#txtASPurchase").val("");
        $("#txtASDate").val("");
        $("#txtASCost").val("");

       
        $("#btnOtherAssetsSubmit").text("Submit");
        // Remove validation error messages
        $(".validation-error").remove();

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }
    $(document).on("click", "#btnOtherAssetsClear", function () {

        resetOtherAssetsForm();
        $("#btnOtherAssetsSubmit").text("Submit");

    });

    $(document).off("click", ".otherAsset-editbtn").on("click", ".otherAsset-editbtn", function () {

        // Remove old validation messages
        $(".validation-error").remove();

        // Get all data attributes from Edit button
        var id = $(this).data("id");
        var type = $(this).data("type");
        var serialNo = $(this).data("serialno");
        var description = $(this).data("description");
        var security = $(this).data("security");
        var date = $(this).data("date");
        var oamount = $(this).data("oamount");

        // Fill form fields
        $("#hiddenOtherAssetsId").val(id);
        $("#ddlASType").val(type);
        $("#txtASSN").val(serialNo);
        $("#txtASDes").val(description);
        $("#txtASPurchase").val(security);
        $("#txtASDate").val(date);
        $("#txtASCost").val(oamount);

        // Change button text
        $("#btnOtherAssetsSubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

    //------------------ Disposal Asets

    $(document).off("click", "#btnDisposalAsetsSubmit").on("click", "#btnDisposalAsetsSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Collect values from form
        let disposalAsetsId = $("#hiddenDisposalAsetsId").val();
        let type = $("#ddlDAType").val();
        let serialNumber = $("#txtDASN").val();
        let description = $("#txtDADes").val();
        let dateOfDisposal = $("#txtDADate").val();
        let salesProceed = $("#txtDASales").val();
        let dateAcquired = $("#txtDADateAcquired").val();
        let cost = $("#txtDACost").val();

        let isValid = true;

        $(".validation-error").remove();

        if (!type) {
            $("#ddlDAType").after('<div class="text-danger validation-error">Please select Type .</div>');
            isValid = false;
        }
        if (!serialNumber.trim()) {
            $("#txtDASN").after('<div class="text-danger validation-error">S/N is required.</div>');
            isValid = false;
        }

        if (!isValid) {
            $btn.prop("disabled", false);
            return;
        }

        // Build object matching your ViewModel/DTO
        var disposalAssets = {
            SelfonlineDisposalAssetsID: disposalAsetsId,
            TransactionType: disposalAsetsId ? "Edit" : "Add",
            Type: type,
            SerialNumber: serialNumber,
            Description: description,
            DateOfDisposal: dateOfDisposal,
            SalesProceed: salesProceed,
            DateAcquired: dateAcquired,
            Cost: cost
        };

        // Ajax call to save
        $.ajax({
            url: '/SelfOnlineFlow/SaveEditSelfonlineLiabilitiesDisposalAssets',
            type: 'POST',
            data: disposalAssets,
            success: function (response) {
                $btn.prop("disabled", false);
                showMessage(disposalAsetsId ? "Updated successfully." : "Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");
                resetDisposalAsetsForm();
                // Refresh Disposal Assets grid
                $("#DisposalAsetsGrid").load("/SelfOnlineFlow/LoadLiabilities #DisposalAsetsGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });

    function resetDisposalAsetsForm() {

        $("#hiddenDisposalAsetsId").val("");
        $("#ddlDAType").val("");
        $("#txtDASN").val("");
        $("#txtDADes").val("");
        $("#txtDADate").val("");
        $("#txtDASales").val("");
        $("#txtDADateAcquired").val("");
        $("#txtDACost").val("");


        $("#btnDisposalAsetsSubmit").text("Submit");
        $(".validation-error").remove();

        $("html, body").animate({ scrollTop: 0 }, "smooth");
    }
    $(document).on("click", "#btnDisposalAsetsClear", function () {

        resetDisposalAsetsForm();
        $("#btnDisposalAsetsSubmit").text("Submit");

    });

    $(document).off("click", ".disposalAsets-editbtn").on("click", ".disposalAsets-editbtn", function () {

        $(".validation-error").remove();

        var id = $(this).data("id");
        var type = $(this).data("type");
        var serialNo = $(this).data("serialno");
        var description = $(this).data("description");
        var ddate = $(this).data("ddate");
        var salesProceed = $(this).data("salesproceed");
        var adate = $(this).data("adate");
        var cost = $(this).data("cost");

        // Fill form fields
        $("#hiddenDisposalAsetsId").val(id);
        $("#ddlDAType").val(type);
        $("#txtDASN").val(serialNo);
        $("#txtDADes").val(description);
        $("#txtDADate").val(ddate);
        $("#txtDASales").val(salesProceed);
        $("#txtDADateAcquired").val(adate);
        $("#txtDACost").val(cost);

        $("#btnDisposalAsetsSubmit").text("Update");
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });


    //----------- delete record

    let deleteLiabilitiesId = null;
    let deleteCategoryName = null;

    $(document).on("click", ".allliabilities-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteLiabilitiesId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });


    $(document).on("click", ".allliabilities-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteLiabilitiesId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });
    $(document).on("click", ".otherAsset-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteLiabilitiesId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });
    $(document).on("click", ".disposalAsets-deletebtn", function () {

        if ($(this).data("disabled")) {
            // Stop the modal from opening if disabled
            e.preventDefault();
            e.stopImmediatePropagation();
            return false;
        }
        deleteLiabilitiesId = $(this).data("id");
        deleteCategoryName = $(this).data("name");

        $('#selfonline_confirmDeleteModal').modal('show');

    });

    $(document).on("click", "#selfonline_confirmDeleteBtn", function () {

        if (!deleteLiabilitiesId) return;

        var deleteData = {
            deleteId: deleteLiabilitiesId,
            categoryName: deleteCategoryName
        };


        $.ajax({
            url: '/SelfOnlineFlow/DeleteSelfOnlineAssetsLiabilitiesDetails',
            type: 'POST',
            data: deleteData,
            success: function (response) {
                $('#selfonline_confirmDeleteModal').modal('hide');
                if (deleteCategoryName == "AllLiabilities") {
                    // Refresh grid so user sees latest data
                    $("#LiabilityGrid").load("/SelfOnlineFlow/LoadLiabilities #LiabilityGrid > *");
                }
                if (deleteCategoryName == "OtherAssets") {
                    // Refresh grid so user sees latest data
                    $("#OtherAssetsGrid").load("/SelfOnlineFlow/LoadLiabilities #OtherAssetsGrid > *");
                }
                if (deleteCategoryName == "DisposalAsets") {
                    // Refresh grid so user sees latest data
                    $("#DisposalAsetsGrid").load("/SelfOnlineFlow/LoadLiabilities #DisposalAsetsGrid > *");
                }
                

            },
            error: function () {
                alert("Error deleting.");
            }
        });

    });
});

$('#linkSummaryContinue').on('click', function () {
    $.ajax({
        url: '/SelfOnlineFlow/LoadSummarySection',
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
$('#linkAssetsContinue').on('click', function () {
    //$.ajax({
    //    url: '/SelfOnlineFlow/LoadAssets',
    //    type: 'GET',
    //    success: function (data) {
    //        $('#in-this-section-container').html(data);
    //    },
    //    error: function () {
    //        alert("Error loading section content.");
    //    }
    //});
    $("html, body").animate({ scrollTop: 0 }, "smooth");
});