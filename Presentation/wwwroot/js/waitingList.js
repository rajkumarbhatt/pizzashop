function openWaitingTokenModal() {
    $("#waitingTokenModal").modal("show");
    $.validator.unobtrusive.parse('#waitingTokenModal');
    AddToWaitingListFormSubmit();
}
function AddToWaitingListFormSubmit() {
    $("#waitingTokenForm").off("submit").submit(function (e) {
        e.preventDefault();
        var form = $(this)[0];
        if(!$("#waitingTokenForm").valid()) {
            return;
        }
        var formData = new FormData(form);
        $.ajax({
            url: "/OrderApp/AddToWaitingList",
            type: "POST",
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $("#waitingTokenModal").modal("hide");
                    $.ajax({
                        type: "GET",
                        url: "/WaitingList/GetWaitingList",
                        success: function (response) {
                            $('#waitingListPartial').html(response);
                            $.ajax({
                                type: "GET",
                                url: "/WaitingList/GetWaitingListBasedOnSection",
                                data: { sectionId: $("#currentSection").val() },
                                success: function (response) {
                                    $('#waitingListTablePartial').html(response);
                                    var paraId = "para" + $("#currentSection").val();
                                    $('.text-gray').removeClass('active-waiting-list');
                                    $('#' + paraId).addClass('active-waiting-list');   
                                },
                                error: function (response) {
                                    console.log(response);
                                }
                            });
                        },
                        error: function (response) {
                            console.log(response);
                        }
                    });
                }
                else {
                    toastr.error(response.message);
                }
            }
        });
    });
}

AddToWaitingListFormSubmit();

$(document).ready(function () {
    $("#EmailWaitingToken").on("input", function () {
        clearTimeout(this.delayTimer);
        this.delayTimer = setTimeout(function () {
            var email = $("#EmailWaitingToken").val();
            if (email.length > 2) {
                $.ajax({
                    url: "/WaitingList/GetCustomerSuggestions",
                    type: "GET",
                    data: { email: email },
                    success: function (response) {
                        var suggestions = response.customerSuggetions;
                        var suggestionsList = $("#emailSuggestions");
                        suggestionsList.empty();
                        if (suggestions.length > 0) {
                            suggestionsList.show();
                            suggestions.forEach(function (item) {
                                suggestionsList.append(
                                    `<li class="dropdown-item" onclick="fillForm('${item.email}', '${item.name}', '${item.mobileNumber}')">${item.email}</li>`
                                );
                            });
                        } else {
                            suggestionsList.hide();
                        }
                    },
                    error: function () {
                        console.error("Error fetching suggestions.");
                    }
                });
            } else {
                $("#emailSuggestions").hide();
            }
        }, 500);
    });

    $(document).click(function (e) {
        if (!$(e.target).closest("#EmailWaitingToken, #emailSuggestions").length) {
            $("#emailSuggestions").hide();
        }
    });
});

function fillForm(email, name, mobileNumber) {
    $("#EmailWaitingToken").val(email);
    $("#NameWaitingToken").val(name);
    $("#MobileNumberWaitingToken").val(mobileNumber);
    $("#emailSuggestions").hide();
}

function clearWaitingTokenModal() {
    $("#waitingTokenForm").trigger("reset");
    $("#waitingTokenForm").find("input[type=hidden]").val("-1");
    $("#emailSuggestions").hide();
    $(".text-danger").text("");
}

