
/*******************************************************************************
Application - Configuration
*******************************************************************************/

// Init
var app = app || {};

// Set
app.config = {};

// Load the config.client.json into the application
api.ajax.config("config/config.json", function (clientConfig) {
    app.config = clientConfig;
});

// Load the config.global.json into the application
api.ajax.config(app.config.url.configuration.global, function (globalConfig) {
    //merge with client config
    $.extend(true, app.config, globalConfig);
});

// Load the Widget Snippet into the application
api.ajax.config(C_APP_URL_PXWIDGET_SNIPPET, function (snippet) {
    app.config.entity.data.snippet = snippet;
}, { dataType: "html" });