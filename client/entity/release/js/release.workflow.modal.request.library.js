/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.release = app.release || {};

app.release.workflow.modal = app.release.workflow.modal || {};

app.release.workflow.modal.request = {};
app.release.workflow.modal.request.modal = {};
app.release.workflow.modal.request.validation = {};
app.release.workflow.modal.request.ajax = {};
app.release.workflow.modal.request.callback = {};
//#endregion

//#region Request

/**
* 
 * @param {*} data
 */
app.release.workflow.modal.request.setFlag = function () {
    // Modal Request Publish
    $("#request-workflow-modal-request-publish .bootstrap-toggle").bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "success",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });
    $("#request-workflow-modal-request-publish [name=wrq-emergency-flag]").bootstrapToggle("off");
    $("#request-workflow-modal-request-publish [name=wrq-reservation-flag]").bootstrapToggle(app.release.RlsReservationFlag ? "on" : "off");
    $("#request-workflow-modal-request-publish [name=wrq-archive-flag]").bootstrapToggle(app.release.RlsArchiveFlag ? "on" : "off");

    // Modal Request Flag
    $("#request-workflow-modal-request-flag .bootstrap-toggle").bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["true"],
        off: app.label.static["false"],
        onstyle: "success",
        offstyle: "warning",
        width: C_APP_TOGGLE_LENGTH
    });
    $("#request-workflow-modal-request-flag [name=wrq-reservation-flag]").bootstrapToggle(app.release.RlsReservationFlag ? "on" : "off");
    $("#request-workflow-modal-request-flag [name=wrq-archive-flag]").bootstrapToggle(app.release.RlsArchiveFlag ? "on" : "off");
};

/**
 * 
 */
app.release.workflow.modal.request.create = function () {
    var RqsCode = $("#release-workflow-request [name=rqs-code] option:selected").val();
    var RqsValue = $("#release-workflow-request [name=rqs-code] option:selected").text();
    switch (RqsCode) {

        case C_APP_TS_REQUEST_PUBLISH:
            app.release.workflow.modal.request.setFlag();

            $("#request-workflow-modal-request-publish [name=rqs-value]").html(RqsValue);
            $("#request-workflow-modal-request-publish").modal("show").on('shown.bs.modal', function (e) {
                app.release.workflow.modal.request.checkDatetime();

            });
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            app.release.workflow.modal.request.setFlag();

            $("#request-workflow-modal-request-flag [name=rqs-value]").html(RqsValue);
            $("#request-workflow-modal-request-flag").modal("show");
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-request-delete [name=rqs-value]").html(RqsValue);
            $("#request-workflow-modal-request-delete").modal("show");
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-request-rollback [name=rqs-value]").html(RqsValue);
            $("#request-workflow-modal-request-rollback").modal("show");
            break;
    }

    app.release.workflow.modal.request.validation.create(RqsCode);
};

/**
* 
 * @param {*} RqsCode
 */
app.release.workflow.modal.request.validation.create = function (RqsCode) {
    switch (RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            $("#request-workflow-modal-request-publish form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-request-publish [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.request.ajax.create(RqsCode);
                    $("#request-workflow-modal-request-publish").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            $("#request-workflow-modal-request-flag form").trigger("reset").validate({
                ignore: [],
                rules: {

                },
                errorPlacement: function (error, element) {
                    $("#request-workflow-modal-request-flag [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.request.ajax.create(RqsCode);
                    $("#request-workflow-modal-request-flag").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-request-delete form").trigger("reset").validate({
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
                    $("#request-workflow-modal-request-delete [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.request.ajax.create(RqsCode);
                    $("#request-workflow-modal-request-delete").modal("hide");
                }
            }).resetForm();
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-request-rollback form").trigger("reset").validate({
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
                    $("#request-workflow-modal-request-rollback [name=" + element[0].name + "-error-holder]").append(error[0]);
                },
                submitHandler: function (form) {
                    $(form).sanitiseForm();
                    app.release.workflow.modal.request.ajax.create(RqsCode);
                    $("#request-workflow-modal-request-rollback").modal("hide");
                }
            }).resetForm();
            break;
    }
};

/**
* 
 * @param {*} RqsCode
 */
