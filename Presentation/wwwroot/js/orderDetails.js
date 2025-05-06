function downloadPdfInvoice(orderId) {
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

