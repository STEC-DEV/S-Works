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


//window.dropdownInit = function (dotnetHelper) {
//    function handleClickOutside(event) {
//        const dropdowns = document.getElementsByClassName('container');
//        for (let dropdown of dropdowns) {
//            if (!dropdown.contains(event.target)) {
//                dotnetHelper.invokeMethodAsync('HandleClickOutside');
//            }
//        }
//    }

//    document.addEventListener('click', handleClickOutside);

//    // Store the event listener for cleanup
//    window.dropdownCleanup = function () {
//        document.removeEventListener('click', handleClickOutside);
//    };
//}

window.dropdownInit = function (dotnetHelper, elementId = null) {
    function handleClickOutside(event) {
        const containers = elementId
            ? [document.getElementById(elementId)]
            : document.getElementsByClassName('container');

        for (let container of containers) {
            if (!container.contains(event.target)) {
                dotnetHelper.invokeMethodAsync('HandleClickOutside');
            }
        }
    }

    document.addEventListener('click', handleClickOutside);

    // Store the event listener for cleanup
    window.dropdownCleanup = function () {
        document.removeEventListener('click', handleClickOutside);
    };
}



//window.addOutsideClickListener = {
//    addClickListener: function (dotNetHelper) {
//        // 이벤트 핸들러 함수를 별도로 저장
//        this.clickHandler = function (event) {
//            const viewerElement = document.querySelector('.viewer');
//            if (viewerElement && !viewerElement.contains(event.target)) {
//                dotNetHelper.invokeMethodAsync('HideDropdown');
//            }
//        };

//        // 이벤트 리스너 등록
//        document.addEventListener('click', this.clickHandler);
//    },

//    // 이벤트 리스너 제거를 위한 메서드 추가
//    removeClickListener: function () {
//        if (this.clickHandler) {
//            document.removeEventListener('click', this.clickHandler);
//        }
//    }
//};