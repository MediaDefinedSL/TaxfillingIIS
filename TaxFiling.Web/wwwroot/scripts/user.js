
$(function () {

    var istin = $("#isTin").val();

    //if (istin == 0) {

    //    let myModal = new bootstrap.Modal(document.getElementById('File-my-taxes'));
    //    myModal.show();
    //}

    $(document).on("click", "#btnAddUser", function (e) {
        e.preventDefault();

        var $btn = $(this);
        //$btn.setButtonDisabled(true);
        $btn.prop("disabled", true); // disable button

        //let formData = new FormData();

        let firstName = $("#FirstName").val();
        let lastName = $("#LastName").val();
        let email = $("#Email").val();
        let phone = $("#Phone").val();
        let password = $("#Password").val();
        let confirmPassword = $("#ConfirmPassword").val();

        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var phonePattern = /^(\+?\d{1,3}[- ]?)?\d{10}$/;

        var user = {
            FirstName: $("#FirstName").val(),
            LastName: $("#LastName").val(),
            Email: $("#Email").val(),
            Phone: $("#Phone").val(),
            Password: $("#Password").val()
        };
        if (firstName.length == 0) {
            notifyError(false, "First Name is required");
            // $btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (lastName.length == 0) {
            notifyError(false, "Last Name is required");
            //$btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (email.length == 0) {
            notifyError(false, "Email is required");
            //$btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (!emailPattern.test(email)) {
            notifyError(false, "Ivalid email");
            //$btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (phone.length == 0) {
            notifyError(false, "Phone is required");
            // $btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (!phonePattern.test(phone)) {
            notifyError(false, "Invalid phone number!");
            //$btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (password.length == 0) {
            notifyError(false, "Password is required");
            //  $btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        if (password.length < 6) {
            notifyError(false, "Password must be at least 6 characters.");
            //$btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
            
        }
        else if (confirmPassword.length == 0) {
            notifyError(false, "Confirm Password is required");
            // $btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else if (password !== confirmPassword) {
            notifyError(false, "Passwords do not match.");
            // $btn.setButtonDisabled(false);
            $btn.prop("disabled", false); // disable button
        }
        else {
            $.ajax({
                url: `${appUrl}/account/userregister`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(user),
                success: function (response) {
                    if (response.responseResult != null) {
                        console.log(response.responseResult);
                        if (response.responseResult.success) {
                            notifySuccess("", response.responseResult.message);
                            var userid = response.responseResult.resultGuid;
                            var name = response.responseResult.name;
                            var tinNo = response.responseResult.data.tinNo;
                            var nicno = response.responseResult.data.nicno;
                            console.log(response.responseResult.data);
                            console.log(userid);
                            $('#UserId').val(userid);
                            $('#UserFullName').val(name);

                            let myModal = new bootstrap.Modal(document.getElementById('File-my-taxes'));
                            myModal.show();

                        } else {
                            notifyError(false, response.responseResult.message);
                        }
                    }
                    else {
                        notifyError(false, 'An error occurred while registering the User.');
                    }

                    //$btn.setButtonDisabled(false);
                    $btn.prop("disabled", false); // disable button
                },
                error: function (xhr) {
                    //$btn.setButtonDisabled(false);
                    $btn.prop("disabled", false); // disable button
                    notifyError(false, 'An error occurred while registering the User.');
                }
            });
        }

    });

    $(document).on("click", "#btnUpdateUser", function (e) {
        e.preventDefault();

        var $btn = $(this);
       // $btn.setButtonDisabled(true);
        $btn.prop("disabled", true);

        let formData = new FormData();

        let userId = $("#UserId").val();
        let firstName = $("#FirstName").val();
        let lastName = $("#LastName").val();
        let email = $("#Email").val();
        let phone = $("#Phone").val();
        let password = $("#Password").val();
        let nicNo = $("#NICNO").val();
        let tinNO = $("#TinNo").val();
        //let confirmPassword = $("#ConfirmPassword").val();
        let imageFile = $("#customFile2")[0]?.files[0];
        let beforeProfileImagePath = $("#BeforeProfileImagePath").val();

        formData.append("UserId", userId);
        formData.append("FirstName", firstName);
        formData.append("LastName", lastName);
        formData.append("Email", email);
        formData.append("Phone", phone);
        formData.append("NICNO", nicNo);
        formData.append("TinNo", tinNO);
        formData.append("IsTin", 1);
        formData.append("BeforeProfileImagePath", beforeProfileImagePath);
        if (imageFile) {
            formData.append("ProfileImage", imageFile);
        }

        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var phonePattern = /^(\+?\d{1,3}[- ]?)?\d{10}$/;
        
        console.log(formData);
        if (firstName.length == 0) {
            notifyError(false, "First Name is required");
            $btn.prop("disabled", false);
          //  $btn.setButtonDisabled(false);
        }
        else if (lastName.length == 0) {
            notifyError(false, "Last Name is required");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else if (email.length == 0) {
            notifyError(false, "Email is required");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else if (!emailPattern.test(email)) {
            notifyError(false, "Ivalid email");
            $btn.prop("disabled", false);
          //  $btn.setButtonDisabled(false);
        }
        else if (phone.length == 0) {
            notifyError(false, "Phone is required");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else if (!phonePattern.test(phone)) {
            notifyError(false, "Invalid phone number!");
            $btn.prop("disabled", false);
            //$btn.setButtonDisabled(false);
        }
        else if (password.length == 0) {
            notifyError(false, "Password is required");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else if (nicNo.length == 0) {
            notifyError(false, "NIC No  is required");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else if (tinNO.length == 0) {
            notifyError(false, "Tin No  is required");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else if (!validateNIC(nicNo)) {
            notifyError(false, "Invalid NIC number.");
            $btn.prop("disabled", false);
           // $btn.setButtonDisabled(false);
        }
        else {

            $.ajax({
                url: `${appUrl}/user/userupdate`,
                type: "PUT",
                contentType: false,
                processData: false,
                data: formData,
                success: function (response) {
                    if (response.responseResult != null) {
                        if (response.responseResult.success) {
                            notifySuccess("", response.responseResult.message);
                            window.location.href = `${appUrl}/home/FileMyTaxes`;

                        } else {
                            notifyError(false, response.responseResult.message);
                        }
                    }
                    else {
                        notifyError(false, 'An error occurred while updating the User.');
                    }

                    // $btn.setButtonDisabled(false);
                    $btn.prop("disabled", false);
                },
                error: function (xhr) {
                    //  $btn.setButtonDisabled(false);
                    $btn.prop("disabled", false);
                    notifyError(false, 'An error occurred while updating the User.');
                }
            });
        }

    });

    $(document).on("click", "#btnRegisterUserTinYesStatus", function (e) {
        e.preventDefault();

        var $btn = $(this);
        //$btn.setButtonDisabled(true);
        $btn.prop("disabled", true); // disable button
        var userId = $('#UserId').val();
        var name = $('#UserFullName').val();
        var email = $('#Email').val();
        var Password = $('#Password').val();
        console.log("UserId:", userId);


        $.ajax({
            url: `${appUrl}/account/updateusertinstatus`,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                userId: userId,
                tinStatus: 1
            }),
            success: function (response) {
                console.log(response.responseResult);
                if (response.responseResult != null) {
                    if (response.responseResult.success) {
                        notifySuccess("", response.responseResult.message);

                        // Now sign in the user
                        $.ajax({
                            url: `${appUrl}/account/signin`,
                            type: "POST",
                            contentType: "application/json",
                            data: JSON.stringify({
                                Username: email,
                                Password: Password,
                                ReturnUrl: null // optionally pass a return URL
                            }),
                            success: function (signInResponse) {
                                console.log(signInResponse);
                                if (signInResponse.success) {

                                    window.location.href = `${appUrl}${signInResponse.returnUrl}`;
                                } else {
                                    notifyError(false, "Sign-in failed after TIN status update.");
                                }
                            },
                            error: function () {
                                notifyError(false, "Error occurred during sign-in.");
                            }
                        });

                    } else {
                        notifyError(false, response.responseResult.message);
                    }
                }
                else {
                    notifyError(false, 'An error occurred while updating the User.');
                }

                //$btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
            },
            error: function (xhr) {
                //$btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
                notifyError(false, 'An error occurred while updating the User.');
            }
        });

    });

    $(document).on("click", "#btnRegisterUserTinNoStatus", function (e) {
        e.preventDefault();

        var $btn = $(this);
        //$btn.setButtonDisabled(true);
        $btn.prop("disabled", true); // disable button
        var userId = $('#UserId').val();
        var name = $('#UserFullName').val();
        var email = $('#Email').val();
        var Password = $('#Password').val();
        console.log("UserId:", userId);

        $.ajax({
            url: `${appUrl}/account/updateusertinstatus`,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                userId: userId,
                tinStatus: 0
            }),
            success: function (response) {
                if (response.responseResult != null) {
                    if (response.responseResult.success) {
                        notifySuccess("", response.responseResult.message);


                        // Now sign in the user
                        $.ajax({
                            url: `${appUrl}/account/signin`,
                            type: "POST",
                            contentType: "application/json",
                            data: JSON.stringify({
                                Username: email,
                                Password: Password,
                                ReturnUrl: null // optionally pass a return URL
                            }),
                            success: function (signInResponse) {
                                console.log(signInResponse);
                                if (signInResponse.success) {

                                    window.location.href = `${appUrl}/home/notinnumber`;
                                    //  window.location.href = `${appUrl}${signInResponse.returnUrl}`;
                                } else {
                                    notifyError(false, "Sign-in failed after TIN status update.");
                                }
                            },
                            error: function () {
                                notifyError(false, "Error occurred during sign-in.");
                            }
                        });

                    } else {
                        notifyError(false, response.responseResult.message);
                    }
                }
                else {
                    notifyError(false, 'An error occurred while updating the User.');
                }
                $btn.prop("disabled", false); // disable button
               // $btn.setButtonDisabled(false);
            },
            error: function (xhr) {
                //$btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
                notifyError(false, 'An error occurred while updating the User.');
            }
        });

    });

    $(document).on("click", "#btnUserTinYesStatus", function (e) {
        e.preventDefault();

        var $btn = $(this);
        // $btn.setButtonDisabled(true);
        $btn.prop("disabled", true); // disable button
        var userId = $('#UserId').val();
        var name = $('#UserFullName').val();
        console.log("UserId:", userId);

        $.ajax({
            url: `${appUrl}/account/updateusertinstatus`,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                userId: userId,
                tinStatus: 1
            }),
            success: function (response) {
                if (response.responseResult != null) {
                    if (response.responseResult.success) {
                        notifySuccess("", response.responseResult.message);
                        var name = $('#UserFullName').val();
                        /*  window.location.href = `${appUrl}/home/FileMyTaxes`;*/
                        // window.location.href = `${appUrl}${response.responseResult.returnUrl}`;

                    } else {
                        notifyError(false, response.responseResult.message);
                    }
                }
                else {
                    notifyError(false, 'An error occurred while updating the User.');
                }

                //$btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
            },
            error: function (xhr) {
                //  $btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
                notifyError(false, 'An error occurred while updating the User.');
            }
        });

    });

    $(document).on("click", "#btnUserTinNoStatus", function (e) {
        e.preventDefault();

        var $btn = $(this);
        //$btn.setButtonDisabled(true);
        $btn.prop("disabled", true); // disable button
        var userId = $('#UserId').val();
        var name = $('#UserFullName').val();
        console.log("UserId:", userId);

        $.ajax({
            url: `${appUrl}/account/updateusertinstatus`,
            type: "PUT",
            contentType: "application/json",
            data: JSON.stringify({
                userId: userId,
                tinStatus: 0
            }),
            success: function (response) {
                if (response.responseResult != null) {
                    if (response.responseResult.success) {
                        notifySuccess("", response.responseResult.message);
                        // window.location.href = `${appUrl}/home/notinnumber`;
                        window.location.href = `${appUrl}${response.responseResult.returnUrl}`;

                    } else {
                        notifyError(false, response.responseResult.message);
                    }
                }
                else {
                    notifyError(false, 'An error occurred while updating the User.');
                }

                //$btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
            },
            error: function (xhr) {
                // $btn.setButtonDisabled(false);
                $btn.prop("disabled", false); // disable button
                notifyError(false, 'An error occurred while updating the User.');
            }
        });

    });

    $(document).on("click", "#btnReset", function (e) {
        var userId = $('#UserId').val();
        $('#resetUserId').val(userId);
        $('#resetPasswordModal').modal('show');
    });


    $('#resetPasswordForm').on('submit', function (e) {
        e.preventDefault();

        var newPassword = $('#newPassword').val();
        var confirmPassword = $('#confirmPassword').val();
        var userId = $('#resetUserId').val();
        alert(userId);

        if (!userId) {
            alert('User identification error. Please try again.');
            return;
        }


    });

});

function validateNIC(nic) {
    var oldNICPattern = /^[0-9]{9}[vVxX]$/;
    var newNICPattern = /^[0-9]{12}$/;

    if (oldNICPattern.test(nic) || newNICPattern.test(nic)) {
        return true;
    }
    return false;
}


//function removeImage() {

//    const fileInput = document.getElementById('ProfileImage');
//    const fileUpload = document.getElementById('fileUpload');
//    const previewImage = document.getElementById('previewImage');
//    previewContainer.style.display = 'none';
//    fileUpload.style.display = 'block';
//    fileInput.value = '';
//    previewImage.src = '';
//}
function displaySelectedImage(event, elementId) {
    const selectedImage = document.getElementById(elementId);
    const fileInput = event.target;

    if (fileInput.files && fileInput.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            selectedImage.src = e.target.result;
        };

        reader.readAsDataURL(fileInput.files[0]);
    }
}
