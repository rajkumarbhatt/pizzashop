$(document).ready(function () {
    $("#logoutBtn").click(function () {
        // Clear cookies
        document.cookie = "token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
        document.cookie = "email=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";

        // Clear localStorage and sessionStorage
        localStorage.clear();
        sessionStorage.clear();

        // Redirect to the login page or home page
        window.location.href = "/";

        // Prevent back navigation
        history.pushState(null, null, "/");
        window.addEventListener("popstate", function () {
            history.pushState(null, null, "/");
        });
    });
});