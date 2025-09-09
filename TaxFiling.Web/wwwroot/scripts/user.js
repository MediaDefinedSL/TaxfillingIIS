
$(function () {

    var istin = $("#isTin").val();

    $(document).on("input", "#TinNo", function () {
        this.value = this.value.replace(/[^0-9]/g, '');
    });

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
        let isValid = true;

        var emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        var phonePattern = /^(\+?\d{1,3}[- ]?)?\d{10}$/;

        var user = {
            FirstName: $("#FirstName").val(),
            LastName: $("#LastName").val(),
            Email: $("#Email").val(),
            Phone: $("#Phone").val(),
            Password: $("#Password").val()
        };


        // Remove old validation messages
        $(".validation-error").remove();

        if (!firstName.trim()) {
            $("#FirstName").after('<div class="text-danger validation-error">First Name is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!lastName.trim()) {
            $("#LastName").after('<div class="text-danger validation-error">Last Name is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!email.trim()) {
            $("#Email").after('<div class="text-danger validation-error">Email is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        if (!password.trim()) {
            $("#Password").after('<div class="text-danger validation-error">Password is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        else {
            if (password.length < 6) {
                $("#Password").after('<div class="text-danger validation-error">Password must be at least 6 characters.</div>');
                $btn.prop("disabled", false);
                isValid = false;
            }
            else {
                if (password !== confirmPassword) {
                    $("#Password").after('<div class="text-danger validation-error">Passwords do not match.</div>');
                    $btn.prop("disabled", false);
                    isValid = false;
                }
            }
        }
        if (!phone.trim()) {
            $("#Phone").after('<div class="text-danger validation-error">Phone Number is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
        else {
            if (!phonePattern.test(phone)) {
                $("#Phone").after('<div class="text-danger validation-error">Invalid Phone number!</div>');
                $btn.prop("disabled", false);
                isValid = false;
            }
        }
 
        if (!confirmPassword) {
            $("#ConfirmPassword").after('<div class="text-danger validation-error">Confirm Password is required.</div>');
            $btn.prop("disabled", false);
            isValid = false;
        }
       

        if (!isValid) {
            return;
        }

      
            $.ajax({
                url: `${appUrl}/account/userregister`,
                type: "POST",
                contentType: "application/json",
                data: JSON.stringify(user),
                success: function (response) {
                    if (response.responseResult != null) {
                        console.log(response.responseResult);
                        if (response.responseResult.success) {
                         
                            showMessage(response.responseResult.message, "success");

                            var userid = response.responseResult.resultGuid;
                            var name = response.responseResult.name;
                            var tinNo = response.responseResult.data.tinNo;
                            var nicno = response.responseResult.data.nicno;
                            console.log(response.responseResult.data);
                            console.log(userid);
                            $('#UserId').val(userid);
                            $('#UserFullName').val(name);

                            setTimeout(function () {
                                $('#mytaxes_tin').modal('show');
                            }, 1500);

                        } else {
                            showMessage(response.responseResult.message, "error");
                        }
                    }
                    else {
                        showMessage('An error occurred while registering the User.', "error");  
                    }

                    $btn.prop("disabled", false); 
                },
                error: function (xhr) {

                    $btn.prop("disabled", false); 
                    showMessage('An error occurred while registering the User.', "error");  
                }
            });
      

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
                    console.log(response);
                    if (response.responseResult != null) {
                        if (response.responseResult.success) {
                           
                            showMessage(response.responseResult.message, "success");

                            var imagePath = appUrl + response.user.profileImagePath;
                            console.log(imagePath);
                          //  $("#btnprofileimage").css("background-image", "url('" + imagePath + "')");
                            window.location.href = `${appUrl}/home/FileMyTaxes`;
                           

                        } else {
                          
                            showMessage(response.responseResult.message, "error");
                        }
                    }
                    else {
                        notifyError(false, 'An error occurred while updating the User.');
                    }

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
                       // notifySuccess("", response.responseResult.message);
                        showMessage(response.responseResult.message, "success");
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
                       // notifySuccess("", response.responseResult.message);
                        showMessage("Populate Name and email and Subject as Help me to create TIN Number", "success");
                       
                        setTimeout(function () {
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

                                    window.location.href = `${appUrl}/home/ContactUs`;
                                  
                                } else {
                                   // notifyError(false, "Sign-in failed after TIN status update.");
                                    showMessage("Sign-in failed after TIN status update.", "error");
                                }
                            },
                            error: function () {
                                showMessage("Error occurred during sign-in.", "error");
                               // notifyError(false, "Error occurred during sign-in.");
                            }
                       });
                        }, 1500); 

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
                        //notifySuccess("", response.responseResult.message);
                        showMessage(response.responseResult.message, "success");
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

function showMessage(message, type = "success", autoClose = true, autoCloseTime = 5000) {
    const modal = document.getElementById("enhancedModalUser");
    const modalContent = modal.querySelector(".enhanced-modal-content");
    const msgElem = document.getElementById("enhancedModalMessageUser");
    const closeBtn = document.getElementById("enhancedModalCloseUser");

    // Set message text
    msgElem.textContent = message;

    // Remove previous type classes
    modalContent.classList.remove("modal-success", "modal-error", "modal-info");

    // Add current type class
    if (type === "success") modalContent.classList.add("modal-success");
    else if (type === "error") modalContent.classList.add("modal-error");
    else modalContent.classList.add("modal-info");

    // Show modal
    modal.style.display = "flex";

    // Close function
    function closeModal() {
        modal.style.display = "none";
        // Cleanup event listeners to avoid duplicates if called again
        closeBtn.removeEventListener("click", closeModal);
        window.removeEventListener("click", outsideClick);
        if (autoCloseTimer) clearTimeout(autoCloseTimer);
    }

    // Close when clicking close button
    closeBtn.addEventListener("click", closeModal);

    // Close when clicking outside modal content
    function outsideClick(e) {
        if (e.target === modal) closeModal();
    }
    window.addEventListener("click", outsideClick);

    // Auto-close timer if enabled
    let autoCloseTimer;
    if (autoClose) {
        autoCloseTimer = setTimeout(closeModal, autoCloseTime);
    }
}


