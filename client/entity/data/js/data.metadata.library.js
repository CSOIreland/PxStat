/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces
app.data = app.data || {};

app.data.metadata = {};
app.data.metadata.ajax = {};
app.data.metadata.callback = {};
app.data.metadata.searchResults = null;

//#endregion

/**
* 
* @param {*} apiParams
*/
app.data.metadata.ajax.readNavigationResults = function (apiParams) {
    api.ajax.jsonrpc.request(app.config.url.api.public,
        "PxStat.System.Navigation.Navigation_API.Search",
        apiParams,
        "app.data.metadata.callback.readResults",
        {
            "PrcCode": apiParams.PrcCode
        }
    );
};

/**
* @param {*} Search
*/
app.data.metadata.ajax.readSearch = function (Search) {
    //The "Search" value is not sending null value. The value is string or empty string.
    Search = Search || $("#data-search-input").find("[name=search-input]").val();
    if (Search) {
        // Populate the search input to make the search noticeable
        $("#data-search-input").find("[name=search-input]").val(Search);
    }
    api.ajax.jsonrpc.request(app.config.url.api.public,
        "PxStat.System.Navigation.Navigation_API.Search",
        {
            "Search": Search,
            "LngIsoCode": app.label.language.iso.code,
        },
        "app.data.metadata.callback.readResults",
        null,
        null,
        null,
        { async: false }
    );
};

