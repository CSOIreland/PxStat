
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



