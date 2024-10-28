//window.initializeClickOutside = (dotnetHelper, element) => {
//    if (!window.clickOutsideHandler) {  // 중복 등록 방지
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
    // 각 컴포넌트에 대해 고유한 ID 생성
    const componentId = Math.random().toString(36).substring(2, 15);
    
    // 이 컴포넌트에 대한 핸들러 생성
    const handler = (event) => {
        if (element && !element.contains(event.target)) {
            dotnetHelper.invokeMethodAsync('HandleClickOutside');
        }
    };
    
    // 핸들러 저장
    window.multiSelectHandlers[componentId] = {
        handler: handler,
        dotnetHelper: dotnetHelper
    };
    
    // 이벤트 리스너 등록
    document.addEventListener('mousedown', handler);
    
    // componentId 반환하여 Blazor 컴포넌트에서 보관
    return componentId;
};

window.removeClickOutside = (componentId) => {
    if (window.multiSelectHandlers[componentId]) {
        const { handler } = window.multiSelectHandlers[componentId];
        document.removeEventListener('mousedown', handler);
        delete window.multiSelectHandlers[componentId];
    }
};
