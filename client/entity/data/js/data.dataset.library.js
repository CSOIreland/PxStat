/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces
app.data = app.data || {};

app.data.dataset = {};
app.data.dataset.ajax = {};
app.data.dataset.callback = {};

app.data.dataset.selectionCount = null;
app.data.dataset.totalCount = null;

app.data.dataset.apiParamsData = {};
//#endregion

/**
* 
*/
app.data.dataset.ajax.readMetadata = function () {
    if (app.data.MtrCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadMetadata",
            {
                "matrix": app.data.MtrCode,
                "format": {
                    "type": C_APP_FORMAT_TYPE_DEFAULT,
                    "version": C_APP_FORMAT_VERSION_DEFAULT
                },
                "language": app.data.LngIsoCode,
                "m2m": false
            },
            "app.data.dataset.callback.readMetadata",
            null,
            null,
            null,
            { async: false });
    }
    else if (app.data.RlsCode) {
        api.ajax.jsonrpc.request(app.config.url.api.private,
            "PxStat.Data.Cube_API.ReadPreMetadata",
            {
                "release": app.data.RlsCode,
                "format": {
                    "type": C_APP_FORMAT_TYPE_DEFAULT,
                    "version": C_APP_FORMAT_VERSION_DEFAULT
                },
                "language": app.data.LngIsoCode,
                "m2m": false
            },
            "app.data.dataset.callback.readMetadata",
            null,
            null,
            null,
            { async: false });
    }
};

/**
* 
* @param {*} data
*/
app.data.dataset.callback.readMetadata = function (data) {
    data = data ? JSONstat(data) : data;

    if (data && data.length) {
        if (app.data.MtrCode) {
            app.navigation.breadcrumb.set([data.extension.subject.value, {
                "text": data.extension.product.value,
                "goTo": {
                    "pRelativeURL": "entity/data/",
                    "pNav_link_SelectorToHighlight": "#nav-link-data",
                    "pParams": {
                        "PrcCode": data.extension.product.code
                    }
                }
            }]);
        };

        if (app.data.MtrCode) {
            app.data.dataset.callback.readMatrixNotes(data);
        }
        app.data.dataset.callback.drawTableSelection(data);
        app.data.dataview.ajax.format();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

app.data.dataset.confirmSoftThreshold = function (pMessage, pCallbackMethod, pCallbackParams) {

    // Set the body of the Modal - Empty the container first
    $("#data-dataview-confirm-soft").find(C_API_SELECTOR_MODAL_BODY).empty().html(pMessage);

    $("#modal-button-confirm-data").once("click", function () {
        // Run the Callback function
        pCallbackMethod(pCallbackParams);

        // Close the Modal
        $("#data-dataview-confirm-soft").modal('hide');
    });

    // Display the Modal
    $("#data-dataview-confirm-soft").modal();
};

app.data.dataset.confirmHardThreshold = function (pMessage) {
    // Set the body of the Modal
    $("#data-dataview-confirm-hard").find(C_API_SELECTOR_MODAL_BODY).empty().html(pMessage);

    // Display the Modal
    $("#data-dataview-confirm-hard").modal();
}
/**
* 
* @param {*} data
*/
app.data.dataset.callback.drawTableSelection = function (data) {
    //hide no search results message
    $("#data-search-row-desktop [name=no-search-results], #data-search-row-responsive [name=no-search-results]").hide();
    $("#data-search-row-desktop [name=search-input], #data-search-row-responsive [name=search-input]").val("");

    //hide back button if viewing data from release entity
    if (app.data.RlsCode) {
        $("#data-dataset-row").find("[name=back-to-select-results]").hide();
    };

    $("button [name=button-show-data-text]").text(app.label.static["view-all"]);
    var matrixSelection = $("#data-dataset-templates").find("[name=matrix-selection]").clone();
    matrixSelection.find("[name=mtr-title]").text(data.label);
    matrixSelection.find("[name=mtr-code]").text(data.extension.matrix);
    //update date
    if (!data.updated || data.updated == C_APP_DATETIME_DEFAULT) {
        matrixSelection.find("[name=updated-date-and-time]").addClass("d-none");
    }
    else {
        matrixSelection.find("[name=updated-date]").text(data.updated ? moment(data.updated, app.config.mask.datetime.ajax).format(app.config.mask.date.display) : "");
        matrixSelection.find("[name=updated-time]").text(data.updated ? moment(data.updated, app.config.mask.datetime.ajax).format(app.config.mask.time.display) : "");
    }
    // emergency flag
    if (data.extension.emergency) {
        matrixSelection.find("[name=emergency-flag]").removeClass("d-none");
    }
    //geo flag
    if (data.role.geo && app.config.plugin.highcharts.enabled) {
        matrixSelection.find("[name=geo-flag]").removeClass("d-none");
        matrixSelection.find("[name=map-header]").removeClass("d-none");
    }
    //official flag
    if (!data.extension.official) {
        matrixSelection.find("[name=official-flag]").removeClass("d-none");
    }
    //analytical flag
    if (data.extension.analytical) {
        matrixSelection.find("[name=analytical-flag]").removeClass("d-none");
    }
    //archive flag
    if (data.extension.archive) {
        matrixSelection.find("[name=archive-flag], [name=archive-header]").removeClass("d-none");
    }
    //dependency flag
    if (data.extension.dependency) {
        matrixSelection.find("[name=dependency-flag]").removeClass("d-none");
    }
    //reservation flag
    if (data.extension.reservation) {
        matrixSelection.find("[name=reservation-flag], [name=under-reservation-header]").removeClass("d-none");
    }
    //Add badge for language.
    matrixSelection.find("[name=language]").text(data.extension.language.name);

    //dimension pill
    for (i = 0; i < data.length; i++) {
        if (data.Dimension(i).role == "classification" || data.Dimension(i).role == "geo") {
            var dimension = $("#data-dataset-templates").find("[name=dimension]").clone();
            dimension.text(data.Dimension(i).label);
            matrixSelection.find("[name=dimensions]").append(dimension);
        }
    }

    for (i = 0; i < data.length; i++) {
        if (data.Dimension(i).role == "time") {
            //frequency pill
            var frequency = $("#data-metadata-templates").find("[name=frequency]").clone();
            frequency.text(data.Dimension(i).label);
            matrixSelection.find("[name=dimensions]").append(frequency);

            //frequency span
            var frequencySpan = $("#data-metadata-templates").find("[name=frequency-span]").clone();
            frequencySpan.text(function () {
                return "[" + data.Dimension(i).Category(0).label + " - " + data.Dimension(i).Category(data.Dimension(i).length - 1).label + "]";
            });
            matrixSelection.find("[name=dimensions]").append(frequencySpan);
        }
    }

    //copyright
    matrixSelection.find("[name=copyright]").html(
        $("<i>", {
            class: "far fa-copyright mr-1"
        }).get(0).outerHTML + data.extension.copyright.name
    ).attr("href", data.extension.copyright.href);

    //build select elements
    app.data.dataset.totalCount = 1;
    for (i = 0; i < data.length; i++) {
        app.data.dataset.totalCount = app.data.dataset.selectionCount = app.data.dataset.totalCount * data.Dimension(i).length;
        var dimensionContainer = $("#data-dataset-templates").find("[name=dimension-container]").clone();
        var dimensionCode = $("<small>", {
            "text": " - " + data.id[i],
            "name": "dimension-code",
            "class": "d-none"
        }).get(0).outerHTML;

        dimensionContainer.find("[name=dimension-label]").html(data.Dimension(i).label + dimensionCode);
        dimensionContainer.find("[name=dimension-count]").text(data.Dimension(i).length);

        dimensionContainer.find("[name=select-all]").attr("idn", data.id[i]);
        dimensionContainer.find("[name=sort-options]").attr("idn", data.id[i]);
        dimensionContainer.find("[name=dimension-filter]").attr("idn", data.id[i]);

        if (data.Dimension(i).role == "time") {
            $.each(data.Dimension(i).id, function (index, value) {
                var option = $('<option>', {
                    value: value,
                    "code-true": data.Dimension(i).Category(index).label + " (" + value + ")",
                    "code-false": data.Dimension(i).Category(index).label,
                    text: data.Dimension(i).Category(index).label,
                    title: data.Dimension(i).Category(index).label,
                    filtered: 'true',
                });
                dimensionContainer.find("select").append(option);
            });
            dimensionContainer.find("select").attr("idn", data.id[i]).attr("role", data.Dimension(i).role).attr("sort", "desc");
            //reverse select based on codes so most recent time first
            dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                return $(x).val() < $(y).val() ? 1 : -1;
            }));
        }
        else {
            $.each(data.Dimension(i).id, function (index, value) {
                var option = $('<option>', {
                    value: value,
                    "code-true": data.Dimension(i).Category(index).label + (data.Dimension(i).Category(index).unit ? " (" + data.Dimension(i).Category(index).unit.label + ")" : "") + " (" + value + ")",
                    "code-false": data.Dimension(i).Category(index).label + (data.Dimension(i).Category(index).unit ? " (" + data.Dimension(i).Category(index).unit.label + ")" : ""),
                    text: data.Dimension(i).Category(index).label + (data.Dimension(i).Category(index).unit ? " (" + data.Dimension(i).Category(index).unit.label + ")" : ""),
                    title: data.Dimension(i).Category(index).label,
                    filtered: 'true',
                });
                dimensionContainer.find("select").append(option);
            });
            dimensionContainer.find("select").attr("idn", data.id[i]).attr("role", data.Dimension(i).role);
        }
        matrixSelection.find("[name=dimension-containers]").append(dimensionContainer.get(0).outerHTML);
    }
    matrixSelection.find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.totalCount));
    matrixSelection.find("[name=data-total-cells]").text(app.library.utility.formatNumber(app.data.dataset.totalCount));
    $("#data-dataview-selected-table").html(matrixSelection.get(0).outerHTML);
    //select all
    $("#data-dataview-selected-table").find("input[name=select-all]").once("change", function () {
        var dimension = $(this).attr("idn");
        var select = $("#data-dataview-selected-table").find("select[idn='" + dimension + "']");
        //clear filter
        $("#data-dataview-selected-table").find("input[name=dimension-filter][idn='" + dimension + "']").val("").trigger("search");
        select.find("option:enabled").prop('selected', $(this).is(':checked'));
        app.data.dataset.callback.countSelection();
        app.data.dataset.callback.buildApiParams();
    });

    //sort
    $("#data-dataview-selected-table").find("[name=sort-options]").once("click", function () {
        var dimension = $(this).attr("idn");
        var select = $("#data-dataview-selected-table").find("select[idn='" + dimension + "']");
        var status = select.attr("sort");

        select.html(select.find('option').sort(function (x, y) {
            switch (status) {
                case "asc":
                    select.attr("sort", "desc");
                    return $(x).text() < $(y).text() ? 1 : -1;
                case "desc":
                    select.attr("sort", "asc");
                    return $(x).text() > $(y).text() ? 1 : -1;
                default:
                    break;
            }

        }));

    });


    //filter
    $("#data-dataview-selected-table").find("[name=dimension-filter]").once("keyup search", function () {
        var dimension = $(this).attr("idn");
        var selectAll = $("#data-dataview-selected-table").find("input[name=select-all][idn='" + dimension + "']");
        var select = $("#data-dataview-selected-table").find("select[idn='" + dimension + "']");
        var filter = $(this).val().toLowerCase();

        //enable case insensitive filtering
        select.find('option:enabled').each(function () {
            if ((this.text.toLowerCase().indexOf(filter) > -1) || $(this).is(':selected')) {
                select.find('option:disabled').attr('class', 'd-none');
                selectAll.prop("disabled", false);
                $(this).attr('filtered', 'true').show();
            }
            else {
                $(this).attr('filtered', 'false').hide();
            }
        });
        var numberVisible = select.find('option').filter(function () {
            return $(this).css('display') !== 'none';
        }).length;

        if (numberVisible == 0) {
            $(select.find('option:disabled').attr('class', 'd-block'));
            selectAll.prop("disabled", true);
        }
        else {

        }

    });


    //click on an option, refresh count
    $("#data-dataview-selected-table").find("[name=dimension-select]").once('change', function () {
        $("button [name=button-show-data-text]").text(app.label.static["view-selection"]);
        app.data.dataset.callback.countSelection();
        app.data.dataset.callback.buildApiParams();
    });

    //reset
    $("#data-dataview-selected-table").find("[name=reset]").once("click", function () {

        $("#data-dataview-selected-table").find("#code-toggle-select").bootstrapToggle('on');


        $("#data-dataview-selected-table").find("[name=dimension-select]").each(function () {
            $(this).find('option').prop('selected', false);
        });
        $("#data-dataview-selected-table").find("[name=dimension-filter]").each(function () {
            $(this).val("").trigger("keyup");
        });

        $("#data-dataview-selected-table").find("[name=select-all]").each(function () {
            $(this).prop('checked', false);
        });

        $("#data-dataview-selected-table").find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.totalCount));
        $("button [name=button-show-data-text]").text(app.label.static["view-all"]);
        app.data.dataset.callback.buildApiParams();

    });

    //show codes
    $('#code-toggle-select').bootstrapToggle("destroy").bootstrapToggle({
        on: app.label.static["false"],
        off: app.label.static["true"],
        onstyle: "tertiary",
        offstyle: "neutral",
        width: C_APP_TOGGLE_LENGTH
    });

    $('#code-toggle-select').once("change", function () {
        if (!$(this).prop('checked')) {

            $("#data-dataview-selected-table").find("[name=dimension-code]").each(function () {
                $(this).removeClass("d-none");
            });

            $("#data-dataview-selected-table").find("option").each(function () {
                var codeTrue = $(this).attr("code-true");
                $(this).text(codeTrue);
                $(this).attr("title", codeTrue);

            });
        }
        else {
            $("#data-dataview-selected-table").find("[name=dimension-code]").each(function () {
                $(this).addClass("d-none");
            });

            $("#data-dataview-selected-table").find("option").each(function () {
                var codeFalse = $(this).attr("code-false");
                $(this).text(codeFalse);
                $(this).attr("title", codeFalse);

            });
        }
    });

    //show data
    $("#data-dataview-selected-table").find("[name=show-data]").once("click", function () {
        if (app.data.dataset.selectionCount >= app.config.entity.data.threshold.hard) {
            app.data.dataset.confirmHardThreshold(app.library.html.parseDynamicLabel("error-read-exceeded", [app.library.utility.formatNumber(app.data.dataset.selectionCount), app.config.entity.data.threshold.hard]));
        }
        else if (app.data.dataset.selectionCount >= app.config.entity.data.threshold.soft) {
            app.data.dataset.confirmSoftThreshold(app.library.html.parseDynamicLabel("confirm-read", [app.library.utility.formatNumber(app.data.dataset.selectionCount)]), app.data.dataview.ajax.data);
        }
        else {
            //AJAX call get Data Set
            app.data.dataview.ajax.data();
        }
    });


    $("#data-dataview-selected-table").find("[name=show-data-map]").once("click", function (e) {
        e.preventDefault();
        var status = $(this).attr("status");
        if (status == "data") { //currently data, switch to map
            $("#data-view-container").empty();
            $("#data-view-container").empty();
            $(this).attr("status", "map");
            $(this).find("[name=text]").text(app.label.static.data);
            $("#data-dataview-selected-table").find("[name=map-container]").removeClass("d-none");
            $("#data-dataview-selected-table").find("[name=dimension-containers]").addClass("d-none");
            $("#data-dataview-selected-table").find("[name=card-footer]").hide();
            var apiParams = {
                "language": app.data.LngIsoCode,
                "format": {
                    "type": C_APP_FORMAT_TYPE_DEFAULT,
                    "version": C_APP_FORMAT_VERSION_DEFAULT
                },
                "m2m": false
            };
            if (app.data.MtrCode) {
                apiParams.matrix = app.data.MtrCode;
            }
            else if (app.data.RlsCode) {
                apiParams.matrix = app.data.RlsCode;
            }
            if (app.config.plugin.highcharts.enabled) {
                app.data.dataview.ajax.mapMetadata(apiParams);
            }
        }
        else { //currently map, switch to data
            $("#data-accordion-api").show();
            $(this).attr("status", "data");
            $(this).find("[name=text]").text("Map");
            $("#data-dataview-selected-table").find("[name=map-container]").addClass("d-none");
            $("#data-dataview-selected-table").find("[name=dimension-containers]").removeClass("d-none");
            $("#data-dataview-selected-table").find("[name=card-footer]").show();
            app.data.dataset.callback.buildApiParams();
        }
    });
    app.data.dataset.callback.buildApiParams();
    $("#data-dataset-row").show();
    $('[data-toggle="tooltip"]').tooltip();

    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
};

