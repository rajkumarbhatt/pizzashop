function changeCategoryKot(id) {
  var categoryId = id.split("-")[2];
  var search = $("#searchKotMenuItems").val();
  if (categoryId == "all") {
    categoryId = -1;
  } else if (categoryId == "favourite") {
    categoryId = -2;
  }
  var elements = document.getElementsByClassName("active-kot-sidebar-item");
  while (elements.length > 0) {
    elements[0].classList.remove("active-kot-sidebar-item");
  }
  var element = document.getElementById(id);
  var otherElement = "";
  if (id.includes("Mobile")) {
    var otherId = id.replace("Mobile", "");
    otherElement = document.getElementById(otherId);
  } else {
    var otherId = id.replace("category", "categoryMobile");
    otherElement = document.getElementById(otherId);
  }
  element.classList.add("active-kot-sidebar-item");
  otherElement.classList.add("active-kot-sidebar-item");

  $.ajax({
    url: "/OrderAppMenu/GetKotMenuItemsBasedOnCategory",
    type: "GET",
    data: { categoryId: categoryId, search: search },
    success: function (response) {
      $("#kotMenuItemsListPartialStart").html(response);
      currentCategory = categoryId;
      $("#categoriesKotMenuOffCanvas").offcanvas("hide");
    },
  });
}

$(document).ready(function () {
  $("#searchKotMenuItems").on("keyup", function () {
    clearTimeout($.data(this, "timer"));
    var search = $(this).val();
    var categoryId = $(".active-kot-sidebar-item").attr("id").split("-")[2];
    if (categoryId == "all") {
      categoryId = -1;
    } else if (categoryId == "favourite") {
      categoryId = -2;
    }
    $(this).data(
      "timer",
      setTimeout(function () {
        $.ajax({
          url: "/OrderAppMenu/SearchMenuItemsKot",
          type: "GET",
          data: { search: search, categoryId: categoryId },
          success: function (response) {
            $("#kotMenuItemsListPartialStart").html(response);
          },
        });
      }, 500)
    );
  });
});

$(document).ready(function () {
  $(".star").click(function () {
    var starId = $(this).attr("id");
    var starType = starId.split("-")[1];
    var starNumber = starId.split("-")[2];
    var filledStarId = "filledStar-" + starType + "-" + starNumber;
    var emptyStarId = "emptyStar-" + starType + "-" + starNumber;
    $("#" + emptyStarId).addClass("d-none");
    $("#" + filledStarId).removeClass("d-none");
    for (var i = 1; i <= 5; i++) {
      $("#filledStar-" + starType + "-" + i).addClass("d-none");
      $("#emptyStar-" + starType + "-" + i).removeClass("d-none");
    }
    for (var i = 1; i <= starNumber; i++) {
      $("#emptyStar-" + starType + "-" + i).addClass("d-none");
      $("#filledStar-" + starType + "-" + i).removeClass("d-none");
    }
  });
});

function saveCustomerReview() {
  var foodRating = 0;
  var serviceRating = 0;
  var ambienceRating = 0;
  for (var i = 1; i <= 5; i++) {
    if ($("#emptyStar-food-" + i).hasClass("d-none")) {
      foodRating++;
    }
    if ($("#emptyStar-service-" + i).hasClass("d-none")) {
      serviceRating++;
    }
    if ($("#emptyStar-ambience-" + i).hasClass("d-none")) {
      ambienceRating++;
    }
  }
  var orderReviewByCustomer = $("#orderReviewByCustomer").val();
  data = {
    FoodRating: foodRating,
    ServiceRating: serviceRating,
    AmbienceRating: ambienceRating,
    OrderReviewByCustomer: orderReviewByCustomer,
    OrderId: parseInt(urlTemp.substring(urlTemp.lastIndexOf("/") + 1)),
  };
  $.ajax({
    url: "/OrderAppMenu/SaveCustomerReview",
    type: "POST",
    data: { saveCustomerReviewViewModel: data },
    success: function (response) {
      $("#CustomerRatingModal").modal("hide");
      if (response.success) {
        toastr.success(response.message);
        setTimeout(function () {
          window.location.href = "/OrderApp";
        }, 1000);
      } else {
        toastr.error(response.message);
      }
    },
  });
}

function selectModifier(id) {
  if (orderId != "Menu") {
    var modifierId = parseInt(id.split("-")[1]);
    var modifierGroupId = parseInt(id.split("-")[2]);
    var minSelection = parseInt(id.split("-")[3]);
    var maxSelection = parseInt(id.split("-")[4]);
    var modifierPrice = parseFloat(id.split("-")[5]);
    var modifierName = id.split("-")[6];
    var selectedModifier = {
      ModifierId: modifierId,
      ModifierGroupId: modifierGroupId,
      ModifierPrice: modifierPrice,
      ModifierName: modifierName,
    };
    if (
      selectedModifiers.find(
        (modifier) =>
          modifier.ModifierId === modifierId &&
          modifier.ModifierGroupId === modifierGroupId
      )
    ) {
      selectedModifiers = selectedModifiers.filter(
        (modifier) =>
          !(
            modifier.ModifierId === modifierId &&
            modifier.ModifierGroupId === modifierGroupId
          )
      );
      document.getElementById(id).classList.remove("selected-modifier");
    } else {
      var currentModifierGroupSelected = selectedModifiers.filter(
        (modifier) => modifier.ModifierGroupId === modifierGroupId
      );
      if (currentModifierGroupSelected.length < maxSelection) {
        selectedModifiers.push(selectedModifier);
        document.getElementById(id).classList.add("selected-modifier");
      } else {
        toastr.error(
          "You can select maximum " +
            maxSelection +
            " modifiers from this group"
        );
      }
    }
  }
}

function clearModifiersList() {
  if (orderId != "Menu") {
    selectedModifiers = [];
    var elements = document.getElementsByClassName("modifer-card-add-item-kot");
    for (var i = 0; i < elements.length; i++) {
      elements[i].classList.remove("selected-modifier");
    }
  }
}

