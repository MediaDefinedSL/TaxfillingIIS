$(function () {

    $('#linkLastyearContinue').on('click', function () {

        var selectedId = $("input[name='imgbackground']:checked").val();

        if (selectedId) {
            console.log("Selected LastYear ID: " + selectedId);
            $.ajax({
                url: '/SelfOnlineFlow/UpdateLastYear',
                type: 'PUT',
                data: { taxReturnlastyearId: selectedId },
                success: function (response) {
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
                },
                error: function () {
                    alert("Error saving taxpayer.");
                }
            });

        } else {
            showMessage("Please select a last year filling option.","error");
        }
    });

    $('#linkLastyearNext').on('click', function () {
       
        $.ajax({
            url: '/SelfOnlineFlow/LoadMaritalStatus',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);

                $('.sub-link').removeClass('active');
                $('#linkMaritalStatus').addClass('active');
                $("html, body").animate({ scrollTop: 0 }, "smooth");
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });
});