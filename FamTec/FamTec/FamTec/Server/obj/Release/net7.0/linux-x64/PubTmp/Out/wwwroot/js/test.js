// 화면 PDF 변환
function btnPrint() {
    var divContents = document.getElementById("text").innerHTML;
    var printWindow = window.open('', '', 'height=800,width=1200');
    printWindow.document.write('<html><head><title>제목이 들어갈 자리</title>');
    printWindow.document.write('<link rel="stylesheet" href="css/app.css" type="text/css" />'); // CSS 링크 추가
    printWindow.document.write('<link rel="stylesheet" href="css/Common.css" type="text/css" />'); // CSS 링크 추가
    printWindow.document.write('<link rel="stylesheet" href="FamTec.Client.styles.css" type="text/css" />'); // CSS 링크 추가
    
    printWindow.document.write('</head><body >');
    printWindow.document.write(divContents);
    printWindow.document.write('</body></html>');
    setTimeout(function () {
        printWindow.print();
        printWindow.document.close();
    }, 250);
}