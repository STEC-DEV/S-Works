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