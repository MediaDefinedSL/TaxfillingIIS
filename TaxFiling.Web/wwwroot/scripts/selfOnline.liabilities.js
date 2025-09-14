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

    $(document).off("click", ".allliabilities-editbtn").on("click", ".allliabilities-editbtn", function () {

        // Remove old validation messages
        $(".validation-error").remove();

        // Get all data attributes from Edit button
        var id = $(this).data("id");
        var type = $(this).data("type");
        var serialNo = $(this).data("serialno");
        var description = $(this).data("description");
        var acquisitionMode = $(this).data("acquisitionmode");
        var date = $(this).data("date");
        var cost = $(this).data("cost");

        // Fill form fields
        $("#hiddenOtherAssetsId").val(id);
        $("#ddlASType").val(type);
        $("#txtASSN").val(serialNo);
        $("#txtASDes").val(description);
        $("#txtASPurchase").val(acquisitionMode);
        $("#txtASDate").val(date);
        $("#txtASCost").val(cost);

        // Change button text
        $("#btnLiabilitySubmit").text("Update");

        // Scroll to form
        $("html, body").animate({ scrollTop: 0 }, "smooth");
    });

});