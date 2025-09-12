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
                $("#ImmovablePropertiesGrid").load("/SelfOnlineFlow/LoadAssets #ImmovablePropertiesGrid > *");
            },
            error: function () {
                $btn.prop("disabled", false);
                alert("Error saving immovable property.");
            }
        });
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
        // AJAX POST
        $.ajax({
            url: '/SelfOnlineFlow/AddEditSelfOnlineMotorVehicle', // Replace with your controller action
            type: 'POST',
            data: motorVehicle,
            success: function (response) {
                $btn.prop("disabled", false);
                if (response.success) {
                    showMessage("Saved successfully", "success");
                    $("html, body").animate({ scrollTop: 0 }, "smooth");

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
});