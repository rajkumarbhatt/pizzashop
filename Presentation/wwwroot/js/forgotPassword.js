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

  $('#forgot-password-form').submit(function (e) {

    e.preventDefault();
    validateEmail();
    if (!isEmailValid) {
      return;
    }
    var email = $('#email').val();
    $('.loader-container').removeClass('d-none');
    $.ajax({
      url: '/api/forgotpassword',
      method: 'POST',
      contentType: 'application/json', 
      data: JSON.stringify({ email: email }), 
      success: function (response) {
        if (response.success) {
          toastr.success(response.message); 
          setTimeout(function () {
            window.location.href = '/Home';
            $('.loader-container').addClass('d-none');
          }, 1000);
        } else {
          toastr.error(response.message); 
          $('.loader-container').addClass('d-none');
        }
      },
      error: function (xhr) {
        if (xhr.responseJSON && xhr.responseJSON.message) {
          toastr.error(xhr.responseJSON.message); 
        } else {
          toastr.error('An unexpected error occurred.'); 
        }
        $('.loader-container').addClass('d-none');
      }
    });
  });
});