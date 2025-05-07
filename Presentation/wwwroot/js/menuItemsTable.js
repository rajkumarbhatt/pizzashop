$("#DeleteItemsIntermediateCheckbox").click(function () {
    try {
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
    } catch (error) {
        console.error(error);
    }
});

$(".item-name-checkbox").click(function () {
    try {
        if ($(".item-name-checkbox:checked").length === $(".item-name-checkbox").length) {
            $("#DeleteItemsIntermediateCheckbox").prop("checked", true);
        } else {
            $("#DeleteItemsIntermediateCheckbox").prop("checked", false);
        }
    } catch (error) {
        console.error(error);
    }
});

function addItemToDeleteList(itemId) {
    try {
        itemId = parseInt(itemId);
        if (deleteItemsList.includes(itemId)) {
            deleteItemsList = deleteItemsList.filter(item => item !== itemId);
        } else {
            deleteItemsList.push(itemId);
        }
    } catch (error) {
        console.error(error);
    }
}

var isDeleting = false;

function deleteSelectedItems() {
    try {
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
                        data: { categoryId: $("#categoryId").val(), pageIndex: 1, pageSize: $("#PageSize").val() },
                        success: function (data) {
                            $("#partialViewStartingInner").html(data);
                        },
                        error: function (data) {
                            console.error(data);
                        }
                    });
                    $("#deleteItems").modal("hide");
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (err) {
                isDeleting = false;
                console.error(err);
            }
        });
    } catch (error) {
        isDeleting = false;
        console.error(error);
    }
}

$("#nextButton").click(function () {
    try {
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
    } catch (error) {
        console.error(error);
    }
});

$("#previousButton").click(function () {
    try {
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
    } catch (error) {
        console.error(error);
    }
});

function changePageSize(pageSize) {
    try {
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
    } catch (error) {
        console.error(error);
    }
}

$("#searchInput").on("keyup", function () {
    try {
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
    } catch (error) {
        console.error(error);
    }
});

$("#searchInput2").on("keyup", function () {
    try {
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
    } catch (error) {
        console.error(error);
    }
});

function saveChangesOfSwitch(itemId) {
    try {
        var idCustom = "flexSwitchCheckChecked" + itemId;
        var isAvailable = document.getElementById(idCustom).checked;
        $.ajax({
            url: '/Menu/UpdateItemAvailability',
            type: 'POST',
            data: { itemId: itemId, isAvailable: isAvailable },
            success: function (data) {
            }
        });
    } catch (error) {
        console.error(error);
    }
}

function changeItems(categoryId) {
    try {
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
    } catch (error) {
        console.error(error);
    }
}

function makeThisCategoryActive(categoryId) {
    try {
        $(".menu-category-navigation-items").removeClass("active-nav-item");
        $("#div" + categoryId + "four").addClass("active-nav-item");
        $(".gray-dot").removeClass("d-none");
        $("#grayDot" + categoryId).addClass("d-none");
        $(".blue-dot").addClass("d-none");
        $("#blueDot" + categoryId).removeClass("d-none");
    } catch (error) {
        console.error(error);
    }
}

function deleteItem(itemId) {
    try {
        var currentCategoryId = $("#categoryId").val();
        var currentPageIndex = pageIndexItemsOfModal;
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
                                console.error(data);
                            }
                        });
                        $("#deleteItem").modal("hide");
                    } else {
                        toastr.error(data);
                    }
                }
            });
        });
    } catch (error) {
        console.error(error);
    }
}

$(document).off("one").one('keydown', function (e) {
    try {
        if (e.key === "Delete" && isItem) {
            deleteSelectedItems();
        }
    } catch (error) {
        console.error(error);
    }
});

if (typeof deleteItemsList !== 'undefined') {
    deleteItemsList.forEach(item => {
        try {
            $(".item-name-checkbox").each(function () {
                if (parseInt($(this).val()) === item) {
                    $(this).prop("checked", true);
                }
            });
        } catch (error) {
            console.error(error);
        }
    });
}


if ($(".item-name-checkbox:checked").length === $(".item-name-checkbox").length) {
    $("#DeleteItemsIntermediateCheckbox").prop("checked", true);
} else {
    $("#DeleteItemsIntermediateCheckbox").prop("checked", false);
}

if ($(".item-name-checkbox:checked").length == 0) {
    $("#DeleteItemsIntermediateCheckbox").prop("checked", false);
}