$(function () {

    $('#linkTaxPayerContinue').on('click', function () {

        var selectedId = $("input[name='imgbackground']:checked").val();

        if (selectedId) {
            console.log("Selected MaritalStatus ID: " + selectedId);
            $.ajax({
                url: '/SelfOnlineFlow/UpdateMaritalStatus',
                type: 'PUT',
                data: { maritalStatusId: selectedId },
                success: function (response) {
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
                },
                error: function () {
                    alert("Error saving taxpayer.");
                }
            });

        } else {
            alert("Please select a taxpayer.");
        }
    });

    $('#linkTaxPayerNext').on('click', function () {
      
        $.ajax({
            url: '/SelfOnlineFlow/LoadTaxPayer',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkTaxPayer').addClass('active');
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });
});