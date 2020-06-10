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

app.release.workflow.modal.request.fastrackResponse = false;
app.release.workflow.modal.request.fastrackSignoff = false;
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
    $("#request-workflow-modal-request-publish [name=wrq-exceptional-flag]").bootstrapToggle("off");
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
            if (app.release.workflow.modal.request.fastrackResponse) {
                $("#request-workflow-modal-request-publish [name=auto-response-warning]").show();
            }

            if (app.release.workflow.modal.request.fastrackResponse && app.release.workflow.modal.request.fastrackSignoff) {
                $("#request-workflow-modal-request-publish [name=auto-signoff-warning]").show();
            }

            //check navigation if auto signoff
            if ((!app.release.SbjCode || !app.release.PrcCode) && app.release.workflow.modal.request.fastrackResponse && app.release.workflow.modal.request.fastrackSignoff) {
                $("#request-workflow-modal-request-publish [name=navigation-warning]").show();
                $("#request-workflow-modal-request-publish [type=submit]").prop('disabled', true);
            }

            $("#request-workflow-modal-request-publish [name=rqs-value]").html(RqsValue);
            $("#request-workflow-modal-request-publish").modal("show").on('shown.bs.modal', function (e) {
                app.release.workflow.modal.request.checkDatetime();
            }).on('hide.bs.modal', function (e) { //clean up
                $("#request-workflow-modal-request-publish [name=auto-signoff-warning]").hide();
                $("#request-workflow-modal-request-publish [name=auto-response-warning]").hide();
                $("#request-workflow-modal-request-publish [name=navigation-warning]").hide();
                $("#request-workflow-modal-request-publish [type=submit]").prop('disabled', false);

            });
            break;
        case C_APP_TS_REQUEST_PROPERTY:
            app.release.workflow.modal.request.setFlag();

            if (app.release.workflow.modal.request.fastrackResponse) {
                $("#request-workflow-modal-request-flag [name=auto-response-warning]").show();
            }

            if (app.release.workflow.modal.request.fastrackResponse && app.release.workflow.modal.request.fastrackSignoff) {
                $("#request-workflow-modal-request-flag [name=auto-signoff-warning]").show();
            }

            $("#request-workflow-modal-request-flag [name=rqs-value]").html(RqsValue);
            $("#request-workflow-modal-request-flag").modal("show").on('hide.bs.modal', function (e) { //hide warnings
                $("#request-workflow-modal-request-flag [name=auto-signoff-warning]").hide();
                $("#request-workflow-modal-request-flag [name=auto-response-warning]").hide();
            });
            break;
        case C_APP_TS_REQUEST_DELETE:
            $("#request-workflow-modal-request-delete [name=rqs-value]").html(RqsValue);

            if (app.release.workflow.modal.request.fastrackResponse) {
                $("#request-workflow-modal-request-delete [name=auto-response-warning]").show();
            }

            if (app.release.workflow.modal.request.fastrackResponse && app.release.workflow.modal.request.fastrackSignoff) {
                $("#request-workflow-modal-request-delete [name=auto-signoff-warning]").show();
            }

            $("#request-workflow-modal-request-delete").modal("show").on('hide.bs.modal', function (e) { //hide warnings
                $("#request-workflow-modal-request-delete [name=auto-signoff-warning]").hide();
                $("#request-workflow-modal-request-delete [name=auto-response-warning]").hide();
            });
            break;
        case C_APP_TS_REQUEST_ROLLBACK:
            $("#request-workflow-modal-request-rollback [name=rqs-value]").html(RqsValue);

            if (app.release.workflow.modal.request.fastrackResponse) {
                $("#request-workflow-modal-request-rollback [name=auto-response-warning]").show();
            }

            if (app.release.workflow.modal.request.fastrackResponse && app.release.workflow.modal.request.fastrackSignoff) {
                $("#request-workflow-modal-request-rollback [name=auto-signoff-warning]").show();
            }

            $("#request-workflow-modal-request-rollback").modal("show").on('hide.bs.modal', function (e) { //hide warnings
                $("#request-workflow-modal-request-rollback [name=auto-signoff-warning]").hide();
                $("#request-workflow-modal-request-rollback [name=auto-response-warning]").hide();
            });
            break;
    }

    app.release.workflow.modal.request.validation.create(RqsCode);
};

