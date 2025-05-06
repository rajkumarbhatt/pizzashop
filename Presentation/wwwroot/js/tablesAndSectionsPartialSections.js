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