/**
* 
* @param {*} response
*/
app.data.metadata.callback.readResults = function (response, params) {
    //always reset breadcrumb
    app.navigation.breadcrumb.set([]);
    params = params || {};
    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
        $("#data-filter, #data-metadata-row [name=search-results]").empty();
        $("#data-search-input").find("[name=no-search-results]").show();
        $("#data-search-results-pagination [name=pagination]").hide();
        $("#data-navigation").find(".navbar-collapse").collapse('show');
    }
    else if (response.data) {
        $("#data-filter, #data-metadata-row [name=search-results]").empty();
        $("#data-search-input").find("[name=no-search-results]").hide();
        $("#data-dataview-selected-table, #panel, #data-view-container").empty();
        $("#data-accordion-api").hide();
        $("#data-search-results-pagination").show();
        if (params.PrcCode) {
            //update breadcrumb
            if (app.data.MtrCode) {
                app.navigation.breadcrumb.set([response.data[0].SbjValue, {
                    "text": response.data[0].PrcValue,
                    "goTo": {
                        "pRelativeURL": "entity/data/",
                        "pNav_link_SelectorToHighlight": "#nav-link-data",
                        "pParams": {
                            "PrcCode": response.data[0].PrcCode
                        }
                    }
                }]);
            };
        };

        if (params.CprCode) {
            //update breadcrumb with copyright value
            if (app.data.MtrCode) {
                app.navigation.breadcrumb.set([{
                    "text": response.data[0].CprValue,
                    "goTo": {
                        "pRelativeURL": "entity/data/",
                        "pNav_link_SelectorToHighlight": "#nav-link-data",
                        "pParams": {
                            "CprCode": params.CprCode
                        }
                    }
                }]);
            };
        }

        // Implement GoTo
        if (params.MtrCode && response.data.length == 1) {
            //collapse latest releases
            $("#data-latest-releases").remove();
            $("#data-accordion-collection-api").hide();
            //collapse navigation
            $("#data-navigation").find(".navbar-collapse").collapse("hide");

            app.data.init(response.data[0].LngIsoCode, response.data[0].MtrCode, null, response.data[0].MtrCode);
            app.data.dataset.ajax.readMetadata();

            $("#data-metadata-row, #data-filter, #data-search-results-pagination [name=pagination]").hide();
            $("#data-dataset-row").find("[name=back-to-select-results]").show();
            $("#data-navigation").find(".navbar-collapse").collapse('hide');
            return;
        }
        $("#data-filter").empty();
        app.data.metadata.searchResults = response.data;
        var searchResultsSort = $("#data-metadata-templates").find("[name=search-results-sort]").clone();
        searchResultsSort.find("[name=search-results-sort-select]").empty().append($('<option>', {
            selected: "selected",
            value: "relevance",
            text: app.label.static["relevance"]
        })).append($('<option>', {
            value: "newest",
            text: app.label.static["newest-first"]
        })).append($('<option>', {
            value: "oldest",
            text: app.label.static["oldest-first"]
        }));
        $("#data-filter").append(searchResultsSort);

        var subjects = [];
        var products = [];
        var copyrights = [];
        var languages = [];

        $.each(app.data.metadata.searchResults, function (key, value) {
            subjects.push({
                SbjCode: value.SbjCode,
                SbjValue: value.SbjValue
            });

            products.push({
                PrcCode: value.PrcCode,
                PrcValue: value.PrcValue
            });

            copyrights.push({
                CprCode: value.CprCode,
                CprValue: value.CprValue
            });
            languages.push({
                LngIsoCode: value.LngIsoCode,
                LngIsoName: value.LngIsoName
            });
        });

        //build subjects filter list. Loop through all subjects and filter
        var subjectsFiltered = [];
        var productsFiltered = [];
        var copyrightsFiltered = [];
        var languagesFiltered = [];

        //group subjects
        var mapSubjects = new Map();
        var item;
        for (item of subjects) {
            if (!mapSubjects.has(item.SbjCode)) {
                mapSubjects.set(item.SbjCode, true);    // set any value to Map
                subjectsFiltered.push({
                    SbjCode: item.SbjCode,
                    SbjValue: item.SbjValue,
                    SbjCount: subjects.reduce(function (r, a) {
                        return r + (a.SbjCode === item.SbjCode);
                    }, 0)
                });
            }
        }
        //group products
        var mapProducts = new Map();
        for (item of products) {
            if (!mapProducts.has(item.PrcCode)) {
                mapProducts.set(item.PrcCode, true);    // set any value to Map
                productsFiltered.push({
                    PrcCode: item.PrcCode,
                    PrcValue: item.PrcValue,
                    PrcCount: products.reduce(function (r, a) {
                        return r + (a.PrcCode === item.PrcCode);
                    }, 0)
                });
            }
        }

        //group copyrights
        var mapCopyrights = new Map();
        for (item of copyrights) {
            if (!mapCopyrights.has(item.CprCode)) {
                mapCopyrights.set(item.CprCode, true);    // set any value to Map
                copyrightsFiltered.push({
                    CprCode: item.CprCode,
                    CprValue: item.CprValue,
                    CprCount: copyrights.reduce(function (r, a) {
                        return r + (a.CprCode === item.CprCode);
                    }, 0)
                });
            }
        }

        //group languages
        var mapLanguages = new Map();
        for (item of languages) {
            if (!mapLanguages.has(item.LngIsoCode)) {
                mapLanguages.set(item.LngIsoCode, true);    // set any value to Map
                languagesFiltered.push({
                    LngIsoCode: item.LngIsoCode,
                    LngIsoName: item.LngIsoName,
                    LngIsoCount: languages.reduce(function (r, a) {
                        return r + (a.LngIsoCode === item.LngIsoCode);
                    }, 0)
                });
            }
        }

        //Subject Filter List
        var subjectFilterList = $("#data-metadata-templates").find("[name=filter-list]").clone();
        subjectFilterList.attr("filter-type", "subject");
        var subjectFilterHeading = $("#data-metadata-templates").find("[name=filter-list-heading]").clone();
        subjectFilterList.append(subjectFilterHeading.text(app.label.static["subjects"]));

        $.each(subjectsFiltered, function (key, value) {
            var item = $("#data-metadata-templates").find("[name=filter-list-item]").clone();
            item.attr("sbj-code", value.SbjCode);
            item.attr("active", "false");
            item.find("[name=filter-list-item-value]").text(value.SbjValue);
            item.find("[name=filter-list-item-count]").text(value.SbjCount);
            subjectFilterList.append(item);
        });

        //Products Filter List
        var productsFilterList = $("#data-metadata-templates").find("[name=filter-list]").clone();
        productsFilterList.attr("filter-type", "product");
        var productsFilterHeading = $("#data-metadata-templates").find("[name=filter-list-heading]").clone();
        productsFilterList.append(productsFilterHeading.text(app.label.static["products"]));

        $.each(productsFiltered, function (key, value) {
            var item = $("#data-metadata-templates").find("[name=filter-list-item]").clone();
            item.attr("prc-code", value.PrcCode);
            item.attr("active", "false");
            item.find("[name=filter-list-item-value]").text(value.PrcValue);
            item.find("[name=filter-list-item-count]").text(value.PrcCount);
            productsFilterList.append(item);
        });

        //Copyrights Filter List
        var copyrightsFilterList = $("#data-metadata-templates").find("[name=filter-list]").clone();
        copyrightsFilterList.attr("filter-type", "copyright");
        var copyrightsFilterHeading = $("#data-metadata-templates").find("[name=filter-list-heading]").clone();
        copyrightsFilterList.append(copyrightsFilterHeading.text(app.label.static["copyrights"]));

        $.each(copyrightsFiltered, function (key, value) {
            var item = $("#data-metadata-templates").find("[name=filter-list-item]").clone();
            item.attr("cpr-code", value.CprCode);
            item.attr("active", "false");
            item.find("[name=filter-list-item-value]").text(value.CprValue);
            item.find("[name=filter-list-item-count]").text(value.CprCount);
            copyrightsFilterList.append(item);
        });

        //Languages Filter List
        var languagesFilterList = $("#data-metadata-templates").find("[name=filter-list]").clone();
        languagesFilterList.attr("filter-type", "language");
        var languagesFilterHeading = $("#data-metadata-templates").find("[name=filter-list-heading]").clone();
        languagesFilterList.append(languagesFilterHeading.text(app.label.static["languages"]));
        $.each(languagesFiltered, function (key, value) {
            var item = $("#data-metadata-templates").find("[name=filter-list-item]").clone();
            item.attr("lng-iso-code", value.LngIsoCode);
            item.attr("active", "false");
            item.find("[name=filter-list-item-value]").text(value.LngIsoName);
            item.find("[name=filter-list-item-count]").text(value.LngIsoCount);
            languagesFilterList.append(item);
        });

        //collapse navigation HTML
        $("#data-navigation").find(".navbar-collapse").collapse('hide');
        $("#data-filter").append(languagesFilterList.get(0).outerHTML + subjectFilterList.get(0).outerHTML + productsFilterList.get(0).outerHTML + copyrightsFilterList.get(0).outerHTML);
        //Prevent clicking on filter headings
        $("#data-filter").find("[name=filter-list-heading]").once("click", function (e) { e.preventDefault(); });
        //Filter Results Click
        $("#data-filter").find("[name=filter-list-item]").once("click", function (e) {
            e.preventDefault();
            if ($(this).attr("active") == "false") {
                $(this).find("[name=filter-list-item-checkbox]").removeClass("far fa-square").addClass("far fa-check-square");
                $(this).attr("active", "true");
            }
            else {
                $(this).find("[name=filter-list-item-checkbox]").removeClass("far fa-check-square").addClass("far fa-square");
                $(this).attr("active", "false");
            }
            app.data.metadata.callback.filterResults();
        });
        //Search results sort select Change event
        $("#data-filter").find("[name=search-results-sort-select]").once("change", function (e) {
            app.data.metadata.callback.filterResults();
        });
        app.data.metadata.callback.filterResults();
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);

    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
};


