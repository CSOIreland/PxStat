/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.savedquery = {};
app.savedquery.ajax = {};
app.savedquery.callback = {};


//#endregion

app.savedquery.ajax.getSavedQueries = function () {

    if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Subscription.Query_API.ReadAll",
            {},
            "app.savedquery.callback.getSavedQueries"
        );
    }
    else {
        //must be firebase user, get fresh token
        app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
            api.ajax.jsonrpc.request(
                app.config.url.api.jsonrpc.private,
                "PxStat.Subscription.Query_API.ReadAll",
                {
                    "Uid": app.auth.firebase.user.details.uid,
                    "AccessToken": accessToken
                },
                "app.savedquery.callback.getSavedQueries"
            );
        }).catch(tokenerror => {
            api.modal.error(tokenerror);
        });
    }


}

app.savedquery.callback.getSavedQueries = function (data) {
    if ($.fn.dataTable.isDataTable("#saved-query-container table")) {
        app.library.datatable.reDraw("#saved-query-container table", data);
    } else {
        var localOptions = {
            drawCallback: function (settings) {
                app.savedquery.drawCallback();
            },
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { idn: data.Id };
                        return app.library.html.link.view(attributes, data.Matrix, data.Matrix);
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return data.TagName
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.label.static[data.SnippetType]
                    }
                },
                {
                    data: null,
                    type: "natural",
                    render: function (data, type, row) {
                        return app.library.html.boolean(row.FluidTime, true, true);
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        var attributes = {
                            "idn": data.Id,
                            "tag-name": data.TagName
                        };
                        return app.library.html.deleteButton(attributes, false);
                    },
                    "width": "1%"
                }
            ]
        }

        $("#saved-query-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.savedquery.drawCallback(data);
        });
    }
};

app.savedquery.drawCallback = function (data) {
    $("#saved-query-container table").find("[name=" + C_APP_NAME_LINK_VIEW + "]").once("click", function (e) {
        e.preventDefault();
        app.savedquery.ajax.getQuery($(this).attr("idn"));
    });
    // click event delete
    $("#saved-query-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [$(this).attr("tag-name")]), app.savedquery.ajax.delete, $(this).attr("idn"));
    });

};

app.savedquery.ajax.delete = function (queryId) {
    if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {
        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Subscription.Query_API.Delete",
            {
                "UserQueryId": queryId
            },
            "app.savedquery.callback.deleteQuery",
            null,
            "app.savedquery.callback.deleteQueryError",
            null
        );
    }
    else {
        //must be firebase user, get fresh token
        app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
            api.ajax.jsonrpc.request(
                app.config.url.api.jsonrpc.private,
                "PxStat.Subscription.Query_API.Delete",
                {
                    "Uid": app.auth.firebase.user.details.uid,
                    "AccessToken": accessToken,
                    "UserQueryId": queryId
                },
                "app.savedquery.callback.deleteQuery",
                null,
                "app.savedquery.callback.deleteQueryError",
                null
            );
        }).catch(tokenerror => {
            api.modal.error(tokenerror);
        });
    }
};

app.savedquery.callback.deleteQuery = function (response) {
    if (response == C_API_AJAX_SUCCESS) {
        // Display Success Modal
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [""]));
        app.savedquery.ajax.getSavedQueries();
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
 * Callback after delete
 * @param  {} error
 */
app.savedquery.callback.deleteQueryError = function (error) {
    //Refresh the table
    app.savedquery.ajax.getSavedQueries();
};


app.savedquery.ajax.getQuery = function (queryId) {
    if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {

        api.ajax.jsonrpc.request(
            app.config.url.api.jsonrpc.private,
            "PxStat.Subscription.Query_API.Read",
            {
                "UserQueryId": queryId
            },
            "app.savedquery.callback.savedQueryRead"
        );
    }
    if (app.navigation.user.isSubscriberAccess) {
        //must be firebase user, get fresh token
        app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
            api.ajax.jsonrpc.request(
                app.config.url.api.jsonrpc.private,
                "PxStat.Subscription.Query_API.Read",
                {
                    "Uid": app.auth.firebase.user.details.uid,
                    "AccessToken": accessToken,
                    "UserQueryId": queryId
                },
                "app.savedquery.callback.savedQueryRead"
            );
        }).catch(tokenerror => {
            api.modal.error(app.label.static["firebase-authentication-error"]);
            console.log("firebase authentication error : " + tokenerror);
        });
    }
}

/**
 * Callback for read
 * @param {*} data
 */
app.savedquery.callback.savedQueryRead = function (response) {
    if (response) {
        var isogram = response[0].SnippetIsogram;
        //Load dynamically the ISOGRAM
        if (typeof pxWidget === "undefined" || isogram.search(pxWidget.root) == -1) {
            jQuery.ajax({
                "url": isogram,
                "dataType": "script",
                "async": false,
                "success": function () {
                    app.savedquery.callback.savedQueryDraw(response);
                },
                "error": function (jqXHR, textStatus, errorThrown) {
                    api.modal.exception(app.label.static["api-ajax-exception"]);
                    console.log(isogram + " failed to load")
                }
            });
        }
        else {
            app.savedquery.callback.savedQueryDraw(response);
        }
    }
    // Handle Exception
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.savedquery.callback.savedQueryDraw = function (response) {
    pxWidget.draw.init(
        response[0].SnippetType,
        "saved-query-widget-wrapper",
        JSON.parse(nacl.util.encodeUTF8(nacl.util.decodeBase64(response[0].SnippetQueryBase64)))
    );

    $("#saved-query-widget-modal").find("[name=query-name]").html(response[0].Matrix + ": " + response[0].TagName);
    $("#saved-query-widget-modal").modal("show");

    //draw snippet code
    var snippet = app.config.entity.data.snippet;
    snippet = snippet.sprintf([response[0].SnippetIsogram, response[0].SnippetType, app.library.utility.randomGenerator('pxwidget'), JSON.stringify(JSON.parse(nacl.util.encodeUTF8(nacl.util.decodeBase64(response[0].SnippetQueryBase64))))]);
    $("#saved-query-widget-modal-snippet-code-copy").hide().text(snippet.trim()).fadeIn();
    Prism.highlightAll();


    $("#saved-query-widget-modal-snippet-code-download-button").once("click", function () {
        // Download the snippet file
        app.library.utility.download(response[0].Matrix + "_" + response[0].TagName.replace(/ /g, "_").toLowerCase() + '.' + moment(Date.now()).format(app.config.mask.datetime.file), $("#saved-query-widget-modal-snippet-code-copy").text(), "html", "text/html");
    });
};





