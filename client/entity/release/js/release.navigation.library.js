/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.navigation = {};
app.release.navigation.associatedProducts = [];
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
    $("#release-navigation-modal").find("[name=prc-code]").empty();
    if (!SbjCode) {
        //no subject, so disable product dropdown
        $("#release-navigation-modal").find("[name=prc-code]").prop('disabled', true);
        return
    }
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
        // Enable the ability to set the association when core product is set
        $("#release-information [name=update-association]").attr("disabled", false);
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

//#region associated product / subject
app.release.navigation.ajax.associationRead = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.ReleaseProductAssociation_API.Read",
        {
            "RlsCode": app.release.RlsCode,
            "LngIsoCode": app.label.language.iso.code
        },
        "app.release.navigation.callback.associationRead")

};

app.release.navigation.callback.associationRead = function (data) {
    //add products to array for later use
    app.release.navigation.associatedProducts = [];
    $.each(data, function (index, value) {
        app.release.navigation.associatedProducts.push(value.PrcCode);
    });

    app.release.navigation.associatedProducts
    $("#release-navigation-association-read-modal").modal("show");
    if ($.fn.dataTable.isDataTable("#release-navigation-association-read-modal table")) {
        app.library.datatable.reDraw("#release-navigation-association-read-modal table", data);
    } else {
        var localOptions = {
            data: data,
            columns: [
                { data: "SbjValue" },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.PrcCode + " (" + row.PrcValue + ")";
                    }
                },
                {
                    data: null,
                    render: function (_data, _type, row) {
                        return app.library.html.link.external({ name: "product-url-" + row.PrcCode }, app.config.url.application + C_COOKIE_LINK_PRODUCT + "/" + row.PrcCode)
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.PrcCode, prcValue: row.PrcValue });
                    },
                    "width": "1%"
                }],
            drawCallback: function (settings) {
                app.release.navigation.callback.drawCallbackAssociationRead();

            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#release-navigation-association-read-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.release.navigation.callback.drawCallbackAssociationRead();
        });
    }

};

app.release.navigation.callback.drawCallbackAssociationRead = function () {
    // Delete action
    $("#release-navigation-association-read-modal table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
        api.modal.confirm(
            app.library.html.parseDynamicLabel("confirm-delete", [$(this).attr("idn") + " (" + $(this).attr("prcValue") + ")"]),
            app.release.navigation.ajax.associationDelete,
            {
                prcCode: $(this).attr("idn"),
                prcValue: $(this).attr("prcValue")
            }

        );
    });
};

app.release.navigation.ajax.associationDelete = function (params) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.ReleaseProductAssociation_API.Delete",
        {
            "RlsCode": app.release.RlsCode,
            "PrcCode": params.prcCode
        },
        "app.release.navigation.callback.associationDelete",
        params)
};

app.release.navigation.callback.associationDelete = function (data, params) {
    if (data == C_API_AJAX_SUCCESS) {
        // Display Success Modal
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [params.prcCode + " (" + params.prcValue + ")"]));
        app.release.navigation.ajax.associationRead();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

app.release.navigation.associationAdd = function () {
    app.release.navigation.validation.associationAdd();
    app.release.navigation.ajax.associationReadSubject();
    $("#release-navigation-association-add-modal").modal("show");
};


app.release.navigation.ajax.associationReadSubject = function () {
    // Disable Product because it depends on Subject
    $("#release-navigation-association-add-modal [name=prc-code]").prop('disabled', true);

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Subject_API.Read",
        null,
        "app.release.navigation.callback.associationReadSubject")
};

/**
* 
 * @param {*} data
 * @param {*} params
 */
app.release.navigation.callback.associationReadSubject = function (data) {
    // Load select2
    $("#release-navigation-association-add-modal").find("[name=sbj-code]").empty().append($("<option>")).select2({
        dropdownParent: $('#release-navigation-association-add-modal'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.release.navigation.mapDataSubject(data)
    });

    // Enable and Focus Search input
    $("#release-navigation-association-add-modal").find("[name=sbj-code]").prop('disabled', false);

    //Update Subject search Search functionality
    $("#release-navigation-association-add-modal").find("[name=sbj-code]").on('select2:select', function (e) {
        var selectedObject = e.params.data;
        if (selectedObject) {
            // Some item from your model is active!
            if (selectedObject.id.toLowerCase() == $("#release-navigation-association-add-modal").find("[name=sbj-code]").val().toLowerCase()) {
                // This means the exact match is found. Use toLowerCase() if you want case insensitive match.
                app.release.navigation.ajax.associationReadProduct(selectedObject.id);
            }
            else {
                // Disable Product if the Subject does not match
                $("#release-navigation-association-add-modal [name=prc-code]").prop('disabled', true);
            }
        } else {
            // Disable Product if the Subject does not match
            $("#release-navigation-association-add-modal [name=prc-code]").prop('disabled', true);
        }
    });

};

/**
* 
 * @param {*} SbjCode
 * @param {*} PrcCode
 */
app.release.navigation.ajax.associationReadProduct = function (SbjCode) {
    SbjCode = SbjCode || null;

    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.System.Navigation.Product_API.Read",
        { SbjCode: SbjCode },
        "app.release.navigation.callback.associationReadProduct",
        SbjCode
    );
};

/**
* 
 * @param {*} data
 * @param {*} params
 */
app.release.navigation.callback.associationReadProduct = function (data) {
    // Load select2
    //delete core product from dropdown if there so it cannot be set as associate
    app.release.navigation.associatedProducts.push(app.release.PrcCode);
    var itemsForDropdown = $.grep(app.release.navigation.mapDataProduct(data), function (el, i) {
        if ($.inArray(el.PrcCode, app.release.navigation.associatedProducts) != -1) {
            return false; // product already used, delete from list
        }
        return true; // product not already used, keep in list
    });


    $("#release-navigation-association-add-modal").find("[name=prc-code]").empty().append($("<option>")).select2({
        dropdownParent: $('#release-navigation-association-add-modal'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: itemsForDropdown
    });

    // Enable Search input
    $("#release-navigation-association-add-modal").find("[name=prc-code]").prop('disabled', false);
};

app.release.navigation.validation.associationAdd = function () {
    $("#release-navigation-association-add-modal form").trigger("reset").validate({
        rules: {
            "sbj-code": {
                required: true
            },
            "prc-code": {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#release-navigation-association-add-modal [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            var PrcCode = $("#release-navigation-association-add-modal [name=prc-code]").val();
            app.release.navigation.ajax.addAssociation(PrcCode);
            $("#release-navigation-association-add-modal").modal("hide");
        }
    }).resetForm();
};

app.release.navigation.ajax.addAssociation = function (PrcCode) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.ReleaseProductAssociation_API.Create",
        {
            "RlsCode": app.release.RlsCode,
            "PrcCode": PrcCode
        },
        "app.release.navigation.callback.addAssociation",
        null,
        null,
        null,
        { async: false });
};

app.release.navigation.callback.addAssociation = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
        app.release.navigation.ajax.associationRead();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion