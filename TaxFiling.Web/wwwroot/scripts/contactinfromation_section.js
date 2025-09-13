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
        if (emailPrimary.length > 0) { 
            if (!emailPattern.test(emailPrimary)) {
                $("#EmailPrimary").after('<div class="text-danger validation-error">Primary email is Invalid email.</div>');
                isValid = false;
            }
        }
        if (emailSecondary.length > 0) {
            if (!emailPattern.test(emailSecondary)) {
                $("#EmailSecondary").after('<div class="text-danger validation-error">Secondary email is Invalid email.</div>');
                isValid = false;
            }
        }
        if (mobilePhone.length > 0) { 
            if (!phonePattern.test(mobilePhone)) {
                $("#MobilePhone").after('<div class="text-danger validation-error">Invalid phone number!</div>');
                isValid = false;
            }
        }
        //if (!phonePattern.test(mobilePhone)) {
            //$("#MobilePhone").after('<div class="text-danger validation-error">Invalid phone number!</div>');
            //isValid = false;
        // }
        if (homePhone.length > 0) {
            if (!phonePattern.test(homePhone)) {
                $("#HomePhone").after('<div class="text-danger validation-error">Invalid phone number!</div>');

                isValid = false;
            }
        }

        if (whatsApp.length > 0) {
            if (!phonePattern.test(whatsApp)) {
                $("#WhatsApp").after('<div class="text-danger validation-error">Invalid phone number!</div>');
                isValid = false;
            }
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
                if (document.getElementById("docUploadStatus")) {
                    document.getElementById("personalInfoCompleted").value = "1";
                    const el = document.getElementById("personalInfoStatus");
                    if (!el) return;

                    if (document.getElementById("personalInfoCompleted").value == 1) {
                        if (el.textContent == "Missing") { 
                            el.textContent = "✓";
                            el.className = "text-success";
                            const bar = document.getElementById("progressBar");
                            const widthValue = bar.style.width; // "54%"
                            const progress = parseInt(widthValue, 10);
                            const newProgress = progress + 18;

                            if (bar) {
                                bar.style.width = newProgress + "%";
                                bar.textContent = newProgress + "%";
                            }
                            // Get the row
                            const row = document.getElementById("assessment-row-2024");
                            // Find the progress bar inside this row
                            const progressBar = row.querySelector(".progress-bar");
                            // Set progress (example: 80%)
                            const newProgressBarValue = newProgress;
                            progressBar.style.width = newProgressBarValue + "%";
                            const badge = document.getElementById("status-badge-2024");
                            if (badge.textContent.trim() == "NEW") {
                                const year = 2024;
                                badge.classList.remove("bg-info", "text-dark");
                                badge.classList.add("bg-warning", "text-white");
                                badge.textContent = "DRAFT";
                                const header = document.getElementById("progressHeader");
                                // update text
                                header.textContent = `${year}/${year + 1} – Draft`;
                                // reset classes (keep "card-header")
                                header.className = "card-header bg-warning";
                                $("#statusBadge")
                                    .text("DRAFT")
                                    .removeClass("bg-info")
                                    .addClass("bg-warning text-dark");
                            }
                        }
                    }
                }
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