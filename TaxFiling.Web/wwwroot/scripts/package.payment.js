$(function () {

    $(document).on('click', '#btnContinue', function () {

        const newTab = window.open('', '_blank');

        $.ajax({
            url: `${appUrl}/Payment/StartPayment`,
            type: "POST",
            contentType: "application/json",
            data: {},
            success: function (response) {

                newTab.location.href = response.redirectUrl;

              
            },
            error: function (xhr) {
                console.log(xhr);
            }
        });

    });
 

});