function downloadPdfInvoice(orderId, orderStatus) {
    debugger
    if (orderStatus == 'Cancelled') {
        toastr.error("The order is cancelled. You cannot download the invoice");
        return;
    } else if (orderStatus == 'In Progress' || orderStatus == 'Pending' || orderStatus == 'Served') {
        toastr.error("The order is not yet completed. Please try again later.");
        return;
    }
    const url = '/Order/DownloadInvoice?orderId=' + orderId;
    $('.loader-container').removeClass('d-none');
    fetch(url, { method: "GET" })
        .then(response => response.blob())
        .then(blob => {
            const url = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = url;
            a.download = `Order_${orderId}.pdf`;
            document.body.appendChild(a);
            a.click();
            window.URL.revokeObjectURL(url);
            $('.loader-container').addClass('d-none');
        })
        .catch(error => {
        });
}

