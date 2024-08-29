window.myChart = null; // 차트를 전역 변수로 선언

window.initializeStackedChart = (labels, dataset1, dataset2) => {
    var ctx = document.getElementById('myStackedChart').getContext('2d');

    // 기존 차트가 없으면 새로 생성
    if (!window.myChart) {
        window.myChart = new Chart(ctx, {
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
                        label: '미화',
                        data: dataset2,
                        backgroundColor: 'rgba(54, 162, 235, 0.2)',
                        borderColor: 'rgba(54, 162, 235, 1)',
                        borderWidth: 1
                    }
                ]
            },
            options: {
                scales: {
                    x: {
                        stacked: true,
                    },
                    y: {
                        stacked: true
                    }
                }
            }
        });
    } else {
        // 기존 차트가 있으면 데이터만 업데이트
        window.myChart.data.datasets[0].data = dataset1;
        window.myChart.data.datasets[1].data = dataset2;
        window.myChart.update(); // 차트 업데이트
    }
};