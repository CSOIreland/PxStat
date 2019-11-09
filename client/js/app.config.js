
/*******************************************************************************
Application - Configuration
*******************************************************************************/

// Init
var app = app || {};

// Set
app.config = {};

// Load the config.json into the application
api.ajax.config("config/config.json", function (config) {
    // Parse JSON string into object
    app.config = JSON.parse(config);
});