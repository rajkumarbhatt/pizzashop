function showAndHidePassword(flag) {
    if (flag) {
        document.getElementById("current-password").type = "text";
        document.getElementById("show-current-password").classList.add("d-none");
        document.getElementById("hide-current-password").classList.remove("d-none");
    } else {
        document.getElementById("current-password").type = "password";
        document.getElementById("show-current-password").classList.remove("d-none");
        document.getElementById("hide-current-password").classList.add("d-none");
    }
}

function showAndHideNewPassword(flag) {
    if (flag) {
        document.getElementById("new-password").type = "text";
        document.getElementById("show-new-password").classList.add("d-none");
        document.getElementById("hide-new-password").classList.remove("d-none");
    } else {
        document.getElementById("new-password").type = "password";
        document.getElementById("show-new-password").classList.remove("d-none");
        document.getElementById("hide-new-password").classList.add("d-none");
    }
}

function showAndHideConfirmPassword(flag) {
    if (flag) {
        document.getElementById("confirm-password").type = "text";
        document.getElementById("show-confirm-password").classList.add("d-none");
        document.getElementById("hide-confirm-password").classList.remove("d-none");
    } else {
        document.getElementById("confirm-password").type = "password";
        document.getElementById("show-confirm-password").classList.remove("d-none");
        document.getElementById("hide-confirm-password").classList.add("d-none");
    }
}

$("#cancel-button").click(function () {
    window.location.href = "/Dashboard";
});

$(document).ready(function () {
    $("#change-password-form").submit(function (e) {
        e.preventDefault();
        var currentPassword = $("#current-password").val();
        var newPassword = $("#new-password").val();
        var confirmPassword = $("#confirm-password").val();
        $('.loader-container').removeClass('d-none');
        $.ajax({
            type: "POST",
            url: "/account/changepassword",
            data: JSON.stringify({ CurrentPassword: currentPassword, NewPassword: newPassword, ConfirmPassword: confirmPassword }),
            contentType: "application/json",
            success: function (data) {
                console.log(data);
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function () {
                        window.location.href = "/Dashboard";
                        $('.loader-container').addClass('d-none');
                    }, 1000);
                } else {
                    toastr.error(data.message);
                    $('.loader-container').addClass('d-none');
                }
            },
            error: function (data) {
                console.log(data);
                toastr.error(data.message);
                $('.loader-container').addClass('d-none');
            }
        });
    });
});