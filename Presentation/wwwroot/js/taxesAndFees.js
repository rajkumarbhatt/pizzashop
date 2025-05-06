function openAddTaxModal() {
    $(".text-danger").text("");
    $("#addTax").modal("show");
}
function addTax () {
    if (!$("#AddTaxFormModal").valid()) {
        return;
    }
    var taxId = $("#TaxId").val();
    var taxName = $("#TaxName").val();
    var isEnabled = $("#IsEnabled").is(":checked");
    var taxType = $("#TaxType").val();
    var taxAmount = $("#TaxAmount").val();
    data = {
        TaxId: taxId,
        TaxName: taxName,
        IsEnabled: isEnabled,
        TaxType: taxType,
        TaxAmount: taxAmount
    };
    $.ajax({
        type: "POST",
        url: '/TaxAndFee/AddTax',
        data: { addTaxViewModal: data },
        success: function (data) {
            console.log(data);
            if (data.success) {
                toastr.success(data.message);
                $.ajax({
                    type: "GET",
                    url: '/TaxAndFee/GetTaxes',
                    success: function (data) {
                        $("#partialViewStarting").html(data);
                        clearAddTaxModal();
                    }
                });
                $("#addTax").modal("hide");
            } else {
                toastr.error(data.message);
            }
        },
        error: function (data) {
            console.log(data);
        }
    });
}

$("#searchInput").on("keyup", function () {
    var value = $(this).val().toLowerCase();
    clearTimeout($.data(this, 'timer'));
    $(this).data('timer', setTimeout(function () {
        $.ajax({
            type: "GET",
            url: '/TaxAndFee/Search',
            data: { searchValue : value },
            success: function (data) {
                $("#partialViewStarting").html(data);
            }
        });
    }, 300));
});


function clearAddTaxModal() {
    $("#TaxName").val("");
    $("#TaxType").val("Percentage");
    $("#TaxAmount").val("0");
    $("#TaxId").val("-1");
    $("#IsEnabled").prop("checked", false);
    $("#addTaxModalTitle").text("Add Tax");
}



function deleteTax(id) {
    $("#deleteTax").modal("show");
    $("#deleteTaxButton").off("click").click(function () {
        $.ajax({
            type: "DELETE",
            url: "/TaxAndFee/Delete",
            data: { id: id },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    $.ajax({
                        type: "GET",
                        url: "/TaxAndFee/GetTaxes",
                        success: function (data) {
                            $("#partialViewStarting").html(data);
                        }
                    });
                } else {
                    toastr.error("Unauthorized Access");
                }
            },
            error: function (data) {
                toastr.error("Unauthorized Access");
            }
        });
    });
}