if (typeof orderId !== "undefined") {
  if (orderId == "Menu") {
    var addModifierButton = document.getElementById("AddModifiersButton");
    if (addModifierButton) {
      addModifierButton.classList.add("d-none");
    }
  }
}

function sameSelection(itemId) {
  var selectedModifierIds = selectedModifiers.map(function (modifier) {
    return modifier.ModifierId;
  });
  if (selectedModifierIds.length == 0) {
    var isThere = false;
    itemList.forEach(function (item) {
      if (item.ItemId == itemId && item.ModifierIds.length == 0) {
        isThere = true;
      }
    });
    if (isThere) {
      return true;
    } else {
      return false;
    }
  }
  if (itemList.length > 0) {
    for (var i = 0; i < itemList.length; i++) {
      var item = itemList[i];
      if (
        item.ItemId == itemId &&
        item.ModifierIds.length == selectedModifierIds.length
      ) {
        var same = true;
        for (var j = 0; j < item.ModifierIds.length; j++) {
          if (item.ModifierIds[j] != selectedModifierIds[j]) {
            same = false;
            break;
          }
        }
        if (same) {
          return true;
        }
      }
    }
  }
  return false;
}

function addModifiers(itemId, itemName, itemPrice, isDirect) {
  if (isDirect || checkForMinMax()) {
    var selectedModifierIds = selectedModifiers.map(function (modifier) {
      return modifier.ModifierId;
    });
    selectedModifierIds = "modifiers" + selectedModifierIds.join("-");
    if (sameSelection(itemId)) {
      increaseQuantity(
        itemId + "increaseQuantity" + selectedModifierIds,
        event
      );
      $("#selectModifiersModal").modal("hide");
      selectedModifiers = [];
      return;
    }
    var accordianItemDetails = document.getElementById("accordianItemDetails");
    var customAccordianId = "id-" + itemId + "collapse" + selectedModifierIds;
    var orderItemTotalPriceValueCustomId =
      itemId + "orderItemTotalPriceValue" + selectedModifierIds;
    var orderModifiersTotalPriceValueCustomId =
      itemId + "orderModifiersTotalPriceValue" + selectedModifierIds;
    var increaseQuantityCustomId =
      itemId + "increaseQuantity" + selectedModifierIds;
    var decreaseQuantityCustomId =
      itemId + "decreaseQuantity" + selectedModifierIds;
    var itemQuantityCustomId = itemId + "itemQuantity" + selectedModifierIds;
    var accordianButtonCustomId =
      itemId + "accordianButton" + selectedModifierIds;
    var trashIconCustomId = itemId + "trashIcon" + selectedModifierIds;
    var accordianItem = `
            <div class="accordion-item mb-2 div-custom-accordian" id="${accordianButtonCustomId}">
                <h2 class="accordion-header " id="headingOne">
                    <button class="accordion-button font-accordian" type="button" data-bs-toggle="collapse"
                        data-bs-target="#${customAccordianId}" aria-expanded="true" aria-controls="collapseOne">
                        <div class="d-flex justify-content-between align-items-center w-100 font-accordian-head">
                            <p class="w-50 font-weight-bold-slight">${itemName}</p>
                            <div class="quantity-card-order-items d-flex justify-content-center align-items-center p-1 mx-auto my-auto"
                                style="width: 15%;">
                                <p> <span class="p-2" id="${decreaseQuantityCustomId}" onclick="decreaseQuantity(id, event)">-</span><span id="${itemQuantityCustomId}">1</span><span id="${increaseQuantityCustomId}" class="p-2" onclick="increaseQuantity(id, event)">+</span></p>
                            </div>
                            <div style="width: 15%;" class="d-flex flex-column gap-2 font-accordian-head">
                                <p id="${orderItemTotalPriceValueCustomId}">${itemPrice}</p>
                                <p id="${orderModifiersTotalPriceValueCustomId}" class="font-accordian"></p>
                            </div>
                            <div id="${trashIconCustomId}" onclick="deleteItemFromOrder(id, event)">
                                <img src="/images/icons/delete.svg" class="small-image">
                            </div>
                        </div>
                    </button>
                </h2>

                <div id="${customAccordianId}" class="accordion-collapse collapse" aria-labelledby="headingOne"
                    data-bs-parent="#accordionExample">
                    <div class="accordion-body">
                        <table class="" style="width: 35%;">
                            <tbody id="selectedModifiersTable-${customAccordianId}">
                                
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        `;
    var rows = "";
    var totalModifiersPrice = 0;
    for (var i = 0; i < selectedModifiers.length; i++) {
      var modifier = selectedModifiers[i];
      totalModifiersPrice += modifier.ModifierPrice;
    }
    for (var i = 0; i < selectedModifiers.length; i++) {
      var modifier = selectedModifiers[i];
      var modifierCustomId =
        itemId +
        "modifier-" +
        modifier.ModifierName +
        "-" +
        selectedModifierIds;
      var row = `
                <tr class="font-accordian">
                    <td>•</td>
                    <td>${modifier.ModifierName}</td>
                    <td id="${modifierCustomId}">${modifier.ModifierPrice}</td>
                </tr>
            `;
      rows += row;
    }
    accordianItemDetails.insertAdjacentHTML("beforeend", accordianItem);
    var orderModifiersTotalPriceValue = document.getElementById(
      orderModifiersTotalPriceValueCustomId
    );
    var subTotalElement = document.getElementById("orderSubtotalValue");
    var Subtotal = parseFloat(subTotalElement.innerHTML) || 0;
    var selectedModifiersTable = document.getElementById(
      "selectedModifiersTable-" + customAccordianId
    );
    orderModifiersTotalPriceValue.innerHTML = totalModifiersPrice;
    subTotalElement.innerHTML = parseFloat(itemPrice) + totalModifiersPrice;
    Subtotal = Subtotal + parseFloat(itemPrice) + totalModifiersPrice;
    subTotalElement.innerHTML = Subtotal.toFixed(2);
    selectedModifiersTable.insertAdjacentHTML("beforeend", rows);
    $("#selectModifiersModal").modal("hide");
    var itemTax = itemTaxList.find((item) => item.itemId == itemId);
    var itemTaxAmount = 0;
    if (itemTax.isDefault) {
      itemTaxAmount = parseFloat(
        ((itemTax.itemPrice * itemTax.taxPercentage) / 100).toFixed(2)
      );
    }
    itemList.push({
      ItemId: parseInt(itemId),
      Quantity: parseInt(1),
      ModifierIds: selectedModifiers.map(function (modifier) {
        return modifier.ModifierId;
      }),
      OrderItemId: -1,
      ItemTax: itemTaxAmount,
    });
    selectedModifiers = [];
    if ($("#taxListContainer").hasClass("d-none")) {
      $("#taxListContainer").removeClass("d-none");
    }
    updateTaxes(Subtotal);
  }
}