/**
* Callback Read Matrix Notes
* @param {*} data
*/
app.data.dataset.callback.readMatrixNotes = function (data) {

    var matrixNotes = $("#data-dataset-templates").find("[name=matrix-notes]").clone();

    //notes
    if (data.note && data.note.length) {
        matrixNotes.find("[name=notes]").removeClass("d-none");
        $.each(data.note, function (index, value) {
            matrixNotes.find("[name=notes]").find(".card-body").append(
                $("<p>", {
                    html: app.library.html.parseBbCode(value)
                }).get(0).outerHTML
            );
        });
    }

    //reasons
    if (data.extension.reasons && data.extension.reasons.length) {
        matrixNotes.find("[name=reasons]").removeClass("d-none");
        var reasons = data.extension.reasons;
        $.each(reasons, function (index, value) {
            var reason = $("#data-dataset-templates").find("[name=reason]").clone();
            reason.html(app.library.html.parseBbCode(value));
            matrixNotes.find("[name=reasons]").find(".card-body").find(".list-group").append(reason);
        });

    }

    //contact name
    if (data.extension.contact.name) {
        matrixNotes.find("[name=contact-name]").text(data.extension.contact.name);
    }
    else {
        matrixNotes.find("[name=contact-name-row]").remove();
    }

    //contact email
    if (data.extension.contact.email) {

        matrixNotes.find("[name=contact-email]").html(app.library.html.email(data.extension.contact.email));
    }
    else {
        matrixNotes.find("[name=contact-email-row]").remove();
    }

    //contact phone
    if (data.extension.contact.phone) {
        matrixNotes.find("[name=contact-phone]").text(data.extension.contact.phone);
    }
    else {
        matrixNotes.find("[name=contact-phone-row]").remove();
    };

    $("#panel").html(matrixNotes.get(0).outerHTML);
    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

    // Run Sharethis.
    app.data.sharethis(data.extension.matrix);
    app.data.dataset.ajax.format();

};

