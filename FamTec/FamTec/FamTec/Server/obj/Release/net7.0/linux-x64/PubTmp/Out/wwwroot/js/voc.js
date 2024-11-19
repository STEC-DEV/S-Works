function qrcreate(strValue, bdname) {
    var qr = document.getElementById("qrcodes");

    // 이전 QR 코드 제거
    qr.innerHTML = "";

    var qrcode = new QRCode(qr, {
        width: 100,
        height: 100
    });

    // QR 생성
    qrcode.makeCode(strValue);
    setTimeout(() => funcImagedownload(bdname), 500);
}

function funcImagedownload(bulingName)
{
    var temp = document.getElementById("qrcodes");
    var temp1 = temp.querySelector('img');
    var temp2 = temp1.src;

    // blob 변환
    var imgData = atob(temp2.split(",")[1]);
    var len = imgData.length;
    var buf = new ArrayBuffer(len); // 비트를 담을 버퍼를 만든다.
    var view = new Uint8Array(buf); // 버퍼를 8bit Unsigned int로 담는다.
    var blob, i;

    for (i = 0; i < len; i++) {
        view[i] = imgData.charCodeAt(i) & 0xff; // 비트 마스킹을 통해 msb를 보호한다.
    }

    // Blob 객체를 image/png 타입으로 생성한다. {application/octet-stream도 가능}
    blob = new Blob([view], { type: "image/png" });
    const url = window.URL.createObjectURL(blob); // 객체 URL 생성 (페이지 내에서 Blob에 담긴 바이너리 데이터를 참조할 수 있는 URL을 생성한다.)

    const anchor = document.createElement("a"); // 가상 anchor 태그 생성
    anchor.href = url; // anchor 태그에 생성한 URL을 연결
    anchor.download = bulingName +".png"; // 다운로드 파일 이름
    anchor.click(); // 클릭이벤트 발생

    window.URL.revokeObjectURL(blob) // URL을 해제해줘야함 - 가비지컬렉터
}