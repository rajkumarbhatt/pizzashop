$("#SectionName").on("input", function () {
    if (this.value.length <= 0) {
        $("#SectionNameError").text("Section Name is required.");
    } else if (this.value.length > 50) {
        $("#SectionNameError").text("Section Name must be less than 50 characters.");
    } else {
        $("#SectionNameError").text("");
    }
});

$("#SectionDescription").on("input", function () {
    if (this.value.length > 150) {
        $("#SectionDescriptionError").text("Section Description must be less than 150 characters.");
    } else {
        $("#SectionDescriptionError").text("");
    }
});

function validateSectionForm () {
    var sectionName = document.getElementById("SectionName").value;
    var sectionDescription = document.getElementById("SectionDescription").value;

    if (sectionName.length <= 0) {
        $("#SectionNameError").text("Section Name is required.");
        return false;
    } else if (sectionName.length > 50) {
        $("#SectionNameError").text("Section Name must be less than 50 characters.");
        return false;
    } else {
        $("#SectionNameError").text("");
    }

    if (sectionDescription.length > 150) {
        $("#SectionDescriptionError").text("Section Description must be less than 150 characters.");
        return false;
    } else {
        $("#SectionDescriptionError").text("");
    }

    return true;
}


function addSection() {
    if (!validateSectionForm()) {
        return;
    }
    var sectionName = document.getElementById("SectionName").value;
    var sectionId = document.getElementById("SectionIdModal").value;
    var sectionDescription = document.getElementById("SectionDescription").value;
    $.ajax({
        type: "POST",
        url: '/TableAndSection/AddSection',
        data: { sectionName: sectionName, sectionDescription: sectionDescription, sectionId: sectionId },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                clearAddSectionForm();
                $.ajax({
                    type: "GET",
                    url: '/TableAndSection/SectionsFilter',
                    data: { pageIndex: 1, pageSize: pageSizeOfModal, sectionId: sectionId },
                    success: function (data) {
                        $("#items-content").html(data);
                        $("#addSectionModal").modal("hide");
                        if (sectionId == "-1") {
                            $("#SectionId").val(sectionIdFromModal);
                        }
                        makeThisSectionActive($("#SectionId").val());
                    }
                })
            } else {
                toastr.error(response.message);
            }
            $(".text-danger").text("");
        },
        error: function (response) {
            toastr.error("Unauthorized Access");
        }
    });
}

function editSection(sectionId) {
    $.ajax({
        type: "GET",
        url: '/TableAndSection/EditSection',
        data: { sectionId: sectionId },
        success: function (response) {
            document.getElementById("SectionName").value = response.name;
            document.getElementById("SectionDescription").value = response.description;
            document.getElementById("SectionIdModal").value = sectionId;
            $("#addSectionModalTitle").text("Edit Section");
            $(".text-danger").text("");
            $("#addSectionModal").modal("show");
        },
        error: function (response) {
            toastr.error("Unauthorized Access");
        }
    });
}

let isDeletingSections = false;
function deleteSection(sectionId) {
    if (isDeletingSections) return;
    isDeletingSections = true;
    $("#deleteSectionButton").off("click").click(function () {
        $.ajax({
            type: "DELETE",
            url: '/TableAndSection/DeleteSection',
            data: { sectionId: sectionId },
            success: function (response) {
                isDeletingSections = false;
                if (response.success) {
                    toastr.success(response.message);
                    $("#SectionId").val(sectionIdFromModal);
                    $.ajax({
                        type: "GET",
                        url: '/TableAndSection/SectionsFilter',
                        data: { pageIndex: 1, pageSize: pageSizeOfModal, sectionId: $("#SectionId").val() },
                        success: function (data) {
                            $("#items-content").html(data);
                            makeThisSectionActive($("#SectionId").val());
                        }
                    });
                    $("#deleteSectionModal").modal("hide");
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                isDeletingSections = false;
                toastr.error("Unauthorized Access");
            }
        });
    });  
}

let isDeletingTables = false;
function deleteSelectedTables() {
    if (isDeletingTables) return;
    isDeletingTables = true;

    if (!deleteTableIds || deleteTableIds.length === 0) {
        toastr.error("No tables selected for deletion.");
        isDeletingTables = false;  
        return;
    }
    $.ajax({
        type: "DELETE",
        url: '/TableAndSection/DeleteTables',
        data: { tableIds: deleteTableIds },
        success: function (response) {
            isDeletingTables = false;  
            if (response.success) {
                toastr.success(response.message);
                deleteTableIds = [];
                $.ajax({
                    type: "GET",
                    url: '/TableAndSection/TablesFilter',
                    data: { pageIndex: 1, pageSize: pageSizeOfModal, sectionId: $("#SectionId").val(), searchValue: $("#searchInput").val() },
                    success: function (data) {
                        $("#tablePartialView").html(data);
                    }
                });
            } else {
                toastr.error(response.message);
            }
        },
        error: function (response) {
            isDeletingTables = false;
            toastr.error("An error occurred. Please try again.");
            console.error("Error response: ", response);
        }
    });
}

function deleteTableId(id) {
    $("#deleteTableButton").off("click").click(function () {
        $.ajax({
            type: "DELETE",
            url: '/TableAndSection/DeleteTable',
            data: { tableId: id },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $.ajax({
                        type: "GET",
                        url: '/TableAndSection/TablesFilter',
                        data: { pageIndex: $("#PageIndex").val(), pageSize: pageSizeOfModal, sectionId: $("#SectionId").val()},
                        success: function (data) {
                            $("#tablePartialView").html(data);
                            $("#searchInput").val("");
                        }
                    });
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                toastr.error("Unauthorized Access");
            }
        });
    });
}

