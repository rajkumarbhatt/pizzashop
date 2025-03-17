var emailStored = localStorage.getItem('email');
if (emailStored) {
  $('#email').val(emailStored);
}

function validateEmail() {
  var email = $('#email').val();
  if (email.length <= 0) {
    isEmailValid = false;
    $('#email-error').text('Email is required');
  } else if (!email.match(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/)) {
    isEmailValid = false;
    $('#email-error').text('Email is invalid');
  } else {
    $('#email-error').text('');
    isEmailValid = true;
  }
}

$('#email').on('input', function () {
  $('#login-error').text('');
  validateEmail();
});
 
var isEmailValid = true;

$(document).ready(function () {
  console.log('ready');

  $('#forgot-password-form').submit(function (e) {
    console.log('submit');

    e.preventDefault();
    validateEmail();
    if (!isEmailValid) {
      return;
    }
    var email = $('#email').val();

    $.ajax({
      url: '/api/forgotpassword',
      method: 'POST',
      contentType: 'application/json', 
      data: JSON.stringify({ email: email }), 
      success: function (response) {
        if (response.success) {
          toastr.success(response.message); 
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