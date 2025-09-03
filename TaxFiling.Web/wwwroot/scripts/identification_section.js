$(function () {

    $('#linkIdentificationContinue').on('click', function () {

        $(".validation-error").remove();

        let firstName = $("#FirstName").val();
        let middleName = $("#MiddleName").val();
        let lastName = $("#LastName").val();

        let dateOfBirth = $("#DateOfBirth").val();
        let taxNumber = $("#TaxNumber").val();
        let NIC_NO = $("#NIC_NO").val();
        let gender = $("#Gender").val();
        let address = $("#Address").val();
        let title = $("#drpTitle").val();
        let passportNo = $("#PassportNo").val();
        let nationality = $("#Nationality").val();
        let ocupation = $("#Occupation").val();
        let employerName = $("#EmployerName").val();
        
        let isValid = true;
        let AgeValid = true;

       
        $

        if (firstName.length == 0) {
            $("#FirstName").after('<div class="text-danger validation-error">First Name is required.</div>');
            isValid = false;
        }
       if (lastName.length == 0) {
            $("#LastName").after('<div class="text-danger validation-error">Last Name is required.</div>');
            isValid = false;
           
        }
        //if (dateOfBirth.length == 0) {
        //    $("#DateOfBirth").after('<div class="text-danger validation-error">Date Birthday is required.</div>');
        //    isValid = false;
        //}
         if (gender.length == 0) {
            $("#Gender").after('<div class="text-danger validation-error">Gender is required.</div>');
            isValid = false;
        }
        if (!dateOfBirth) {
            $("#DateOfBirth").after('<div class="text-danger validation-error">Date of Birth is required.</div>');
            isValid = false;
        } else {
            let dob = new Date(dateOfBirth);
            let today = new Date();
            let minAgeDate = new Date();
            minAgeDate.setFullYear(today.getFullYear() - 18);

            if (dob > minAgeDate) {
                $("#DateOfBirth").after('<div class="text-danger validation-error">You must be at least 18 years old.</div>');
                isValid = false;
            }
        }

        if (!isValid) {
         
            return;
        }

        var user = {
            FirstName: $("#FirstName").val(),
            MiddleName: $("#MiddleName").val(),
            LastName: $("#LastName").val(),
            DateOfBirth: $("#DateOfBirth").val(),
            TaxNumber: $("#TaxNumber").val(),
            NIC_NO: $("#NIC_NO").val(),
            Gender: $("#Gender").val(),
            Address: $("#Address").val(),
            Title: title,
            PassportNo: passportNo,
            Nationality: nationality,
            Occupation: ocupation,
            EmployerName: employerName

        };
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


       
    });

    $('#linkIdentificationNext').on('click', function () {

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