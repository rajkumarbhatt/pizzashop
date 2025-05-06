$("#nextButton").click(function () {
  const currentPageIndex = pageIndexOfModal;
  const nextPageIndex = currentPageIndex + 1;
  const searchValue = $("#searchInput").val().toLowerCase();
  const pageSize = pageSizeOfModal;
  $.ajax({
    url: "/UserList/SearchUser",
    type: "GET",
    data: {
      pageIndex: nextPageIndex,
      pageSize: pageSize,
      searchValue: searchValue,
      sortColumn: sortColumn,
      sortColumnDirection: sortColumnDirection,
    },
    success: function (data) {
      $("#partialViewStarting").html(data);
    },
  });
});

$("#previousButton").click(function () {
  const currentPageIndex = pageIndexOfModal;
  const previousPageIndex = currentPageIndex - 1;
  const searchValue = $("#searchInput").val().toLowerCase();
  const pageSize = pageSizeOfModal;
  $.ajax({
    url: "/UserList/SearchUser",
    type: "GET",
    data: {
      pageIndex: previousPageIndex,
      pageSize: pageSize,
      searchValue: searchValue,
      sortColumn: sortColumn,
      sortColumnDirection: sortColumnDirection,
    },
    success: function (data) {
      $("#partialViewStarting").html(data);
    },
  });
});

function changePageSize(pageSize) {
  const currentPageIndex = 1;
  const searchValue = $("#searchInput").val().toLowerCase();
  $.ajax({
    url: "/UserList/SearchUser",
    type: "GET",
    data: {
      pageIndex: currentPageIndex,
      pageSize: pageSize,
      searchValue: searchValue,
      sortColumn: sortColumn,
      sortColumnDirection: sortColumnDirection,
    },
    success: function (data) {
      $("#partialViewStarting").html(data);
    },
  });
}

$("#sortNames").click(function () {
  if (ascendingName) {
    sortColumnDirection = "asc";
    ascendingName = false;
  } else {
    sortColumnDirection = "desc";
    ascendingName = true;
  }
  sortColumn = "FirstName";
  const currentPageIndex = 1;
  const pageSize = pageSizeOfModal;
  const searchValue = $("#searchInput").val().toLowerCase();
  $.ajax({
    url: "/UserList/SearchUser",
    type: "GET",
    data: {
      pageIndex: currentPageIndex,
      pageSize: pageSize,
      searchValue: searchValue,
      sortColumn: sortColumn,
      sortColumnDirection: sortColumnDirection,
    },
    success: function (data) {
      $("#partialViewStarting").html(data);
    },
  });
});

$("#sortRoles").click(function () {
  if (ascendingRole) {
    sortColumnDirection = "asc";
    ascendingRole = false;
  } else {
    sortColumnDirection = "desc";
    ascendingRole = true;
  }
  const currentPageIndex = 1;
  const pageSize = pageSizeOfModal;
  sortColumn = "RoleId";
  const searchValue = $("#searchInput").val().toLowerCase();
  $.ajax({
    url: "/UserList/SearchUser",
    type: "GET",
    data: {
      pageIndex: currentPageIndex,
      pageSize: pageSize,
      searchValue: searchValue,
      sortColumn: sortColumn,
      sortColumnDirection: sortColumnDirection,
    },
    success: function (data) {
      $("#partialViewStarting").html(data);
    },
  });
});

$("#searchInput").on("keyup", function () {
  clearTimeout($.data(this, "timer"));
  var searchValue = $(this).val().toLowerCase();
  if (searchValue == "") {
    searchValue = null;
  }
  $(this).data(
    "timer",
    setTimeout(function () {
      const currentPageIndex = 1;
      const pageSize = pageSizeOfModal;
      $.ajax({
        url: "/UserList/SearchUser",
        type: "GET",
        data: {
          pageIndex: currentPageIndex,
          pageSize: pageSize,
          searchValue: searchValue,
          sortColumn: sortColumn,
          sortColumnDirection: sortColumnDirection,
        },
        success: function (data) {
          $("#partialViewStarting").html(data);
        },
      });
    }, 300)
  );
});

function openModel(id) {
  var pageIndex = pageIndexOfModal;
  var pageSize = pageSizeOfModal;
  var searchValue = $("#searchInput").val().toLowerCase();
  var sortColumn = "FirstName";
  var sortColumnDirection = "asc";
  $("#deleteButton")
    .off("click")
    .click(function () {
      $.ajax({
        type: "DELETE",
        url: "/UserList/DeleteUser/" + id,
        data: { id: id },
        success: function (data) {
          if (data.success) {
            toastr.success(data.message);
            $.ajax({
              url: '/UserList/SearchUser',
              type: "GET",
              data: {
                pageIndex: pageIndex,
                pageSize: pageSize,
                searchValue: searchValue,
                sortColumn: sortColumn,
                sortColumnDirection: sortColumnDirection,
              },
              success: function (data) {
                $("#partialViewStarting").html(data);
              },
            });
          } else {
            toastr.error(data.message);
          }
        },
        error: function () {
          toastr.error("Error");
        },
      });
    });
}