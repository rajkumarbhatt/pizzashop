function openAddModifierGroupModal() {
    $("#addMGName").text("");
    $("#AddModifierName").val("");
    $("#AddModifierDescription").val("");
    $("#AddModifierGroupModal").modal("show");
    addModifierGroupFunc();
    selectedModifiers = [];
}

function addModifierGroupFunc() {
    $("#AddModifierButton2").off("click").click(function () {
        try {
            var name = $("#AddModifierName").val();
            if (name.length <= 0) {
                $("#addMGName").text("Modifier Group's name is required.");
                return;
            }
            var description = $("#AddModifierDescription").val();
            var modifierIds = selectedModifiers.map(x => x.id);
            var modifierGroupId = $("#ModifierGroupId").val();
            $.ajax({
                url: '/Menu/AddModifierGroup',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ ModifierGroupName: name, ModifierGroupDescription: description, SelectedModifierIds: modifierIds, ModifierGroupId: modifierGroupId }),
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $("#AddModifierGroupModal").modal("hide");
                        $("#addModifierGroupModalTitle").text("Add Modifier Group");
                        $.ajax({
                            type: "GET",
                            url: "/Menu/RefreshModifiersPartial",
                            data: { modifierGroupId: modifierGroupId },
                            success: function (data) {
                                $("#modifiers-content").html(data);
                                selectedModifiers = [];
                                $("#AddModifierName").val("");
                                $("#AddModifierDescription").val("");
                                $("#ModifierGroupId").val("-1");
                                $("#modifierGroupId21").val(modifierGroupId);
                                if (modifierGroupId == -1) {
                                    makeThisModifierGroupActive(1);
                                } else {
                                    makeThisModifierGroupActive(modifierGroupId);
                                }
                            },
                            error: function (data) {
                                console.log(data);
                            }
                        });
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        } catch (error) {
            console.error(error);
        }
    });
}

function deleteModifierGroupFunc(modifierId) {
    $("#deleteModifierGroupButton").click(function () {
        try {
            $.ajax({
                url: '/Menu/DeleteModifierGroup',
                type: 'DELETE',
                data: { modifierGroupId: modifierId },
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $("#deleteModifierGroup").modal("hide");
                        $.ajax({
                            type: "GET",
                            url: "/Menu/RefreshModifiersPartial",
                            success: function (data) {
                                $("#modifiers-content").html(data);
                            },
                            error: function (data) {
                                console.log(data);
                            }
                        });
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        } catch (error) {
            console.error(error);
        }
    });
}

var selectedModifiersGroupsAddModifier = [];
if (typeof selectedModifiersGroupsAddModifier2 !== "undefined") {
    if (selectedModifiersGroupsAddModifier2) {  
        selectedModifiersGroupsAddModifier = selectedModifiersGroupsAddModifier2;
    }
}

function addModifierGroupToModifier(id) {
    try {
        var modifierGroupId = id.replace("AddModifier", "");
        modifierGroupId = parseInt(modifierGroupId);
        if (selectedModifiersGroupsAddModifier.includes(modifierGroupId)) {
            selectedModifiersGroupsAddModifier = selectedModifiersGroupsAddModifier.filter(x => x !== modifierGroupId);
            $("#AddModifierModifierGroupIds").val(selectedModifiersGroupsAddModifier);
        } else {
            selectedModifiersGroupsAddModifier.push(modifierGroupId);
            $("#AddModifierModifierGroupIds").val(selectedModifiersGroupsAddModifier);
        }
    } catch (error) {
        console.error(error);
    }
}

function openAddModifierModal() {
    try {
        $("#AddModifierModal").modal("show");
        $.validator.unobtrusive.parse("#AddModifierModal");
        $("#AddModifierModalTitle").text("Add New Modifier");
        $("#custom-multiselect span:first-child").text("Select Categories");
        $(".text-danger").text("");
        $(".multi-checkbox").change(function () {
            var selected = [];
            $(".multi-checkbox:checked").each(function () {
                selected.push($(this).next("label").text());
            });

            if (selected.length > 0) {
                $("#custom-multiselect span:first-child").text(selected.join(", "));
            } else {
                $("#custom-multiselect span:first-child").text("Select Categories");
            }
        });
        addModifier();
    } catch (error) {
        console.error(error);
    }
}

function addModifier() {
    $("#addModifierModalForm").off("submit").submit(function (e) {
        e.preventDefault();
        try {
            if (!$("#addModifierModalForm").valid()) {
                return;
            }
            var name = $("#AddModifierName21").val();
            var rate = $("#AddModifierRate").val();
            var quantity = $("#AddModifierQuantity").val();
            var unit = $("#AddModifierUnit").val();
            var description = $("#AddModifierDescription21").val();
            var id = $("#AddModifierId").val();
            var modifierGroupIds = selectedModifiersGroupsAddModifier;
            $.ajax({
                url: '/Menu/AddModifier',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ Name: name, Rate: rate, Quantity: quantity, Unit: unit, Description: description, ModifierGroupIds: modifierGroupIds, Id: id }),
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $("#AddModifierModal").modal("hide");
                        $.ajax({
                            url: '/Menu/ModifiersFilter',
                            type: 'GET',
                            data: { pageIndex: pageIndexOfModifierFromModal, pageSize: pageSizeOfModifierFromModal, modifierGroupId: $("#modifierGroupId21").val() },
                            success: function (data) {
                                $("#ModifiersTablePartial").html(data);
                                clearAddModifierModal();
                                $("#AddModifierModal").modal("hide");
                            }
                        });
                    } else {
                        toastr.error(data.message);
                    }
                }
            });
        } catch (error) {
            console.error(error);
        }
    });
}

