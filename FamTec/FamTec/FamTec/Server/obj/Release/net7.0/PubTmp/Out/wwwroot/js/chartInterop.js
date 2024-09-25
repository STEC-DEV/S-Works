window.chartInstance = null;

window.createStackedBarChart = (canvasId, labels, dataset1, dataset2, dataset3, dataset4, dataset5, dataset6, dataset7, dataset8, dataset9) => {
    console.log(dataset1);
    console.log(dataset2);
    console.log(dataset3);
    console.log(dataset4);
    console.log(dataset5);
    console.log(dataset6);
    console.log(dataset7);
    console.log(dataset8);
    console.log(dataset9);

    var ctx = document.getElementById(canvasId).getContext('2d');
    window.chartInstance = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [
                {

                label: '미분류',
                data: dataset1,
                backgroundColor: 'rgba(255, 99, 132, 0.2)',
                borderColor: 'rgba(255, 99, 132, 1)',
                borderWidth: 1
            },
            {
                label: '기계',
                data: dataset2,
                backgroundColor: 'rgba(54, 162, 235, 0.2)',
                borderColor: 'rgba(54, 162, 235, 1)',
                borderWidth: 1
            },
            {
                label: '전기',
                data: dataset3,
                backgroundColor: 'rgba(102, 102, 204, 0.2)',
                borderColor: 'rgba(102, 102, 255, 1)',
                borderWidth: 1
            },
            {
                label: '승강',
                data: dataset4,
                backgroundColor: 'rgba(153, 204, 153, 0.2)',
                borderColor: 'rgba(204, 255, 204, 1)',
                borderWidth: 1
            },
            {
                label: '건축',
                data: dataset5,
                backgroundColor: 'rgba(102, 051, 153, 0.2)',
                borderColor: 'rgba(102, 051, 204, 1)',
                borderWidth: 1
            },
            {
                label: '소방',
                data: dataset6,
                backgroundColor: 'rgba(102, 051, 000, 0.2)',
                borderColor: 'rgba(204, 153, 102, 1)',
                borderWidth: 1
            },
            {
                label: '통신',
                data: dataset7,
                backgroundColor: 'rgba(000, 102, 153, 0.2)',
                borderColor: 'rgba(051, 153, 204, 1)',
                borderWidth: 1
            },
            {
                label: '미화',
                data: dataset8,
                backgroundColor: 'rgba(102, 204, 000, 0.2)',
                borderColor: 'rgba(153, 255, 051, 1)',
                borderWidth: 1
            },
            {
                label: '보안',
                data: dataset9,
                backgroundColor: 'rgba(204, 000, 102, 0.2)',
                borderColor: 'rgba(255, 051, 153, 1)',
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
                    },
                    categoryPercentage: 0.8, // Adjust this value to control the width of each category (space between groups)
                    barPercentage: 0.9 // Adjust this value to control the width of each bar within a category
                },
                y: {
                    stacked: true, // STACK 형
                    beginAtZero: true, // 축이 0부터 시작할지 여부

                    //min: 0, // 최소값
                    //max: 1, // 최대값
                    ticks: {
                        stepSize: 1 // 틱 마크 간격을 0.5로 설정
                    }
                }
            },
            plugins: {
                legend: { // 범례
                    display: true, // 범례 표시여부
                    position: 'top' // bottom, top, left, right 위치설정
                },
                title: {
                    display: true, // 차트 제목 표시여부
                    text: '민원 접수' // 제목
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
       
       
        
    });
}

window.updateChartData = (dataset1, dataset2, dataset3, dataset4, dataset5, dataset6, dataset7, dataset8, dataset9) => {
    if (window.chartInstance) {
        console.log("== JS 데이터");
        console.log(dataset1);
        console.log(dataset2);
        console.log(dataset3);
        console.log(dataset4);
        console.log(dataset5);
        console.log(dataset6);
        console.log(dataset7);
        console.log(dataset8);
        console.log(dataset9);
        
        window.chartInstance.data.datasets[0].data = dataset1; // 미분류 데이터 업데이트
        window.chartInstance.data.datasets[1].data = dataset2; //  데이터 업데이트
        window.chartInstance.data.datasets[2].data = dataset3; //  데이터 업데이트
        window.chartInstance.data.datasets[3].data = dataset4; //  데이터 업데이트
        window.chartInstance.data.datasets[4].data = dataset5; //  데이터 업데이트
        window.chartInstance.data.datasets[5].data = dataset6; //  데이터 업데이트
        window.chartInstance.data.datasets[6].data = dataset7; //  데이터 업데이트
        window.chartInstance.data.datasets[7].data = dataset8; //  데이터 업데이트
        window.chartInstance.data.datasets[8].data = dataset9; //  데이터 업데이트

        window.chartInstance.update();
    }
}