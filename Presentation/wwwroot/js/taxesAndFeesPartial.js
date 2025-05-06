function editTax(id) {
    $.ajax({
        type: "GET",
        url: "/TaxAndFee/Edit",
        data: { id: id },
        success: function (data) {
            $("#TaxName").val(data.tax.name);
            $("#TaxType").val(data.tax.taxType);
            $("#TaxAmount").val(data.tax.amount);
            $("#TaxId").val(data.tax.id);
            $("#IsEnabled").prop("checked", data.tax.isEnabled);
            $("#addTaxModalTitle").text("Edit Tax");
            $('.text-danger').text("");
            $("#addTax").modal("show");
        },
        error: function (data) {
            toastr.error("Unauthorized Access");
        }
    });
}

function saveChangesOfIsEnabled(id) {
    var isEnabled = document.getElementById("isEnabled" + id).checked;
    $.ajax({
        type: "POST",
        url: "/TaxAndFee/SaveChangesOfIsEnabled",
        data: { id: id, isEnabled: isEnabled },
        success: function (data) {
            console.log(data);
        },
        error: function (data) {
            console.log(data);
        }
    });
}