function makeThisSectionActive (sectionId) {
    var divCustomIdTable = "divTable" + sectionId;
    var divCustomIdTable2 = "divTable" + sectionId + "two";
    var divCustomId4 = "divTable" + sectionId + "four";
    var blueDotId = "blueDotTable" + sectionId;
    var grayDotId = "grayDotTable" + sectionId;
    $(".blue-dot-table").addClass('d-none');
    $(".gray-dot-table").removeClass('d-none');
    $(".active-category-div").removeClass('active-category-div');
    $(".active-nav-item").removeClass('active-nav-item');
    $("#" + divCustomId4).addClass('active-nav-item');
    $("#" + blueDotId).removeClass('d-none');
    $("#" + grayDotId).addClass('d-none');
    $("#SectionId").val(sectionId);
}

function addTable() {
    if (!$("#AddTableModalForm").valid()) {
        return;
    }
    var tableName = document.getElementById("TableName").value;
    var tableSection = document.getElementById("TableSection").value;
    var tableCapacity = document.getElementById("TableCapacity").value;
    var tableStatus = document.getElementById("TableStatus").value;
    var tableId = document.getElementById("TableId").value;
    var pageIndex = $("#PageIndex").val();
    data = {
        TableId: tableId,
        TableName: tableName,
        SectionId: tableSection,
        TableCapacity: tableCapacity,
        TableStatus: tableStatus,
    };
    $.ajax({
        type: "POST",
        url: '/TableAndSection/AddTable',
        data: { addTableViewModal: data },
        success: function (response) {
            if (response.success) {
                toastr.success(response.message);
                clearAddTableForm();
                $.ajax({
                    type: "GET",
                    url: '/TableAndSection/TablesFilter',
                    data: { pageIndex: pageIndex, pageSize: pageSizeOfModal, sectionId: tableSection, searchValue: $("#searchInput").val() },
                    success: function (data) {
                        $("#tablePartialView").html(data);
                        makeThisSectionActive(tableSection);
                    }
                });
                $("#addTableModal").modal("hide");
            } else {
                toastr.error(response.message);
            }
        },
        error: function (response) {
            toastr.error("Unauthorized Access");
        }
    });
}

$(document).on('keydown', function (e) {
    if (e.key === "Delete") {
        deleteSelectedTables();
    }
});

function clearAddTableForm() {
    document.getElementById("TableId").value = "-1";
    document.getElementById("TableName").value = "";
    document.getElementById("TableSection").value = 1;
    document.getElementById("TableCapacity").value = "";
    document.getElementById("TableStatus").value = "Available";
    document.getElementById("addTableTitle").innerText = "Add Table";   
}

function clearAddSectionForm() {
    document.getElementById("SectionIdModal").value = "-1";
    document.getElementById("SectionName").value = "";
    document.getElementById("SectionDescription").value = "";
    document.getElementById("SectionNameError").innerText = "";
    document.getElementById("addSectionModalTitle").innerText = "Add Section";
}

function openAddTableModal () {
    var sectionId = document.getElementById("SectionId").value;
    if (sectionId == "") {
        toastr.error("Please select a section first.");
        return;
    }
    document.getElementById("TableSection").value = sectionId;
    document.getElementById("TableStatus").value = "Available";
    $('.text-danger').text('');
    $("#addTableModal").modal("show");
}