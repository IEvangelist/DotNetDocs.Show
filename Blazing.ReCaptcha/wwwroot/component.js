async function load() {
    const scripts = Array.from(document.getElementsByTagName("script"));
    if (!scripts.some(s => (s.src || "").startsWith("https://www.google.com/recaptcha/api.js"))) {
        const script = document.createElement("script");
        script.src = "https://www.google.com/recaptcha/api.js?render=explicit";
        script.async = true;
        script.defer = true;
        document.head.appendChild(script);
    }

    if (_recaptchaLoaded === null) {
        _recaptchaLoaded = new Promise(waitForRecaptcha);
    }

    return await _recaptchaLoaded;
}

let _recaptchaLoaded = null;

function waitForRecaptcha(resolve) {
    if (typeof (grecaptcha) !== 'undefined' && typeof (grecaptcha.render) !== 'undefined') {
        resolve();
    } else {
        setTimeout(() => waitForRecaptcha(resolve), 100);
    }
}

function render(dotnetObj, id, siteKey) {
    if (grecaptcha) {
        const element = document.querySelector(`#${id}`);
        return grecaptcha.render(element, {
            "sitekey": siteKey,
            "callback": response => dotnetObj.invokeMethodAsync("OnEvaluated", response),
            "expired-callback": () => dotnetObj.invokeMethodAsync("OnExpired")
        });
    }
}

function getResponse(recaptchaId) {
    if (grecaptcha) {
        return grecaptcha.getResponse(recaptchaId);
    }
}

window.recaptcha = {
    load,
    render,
    getResponse
};
