function changeLine (item) {
    if (item == "modifiers-div") {
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

function changeCategory(item, suffix) {
    const categories = ["sandwich", "pasta", "sides", "salads", "dips", "pizza", "desserts", "burger"];
    const activeClass = "active-nav-item";
    const dNoneClass = "d-none";

    categories.forEach(cat => {
        document.getElementById(`${cat}-span${suffix}`).classList.remove(activeClass);
        document.getElementById(`${cat}-blue-dots${suffix}`).classList.add(dNoneClass);
        document.getElementById(`${cat}-black-dots${suffix}`).classList.remove(dNoneClass);
    });

    document.getElementById(`${item}-span${suffix}`).classList.add(activeClass);
    document.getElementById(`${item}-blue-dots${suffix}`).classList.remove(dNoneClass);
    document.getElementById(`${item}-black-dots${suffix}`).classList.add(dNoneClass);
}

function addEditAndDeleteOptionsOnMouseOver (item, suffix) {
    document.getElementById(`${item}${suffix}`).classList.add("active-category-div")
    document.getElementById(`${item}${suffix}-edit-icon`).classList.remove("d-none")
    document.getElementById(`${item}${suffix}-delete-icon`).classList.remove("d-none")
}

function removeEditAndDeleteOptionsOnMouseOut (item, suffix) {
    document.getElementById(`${item}${suffix}`).classList.remove("active-category-div")
    document.getElementById(`${item}${suffix}-edit-icon`).classList.add("d-none")
    document.getElementById(`${item}${suffix}-delete-icon`).classList.add("d-none")
}

function showSearchBar () {
    document.getElementById("mobile-search-bar").classList.remove("d-none");
    document.getElementById("search-icon").classList.add("d-none");
}

window.addEventListener('mouseup',function(event){
    var mobileSearchBar = document.getElementById('mobile-search-bar');
    if(event.target != mobileSearchBar && event.target.parentNode != mobileSearchBar){
        mobileSearchBar.classList.add('d-none');
        document.getElementById("search-icon").classList.remove("d-none");
    }
});  

$(document).ready(function() {
    $("#AddCategoryModal").click(function() {
        var Name = $("#CategoryName").val();
        var Description = $("#CategoryDescription").val();
        $.ajax({
            url: '/Menu/AddCategory',
            type: 'POST',
            data: { categoryName: Name, categoryDescription: Description },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function() {
                        window.location.href = "/Menu"; 
                    }, 1000);
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });

    $("#EditCategoryModal").click(function() {
        var Id = $("#EditCategoryId").val();
        var Name = $("#EditCategoryName").val();
        var Description = $("#EditCategoryDescription").val();
        $.ajax({
            url: '/Menu/UpdateCategory',
            type: 'PUT',
            data: { categoryId: Id, categoryName: Name, categoryDescription: Description },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function() {
                        window.location.href = "/Menu"; 
                    }, 1000);
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });
});

function editCategory (categoryId, categoryName, categoryDescription) {
    $("#EditCategoryName").val(categoryName);
    $("#EditCategoryDescription").val(categoryDescription);
    $("#EditCategoryId").val(categoryId);
}

function openModal (categoryId) {
    $("#deleteCategoryButton").click(function () {
        $.ajax({
            url: '/Menu/DeleteCategory',
            type: 'DELETE',
            data: { categoryId: categoryId},
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function() {
                        window.location.href = "/Menu"; 
                    }, 1000);
                } else {
                    toastr.error(data.message);
                }
            },
            error: function (data) {
                toastr.error(data.message);
            }
        });
    });
}  