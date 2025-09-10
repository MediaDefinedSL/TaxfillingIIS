$(function () {

    $(document).on("blur", ".numeric-format", function () {
        let value = $(this).val().replace(/,/g, "");
        if (value && !isNaN(value)) {
            let num = parseFloat(value);
            $(this).val(num.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
        }
    });
    // Function to parse formatted numbers
    function parseNumber(value) {
        return parseFloat(value.toString().replace(/,/g, '')) || 0;
    }

    // Function to format numbers with commas and 2 decimals
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

        var taxableIncome =  assessableIncome - totalDeductions;
        if (taxableIncome < 0) taxableIncome = 0; // Prevent negative income

        $("#txtTaxableIncome").val(formatNumber(taxableIncome));
    }

    // Trigger calculation on page load
    updateTaxableIncome();

    // Trigger updates on input changes
    $("#ReliefSolarPanel, #QualifyingPayments").on("input", function () {
        updateTaxableIncome();
    });

    $(document).off("click", "#btnDeductionsSubmit").on("click", "#btnDeductionsSubmit", function () {

    });
});