function changeCategoryKot(id) {
  var id = id.replace("category", "");
  var paraId = "para" + id;
  $(".para-blue-color").removeClass("active-waiting-list");
  document.getElementById(paraId).classList.add("active-waiting-list");
  id = parseInt(id);
  if (inReady) {
    $.ajax({
      type: "GET",
      url: "/KOT/GetReadyItems",
      data: { categoryId: id },
      success: function (data) {
        $("#kotCardsPartialStart").html(data);
        currentCategoryId = id;
      },
      error: function (xhr, status, error) {
        console.error("Error fetching data:", error);
      },
    });
  } else {
    $.ajax({
      type: "GET",
      url: "/KOT/GetKotByCategory",
      data: { categoryId: id },
      success: function (data) {
        $("#kotCardsPartialStart").html(data);
        currentCategoryId = id;
      },
      error: function (xhr, status, error) {
        console.error("Error fetching data:", error);
      },
    });
  }
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
      $(this).text(hours + " hrs " + minutes + " mins " + seconds + " secs");
    } else if (timeParts.length === 4) {
      // Format is "x mins y secs"
      var minutes = parseInt(timeParts[0].replace("mins", ""));
      var seconds = parseInt(timeParts[2].replace("secs", ""));
      seconds += 1;
      if (seconds >= 60) {
        seconds = 0;
        minutes += 1;
      }
      $(this).text(minutes + " mins " + seconds + " secs");
    } else if (timeParts.length === 2) {
      // Format is "y secs"
      var seconds = parseInt(timeParts[0].replace("secs", ""));
      var minutes = 0;
      seconds += 1;
      if (seconds >= 60) {
        seconds = 0;
        minutes += 1;
      } else {
        seconds = seconds;
      }
      var res = "";
      if (minutes > 0) {
        res = minutes + " mins " + seconds + " secs";
      } else {
        res = seconds + " secs";
      }
      $(this).text(res);
    }
  });
}

function clearMarkAsReadyList() {
  readyItems = [];
}
function decreaseQuanity(id) {
  var itemId = id.split("decreaseQuantity")[0];
  var orderItemId = id.split("decreaseQuantity")[1].split("-")[0];
  var itemQuantity = id.split("decreaseQuantity")[1].split("-")[1];
  var quantityId = itemId + "quantity" + orderItemId + "-" + itemQuantity;
  var quantityElement = document.getElementById(quantityId);
  var currentQuantity = parseInt(quantityElement.innerText);
  if (inReady) {
    if (currentQuantity <= 1) {
      quantityElement.innerText = 1;
    } else {
      quantityElement.innerText = currentQuantity - 1;
    }
  } else {
    if (currentQuantity <= 1) {
      quantityElement.innerText = 1;
    } else {
      quantityElement.innerText = currentQuantity - 1;
    }
  }
}

function increaseQuantity(id) {
  var itemId = id.split("increaseQuantity")[0];
  var orderItemId = id.split("increaseQuantity")[1].split("-")[0];
  var itemQuantity = id.split("increaseQuantity")[1].split("-")[1];
  var quantityId = itemId + "quantity" + orderItemId + "-" + itemQuantity;
  var quantityElement = document.getElementById(quantityId);
  var currentQuantity = parseInt(quantityElement.innerText);
  if (currentQuantity >= itemQuantity) {
    quantityElement.innerText = itemQuantity;
  } else {
    quantityElement.innerText = currentQuantity + 1;
  }
}

$(".ready-item-checkbox").change(function () {
  var checkboxId = $(this).attr("id");
  var itemId = checkboxId.split("checkbox")[0];
  var orderItemId = checkboxId.split("checkbox")[1].split("-")[0];
  var itemQuantity = checkboxId.split("checkbox")[1].split("-")[1];
  if ($(this).is(":checked")) {
    readyItems.push({
      Id: parseInt(itemId),
      OrderItemId: parseInt(orderItemId),
      Quantity: parseInt(itemQuantity),
    });
  } else {
    readyItems = readyItems.filter(function (item) {
      return (
        item.Id !== parseInt(itemId) ||
        item.OrderItemId !== parseInt(orderItemId)
      );
    });
  }
});

