//window.initializeClickOutside = (dotnetHelper, element) => {
//    if (!window.clickOutsideHandler) {  // �ߺ� ��� ����
//        window.clickOutsideHandler = (event) => {
//            if (element && !element.contains(event.target)) {
//                dotnetHelper.invokeMethodAsync('HandleClickOutside');
//            }
//        };

//        document.addEventListener('mousedown', window.clickOutsideHandler);
//    }
//};

//window.removeClickOutside = () => {
//    if (window.clickOutsideHandler) {
//        document.removeEventListener('mousedown', window.clickOutsideHandler);
//        window.clickOutsideHandler = null;
//    }
//};


window.multiSelectHandlers = window.multiSelectHandlers || {};

window.initializeClickOutside = (dotnetHelper, element) => {
    // �� ������Ʈ�� ���� ������ ID ����
    const componentId = Math.random().toString(36).substring(2, 15);
    
    // �� ������Ʈ�� ���� �ڵ鷯 ����
    const handler = (event) => {
        if (element && !element.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('HandleClickOutside');
        }
    };
    
    // �ڵ鷯 ����
    window.multiSelectHandlers[componentId] = {
        handler: handler,
        dotnetHelper: dotnetHelper
    };
    
    // �̺�Ʈ ������ ���
    document.addEventListener('mousedown', handler);
    
    // componentId ��ȯ�Ͽ� Blazor ������Ʈ���� ����
    return componentId;
};

window.removeClickOutside = (componentId) => {
    if (window.multiSelectHandlers[componentId]) {
        const { handler } = window.multiSelectHandlers[componentId];
        document.removeEventListener('mousedown', handler);
        delete window.multiSelectHandlers[componentId];
    }
};
