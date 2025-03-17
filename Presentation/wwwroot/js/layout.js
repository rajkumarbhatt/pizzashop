var controllers = ['Dashboard', 'UserList', 'RoleAndPermission', 'Menu', 'TableAndSection', 'TaxAndFee', 'Order', 'Customer']


var url = window.location.pathname;
var controllerName = controllers.find(controller => url.includes(controller));
// console.log(controllerName);

$(document).ready(function () {
    $(".margin-left-sidebar-element").removeClass("active");
    $("#" + controllerName).addClass("active");
    $("#" + controllerName + "-mobile").addClass("active");


    $(".sidebar-font").removeClass("active");
    $("#" + controllerName + "-span").addClass("active");
    $("#" + controllerName + "-span-mobile").addClass("active");

    $(".sidebar-icons").each(function () {
        var src = $(this).attr("src");
        src = src.replace("-active.svg", ".svg");
    });
    var src = $("#" + controllerName + "-svg").attr("src");
    src = src.replace(".svg", "-active.svg"); 
    $("#" + controllerName + "-svg").attr("src", src);

    var srcMobile = $("#" + controllerName + "-svg-mobile").attr("src");
    srcMobile = srcMobile.replace(".svg", "-active.svg"); 
    console.log(srcMobile);
    $("#" + controllerName + "-svg-mobile").attr("src", srcMobile);
});
