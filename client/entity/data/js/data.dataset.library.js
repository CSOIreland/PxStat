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
        api.ajax.jsonrpc.request(app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadMetadata",
            {
                "matrix": app.data.MtrCode,
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
* @param {*} response
*/
app.data.dataset.callback.readMetadata = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data) {
        var data = JSONstat(response.data);

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
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
* @param {*} data
*/
app.data.dataset.callback.drawTableSelection = function (data) {
    //hide back button if viewing data from release entity
    if (app.data.RlsCode) {
        $("#data-dataset-row").find("[name=back-to-select-results]").hide();
    };

    $("button [name=button-show-data-text]").text(app.label.static["view-all"]);
    var matrixSelection = $("#data-dataset-templates").find("[name=matrix-selection]").clone();
    matrixSelection.find("[name=mtr-title]").text(data.label);
    matrixSelection.find("[name=mtr-code]").text(data.extension.matrix);
    //update date
    if (data.updated == app.config.mask.datetime.default) {
        matrixSelection.find("[name=updated-date-and-time]").addClass("d-none");
    }
    else {
        matrixSelection.find("[name=updated-date]").text(moment(data.updated, app.config.mask.datetime.ajax).format(app.config.mask.date.display));
        matrixSelection.find("[name=updated-time]").text(moment(data.updated, app.config.mask.datetime.ajax).format(app.config.mask.time.display));
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
        matrixSelection.find("[name=unofficial-flag]").removeClass("d-none");
    }
    //analytical flag
    if (data.extension.analytical) {
        matrixSelection.find("[name=analytical-flag]").removeClass("d-none");
    }
    //archive flag
    if (data.extension.archive) {
        matrixSelection.find("[name=archive-flag]").removeClass("d-none");
    }
    //dependency flag
    if (data.extension.dependency) {
        matrixSelection.find("[name=dependency-flag]").removeClass("d-none");
    }
    //reservation flag
    if (data.extension.reservation) {
        matrixSelection.find("[name=reservation-flag]").removeClass("d-none");
    }
    //Add badge for language.
    matrixSelection.find("[name=language]").text(data.extension.language.name);
    //dimension pills
    for (i = 0; i < data.length; i++) {
        if (data.Dimension(i).role == "classification") {
            var dimension = $("#data-dataset-templates").find("[name=dimension]").clone();
            dimension.text(data.Dimension(i).label);
            matrixSelection.find("[name=dimensions]").append(dimension);
        }
        if (data.Dimension(i).role == "time") {
            matrixSelection.find("[name=dimension-time]").text(data.Dimension(i).label);
            matrixSelection.find("[name=dimension-time-span]").text(function () {
                return "[" + data.Dimension(i).Category(0).label + " - " + data.Dimension(i).Category(data.Dimension(i).length - 1).label + "]";
            });
        }
    }
    //copyright
    matrixSelection.find("[name=copyright]").html(
        $("<i>", {
            class: "far fa-copyright mr-1"
        }).get(0).outerHTML + data.extension.copyright.label
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
            //reverse select so most recent time first
            dimensionContainer.find("select").html(dimensionContainer.find('option').sort(function (x, y) {
                return $(x).text() < $(y).text() ? 1 : -1;
            }));
        }
        else {
            $.each(data.Dimension(i).id, function (index, value) {
                var option = $('<option>', {
                    value: value,
                    "code-true": data.Dimension(i).Category(index).label + " (" + value + ")",
                    "code-false": data.Dimension(i).Category(index).label,
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
    $("#matrix-selection-placeholder").html(matrixSelection.get(0).outerHTML);
    //select all
    $("#matrix-selection-placeholder").find("input[type='checkbox']").once("change", function () {
        var dimension = $(this).attr("idn");
        var select = $("#matrix-selection-placeholder").find("select[idn='" + dimension + "']");
        if ($(this).is(':checked')) {
            select.find("option:enabled[filtered='true']").prop('selected', true);
        } else {
            select.find('option:enabled').prop('selected', false);
        }
        app.data.dataset.callback.countSelection();
        app.data.dataset.callback.buildApiParams();
    });

    //sort
    $("#matrix-selection-placeholder").find("[name=sort-options]").once("click", function () {
        var dimension = $(this).attr("idn");
        var select = $("#matrix-selection-placeholder").find("select[idn='" + dimension + "']");
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
    $("#matrix-selection-placeholder").find("[type=search]").once("keyup", function () {
        var dimension = $(this).attr("idn");
        var selectAll = $("#matrix-selection-placeholder").find("input[name=select-all][idn='" + dimension + "']");
        var select = $("#matrix-selection-placeholder").find("select[idn='" + dimension + "']");
        var filter = $(this).val().toUpperCase();

        select.find('option:enabled').each(function () {
            if ((this.text.toUpperCase().indexOf(filter) > -1) || $(this).is(':selected')) {
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
    $("#matrix-selection-placeholder").find("[name=dimension-select]").once('change', function () {
        $("button [name=button-show-data-text]").text(app.label.static["view-selection"]);
        app.data.dataset.callback.countSelection();
        app.data.dataset.callback.buildApiParams();
    });

    //reset
    $("#matrix-selection-placeholder").find("[name=reset]").once("click", function () {
        $("#matrix-selection-placeholder").find("[name=dimension-select]").each(function () {
            $(this).find('option').prop('selected', false);
        });
        $("#matrix-selection-placeholder").find("[name=dimension-filter]").each(function () {
            $(this).val("").trigger("keyup");
        });

        $("#matrix-selection-placeholder").find("[name=select-all]").each(function () {
            $(this).prop('checked', false);
        });

        $("#matrix-selection-placeholder").find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.totalCount));
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

            $("#matrix-selection-placeholder").find("[name=dimension-code]").each(function () {
                $(this).removeClass("d-none");
            });

            $("#matrix-selection-placeholder").find("option").each(function () {
                var codeTrue = $(this).attr("code-true");
                $(this).text(codeTrue);
                $(this).attr("title", codeTrue);

            });
        }
        else {
            $("#matrix-selection-placeholder").find("[name=dimension-code]").each(function () {
                $(this).addClass("d-none");
            });

            $("#matrix-selection-placeholder").find("option").each(function () {
                var codeFalse = $(this).attr("code-false");
                $(this).text(codeFalse);
                $(this).attr("title", codeFalse);

            });
        }
    });

    //show data
    $("#matrix-selection-placeholder").find("[name=show-data]").once("click", function () {
        if (app.data.dataset.selectionCount >= app.config.entity.data.threshold.hard) {
            api.modal.information(app.library.html.parseDynamicLabel("you-have-requested-records-which-exceeds-the-limit-of", [app.library.utility.formatNumber(app.data.dataset.selectionCount), app.config.entity.data.threshold.hard]));
        }
        else if (app.data.dataset.selectionCount >= app.config.entity.data.threshold.soft) {
            api.modal.confirm(app.library.html.parseDynamicLabel("confirm", [app.library.utility.formatNumber(app.data.dataset.selectionCount)]), app.data.dataview.ajax.data);
        }
        else {
            //AJAX call get Data Set
            app.data.dataview.ajax.data();
        }
    });


    $("#matrix-selection-placeholder").find("[name=show-data-map]").once("click", function (e) {
        e.preventDefault();
        var status = $(this).attr("status");
        if (status == "data") { //currently data, switch to map
            $("#data-view-container").empty();
            $("#data-view-container").empty();
            $(this).attr("status", "map");
            $(this).find("[name=text]").text(app.label.static.data);
            $("#matrix-selection-placeholder").find("[name=map-container]").removeClass("d-none");
            $("#matrix-selection-placeholder").find("[name=dimension-containers]").addClass("d-none");
            $("#matrix-selection-placeholder").find("[name=card-footer]").removeClass("d-flex").addClass("d-none");
            var apiParams = {
                "language": app.data.LngIsoCode,
                "format": C_APP_TS_FORMAT_JSON_STAT,
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
            $("#matrix-selection-placeholder").find("[name=map-container]").addClass("d-none");
            $("#matrix-selection-placeholder").find("[name=dimension-containers]").removeClass("d-none");
            $("#matrix-selection-placeholder").find("[name=card-footer]").addClass("d-flex").removeClass("d-none");
            app.data.dataset.callback.buildApiParams();
        }
    });
    app.data.dataset.callback.buildApiParams();
    $("#data-dataset-row").show();
    $('[data-toggle="tooltip"]').tooltip();

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

    // Run Sharethis.
    app.data.sharethis(data.extension.matrix);
    $("#panel").find("[name=download-full-dataset-json]").once("click", function (e) {
        e.preventDefault();
        app.data.dataset.callback.fullDownload(C_APP_TS_FORMAT_JSON_STAT);
    });
    $("#panel").find("[name=download-full-dataset-px]").once("click", function (e) {
        e.preventDefault();
        app.data.dataset.callback.fullDownload(C_APP_TS_FORMAT_PX);
    });
    $("#panel").find("[name=download-full-dataset-csv]").once("click", function (e) {
        e.preventDefault();
        app.data.dataset.callback.fullDownload(C_APP_FORMAT_CSV);
    });

};

app.data.dataset.callback.fullDownload = function (format) {
    var apiParams = {
        "matrix": app.data.MtrCode,
        "language": app.data.LngIsoCode,
        "format": format,
        "m2m": false
    };
    app.data.dataset.ajax.downloadDataset(apiParams);
}

/**
* 
*/
app.data.dataset.callback.countSelection = function () {
    var count = 1;
    $("#matrix-selection-placeholder").find("[name=dimension-select]").each(function () {
        var dimension = $(this).attr("idn");
        var totalOptions = $(this).find("option:enabled").length;
        var selectedOptions = $(this).find('option:selected').length;

        //if any option clicked check if select all should be ticked or not
        if (totalOptions == selectedOptions) { // all selected
            $("#matrix-selection-placeholder").find("input[type=checkbox][idn='" + dimension.replace("'", "\\'") + "']").prop('checked', true);
            count = count * totalOptions;
        }
        else if (selectedOptions == 0) {
            count = count * totalOptions;
        }
        else {
            count = count * $(this).find('option:selected').length;
            $("#matrix-selection-placeholder").find("input[type=checkbox][idn='" + dimension.replace("'", "\\'") + "']").prop('checked', false);
        }
    });
    app.data.dataset.selectionCount = count;

    $("#matrix-selection-placeholder").find("[name=data-count-cells]").text(app.library.utility.formatNumber(app.data.dataset.selectionCount));
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
        "format": C_APP_TS_FORMAT_JSON_STAT,
        "role": {
            "time": [
                $("#matrix-selection-placeholder").find("[name=dimension-containers]").find("select[role=time]").attr("idn")
            ],
            "metric": [
                $("#matrix-selection-placeholder").find("[name=dimension-containers]").find("select[role=metric]").attr("idn")
            ]
        },
        "dimension": [],
        "m2m": false
    };

    $("#matrix-selection-placeholder").find("[name=dimension-containers]").find("select").each(function (index) {
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
    jQuery.extend(app.data.dataset.apiParamsData, localParams);
    $("#data-accordion-api").find("[name=github-link]").attr("href", C_APP_URL_GITHUB_API_CUBE);
    $("#data-accordion-api").find("[name=api-url]").text(app.config.url.api.public);
    $("#data-accordion-api").find("[name=api-object]").text(function () {
        var JsonQuery = {
            "jsonrpc": C_APP_API_JSONRPC_VERSION,
            "method": C_APP_API_READ_DATASET_METHOD,
            "params": null
        };
        var apiParams = jQuery.extend({}, app.data.dataset.apiParamsData);
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
        $("#matrix-selection-placeholder, #panel, #data-view-container").empty();
        $("#data-metadata-row, #data-filter, #data-search-results-pagination [name=pagination]").show();
        $("#data-accordion-api").hide();
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
        api.ajax.jsonrpc.request(app.config.url.api.public,
            "PxStat.Data.Cube_API.ReadDataset",
            apiParams,
            "app.data.dataset.callback.downloadDataset",
            apiParams,
            null,
            null,
            { async: false });
    }
    else if (app.data.RlsCode) {
        api.ajax.jsonrpc.request(app.config.url.api.private,
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
* @param {*} response
* @param {*} apiParams
*/
app.data.dataset.callback.downloadDataset = function (response, apiParams) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {
        var data = response.data;
        var mimeType = "";
        var fileData = null;
        var fileExtension = null;
        switch (apiParams.format) {
            case C_APP_TS_FORMAT_JSON_STAT:
                mimeType = "application/json";
                fileData = JSON.stringify(data);
                fileExtension = "." + C_APP_EXTENSION_JSON_STAT;
                break;
            default:
                mimeType = "text/plain";
                fileData = data;
                fileExtension = "." + apiParams.format;
                break;
        }

        var blob = new Blob([fileData], { type: mimeType });
        var downloadUrl = URL.createObjectURL(blob);
        var a = document.createElement("a");
        a.href = downloadUrl;
        a.download = app.data.fileNamePrefix + '.' + moment(Date.now()).format(app.config.mask.datetime.file) + fileExtension;

        if (document.createEvent) {
            // https://developer.mozilla.org/en-US/docs/Web/API/Document/createEvent
            var event = document.createEvent('MouseEvents');
            event.initEvent('click', true, true);
            a.dispatchEvent(event);
        }
        else {
            a.click();
        }
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion

