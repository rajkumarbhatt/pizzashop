function selectTable(id, status, orderId) {
    if (status == "Assigned" || status == "Running") {
        window.location.href = "/OrderApp/Menu/" + orderId;
        return;
    }
    var tableId = id.split("table")[1].split("section")[0];
    var sectionId = id.split("section")[1];
    var tableCard = document.getElementById(id);
    tableCard.classList.toggle("selected-table-card");
    if (tableCard.classList.contains("selected-table-card")) {
        selectedTables.push({ tableId: tableId, sectionId: sectionId });
    }
    else {
        selectedTables = selectedTables.filter(function (table) {
            return table.tableId != tableId;
        });
    }
}

function assignTables(id) {
    var sectionId = id.split("section")[1];
    var selectedTablesSection = selectedTables.filter(function (table) {
        return table.sectionId == sectionId;
    });
    if (selectedTablesSection.length == 0) {
        toastr.error("Please select table(s) to assign");
        return;
    }
    var selectedTableIds = selectedTablesSection.map(function (table) {
        return parseInt(table.tableId);
    });
    $.ajax({
        url: "/OrderApp/GetWaitingListForCurrentSection",
        type: "GET",
        data: { sectionId: sectionId },
        success: function (response) {
            if (response.success) {
                var tableBody = $("#waitingTokenTableBody");
                tableBody.empty();
                if (response.customerDetails.length == 0) 
                {
                    tableBody.append(
                        `<tr>
                            <td colspan="4" class="text-center font-weight-bold-slight ">No waiting token available</td>
                        </tr>`
                    );
                } else {
                    response.customerDetails.forEach(function (item) {
                        tableBody.append(
                            `<tr>
                                <td><input onclick="fillForm2('${item.email}', '${item.name}', '${item.phoneNumber}', '${item.tokenNumber}', '${item.numberOfPersons}')" type="radio" id="waitingList" name="waitingList" /></td>
                                <td>${item.tokenNumber}</td>
                                <td>${item.name}</td>
                                <td>${item.numberOfPersons}</td>
                            </tr>`
                        );
                    });
                }
                $("#SectionDropDownOffcanvas").val(sectionId);
                $("#offcanvasRight").offcanvas("show");
            }
            else {
                toastr.error(response.message);
            }
        },
    });
    AddOrder(sectionId);
}

function AddOrder (sectionId) {
    $("#AssignTableToCustomerForm").off("submit").submit(function (e) {
         var selectedTablesSection = selectedTables.filter(function (table) {
            return table.sectionId == sectionId;
        });
        if (selectedTablesSection.length == 0) {
            toastr.error("Please select table(s) to assign");
            return;
        }
        var selectedTableIds = selectedTablesSection.map(function (table) {
            return parseInt(table.tableId);
        });
        e.preventDefault();
        var form = $(this)[0];
        if(!$(this).valid()) {
            return;
        }
        var formData = new FormData(form);
        formData.append("tableIds", JSON.stringify(selectedTableIds));
        $.ajax({
            url: "/OrderApp/AssignTablesToCustomer",
            type: "POST",
            contentType: false,
            processData: false,
            data: formData,
            success: function (response) {
                if (response.success) {
                    toastr.success(response.message);
                    selectedTables = [];
                    $("#offcanvasRight").offcanvas("hide");
                    setTimeout(function () {
                        window.location.href = "/OrderApp/Menu/" + response.orderId;    
                    }, 1000);
                }
                else {
                    toastr.error(response.message);
                }
            },
        });
    });
}

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

function fillForm2(email, name, mobileNumber, tokenNumber, noOfPersons) {
    $("#EmailOffCanvasWaitingToken").val(email);
    $("#EmailOffCanvasWaitingToken").attr("readonly", true);
    document.getElementById('EmailOffCanvasWaitingToken').style.backgroundColor = "#E9ECEF";
    document.getElementById('EmailOffCanvasWaitingToken').classList.add("cursor-none");
    $("#NameOffCanvasWaitingToken").val(name);
    $("#MobileNumberOffCanvasWaitingToken").val(mobileNumber);
    $("#IdOffCanvasWaitingToken").val(tokenNumber);
    $("#NumberOfPeopleWaitingListOffCanvas").val(noOfPersons);
}

function clearWaitingTokenModal() {
    $("#waitingTokenForm").trigger("reset");
    $("#waitingTokenForm").find("input[type=hidden]").val("-1");
    $("#emailSuggestions").hide();
    $("#waitingTokenForm").find(".text-danger").each(function () {
        $(this).text("");
    });
}



function updateTimers() {
    $(".order-timer").each(function () {
        var currentTime = $(this).text();
        if (currentTime === "N/A") {
            return;
        }
        var timeParts = currentTime.split(" ");
        if (timeParts.length === 6) {
            // Format is "a hrs x mins y secs"
            var hours = parseInt(timeParts[0].replace("hrs", ""));
            var minutes = parseInt(timeParts[2].replace("mins", ""));
            var seconds = parseInt(timeParts[4].replace("secs", ""));
            seconds += 1;
            if (seconds >= 60) {
                seconds = 0;
                minutes += 1;
            }
            if (minutes >= 60) {
                minutes = 0;
                hours += 1;
            }
            $(this).text(
                hours + " hrs " +
                minutes + " mins " +
                seconds + " secs"
            );
        }
        else if (timeParts.length === 4) {
            // Format is "x mins y secs"
            var minutes = parseInt(timeParts[0].replace("mins", ""));
            var seconds = parseInt(timeParts[2].replace("secs", ""));
            seconds += 1;
            if (seconds >= 60) {
                seconds = 0;
                minutes += 1;
            }
            $(this).text(
                minutes + " mins " +
                seconds + " secs"
            );
        }
        else if (timeParts.length === 2) {
            // Format is "y secs"
            var seconds = parseInt(timeParts[0].replace("secs", ""));
            var minutes = 0;
            seconds += 1;
            if (seconds >= 60) {
                seconds = 0;
                minutes += 1;
            }
            else {
                seconds = seconds;
            }
            var res = "";
            if (minutes > 0) {
                res = minutes + " mins " + seconds + " secs";
            }
            else {
                res = seconds + " secs";
            }
            $(this).text(
                res
            );

        }
    });
}

function openWaitingTokenModal2(id) {
    var sectionId = id.split("section")[1].split("name")[0];
    var sectioName = id.split("section")[1].split("name")[1];
    var accordianButtonId = "button" + sectionId;
    $("#WaitingListSection").empty();
    $("#WaitingListSection").append(
        `<option value="${sectionId}" selected>${sectioName}</option>`
    );
    $("#WaitingListSection").val(sectionId);
    $("#waitingTokenModal").modal("show");
    $.validator.unobtrusive.parse("#waitingTokenModal");
    AddToWaitingListFormSubmit();
}