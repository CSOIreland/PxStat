
/*******************************************************************************
Application - Label
*******************************************************************************/

// Init
var app = app || {};

// Set
app.label = {};
app.label.language = {};

// Init 
app.label.init = function () {
    // Init target language with default one
    var targetLanguage = $.extend(true, {}, app.config.language);
    // Get existing Cookie Language
    if (Cookies.getJSON(C_COOKIE_LANGUAGE)) {
        targetLanguage = Cookies.getJSON(C_COOKIE_LANGUAGE);
    }
    // Reset Cookie language in case the target language fails
    Cookies.set(C_COOKIE_LANGUAGE, app.config.language, app.config.plugin.jscookie.persistent);
    // Store the reset language for later use
    app.label.language = $.extend(true, {}, app.config.language);

    // Load the (master) English language
    api.ajax.config(C_APP_URL_PXLANGUAGEPLUGINS + "en.json", function (label) {
        $.extend(true, app.label, label);
    });

    // Attempt to merge the target language if different from the master
    if (targetLanguage.iso.code != C_APP_MASTER_LANGUAGE) {
        //fetch client language
        api.ajax.config(C_APP_URL_PXLANGUAGEPLUGINS + targetLanguage.iso.code + ".json", function (label) {
            //No need to check for server language pack. When we draw the language dropdown, we already handle this
            // Extend lable sourced form target language
            $.extend(true, app.label, label);
            // Set the target language in the cookie
            Cookies.set(C_COOKIE_LANGUAGE, targetLanguage, app.config.plugin.jscookie.persistent);
            // Store for later use
            app.label.language = targetLanguage;
        });
    };
}, app.label.init();