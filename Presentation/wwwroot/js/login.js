if (localStorage.getItem('email')) {
  document.getElementById('email').value = localStorage.getItem('email');
}

// if cookie has email, redirect to dashboard
if (document.cookie.includes('email')) {
  window.location.href = '/Dashboard';
}

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

var isEmailValid = true;
var isPasswordValid = true;

function validatePassword() {
  var password = $('#password').val();
  if (password.length <= 0) {
    isPasswordValid = false;
    $('#password-error').text('Password is required');
  } else if (!password.match(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/)) {
    isPasswordValid = false;
    $('#password-error').text('Password must contain at least one numeric digit, one uppercase and one lowercase letter, and at least 6 or more characters');
  } else {
    $('#password-error').text('');
    isPasswordValid = true;
  }
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

  // store email in local storage
  localStorage.setItem('email', $('#email').val());
});

$('#password').on('input', function () {
  validatePassword();
  $('#login-error').text('');
});

$(document).ready(function () {
  $('#login-form').submit(function (e) {
      e.preventDefault();

      validateEmail();
      validatePassword();

      if (!isEmailValid || !isPasswordValid) {
          return;
      }

      var email = $('#email').val();
      var password = $('#password').val();
      var rememberMe = $('#remember-me').is(':checked');

      $('.loader-container').removeClass('d-none');

      $.ajax({
          url: '/api/validate',
          type: 'POST',
          contentType: 'application/json',
          data: JSON.stringify({
              email: email,
              password: password
          }),
          success: function (response) {
              if (response.success) {
                  if (rememberMe) {
                      document.cookie = `email=${email}; max-age=${60 * 60 * 24 * 7}`;
                      document.cookie = `token=${response.token}; max-age=${60 * 60 * 24 * 7}`;
                  }
                  else {
                    document.cookie = `token=${response.token};`;
                  }
                  toastr.success(response.message);

                  setTimeout(function () {
                      window.location.href = '/Dashboard';
                      $('.loader-container').addClass('d-none');
                  }, 100);
                 
              } else {
                  toastr.error(response.message);
                  $('.loader-container').addClass('d-none'); 
              }
          },
          error: function (xhr) {
              console.log(xhr);
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
