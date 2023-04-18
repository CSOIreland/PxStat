/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = app.build || {};
app.build.create.import = {};
app.build.create.import.ajax = {};
app.build.create.import.callback = {};
app.build.create.import.validate = {};
app.build.create.import.validate.ajax = {};
app.build.create.import.validate.callback = {};
//#endregion

/**
 *
 */
app.build.create.import.cancel = function () {
    //clean up modal
    $("#build-create-import").find("[name=upload-file-name]").empty().hide();
    $("#build-create-import").find("[name=upload-file-tip]").show();

    // Disable Parse Button
    $("#build-create-import").find("[name=parse-source-file]").prop("disabled", true);

    //clean up namespaced variables
    app.build.create.file.import.FrqCode = null;
    app.build.create.file.import.FrqValue = null;
    app.build.create.file.import.Signature = null;
    app.build.create.file.import.content.UTF8 = null;
    app.build.create.file.import.content.Base64 = null;
};

app.build.create.import.reset = function () {
    $("#build-create-import").find("[name=build-create-import-file]").val("");
    app.build.create.import.cancel();
}

/**
 *
 */
app.build.create.import.validate.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.Validate",
        {
            "FrqCodeTimeval": app.build.create.file.import.FrqCode,
            "FrqValueTimeval": app.build.create.file.import.FrqValue,
            "MtrInput": app.build.create.file.import.content.Base64,
            "LngIsoCode": app.label.language.iso.code
        },
        "app.build.create.import.validate.callback.read",
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        }
    );
    // Add the progress bar
    api.spinner.progress.start(api.spinner.progress.getTimeout(app.build.create.file.import.content.Base64.length, app.config.transfer.unitsPerSecond["PxStat.Build.Build_API.Read"]));
};

app.build.create.import.validate.callback.read = function (data) {
    if (data) {
        if (data.Signature) {
            // Store for later use
            app.build.create.file.import.Signature = data.Signature;
            app.build.create.import.ajax.read();
        }
        else {
            // Populate the Frequency list
            $("#build-create-modal-frequency").find("[name=frequency-radio-group]").empty();

            $.each(data.FrqValueCandidate, function (key, value) {
                $("#build-create-modal-frequency").find("[name=frequency-radio-group]").append(function () {
                    return $("<li>", {
                        "class": "list-group-item",
                        "html": $("<input>", {
                            "type": "radio",
                            "name": "frq-value",
                            "value": value
                        }).get(0).outerHTML + " " + value
                    }).get(0).outerHTML;
                });
            });
            // Show the modal
            $("#build-create-modal-frequency").modal("show");
        }
    }
    else api.modal.exception(app.label.static["api-ajax-exception"]);

};


app.build.create.import.ajax.read = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Build.Build_API.Read",
        {
            "FrqCodeTimeval": app.build.create.file.import.FrqCode,
            "FrqValueTimeval": app.build.create.file.import.FrqValue,
            "MtrInput": app.build.create.file.import.content.Base64,
            "Signature": app.build.create.file.import.Signature
        },
        "app.build.create.import.callback.read",
        null,
        null,
        null,
        {
            async: false,
            timeout: app.config.transfer.timeout
        });
};

app.build.create.import.callback.read = function (data) {
    if (data && Array.isArray(data) && data.length) {
        $("#build-create-import").modal("hide");

        app.build.create.file.import.content.JsonStat = [];
        //parse each JSON-stat data and push to namespace variable
        $.each(data, function (index, data) {

            var jsonStat = data ? JSONstat(data) : null;
            if (jsonStat && jsonStat.length) {
                app.build.create.file.import.content.JsonStat.push(jsonStat);
            } else {
                haveAllParsed = false;
                app.build.create.file.import.content.JsonStat = [];
                api.modal.exception(app.label.static["api-ajax-exception"]);
                return false;
            }
        });

        if (app.build.create.file.import.content.JsonStat.length)
            app.build.create.import.callback.read.drawProperties();

    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.build.create.import.callback.read.drawProperties = function () {
    //call this to clear any errors and reset the validation
    app.build.create.initiate.validation.setup();

    //get matrix code
    var defaultData = app.build.create.file.import.content.JsonStat[0];

    var mrtValue = defaultData.extension.matrix;
    $("#build-create-initiate-setup").find("[name=mtr-value]").val(mrtValue);

    //get and set the frequency code
    var frqCode = null;
    for (i = 0; i < defaultData.length; i++) {
        if (defaultData.Dimension(i).role == "time") {
            frqCode = defaultData.id[i];
        }
    };

    //make unselected by default
    $("#build-create-initiate-setup [name=frequency-code]").val("select");
    $("#build-create-initiate-setup [name=frequency-code] > option").each(function () {
        if (this.value == frqCode) {
            $("#build-create-initiate-setup [name=frequency-code]").val(this.value);
            return;
        }
    });

    //make unselected by default
    $("#build-create-initiate-setup [name=copyright-code]").val("select");
    //if valid copyright, select this
    $("#build-create-initiate-setup [name=copyright-code] > option").each(function () {
        if (this.value == defaultData.extension.copyright.code) {
            $("#build-create-initiate-setup [name=copyright-code]").val(this.value);
            return;
        }
    });

    //get and set the official flag
    if (defaultData.extension.official) {
        $("#build-create-initiate-setup [name=official-flag]").bootstrapToggle('on');
    }
    else {
        $("#build-create-initiate-setup [name=official-flag]").bootstrapToggle('off');
    }

    //set languages
    var importLanguages = [];
    $.each(app.build.create.file.import.content.JsonStat, function (index, data) {
        importLanguages.push(data.extension.language.code)
    });
    $("[name=lng-group]").each(function () {
        if (jQuery.inArray($(this).attr("value"), importLanguages) > -1) {
            $(this).prop("checked", true);
        }
        else {
            $(this).prop("checked", false);
        }
    });

    //always check default language
    $("[name=lng-group][value=" + app.config.language.iso.code + "]").prop("checked", true);
}

/**
 * Bind validation for frequency validation
 */
app.build.create.import.validate.frequencyModal = function () {
    $("#build-create-modal-frequency form").trigger("reset").validate({
        rules: {
            "frq-value": {
                required: true
            },
            "frq-code": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#build-create-modal-frequency [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            $("#build-create-modal-frequency").modal("hide");
            // Store for later use
            app.build.create.file.import.FrqCode = $("#build-create-modal-frequency").find("[name=frq-code]").val();
            app.build.create.file.import.FrqValue = $("#build-create-modal-frequency").find("[name=frq-value]:checked").val();

            app.build.create.import.validate.ajax.read();
        }
    }).resetForm();
};