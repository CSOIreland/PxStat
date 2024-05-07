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

app.configuration.apiConfig = null;

//#endregion

//#region get app version
app.configuration.ajax.getAppVersion = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Config.Config_API.ReadVersions",
        {
            "type": "app"
        },
        "app.configuration.callback.getAppVersion",
        null,
        null,
        null,
        { async: false }
    );

};

/**
 * Read client config callback
 * @param {*} data 
 */
app.configuration.callback.getAppVersion = function (data) {
    $("#configuration-read-app-content").find('[name="current-version"]').text(data)
};
//#endregion

//#region get api version
app.configuration.ajax.getApiVersion = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Config.Config_API.ReadVersions",
        {
            "type": "api"
        },
        "app.configuration.callback.getApiVersion",
        null,
        null,
        null,
        { async: false }
    );

};

/**
 * Read api version callback
 * @param {*} data 
 */
app.configuration.callback.getApiVersion = function (data) {
    $("#configuration-read-api-content").find('[name="current-version"]').text(data)
};
//#endregion


//#region read configs
/**
 * Read client config
 */
app.configuration.ajax.getClientConfig = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Config.Config_API.ReadApp",
        {
            "name": "config.client.json"
        },
        "app.configuration.callback.getClientConfig",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Read client config callback
 * @param {*} data 
 */
app.configuration.callback.getClientConfig = function (data) {
    app.configuration.clientConfig = data;
    $("#configuration-client-config").find("[name=config-obj]").empty().text(JSON.stringify(data, null, "\t"));
    Prism.highlightAll();
    new ClipboardJS("#configuration-client-config [name=copy-snippet-code]");
};

/**
 * Read global config 
 * */
app.configuration.ajax.getGlobalConfig = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Config.Config_API.ReadApp",
        {
            "name": "config.global.json"
        },
        "app.configuration.callback.getGlobalConfig",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Read global config callback
 * 
 * @param {*} data 
 */
app.configuration.callback.getGlobalConfig = function (data) {
    app.configuration.globalConfig = data;
    $("#configuration-global-config").find("[name=config-obj]").empty().text(JSON.stringify(data, null, "\t"));
    Prism.highlightAll();
    new ClipboardJS("#configuration-global-config [name=copy-snippet-code]");
};

/**
 * Read server config
 */
app.configuration.ajax.getServerConfig = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Config.Config_API.ReadApp",
        {
            "name": "config.server.json"
        },
        "app.configuration.callback.getServerConfig",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Read server config callback
 * @param {*} data 
 */
app.configuration.callback.getServerConfig = function (data) {
    app.configuration.serverConfig = data;
    $("#configuration-server-config").find("[name=config-obj]").empty().text(JSON.stringify(data, null, "\t"));
    Prism.highlightAll();
    new ClipboardJS("#configuration-server-config [name=copy-snippet-code]");
};

/**
 * Get schema for schema modal
 * @param {*} configType 
 */
app.configuration.ajax.getSchemaRead = function (configType) {
    var apiParams = {
        "name": null
    };
    switch (configType) {
        case "client":
            apiParams.name = "config.client.json";
            break;
        case "global":
            apiParams.name = "config.global.json";
            break;
        case "server":
            apiParams.name = "config.server.json";
            break;
        default:
            break;
    }

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Config.Config_API.ReadSchema",
        apiParams,
        "app.configuration.callback.getSchemaRead",
        configType
    );
};

/**
 * Get schema for schema modal callback
 * @param {*} data 
 * @param {*} configType 
 */
app.configuration.callback.getSchemaRead = function (data, configType) {
    //initiate all copy to clipboard 
    $("#configuration-modal-schema-copy").empty().text(JSON.stringify(data, null, "\t"));
    Prism.highlightAll();
    new ClipboardJS("#configuration-modal-schema [name=copy-snippet-code]");
    $("#configuration-modal-schema").modal("show");
};

/**
 * Get api config
 */
app.configuration.ajax.getApiConfig = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Config.Config_API.ReadApi",
        {
            "type": "API"
        },
        "app.configuration.callback.getApiConfig",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Get api config callback
 * @param {*} data 
 */
app.configuration.callback.getApiConfig = function (data) {
    app.configuration.apiConfig = data;

    // Load select2
    $("#configuration-read-api-content").find("[name=api-key]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.configuration.callback.mapApiConfigData(data)
    }).on('select2:select', function (e) {
        var selectedKey = $.grep(app.configuration.apiConfig, function (n, i) { // just use arr
            return n.API_KEY == e.params.data.API_KEY;
        });

        if (selectedKey[0].API_SENSITIVE_VALUE) {
            $("#configuration-read-api-content").find("[name=api-value]").addClass("is-invalid");
            $("#configuration-read-api-content").find("[name=sensitive-message]").show();
            $("#configuration-read-api-content").find("[name=api-value]").attr("type", "password");
        }
        else {
            $("#configuration-read-api-content").find("[name=api-value]").removeClass("is-invalid");
            $("#configuration-read-api-content").find("[name=sensitive-message]").hide();
            $("#configuration-read-api-content").find("[name=api-value]").attr("type", "text");
        }

        $("#configuration-read-api-content").find("[name=api-value]").val(selectedKey[0].API_VALUE).prop('disabled', false);
        $("#configuration-read-api-content").find("[name=update]").prop('disabled', false);
    }).on('select2:clear', function () {
        $("#configuration-read-api-content").find("[name=api-value]").val("").prop('disabled', true);
        $("#configuration-read-api-content").find("[name=update]").prop('disabled', true);
    });

};