app.data.dataset.ajax.format = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.public,
        "PxStat.System.Settings.Format_API.Read",
        {
            // "LngIsoCode": null,
            "FrmDirection": C_APP_TS_FORMAT_DIRECTION_DOWNLOAD
        },
        "app.data.dataset.callback.format"
    );
}

app.data.dataset.callback.format = function (data) {
    if (data && Array.isArray(data) && data.length) {
        $("#panel [name=matrix-notes]").find().empty();
        $.each(data, function (index, format) {
            var formatLink = $("#data-dataset-templates").find("[name=download-dataset-format]").clone();
            formatLink.attr(
                {
                    "frm-type": format.FrmType,
                    "frm-version": format.FrmVersion
                });
            formatLink.find("[name=type]").text(format.FrmType);
            formatLink.find("[name=version]").text(format.FrmVersion);
            $("#panel [name=download-full-dataset]").append(formatLink);
        });

        $("#panel [name=download-dataset-format]").once("click", function (e) {
            e.preventDefault();
            app.data.dataset.callback.fullDownload($(this).attr("frm-type"), $(this).attr("frm-version"));
        });
    }
    // Handle no data
    else api.modal.information(app.label.static["api-ajax-nodata"]);

}


app.data.dataset.callback.fullDownload = function (format, version) {
    var apiParams = {
        "matrix": app.data.MtrCode,
        "language": app.data.LngIsoCode,
        "format": {
            "type": format,
            "version": version
        },
        "m2m": false
    };

    app.data.dataset.ajax.downloadDataset(apiParams);
}

