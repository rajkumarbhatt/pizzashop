$(document).ready(function () {
  $("#select-country").change(function () {
    try {
      $("#select-state").empty();
      $("#select-city").empty();
      $("#select-state").append($("<option>").text("Select State").val(""));
      $("#select-city").append($("<option>").text("Select City").val(""));

      var countryId = $(this).val();
      $.ajax({
        url: "/Profile/GetStates",
        type: "GET",
        data: { countryId: countryId },
        success: function (data) {
          $.each(data, function (i, state) {
            $("#select-state").append(
              $("<option>").text(state.name).val(state.id)
            );
          });
        },
        error: function () {
          toastr.error("Failed to fetch states.");
        },
      });
    } catch (error) {
      console.error("Error in #select-country change handler:", error);
      toastr.error("An unexpected error occurred.");
    }
  });

  $("#select-state").change(function () {
    try {
      var stateId = $(this).val();
      $.ajax({
        url: "/Profile/GetCities",
        type: "GET",
        data: { stateId: stateId },
        success: function (data) {
          $("#select-city").empty();
          $("#select-city").append($("<option>").text("Select City").val(""));
          $.each(data, function (i, city) {
            $("#select-city").append($("<option>").text(city.name).val(city.id));
          });
        },
        error: function () {
          toastr.error("Failed to fetch cities.");
        },
      });
    } catch (error) {
      console.error("Error in #select-state change handler:", error);
      toastr.error("An unexpected error occurred.");
    }
  });

  document.getElementById("upload-btn").addEventListener("change", (e) => {
    try {
      const file = e.target.files[0];
      if (file && !file.type.startsWith("image/")) {
        toastr.error("Please select an image file.");
        return;
      }
      if (file) {
        document.getElementById("file-name").textContent = file.name;
      }
    } catch (error) {
      console.error("Error in file upload handler:", error);
      toastr.error("An unexpected error occurred during file upload.");
    }
  });

  $("#create-user-form").submit(function (e) {
    e.preventDefault();
    try {
      if (!$(this).valid()) {
        return;
      }
      var form = $(this)[0];
      var formData = new FormData(form);
      $(".loader-container").removeClass("d-none");
      var userId = $("#userid").val();
      $.ajax({
        url: "/UserList/CreateUser",
        type: "POST",
        data: formData,
        processData: false,
        contentType: false,
        success: function (data) {
          if (data.success) {
            toastr.success(data.message);
            setTimeout(function () {
              window.location.href = "/UserList";
              $(".loader-container").addClass("d-none");
            }, 1000);
          } else {
            toastr.error(data.message);
            $(".loader-container").addClass("d-none");
          }
        },
        error: function () {
          toastr.error("An error occurred while processing your request");
          $(".loader-container").addClass("d-none");
        },
      });
    } catch (error) {
      console.error("Error in form submission handler:", error);
      toastr.error("An unexpected error occurred during form submission.");
      $(".loader-container").addClass("d-none");
    }
  });
});

function showAndHidePassword(flag) {
  try {
    if (flag) {
      document.getElementById("password").type = "text";
      document.getElementById("show-current-password").classList.add("d-none");
      document.getElementById("hide-current-password").classList.remove("d-none");
    } else {
      document.getElementById("password").type = "password";
      document.getElementById("show-current-password").classList.remove("d-none");
      document.getElementById("hide-current-password").classList.add("d-none");
    }
  } catch (error) {
    console.error("Error in showAndHidePassword function:", error);
    toastr.error("An unexpected error occurred while toggling the password visibility.");
  }
}
