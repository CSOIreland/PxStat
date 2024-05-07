/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Create Namespace
// Create Namespace
app.email = {};
app.email.ajax = {};
app.email.callback = {};
app.email.validation = {};
//#endregion 

//#region Set Up
/**
 * Read data for groups
 */
app.email.ajax.selectGroup = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.ReadAccess",
        { CcnUsername: null },
        "app.email.callback.selectGroup"
    );
};


/**
 * Set email Select Group
 * @param {*} data
 */
app.email.callback.selectGroup = function (data) {
    // Load select2
    $("#email-container").find("[name=group]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        width: '100%',
        placeholder: app.label.static["all-groups"],
        data: app.email.callback.mapData(data)
    });
    // Enable and Focus Search input
    $("#email-container").find("[name=group]").prop('disabled', false).focus();
};

/**
 * Create proper data source
 * @param {*} dataAPI
 */
app.email.callback.mapData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.GrpCode;
        dataAPI[i].text = item.GrpCode + " (" + item.GrpName + ")";
    });
    return dataAPI;
};

/**Reset the screen
 * 
 */
app.email.reset = function () {
    app.email.validation.create();
    app.email.ajax.selectGroup();
};

//#endregion 

//#region validation

/**
*  Validation function for Create User
*/
app.email.validation.create = function () {
    $("#email-container form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        ignore: [],
        rules: {
            "subject":
            {
                required: true,
            },
            "message":
            {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#email-container form [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.email.ajax.create();
        }
    }).resetForm();
};
//#endregion 

//#region Ajax call

/**
 * Create User Ajax call
 *
 */
app.email.ajax.create = function () {
    //check for demo site
    if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
        api.modal.error(app.label.static["demo-site-restricted-access"]);
        return
    }
    // CAll Ajax to Create send message.
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Notification.Email_API.GroupMessageCreate",
        {
            "GroupCodes": $("#email-container").find("[name=group]").val(),
            "Subject": $("#email-container").find("[name=subject]").val(),
            "Body": $("#email-container").find("[name=message]").val()
        },
        "app.email.callback.create",
        null,
        null,
        null,
        { async: false }
    );
};

/**
 * Create User to Table after Ajax success call
 * @param  {} data
 * 
  */
app.email.callback.create = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        //Close modal
        api.modal.success(app.label.static["email-sent"]);
        //Reset screen
        app.email.reset();
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};
//#endregion
