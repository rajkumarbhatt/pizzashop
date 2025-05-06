function addModifierToDeleteList(modifierId) {
    if (deleteModifierList.includes(modifierId)) {
        deleteModifierList = deleteModifierList.filter(item => item !== modifierId);
    } else {
        deleteModifierList.push(modifierId);
    }
}

$("#modifiers-name-checkbox-intermediate").click(function () {
    if ($(this).is(":checked")) {
        $(".modifiers-name-checkbox").prop("checked", true);
        $(".modifiers-name-checkbox").each(function () {
            deleteModifierList.push(parseInt($(this).val()));
        });
    } else {
        $(".modifiers-name-checkbox").prop("checked", false);
        $('.modifiers-name-checkbox').each(function () {
            deleteModifierList = deleteModifierList.filter(item => item !== parseInt($(this).val()));
        });
    }
});

$(".modifiers-name-checkbox").click(function () {
    if ($(".modifiers-name-checkbox:checked").length === $(".modifiers-name-checkbox").length) {
        $("#modifiers-name-checkbox-intermediate").prop("checked", true);
    } else {
        $("#modifiers-name-checkbox-intermediate").prop("checked", false);
    }
});

var isDeletingModifiers = false;

function deleteSelectedModifiers() {
    if (isDeletingModifiers) return;
    isDeletingModifiers = true;

    deleteModifierList = [...new Set(deleteModifierList.map(function (x) {
        return parseInt(x, 10);
    }))];

    if (deleteModifierList.length === 0) {
        toastr.error("Please select at least one modifier to delete.");
        isDeletingModifiers = false;
        return;
    }

    var modifierGroupId = $("#modifierGroupId21").val();
    $.ajax({
        url: '/Menu/DeleteSelectedModifiers',
        type: 'DELETE',
        data: { modifierIds: deleteModifierList, modifierGroupId: modifierGroupId },
        success: function (data) {
            isDeletingModifiers = false;

            if (data.success) {
                toastr.success(data.message);
                deleteModifierList = [];
                $.ajax({
                    url: '/Menu/ModifiersFilter',
                    type: 'GET',
                    data: { pageIndex: pageIndexOfModifierFromModal, pageSize: pageSizeOfModifierFromModal, modifierGroupId: modifierGroupId },
                    success: function (data) {
                        $("#ModifiersTablePartial").html(data);
                    },
                    error: function (err) {
                        console.error("Error fetching modifiers filter data", err);
                    }
                });
            } else {
                toastr.error(data.message);
            }
        },
        error: function (err) {
            isDeletingModifiers = false;
            console.error("Error deleting modifiers", err);
        }
    });
}



$("#nextButtonModifier").click(function () {
    const currentPageIndex = pageIndexOfModifierFromModal;
    const nextPageIndex = currentPageIndex + 1;
    const searchValue = $("#searchInputModifier").val().toLowerCase();
    const pageSize = pageSizeOfModifierFromModal;
    var modifierGroupId = $("#modifierGroupId21").val();
    $.ajax({
        url: '/Menu/ModifiersFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, modifierGroupId: modifierGroupId, searchValue: searchValue },
        success: function (data) {
            $("#ModifiersTablePartial").html(data);
        }
    });
});

$("#previousButtonModifier").click(function () {
    const currentPageIndex = pageIndexOfModifierFromModal;
    const nextPageIndex = currentPageIndex - 1;
    const searchValue = $("#searchInputModifier").val().toLowerCase();
    const pageSize = pageSizeOfModifierFromModal;
    var modifierGroupId = $("#modifierGroupId21").val();
    $.ajax({
        url: '/Menu/ModifiersFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, modifierGroupId: modifierGroupId, searchValue: searchValue },
        success: function (data) {
            $("#ModifiersTablePartial").html(data);
        }
    });
});

function changePageSizeModifier(pageSize) {
    const nextPageIndex = 1;
    const searchValue = $("#searchInputModifier").val().toLowerCase();
    var modifierGroupId = $("#modifierGroupId21").val();
    $.ajax({
        url: '/Menu/ModifiersFilter',
        type: 'GET',
        data: { pageIndex: nextPageIndex, pageSize: pageSize, modifierGroupId: modifierGroupId, searchValue: searchValue },
        success: function (data) {
            $("#ModifiersTablePartial").html(data);
        }
    });
}

$("#searchInputModifier").on("keyup", function () {
    clearTimeout($.data(this, 'timer'));
    var searchValue = $(this).val().toLowerCase();
    var modifierGroupId = $("#modifierGroupId21").val();
    $(this).data('timer', setTimeout(function () {
        const currentPageIndex = 1;
        const pageSize = pageSizeOfModifierFromModal;
        $.ajax({
            url: '/Menu/ModifiersFilter',
            type: 'GET',
            data: { pageIndex: currentPageIndex, pageSize: pageSize, modifierGroupId: modifierGroupId, searchValue: searchValue },
            success: function (data) {
                $("#ModifiersTablePartial").html(data);
            }
        });
    }, 300));
});

function changeModifiers(modifierGroupId) {
    $.ajax({
        url: '/Menu/ModifiersFilter',
        type: 'GET',
        data: { pageIndex: 1, pageSize: $("#PageSizeModifiers").val(), modifierGroupId: modifierGroupId },
        success: function (data) {
            $("#modifierGroupId21").val(modifierGroupId);
            $("#ModifiersTablePartial").html(data);
            makeThisModifierGroupActive(modifierGroupId);
            deleteModifierList = [];
        }
    });
    deleteModifierList.forEach(function (item) {
        $("#flexCheckChecked" + item).prop("checked", false);
    });
    deleteModifierList = [];
}

function makeThisModifierGroupActive(modifierGroupId) {
    $(".menu-category-navigation-items-modifier").removeClass("active-nav-item");
    $("#modifierGroup" + modifierGroupId + "four").addClass("active-nav-item");
    $(".gray-dot").removeClass("d-none");
    $("#gray-dot" + modifierGroupId).addClass("d-none");
    $(".blue-dot").addClass("d-none");
    $("#blue-dot" + modifierGroupId).removeClass("d-none");
}

function deleteModifierFunc(modifierId) {
    $("#deleteModifierButton").click(function () {
        $.ajax({
            url: '/Menu/DeleteModifier',
            type: 'DELETE',
            data: { modifierId: modifierId, modifierGroupId: $("#modifierGroupId21").val() },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    $.ajax({
                        url: '/Menu/ModifiersFilter',
                        type: 'GET',
                        data: { pageIndex: pageIndexOfModifierFromModal, pageSize: pageSizeOfModifierFromModal, modifierGroupId: $("#modifierGroupId21").val() },
                        success: function (data) {
                            $("#ModifiersTablePartial").html(data);
                            $("#searchInputModifier").val("");
                        }
                    });
                } else {
                    toastr.error(data.message);
                }
            }
        });
    });
}


$(document).on('keydown', function (e) {
    if (e.key === "Delete" && isModifier) {
        deleteSelectedModifiers();
    }
});



$(".modifiers-name-checkbox").each(function () {
    if (deleteModifierList.includes(parseInt($(this).val(), 10))) {
        $(this).prop("checked", true);
    }
}); 

if ($(".modifiers-name-checkbox:checked").length === $(".modifiers-name-checkbox").length) {
    $("#modifiers-name-checkbox-intermediate").prop("checked", true);
} else {
    $("#modifiers-name-checkbox-intermediate").prop("checked", false);
}

if ($(".modifiers-name-checkbox:checked").length === 0) {
    $("#modifiers-name-checkbox-intermediate").prop("checked", false);
}