function checkForMinMax() {
  var getElementsByClassName = document.getElementsByClassName(
    "min-greater-than-zero"
  );
  for (var i = 0; i < getElementsByClassName.length; i++) {
    var element = getElementsByClassName[i];
    var modifierGroupId = parseInt(
      element.getAttribute("data-modifier-group-id")
    );
    var minSelection = parseInt(element.getAttribute("data-min-selection"));
    var maxSelection = parseInt(element.getAttribute("data-max-selection"));
    var selectedModifierGroup = selectedModifiers.filter(function (modifier) {
      return modifier.ModifierGroupId === modifierGroupId;
    });
    if (selectedModifierGroup.length < minSelection) {
      toastr.error(
        "You must select minimum " + minSelection + " modifiers from this group"
      );
      return false;
    }
  }
  return true;
}

function increaseQuantity(id, event) {
  event.stopPropagation();
  var itemId = id.split("increaseQuantity")[0];
  var itemQuantityCustomId =
    itemId + "itemQuantity" + id.split("increaseQuantity")[1];
  var currentValue = parseInt(
    document.getElementById(itemQuantityCustomId).innerHTML
  );
  var newValue = currentValue + 1;
  var itemPriceElement = document.getElementById(
    itemId + "orderItemTotalPriceValue" + id.split("increaseQuantity")[1]
  );
  var itemPrice = itemPriceElement
    ? parseFloat(itemPriceElement.innerHTML / currentValue)
    : 0;
  var newItemPrice = itemPrice * newValue;
  itemPriceElement.innerHTML = newItemPrice.toFixed(2);
  var orderModifiersTotalPriceValue =
    parseFloat(
      document.getElementById(
        itemId +
          "orderModifiersTotalPriceValue" +
          id.split("increaseQuantity")[1]
      ).innerHTML / currentValue
    ) || 0;
  var orderModifiersNewTotalPriceValue =
    orderModifiersTotalPriceValue * newValue;
  document.getElementById(
    itemId + "orderModifiersTotalPriceValue" + id.split("increaseQuantity")[1]
  ).innerHTML = orderModifiersNewTotalPriceValue.toFixed(2);
  var subTotalElement = document.getElementById("orderSubtotalValue");
  var Subtotal = parseFloat(subTotalElement.innerHTML) || 0;
  document.getElementById(itemQuantityCustomId).innerHTML = newValue;
  Subtotal += itemPrice + orderModifiersTotalPriceValue;
  Subtotal = Subtotal.toFixed(2);
  subTotalElement.innerHTML = Subtotal;
  var modifierIds = id.split("increaseQuantity")[1].split("-");
  modifierIds[0] = modifierIds[0].split("modifiers")[1];
  modifierIds = modifierIds.map(function (modifierId) {
    return parseInt(modifierId);
  });
  var OrderItemId = itemList.filter(function (item) {
    if (item.ItemId == itemId) {
      if (item.ModifierIds.length == 0) {
        return item.OrderItemId;
      }
      if (
        item.ModifierIds.every((m) => modifierIds.includes(m)) &&
        modifierIds.every((m) => item.ModifierIds.includes(m))
      ) {
        return item.OrderItemId;
      }
    }
  });
  if (OrderItemId[0].OrderItemId == -1) {
    itemList.filter(function (item) {
      if (item.ItemId == itemId) {
        if (item.ModifierIds.length == 0) {
          item.Quantity = parseInt(newValue);
        }
        if (
          item.ModifierIds.every((m) => modifierIds.includes(m)) &&
          modifierIds.every((m) => item.ModifierIds.includes(m))
        ) {
          item.Quantity = parseInt(newValue);
        }
      }
    });
  } else {
    itemList.filter(function (item) {
      if (item.OrderItemId == OrderItemId[0].OrderItemId) {
        item.Quantity = parseInt(newValue);
      }
    });
  }
  updateTaxes(Subtotal);
  var selectedModifiersTable = document.getElementById(
    "selectedModifiersTable-id-" +
      itemId +
      "collapse" +
      id.split("increaseQuantity")[1]
  );
  var selectedModifiers = selectedModifiersTable.getElementsByTagName("tr");
  for (var i = 0; i < selectedModifiers.length; i++) {
    var modifierPriceElement =
      selectedModifiers[i].getElementsByTagName("td")[2];
    var modifierElementPrice = parseFloat(
      modifierPriceElement.innerHTML / currentValue
    );
    var newModifierPrice = modifierElementPrice * newValue;
    modifierPriceElement.innerHTML = newModifierPrice.toFixed(2);
  }
}

