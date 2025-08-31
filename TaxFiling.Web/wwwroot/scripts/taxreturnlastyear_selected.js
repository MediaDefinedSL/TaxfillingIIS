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
            alert("Please select a taxpayer.");
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
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });
});