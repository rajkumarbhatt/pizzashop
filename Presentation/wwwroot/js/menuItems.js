function openModalAllModifiers() {
    $("#AddModifierGroupModal2").modal("show");
    tickSelectedModifiers();
}

function addModifierGroupToList(id) {
    var isChecked = document.getElementById(id).checked;
    var idInt = parseInt(id);
    if (isChecked) {
        selectModifierGroups.push({
            Id: idInt,
            MinimumQuantity: 0,
            MaximumQuantity: 0
        });
    } else {
        selectModifierGroups = selectModifierGroups.filter((value) => value.Id !== idInt);
    }
    var selectModifierGroupsString = JSON.stringify(selectModifierGroups);
    $.ajax({
        type: "GET",
        url: "/Menu/GetModifierGroupData",
        data: { modifierGroupIds: selectModifierGroupsString },
        success: function (response) {
            selectModifierGroups = JSON.parse(selectModifierGroupsString);
            $("#modifierGroupdata").html(response);
        }
    });
}

function addMinimum(id, value) {
    var index = selectModifierGroups.findIndex(x => x.Id === id);
    selectModifierGroups[index].MinimumQuantity = value;
    var selectModifierGroupsString = JSON.stringify(selectModifierGroups);
    var maximumSelect = document.getElementById(id + "max");
    var minimumSelect = document.getElementById(id + "min");
    var minimumValue = minimumSelect.value;
    var maximumOptions = maximumSelect.options;
    for (var i = 0; i < maximumOptions.length; i++) {
        if (parseInt(maximumOptions[i].value) < minimumValue) {
            maximumOptions[i].style.display = "none";
        } else {
            maximumOptions[i].style.display = "block";
        }
    }
    if (parseInt(maximumSelect.value) < minimumValue) {
        maximumSelect.value = minimumValue;
        addMaximum(id, minimumValue);
    }
}

function addMaximum(id, value) {
    var index = selectModifierGroups.findIndex(x => x.Id === id);
    selectModifierGroups[index].MaximumQuantity = value;
    var minimumSelect = document.getElementById(id + "min");
    var minimumValue = minimumSelect.value;
    var selectModifierGroupsString = JSON.stringify(selectModifierGroups);
    if (value < minimumValue) {
        minimumSelect.value = value;
        addMinimum(id, value);
    }
}

document.getElementById('Image').addEventListener('change', (e) => {
    const file = e.target.files[0];
    if (file) {
        const file = e.target.files[0];
        document.getElementById('file-name').textContent = file.name;
    }
});

function removeTag(value) {
    const customMultiSelect = document.querySelector('.custom-multi-select');
    const selectedTagsContainer = customMultiSelect.querySelector('.selected-tags');
    const tag = selectedTagsContainer.querySelector(`[data-value="${value}"]`);
    if (tag) {
        tag.remove();
    }
}

function deleteSelectedModifierGroup(id) {
    var index = selectModifierGroups.findIndex(x => x.Id === id);
    selectModifierGroups.splice(index, 1);
    var selectModifierGroupsString = JSON.stringify(selectModifierGroups);
    document.getElementById(id).checked = false;
    removeTag(id);

    $.ajax({
        type: "GET",
        url: "/Menu/GetModifierGroupData",
        data: { modifierGroupIds: selectModifierGroupsString },
        success: function (response) {
            selectModifierGroups = JSON.parse(selectModifierGroupsString);
            $("#modifierGroupdata").html(response);
        }
    });
}

