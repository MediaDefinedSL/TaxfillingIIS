$(function () {

    $('#linkTaxPayerContinue').on('click', function () {
        let careOf = $("#CareOf").val();
        let apt = $("#Apt").val();
        let streetNumber = $("#StreetNumber").val();

        let street = $("#Street").val();
        let city = $("#City").val();


        if (streetNumber.length == 0) {
            notifyError(false, "Street Number is required");

        }
        else if (street.length == 0) {
            notifyError(false, "Street is required");

        }
        else if (DateOfBirth.length == 0) {
            notifyError(false, "City  is required");

        }

        else { 


        var user = {
            CareOf: $("#CareOf").val(),
            Apt: $("#Apt").val(),
            StreetNumber: $("#StreetNumber").val(),
            Street: $("#Street").val(),
            City: $("#City").val()
        };


        $.ajax({
            url: `${appUrl}/SelfOnlineFlow/UpdateContactInfromation`,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify(user),
            success: function (response) {
                $.ajax({
                    url: '/SelfOnlineFlow/LoadSummary',
                    type: 'GET',
                    success: function (data) {
                        $('#in-this-section-container').html(data);
                        $('.sub-link').removeClass('active');
                        $('#linkSummary').addClass('active');
                    },
                    error: function () {
                        alert("Error loading section content.");
                    }
                });
            },
            error: function () {
                alert("Error saving taxpayer.");
            }
        });
    }


    });

    $('#linkTaxPayerNext').on('click', function () {

        $.ajax({
            url: '/SelfOnlineFlow/LoadIdentification',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkIdentification').addClass('active');
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });

});