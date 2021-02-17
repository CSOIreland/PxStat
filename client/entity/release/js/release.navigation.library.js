/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.navigation = {};
app.release.navigation.ajax = {};
app.release.navigation.callback = {};
app.release.navigation.validation = {};
//#endregion

//#region Read
/** 
 * 
 */
app.release.navigation.read = function () {
    //Add validation and reset form
    app.release.navigation.validation.update();
    $("#release-navigation-modal").modal("show");
    app.release.navigation.ajax.readSubject(app.release.SbjCode);
    app.release.navigation.ajax.readProduct(app.release.SbjCode, app.release.PrcCode);
};

/**
* 
 * @param {*} SbjCode
 */
app.release.navigation.ajax.readSubject = function (SbjCode) {
    SbjCode = SbjCode || null;

    // Disable Product because it depends on Subject
    $("#release-navigation-modal [name=prc-code]").prop('disabled', true);

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Subject_API.Read",
        null,
        "app.release.navigation.callback.readSubject",
        { SbjCode: SbjCode });
};

/**
* 
 * @param {*} data
 * @param {*} params
 */
app.release.navigation.callback.readSubject = function (data, params) {
    // Load select2
    $("#release-navigation-modal").find("[name=sbj-code]").empty().append($("<option>")).select2({
        dropdownParent: $('#release-navigation-modal'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.release.navigation.mapDataSubject(data)
    });

    // Enable and Focus Search input
    $("#release-navigation-modal").find("[name=sbj-code]").prop('disabled', false);

    //Update Subject search Search functionality
    $("#release-navigation-modal").find("[name=sbj-code]").on('select2:select', function (e) {
        var selectedObject = e.params.data;
        if (selectedObject) {
            // Some item from your model is active!
            if (selectedObject.id.toLowerCase() == $("#release-navigation-modal").find("[name=sbj-code]").val().toLowerCase()) {
                // This means the exact match is found. Use toLowerCase() if you want case insensitive match.
                app.release.navigation.ajax.readProduct(selectedObject.id);
            }
            else {
                // Disable Product if the Subject does not match
                $("#release-navigation-modal [name=prc-code]").prop('disabled', true);
            }
        } else {
            // Disable Product if the Subject does not match
            $("#release-navigation-modal [name=prc-code]").prop('disabled', true);
        }
    });

    // Set Subject if any
    if (params.SbjCode) {
        /*  Multi-steps:
        *  1. Set the Value
        *  2. Trigger Change to display the set Value above
        */
        $("#release-navigation-modal").find("[name=sbj-code]").val(app.release.SbjCode).trigger("change");
    }
};

/**
* 
 * @param {*} SbjCode
 * @param {*} PrcCode
 */
app.release.navigation.ajax.readProduct = function (SbjCode, PrcCode) {
    SbjCode = SbjCode || null;
    PrcCode = PrcCode || null;

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Product_API.Read",
        { SbjCode: SbjCode },
        "app.release.navigation.callback.readProduct",
        { PrcCode: PrcCode });
};

/**
* 
 * @param {*} data
 * @param {*} params
 */
app.release.navigation.callback.readProduct = function (data, params) {
    // Load select2
    $("#release-navigation-modal").find("[name=prc-code]").empty().append($("<option>")).select2({
        dropdownParent: $('#release-navigation-modal'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.release.navigation.mapDataProduct(data)
    });

    // Enable Search input
    $("#release-navigation-modal").find("[name=prc-code]").prop('disabled', false);

    // Set Product if any
    if (params.PrcCode) {
        /*  Multi-steps:
        *  1. Set the Value
        *  2. Trigger Change to display the set Value above
        *  3. Trigger type: 'select2:select' to load the Select2 object 
        */
        $("#release-navigation-modal").find("[name=prc-code]").val(params.PrcCode).trigger("change");
    }
};

/**
 * 
 */
app.release.navigation.validation.update = function () {
    $("#release-navigation-modal form").trigger("reset").validate({
        rules: {
            "sbj-code": {
                required: true
            },
            "prc-code": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#release-navigation-modal [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            var PrcCode = $("#release-navigation-modal [name=prc-code]").val();
            app.release.navigation.ajax.update(PrcCode);
            $("#release-navigation-modal").modal("hide");
        }
    }).resetForm();
};

/**
* 
 * @param {*} PrcCode
 */
app.release.navigation.ajax.update = function (PrcCode) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Release_API.UpdateProduct",
        { "RlsCode": app.release.RlsCode, "PrcCode": PrcCode },
        "app.release.navigation.callback.update",
        null,
        null,
        null,
        { async: false });
};

/**
* 
 * @param {*} data
 */
app.release.navigation.callback.update = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
        app.release.information.read();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion

//#region Select2
/**
 * Map API data to select dropdown data model
 * @param {*} data 
 */
app.release.navigation.mapDataSubject = function (data) {
    $.each(data, function (i, item) {
        // Add ID and TEXT to the list
        data[i].id = item.SbjCode;
        data[i].text = item.SbjValue;

    });
    return data;
};
/**
 * Map API data to select dropdown data model
 * @param {*} data 
 */
app.release.navigation.mapDataProduct = function (data) {
    $.each(data, function (i, item) {
        // Add ID and TEXT to the list
        data[i].id = item.PrcCode;
        data[i].text = item.PrcCode + " (" + item.PrcValue + ")";
    });
    return data;
};
//#endregion