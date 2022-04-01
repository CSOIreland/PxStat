/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.subscriber = {};
app.subscriber.ajax = {};
app.subscriber.callback = {};
//#endregion

app.subscriber.ajax.readSubscriber = function () {
    api.spinner.start();
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Subscription.Subscriber_API.Read",
        {},
        "app.subscriber.callback.readSubscriber"
    );
};

/**
 * Callback for read
 * @param {*} data
 */
app.subscriber.callback.readSubscriber = function (data) {
    app.subscriber.drawDataTable(data);
};
/**
 * Draw table
 * @param {*} data
 */

app.subscriber.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#subscriber-read-container table")) {
        app.library.datatable.reDraw("#subscriber-read-container table", data);
    } else {

        var localOptions = {
            drawCallback: function (settings) {
                app.subscriber.drawCallback();
            },
            data: data,
            createdRow: function (row, data, dataIndex) {
                if (!data.CcnEmail) {
                    $(row).addClass('table-danger');
                }
            },
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.DisplayName || row.CcnEmail;
                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.CcnEmail || app.library.html.parseStaticLabel("invalid-user");
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        var attributes = { idn: row.SbrUserId, email: row.CcnEmail, displayName: row.DisplayName };
                        return app.library.html.deleteButton(attributes, false);
                    },
                    "width": "1%"
                }
            ]
        }

        $("#subscriber-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.subscriber.drawCallback();
        });
    }
    api.spinner.stop();
}

app.subscriber.drawCallback = function () {
    // click event delete
    $("#subscriber-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        var userDetails = {
            "Uid": $(this).attr("idn"),
            "email": $(this).attr("email") || app.library.html.parseStaticLabel("invalid-user"),
            "displayName": $(this).attr("displayName") || app.library.html.parseStaticLabel("invalid-user")
        };
        api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [userDetails.displayName]), app.subscriber.ajax.delete, userDetails);
    });
};

/** 
* Modal to confirm delete
* @param  {} idn
* @param  {} email
*/

app.subscriber.ajax.delete = function (userDetails) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Subscription.Subscriber_API.Delete",
        {
            Uid: userDetails.Uid
        },
        "app.subscriber.callback.deleteSubscriber",
        userDetails.email
    );
};

/** 
* Modal delete success
* @param  {} email
*/

app.subscriber.callback.deleteSubscriber = function (response, email) {
    if (response == C_API_AJAX_SUCCESS) {
        // Display Success Modal
        api.modal.success(app.library.html.parseDynamicLabel("success-subscriber-deleted", [email]));
        app.subscriber.ajax.readSubscriber();
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};