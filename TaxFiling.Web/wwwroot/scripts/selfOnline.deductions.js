$(function () {
    $(document).on("blur", ".numeric-format", function () {
        let value = $(this).val().replace(/,/g, "");
        if (value && !isNaN(value)) {
            let num = parseFloat(value);
            $(this).val(num.toLocaleString("en-US", { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
        }
    });


   

    // Trigger calculation on page load
    updateTotalReliefs();

    // Trigger calculation when ReliefSolarPanel changes
    $(".numeric-format").on("input", function () {
        updateTotalReliefs();
    });


    // Function to calculate total reliefs
    function updateTotalReliefs() {
        // Get values from the inputs
        var rentRelief = parseFloat($("#RentRelief25").val().replace(/,/g, '')) || 0;
        var solarRelief = parseFloat($("#ReliefSolarPanel").val().replace(/,/g, '')) || 0;
        var personalRelief = parseFloat($("#PersonalRelief").val().replace(/,/g, '')) || 0;

        // Calculate total
        var total = rentRelief + solarRelief + personalRelief;

        // Update total reliefs textbox with comma and 2 decimals
        $("#txtTotalReliefs").val(total.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 }));
    }
    $(document).off("click", "#btnDeductionsSubmit").on("click", "#btnDeductionsSubmit", function () {

    });
});