function decreaseQuantity(id, event) {
  event.stopPropagation();
  var itemId = id.split("decreaseQuantity")[0];
  var itemQuantityCustomId =
    itemId + "itemQuantity" + id.split("decreaseQuantity")[1];
  var currentValue = parseInt(
    document.getElementById(itemQuantityCustomId).innerHTML
  );
  var modifierIds = id.split("decreaseQuantity")[1].split("-");
  modifierIds[0] = modifierIds[0].split("modifiers")[1];
  modifierIds = modifierIds.map(function (modifierId) {
    return parseInt(modifierId);
  });
  var OrderItemId = itemList.filter(function (item) {
    if (item.ItemId == itemId) {
      if (item.ModifierIds.length == 0) {
        return item.OrderItemId;
      }
      if (
        item.ModifierIds.every((m) => modifierIds.includes(m)) &&
        modifierIds.every((m) => item.ModifierIds.includes(m))
      ) {
        return item.OrderItemId;
      }
    }
  });
  $.ajax({
    url: "/OrderAppMenu/CanReduceFromOrder",
    type: "GET",
    data: {
      orderItemId: OrderItemId[0].OrderItemId,
      currentQuantity: currentValue,
    },
    success(response) {
      if (response.canReduce) {
        var newValue = currentValue - 1;
        var itemPrice = parseFloat(
          document.getElementById(
            itemId +
              "orderItemTotalPriceValue" +
              id.split("decreaseQuantity")[1]
          ).innerHTML / currentValue
        );
        var newItemPrice = itemPrice * newValue;
        document.getElementById(
          itemId + "orderItemTotalPriceValue" + id.split("decreaseQuantity")[1]
        ).innerHTML = newItemPrice.toFixed(2);
        var orderModifiersTotalPriceValue =
          parseFloat(
            document.getElementById(
              itemId +
                "orderModifiersTotalPriceValue" +
                id.split("decreaseQuantity")[1]
            ).innerHTML / currentValue
          ) || 0;
        var orderModifiersNewTotalPriceValue =
          orderModifiersTotalPriceValue * newValue;
        document.getElementById(
          itemId +
            "orderModifiersTotalPriceValue" +
            id.split("decreaseQuantity")[1]
        ).innerHTML = orderModifiersNewTotalPriceValue.toFixed(2);
        var subTotalElement = document.getElementById("orderSubtotalValue");
        var Subtotal = parseFloat(subTotalElement.innerHTML);
        document.getElementById(itemQuantityCustomId).innerHTML = newValue;
        if (currentValue > 1) {
          Subtotal -= itemPrice + orderModifiersTotalPriceValue;
          Subtotal = Subtotal.toFixed(2);
          subTotalElement.innerHTML = Subtotal;
          hideTaxesDiv(Subtotal);
          if (OrderItemId[0].OrderItemId == -1) {
            itemList.filter(function (item) {
              if (item.ItemId == itemId) {
                if (item.ModifierIds.length == 0) {
                  item.Quantity = parseInt(newValue);
                }
                if (
                  item.ModifierIds.every((m) => modifierIds.includes(m)) &&
                  modifierIds.every((m) => item.ModifierIds.includes(m))
                ) {
                  item.Quantity = parseInt(newValue);
                }
              }
            });
          } else {
            itemList.filter(function (item) {
              if (item.OrderItemId == OrderItemId[0].OrderItemId) {
                item.Quantity = parseInt(newValue);
              }
            });
          }
          updateTaxes(Subtotal);
          var selectedModifiersTable = document.getElementById(
            "selectedModifiersTable-id-" +
              itemId +
              "collapse" +
              id.split("decreaseQuantity")[1]
          );
          var selectedModifiers =
            selectedModifiersTable.getElementsByTagName("tr");
          for (var i = 0; i < selectedModifiers.length; i++) {
            var modifierPriceElement =
              selectedModifiers[i].getElementsByTagName("td")[2];
            var modifierElementPrice = parseFloat(
              modifierPriceElement.innerHTML / currentValue
            );
            var newModifierPrice = modifierElementPrice * newValue;
            modifierPriceElement.innerHTML = newModifierPrice.toFixed(2);
          }
        }
        if (currentValue == 1) {
          Subtotal -= itemPrice + orderModifiersTotalPriceValue;
          Subtotal = Subtotal.toFixed(2);
          subTotalElement.innerHTML = Subtotal;
          var accordianButtonCustomId =
            itemId + "accordianButton" + id.split("decreaseQuantity")[1];
          var accordianItem = document.getElementById(accordianButtonCustomId);
          accordianItem.remove();
          itemList.filter(function (item) {
            if (item.ItemId == itemId) {
              itemList.splice(itemList.indexOf(item), 1);
            }
          });
          updateTaxes(Subtotal);
          makeTaxesZeroIfNecessary(Subtotal);
          hideTaxesDiv(Subtotal);
        }
      } else {
        toastr.error("You cannot reduce this item as the item is ready");
      }
    },
  });
  event.stopPropagation();
}

