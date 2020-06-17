window.utilities = {
    scrollIntoView: (selector) => {
        try {
            if (selector) {
                const element = document.getElementById(selector);
                if (element) {
                    element.scrollIntoView(true);
                }
            }
        } catch (error) {
            console.error(error);
        }
    }
};