/**
* 
*/
app.data.dataset.callback.countSelection = function () {
    var count = 1;
    $("#data-dataview-selected-table").find("[name=dimension-select]").each(function () {
        var dimension = $(this).attr("idn");
        var totalOptions = $(this).find("option:enabled").length;
        var selectedOptions = $(this).find('option:selected').length;

        //if any option clicked check if select all should be ticked or not
        if (totalOptions == selectedOptions) { // all selected
            $("#data-dataview-selected-table").find("input[name=select-all][idn='" + dimension.replace("'", "\\'") + "']").prop('checked', true);
            count = count * totalOptions;
        }
        else if (selectedOptions == 0) {
            count = count * totalOptions;
        }
        else {
            count = count * $(this).find('option:selected').length;
            $("#data-dataview-selected-table").find("input[name=select-all][idn='" + dimension.replace("'", "\\'") + "']").prop('checked', false);
        }
    });
    app.data.dataset.selectionCount = count;

    $("#data-dataview-selected-table").find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.selectionCount));
};

/**
* 
*/
app.data.dataset.callback.buildApiParams = function () {
    $("#data-view-container").fadeOut();
    if (app.data.MtrCode) {
        app.data.dataset.apiParamsData = {
            "matrix": app.data.MtrCode
        };
    }
    else if (app.data.RlsCode) {
        app.data.dataset.apiParamsData = {
            "release": app.data.RlsCode
        };
    }
    var localParams = {
        "language": app.data.LngIsoCode,
        "format": {
            "type": C_APP_FORMAT_TYPE_DEFAULT,
            "version": C_APP_FORMAT_VERSION_DEFAULT
        },
        "role": {
            "time": [
                $("#data-dataview-selected-table").find("[name=dimension-containers]").find("select[role=time]").attr("idn")
            ],
            "metric": [
                $("#data-dataview-selected-table").find("[name=dimension-containers]").find("select[role=metric]").attr("idn")
            ]
        },
        "dimension": [],
        "m2m": false
    };

    $("#data-dataview-selected-table").find("[name=dimension-containers]").find("select").each(function (index) {
        var numVariables = $(this).find('option:enabled').length;
        var dimension = {
            "id": $(this).attr("idn"),
            "category": {
                "index": []
            }
        };
        $(this).find('option:selected[filtered=true]').each(function () {
            dimension.category.index.push(this.value);
        });
        if (dimension.category.index.length != numVariables && dimension.category.index.length > 0) { //only include dimension if not all variables selected
            localParams.dimension.push(dimension);
        }
    });

    //extend apiParams with local params
    $.extend(true, app.data.dataset.apiParamsData, localParams);
    $("#data-accordion-api").find("[name=github-link]").attr("href", C_APP_URL_GITHUB_API_CUBE);
    $("#data-accordion-api").find("[name=api-url]").text(app.config.url.api.public);
    $("#data-accordion-api").find("[name=api-object]").text(function () {
        var JsonQuery = {
            "jsonrpc": C_APP_API_JSONRPC_VERSION,
            "method": "PxStat.Data.Cube_API.ReadDataset",
            "params": null
        };
        var apiParams = $.extend(true, {}, app.data.dataset.apiParamsData);
        delete apiParams.m2m;
        JsonQuery.params = apiParams;
        return JSON.stringify(JsonQuery, null, "\t");
    });
    // Refresh the Prism highlight
    Prism.highlightAll();
    $("#data-accordion-api").show();
};

