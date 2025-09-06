$(function () {

    $('#linkContactInformationrContinue').on('click', function () {

        $(".validation-error").remove();

        let careOf = $("#CareOf").val();
        let apt = $("#Apt").val();
        let streetNumber = $("#StreetNumber").val();

        let street = $("#Street").val();
        let city = $("#City").val();
        let district = $("#District").val();
        let postalCode = $("#PostalCode").val();
        let country = $("#Country").val();
        let emailPrimary = $("#EmailPrimary").val();
        let emailSecondary = $("#EmailSecondary").val();
        let mobilePhone = $("#MobilePhone").val();
        let homePhone = $("#HomePhone").val();
        let whatsApp = $("#WhatsApp").val();
        let preferredCommunicationMethod = $("#PreferredCommunicationMethod").val();  

        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var phonePattern = /^(\+?\d{1,3}[- ]?)?\d{10}$/;

        let isValid = true;

        if (streetNumber.length == 0) {
            $("#StreetNumber").after('<div class="text-danger validation-error">Street Number is required.</div>');
            isValid = false;
        }
       if (street.length == 0) {
           $("#Street").after('<div class="text-danger validation-error">Street name is required.</div>');
            isValid = false;
        }
        if (city.length == 0) {
            $("#City").after('<div class="text-danger validation-error">City is required.</div>');
            isValid = false;
        }
        if (emailPrimary.length == 0) {
            $("#EmailPrimary").after('<div class="text-danger validation-error">Primary Email is required.</div>');
            isValid = false;
        }
        if (mobilePhone.length == 0) {
            $("#MobilePhone").after('<div class="text-danger validation-error">Mobile Phone is required.</div>');
            isValid = false;
        }
        if (!emailPattern.test(emailPrimary)) { 
            $("#EmailPrimary").after('<div class="text-danger validation-error">Primary email is Ivalid email.</div>');
            isValid = false;
        }
        if (!emailPattern.test(emailSecondary)) {
            $("#EmailSecondary").after('<div class="text-danger validation-error">Secondary email is Ivalid email.</div>');
            isValid = false;
        }
        if (!phonePattern.test(mobilePhone)) {
            $("#EmailSecondary").after('<div class="text-danger validation-error">Invalid phone number!</div>');
            isValid = false;
        }
        if (!phonePattern.test(mobilePhone)) {
            $("#MobilePhone").after('<div class="text-danger validation-error">Invalid phone number!</div>');
            isValid = false;
        }
        if (!phonePattern.test(homePhone)) {
            $("#HomePhone").after('<div class="text-danger validation-error">Invalid phone number!</div>');
              isValid = false;
       }
        if (!phonePattern.test(whatsApp)) {
           $("#WhatsApp").after('<div class="text-danger validation-error">Invalid phone number!</div>');
             isValid = false;
        }

        if (!isValid) {
            return;
        }

        var user = {
            CareOf: $("#CareOf").val(),
            Apt: $("#Apt").val(),
            StreetNumber: $("#StreetNumber").val(),
            Street: $("#Street").val(),
            City: $("#City").val(),
            District: district,
            PostalCode: postalCode,
            Country: country,
            EmailPrimary: emailPrimary,
            EmailSecondary: emailSecondary,
            MobilePhone: mobilePhone,
            HomePhone: homePhone,
            WhatsApp: whatsApp,
            PreferredCommunicationMethod: preferredCommunicationMethod
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
                        $("html, body").animate({ scrollTop: 0 }, "smooth");
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

    $('#linkContactInformationNext').on('click', function () {

        $.ajax({
            url: '/SelfOnlineFlow/LoadIdentification',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkIdentification').addClass('active');
                $("html, body").animate({ scrollTop: 0 }, "smooth");
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });

});