function editWaitingList(tokenNumber) {
    $.ajax({
        type: "GET",
        url: "/WaitingList/EditWaitingList",
        data: { id: tokenNumber },
        success: function (response) {
            $('#waitingListModalStarting').html(response);
            $('#waitingTokenModal').modal('show');
            $.validator.unobtrusive.parse('#waitingTokenModal');
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function deleteFromList(tokenNumber) {
    $('#deleteWaitingList').modal('show');
    $('#deleteButton').off("click").click(function () {
        $.ajax({
            type: "DELETE",
            url: "/WaitingList/DeleteWaitingList",
            data: { id: tokenNumber },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#deleteWaitingList').modal('hide');
                    $.ajax({
                        type: "GET",
                        url: "/WaitingList/GetWaitingList",
                        success: function (response) {
                            $('#waitingListPartial').html(response);
                            $.ajax({
                                type: "GET",
                                url: "/WaitingList/GetWaitingListBasedOnSection",
                                data: { sectionId: $("#currentSection").val() },
                                success: function (response) {
                                    $('#waitingListTablePartial').html(response);
                                    var paraId = "para" + $("#currentSection").val();
                                    $('.text-gray').removeClass('active-waiting-list');
                                    $('#' + paraId).addClass('active-waiting-list');
                                },
                                error: function (response) {
                                    console.log(response);
                                }
                            });
                        },
                        error: function (response) {
                            console.log(response);
                        }
                    });
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                console.log(response);
            }
        });
    });
}

function changeSection(sectionId) {
    var paraId = "para" + sectionId;
    $('.text-gray').removeClass('active-waiting-list');
    $('#' + paraId).addClass('active-waiting-list');
    $.ajax({
        type: "GET",
        url: "/WaitingList/GetWaitingListBasedOnSection",
        data: { sectionId: sectionId },
        success: function (response) {
            $('#waitingListTablePartial').html(response);
            $("#currentSection").val(sectionId);
        },
        error: function (response) {
            console.log(response);
        }
    });
}

function assignTableModalOpen(tokenNumber) {
    $('#AssignTableModal').modal('show');
    $("#assgnTableToCustomerButton").off("click").click(function () {
        if (selectedTables.length <= 0) {
            toastr.error("Please select tables to assign");
            return;
        }
        var tableId = $('#selectTable').val();
        $.ajax({
            type: "POST",
            url: "/WaitingList/AssignTable",
            data: { waitingListId: tokenNumber, tableIds: selectedTables, sectionId: $('#selectSection').val() },
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    $('#AssignTableModal').modal('hide');
                    $.ajax({
                        type: "GET",
                        url: "/WaitingList/GetWaitingList",
                        success: function (response) {
                            $('#waitingListPartial').html(response);
                            $.ajax({
                                type: "GET",
                                url: "/WaitingList/GetWaitingListBasedOnSection",
                                data: { sectionId: $("#currentSection").val() },
                                success: function (response) {
                                    $('#waitingListTablePartial').html(response);
                                    var paraId = "para" + $("#currentSection").val();
                                    $('.text-gray').removeClass('active-waiting-list');
                                    $('#' + paraId).addClass('active-waiting-list');
                                    selectedTables = []; 
                                },
                                error: function (response) {
                                    console.log(response);
                                }
                            });
                        },
                        error: function (response) {
                            console.log(response);
                        }
                    });
                } else {
                    toastr.error(response.message);
                }
            },
            error: function (response) {
                console.log(response);
            }
        });
    })

}

function getTables(sectionId) {
    $('#selectTable').empty();
    $('#selectTable').append('<option value="">Select Table</option>');
    $.ajax({
        type: "GET",
        url: "/WaitingList/GetAvailableTables",
        data: { sectionId: sectionId },
        success: function (response) {
            $('#selectTableUl').empty();
            $.each(response, function (key, value) {
                $('#selectTableUl').append('<li><input type="checkbox" style="height:1em;" class="form-check-input multi-checkbox" value="' + value.id + '"><label class="form-check-label">' + value.name + '</label></li>');
            });
            
            $('.multi-checkbox').change(function () {
                if (selectedTables.indexOf(parseInt($(this).val())) !== -1) {
                    selectedTables.splice(selectedTables.indexOf(parseInt($(this).val())), 1);
                } else {
                    selectedTables.push(parseInt($(this).val()));
                }
            });
        },
        error: function (response) {
            console.log(response);
        }
    });
}