function editModifierGroup(modifierGroupId) {
    try {
        $.ajax({
            url: '/Menu/EditModifierGroup',
            type: 'GET',
            data: { modifierGroupId: modifierGroupId },
            success: function (data) {
                $("#modifiers-content").html(data);
                $("#AddModifierGroupModal").modal("show");
                $("#addModifierGroupModalTitle").text("Edit Modifier Group");
                addModifierGroupFunc();
                makeThisModifierGroupActive(modifierGroupId);
            },
            error: function (error) {
                console.log(error);
            }
        });
    } catch (error) {
        console.error(error);
    }
}

$(".multi-checkbox").change(function () {
    try {
        var selected = [];
        $(".multi-checkbox:checked").each(function () {
            selected.push($(this).next("label").text());
        });

        if (selected.length > 0) {
            $("#custom-multiselect span:first-child").text(selected.join(", "));
        } else {
            $("#custom-multiselect span:first-child").text("Select Categories");
        }
    } catch (error) {
        console.error(error);
    }
});

function editModifier(modifierId) {
    try {
        var modifierGroupId = $("#modifierGroupId21").val();
        $.ajax({
            url: '/Menu/EditModifier',
            type: 'GET',
            data: { modifierId: modifierId, modifierGroupId: modifierGroupId },
            success: function (data) {
                $("#addEditModifierModal").html(data);
                $("#AddModifierModalTitle").text("Edit Modifier");
                $("#AddModifierModal").modal('show');
                $.validator.unobtrusive.parse("#AddModifierModal");
                $("#modifierGroupId21").val(modifierGroupId);
                addModifier();
                makeThisModifierGroupActive(modifierGroupId);
            },
            error: function (error) {
                console.log(error);
            }
        });
    } catch (error) {
        console.error(error);
    }
}

function clearSelectedModifiersList() {
    try {
        selectedModifiers = [];
        $("#modifiersSelected").html("");
        $("#AddModifierName").val("");
        $("#AddModifierDescription").val("");
        $("#ModifierGroupId").val("-1");
        $("#addModifierGroupModalTitle").text("Add Modifier Group");
    } catch (error) {
        console.error(error);
    }
}

function clearAddModifierModal() {
    try {
        $("#AddModifierModal").modal("hide");
        $("#AddModifierModalTitle").text("Add New Modifier");
        $("#AddModifierName21").val("");
        $("#AddModifierRate").val("");
        $("#AddModifierQuantity").val("");
        $("#AddModifierUnit").val("");
        $("#AddModifierDescription21").val("");
        selectedModifiersGroupsAddModifier.forEach(x => {
            var customId = "AddModifier" + x;
            $("#" + customId).prop("checked", false);
        });
        selectedModifiersGroupsAddModifier = [];
        $("#AddModifierModifierGroupIds").val("");
        $("#AddModifierId").val("-1");
    } catch (error) {
        console.error(error);
    }
}

function showDivM(modifierGroupId) {
    try {
        modifierGroupId.classList.remove('d-none');
        var divId2 = modifierGroupId.id + "two";
        $("#" + divId2).addClass('active-category-div');
    } catch (error) {
        console.error(error);
    }
}

function hideDivM(modifierGroupId) {
    try {
        modifierGroupId.classList.add('d-none');
        var divId2 = modifierGroupId.id + "two";
        $("#" + divId2).removeClass('active-category-div');
    } catch (error) {
        console.error(error);
    }
}

function showSearchBarModifier() {
    try {
        document.getElementById("mobile-search-bar-modifier").classList.remove("d-none");
        document.getElementById("search-icon-modifier").classList.add("d-none");
    } catch (error) {
        console.error(error);
    }
}