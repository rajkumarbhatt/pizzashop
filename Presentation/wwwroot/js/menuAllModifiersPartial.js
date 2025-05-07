$(".select-modifs-intermediate").click(function () {
    try {
        if (this.checked) {
            $(".all-modifiers-select").each(function () {
                this.checked = true;
                selectedModifiers.push({
                    id: parseInt(this.id.split("_")[1]),
                    name: this.id.split("_")[0]
                });
                selectedModifiers = selectedModifiers.filter((v, i, a) => a.findIndex(t => (t.id === v.id)) === i);
            });
        } else {
            $(".all-modifiers-select").each(function () {
                this.checked = false;
                const modifierId = this.id.split("_")[1];
                const index = selectedModifiers.findIndex(x => x.id === parseInt(modifierId));
                selectedModifiers.splice(index, 1);
            });
        }
        addModalsToList();
    } catch (error) {
        console.error("Error in select-modifs-intermediate click handler:", error);
    }
});

$(".all-modifiers-select").click(function () {
    try {
        if ($(".all-modifiers-select:checked").length === $(".all-modifiers-select").length) {
            $(".select-modifs-intermediate").prop("checked", true);
        } else {
            $(".select-modifs-intermediate").prop("checked", false);
        }
    } catch (error) {
        console.error("Error in all-modifiers-select click handler:", error);
    }
});

function addTagCustom(modifierId, modifierName) {
    try {
        var tagsDiv = document.getElementById("modifiersSelected");
        var tag = document.createElement("div");
        tag.className = "tag ms-1";
        tag.setAttribute("data-value", modifierId);
        tag.innerHTML = `
        ${modifierName}
        <span class="tag-delete-modifier-group" style="cursor:pointer;">x</span>
        `;
        tagsDiv.appendChild(tag);
    } catch (error) {
        console.error("Error in addTagCustom:", error);
    }
}

$(document).on('click', '.tag-delete-modifier-group', function () {
    try {
        var modifierId = this.parentElement.getAttribute("data-value");
        var index = selectedModifiers.findIndex(x => x.id.toString() === modifierId);
        if (index !== -1) { 
            var nameFormatted = selectedModifiers[index].name.replace(/\s+/g, '/'); 
            var customId = `${nameFormatted}_${selectedModifiers[index].id}`;
            var checkbox = document.getElementById(customId);
            if (checkbox) {
                checkbox.checked = false;
            } else {
                console.error("Checkbox not found:", customId);
            }
            selectedModifiers.splice(index, 1);
        }
        this.parentElement.remove();
    } catch (error) {
        console.error("Error in tag-delete-modifier-group click handler:", error);
    }
});

function addToList(id) {
    try {
        let modifierId = id.split("_")[1];
        modifierId = parseInt(modifierId);
        let modifierName = id.split("_")[0];
        modifierName = modifierName.replace("/", " ");
        const modifier = {
            id: modifierId,
            name: modifierName
        };
        if (selectedModifiers.length === 0) {
            selectedModifiers.push(modifier);
        } else {
            const index = selectedModifiers.findIndex(x => x.id === modifierId);
            if (index === -1) {
                selectedModifiers.push(modifier);
            } else {
                selectedModifiers.splice(index, 1);
            }
        }
    } catch (error) {
        console.error("Error in addToList:", error);
    }
}

function addModalsToList() {
    try {
        var tagsDiv = document.getElementById("modifiersSelected");
        tagsDiv.innerHTML = "";
        if (typeof selectedModifiers !== "undefined") {
            selectedModifiers.forEach(modifier => {
                addTagCustom(modifier.id, modifier.name);
            });
        }

        document.querySelectorAll(".tag-delete").forEach(tag => {
            tag.addEventListener("click", function () {
                const modifierId = this.parentElement.getAttribute("data-value");
                const index = selectedModifiers.findIndex(x => x.id === parseInt(modifierId));
                selectedModifiers.splice(index, 1);
                this.parentElement.remove();
            });
        });
    } catch (error) {
        console.error("Error in addModalsToList:", error);
    }
}

