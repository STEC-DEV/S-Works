// 엑셀 파일 다운로드
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename + '.xlsx';
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function downloadFile(url, fileName) {
    const link = document.createElement('a');
    link.href = url;
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}



function downloadFileFromStream(fileName, contentBase64) {
    const link = document.createElement('a');
    console.log('aa')

    link.download = filename + '.xlsx';
    link.href = `data:application/octet-stream;base64,${contentBase64}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}