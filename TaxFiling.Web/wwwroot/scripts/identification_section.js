$(function () {

    $('#linkTaxPayerContinue').on('click', function () {
        let firstName = $("#FirstName").val();
        let middleName = $("#MiddleName").val();
        let lastName = $("#LastName").val();

        let dateOfBirth = $("#DateOfBirth").val();
        let taxNumber = $("#TaxNumber").val();
        let NIC_NO = $("#NIC_NO").val();
        let gender = $("#Gender").val();
        let address = $("#Address").val();


        var user = {
            FirstName: $("#FirstName").val(),
            MiddleName: $("#MiddleName").val(),
            LastName: $("#LastName").val(),
            DateOfBirth: $("#DateOfBirth").val(),
            TaxNumber: $("#TaxNumber").val(),
            NIC_NO: $("#NIC_NO").val(),
            Gender: $("#Gender").val(),
            Address: $("#Address").val(),
        };

        console.log(user);

        if (firstName.length == 0) {
            notifyError(false, "First Name is required");
          
        }
        else if (lastName.length == 0) {
            notifyError(false, "Last Name is required");
           
        }
        else if (DateOfBirth.length == 0) {
            notifyError(false, "Date Birthday is required");
           
        }
        else if (DateOfBirth.length == 0) {
            notifyError(false, "Gender is required");

        }

        else { 
            $.ajax({
                url: `${appUrl}/SelfOnlineFlow/UpdateUserIdentifications`,
                type: "PUT",
                contentType: "application/json",
                data: JSON.stringify(user),
                success: function (response) {
                    $.ajax({
                        url: '/SelfOnlineFlow/LoadContactInformation',
                        type: 'GET',
                        success: function (data) {
                            $('#in-this-section-container').html(data);
                            $('.sub-link').removeClass('active');
                            $('#linkContactInformation').addClass('active');
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
            url: '/SelfOnlineFlow/LoadTaxReturnLastyear',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkLastYear').addClass('active');
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });

});