app.release.workflow.modal.request.ajax.ReadCurrentAccess = function () {
    //Check the privilege of the user 
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Security.Account_API.ReadCurrentAccess",
        { CcnUsername: null },
        "app.release.workflow.modal.request.callback.ReadCurrentAccess",
        null,
        null,
        null,
        { async: false }
    );
};

app.release.workflow.modal.request.callback.ReadCurrentAccess = function (data) {
    //set to safest workflow
    app.release.workflow.modal.request.fastrackResponse = false;
    app.release.workflow.modal.request.fastrackSignoff = false;
    switch (data[0].PrvCode) {
        case C_APP_PRIVILEGE_MODERATOR:
            //if moderator initiates request and has approval rights, then allow auto response
            if (app.release.isApprover && app.config.workflow.fastrack.response.approver) {
                app.release.workflow.modal.request.fastrackResponse = true;
            }
            break;
        case C_APP_PRIVILEGE_POWER_USER:

            if (app.config.workflow.fastrack.response.poweruser) {
                app.release.workflow.modal.request.fastrackResponse = true;
            }

            if (app.config.workflow.fastrack.signoff.poweruser) {
                app.release.workflow.modal.request.fastrackSignoff = true;
            }

            break;
        case C_APP_PRIVILEGE_ADMINISTRATOR:

            if (app.config.workflow.fastrack.response.administrator) {
                app.release.workflow.modal.request.fastrackResponse = true;
            }

            if (app.config.workflow.fastrack.signoff.administrator) {
                app.release.workflow.modal.request.fastrackSignoff = true;
            }
            break;
        default:
            app.release.workflow.modal.request.fastrackResponse = false;
            app.release.workflow.modal.request.fastrackSignoff = false;
            break;
    }
    app.release.workflow.modal.request.create();
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
                "WrqExceptionalFlag": $("#request-workflow-modal-request-publish [name=wrq-exceptional-flag]").prop("checked"),
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
    var time = app.config.workflow.embargo.time.split(":");
    var defaultDate = new Date(date);
    defaultDate.setHours(parseInt(time[0]), parseInt(time[1]), parseInt(time[2]));
    return defaultDate;
};

/**
 * 
 */
app.release.workflow.modal.request.checkDatetime = function () {
    var date = new Date();
    var isExceptional = $("#request-workflow-modal-request-publish [name=wrq-exceptional-flag]").prop("checked");
    if (!isExceptional) {
        if (date > app.release.workflow.modal.request.setDefaultPublishTime(date)) {
            // Too late, changed date to tomorrow
            date.setDate(date.getDate() + 1);

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            while ($.inArray(date.getDay(), app.config.workflow.embargo.day) == -1) {
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
        "timePicker": isExceptional ? true : false,
        "timePicker24Hour": true,
        "timePickerIncrement": 1,
        "locale": app.label.plugin.daterangepicker,
        "isInvalidDate": function (date) {
            if (isExceptional) {
                return false;
            } else {
                // Filter by Embargo Days
                return $.inArray(date.toDate().getDay(), app.config.workflow.embargo.day) == -1;
            }
        }
    }).val(moment(date).format(app.config.mask.datetime.display)).once("change", function () {
        var isExceptional = $("#request-workflow-modal-request-publish [name=wrq-exceptional-flag]").prop("checked");
        if (!isExceptional) {
            //Overwrite the default time by DateRangePicker (00:00:00) with the Default Publish Time
            exceptionalDate = moment($(this).val(), app.config.mask.datetime.display).toDate();
            exceptionalDate = app.release.workflow.modal.request.setDefaultPublishTime(exceptionalDate);
            $(this).val(moment(exceptionalDate).format(app.config.mask.datetime.display));
        }
    });
};
//#endregion