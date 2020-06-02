/*******************************************************************************
Custom JS application specific // 
*******************************************************************************/
//#region Namespaces definitions
app.configuration = {};
app.configuration.ajax = {};
app.configuration.callback = {};
app.configuration.clientConfig = {};
app.configuration.clientConfigNodes = {
    "objects": {},
    "dataTypes": {}
};
app.configuration.globalConfig = {};
app.configuration.globalConfigNodes = {
    "objects": {},
    "dataTypes": {}
};
app.configuration.serverConfig = {};
app.configuration.serverConfigNodes = {
    "objects": {},
    "dataTypes": {}
};
app.configuration.modal = {};

//#endregion

//#region search configs

app.configuration.ajax.getFiles = function () {
    //read client side config
    api.ajax.config("config/config.json", function (clientConfig) {
        app.configuration.clientConfig = clientConfig;
        $("#configuration-client-config").find("[name=config-obj]").text(JSON.stringify(clientConfig, null, "\t"));
        Prism.highlightAll();
    }, { async: true });

    api.ajax.config(app.config.url.configuration.global, function (globalConfig) {
        app.configuration.globalConfig = globalConfig;
        $("#configuration-global-config").find("[name=config-obj]").text(JSON.stringify(globalConfig, null, "\t"));
        Prism.highlightAll();
    }, { async: true });

    api.ajax.config(app.config.url.configuration.server, function (serverConfig) {
        app.configuration.serverConfig = serverConfig;
        $("#configuration-server-config").find("[name=config-obj]").text(JSON.stringify(serverConfig, null, "\t"));
        Prism.highlightAll();
    }, { async: true });

}


app.configuration.modal.search = function () {
    var allNodes = {};
    app.configuration.clientConfigNodes.dataTypes = app.configuration.parseConfig(app.configuration.clientConfig, [], "client")
    app.configuration.globalConfigNodes.dataTypes = app.configuration.parseConfig(app.configuration.globalConfig, [], "global")
    app.configuration.serverConfigNodes.dataTypes = app.configuration.parseConfig(app.configuration.serverConfig, [], "server")

    $.extend(allNodes,
        app.configuration.clientConfigNodes.objects,
        app.configuration.clientConfigNodes.dataTypes,
        app.configuration.globalConfigNodes.objects,
        app.configuration.globalConfigNodes.dataTypes,
        app.configuration.serverConfigNodes.objects,
        app.configuration.serverConfigNodes.dataTypes
    );

    var searchObj = [];
    $.each(allNodes, function (key, value) {
        searchObj.push(
            {
                "id": key,
                "text": "[" + value.source + "] " + key + " {" + value.type + "}",
                "source": value.source,
                "type": value.type,
                "value": value
            }
        );
    });

    $("#configuration-modal-search").find("[name=select-node]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: searchObj.sort((a, b) => (a.text > b.text) ? 1 : -1)
    }).on('select2:select', function (e) {
        $("#configuration-modal-search").find("[name=type]").text(e.params.data.value.type);
        $("#configuration-modal-search").find("[name=source]").text(e.params.data.value.source);
        $("#configuration-modal-search").find("[name=value]").text(JSON.stringify(e.params.data.value.value, null, "\t"));
        Prism.highlightAll();
    }).on('select2:clear', function (e) {
        $("#configuration-modal-search").find("[name=type]").empty();
        $("#configuration-modal-search").find("[name=source]").empty();
        $("#configuration-modal-search").find("[name=value]").empty();
    });




    $("#configuration-modal-search").modal("show");
};

app.configuration.parseConfig = function (obj, keys = [], source) {
    var objPath = null;
    $.each(keys, function (index, value) {
        objPath = objPath ? objPath + "." + value : value;
    });
    if (objPath) {
        switch (source) {
            case "client":
                app.configuration.clientConfigNodes.objects[objPath] = {
                    "type": "object",
                    "value": obj,
                    "source": app.label.static["client"] //source //this value need to come from the dictionary 
                };
                break;
            case "global":
                app.configuration.globalConfigNodes.objects[objPath] = {
                    "type": "object",
                    "value": obj,
                    "source": app.label.static["global"]
                };
                break;
            case "server":
                app.configuration.serverConfigNodes.objects[objPath] = {
                    "type": "object",
                    "value": obj,
                    "source": app.label.static["server"]
                };
                break;

            default:
                break;
        }
    }



    return Object.keys(obj).reduce(function (acc, key) {
        return Object.assign(acc, (obj[key]
            && obj[key].constructor
            && obj[key].constructor.prototype
            && obj[key].constructor.prototype.hasOwnProperty("isPrototypeOf"))
            ? app.configuration.parseConfig(obj[key], keys.concat(key), source)
            : {
                [keys.concat(key).join(".")]: {
                    "type": jQuery.type(obj[key]),
                    "value": obj[key],
                    source: app.label.static[source]
                }
            }
        )
    }, {})
}
//#endregion


//#region load config
app.configuration.ajax.reload = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Security.Configuration_API.Refresh",
        {},
        "app.configuration.callback.reload",
        null,
        null,
        null
    );
};
app.configuration.callback.reload = function (data) {
    api.modal.success(app.label.static["success-configuration-reload"]);
    //Reload application on close Successful Config Reload modal
    $("#modal-success").on('hide.bs.modal', function (e) {
        window.location.href = window.location.pathname;
    });
};

//#endregion