/**
* 
*/
app.data.metadata.callback.filterResults = function () {
    //arrays of codes to show
    var subjectsSelected = [];
    var productsSelected = [];
    var copyrightsSelected = [];
    var languagesSelected = [];
    //subjectsSelected
    $("#data-filter").find("[filter-type=subject]").find("[name=filter-list-item]").each(function (index) {
        if ($(this).attr("active") == "true") {
            //FIXME: Server response return int. The jQuery.inArray is strict compare, see code below.
            subjectsSelected.push(parseInt($(this).attr("sbj-code")));
        }
    });
    //productsSelected
    $("#data-filter").find("[filter-type=product]").find("[name=filter-list-item]").each(function (index) {
        if ($(this).attr("active") == "true") {
            //The "prc-code" is no longer int 
            //var prcCodeInt = parseInt($(this).attr("prc-code"));
            productsSelected.push($(this).attr("prc-code"));
        }
    });
    //copyrightsSelected
    $("#data-filter").find("[filter-type=copyright]").find("[name=filter-list-item]").each(function (index) {
        if ($(this).attr("active") == "true") {
            copyrightsSelected.push($(this).attr("cpr-code"));
        }
    });
    //languagesSelected
    $("#data-filter").find("[filter-type=language]").find("[name=filter-list-item]").each(function (index) {
        if ($(this).attr("active") == "true") {
            languagesSelected.push($(this).attr("lng-iso-code"));
        }
    });
    var filteredResults = [];
    if (subjectsSelected.length || productsSelected.length || copyrightsSelected.length || languagesSelected.length) {
        $.each(app.data.metadata.searchResults, function (key, value) {
            var count = 0;
            if (jQuery.inArray(value.SbjCode, subjectsSelected) != -1) {
                count++;
            }
            //if (productsSelected.includes(value.PrcCode)) {
            if (jQuery.inArray(value.PrcCode, productsSelected) != -1) {
                count++;
            }
            //if (copyrightsSelected.includes(value.CprCode)) {
            if (jQuery.inArray(value.CprCode, copyrightsSelected) != -1) {

                count++;
            }
            //if (languagesSelected.includes(value.LngIsoCode)) {
            if (jQuery.inArray(value.LngIsoCode, languagesSelected) != -1) {
                count++;
            }
            if (count > 0) {
                filteredResults.push(value);
            }
        });
        app.data.metadata.callback.sortResults(filteredResults);
    }
    else {
        app.data.metadata.callback.sortResults(app.data.metadata.searchResults);
    }
};

