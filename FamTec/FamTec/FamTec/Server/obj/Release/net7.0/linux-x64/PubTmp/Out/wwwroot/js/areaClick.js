// wwwroot/js/selectField.js
window.clickHandler = {
    addClickListener: function (dotNetHelper) {
        window.addEventListener('click', function (e) {
            // selectbox 클래스를 가진 요소를 찾습니다
            const selectBox = e.target.closest('.selectbox');

            // 클릭된 요소가 selectbox 내부가 아니라면 닫습니다
            if (!selectBox) {
                dotNetHelper.invokeMethodAsync('CloseSelectBox');
            }
        });
    },

    removeClickListener: function (dotNetHelper) {
        window.removeEventListener('click', function (e) {
            const selectBox = e.target.closest('.selectbox');
            if (!selectBox) {
                dotNetHelper.invokeMethodAsync('CloseSelectBox');
            }
        });
    }
};






// wwwroot/js/timelineHandler.js
window.timelineHandler = {
    initialize: function (dotNetHelper) {
        const handleClickOutside = (event) => {
            const timelineComponent = document.querySelector('.timeline');
            const alarmButton = document.querySelector('.nav-tab-item.alarm');

            // 클릭된 요소가 타임라인이나 알람 버튼의 하위 요소인지 확인
            if (timelineComponent && !timelineComponent.contains(event.target) &&
                alarmButton && !alarmButton.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('CloseTimeline');
            }
        };

        // 이벤트 리스너 등록
        document.addEventListener('click', handleClickOutside);

        // cleanup 함수를 반환
        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }
};




window.dropdownInit = function (dotnetHelper, elementId = null) {
    function handleClickOutside(event) {
        try {
            let containers = [];

            if (elementId) {
                // ID가 지정된 경우
                const container = document.getElementById(elementId);
                if (container) {
                    containers.push(container);
                }
            } else {
                // ID가 지정되지 않은 경우
                containers = Array.from(document.getElementsByClassName('container'));
            }

            // 클릭된 요소가 컨테이너 외부인 경우에만 처리
            const isClickedOutside = containers.every(container =>
                container && !container.contains(event.target)
            );

            if (isClickedOutside && containers.length > 0) {
                dotnetHelper.invokeMethodAsync('HandleClickOutside');
            }
        } catch (error) {
            console.error('Error in handleClickOutside:', error);
        }
    }

    document.addEventListener('click', handleClickOutside);

    window.dropdownCleanup = function () {
        document.removeEventListener('click', handleClickOutside);
    };
}
