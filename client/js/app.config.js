
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
    //get first 2 digits of status code
    switch (data.status.toString().substr(0, 2)) {
        case 20:
        case 30:
            //do nothing, probably a proxy or redirect
            break;
        case 50:
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

// Using Promise.all to wait for both functions to complete
//fetch backward compatible config files

api.ajax.config('backward.config.client.json', function (backwardClientConfig) {
    $.extend(true, app.config, backwardClientConfig);
});
api.ajax.config('backward.config.global.json', function (backwardGlobalConfig) {
    $.extend(true, app.config, backwardGlobalConfig);
});
// Load the config.client.json into the application
api.ajax.config(C_APP_CONFIG_CLIENT, function (clientConfig) {
    //merge with client config
    $.extend(true, app.config, clientConfig);
});

// Load the config.global.json into the application
api.ajax.config(C_APP_CONFIG_GLOBAL, function (globalConfig) {
    //merge with global config
    $.extend(true, app.config, globalConfig);
});

// Load the Widget Snippet into the application
api.ajax.config(C_APP_URL_PXWIDGET_SNIPPET, function (snippet) {
    //merge with snippet config
    app.config.entity.data.snippet = snippet;
}, { dataType: "html" });



