/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.comment = {};
app.release.comment.validation = {};
app.release.comment.ajax = {};
app.release.comment.callback = {};
//#endregion

//#region Read

/**
* 
* @param {*} data
*/
app.release.comment.render = function (data) {
    app.release.comment.validation.update();
    if (data.CmmValue) {
        $("#release-comment [name=create]").hide();
        $("#release-comment [name=update]").show();
        $("#release-comment [name=delete]").prop("disabled", false);
        $("#release-comment [name=cmm-value]").empty().html(app.library.html.parseBbCode(data.CmmValue)).show();

        var tinyMceId = $("#release-comment-modal-update [name=cmm-value]").attr("id");
        tinymce.get(tinyMceId).setContent(data.CmmValue);

        $("#release-comment-modal-update [name=cmm-value-update]").val(data.CmmValue);

    } else {
        $("#release-comment [name=create]").show();
        $("#release-comment [name=update]").hide();
        $("#release-comment [name=delete]").prop("disabled", true);
        $("#release-comment [name=cmm-value]").empty().hide();

        var tinyMceId = $("#release-comment-modal-create [name=cmm-value]").attr("id");
        tinymce.get(tinyMceId).setContent("");
    }

    if (app.release.isModerator || app.release.isHistorical) {
        $("#release-comment [name=create]").prop("disabled", true);
        $("#release-comment [name=update]").prop("disabled", true);
        $("#release-comment [name=delete]").prop("disabled", true);
    }

    $("#release-comment").hide().fadeIn();
};
//#endregion

//#region Create
/**
* 
*/
app.release.comment.create = function () {
    $("#release-comment-modal-create").modal("show");
    app.release.comment.validation.create();
};

/**
*
*/
app.release.comment.validation.create = function () {
    $("#release-comment-modal-create form").trigger("reset").validate({
        ignore: [],
        rules: {
            "cmm-value": {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#release-comment-modal-create [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.release.comment.ajax.create();
            $("#release-comment-modal-create").modal("hide");
        }
    }).resetForm();
};

/**
* 
*/
app.release.comment.ajax.create = function () {
    var tinyMceId = $("#release-comment-modal-create").find("[name=cmm-value]").attr("id");
    var CmmValue = tinymce.get(tinyMceId).getContent();

    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Release_API.UpdateComment",
        { "RlsCode": app.release.RlsCode, "CmmValue": CmmValue },
        "app.release.comment.callback.create",
        null,
        null,
        null,
        { async: false });
};

/**
* 
* @param {*} response
*/
app.release.comment.callback.create = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data == C_APP_API_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [""]));
        app.release.read();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion

//#region Update

/**
* 
*/
app.release.comment.update = function () {
    var cmmValue = $("#release-comment-modal-update [name=cmm-value-update]").val();
    var tinyMceId = $("#release-comment-modal-update [name=cmm-value]").attr("id");
    tinymce.get(tinyMceId).setContent(cmmValue);
    // Empty error. (Do not delete.)
    $("#release-comment-modal-update [name=cmm-value-error-holder]").empty();
    $("#release-comment-modal-update").modal("show");
};

/**
* 
*/
app.release.comment.validation.update = function () {
    $("#release-comment-modal-update form").trigger("reset").validate({
        ignore: [],
        rules: {
            "cmm-value": {
                required: function (element) {
                    tinymce.triggerSave();
                    return true;
                }
            }
        },
        errorPlacement: function (error, element) {
            $("#release-comment-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.release.comment.ajax.update();
            $("#release-comment-modal-update").modal("hide");
        }
    }).resetForm();
};

/**
* 
*/
app.release.comment.ajax.update = function () {
    var tinyMceId = $("#release-comment-modal-update [name=cmm-value]").attr("id");
    var CmmValue = tinymce.get(tinyMceId).getContent();
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Release_API.UpdateComment",
        { "RlsCode": app.release.RlsCode, "CmmValue": CmmValue },
        "app.release.comment.callback.update",
        null,
        null,
        null,
        { async: false });
};

/**
* 
* @param {*} response
*/
app.release.comment.callback.update = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data == C_APP_API_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
        app.release.read();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion

//#region Delete

/**
* 
*/
app.release.comment.delete = function () {
    api.modal.confirm(app.label.static["confirm-delete"], app.release.comment.ajax.delete);
};

/**
* 
*/
app.release.comment.ajax.delete = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Data.Release_API.DeleteComment",
        { "RlsCode": app.release.RlsCode },
        "app.release.comment.callback.delete",
        null,
        null,
        null,
        { async: false });
};

/**
* 
* @param {*} response
*/
app.release.comment.callback.delete = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data == C_APP_API_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [""]));
        app.release.read();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion