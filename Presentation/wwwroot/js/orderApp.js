function AddToWaitingListFormSubmit() {
    $("#waitingTokenForm").off("submit").submit(function (e) {
        e.preventDefault();
        var form = $(this)[0];
        if (!$("#waitingTokenForm").valid()) {
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
                }
                else {
                    toastr.error(response.message);
                }
            }
        });
    });
}

$(document).ready(function () {
    $("#EmailOffCanvasWaitingToken").on("input", function () {
        clearTimeout(this.delayTimer);
        this.delayTimer = setTimeout(function () {
            var email = $("#EmailOffCanvasWaitingToken").val();
            if (email.length > 2) {
                $.ajax({
                    url: "/WaitingList/GetCustomerSuggestions",
                    type: "GET",
                    data: { email: email },
                    success: function (response) {
                        var suggestions = response.customerSuggetions;
                        var suggestionsList = $("#emailSuggestionsOffcanvas");
                        suggestionsList.empty();
                        if (suggestions.length > 0) {
                            suggestionsList.show();
                            suggestions.forEach(function (item) {
                                suggestionsList.append(
                                    `<li class="dropdown-item" onclick="fillFormOffCanvas('${item.email}', '${item.name}', '${item.mobileNumber}')">${item.email}</li>`
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
                $("#emailSuggestionsOffcanvas").hide();
            }
        }, 500);
    });

    $(document).click(function (e) {
        if (!$(e.target).closest("#EmailOffCanvasWaitingToken, #emailSuggestionsOffcanvas").length) {
            $("#emailSuggestionsOffcanvas").hide();
        }
    });
});

function fillFormOffCanvas(email, name, mobileNumber) {
    $("#EmailOffCanvasWaitingToken").val(email);
    $("#NameOffCanvasWaitingToken").val(name);
    $("#MobileNumberOffCanvasWaitingToken").val(mobileNumber);
    $("#emailSuggestionsOffcanvas").hide();
}

function clearOffCanvas() {
    $("#waitingTokenTableBody").empty();
    $("#EmailOffCanvasWaitingToken").val("");
    $("#NameOffCanvasWaitingToken").val("");
    $("#MobileNumberOffCanvasWaitingToken").val("");
    $("#NumberOfPeopleWaitingListOffCanvas").val("");
    $("#SectionDropDownOffcanvas").val("");
    $("#IdOffCanvasWaitingToken").val("-1");
    $("#emailSuggestionsOffcanvas").hide();
    $(".text-danger").text("");
    $("#EmailOffCanvasWaitingToken").attr("readonly", false);
    document.getElementById('EmailOffCanvasWaitingToken').style.backgroundColor = "white";
    document.getElementById('EmailOffCanvasWaitingToken').classList.remove("cursor-none");
}