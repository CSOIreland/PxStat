
/*******************************************************************************
Application - Label
*******************************************************************************/

/*******************************************************************************
Application - Configuration
*******************************************************************************/

// Init
var app = app || {};

// Set
app.label = {};
app.label.language = {};

// Init 
if (!Cookies.getJSON(C_COOKIE_LANGUAGE)) {
    // Set the default language and settings from configuration
    Cookies.set(C_COOKIE_LANGUAGE, app.config.language, app.config.plugin.jscookie);
    // Store for later use
    app.label.language = app.config.language;
} else {
    // Store for later use
    app.label.language = Cookies.getJSON(C_COOKIE_LANGUAGE);
}

// Load the (master) English language
api.ajax.config("internationalisation/label/en.json", function (label) {
    // Parse JSON string into object
    $.extend(true, app.label, JSON.parse(label));
});

// Merge the chosen language if different from the (master) English
if (app.label.language.iso.code != "en") {
    api.ajax.config("internationalisation/label/" + app.label.language.iso.code + ".json", function (label) {
        // Parse JSON string into object
        $.extend(true, app.label, JSON.parse(label));
    });
}