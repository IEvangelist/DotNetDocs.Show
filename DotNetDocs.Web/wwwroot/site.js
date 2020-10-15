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
    },
    nudgeTwitter: () => {
        if (twttr && twttr.widgets) {
            twttr.widgets.load();
        }
    },
    loadTwitterImages: () => {
        if ('loading' in HTMLImageElement.prototype) {
            document.querySelectorAll('img[loading="lazy"]')
                .forEach(img => img.src = img.dataset.src);
        } else {
            const lazyScript = document.createElement('script');
            lazyScript.src = 'https://cdnjs.cloudflare.com/ajax/libs/lazysizes/4.1.8/lazysizes.min.js';
            document.body.appendChild(lazyScript);
        }
    }
};
