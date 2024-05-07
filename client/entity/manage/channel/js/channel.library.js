/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.channel = {};
app.channel.readChannel = {};
app.channel.validation = {};
app.channel.ajax = {};
app.channel.callback = {};

//#endregion

app.channel.ajax.readChannel = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Subscription.Channel_API.Read",
        {},
        "app.channel.callback.readChannel"
    );
};
app.channel.callback.readChannel = function (data) {

    $.each(data, function (key, value) {

        var input = $("<input>", {
            type: "radio",
            name: "channel-type",
            value: value.ChnCode
        }).get(0).outerHTML;

        var label = $("<label>", {
            text: value.ChnName
        }).get(0).outerHTML;

        $('#channel-container').find("[name=channel-radio-wrapper]").append(input + " " + label + "<br>");
    });
    //define validation after tinyMCE initiated
    app.channel.validation.create();
};
/**
*  Get Languages from api to populate Language
*/
app.channel.ajax.readLanguage = function () {
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
        "PxStat.System.Settings.Language_API.Read",
        { LngIsoCode: null },
        "app.channel.callback.readLanguage");
};

/**
 * Callback from server for read languages
 *
 * @param {*} data
 */
app.channel.callback.readLanguage = function (data) {
    app.channel.callback.drawLanguage(data);
};

/**
 * Draw screen for languages
 *
 * @param {*} data
 */
app.channel.callback.drawLanguage = function (data) {
    $("#channel-container").find("[name=language]").append($("<option>", {
        "value": "all-languages",
        "text": "All Languages"
    }));
    $.each(data, function (key, value) {
        $("#channel-container").find("[name=language]").append($("<option>", {
            "value": value.LngIsoCode,
            "text": value.LngIsoName
        }));
    });

};




/**
*  Validation function for Create User
*/
app.channel.validation.create = function () {
    $("#channel-container form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        ignore: [],
        rules: {
            "subject":
            {
                required: true,
            },
            "channel-type":
            {
                required: true
            },
            "body":
            {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#channel-container form [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            var formInput = $(":input").serializeArray();
            app.channel.ajax.sendNotification(formInput);
            $(form).sanitiseForm();
        }
    }).resetForm();
};
//#endregion



app.channel.ajax.sendNotification = function () {
    //check for demo site
    if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
        api.modal.error(app.label.static["demo-site-restricted-access"]);
        return
    }
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Subscription.NotificationChannel_API.Send",
        {
            "LngIsoCode": $("#channel-container").find("[name=language]").val() == "all-languages" ? null : $("#channel-container").find("[name=language]").val(),
            "ChnCode": $("#channel-container").find("[name=channel-type]:checked").val(),
            "EmailSubject": $("#channel-container").find("[name=subject]").val(),
            "EmailBody": $("#channel-container").find("[name=body]").val(),
        },
        "app.channel.callback.sendNotification"
    );
};


app.channel.callback.sendNotification = function (response) {
    //response will contain a list of unsuccessfull emails sent
    if (!response.length) {
        // Display Success Modal
        api.modal.success(app.library.html.parseStaticLabel("success-notification-sent"));
    }
    else {
        //show errors to user
        var errorMessage = $("<p>", {
            html: app.label.static["notification-send-error"]
        });
        var errors = $("<ul>", {
            class: "list-group"
        });
        $.each(response, function (index, value) {
            var error = $("<li>", {
                class: "list-group-item",
                html: value
            });
            errors.append(error);
        });
        api.modal.information(errorMessage.get(0).outerHTML + errors.get(0).outerHTML);
    };
};



//#endregion
