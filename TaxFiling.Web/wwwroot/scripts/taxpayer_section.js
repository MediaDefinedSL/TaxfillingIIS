$(function () {
   
    $('#linkTaxPayerContinue').on('click', function () {
       
        var selectedId = $("input[name='imgbackground']:checked").val();

        if (selectedId) {
            console.log("Selected TaxPayer ID: " + selectedId);
            $.ajax({
                url: '/SelfOnlineFlow/UpdateTaxPayerID',
                type: 'PUT',
                data: { taxPayerId: selectedId },
                success: function (response) {
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
            url: '/SelfOnlineFlow/LoadInThisSection',
            type: 'GET',
            success: function (data) {
                $('#in-this-section-container').html(data);
                $('.sub-link').removeClass('active');
                $('#linkInThisSection').addClass('active');
            },
            error: function () {
                alert("Error loading section content.");
            }
        });

    });

});