$("#previousButtonAllModifiers").click(function () {
    try {
        const currentPageIndex = pageIndexAllModifiersFromModal;
        const nextPageIndex = currentPageIndex - 1;
        const searchValue = $("#searchInputAllModifiers").val().toLowerCase();
        const pageSize = pageSizeAllModifiersFromModal;
        $.ajax({
            url: '/Menu/AllModifiersFilter',
            type: 'GET',
            data: { pageIndex: nextPageIndex, pageSize: pageSize, searchValue: searchValue },
            success: function (data) {
                $("#allModifiersPartial").html(data);
                tickSelectedModifiers();
            }
        });
    } catch (error) {
        console.error("Error in previousButtonAllModifiers click handler:", error);
    }
});

$("#nextButtonAllModifiers").click(function () {
    try {
        const currentPageIndex = pageIndexAllModifiersFromModal;
        const nextPageIndex = currentPageIndex + 1;
        const searchValue = $("#searchInputAllModifiers").val().toLowerCase();
        const pageSize = pageSizeAllModifiersFromModal;
        $.ajax({
            url: '/Menu/AllModifiersFilter',
            type: 'GET',
            data: { pageIndex: nextPageIndex, pageSize: pageSize, searchValue: searchValue },
            success: function (data) {
                $("#allModifiersPartial").html(data);
                tickSelectedModifiers();
            }
        });
    } catch (error) {
        console.error("Error in nextButtonAllModifiers click handler:", error);
    }
});

function tickSelectedModifiers() {
    try {
        if (typeof selectedModifiers !== "undefined") {
            selectedModifiers.forEach(modifier => {
                const customId = modifier.name.replace(' ', '/') + "_" + modifier.id;
                if(document.getElementById(customId) != null){
                    document.getElementById(customId).checked = true;
                }
            });
        }
    } catch (error) {
        console.error("Error in tickSelectedModifiers:", error);
    }
}

function changePageSizeAllModifiers(pageSize) {
    try {
        const nextPageIndex = 1;
        const searchValue = $("#searchInputAllModifiers").val().toLowerCase();
        $.ajax({
            url: '/Menu/AllModifiersFilter',
            type: 'GET',
            data: { pageIndex: nextPageIndex, pageSize: pageSize, searchValue: searchValue },
            success: function (data) {
                $("#allModifiersPartial").html(data);
                tickSelectedModifiers();
            }
        });
    } catch (error) {
        console.error("Error in changePageSizeAllModifiers:", error);
    }
}

$("#searchInputAllModifiers").on("keyup", function () {
    try {
        clearTimeout($.data(this, 'timer'));
        var searchValue = $(this).val().toLowerCase();
        $(this).data('timer', setTimeout(function () {
            const currentPageIndex = 1;
            const pageSize = pageSizeAllModifiersFromModal;
            $.ajax({
                url: '/Menu/AllModifiersFilter',
                type: 'GET',
                data: { pageIndex: currentPageIndex, pageSize: pageSize, searchValue: searchValue },
                success: function (data) {
                    $("#allModifiersPartial").html(data);
                    tickSelectedModifiers();
                }
            });
        }, 300));
    } catch (error) {
        console.error("Error in searchInputAllModifiers keyup handler:", error);
    }
});

try {
    if (typeof selectedModifiers !== "undefined") {
        selectedModifiers.forEach(modifier => {
            const customId = modifier.name.replace(' ', '/') + "_" + modifier.id;
            if(document.getElementById(customId) != null){
                document.getElementById(customId).checked = true;
            }
        });
    }

    if ($(".all-modifiers-select:checked").length === $(".all-modifiers-select").length) {
        $(".select-modifs-intermediate").prop("checked", true);
    } else {
        $(".select-modifs-intermediate").prop("checked", false);
    }

    if ($(".all-modifiers-select:checked").length === 0) {
        $(".select-modifs-intermediate").prop("checked", false);
    }
} catch (error) {
    console.error("Error in initial setup:", error);
}