function initializeFormSubmit() {
    $("#AddItemForm").off("submit").submit(function (e) {
        e.preventDefault();
        if (!$("#AddItemForm").valid()) {
            return;
        }
        if (!checkMinMaxData()) {
            toastr.error("Please enter valid minimum and maximum values");
            return;
        }
        $("#ModifierGroupIds").val(JSON.stringify(selectModifierGroups));
        var form = $(this)[0];
        var formData = new FormData(form);
        var currentCategoryId = $("#categoryId").val();
        var currentPageIndex = pageIndexItemsOfModal;
        var currentPageSize = $("#PageSize").val();
        $.ajax({
            type: "POST",
            url: "/Menu/AddItem",
            data: formData,
            contentType: false,
            processData: false,
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    selectModifierGroups = [];
                    $.ajax({
                        type: "GET",
                        url: "/Menu/ItemsFilter",
                        data: { categoryId: currentCategoryId, pageIndex: currentPageIndex, pageSize: currentPageSize },
                        success: function (data) {
                            $("#partialViewStartingInner").html(data);
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                    $("#addItemModal").modal("hide");
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                toastr.error(response.message);
            }
        });
    });
}

function checkMinMaxData() {
    let isMinMaxDataValid = true;
    selectModifierGroups.forEach(function (modifierGroup) {
        if (modifierGroup.MinimumQuantity > modifierGroup.MaximumQuantity) {
            isMinMaxDataValid = false;
        }
    });
    return isMinMaxDataValid;
}




function editItem(id) {
    $.ajax({
        type: "GET",
        url: "/Menu/GetItemData",
        data: { itemId: id },
        success: function (response) {
            $("#AddItemModalPartial").html(response);
            $("#AddItemModalHeading").text("Edit Item");
            $("#AddItemButton").text("Update");
            triggerMultiSelect();
            SyncEditChanges();
            initializeFormSubmit();
            $("#addItemModal").modal("show");
            $("IdItemEdit").val(id);
            $.validator.unobtrusive.parse("#AddItemForm");
        },
        error: function (response) {
            toastr.error(response.message);
        }
    });
    isEditCalled = true;
}

function addTag(value, text) {
    const customMultiSelect = document.querySelector('.custom-multi-select');
    const selectedTagsContainer = customMultiSelect.querySelector('.selected-tags');

    const tag = document.createElement('div');
    tag.className = 'tag';
    tag.setAttribute('data-value', value);
    tag.innerHTML = `
    ${text}
    <span class="tag-delete"></span>
    `;
    selectedTagsContainer.appendChild(tag);
    tag.querySelector('.tag-delete').addEventListener('click', () => {
        tag.remove();
        const option = selectElement.querySelector(`option[value="${value}"]`);
        if (option) {
            option.selected = false;
        }
        const checkbox = dropdownOptions.querySelector(`input[id="custom-option-${value}"]`);
        if (checkbox) {
            checkbox.checked = false;
        }
    });
}

function SyncEditChanges() {
    selectModifierGroups = JSON.parse($("#ModifierGroupIdsEdit").val());
    selectModifierGroups.forEach(function (modifierGroup) {
        document.getElementById(modifierGroup.Id).checked = true;
        addTag(modifierGroup.Id, modifierGroup.Name);
    });

    selectModifierGroups.forEach(function (modifierGroup) {
        var minimumSelect = document.getElementById(modifierGroup.Id + "min");
        var maximumSelect = document.getElementById(modifierGroup.Id + "max");
        minimumSelect.value = modifierGroup.MinimumQuantity;
        maximumSelect.value = modifierGroup.MaximumQuantity;
        var maximumOptions = maximumSelect.options;
        for (var i = 0; i < maximumOptions.length; i++) {
            if (parseInt(maximumOptions[i].value) < modifierGroup.MinimumQuantity) {
                maximumOptions[i].style.display = "none";
            } else {
                maximumOptions[i].style.display = "block";
            }
        }
    });
}

function UnSyncEditChanges() {
    let value = $("#ModifierGroupIdsEdit").val();

    if (!value) {
        console.error("ModifierGroupIdsEdit value is empty or undefined.");
        return;
    }

    try {
        selectModifierGroups = JSON.parse(value);

        selectModifierGroups.forEach(function (modifierGroup) {
            if (modifierGroup.Id) {
                document.getElementById(modifierGroup.Id).checked = false;
                deleteSelectedModifierGroup(modifierGroup.Id);
                removeTag(modifierGroup.Id);
            }
        });
    } catch (error) {
        console.error("Invalid JSON in ModifierGroupIdsEdit:", value, error);
    }
}


//edit category
function editCategory(categoryId) {
    $.ajax({
        url: '/Menu/EditCategory',
        type: 'GET',
        data: { categoryId: categoryId },
        success: function (data) {
            $("#menuModalsPartialStart").html(data);
            $("#AddCategoryModalTitle").text("Edit Category");
            $("#categoryModal").modal("show");
            $.validator.unobtrusive.parse("#addCategoryForm");
            AddCategory();
        },
        error: function (data) {
            toastr.error(data.message);
        }
    })
}

//delete category
function openModal(categoryId) {
    $("#deleteCategoryButton").click(function () {
        $.ajax({
            url: '/Menu/DeleteCategory',
            type: 'DELETE',
            data: { categoryId: categoryId },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    $.ajax({
                        type: "GET",
                        url: "/Menu/RefreshItemsPartial",
                        success: function (data) {
                            $("#partialViewStarting").html(data);
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                } else {
                    if (!data.success) {
                        if (data.message != null) {
                            toastr.error(data.message);
                        } else {
                            toastr.error("Not Authorized");
                        }
                    }
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });
}

function changeDefaultTax() {
    var value = document.getElementById("IsDefaultTaxable").checked;
    if (value) {
        $("#TaxPercentage").prop("disabled", false);
        $("#ShortCode").prop("disabled", false);
    } else {
        $("#TaxPercentage").prop("disabled", true);
        $("#ShortCode").prop("disabled", true);
    }
}


function changeLine(item) {
    if (item == "modifiers-div") {
        isItem = false;
        isModifier = true;
        document.getElementById("underline-para").style.marginLeft = "120px";
        document.getElementById("underline-para").style.width = "90px";
        document.getElementById("modifiers-div").classList.add("active-menu-item");
        document.getElementById("items-div").classList.remove("active-menu-item");
        document.getElementById("black-menu").classList.remove("d-none");
        document.getElementById("blue-menu").classList.add("d-none");
        document.getElementById("blue-modifier").classList.remove("d-none");
        document.getElementById("black-modifier").classList.add("d-none");
        document.getElementById("items-content").classList.add("d-none");
        document.getElementById("modifiers-content").classList.remove("d-none");
    } else {
        isItem = true;
        isModifier = false;
        document.getElementById("underline-para").style.marginLeft = "30px";
        document.getElementById("underline-para").style.width = "70px";
        document.getElementById("items-div").classList.add("active-menu-item");
        document.getElementById("modifiers-div").classList.remove("active-menu-item");
        document.getElementById("black-menu").classList.add("d-none");
        document.getElementById("blue-menu").classList.remove("d-none");
        document.getElementById("black-modifier").classList.remove("d-none");
        document.getElementById("blue-modifier").classList.add("d-none");
        document.getElementById("items-content").classList.remove("d-none");
        document.getElementById("modifiers-content").classList.add("d-none");
    }
}

function addItem() {
    initializeFormSubmit();
    if (isEditCalled) {
        UnSyncEditChanges();
        $.ajax({
            type: "GET",
            url: "/Menu/ResetAddItemForm",
            success: function (data) {
                selectModifierGroups = [];
                $("#AddItemModalPartial").html(data);
                triggerMultiSelect();
                initializeFormSubmit();
                $("#AddItemModalHeading").text("Add Item");
                $("#AddItemButton").text("Save");
                $("#addItemModal").modal("show");
                $.validator.unobtrusive.parse("#AddItemForm");
            },
            error: function (data) {
                console.log(data);
            }
        });
        return;
    } else {
        triggerMultiSelect();
        $.ajax({
            type: "GET",
            url: "/Menu/ResetAddItemForm",
            success: function (data) {
                selectModifierGroups = [];
                $("#AddItemModalPartial").html(data);
                triggerMultiSelect();
                initializeFormSubmit();
                $("#AddItemModalHeading").text("Add Item");
                $("#AddItemButton").text("Save");
                $("#addItemModal").modal("show");
                $.validator.unobtrusive.parse("#AddItemForm");
            },
            error: function (data) {
                console.log(data);
            }
        });
    }
    return;
}


function showDiv(divId) {
    divId.classList.remove('d-none');
    var divId2 = divId.id + "two";
    $("#" + divId2).addClass('active-category-div');
}

function hideDiv(divId) {
    divId.classList.add('d-none');
    var divId2 = divId.id + "two";
    $("#" + divId2).removeClass('active-category-div');
}

function OpenAddCategoryModal() {
    $("#CategoryName").val("");
    $("#CategoryDescription").val("");
    $("#CategoryIdAddCategoryModal").val("");
    $("#CateGoryNameError").text("");
    $("#CateGoryDescriptionError").text("");
    $("#AddCategoryModalTitle").text("Add Category");
    $("#categoryModal").modal("show");
    $.validator.unobtrusive.parse("#addCategoryForm");
    AddCategory();
}

function AddCategory() {
    $("#addCategoryForm").off("submit").on("submit", function (e) {
        e.preventDefault();
        if (!$("#addCategoryForm").valid()) {
            return;
        }
        var Id = 0;
        console.log(parseInt($("#CategoryIdAddCategoryModal").val()));
        if (parseInt($("#CategoryIdAddCategoryModal").val()) > 0)
        {
            Id = $("#CategoryIdAddCategoryModal").val()
        }
        var formData = 
        {   
            Id: Id,
            Name: $("#CategoryName").val(),
            Description: $("#CategoryDescription").val()
        }
        $.ajax({
            url: '/Menu/AddCategory',
            type: 'POST',
            data: { addEditCategoryViewModel: formData },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);

                    $.ajax({
                        type: "GET",
                        url: "/Menu/RefreshItemsPartial",
                        success: function (data) {
                            $("#partialViewStarting").html(data);
                        },
                        error: function (data) {
                            console.log(data);
                        }
                    });
                    $("#categoryModal").modal("hide");
                } else {
                    if (data.message != null) {
                        toastr.error(data.message);
                    } else {
                        toastr.error("Not Authorized");
                    }
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });
}

function showSearchBar() {
    document.getElementById("mobile-search-bar").classList.remove("d-none");
    document.getElementById("search-icon").classList.add("d-none");
}