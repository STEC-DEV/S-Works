window.downloadQrCode = function (base64Image, fileName) {
    var link = document.createElement('a');
    link.href = 'data:image/png;base64,' + base64Image;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};