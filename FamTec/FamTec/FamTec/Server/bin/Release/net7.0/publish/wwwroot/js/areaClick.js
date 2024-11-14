// wwwroot/js/selectField.js
window.clickHandler = {
    addClickListener: function (dotNetHelper) {
        window.addEventListener('click', function (e) {
            // selectbox Ŭ������ ���� ��Ҹ� ã���ϴ�
            const selectBox = e.target.closest('.selectbox');

            // Ŭ���� ��Ұ� selectbox ���ΰ� �ƴ϶�� �ݽ��ϴ�
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

            // Ŭ���� ��Ұ� Ÿ�Ӷ����̳� �˶� ��ư�� ���� ������� Ȯ��
            if (timelineComponent && !timelineComponent.contains(event.target) &&
                alarmButton && !alarmButton.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('CloseTimeline');
            }
        };

        // �̺�Ʈ ������ ���
        document.addEventListener('click', handleClickOutside);

        // cleanup �Լ��� ��ȯ
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
//        // �̺�Ʈ �ڵ鷯 �Լ��� ������ ����
//        this.clickHandler = function (event) {
//            const viewerElement = document.querySelector('.viewer');
//            if (viewerElement && !viewerElement.contains(event.target)) {
//                dotNetHelper.invokeMethodAsync('HideDropdown');
//            }
//        };

//        // �̺�Ʈ ������ ���
//        document.addEventListener('click', this.clickHandler);
//    },

//    // �̺�Ʈ ������ ���Ÿ� ���� �޼��� �߰�
//    removeClickListener: function () {
//        if (this.clickHandler) {
//            document.removeEventListener('click', this.clickHandler);
//        }
//    }
//};