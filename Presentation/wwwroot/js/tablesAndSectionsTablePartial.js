function editTableFunc(id) {
    $.ajax({
        url: '/TableAndSection/EditTable',
        type: 'GET',
        data: { tableId: id },
        success: function (data) {
            $("#TableName").val(data.name);
            $("#TableCapacity").val(data.capacity);
            if (data.status === "Available") {
                $("#TableStatus").val("Available");
            } else {
                $("#TableStatus").val("Occupied");
            }
            $("#TableSection").val(data.sectionId);
            $("#TableId").val(data.id);
            $("#addTableTitle").text("Edit Table");
            $('.text-danger').text("");
            $("#addTableModal").modal("show");
        }
    });
}

$("#DeleteTablesIntermediateCheckbox").click(function () {
    if ($(this).is(":checked")) {
        $(".table-name-checkbox").prop("checked", true);
        $(".table-name-checkbox").each(function () {
            if (!deleteTableIds.includes(parseInt($(this).val()))) {
                deleteTableIds.push(parseInt($(this).val()));
            }
        });
    } else {
        $(".table-name-checkbox").prop("checked", false);
        $(".table-name-checkbox").each(function () {
            deleteTableIds = deleteTableIds.filter(id => id !== parseInt($(this).val()));
        });
    }
});

$(".table-name-checkbox").click(function () {
    if ($(".table-name-checkbox:checked").length === $(".table-name-checkbox").length) {
        $("#DeleteTablesIntermediateCheckbox").prop("checked", true);
    } else {
        $("#DeleteTablesIntermediateCheckbox").prop("checked", false);
    }
});

function addToList(tableId) {
    if (deleteTableIds.includes(tableId)) {
        deleteTableIds = deleteTableIds.filter(id => id !== tableId);
    } else {
        deleteTableIds.push(tableId);
    }
}

function changeTables(sectionId) {
    var searchValue = $("#searchInput").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    $.ajax({
        url: '/TableAndSection/TablesFilter',
        type: 'GET',
        data: { pageIndex: 1, pageSize: pageSize, sectionId: sectionId, searchValue: searchValue },
        success: function (data) {
            $("#tablePartialView").html(data);
            $("#SectionId").val(sectionId);
            makeThisSectionActive(sectionId);
        }
    });
}

$("#nextButton").click(function () {
    var currentPageIndex = pageIndexOfModal;
    var nextPageIndex = currentPageIndex + 1;
    var searchValue = $("#searchInput").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    var sectionId = $("#SectionId").val();
    $.ajax({
        url: '/TableAndSection/TablesFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, sectionId: sectionId, searchValue: searchValue },
        success: function (data) {
            $("#tablePartialView").html(data);
        }
    });
});

$("#previousButton").click(function () {
    var currentPageIndex = pageIndexOfModal;
    var previousPageIndex = currentPageIndex - 1;
    var searchValue = $("#searchInput").val().toLowerCase();
    var pageSize = pageSizeOfModal;
    var sectionId = $("#SectionId").val();
    $.ajax({
        url: '/TableAndSection/TablesFilter',
        type: 'GET',
        data: { pageIndex: previousPageIndex, pageSize: pageSize, sectionId: sectionId, searchValue: searchValue },
        success: function (data) {
            $("#tablePartialView").html(data);
        }
    });
});

function changePageSize(pageSize) {
    var searchValue = $("#searchInput").val().toLowerCase();
    var sectionId = $("#SectionId").val();
    $.ajax({
        url: '/TableAndSection/TablesFilter',
        type: 'GET',
        data: { pageIndex: 1, pageSize: pageSize, sectionId: sectionId, searchValue: searchValue },
        success: function (data) {
            $("#tablePartialView").html(data);
        }
    });
}

$("#searchInput").on("keyup", function () {
    clearTimeout($.data(this, 'timer'));
    var searchValue = $(this).val().toLowerCase();
    var sectionId = $("#SectionId").val();
    $(this).data('timer', setTimeout(function () {
        const currentPageIndex = 1;
        const pageSize = pageSizeOfModal
        $.ajax({
            url: '/TableAndSection/TablesFilter',
            type: 'GET',
            data: { pageIndex: currentPageIndex, pageSize: pageSize, sectionId: sectionId, searchValue: searchValue },
            success: function (data) {
                $("#tablePartialView").html(data);
            }
        });
    }, 300));
});

$('.table-name-checkbox').each(function () {
    if (deleteTableIds.includes(parseInt($(this).val()))) {
        $(this).prop('checked', true);
    }
});

if ($(".table-name-checkbox:checked").length === $(".table-name-checkbox").length) {
    $("#DeleteTablesIntermediateCheckbox").prop("checked", true);
} else {
    $("#DeleteTablesIntermediateCheckbox").prop("checked", false);
}

if ($(".table-name-checkbox:checked").length === 0) {
    $("#DeleteTablesIntermediateCheckbox").prop("checked", false);
}