function deleteItemFromOrder(trashId, event) {
  event.stopPropagation();
  var itemId = trashId.split("trashIcon")[0];
  var modifierIds = trashId.split("trashIcon")[1].split("-");
  modifierIds[0] = modifierIds[0].split("modifiers")[1];
  modifierIds = modifierIds.map(function (modifierId) {
    return parseInt(modifierId);
  });
  var OrderItemId = itemList.filter(function (item) {
    if (item.ItemId == itemId) {
      if (item.ModifierIds.length == 0) {
        return item.OrderItemId;
      }
      if (
        item.ModifierIds.every((m) => modifierIds.includes(m)) &&
        modifierIds.every((m) => item.ModifierIds.includes(m))
      ) {
        return item.OrderItemId;
      }
    }
  });
  $.ajax({
    url: "/OrderAppMenu/CanDeleteFromOrder",
    type: "GET",
    data: { orderItemId: OrderItemId[0].OrderItemId },
    success(response) {
      if (response.canDelete) {
        var itemPrice = parseFloat(
          document.getElementById(
            trashId.split("trashIcon")[0] +
              "orderItemTotalPriceValue" +
              trashId.split("trashIcon")[1]
          ).innerHTML
        );
        var orderModifiersTotalPriceValue =
          parseFloat(
            document.getElementById(
              itemId +
                "orderModifiersTotalPriceValue" +
                trashId.split("trashIcon")[1]
            ).innerHTML
          ) || 0;
        var currentQuantity = parseInt(
          document.getElementById(
            itemId + "itemQuantity" + trashId.split("trashIcon")[1]
          ).innerHTML
        );
        var subTotalElement = document.getElementById("orderSubtotalValue");
        var Subtotal = parseFloat(subTotalElement.innerHTML) || 0;
        Subtotal -= itemPrice + orderModifiersTotalPriceValue;
        Subtotal = parseFloat(Subtotal).toFixed(2);
        subTotalElement.innerHTML = Subtotal;
        var accordianButtonCustomId =
          itemId + "accordianButton" + trashId.split("trashIcon")[1];
        var accordianItem = document.getElementById(accordianButtonCustomId);
        accordianItem.remove();
        updateTaxes(Subtotal);
        makeTaxesZeroIfNecessary(Subtotal);
        hideTaxesDiv(Subtotal);
        itemList.filter(function (item) {
          if (item.OrderItemId == OrderItemId[0].OrderItemId) {
            itemList.splice(itemList.indexOf(item), 1);
          }
        });
      } else {
        toastr.error("You cannot delete this item as the item is ready");
      }
    },
  });
}

function makeTaxesZeroIfNecessary(Subtotal) {
  if (parseInt(Subtotal) == 0) {
    var temp = document.getElementById("taxAmountIds").value;
    var taxList = document.getElementById("taxAmountIds").value.split(",");
    taxList.pop();
    for (var i = 0; i < taxList.length; i++) {
      var taxId = taxList[i];
      document.getElementById(taxId).innerHTML = parseFloat(0).toFixed(2);
    }
    var orderTotalValue = document.getElementById("orderTotalValue");
    orderTotalValue.innerHTML = parseFloat(0).toFixed(2);
  }
}

function updateTaxes(Subtotal) {
  var temp = document.getElementById("taxAmountIds").value;
  var taxList = document.getElementById("taxAmountIds").value.split(",");
  var totalTax = 0;
  taxList.pop();
  for (var i = 0; i < taxList.length; i++) {
    var taxId = taxList[i];
    var taxType = taxId.split("-")[0];
    var taxAmount = taxId.split("-")[3];
    if (taxType == "Percentage") {
      var taxValue = (parseFloat(taxAmount) / 100) * Subtotal;
      totalTax += taxValue;
      document.getElementById(taxId).innerHTML = taxValue.toFixed(2);
      if (taxListNameAndAmount.length == 0) {
        taxListNameAndAmount.push({
          TaxName: taxId.split("-")[1],
          TaxAmount: taxValue.toFixed(2),
        });
      } else {
        var found = false;
        for (var j = 0; j < taxListNameAndAmount.length; j++) {
          if (taxListNameAndAmount[j].TaxName == taxId.split("-")[1]) {
            found = true;
            taxListNameAndAmount[j].TaxAmount = taxValue.toFixed(2);
            break;
          }
        }
        if (!found) {
          taxListNameAndAmount.push({
            TaxName: taxId.split("-")[1],
            TaxAmount: taxValue.toFixed(2),
          });
        }
      }
    } else if (taxType == "Fixed Amount") {
      var taxValue = parseFloat(taxAmount);
      totalTax += taxValue;
      if (taxListNameAndAmount.length == 0) {
        taxListNameAndAmount.push({
          TaxName: taxId.split("-")[1],
          TaxAmount: taxValue.toFixed(2),
        });
      } else {
        var found = false;
        for (var j = 0; j < taxListNameAndAmount.length; j++) {
          if (taxListNameAndAmount[j].TaxName == taxId.split("-")[1]) {
            found = true;
            taxListNameAndAmount[j].TaxAmount = taxValue.toFixed(2);
            break;
          }
        }
        if (!found) {
          taxListNameAndAmount.push({
            TaxName: taxId.split("-")[1],
            TaxAmount: taxValue.toFixed(2),
          });
        }
      }
      if (taxId.split("-")[1] != "Service Tax") {
        document.getElementById(taxId).innerHTML = taxValue.toFixed(2);
      }
    }
  }
  var otherTax = 0;
  itemList.forEach(function (item) {
    if (item.ItemTax > 0) {
      otherTax += item.ItemTax * item.Quantity;
    }
  });
  if (taxListNameAndAmount.find((item) => item.TaxName == "Other Tax")) {
    taxListNameAndAmount.find((item) => item.TaxName == "Other Tax").TaxAmount =
      otherTax.toFixed(2);
  } else {
    taxListNameAndAmount.push({
      TaxName: "Other Tax",
      TaxAmount: otherTax.toFixed(2),
    });
  }
  totalTax = parseFloat(totalTax).toFixed(2);
  var orderTotalValue = document.getElementById("orderTotalValue");
  var otherTaxValue = document.getElementById("otherTaxValue");
  if (otherTax > 0) {
    otherTaxValue.innerHTML = otherTax.toFixed(2);
  } else {
    otherTaxValue.innerHTML = 0;
  }
  var orderTotal =
    parseFloat(Subtotal) + parseFloat(totalTax) + parseFloat(otherTax);
  if ($("#serviceTaxCheckBox").is(":checked") == false) {
    orderTotal -= parseFloat(serviceTaxAmount);
  }
  orderTotal = parseFloat(orderTotal).toFixed(2);
  orderTotalValue.innerHTML = orderTotal;
}

function hideTaxesDiv(subTotal) {
  var taxListContainer = document.getElementById("taxListContainer");
  if (parseInt(subTotal) == 0) {
    taxListContainer.classList.add("d-none");
  } else {
    taxListContainer.classList.remove("d-none");
  }
}

