$("#previousButtonOrders").click(function () {
    var pageIndex = pageIndexOfModal - 1;
    var pageSize = pageSizeOfModal;
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: pageIndex, pageSize: pageSize, searchValue: searchValue,  time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate},
        success: function (data) {
            $("#customersTablePartial").html(data);
        }
    });
});

$("#nextButtonOrders").click(function () {
    var pageIndex = pageIndexOfModal + 1;
    var pageSize = pageSizeOfModal;
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: pageIndex, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#customersTablePartial").html(data);
        }
    });
});

function changePageSizeOrders(pageSize) {
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#customersTablePartial").html(data);
        }
    });
}

$("#searchInputCustomers").on("input", function () {
    clearTimeout(window.searchTimeout);
    window.searchTimeout = setTimeout(function () {
        var searchValue = $("#searchInputCustomers").val().toLowerCase();
        var pageSize = pageSizeOfModal;
        var time = $("#CustomersTimeSelect").val();
        var fromDate = $("#fromDate").val();
        var toDate = $("#toDate").val();
        $.ajax({
            url: '/Customer/FilterCustomers',
            type: "GET",
            data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
            success: function (data) {
                $("#customersTablePartial").html(data);
            }
        });
    }, 500);
});

$("#sortNameCustomer").click(function () {
    sortColumn = "name";
    if (isNameAsc) {
        sortDirection = "desc";
        isNameAsc = false;
    } else {
        isNameAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#customersTablePartial").html(data);
        }
    });
});

$("#sortCustomerDate").click(function () {
    sortColumn = "date";
    if (isDateAsc) {
        sortDirection = "desc";
        isDateAsc = false;
    } else {
        isDateAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#customersTablePartial").html(data);
        }
    });
});

$("#sortTotalOrdersCustomers").click(function () {
    sortColumn = "totalOrders";
    if (isTotalOrdersAsc) {
        sortDirection = "desc";
        isTotalOrdersAsc = false;
    } else {
        isTotalOrdersAsc = true;
        sortDirection = "asc";
    }
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#customersTablePartial").html(data);
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
    var searchValue = $("#searchInputCustomers").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    var time = $("#CustomersTimeSelect").val();
    var fromDate = $("#fromDate").val();
    var toDate = $("#toDate").val();
    $.ajax({
        url: '/Customer/FilterCustomers',
        type: "GET",
        data: { pageIndex: 1, pageSize: pageSize, searchValue: searchValue, time: time, sort: sortColumn, order: sortDirection, fromDate: fromDate, toDate: toDate },
        success: function (data) {
            $("#customersTablePartial").html(data);
        }
    });
});

function showCustomerDetails(id) {
    $.ajax({
        url: '/Customer/GetCustomerDetails',
        type: "GET",
        data: { id: id },
        success: function (data) {
            $("#customerDetailsModal").html(data);
            $("#CustomerDetailsModal").modal("show");
        }
    });
}