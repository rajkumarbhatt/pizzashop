function openModel(id) {
    $("#deleteButton").click(function () {
        $.ajax({
            type: "DELETE",
            url: "/UserList/DeleteUser/" + id,
            data: { id: id },
            success: function (data) {
                if (data.success) {
                    toastr.success(data.message);
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                } else {
                    toastr.error(data.message);
                }
            },
            error: function () {
                toastr.error("Error");
            }
        });
    });
}

function editUser(id) {
    window.location.href = "/UserList/EditUser/" + id;
}