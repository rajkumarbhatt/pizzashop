function showAndHidePassword(flag) {
    const password = document.getElementById('password');
    const showPasswordIcon = document.getElementById('show-password');
    const hidePasswordIcon = document.getElementById('hide-password');
    if (flag) {
        password.type = 'text';
        showPasswordIcon.classList.add('d-none');
        hidePasswordIcon.classList.remove('d-none');
    } else {
        password.type = 'password';
        showPasswordIcon.classList.remove('d-none');
        hidePasswordIcon.classList.add('d-none');
    }
}

function showAndHideConfirmPassword(flag) {
    const password = document.getElementById('confirm-password');
    const showPasswordIcon = document.getElementById('show-confirm-password');
    const hidePasswordIcon = document.getElementById('hide-confirm-password');
    if (flag) {
        password.type = 'text';
        showPasswordIcon.classList.add('d-none');
        hidePasswordIcon.classList.remove('d-none');
    } else {
        password.type = 'password';
        showPasswordIcon.classList.remove('d-none');
        hidePasswordIcon.classList.add('d-none');
    }
}

// var isPasswordValid = true;
// function validatePassword() {
//     var password = $('#password').val();
//     if (password.length <= 0) {
//         isPasswordValid = false;
//         $('#password-error').text('Password is required');
//     } else if (!password.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)) {
//         isPasswordValid = false;
//         $('#password-error').text('Password must contain at least one numeric digit, one uppercase and one lowercase letter, and at least 6 or more characters');
//     } else {
//         $('#password-error').text('');
//         isPasswordValid = true;
//     }
// }

// var isConfirmPasswordValid = true;
// function validateConfirmPassword() {
//     var confirmPassword = $('#confirm-password').val();
//     if (confirmPassword.length <= 0) {
//         isConfirmPasswordValid = false;
//         $('#confirm-password-error').text('Confirm Password is required');
//     } else if (!confirmPassword.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$/)) {
//         isConfirmPasswordValid = false;
//         $('#confirm-password-error').text('Confirm Password must contain at least one numeric digit, one uppercase and one lowercase letter, and at least 6 or more characters');
//     } else if (confirmPassword !== $('#password').val()) {
//         isConfirmPasswordValid = false;
//         $('#confirm-password-error').text('Confirm Password must match Password');
//     } else {
//         $('#confirm-password-error').text('');
//         isConfirmPasswordValid = true;
//     }
// }

// $('#password').on('input', function () {
//     validatePassword();
// });

// $('#confirm-password').on('input', function () {
//     validateConfirmPassword();
// });
$(document).ready(function () {
    console.log('ready');

    $('#reset-password-form').submit(function (e) {
        console.log('submit');

        e.preventDefault();
        var password = $('#password').val();
        var confirmPassword = $('#confirm-password').val();
        var token = $('#token').val();
        var userid = $('#userid').val();

        $.ajax({
            url: '/api/resetpassword',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ NewPassword: password, Token: token, Userid: userid, ConfirmPassword: confirmPassword }),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        window.location.href = '/Home';
                    }, 2000);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                console.log(xhr);
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    toastr.error(xhr.responseJSON.message);
                } else {
                    toastr.error('An unexpected error occurred.');
                }
            }
        });
    });
});

$(document).ready(function () {
    

    $('#reset-password-first-time-form').submit(function (e) {

        e.preventDefault();
        var password = $('#password').val();
        var confirmPassword = $('#confirm-password').val();
        console.log('submit');

        $.ajax({
            url: '/ResetPassword/NewPassword',
            method: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ NewPassword: password, ConfirmPassword: confirmPassword }),
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    setTimeout(function () {
                        window.location.href = '/Dashboard';
                    }, 2000);
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (xhr) {
                console.log(xhr);
                if (xhr.responseJSON && xhr.responseJSON.message) {
                    toastr.error(xhr.responseJSON.message);
                } else {
                    toastr.error('An unexpected error occurred.');
                }
            }
        });
    });
}
);