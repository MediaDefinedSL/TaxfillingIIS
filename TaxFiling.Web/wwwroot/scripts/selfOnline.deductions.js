$(function () {

    // Format on blur
    $(document).on("blur", ".numeric-format", function () {
        let value = $(this).val().replace(/,/g, "");
        if (value && !isNaN(value)) {
            let num = parseFloat(value);
            $(this).val(num.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
        } else {
            $(this).val("0.00"); // default if left empty
        }
    });

    // Parse number
    function parseNumber(value) {
        return parseFloat(value.toString().replace(/,/g, '')) || 0;
    }

    // Format number
    function formatNumber(value) {
        return value.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
    }

    // Update total reliefs
    function updateTotalReliefs() {
        var rentRelief = parseNumber($("#RentRelief25").val());
        var solarRelief = parseNumber($("#ReliefSolarPanel").val());
        var personalRelief = parseNumber($("#PersonalRelief").val());

        var totalReliefs = rentRelief + solarRelief + personalRelief;
        $("#txtTotalReliefs").val(formatNumber(totalReliefs));
        return totalReliefs;
    }

    // Update total deductions
    function updateTotalDeductions() {
        var totalReliefs = updateTotalReliefs();
        var qualifyingPayments = parseNumber($("#QualifyingPayments").val());

        var totalDeductions = totalReliefs + qualifyingPayments;
        $("#txtTotalDeductions").val(formatNumber(totalDeductions));
        return totalDeductions;
    }

    // Update taxable income
    function updateTaxableIncome() {
        var assessableIncome = parseNumber($("#hndAssessableIncome").val());
        var totalDeductions = updateTotalDeductions();

        var taxableIncome = assessableIncome - totalDeductions;
        if (taxableIncome < 0) taxableIncome = 0; // prevent negative

        $("#txtTaxableIncome").val(formatNumber(taxableIncome));
    }

    // Enforce max for ReliefSolarPanel
    $("#ReliefSolarPanel").on("input", function () {
        var value = parseNumber($(this).val());
        if (value > 600000) {
            value = 600000;
            $(this).val(formatNumber(value));
        }
        updateTaxableIncome();
    });

    // Update when QualifyingPayments changes
    $("#QualifyingPayments").on("input", function () {
        var value = parseNumber($(this).val());
        if (isNaN(value) || value < 0) {
            $(this).val("0.00");
            value = 0;
        }
        updateTaxableIncome();
    });

    // Initial calculation
    updateTaxableIncome();

    // Submit validation
    $(document).off("click", "#btnDeductionsSubmit").on("click", "#btnDeductionsSubmit", function () {
        var $btn = $(this);
        $btn.prop("disabled", true);

        // Get form values
        let selfOnlineTotalId = $("#hiddenSelfOnlineTotalId").val();
        let reliefSolarPanel = $("#ReliefSolarPanel").val();
        let qualifyingPayments = $("#QualifyingPayments").val();
        

        var selfOnlineTotal = {
            SelfOnlineTotalId: selfOnlineTotalId,
            ReliefSolarPanel: reliefSolarPanel,
            QualifyingPayments: qualifyingPayments
        };

        $.ajax({
            url: '/SelfOnlineFlow/UpdateSelfFilingTotalCalculation',
            type: 'POST',
            data: selfOnlineTotal,
            success: function (response) {
                $btn.prop("disabled", false);
                showMessage( "Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");
               
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving.");
            }
        });


    });

  
});