/**
* 
* @param {*} filteredResults
*/
app.data.metadata.callback.sortResults = function (filteredResults) {
    switch ($("#data-filter").find("[name=search-results-sort-select]").val()) {
        case "relevance": //make constant
            function sortByRelevance(array, key) {
                return array.sort(function (a, b) {
                    var x = a[key]; var y = b[key];
                    return ((x > y) ? -1 : ((x < y) ? 1 : 0));
                });
            }
            app.data.metadata.callback.drawPagination(sortByRelevance(filteredResults, "Score"));
            break;
        case "newest": //make constant

            function orderByNewest(arr, dateProp) {
                return arr.slice().sort(function (a, b) {
                    return a[dateProp] > b[dateProp] ? -1 : 1;
                });
            }
            app.data.metadata.callback.drawPagination(orderByNewest(filteredResults, "RlsLiveDatetimeFrom"));
            break;
        case "oldest": //make constant
            function orderByOldest(arr, dateProp) {
                return arr.slice().sort(function (a, b) {
                    return a[dateProp] < b[dateProp] ? -1 : 1;
                });
            }
            app.data.metadata.callback.drawPagination(orderByOldest(filteredResults, "RlsLiveDatetimeFrom"));
            break;
        default:
            app.data.metadata.callback.drawPagination(filteredResults);
            break;
    }
};

/**
* 
* @param {*} filteredResults
*/
app.data.metadata.callback.drawPagination = function (filteredResults) {
    //remove latest releases table
    $("#data-latest-releases").remove();
    $("#data-accordion-collection-api").hide();
    $('#data-search-results-pagination').find("[name=pagination]").twbsPagination('destroy');
    var numPages = Math.ceil(filteredResults.length / app.config.entity.data.pagination);
    $('#data-search-results-pagination').find("[name=pagination]").twbsPagination({
        totalPages: numPages,
        onPageClick: function (event, page) {
            var positionTostart = (page - 1) * app.config.entity.data.pagination;
            app.data.metadata.callback.drawResults(filteredResults.slice(positionTostart, positionTostart + app.config.entity.data.pagination));
        }
    });
};

