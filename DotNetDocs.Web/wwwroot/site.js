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
        }
    }
};

const fallBackImg = img => {
    let index = +img.dataset.index;
    img.src = img.dataset[`src${index}`];
    index++;
    img.dataset.index = index;
    if (index === 4) {
        img.onerror = null;
    }
};
