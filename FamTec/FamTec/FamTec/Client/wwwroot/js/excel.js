// 엑셀 파일 다운로드
function saveAsFile(filename, bytesBase64) {
    console.log("ssssssssss")
    var link = document.createElement('a');
    link.download = filename + '.xlsx';
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}



function downloadFileFromStream(filename, contentBase64) {    
    const link = document.createElement('a');

    link.download = filename;
    link.href = `data:application/octet-stream;base64,${contentBase64}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function downloadFile(fileUrl, fileName) {
    const a = document.createElement('a');
    a.href = fileUrl;
    a.download = fileName;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
}