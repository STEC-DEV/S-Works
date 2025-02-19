// QR 코드와 로고를 합쳐 다운로드하는 함수
function mergeQrAndLogo(qrCodeBase64, logoPath, outputFileName) {
    return new Promise((resolve, reject) => {
        // QR 코드 이미지 로드
        const qrImage = new Image();
        qrImage.src = `data:image/png;base64,${qrCodeBase64}`;

        qrImage.onload = function () {
            // 로고 이미지 로드
            const logoImage = new Image();
            logoImage.src = logoPath;

            logoImage.onload = function () {
                try {
                    // 캔버스 생성 및 초기화
                    const canvas = document.createElement("canvas");
                    const context = canvas.getContext("2d");

                    // 캔버스 크기 설정 (QR 코드 크기와 동일)
                    canvas.width = qrImage.width;
                    canvas.height = qrImage.height;
                    const logoWidth = canvas.width * 0.2;  // QR 코드 크기의 20%
                    const logoHeight = canvas.height * 0.2; // QR 코드 크기의 20%

                    //테두리
                    function drawRoundedRect(context, x, y, width, height, radius) {
                        context.beginPath();
                        context.moveTo(x + radius, y);
                        context.lineTo(x + width - radius, y);
                        context.quadraticCurveTo(x + width, y, x + width, y + radius);
                        context.lineTo(x + width, y + height - radius);
                        context.quadraticCurveTo(x + width, y + height, x + width - radius, y + height);
                        context.lineTo(x + radius, y + height);
                        context.quadraticCurveTo(x, y + height, x, y + height - radius);
                        context.lineTo(x, y + radius);
                        context.quadraticCurveTo(x, y, x + radius, y);
                        context.closePath();
                    }

                    // QR 코드 그리기
                    context.drawImage(qrImage, 0, 0, canvas.width, canvas.height);

                    // 로고 크기 계산 (QR 코드 크기의 1/4로 설정)
                    /*const logoSize = canvas.width / 4;*/
                    const logoX = (canvas.width - logoWidth) / 2;
                    const logoY = (canvas.height - logoHeight) / 2;

                    // 로고 배경 흰색 처리 (둥근 테두리 추가)
                    context.save(); // 캔버스 상태 저장
                    context.fillStyle = "white";
                    drawRoundedRect(context, logoX, logoY, logoWidth, logoHeight, 28); // 반지름 5px
                    context.fill();
                    context.restore(); // 상태 복원

                    // 로고 테두리 추가
                    context.lineWidth = 10; // 테두리 두께
                    context.strokeStyle = "white"; // 테두리 색상
                    drawRoundedRect(context, logoX, logoY, logoWidth, logoHeight, 28); // 둥근 테두리 추가
                    context.stroke();

                    // 로고 그리기
                    context.drawImage(logoImage, logoX, logoY, logoWidth, logoHeight);

                    // 캔버스 내용을 Base64로 변환
                    const mergedImageBase64 = canvas.toDataURL("image/png");

                    // 다운로드 링크 생성 및 실행
                    const link = document.createElement("a");
                    link.href = mergedImageBase64;
                    link.download = outputFileName;
                    document.body.appendChild(link); // 링크 추가
                    link.click(); // 다운로드 실행
                    document.body.removeChild(link); // 링크 제거

                    resolve("QR Code with logo downloaded successfully.");
                } catch (error) {
                    reject("Error during QR Code generation: " + error);
                }
            };

            logoImage.onerror = function () {
                reject("Failed to load the logo image.");
            };
        };

        qrImage.onerror = function () {
            reject("Failed to load the QR code image.");
        };
    });
}