function openItemInstructionsModal(itemId, id) {
  var modifierIds = id.split("accordianButton")[1].split("-");
  modifierIds[0] = modifierIds[0].split("modifiers")[1];
  modifierIds = modifierIds.map(function (modifierId) {
    return parseInt(modifierId);
  });
  var OrderItemId = itemList.filter(function (item) {
    if (item.ItemId == itemId) {
      if (item.ModifierIds.length == 0) {
        return item.OrderItemId;
      }
      if (
        item.ModifierIds.every((m) => modifierIds.includes(m)) &&
        modifierIds.every((m) => item.ModifierIds.includes(m))
      ) {
        return item.OrderItemId;
      }
    }
  });
  $.ajax({
    url: "/OrderAppMenu/GetItemWiseComment",
    type: "GET",
    data: { orderItemId: OrderItemId[0].OrderItemId },
    success: function (response) {
      $("#ItemNameInstruction").val(response.message);
      $("#ItemIdCurrentInput").val(OrderItemId[0].OrderItemId);
      $("#ItemwiseComment").modal("show");
      itemIdCurrent = itemId;
    },
    error: function () {
      console.error("error fetching order wise comment");
    },
  });
}

function syncItemsList() {
  orderItems.forEach((item) => {
    var selectedModifierIds =
      "modifiers" +
      item.modifiers.map((modifier) => parseInt(modifier.modifierId)).join("-");
    var itemId2 = item.itemId;
    var accordianItemDetails = document.getElementById("accordianItemDetails");
    var customAccordianId = "id-" + itemId2 + "collapse" + selectedModifierIds;
    var orderItemTotalPriceValueCustomId =
      itemId2 + "orderItemTotalPriceValue" + selectedModifierIds;
    var orderModifiersTotalPriceValueCustomId =
      itemId2 + "orderModifiersTotalPriceValue" + selectedModifierIds;
    var increaseQuantityCustomId =
      itemId2 + "increaseQuantity" + selectedModifierIds;
    var decreaseQuantityCustomId =
      itemId2 + "decreaseQuantity" + selectedModifierIds;
    var itemQuantityCustomId = itemId2 + "itemQuantity" + selectedModifierIds;
    var accordianButtonCustomId =
      itemId2 + "accordianButton" + selectedModifierIds;
    var trashIconCustomId = itemId2 + "trashIcon" + selectedModifierIds;
    var accordianItem = `
            <div class="accordion-item mb-2 div-custom-accordian" onclick="openItemInstructionsModal(${item.itemId}, id)" id="${accordianButtonCustomId}">
                <h2 class="accordion-header " id="headingOne">
                    <button class="accordion-button font-accordian" type="button" data-bs-toggle="collapse"
                        data-bs-target="#${customAccordianId}" aria-expanded="true" aria-controls="collapseOne">
                        <div class="d-flex justify-content-between align-items-center w-100 font-accordian-head">
                            <p class="w-50 font-weight-bold-slight">${item.itemName}</p>
                            <div class="quantity-card-order-items d-flex justify-content-center align-items-center p-1 mx-auto my-auto"
                                style="width: 15%;">
                                <p> <span class="p-2" id="${decreaseQuantityCustomId}" onclick="decreaseQuantity(id, event)">-</span><span id="${itemQuantityCustomId}">${item.itemQuantity}</span><span id="${increaseQuantityCustomId}" class="p-2" onclick="increaseQuantity(id, event)">+</span></p>
                            </div>
                            <div style="width: 15%;" class="d-flex flex-column gap-2 font-accordian-head">
                                <p id="${orderItemTotalPriceValueCustomId}">${item.itemTotalPrice}</p>
                                <p id="${orderModifiersTotalPriceValueCustomId}" class="font-accordian">${item.modifiersTotalPrice}</p>
                            </div>
                            <div id="${trashIconCustomId}" onclick="deleteItemFromOrder(id, event)">
                                <img src="/images/icons/delete.svg" class="small-image">
                            </div>
                        </div>
                    </button>
                </h2>

                <div id="${customAccordianId}" class="accordion-collapse collapse" aria-labelledby="headingOne"
                    data-bs-parent="#accordionExample">
                    <div class="accordion-body">
                        <table class="" style="width: 35%;">
                            <tbody id="selectedModifiersTable-${customAccordianId}">
                                
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        `;
    var rows = "";
    for (var i = 0; i < item.modifiers.length; i++) {
      var modifier = item.modifiers[i];
      var modifierCustomId =
        itemId2 +
        "modifier-" +
        modifier.ModifierName +
        "-" +
        selectedModifierIds;
      var row = `
                <tr class="font-accordian">
                    <td>•</td>
                    <td>${modifier.modifierName}</td>
                    <td id="${modifierCustomId}">${modifier.modifierPrice}</td>
                </tr>
            `;
      rows += row;
    }
    accordianItemDetails.insertAdjacentHTML("beforeend", accordianItem);
    var selectedModifiersTable = document.getElementById(
      "selectedModifiersTable-" + customAccordianId
    );
    selectedModifiersTable.insertAdjacentHTML("beforeend", rows);
    var itemTax = itemTaxList.find((item) => item.itemId == itemId2);
    var itemTaxAmount = 0;
    if (itemTax.isDefault) {
      itemTaxAmount = parseFloat(
        ((itemTax.itemPrice * itemTax.taxPercentage) / 100).toFixed(2)
      );
    }
    itemList.push({
      ItemId: parseInt(itemId2),
      Quantity: parseInt(item.itemQuantity),
      ModifierIds: item.modifiers.map(function (modifier) {
        return modifier.modifierId;
      }),
      OrderItemId: item.orderItemId,
      ItemTax: itemTaxAmount,
    });
    var Subtotal =
      parseFloat(document.getElementById("orderSubtotalValue").innerHTML) || 0;
    if ($("#taxListContainer").hasClass("d-none")) {
      $("#taxListContainer").removeClass("d-none");
    }
    updateTaxes(Subtotal);
  });
}

