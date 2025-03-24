function editUser(id) {
    window.location.href = "/UserList/EditUser/" + id;
}

document.getElementById("redirectToCreateUser").addEventListener("click", function () {
    window.location.href = "/UserList/CreateUser";
});