window.addOutsideClickListener = {
    addClickListener: function (dotNetHelper) {
        this.clickHandler = function (event) {
            const viewerElement = document.querySelector('.viewer');
            if (viewerElement && !viewerElement.contains(event.target)) {
                dotNetHelper.invokeMethodAsync('HideDropdown');
            }
        };

        document.addEventListener('click', this.clickHandler);
    },

    removeClickListener: function () {
        if (this.clickHandler) {
            document.removeEventListener('click', this.clickHandler);
        }
    }
};