$("#serviceTaxCheckBox")
  .off("change")
  .change(function () {
    if ($(this).is(":checked")) {
      var serviceTaxElement =
        document.getElementsByClassName("service-tax-value");
      serviceTaxElement[0].innerHTML = serviceTaxAmount;
      var updatedTotal =
        parseFloat(document.getElementById("orderTotalValue").innerHTML) +
        parseFloat(serviceTaxAmount);
      document.getElementById("orderTotalValue").innerHTML =
        updatedTotal.toFixed(2);
      taxListNameAndAmount[element].TaxAmount = "49";
    } else {
      var serviceTaxElement =
        document.getElementsByClassName("service-tax-value");
      serviceTaxElement[0].innerHTML = 0;
      var updatedTotal =
        parseFloat(document.getElementById("orderTotalValue").innerHTML) -
        parseFloat(serviceTaxAmount);
      document.getElementById("orderTotalValue").innerHTML =
        updatedTotal.toFixed(2);
    }
  });

function saveOrder() {
  if (serviceTaxCheckBox.checked == false) {
    for (var i = 0; i < taxListNameAndAmount.length; i++) {
      if (taxListNameAndAmount[i].TaxName == "Service Tax") {
        taxListNameAndAmount[i].TaxAmount = "0.00";
      }
    }
  }
  var data = {
    OrderItems: itemList,
    Subtotal: parseFloat($("#orderSubtotalValue").text()),
    Total: parseFloat($("#orderTotalValue").text()),
    OrderId: orderIdTemp,
    OrderTaxes: taxListNameAndAmount,
  };
  $.ajax({
    type: "POST",
    url: "/OrderAppMenu/SaveOrder",
    data: { saveOrderViewModel: data },
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        $.ajax({
          url: "/OrderAppMenu/GetOrderDetails",
          method: "GET",
          data: { orderId: orderIdTemp },
          success: function (response) {
            itemList = [];
            $("#orderItemDetailsPartial").html(response);
          },
          error: function () {
            toastr.error("Failed to load order details.");
          },
        });
      } else {
        toastr.error(response.message);
      }
    },
    error: function (response) {
      toastr.error("An error occurred while saving the order.");
    },
  });
}

function downloadPdfInvoice() {
  console.log("downloadPdfInvoice called with orderIdTemp:", orderIdTemp);
  debugger
  const url = "/Order/DownloadInvoice?orderId=" + orderIdTemp;
  fetch(url, { method: "GET" })
    .then((response) => response.blob())
    .then((blob) => {
      const url = window.URL.createObjectURL(blob);
      const a = document.createElement("a");
      a.href = url;
      a.download = `Order_${orderId}.pdf`;
      document.body.appendChild(a);
      a.click();
      window.URL.revokeObjectURL(url);
      $(".loader-container").addClass("d-none");
    })
    .catch((error) => {});
}

function completeOrder() {
  $("#CompleteOrderModal").modal("show");
  $("#completeOrderButton")
    .off("click")
    .on("click", function () {
      $.ajax({
        type: "POST",
        url: "/OrderAppMenu/CompleteOrder",
        data: { orderId: orderIdTemp },
        success: function (response) {
          if (response.success) {
            toastr.success(response.message);
            $("#CompleteOrderModal").modal("hide");
            $("#CustomerRatingModal").modal("show");
          } else {
            toastr.error(response.message);
          }
        },
        error: function (xhr, status, error) {
          console.error("Error:", error);
        },
      });
    });
}

function cancelOrder() {
  $("#CancelOrderModal").modal("show");
  $("#cancelOrderButton")
    .off("click")
    .on("click", function () {
      $.ajax({
        type: "POST",
        url: "/OrderAppMenu/CancelOrder",
        data: { orderId: orderIdTemp },
        success: function (response) {
          if (response.success) {
            toastr.success(response.message);
            setTimeout(function () {
              window.location.href = "/OrderApp";
            }, 1000);
            $("#CancelOrderModal").modal("hide");
          } else {
            toastr.error(response.message);
          }
        },
        error: function (xhr, status, error) {
          console.error("Error:", error);
        },
      });
    });
}

function selectModifiers(id, name, price) {
  $.ajax({
    url: "/OrderAppMenu/AreModifiersSelected",
    type: "GET",
    data: { itemId: id },
    success: function (data) {
      if (data.areModifiersSelected) {
        $.ajax({
          type: "GET",
          url: "/OrderAppMenu/GetSelectModifiersModalData",
          data: { itemId: id },
          success: function (data) {
            $("#SelectModifiersPartialStaring").html(data);
            $("#selectModifiersModal").modal("show");
          },
          error: function (xhr, status, error) {
            console.error("Error loading modifiers:", error);
          },
        });
      } else {
        addModifiers(id, name, price, true);
      }
    },
    error: function (xhr, status, error) {
      console.error(xhr.responseText);
    },
  });
}

function addToFavourites(id, event) {
  var itemId = parseInt(id.split("empty-heart-")[1]);
  $("#" + id).addClass("d-none");
  $("#filled-heart-" + itemId).removeClass("d-none");
  $.ajax({
    url: "/OrderAppMenu/AddToFavourites",
    type: "PUT",
    data: { itemId: itemId },
    success: function (response) {
      if (response.success) {
        // console.log(response.message);
      } else {
        console.error(response.message);
      }
    },
    error: function () {
      console.error("error adding item to favourites");
    },
  });
  event.stopPropagation();
}

function removeFromFavourites(id, event) {
  var itemId = parseInt(id.split("filled-heart-")[1]);
  $("#" + id).addClass("d-none");
  $("#empty-heart-" + itemId).removeClass("d-none");
  $.ajax({
    url: "/OrderAppMenu/DeleteFromFavourites",
    type: "DELETE",
    data: { itemId: itemId },
    success: function (response) {
      if (response.success) {
        if (currentCategory == -2) {
          $("#category-kot-favourite").click();
        }
      } else {
        console.error(response.message);
      }
    },
    error: function () {
      console.error("error adding item to favourites");
    },
  });
  event.stopPropagation();
}

