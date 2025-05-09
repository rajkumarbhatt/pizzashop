var isSearchClicked = false;

function FilerOrdersBasedOnStatus(status) {
    var searchValue = $("#searchInputOrders").val();
    var pageSize = $("#pageSizeSelectOrders").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, status: status, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
}

function FilerOrdersBasedOnTime(time) {
    var searchValue = $("#searchInputOrders").val();
    var pageSize = $("#pageSizeSelectOrders").val();
    var status = $("#OrderStatusSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, time: time, searchValue: searchValue, status: status, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
}

document.getElementById("exportOrdersBtn").addEventListener("click", function () {
    const status = encodeURIComponent($("#OrderStatusSelect").val());
    const time = encodeURIComponent($("#OrderTimeSelect").val());
    const searchValue = encodeURIComponent($("#searchInputOrders").val());
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    if (!isSearchClicked) {
        fromDate = "dd-mm-yyyy";
        toDate = "dd-mm-yyyy";
    }
    const url = `/Order/ExportOrders?status=${status}&time=${time}&searchValue=${searchValue}&fromDate=${fromDate}&toDate=${toDate}`;

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
            link.download = `Orders_${currentDate}.xlsx`;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
            $('.loader-container').addClass('d-none');
        })
        .catch(error => {
            console.error("Error exporting data:", error);
            toastr.error("An error occurred while exporting orders.");
            $('.loader-container').addClass('d-none');
        });
});


$(document).ready(function () {
    $('.datepicker').datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true
    }).on('show', function () {
        $(this).attr('readonly', true);
    });
});

$("#SearchDate").click(function () {
    isSearchClicked = true;
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    var searchValue = $("#searchInputOrders").val();
    var pageSize = $("#pageSizeSelectOrders").val();
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, fromDate: fromDate, toDate: toDate, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});

$("#ClearFilters").click(function () {
    isSearchClicked = false;
    $("#fromDate").val("dd-mm-yyyy");
    $("#toDate").val("dd-mm-yyyy");
    $("#searchInputOrders").val("");
    $("#OrderStatusSelect").val("All Status");
    $("#OrderTimeSelect").val("All Time");
    var pageSize = $("#pageSizeSelectOrders").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, sort: sortColumn, order: sortDirection },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});