function closeModal(modalId) {
  $("#" + modalId).modal("hide");
  $("#modifierGroupdata").html("");
}

document.querySelectorAll(".permission-checkbox").forEach((checkbox) => {
  checkbox.addEventListener("change", () => {
    var tbodyCheckbox = document.querySelectorAll(
      ".permission-checkbox"
    ).length;
    var tbodyCheckedbox = document.querySelectorAll(
      ".permission-checkbox:checked"
    ).length;
    if (tbodyCheckbox === tbodyCheckedbox) {
      // All selected
      parentCheckbox.indeterminate = false;
      parentCheckbox.checked = true;
    } else if (tbodyCheckedbox > 0) {
      // Some selected
      parentCheckbox.indeterminate = true;
      parentCheckbox.checked = false;
    } else {
      // None selected
      parentCheckbox.indeterminate = false;
      parentCheckbox.checked = false;
    }
  });
});

document.getElementById("ImageItem").addEventListener("change", (e) => {
  const file = e.target.files[0];
  if (file && !file.type.startsWith("image/")) {
    toastr.error("Please select an image file.");
    return;
  }
  if (file) {
    const file = e.target.files[0];
    document.getElementById("file-name-item").textContent = file.name;
  }
});
