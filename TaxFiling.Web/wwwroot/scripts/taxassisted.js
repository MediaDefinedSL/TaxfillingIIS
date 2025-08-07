
$(function () {
    $(document).on("click", "#btnSelfEmployed", function (e) {
        e.preventDefault();

        var $btn = $(this);
        $btn.setButtonDisabled(true);
        var userId = $('#UserId').val();
        alert("userId -" + userId);

        if (userId.length > 0) {
            window.location.href = `${appUrl}/home/TaxAssistedPayment`;
        }
        else {

        }

    });
});