function downloadPdfInvoice(orderId) {
    const url = '/Order/DownloadInvoice?orderId=' + orderId;
    $('.loader-container').removeClass('d-none');
    fetch(url, { method: "GET" })
        .then(response => response.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Order_${orderId}.pdf`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            $('.loader-container').addClass('d-none');
        })
        .catch(error => {
            
        });
}

function viewOrderDetails(orderId) {
    $.ajax({
        url: '/Order/EncryptOrder',
        type: "GET",
        data: { orderId: orderId },
        success: function (data) {
            orderId = data.encryptedOrderId;
            window.location.href = "/Order/OrderDetails/" + orderId;
        }
    });
}

function renderStars() {
    document.querySelectorAll(".rating").forEach(td => {
        let rating = parseFloat(td.getAttribute("data-rating"));
        let percentage = (rating / 5) * 100;
        let starsHtml = `
            <div class="star-container">
                <div class="stars">
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                </div>
                <div class="stars stars-fill" style="width: ${percentage}%;">
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                    <i class="bi bi-star-fill"></i>
                </div>
            </div>
        `;
        
        td.innerHTML = starsHtml;
    });
}

document.addEventListener("DOMContentLoaded", renderStars);

$("#previousButtonOrders").click(function () {
    var pageIndex = pageIndexOfModal - 1;
    var pageSize = pageSixeOfModal;
    var searchValue = $("#searchInputOrders").val();
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: pageIndex, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});

$("#nextButtonOrders").click(function () {
    var pageIndex = pageIndexOfModal + 1;
    var pageSize = pageSixeOfModal;
    var searchValue = $("#searchInputOrders").val();
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: pageIndex, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});

function changePageSizeOrders(pageSize) {
    var status = $("#OrderStatusSelect").val();
    var searchValue = $("#searchInputOrders").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
}

$("#searchInputOrders").on("input", function () {
    clearTimeout(window.searchTimeout);
    window.searchTimeout = setTimeout(function () {
        var searchValue = $("#searchInputOrders").val();
        var pageSize = pageSixeOfModal;
        var status = $("#OrderStatusSelect").val();
        var time = $("#OrderTimeSelect").val();
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        $.ajax({
            url: '/Order/FilterOrders',
            type: "GET",
            data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
            success: function (data) {
                $("#orderTablePartial").html(data);
                renderStars();
            }
        });
    }, 500);
});

$("#sortOrderId").click(function () {
    sortColumn = "id";
    if (isOrderIdAsc) {
        sortDirection = "desc";
        isOrderIdAsc = false;
    } else {
        isOrderIdAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputOrders").val();
    var pageSize = pageSixeOfModal;
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});

$("#sortDate").click(function () {
    sortColumn = "date";
    if (isDateAsc) {
        sortDirection = "desc";
        isDateAsc = false;
    } else {
        isDateAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputOrders").val();
    var pageSize = pageSixeOfModal;
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});

$("#sortCustomerNames").click(function () {
    sortColumn = "customerName";
    if (isCustomerNamesAsc) {
        sortDirection = "desc";
        isCustomerNamesAsc = false;
    } else {
        isCustomerNamesAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputOrders").val();
    var pageSize = pageSixeOfModal;
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
    });
});

$("#sortTotalAmount").click(function () {
    sortColumn = "totalAmount";
    if (isTotalAmountAsc) {
        sortDirection = "desc";
        isTotalAmountAsc = false;
    } else {
        isTotalAmountAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputOrders").val();
    var pageSize = pageSixeOfModal;
    var status = $("#OrderStatusSelect").val();
    var time = $("#OrderTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Order/FilterOrders',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, status: status, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#orderTablePartial").html(data);
            renderStars();
        }
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

    $('.datepicker').datepicker('setEndDate', new Date());

    $('#fromDate').on('change', function () {
        var fromDate = $(this).datepicker('getDate');
        $('#toDate').datepicker('setStartDate', fromDate);
    });

    $('#toDate').on('change', function () {
        var toDate = $(this).datepicker('getDate');
        $('#fromDate').datepicker('setEndDate', toDate);
    });
})