$("#DeleteItemsIntermediateCheckbox").click(function () {
    if (this.checked) {
        $(".item-name-checkbox").prop("checked", true);
        $(".item-name-checkbox").each(function () {
            var itemIdTemp = parseInt($(this).val());
            if (!deleteItemsList.includes(itemIdTemp)) {
                deleteItemsList.push(itemIdTemp);
            }
        });
    } else {
        $(".item-name-checkbox").prop("checked", false);
        $(".item-name-checkbox").each(function () {
            var itemIdTemp = parseInt($(this).val());
            deleteItemsList = deleteItemsList.filter(item => item !== itemIdTemp);
        });
    }
});

$(".item-name-checkbox").click(function () {
    if ($(".item-name-checkbox:checked").length === $(".item-name-checkbox").length) {
        $("#DeleteItemsIntermediateCheckbox").prop("checked", true);
    } else {
        $("#DeleteItemsIntermediateCheckbox").prop("checked", false);
    }
});

function addItemToDeleteList(itemId) {
    itemId = parseInt(itemId);
    if (deleteItemsList.includes(itemId)) {
        deleteItemsList = deleteItemsList.filter(item => item !== itemId);
    } else {
        deleteItemsList.push(itemId);
    }
}

var isDeleting = false;

function deleteSelectedItems() {
    if (isDeleting) return; 
    isDeleting = true;

    deleteItemsList = deleteItemsList.map(item => parseInt(item));
    if (deleteItemsList.length === 0) {
        toastr.error("Please select at least one item to delete.");
        isDeleting = false; 
        return;
    }
    $.ajax({
        url: '/Menu/DeleteSelectedItems',
        type: 'DELETE',
        data: { itemIds: deleteItemsList },
        success: function (data) {
            isDeleting = false;  
            if (data.success) {
                toastr.success(data.message);
                deleteItemsList = [];
                $.ajax({
                    type: "GET",
                    url: "/Menu/ItemsFilter",
                    data: { categoryId: $("#categoryId").val(), pageIndex:1, pageSize: $("#PageSize").val() },
                    success: function (data) {
                        $("#partialViewStartingInner").html(data);
                    },
                    error: function (data) {
                    }
                });
                $("#deleteItems").modal("hide");
            } else {
                toastr.error(data.message);
            }
        },
        error: function (err) {
            isDeleting = false;
        }
    });
}

$("#nextButton").click(function () {
    const currentPageIndex = pageIndexItemsOfModal;
    const nextPageIndex = currentPageIndex + 1;
    const searchValue = $("#searchInput").val().toLowerCase();
    const pageSize = pageSizeItemsOfModal;
    const categoryId = $("#categoryId").val();
    $.ajax({
        url: '/Menu/ItemsFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, categoryId: categoryId, searchValue: searchValue },
        success: function (data) {
            $("#partialViewStartingInner").html(data);
        }
    });
});

$("#previousButton").click(function () {
    const currentPageIndex = pageIndexItemsOfModal;
    const nextPageIndex = currentPageIndex - 1;
    const searchValue = $("#searchInput").val().toLowerCase();
    const pageSize = pageSizeItemsOfModal;
    const categoryId = $("#categoryId").val();
    $.ajax({
        url: '/Menu/ItemsFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, categoryId: categoryId, searchValue: searchValue },
        success: function (data) {
            $("#partialViewStartingInner").html(data);
        }
    });
});

function changePageSize(pageSize) {
    const nextPageIndex = 1;
    const searchValue = $("#searchInput").val().toLowerCase();
    const categoryId = $("#categoryId").val();
    $.ajax({
        url: '/Menu/ItemsFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, categoryId: categoryId, searchValue: searchValue },
        success: function (data) {
            $("#partialViewStartingInner").html(data);
        }
    });
}

