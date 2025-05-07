function addModifierToDeleteList(modifierId) {
    if (deleteModifierList.includes(modifierId)) {
        deleteModifierList = deleteModifierList.filter(item => item !== modifierId);
    } else {
        deleteModifierList.push(modifierId);
    }
}

$("#modifiers-name-checkbox-intermediate").click(function () {
    try {
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
    } catch (error) {
        console.error("Error handling checkbox click", error);
    }
});

$(".modifiers-name-checkbox").click(function () {
    try {
        if ($(".modifiers-name-checkbox:checked").length === $(".modifiers-name-checkbox").length) {
            $("#modifiers-name-checkbox-intermediate").prop("checked", true);
        } else {
            $("#modifiers-name-checkbox-intermediate").prop("checked", false);
        }
    } catch (error) {
        console.error("Error handling individual checkbox click", error);
    }
});

var isDeletingModifiers = false;

function deleteSelectedModifiers() {
    if (isDeletingModifiers) return;
    isDeletingModifiers = true;

    try {
        deleteModifierList = [...new Set(deleteModifierList.map(x => parseInt(x, 10)))];

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
    } catch (error) {
        isDeletingModifiers = false;
        console.error("Error in deleteSelectedModifiers", error);
    }
}

$("#nextButtonModifier").click(function () {
    try {
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
    } catch (error) {
        console.error("Error in nextButtonModifier click", error);
    }
});

$("#previousButtonModifier").click(function () {
    try {
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
    } catch (error) {
        console.error("Error in previousButtonModifier click", error);
    }
});

function changePageSizeModifier(pageSize) {
    try {
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
    } catch (error) {
        console.error("Error in changePageSizeModifier", error);
    }
}

$("#searchInputModifier").on("keyup", function () {
    try {
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
    } catch (error) {
        console.error("Error in searchInputModifier keyup", error);
    }
});

function changeModifiers(modifierGroupId) {
    try {
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
    } catch (error) {
        console.error("Error in changeModifiers", error);
    }
}

function makeThisModifierGroupActive(modifierGroupId) {
    try {
        $(".menu-category-navigation-items-modifier").removeClass("active-nav-item");
        $("#modifierGroup" + modifierGroupId + "four").addClass("active-nav-item");
        $(".gray-dot").removeClass("d-none");
        $("#gray-dot" + modifierGroupId).addClass("d-none");
        $(".blue-dot").addClass("d-none");
        $("#blue-dot" + modifierGroupId).removeClass("d-none");
    } catch (error) {
        console.error("Error in makeThisModifierGroupActive", error);
    }
}

function deleteModifierFunc(modifierId) {
    try {
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
    } catch (error) {
        console.error("Error in deleteModifierFunc", error);
    }
}

$(document).on('keydown', function (e) {
    try {
        if (e.key === "Delete" && isModifier) {
            deleteSelectedModifiers();
        }
    } catch (error) {
        console.error("Error in keydown event", error);
    }
});

$(".modifiers-name-checkbox").each(function () {
    try {
        if (typeof deleteModifierList !== 'undefined') {
            if (deleteModifierList.includes(parseInt($(this).val(), 10))) {
                $(this).prop("checked", true);
            }
        }
    } catch (error) {
        console.error("Error in modifiers-name-checkbox iteration", error);
    }
});

try {
    if ($(".modifiers-name-checkbox:checked").length === $(".modifiers-name-checkbox").length) {
        $("#modifiers-name-checkbox-intermediate").prop("checked", true);
    } else {
        $("#modifiers-name-checkbox-intermediate").prop("checked", false);
    }

    if ($(".modifiers-name-checkbox:checked").length === 0) {
        $("#modifiers-name-checkbox-intermediate").prop("checked", false);
    }
} catch (error) {
    console.error("Error in checkbox state initialization", error);
}