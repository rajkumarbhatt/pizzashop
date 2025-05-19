let previousSelectedValue = "Current Month";
let isCustomTimeSelected = false;
$("#select-duration").on("blur", function() {
	if ($(this).val() === null) {
		if (isCustomTimeSelected) {
			$(this).val("Custom Date");
		} else {
			$(this).val(previousSelectedValue);
		}
	}
});	

$(document).ready(function() {
	$("#select-duration").change(function() {
		var selectedValue = $(this).val();
		if (selectedValue === "Custom Date") {
			isCustomTimeSelected = true;
			$("#errorMessageFromDate").text("");
			$("#errorMessageToDate").text("");
			$("#CustomDatepickerModal").modal("show");
			$("#ApplyDateChangesButton").off("click").click(function() {
				var fromDate = $("#fromDate").val();
				var toDate = $("#toDate").val();
				if (fromDate == "dd-mm-yyyy") {
					$("#errorMessageFromDate").text("Please select a valid date.");
					return;
				}
				if (toDate == "dd-mm-yyyy") {
					$("#errorMessageToDate").text("Please select a valid date.");
					return;
				}
				$.ajax({
					url: '/Dashboard/GetUpdatedData',
					type: "GET",
					data: {
						TimePeriod: selectedValue,
						fromDate: fromDate,
						toDate: toDate
					},
					success: function(data) {
						$("#dashboardPartialStarting").html(data);
						$("#errorMessageFromDate").text("");
						$("#errorMessageToDate").text("");
						$("#CustomDatepickerModal").modal("hide");
						previousSelectedValue = selectedValue;
					}
				});
			});
		} else {
			isCustomTimeSelected = false;
			previousSelectedValue = selectedValue;
			$.ajax({
				url: '/Dashboard/GetUpdatedData',
				type: 'GET',
				data: {
					TimePeriod: selectedValue
				},
				success: function(data) {
					$("#dashboardPartialStarting").html(data);
					$("#fromDate").val("dd-mm-yyyy");
					$("#toDate").val("dd-mm-yyyy");
					$("#errorMessageFromDate").text("");
					$("#errorMessageToDate").text("");
				},
				error: function(xhr, status, error) {
					console.error("Error fetching data:", error);
				}
			});
		}
	});

	$('.datepicker').datepicker({
		format: 'dd-mm-yyyy',
		autoclose: true,
		todayHighlight: true
	}).on('show', function() {
		$(this).attr('readonly', true);
	});

	$('.datepicker').datepicker('setEndDate', new Date());

	$('#fromDate').on('change', function() {
		var fromDate = $(this).datepicker('getDate');
		$('#toDate').datepicker('setStartDate', fromDate);
	});

	$('#toDate').on('change', function() {
		var toDate = $(this).datepicker('getDate');
		$('#fromDate').datepicker('setEndDate', toDate);
	});

	$("#cancelDatePickerButton").click(function() {
		$("#fromDate").val("dd-mm-yyyy");
		$("#toDate").val("dd-mm-yyyy");
		$("#errorMessageFromDate").text("");
		$("#errorMessageToDate").text("");
	});

	$("#closeDatePickerModalButton").click(function() {
		$("#fromDate").val("dd-mm-yyyy");
		$("#toDate").val("dd-mm-yyyy");
		$("#errorMessageFromDate").text("");
		$("#errorMessageToDate").text("");
	});
});

$(".closeDatePickerModalButton").click(function () {
    $("#select-duration").val(previousSelectedValue);
});





window.ChatWidgetConfig = {
	webhook: {
		url: 'https://rajat2348.app.n8n.cloud/webhook/f406671e-c954-4691-b39a-66c90aa2f103/chat',
		route: 'general'
	},
	branding: {
		logo: 'https://i.ibb.co/DHMyqyQh/pizzashop-logo.png',
		name: 'PIZZASHOP', 
		welcomeText: 'Hi 👋, how can we help?', 
		responseTimeText: 'We typically respond right away' 
	},	
	style: {
		primaryColor: '#854fff',
		secondaryColor: '#6b3fd4', 
		position: 'right',
		backgroundColor: '#ffffff', 
		fontColor: '#333333' 
	}
};