app.data.dataset.callback.back = function () {

    //first check if we have search results to display
    var numSearchResults = $("#data-metadata-row [name=search-results] [name=search-result-item]").length;
    if (numSearchResults > 0) {
        //back to search results
        $("#data-dataview-selected-table, #panel, #data-view-container").empty();
        $("#data-metadata-row, #data-filter, #data-search-results-pagination [name=pagination]").show();
        $("#data-accordion-api").hide();
        //run bootstrap toggle to show/hide toggle button
        bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    }
    else {
        // Load default Entity
        api.content.goTo("entity/data/", "#nav-link-data");
    }
    $("#data-dataset-row").find("[name=back-to-select-results]").hide();

}
//#region download dataset

/**
* 
* @param {*} apiParams
*/
app.data.dataset.ajax.downloadDataset = function (apiParams) {
    if (app.data.MtrCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadDataset",
            apiParams,
            "app.data.dataset.callback.downloadDataset",
            apiParams,
            null,
            null,
            { async: false });
    }
    else if (app.data.RlsCode) {
        api.ajax.jsonrpc.request(
            app.config.url.api.private,
            "PxStat.Data.Cube_API.ReadPreDataset",
            apiParams,
            "app.data.dataset.callback.downloadDataset",
            apiParams,
            null,
            null,
            { async: false });
    }
};

/**
* 
* @param {*} data
* @param {*} apiParams
*/
app.data.dataset.callback.downloadDataset = function (data, apiParams) {
    var fileName = app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file);

    switch (apiParams.format.type) {
        case C_APP_TS_FORMAT_TYPE_PX:
            // Download the file
            app.library.utility.download(fileName, data, C_APP_EXTENSION_PX, C_APP_MIMETYPE_PX);
            break;
        case C_APP_TS_FORMAT_TYPE_JSONSTAT:
            // Download the file
            app.library.utility.download(fileName, JSON.stringify(data), C_APP_EXTENSION_JSON, C_APP_MIMETYPE_JSON);
            break;
        case C_APP_TS_FORMAT_TYPE_CSV:
            // Download the file
            app.library.utility.download(fileName, data, C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
            break;
        case C_APP_TS_FORMAT_TYPE_XLSX:
            // Download the file
            app.library.utility.download(fileName, data, C_APP_EXTENSION_XLSX, null, true);
            break;
        default:
            api.modal.exception(app.label.static["api-ajax-exception"]);
            break;
    }
};

//#endregion

