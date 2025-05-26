var controllers = ['Dashboard', 'UserList', 'RoleAndPermission', 'Menu', 'TableAndSection', 'TaxAndFee', 'Order', 'Customer']


var url = window.location.pathname;
var controllerName = controllers.find(controller => url.includes(controller));

$(document).ready(function () {
    $(".margin-left-sidebar-element").removeClass("active");
    $("#" + controllerName).addClass("active");
    $("#" + controllerName + "-mobile").addClass("active");


    $(".sidebar-font").removeClass("active");
    $("#" + controllerName + "-span").addClass("active");
    $("#" + controllerName + "-span-mobile").addClass("active");

    $(".sidebar-icons").each(function () {
        var src = $(this).attr("src");
        if (typeof src !== 'undefined') {
            src = src.replace("-active.svg", ".svg");
        }});
    var src = $("#" + controllerName + "-svg").attr("src");
    if (typeof src !== 'undefined') {
        src = src.replace(".svg", "-active.svg");
    } 
    $("#" + controllerName + "-svg").attr("src", src);

    var srcMobile = $("#" + controllerName + "-svg-mobile").attr("src");
    if (typeof srcMobile !== 'undefined') {
        srcMobile = srcMobile.replace(".svg", "-active.svg");
    }$("#" + controllerName + "-svg-mobile").attr("src", srcMobile);
});

$("#redirectToOrderApp").click(function () {
    window.location.href = "/OrderApp";
});

$("#enable2faBtn").click(function () {
    $("#enable2faModal").modal("show");
})

$("#disable2faBtn").click(function () {
    $("#disable2faModal").modal("show");
})

$("#confirmEnable2faBtn").off("click").click(function () {
    $.ajax({
        url: '/Dashboard/EnableTwoFactorAuthentication',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                toastr.success("Two-factor authentication has been enabled successfully.");
                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            } else {
                toastr.error("Failed to enable two-factor authentication.");
            }
            $("#enable2faModal").modal("hide");
        },
        error: function () {
            toastr.error("An error occurred while enabling two-factor authentication.");
        }
    });
});

$("#confirmDisable2faBtn").off("click").click(function () {
    $.ajax({
        url: '/Dashboard/DisableTwoFactorAuthentication',
        type: 'POST',
        success: function (response) {
            if (response.success) {
                toastr.success("Two-factor authentication has been disabled successfully.");
                setTimeout(function () {
                    window.location.reload();
                }, 1000);
            } else {
                toastr.error("Failed to disable two-factor authentication.");
            }
            $("#disable2faModal").modal("hide");
        },
        error: function () {
            toastr.error("An error occurred while disabling two-factor authentication.");
        }
    });
});