app.release.workflow.modal.request.ajax.create = function (RqsCode) {
    switch (RqsCode) {
        case C_APP_TS_REQUEST_PUBLISH:
            var tinyMceId = $("#request-workflow-modal-request-publish [name=cmm-value]").attr("id");
            var CmmValue = tinymce.get(tinyMceId).getContent();

            obj2send = {
                "RlsCode": app.release.RlsCode,
                "RqsCode": RqsCode,
                "CmmValue": CmmValue,
                "WrqReservationFlag": $("#request-workflow-modal-request-publish [name=wrq-reservation-flag]").prop("checked"),
                "WrqArchiveFlag": $("#request-workflow-modal-request-publish [name=wrq-archive-flag]").prop("checked"),
                "WrqEmergencyFlag": $("#request-workflow-modal-request-publish [name=wrq-emergency-flag]").prop("checked"),
                "WrqDatetime": moment($("#request-workflow-modal-request-publish [name=wrq-datetime]").val(), app.config.mask.datetime.display).format(app.config.mask.datetime.ajax)
            };
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            var tinyMceId = $("#request-workflow-modal-request-flag [name=cmm-value]").attr("id");
            var CmmValue = tinymce.get(tinyMceId).getContent();

            obj2send = {
                "RlsCode": app.release.RlsCode,
                "RqsCode": RqsCode,
                "CmmValue": CmmValue,
                "WrqReservationFlag": $("#request-workflow-modal-request-flag [name=wrq-reservation-flag]").prop("checked"),
                "WrqArchiveFlag": $("#request-workflow-modal-request-flag [name=wrq-archive-flag]").prop("checked")
            };
            break;
        case C_APP_TS_REQUEST_DELETE:
            var tinyMceId = $("#request-workflow-modal-request-delete [name=cmm-value]").attr("id");
            var CmmValue = tinymce.get(tinyMceId).getContent();

            obj2send = {
                "RlsCode": app.release.RlsCode,
                "RqsCode": RqsCode,
                "CmmValue": CmmValue
            };
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            var tinyMceId = $("#request-workflow-modal-request-rollback [name=cmm-value]").attr("id");
            var CmmValue = tinymce.get(tinyMceId).getContent();

            obj2send = {
                "RlsCode": app.release.RlsCode,
                "RqsCode": RqsCode,
                "CmmValue": CmmValue
            };
            break;
    }

    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Workflow.WorkflowRequest_API.Create",
        obj2send,
        "app.release.workflow.modal.request.callback.create",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} data
 */
app.release.workflow.modal.request.callback.create = function (data) {
    if (data == C_APP_API_SUCCESS) {
        var goToParams = {
            "RlsCode": app.release.RlsCode,
            "MtrCode": app.release.MtrCode
        };
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [""]));
        $("#modal-success").one('hidden.bs.modal', function (e) {
            // Force page reload
            api.content.goTo("entity/release/", "#nav-link-release", "#nav-link-release", goToParams);
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
 * @param {*} date
 */
app.release.workflow.modal.request.setDefaultPublishTime = function (date) {
    var time = app.config.embargo.time.split(":");
    var defaultDate = new Date(date);
    defaultDate.setHours(parseInt(time[0]), parseInt(time[1]), parseInt(time[2]));
    return defaultDate;
};

/**
 * 
 */
app.release.workflow.modal.request.checkDatetime = function () {
    var date = new Date();
    var isEmergency = $("#request-workflow-modal-request-publish [name=wrq-emergency-flag]").prop("checked");
    if (!isEmergency) {
        if (date > app.release.workflow.modal.request.setDefaultPublishTime(date)) {
            // Too late, changed date to tomorrow
            date.setDate(date.getDate() + 1);

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            while ($.inArray(date.getDay(), app.config.embargo.day) == -1) {
                // Move the date forward till finding a suitable day
                date.setDate(date.getDate() + 1);
            }
        }
        // Enforce Default Publish Time
        date = app.release.workflow.modal.request.setDefaultPublishTime(date);
    }

    $('#request-workflow-modal-request-publish [name=wrq-datetime]').unbind().daterangepicker({
        "minDate": moment(date).format(app.config.mask.datetime.dateRangePicker),
        "startDate": moment(date).format(app.config.mask.datetime.dateRangePicker),
        "singleDatePicker": true,
        "timePicker": isEmergency ? true : false,
        "timePicker24Hour": true,
        "timePickerIncrement": 1,
        "locale": app.label.plugin.daterangepicker,
        "isInvalidDate": function (date) {
            if (isEmergency) {
                return false;
            } else {
                // Filter by Embargo Days
                return $.inArray(date.toDate().getDay(), app.config.embargo.day) == -1;
            }
        }
    }).val(moment(date).format(app.config.mask.datetime.display)).once("change", function () {
        var isEmergency = $("#request-workflow-modal-request-publish [name=wrq-emergency-flag]").prop("checked");
        if (!isEmergency) {
            //Overwrite the default time by DateRangePicker (00:00:00) with the Default Publish Time
            emergencyDate = moment($(this).val(), app.config.mask.datetime.display).toDate();
            emergencyDate = app.release.workflow.modal.request.setDefaultPublishTime(emergencyDate);
            $(this).val(moment(emergencyDate).format(app.config.mask.datetime.display));
        }
    });
};
//#endregion