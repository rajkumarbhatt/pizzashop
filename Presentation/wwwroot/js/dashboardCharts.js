// Revenue Data Chart
var ctx = document.getElementById("revenueChart").getContext("2d");
var revenueChart = new Chart(ctx, {
	type: "line",
	data: {
		labels: revenueData.labels,
		datasets: [{
			label: "Revenue",
			data: revenueData.values,
			borderColor: "rgba(153, 102, 255, 1)",
			backgroundColor: "rgba(153, 102, 255, 0.2)",
			borderWidth: 2,
			fill: true,
			tension: 0.4,
		}, ],
	},
	options: {
		responsive: true,
		plugins: {
			legend: {
				display: true,
				position: "top",
			},
		},
		scales: {
			x: {
				title: {
					display: true,
					text: "Days",
				},
			},
			y: {
				title: {
					display: true,
					text: "Revenue",
				},
				beginAtZero: true,
			},
		},
	},
});

// Customer Growth Chart
var ctx2 = document.getElementById("customerGrowthChart").getContext("2d");
var customerGrowthChart = new Chart(ctx2, {
	type: "line",
	data: {
		labels: customerGrowthData.labels,
		datasets: [{
			label: "Customer Growth",
			data: customerGrowthData.values,
			borderColor: "rgba(153, 102, 255, 1)",
			backgroundColor: "rgba(153, 102, 255, 0.2)",
			borderWidth: 2,
			fill: true,
			tension: 0.4,
		}, ],
	},
	options: {
		responsive: true,
		plugins: {
			legend: {
				display: true,
				position: "top",
			},
		},
		scales: {
			x: {
				title: {
					display: true,
					text: "Days",
				},
			},
			y: {
				title: {
					display: true,
					text: "Customer Count",
				},
				beginAtZero: true,
			},
		},
	},
});