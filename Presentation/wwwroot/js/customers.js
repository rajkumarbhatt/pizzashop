$(document).ready(function () {
    $('.datepicker').datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true
    }).on('show', function () {
        $(this).attr('readonly', true);
    });

    $('.datepicker').datepicker('setEndDate', new Date());

    $('#fromDate').on('change', function () {
        var fromDate = $(this).datepicker('getDate');
        $('#toDate').datepicker('setStartDate', fromDate);
    });

    $('#toDate').on('change', function () {
        var toDate = $(this).datepicker('getDate');
        $('#fromDate').datepicker('setEndDate', toDate);
    });

    $("#cancelDatePickerButton").click(function () {
        $("#fromDate").val("dd-mm-yyyy");
        $("#toDate").val("dd-mm-yyyy");
        $.ajax({
            url: '/Customer/FilterCustomers',
            type: "GET",
            data: { pageIndex: 1, pageSize: $("#pageSizeSelectOrders").val(), searchValue: $("#searchInputCustomers").val().toLowerCase(), time: $("#CustomersTimeSelect").val(), sort: sortColumn, order: sortDirection, fromDate: "dd-mm-yyyy", toDate: "dd-mm-yyyy" },
            success: function (data) {
                $("#customersTablePartial").html(data);
            }
        });
        $("#errorMessageFromDate").text("");
        $("#errorMessageToDate").text("");
    });

    $("#closeDatePickerModalButton").click(function () {
        $("#fromDate").val("dd-mm-yyyy");
        $("#toDate").val("dd-mm-yyyy");
        $.ajax({
            url: '/Customer/FilterCustomers',
            type: "GET",
            data: { pageIndex: 1, pageSize: $("#pageSizeSelectOrders").val(), searchValue: $("#searchInputCustomers").val().toLowerCase(), time: $("#CustomersTimeSelect").val(), sort: sortColumn, order: sortDirection, fromDate: "dd-mm-yyyy", toDate: "dd-mm-yyyy" },
            success: function (data) {
                $("#customersTablePartial").html(data);
            }
        });
        $("#errorMessageFromDate").text("");
        $("#errorMessageToDate").text("");
    });

});

function FilerOrdersBasedOnTime(value) {
    if (value == "Custom Date") {
        $("#errorMessageFromDate").text("");
        $("#errorMessageToDate").text("");  
        $("#CustomDatepickerModal").modal("show");
        $("#ApplyDateChangesButton").off("click").click(function () {
            var pageIndex = 1;
            var pageSize = $("#pageSizeSelectOrders").val();
            var searchValue = $("#searchInputCustomers").val().toLowerCase();
            var time = $("#CustomersTimeSelect").val();
            var fromDate = $("#fromDate").val();
            var toDate = $("#toDate").val();
            if (fromDate == "dd-mm-yyyy") {
                $("#errorMessageFromDate").text("Please select a valid date.");
                return;
            }
            if (toDate == "dd-mm-yyyy") {
                $("#errorMessageToDate").text("Please select a valid date.");
                return;
            }
            $.ajax({
                url: '/Customer/FilterCustomers',
                type: "GET",
                data: { pageIndex: pageIndex, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
                success: function (data) {
                    $("#customersTablePartial").html(data);
                    $("#CustomDatepickerModal").modal("hide");
                    $("#errorMessageFromDate").text("");
                    $("#errorMessageToDate").text("");
                }
            });
        });
        return;
    }
    var pageIndex = 1;
    var pageSize = $("#pageSizeSelectOrders").val();
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: pageIndex, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection },
        success: function (data) {
            $("#customersTablePartial").html(data);
            $("#fromDate").val("dd-mm-yyyy");
            $("#toDate").val("dd-mm-yyyy");
            $("#errorMessageFromDate").text("");
            $("#errorMessageToDate").text("");
        }
    });
}

document.getElementById("exportOrdersBtn").addEventListener("click", function () {
const time = encodeURIComponent($("#CustomersTimeSelect").val());
const searchValue = encodeURIComponent($("#searchInputCustomers").val().toLowerCase());
const fromDate = encodeURIComponent($("#fromDate").val());
const toDate = encodeURIComponent($("#toDate").val());
const url = `/Customer/ExportCustomers?time=${time}&searchValue=${searchValue}&fromDate=${fromDate}&toDate=${toDate}`;

$('.loader-container').removeClass('d-none');

fetch(url, { method: "GET" })
    .then(async (response) => {
        const contentType = response.headers.get("content-type");
        if (contentType && contentType.includes("application/json")) {
            const jsonResponse = await response.json();
            if (!jsonResponse.success) {
                toastr.error(jsonResponse.message);
            }
            $('.loader-container').addClass('d-none');
            return;
        }

        const blob = await response.blob();
        const link = document.createElement("a");
        link.href = window.URL.createObjectURL(blob);
        const currentDate = new Date().toISOString().slice(0, 10);
        link.download = `Customers_${currentDate}.xlsx`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        $('.loader-container').addClass('d-none');
    })
    .catch(error => {
        console.error("Error exporting data:", error);
        toastr.error("An error occurred while exporting customers.");
        $('.loader-container').addClass('d-none');
    });
});