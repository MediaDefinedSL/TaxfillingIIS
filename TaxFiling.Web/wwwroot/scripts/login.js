

$(function () {
    $('#Username, #Password').on('keydown', function (e) {
        if (e.key === 'Enter' || e.which === 13) {
            e.preventDefault();
            $('#btnSignIn').click();
        }
    });


    $(document).on("click", "#btnSignIn", function () {

        var email = $('#Username').val();
        var Password = $('#Password').val();
        var returnUrl = $('#ReturnUrl').val();
        if (returnUrl.length == 0) {
            returnUrl = null;
        }

        $.ajax({
            url: `${appUrl}/Account/SignIn`,
            type: "POST",
            contentType: "application/json",
            data: JSON.stringify({
                Username: email,
                Password: Password,
                ReturnUrl: returnUrl // optionally pass a return URL
            }),
            success: function (response) {
                if (response.success) {
                    window.location.href = `${appUrl}${response.returnUrl}`; // Redirect after successful login
                   
                } else {
                    notifyError(false, 'Invalid email or password');
                }
            },
            error: function (xhr) {
                console.log(xhr);
            }
        });

    });

   

});