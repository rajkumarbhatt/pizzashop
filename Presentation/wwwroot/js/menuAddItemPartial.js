function closeModal(modalId) {
  try {
    $("#" + modalId).modal("hide");
    $("#modifierGroupdata").html("");
  } catch (error) {
    console.error("Error in closeModal:", error);
  }
}

document.querySelectorAll(".permission-checkbox").forEach((checkbox) => {
  checkbox.addEventListener("change", () => {
    try {
      var tbodyCheckbox = document.querySelectorAll(".permission-checkbox").length;
      var tbodyCheckedbox = document.querySelectorAll(".permission-checkbox:checked").length;
      if (tbodyCheckbox === tbodyCheckedbox) {
        parentCheckbox.indeterminate = false;
        parentCheckbox.checked = true;
      } else if (tbodyCheckedbox > 0) {
        parentCheckbox.indeterminate = true;
        parentCheckbox.checked = false;
      } else {
        parentCheckbox.indeterminate = false;
        parentCheckbox.checked = false;
      }
    } catch (error) {
      console.error("Error in permission-checkbox change event:", error);
    }
  });
});

document.getElementById("ImageItem").addEventListener("change", (e) => {
  try {
    const file = e.target.files[0];
    if (file && !file.type.startsWith("image/")) {
      toastr.error("Please select an image file.");
      return;
    }
    if (file) {
      const file = e.target.files[0];
      document.getElementById("file-name-item").textContent = file.name;
    }
  } catch (error) {
    console.error("Error in ImageItem change event:", error);
  }
});