function markItemAsReady() {
  var listToSend = [];
  readyItems.forEach((id) => {
    var quantityId = id.Id + "quantity" + id.OrderItemId + "-" + id.Quantity;
    var readyCount = document.getElementById(quantityId).innerText;
    listToSend.push({
      Id: parseInt(id.Id),
      Quantity: parseInt(readyCount),
      OrderItemId: parseInt(id.OrderItemId),
    });
  });
  if (listToSend.length === 0) {
    if (inReady) {
      toastr.error("Please select at least one item to mark as in progress.");
      return;
    } else {
      toastr.error("Please select at least one item to mark as prepared.");
      return;
    }
  }
  $.ajax({
    type: "POST",
    url: "/KOT/MarkItemsAsReady",
    data: {
      pageIndex: pageIndexOfModal,
      readyItems: listToSend,
      orderId: orderIdOfModal,
      categoryId: currentCategoryId,
      inReady: inReady,
    },
    success: function (data) {
      $("#kotCardsPartialStart").html(data);
      $("#MarkedAsPreparedModal").modal("hide");
      setTimeout(() => {
        $(".modal-backdrop").remove();
      }, 50);
      readyItems = [];
      if (inReady) toastr.success("Items marked as in progress successfully.");
      else toastr.success("Items marked as prepared successfully.");
    },
    error: function (xhr, status, error) {
      console.error("Error fetching data:", error);
    },
  });
}

function showReadyItems() {
  $("#InProgressButton").removeClass("btn-primary").addClass("btn-contrast");
  $("#ReadyButton").removeClass("btn-contrast").addClass("btn-primary");
  $.ajax({
    type: "GET",
    url: "/KOT/GetReadyItems",
    data: { categoryId: currentCategoryId },
    success: function (data) {
      $("#kotCardsPartialStart").html(data);
      inReady = true;
    },
  });
}

function showNotReadyItems() {
  $("#InProgressButton").removeClass("btn-contrast").addClass("btn-primary");
  $("#ReadyButton").removeClass("btn-primary").addClass("btn-contrast");
  $.ajax({
    type: "GET",
    url: "/KOT/GetKotByCategory",
    data: { categoryId: currentCategoryId },
    success: function (data) {
      $("#kotCardsPartialStart").html(data);
      inReady = false;
    },
  });
}

$("#nextKotCards").click(function () {
  if (inReady) {
    $.ajax({
      type: "GET",
      url: "/KOT/GetReadyItems",
      data: { categoryId: currentCategoryId, pageIndex: pageIndexOfModal3 + 1 },
      success: function (data) {
        $("#kotCardsPartialStart").html(data);
      },
      error: function (xhr, status, error) {
        console.error("Error loading next KOT cards:", error);
      },
    });
  } else {
    $.ajax({
      type: "GET",
      url: "/KOT/GetKotByCategory",
      data: { categoryId: currentCategoryId, pageIndex: pageIndexOfModal3 + 1 },
      success: function (data) {
        $("#kotCardsPartialStart").html(data);
      },
      error: function (xhr, status, error) {
        console.error("Error loading next KOT cards:", error);
      },
    });
  }
});

$("#prevKotCards").click(function () {
  if (inReady) {
    $.ajax({
      type: "GET",
      url: "/KOT/GetKotByCategory",
      data: { categoryId: currentCategoryId, pageIndex: pageIndexOfModal3 - 1 },
      success: function (data) {
        $("#kotCardsPartialStart").html(data);
      },
      error: function (xhr, status, error) {
        console.error("Error loading previous KOT cards:", error);
      },
    });
  } else {
    $.ajax({
      type: "GET",
      url: "/KOT/GetKotByCategory",
      data: { categoryId: currentCategoryId, pageIndex: pageIndexOfModal3 - 1 },
      success: function (data) {
        $("#kotCardsPartialStart").html(data);
      },
      error: function (xhr, status, error) {
        console.error("Error loading previous KOT cards:", error);
      },
    });
  }
});

function openMarkedAsPreparedModal(orderId) {
  $.ajax({
    type: "GET",
    url: "/KOT/GetMarkedAsPreparedModal",
    data: {
      pageIndex: pageIndexOfModal2,
      orderId: orderId,
      categoryId: currentCategoryId,
      inReady: inReady,
    },
    success: function (data) {
      $("#markedAsPreparedModalDiv").html(data);
      if (inReady) $("#ModalButtonText").text("Mark As In Progress");
      else $("#ModalButtonText").text("Mark As Prepared");
      $("#MarkedAsPreparedModal").modal("show");
    },
  });
}

if (typeof connection === "undefined") {
  const connection = new signalR.HubConnectionBuilder()
      .withUrl("/orderHub")
      .build();

  connection
      .start()
      .then(() => {
      })
      .catch((err) => console.error(err.toString()));

  connection.on("ReceiveNewOrder", function () {
      if (inReady) {
          $.ajax({
              type: "GET",
              url: "/KOT/GetReadyItems",
              data: { categoryId: currentCategoryId },
              success: function (data) {
                  $("#kotCardsPartialStart").html(data);
              },
          });
      } else {
          $.ajax({
              type: "GET",
              url: "/KOT/GetKotByCategory",
              data: { categoryId: currentCategoryId },
              success: function (data) {
                  $("#kotCardsPartialStart").html(data);
              },
          });
      }
  });
}