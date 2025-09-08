$(function () {

    $('#divSummaryAssistedContinue').on('click', function () {
        var docUploadStatus = $('#docUploadStatus').val();
        if (docUploadStatus) {

            $('#in-this-section-container').hide();
            $('#linkSummary').removeClass('active');
            $('.dropdown-btn').each(function () {
                var dropdownContent = this.nextElementSibling;
                if (dropdownContent && dropdownContent.classList.contains("dropdown-container")) {
                    dropdownContent.style.display = "none";
                }
            });
            showPanel("documents-section");
        }
        else {
            $('#in-this-section-container').show();
            $.ajax({
                url: '/SelfOnlineFlow/LoadContactInformation',
                type: 'GET',
                success: function (data) {
                    $('#in-this-section-container').html(data);
                    $('.sub-link').removeClass('active');
                    $('#linkContactInformation').addClass('active');
                    $("html, body").animate({ scrollTop: 0 }, "smooth");
                },
                error: function () {
                    alert("Error loading section content.");
                }
            });
        }
        

    });

    $('#divSummaryAssistedNext').on('click', function () {

        var docUploadStatus = $('#docUploadStatus').val();
        if (docUploadStatus) {
           
            $('#in-this-section-container').hide();
            $('#linkSummary').removeClass('active');
            $('.dropdown-btn').each(function () {
                var dropdownContent = this.nextElementSibling;
                if (dropdownContent && dropdownContent.classList.contains("dropdown-container")) {
                    dropdownContent.style.display = "none";
                }
            });
            showPanel("documents-section");
        }
        else {
            $('#in-this-section-container').show();

            $.ajax({
                url: '/SelfOnlineFlow/LoadEmploymentIncome',
                type: 'GET',
                success: function (data) {
                    $('#in-this-section-container').html(data);
                    $('#linkSummary').removeClass('active');
                    $('.linkIncomeLiableTax').addClass('active');

                    $("html, body").animate({ scrollTop: 0 }, "smooth");
                },
                error: function () {
                    alert("Error loading section content.");
                }
            });
        }




    });
    $('#divSummaryAssistedBack').on('click', function () {

        $('#in-this-section-container').show();

        $.ajax({
            url: '/SelfOnlineFlow/LoadContactInformation',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $("html, body").animate({ scrollTop: 0 }, "smooth");
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });

    $('#linkTaxPayerContinue').on('click', function () {
        $('#linkSummary').removeClass('active');
        $('.dropdown-btn').each(function () {
            var dropdownContent = this.nextElementSibling;
            if (dropdownContent && dropdownContent.classList.contains("dropdown-container")) {
                dropdownContent.style.display = "none";
            }
        });
        showPanel("documents-section");



    });

});