/**
* 
* @param {*} paginatedResults
*/
app.data.metadata.callback.drawResults = function (paginatedResults) {
    $("#data-dataset-row").find("[name=back-to-select-results]").hide();
    $("#data-metadata-row, #data-filter, #data-search-results-pagination [name=pagination]").show();
    $("#data-metadata-row").find("[name=search-results]").empty();

    $.each(paginatedResults, function (key, entry) {
        var resultItem = $("#data-metadata-templates").find("[name=search-result-item]").clone(); //a Result Item
        resultItem.attr("mtr-code", entry.MtrCode).attr("lng-iso-code", entry.LngIsoCode);
        resultItem.find("[name=mtr-title]").text(entry.MtrTitle);

        //release date & time   
        resultItem.find("[name=from-date]").text(entry.RlsLiveDatetimeFrom ? moment(entry.RlsLiveDatetimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.date.display) : "");
        resultItem.find("[name=from-time]").text(entry.RlsLiveDatetimeFrom ? moment(entry.RlsLiveDatetimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.time.display) : "");
        if (entry.RlsEmergencyFlag) {
            resultItem.find("[name=emergency-flag]").removeClass("d-none");
        }

        //Matrix details
        resultItem.find("[name=mtr-code]").text(entry.MtrCode);

        if (!entry.MtrOfficialFlag) {
            resultItem.find("[name=official-flag]").removeClass("d-none");
        }

        //Geo flag
        $.each(entry.classification, function (keyGeo, entryGeo) {
            if (entryGeo.ClsGeoFlag) {
                resultItem.find("[name=geo-flag]").removeClass("d-none");
            }
        });

        //archive flag
        if (entry.RlsArchiveFlag) {
            resultItem.find("[name=archive-flag]").removeClass("d-none");
        }

        //reservation flag
        if (entry.RlsReservationFlag) {
            resultItem.find("[name=reservation-flag], [name=under-reservation-header]").removeClass("d-none");
        }

        //analytical flag
        if (entry.RlsAnalyticalFlag) {
            resultItem.find("[name=analytical-flag]").removeClass("d-none");
        }

        //dependency flag
        if (entry.RlsDependencyFlag) {
            resultItem.find("[name=dependency-flag]").removeClass("d-none");
        }

        //language
        resultItem.find("[name=language]").text(entry.LngIsoName);

        //dimension pill
        $.each(entry.classification, function (keyClassification, entryClassification) {
            var dimension = $("#data-metadata-templates").find("[name=dimension]").clone();
            dimension.text(entryClassification.ClsValue);
            resultItem.find("[name=dimensions]").append(dimension);
        });

        //frequency pill
        var frequency = $("#data-metadata-templates").find("[name=frequency]").clone();
        frequency.text(entry.FrqValue);

        resultItem.find("[name=dimensions]").append(frequency);

        //frequency span
        var frequencySpan = $("#data-metadata-templates").find("[name=frequency-span]").clone();
        frequencySpan.text(
            function () {
                var periodsArray = entry.period;
                periodsArray.sort();
                var prdValueMin = periodsArray[0];
                var prdValueMax = periodsArray[periodsArray.length - 1];
                return "[" + prdValueMin + "-" + prdValueMax + "]";
            });
        resultItem.find("[name=dimensions]").append(frequencySpan);

        // copyright
        resultItem.find("[name=copyright]").text(entry.CprValue);
        $("#data-metadata-row").find("[name=search-results]").append(resultItem);
    });
    $("#data-metadata-row").find("[name=search-result-item]").once("click", function (e) {
        e.preventDefault();
        app.data.init($(this).attr("lng-iso-code"), $(this).attr("mtr-code"), null, $(this).attr("mtr-code"));
        $("#data-metadata-row, #data-filter, #data-search-results-pagination [name=pagination]").hide();
        $("#data-dataset-row").find("[name=back-to-select-results]").show();
        $("#data-navigation").find(".navbar-collapse").collapse('hide');
        app.data.dataset.ajax.readMetadata();
    });
    $('[data-toggle="tooltip"]').tooltip();

};