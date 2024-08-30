window.chartInstance = null;

window.createStackedBarChart = (canvasId, labels, dataset1, dataset2) => {
    var ctx = document.getElementById(canvasId).getContext('2d');
    window.chartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Dataset 1',
                data: dataset1,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1
            }, {
                label: 'Dataset 2',
                data: dataset2,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            }]
        },
        options: {
            responsive: false, // false : 크기고정 & true : 고정안함
            maintainAspectRatio: false, // 종횡비 유지 안함
            scales: {
                x: {
                    stacked: true, // STACK 형
                    grid: {
                        display: false, // 그리드 라인을 표시할지 여부
                        color: 'rgba(200, 200, 200, 0.8)' // 그리드 라인의 색상
                    }
                },
                y: {
                    stacked: true, // STACK 형
                    beginAtZero: true, // 축이 0부터 시작할지 여부

                    //min: 0, // 최소값
                    //max: 1, // 최대값
                    //ticks: {
                    //    stepSize: 0.5 // 틱 마크 간격을 0.5로 설정
                    //}
                }
            },
            plugins: {
                legend: { // 범례
                    display: true, // 범례 표시여부
                    position: 'top' // bottom, top, left, right 위치설정
                },
                title: {
                    display: true, // 차트 제목 표시여부
                    text: 'Custom Stacked Bar Chart' // 제목
                },
                tooltip: {
                    enabled: true, // 툴팁 표시여부
                    mode: 'index' // nearest / index / dataset 툴팁의 표시 모드
                }
            },
            animation: { // 차트 애니메이션 설정
                duration: 1000, // 애니메이션의 지속시간 단위 ms
                easing: 'easeInOutQuad' // 애니메이션의 이징 함수 easeInOutQuad | easeOutBounce 
            },
            layout: {
                padding: {
                    left: 10,
                    right: 10,
                    top: 10,
                    bottom: 10
                }
            },
            hover: { // 차트 호버 동작설정
                mode: 'nearest', // nearest | index | dataset 호버모드 설정
                animationDuration: 400 // 호버 애니메이션 지속 시간 ms
            }
        },
        plugins: [{
            // 커스텀 플러그인 정의
            id: 'customDataLabels',
            afterDatasetsDraw(chart, args, options) {
                const { ctx, data } = chart;

                // 각 데이터셋의 막대 위에 텍스트를 그림
                chart.data.datasets.forEach((dataset, i) => {
                    const meta = chart.getDatasetMeta(i);
                    if (!meta.hidden) {
                        meta.data.forEach((element, index) => {
                            // 데이터 값을 가져옴
                            const dataValue = dataset.data[index];

                            // 텍스트 스타일 설정
                            ctx.fillStyle = 'black'; // 텍스트 색상
                            ctx.font = '12px Arial'; // 텍스트 폰트
                            ctx.textAlign = 'center'; // 텍스트 정렬
                            ctx.textBaseline = 'bottom';

                            // 텍스트 위치 설정
                            const { x, y } = element.tooltipPosition();
                            ctx.fillText(dataValue, x, y - 5); // 막대 위에 텍스트 표시
                        });
                    }
                });
            }
        }]
    });
}

window.updateChartData = (dataset1, dataset2) => {
    if (window.chartInstance) {
        window.chartInstance.data.datasets[0].data = dataset1;
        window.chartInstance.data.datasets[1].data = dataset2;
        window.chartInstance.update();
    }
}