$(function () {


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
                showMessage("Saved successfully", "success");
                $("html, body").animate({ scrollTop: 0 }, "smooth");

                // Refresh grid so user sees latest data
                $("#ImmovablePropertiesGrid").load(location.href + " #ImmovablePropertiesGrid>*", "");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
    });
});