/**
 * Map api config for select2
 * @param {*} data 
 * @returns 
 */
app.configuration.callback.mapApiConfigData = function (data) {
    $.each(data, function (i, item) {
        // Add ID and TEXT to the list
        data[i].id = item.API_KEY;
        data[i].text = item.API_KEY;
    });
    return data;
};

//#endregion

//#region search configs
/**
 * set up search modal
 */
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
        dropdownParent: $('#configuration-modal-search'),
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

/**
 * parse all config objects for searching
 * @param {*} obj 
 * @param {*} keys 
 * @param {*} source 
 * @returns 
 */
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

//#region update configs
/**
 * Check that JSON to be updated is valid JSON
 * @param {} configType 
 */
app.configuration.validation = function (configType) {

    //check for valid json
    var updatedConfig = null;

    switch (configType) {
        case "client":
            updatedConfig = $("#configuration-client-config").find("[name=config-obj-edit]").val().trim();
            break;
        case "global":
            updatedConfig = $("#configuration-global-config").find("[name=config-obj-edit]").val().trim();
            break;
        case "server":
            updatedConfig = $("#configuration-server-config").find("[name=config-obj-edit]").val().trim();
            break;
        default:
            break;
    }

    try {
        var updatedConfigParsed = JSON.parse(updatedConfig);
        app.configuration.ajax.getSchemaUpdate(configType, updatedConfigParsed);

    } catch (err) {
        api.modal.error(app.label.static["invalid-json-object"])
    }
};

/**
 * Get schema for validation
 * @param {*} configType 
 * @param {*} updatedConfig 
 */
app.configuration.ajax.getSchemaUpdate = function (configType, updatedConfig) {
    var callbackParams = {
        "type": configType,
        "configObj": updatedConfig
    };

    var apiParams = {
        "name": null
    };
    switch (configType) {
        case "client":
            apiParams.name = "config.client.json";
            break;
        case "global":
            apiParams.name = "config.global.json";
            break;
        case "server":
            apiParams.name = "config.server.json";
            break;
        default:
            break;
    }

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Config.Config_API.ReadSchema",
        apiParams,
        "app.configuration.callback.getSchemaUpdate",
        callbackParams
    );
};

/**
 * Get schema for validation callback
 * @param {*} data 
 * @param {*} callbackParams 
 */
app.configuration.callback.getSchemaUpdate = function (data, callbackParams) {
    var validate = jsen(data, { greedy: true });
    var isValid = validate(callbackParams.configObj);
    if (isValid) {
        api.modal.confirm(
            app.label.static["update-application-config-confirm"],
            app.configuration.ajax.updateAppConfig,
            callbackParams
        );
    }
    // Invalid, display errors
    else {
        var errorWrapper = $("<span>");
        errorWrapper.append($("<p>", {
            "html": "The configuration failed validation against the schema:"
        }));
        var errorList = $("<ul>", {
            class: "list-group"
        });
        $.each(validate.errors, function (_index, value) {
            var error = $("<li>", {
                class: "list-group-item",
                html: JSON.stringify(value)
            });
            errorList.append(error);
        });
        errorWrapper.append(errorList);
        api.modal.error(errorWrapper);
    };
};

/**
 * Update app config
 * @param {*} callbackParams 
 */
app.configuration.ajax.updateAppConfig = function (callbackParams) {
    var apiParams = {
        "name": null,
        "value": callbackParams.configObj
    };
    switch (callbackParams.type) {
        case "client":
            apiParams.name = "config.client.json";
            break;
        case "global":
            apiParams.name = "config.global.json";
            break;
        case "server":
            apiParams.name = "config.server.json";
            break;
        default:
            break;
    }

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Config.Config_API.Create",
        apiParams,
        "app.configuration.callback.updateAppConfig"
    );
};

/**
 * Update app config callback
 * @param {*} data 
 */
app.configuration.callback.updateAppConfig = function (data) {
    if (data == C_API_AJAX_SUCCESS) {

        window.location.href = app.config.url.application + "?body=entity/system/configuration"
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }

};

/**
 * Update api config
 */
app.configuration.ajax.updateApiConfig = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Config.Config_API.Create",
        {
            "type": "API",
            "name": $("#configuration-read-api-content").find("[name=api-key]").find("option:selected").val(),
            "value": $("#configuration-read-api-content").find("[name=api-value]").val().trim()
        },
        "app.configuration.callback.updateApiConfig"
    );
};

/**
 * Update api config callback
 * @param {*} data 
 */
app.configuration.callback.updateApiConfig = function (data) {
    if (data == C_API_AJAX_SUCCESS) {

        window.location.href = app.config.url.application + "?body=entity/system/configuration"
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion