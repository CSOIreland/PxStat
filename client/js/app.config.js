
/*******************************************************************************
Application - Configuration
*******************************************************************************/

// Init
var app = app || {};

// Set
app.config = {};


//probe api to check system status
$.get(C_APP_URL_API + "public/api.restful/PxStat.Config.Config_API.Ping", function (response) {
    if (response != C_API_AJAX_SUCCESS) {
        //general exception
        window.location.href = "./50x.html"
    }
}).fail(function (data, status, xhr) {
    switch (data.status) {
        case 503:
            //system unavailable
            window.location.href = "./50x.html"
            break;
        default:
            //general exception
            var uri = new URI(window.location.href);
            window.location.href = uri.origin() + uri.pathname() + "50x.html?exception=";
            break;
    }
});

// Load the config.client.json into the application
api.ajax.config(C_APP_CONFIG_CLIENT, function (clientConfig) {
    app.config = clientConfig;
});

// Load the config.global.json into the application
api.ajax.config(C_APP_CONFIG_GLOBAL, function (globalConfig) {
    //merge with client config
    $.extend(true, app.config, globalConfig);
});

// Load the Widget Snippet into the application
api.ajax.config(C_APP_URL_PXWIDGET_SNIPPET, function (snippet) {
    app.config.entity.data.snippet = snippet;
}, { dataType: "html" });