if (typeof orderId !== "undefined") {
  if (orderId == "Menu") {
    document.getElementById("thisDivIsImportant").style.width =
      "100% !important";
    var secondImpDev = document.getElementById("thisIsSecondImportant");
    if (secondImpDev) {
      document.getElementById("thisIsSecondImportant").classList.add("d-none");
    }
    document
      .getElementById("orderAppNavbarButtonsToHide")
      .classList.remove("d-md-none");
    document
      .getElementById("orderAppNavbarButtonsToHide")
      .classList.add("d-none");
  } else if (orderId == 0) {
    document.getElementById("thisDivIsImportant").style.width = "100%";
    document
      .getElementById("orderAppNavbarButtonsToHide")
      .classList.add("d-md-none");
    document
      .getElementById("orderAppNavbarHamburgerToHide")
      .classList.add("d-none");
  } else {
    document.getElementById("thisDivIsImportant").style.width = "66%";
    document.getElementById("thisIsSecondImportant").classList.remove("d-none");
    if (window.innerWidth > 1600) {
      document.getElementById("thisIsSecondImportant").style.width = "34%";
    } else {
      document.getElementById("thisIsSecondImportant").style.width = "100%";
    }
    document
      .getElementById("orderAppNavbarButtonsToHide")
      .classList.remove("d-md-none");
    document
      .getElementById("orderAppNavbarHamburgerToHide")
      .classList.remove("d-none");
  }
}

function getCustomerDetails() {
  $.ajax({
    url: "/OrderAppMenu/GetCustomerDetails",
    type: "GET",
    data: { orderId: orderId },
    success: function (response) {
      $("#CustomerDetailsModalPartialStart1").html(response);
      $("#CustomerDetailsMenuModal").modal("show");
      $.validator.unobtrusive.parse("#CustomerDetailsForm");
      submitCustomerDetailsForm();
    },
    error: function () {
      console.error("error fetching customer details");
    },
  });
}

function submitCustomerDetailsForm() {
  $("#CustomerDetailsForm")
    .off("submit")
    .on("submit", function (e) {
      e.preventDefault();
      var form = $(this);
      if (!$("#CustomerDetailsForm").valid()) {
        return;
      }
      var formData = new FormData(this);
      $.ajax({
        type: "POST",
        url: "/OrderAppMenu/UpdateCustomerDetails",
        data: formData,
        processData: false,
        contentType: false,
        success: function (response) {
          if (response.success) {
            toastr.success(response.message);
            $("#CustomerDetailsMenuModal").modal("hide");
          } else {
            toastr.error(response.message);
          }
        },
        error: function () {
          toastr.error("Error occurred while submitting the form.");
        },
      });
    });
}

function getOrderWiseComment() {
  $.ajax({
    url: "/OrderAppMenu/GetOrderWiseComment",
    type: "GET",
    data: { orderId: orderId },
    success: function (response) {
      $("#floatingTextarea2").val(response.message);
      $("#OrderCommentModal").modal("show");
      $.validator.unobtrusive.parse("#OrderCommentModal");
    },
    error: function () {
      console.error("error fetching order wise comment");
    },
  });
}

function addOrderWiseComment() {
  var comment = $("#floatingTextarea2").val();
  $.ajax({
    url: "/OrderAppMenu/AddOrderWiseComment",
    type: "POST",
    data: { orderId: orderId, comment: comment },
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        $("#OrderCommentModal").modal("hide");
        $.ajax({
          url: "/OrderAppMenu/GetOrderDetails",
          method: "GET",
          data: { orderId: orderIdTemp },
          success: function (response) {
            itemList = [];
            $("#orderItemDetailsPartial").html(response);
          },
          error: function () {
            toastr.error("Failed to load order details.");
          },
        });
      } else {
        toastr.error(response.message);
      }
    },
    error: function () {
      toastr.error("Error occurred while submitting the form.");
    },
  });
}

function addItemWiseComment() {
  var modifierIds;
  var comment = $("#ItemNameInstruction").val();
  $.ajax({
    url: "/OrderAppMenu/AddItemWiseComment",
    type: "POST",
    data: { orderItemId: $("#ItemIdCurrentInput").val(), comment: comment },
    success: function (response) {
      if (response.success) {
        toastr.success(response.message);
        $("#ItemwiseComment").modal("hide");
      } else {
        toastr.error(response.message);
      }
    },
    error: function () {
      toastr.error("Error occurred while submitting the form.");
    },
  });
}

function clearOrderCommentForm() {
  $("#floatingTextarea2").val("");
}

function clearItemCommentForm() {
  $("#ItemNameInstruction").val("");
}

function showOrderDetailsTab() {
  if (orderId != "Menu") {
    $("#thisDivIsImportant").addClass("d-none");
    document.getElementById("thisIsSecondImportant").style.display = "block";
    document.getElementById("thisIsSecondImportant").style.width = "100%";
    $("#hideOrderDetailsTabButton").removeClass("d-none");
  }
}

function hideOrderDetailsTab() {
  if (orderId != "Menu") {
    $("#thisDivIsImportant").removeClass("d-none");
    document.getElementById("thisIsSecondImportant").style.display = "none";
    document.getElementById("thisDivIsImportant").style.width = "100%";
  }
  $("#hideOrderDetailsTabButton").addClass("d-none");
}

if (typeof orderId !== "undefined") {
  if (orderId != "Menu") {
    $(window).on("resize", function () {
      document.getElementById("thisDivIsImportant").style.width = "64%";
      document.getElementById("thisDivIsImportant").style.display =
        "block !important";
    });
  } else {
      document.getElementById("thisDivIsImportant").style.width = "100%";
    }
}

function showErrorToastr() {
  toastr.error("The order is still in progress");
}

function redirectToOrderApp() {
  setTimeout(function () {
    window.location.href = "/OrderApp";
  }, 500);
}