$("#searchInput").on("keyup", function () {
    clearTimeout($.data(this, 'timer'));
    var searchValue = $(this).val().toLowerCase();
    var categoryId = $("#categoryId").val();
    $(this).data('timer', setTimeout(function () {
        const currentPageIndex = 1;
        const pageSize = pageSizeItemsOfModal;
        $.ajax({
            url: '/Menu/ItemsFilter',
            type: 'GET',
            data: { pageIndex: currentPageIndex, pageSize: pageSize, categoryId: categoryId, searchValue: searchValue },
            success: function (data) {
                $("#partialViewStartingInner").html(data);
            }
        });
    }, 300));
});

$("#searchInput2").on("keyup", function () {
    clearTimeout($.data(this, 'timer'));
    var searchValue = $(this).val().toLowerCase();
    var categoryId = $("#categoryId").val();
    $(this).data('timer', setTimeout(function () {
        const currentPageIndex = 1;
        const pageSize = pageSizeItemsOfModal;
        $.ajax({
            url: '/Menu/ItemsFilter',
            type: 'GET',
            data: { pageIndex: currentPageIndex, pageSize: pageSize, categoryId: categoryId, searchValue: searchValue },
            success: function (data) {
                $("#partialViewStartingInner").html(data);
            }
        });
    }, 300));
});

function saveChangesOfSwitch(itemId) {
    var idCustom = "flexSwitchCheckChecked" + itemId;
    var isAvailable = document.getElementById(idCustom).checked;
    $.ajax({
        url: '/Menu/UpdateItemAvailability',
        type: 'POST',
        data: { itemId: itemId, isAvailable: isAvailable },
        success: function (data) {
        }
    });
}

function changeItems(categoryId) {
    var searchValue = $("#searchInput").val().toLowerCase();
    $.ajax({
        url: '/Menu/ItemsFilter',
        type: 'GET',
        data: { pageIndex: 1, pageSize: $("#PageSize").val(), categoryId: categoryId, searchValue: searchValue },
        success: function (data) {
            $("#categoryId").val(categoryId);
            $("#partialViewStartingInner").html(data);
            makeThisCategoryActive(categoryId);
        }
    });
    deleteItemsList.forEach(item => {
        $(".item-name-checkbox").each(function () {
            if (parseInt($(this).val()) === item) {
                $(this).prop("checked", true);
            }
        });
    });
    deleteItemsList = [];
}

function makeThisCategoryActive(categoryId) {
    $(".menu-category-navigation-items").removeClass("active-nav-item");
    $("#div" + categoryId + "four").addClass("active-nav-item");
    $(".gray-dot").removeClass("d-none");
    $("#grayDot" + categoryId).addClass("d-none");
    $(".blue-dot").addClass("d-none");
    $("#blueDot" + categoryId).removeClass("d-none");
}

function deleteItem(itemId) {
    var currentCategoryId = $("#categoryId").val();
    var currentPageIndex = pageIndexItemsOfModal
    var currentPageSize = $("#PageSize").val();
    $("#deleteItemButton").off("click").click(function () {
        $.ajax({
            url: '/Menu/DeleteItem',
            type: 'DELETE',
            data: { itemId: itemId },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    $.ajax({
                        type: "GET",
                        url: "/Menu/ItemsFilter",
                        data: { categoryId: currentCategoryId, pageIndex: currentPageIndex, pageSize: currentPageSize },
                        success: function (data) {
                            $("#partialViewStartingInner").html(data);
                            $("#searchInput").val("");
                        },
                        error: function (data) {
                        }
                    });
                    $("#deleteItem").modal("hide");
                } else {
                    toastr.error(data);
                }
            }
        });
    });
}

$(document).off("one").one('keydown', function (e) {
    if (e.key === "Delete" && isItem) {
        deleteSelectedItems();
    }
});

deleteItemsList.forEach(item => {
    $(".item-name-checkbox").each(function () {
        if (parseInt($(this).val()) === item) {
            $(this).prop("checked", true);
        }
    });
});

if ($(".item-name-checkbox:checked").length === $(".item-name-checkbox").length) {
    $("#DeleteItemsIntermediateCheckbox").prop("checked", true);
} else {
    $("#DeleteItemsIntermediateCheckbox").prop("checked", false);
}

if ($(".item-name-checkbox:checked").length == 0) {
    $("#DeleteItemsIntermediateCheckbox").prop("checked", false);
}