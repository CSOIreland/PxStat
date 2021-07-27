/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region namespaces
app.data = app.data || {};

app.data.search = {};
app.data.search.ajax = {};
app.data.search.callback = {};

app.data.searchResult = {};
app.data.searchResult.ajax = {};
app.data.searchResult.callback = {};
app.data.searchResult.result = null;
//#endregion

//#region get languages

//#endregion

//#region get navigation

/**
* Get data from API and Search Navigation
* app.data.search
* @param {*} lngIsoCode
*/
app.data.search.ajax.readNav = function (lngIsoCode) {
    //Side Bar Navigation Menu (Subjects/Products)
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
        "PxStat.System.Navigation.Navigation_API.Read",
        { "LngIsoCode": lngIsoCode },
        "app.data.search.callback.readNav",
        null,
        null,
        null,
        { async: false }
    );
};

/**
* 
* @param {*} data
*/
app.data.search.callback.readNav = function (data) {
    $.each(data, function (index1, valueTheme) {
        var theme = $("#data-search-templates").find("[name=navigation-theme]").clone();
        theme.find(".card-header").attr("id", "navigation-theme-heading-" + index1);
        theme.find("[data-toggle=collapse]").attr("data-target", "#navigation-theme-collapse-" + index1).html(
            $("<span>", {
                "style": "white-space: normal",
                "text": valueTheme.ThmValue
            }).get(0).outerHTML
        );
        theme.find(".collapse").attr("id", "navigation-theme-collapse-" + index1).attr("data-parent", "#data-browse-collapse-accordion").attr("aria-labelledby", "navigation-theme-heading-" + index1);
        $("#data-browse-collapse-accordion").append(theme);

        $.each(valueTheme.subject, function (index2, valueSubject) {
            var subjectDropdownLink = $("#data-search-templates").find("[name=subject-dropdown-link]").clone();
            subjectDropdownLink.attr("href", "#navigation-subject-collapse-" + index1 + "-" + index2).attr("aria-controls", "navigation-subject-collapse-" + index1 + "-" + index2);
            subjectDropdownLink.find("[name=subject-name]").text(valueSubject.SbjValue);
            $("#navigation-theme-collapse-" + index1).find("[name=navigation-theme-card-body]").append(subjectDropdownLink);

            var subjectDropdown = $("#data-search-templates").find("[name=subject-dropdown]").clone();
            subjectDropdown.attr("id", "navigation-subject-collapse-" + index1 + "-" + index2);
            $("#navigation-theme-collapse-" + index1).find("[name=navigation-theme-card-body]").append(subjectDropdown);

            $.each(valueSubject.product, function (index3, valueProduct) {
                var product = $("#data-search-templates").find("[name=navigation-product-item]").clone();
                product.attr("href", app.config.url.application + C_COOKIE_LINK_PRODUCT + "/" + valueProduct.PrcCode);
                product.attr("prc-code", valueProduct.PrcCode);
                product.attr("lng-iso-code", app.label.language.iso.code);
                product.find("[name=product-name]").text(valueProduct.PrcValue);
                product.find("[name=product-release-count]").text(valueProduct.PrcReleaseCount);
                $("#navigation-subject-collapse-" + index1 + "-" + index2).find("[name=product-list-card-body]").append(product);
            });
        });
    });

    $('#data-browse-collapse-accordion').on('hidden.bs.collapse', function (e) {
        $("#" + e.target.id).find(".card-body .collapse").collapse('hide')
    });
    //EVENT ASSIGN
    // Add Click even for selecting Subject-Product at Browse Subjects menu.
    $("#data-navigation").find("[name=navigation-product-item]").once("click", function (e) {
        e.preventDefault();
        //clear search box input
        $("#data-search-row-desktop [name=search-input], #data-search-row-responsive [name=search-input]").val('');
        //empty data screens
        $("#data-dataset-selected-table [name=card-header], #panel, #data-view-container").empty();
        $("#data-accordion-api").hide();
        $("#data-latest-releases").remove();
        $("#data-accordion-collection-api").hide();
        var apiParams = {
            "PrcCode": $(this).attr("prc-code"),
            "LngIsoCode": $(this).attr("lng-iso-code")
        };
        app.data.PrdCode = $(this).attr("prc-code");
        app.data.searchResult.ajax.readNavigationResults(apiParams);
        //Collapse navigation
        $("#data-navigation").find(".navbar-collapse").collapse('hide');

        $("#data-dataset-row").hide();

    });
};
//#endregion get navigation

//#region search results
/**
* 
* @param {*} apiParams
*/
app.data.searchResult.ajax.readNavigationResults = function (apiParams) {
    app.data.isSearch = false;
    $("#data-search-result-pagination").hide();
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
        "PxStat.System.Navigation.Navigation_API.Search",
        apiParams,
        "app.data.searchResult.callback.readResults",
        {
            "PrcCode": apiParams.PrcCode
        }
    );
};

/**
* @param {*} Search
*/
app.data.searchResult.ajax.readSearch = function (search) {
    app.data.isSearch = true;
    app.data.PrdCode = null;
    $("#data-search-result-pagination").show();
    $('#data-search-result-pagination-toggle').bootstrapToggle("off");
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
        "PxStat.System.Navigation.Navigation_API.Search",
        {
            "Search": search,
            "LngIsoCode": app.label.language.iso.code,
        },
        "app.data.searchResult.callback.readResults",
        null,
        null,
        null,
        { async: false }
    );

};

/**
* 
* @param {*} data
*/
app.data.searchResult.callback.readResults = function (data, params) {
    //always reset breadcrumb
    app.navigation.setBreadcrumb([]);
    params = params || {};
    $("#data-filter, #data-search-row-desktop [name=search-results-non-archived], #data-search-row-desktop [name=search-results-archived]").empty();
    if (data && Array.isArray(data) && data.length) {
        $("#data-search-row-desktop [name=search-results]").show();
        $("#data-search-row-desktop [name=no-search-results], #data-search-row-responsive [name=no-search-results]").hide();
        $("#data-dataset-selected-table [name=card-header], #panel, #data-view-container").empty();
        $("#data-dataset-row").hide();
        $("#data-accordion-api").hide();
        if (params.PrcCode) {
            //update breadcrumb
            app.navigation.setBreadcrumb([
                [data[0].SbjValue],
                [data[0].PrcValue, "entity/data/", "#nav-link-data", null, { "PrcCode": data[0].PrcCode }, app.config.url.application + C_COOKIE_LINK_PRODUCT + "/" + data[0].PrcCode]
            ]);
            app.navigation.setTitle(data[0].PrcValue);
            app.navigation.setMetaDescription(app.library.html.parseDynamicLabel("meta-description-product", [data[0].PrcCode, data[0].PrcValue]));
        }

        else if (params.CprCode) {
            //update breadcrumb with copyright value
            app.navigation.setBreadcrumb([
                [data[0].CprValue, "entity/data/", "#nav-link-data", null, { "CprCode": params.CprCode }, app.config.url.application + C_COOKIE_LINK_COPYRIGHT + "/" + data[0].CprCode]
            ]);
            app.navigation.setTitle(data[0].CprValue);
            app.navigation.setMetaDescription(app.library.html.parseDynamicLabel("meta-description-copyright", [data[0].CprCode, data[0].CprValue]));

        }

        // Implement GoTo
        else if (params.MtrCode && data.length == 1) {
            //collapse latest releases
            $("#data-latest-releases").remove();
            $("#data-accordion-collection-api").hide();
            //collapse navigation
            $("#data-navigation").find(".navbar-collapse").collapse("hide");

            app.data.init(data[0].LngIsoCode, data[0].MtrCode, null, data[0].MtrCode, false, true);
            app.data.dataset.draw();

            $("#data-search-row-desktop [name=search-results], #data-filter, #data-search-result-pagination [name=pagination], #data-search-result-pagination [name=pagination-toggle]").hide();
            $("#data-dataset-row").find("[name=back-to-select-results]").show();
            return;
        }
        $("#data-filter").empty();
        app.data.searchResult.result = data;
        var searchResultsSort = $("#data-search-result-templates").find("[name=search-results-sort]").clone();
        searchResultsSort.find("[name=search-results-sort-select]").empty();
        if (app.data.isSearch) {
            searchResultsSort.find("[name=search-results-sort-select]").append($('<option>', {
                value: C_APP_SORT_RELEVANCE,
                text: app.label.static["relevance"]
            }))
        }

        searchResultsSort.find("[name=search-results-sort-select]").append($('<option>', {
            value: C_APP_SORT_ALPHABETICAL,
            text: app.label.static["alphabetical"]
        })).append($('<option>', {
            value: C_APP_SORT_NEWEST,
            text: app.label.static["newest-first"]
        })).append($('<option>', {
            value: C_APP_SORT_OLDEST,
            text: app.label.static["oldest-first"]
        }));
        $("#data-search-row-desktop [name=search-results]").find("[name=refine-search]").hide();
        if (!app.data.isSearch) {
            searchResultsSort.find("option[value=" + C_APP_SORT_NEWEST + "]").prop("selected", true);
        }
        else {
            searchResultsSort.find("option[value=" + C_APP_SORT_RELEVANCE + "]").prop("selected", true);
            if (app.data.searchResult.result.length >= app.config.search.maximumResults) {
                //results truncated by server due to broad search term
                $("#data-search-row-desktop [name=search-results]").find("[name=refine-search]").show().html(app.library.html.parseDynamicLabel("refine-search", [app.config.search.maximumResults]));
            }
        }
        var productShare = $("#data-search-result-templates").find("[name=share]").clone();
        $("#data-filter").append(productShare);
        $("#data-filter").append(searchResultsSort);

        var subjects = [];
        var products = [];
        var copyrights = [];
        var languages = [];
        var archived = [];
        var properties = [];

        $.each(app.data.searchResult.result, function (key, value) {
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
            archived.push({
                RlsArchiveFlag: value.RlsArchiveFlag
            });
            properties.push({
                RlsArchiveFlag: value.RlsArchiveFlag,
                RlsExperimentalFlag: value.RlsExperimentalFlag,
                RlsReservationFlag: value.RlsReservationFlag
            })
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
        if (app.data.isSearch || app.data.isCopyrightGoTo) {
            //Subject Filter List
            var subjectFilterList = $("#data-search-result-templates").find("[name=filter-list]").clone();
            subjectFilterList.attr("filter-type", "subject");
            var subjectFilterHeading = $("#data-search-result-templates").find("[name=filter-list-heading]").clone();
            subjectFilterList.append(subjectFilterHeading.text(app.label.static["subjects"]));

            $.each(subjectsFiltered, function (key, value) {
                var item = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
                item.attr("sbj-code", value.SbjCode);
                item.attr("active", "false");
                item.find("[name=filter-list-item-value]").text(value.SbjValue);
                item.find("[name=filter-list-item-count]").text(value.SbjCount);
                subjectFilterList.append(item);
            });

            //Products Filter List
            var productsFilterList = $("#data-search-result-templates").find("[name=filter-list]").clone();
            productsFilterList.attr("filter-type", "product");
            var productsFilterHeading = $("#data-search-result-templates").find("[name=filter-list-heading]").clone();
            productsFilterList.append(productsFilterHeading.text(app.label.static["products"]));

            $.each(productsFiltered, function (key, value) {
                var item = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
                item.attr("prc-code", value.PrcCode);
                item.attr("active", "false");
                item.find("[name=filter-list-item-value]").text(value.PrcValue);
                item.find("[name=filter-list-item-count]").text(value.PrcCount);
                productsFilterList.append(item);
            });
        }



        //Copyrights Filter List
        var copyrightsFilterList = $("#data-search-result-templates").find("[name=filter-list]").clone();
        copyrightsFilterList.attr("filter-type", "copyright");
        var copyrightsFilterHeading = $("#data-search-result-templates").find("[name=filter-list-heading]").clone();
        copyrightsFilterList.append(copyrightsFilterHeading.text(app.label.static["copyrights"]));

        $.each(copyrightsFiltered, function (key, value) {
            var item = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
            item.attr("cpr-code", value.CprCode);
            item.attr("active", "false");
            item.find("[name=filter-list-item-value]").text(value.CprValue);
            item.find("[name=filter-list-item-count]").text(value.CprCount);
            copyrightsFilterList.append(item);
        });

        //if we have some true properties, draw properties filter
        //properties
        var numArchived = 0;
        var numUnderReservation = 0;
        var numExperimental = 0;

        $.each(properties, function (key, value) {
            if (value.RlsArchiveFlag) {
                numArchived++;
            }
            if (value.RlsReservationFlag) {
                numUnderReservation++;
            }
            if (value.RlsExperimentalFlag) {
                numExperimental++;
            }
        });
        var propertiesFilterList = null;
        if (numArchived > 0 || numUnderReservation > 0 || numExperimental > 0) {
            propertiesFilterList = $("#data-search-result-templates").find("[name=filter-list]").clone();
            propertiesFilterList.attr("filter-type", "properties");
            var propertiesFilterHeading = $("#data-search-result-templates").find("[name=filter-list-heading]").clone();
            propertiesFilterList.append(propertiesFilterHeading.text(app.label.static["properties"]));
            if (numArchived > 0) {
                var itemArchived = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
                itemArchived.attr("archived", true);
                itemArchived.attr("active", "false");
                itemArchived.find("[name=filter-list-item-value]").text(app.label.static["archived"]);
                itemArchived.find("[name=filter-list-item-count]").text(numArchived);
                propertiesFilterList.append(itemArchived);
            }
            if (numUnderReservation > 0) {
                var itemUnderReservation = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
                itemUnderReservation.attr("under-reservation", true);
                itemUnderReservation.attr("active", "false");
                itemUnderReservation.find("[name=filter-list-item-value]").text(app.label.static["under-reservation"]);
                itemUnderReservation.find("[name=filter-list-item-count]").text(numUnderReservation);
                propertiesFilterList.append(itemUnderReservation);
            }
            if (numExperimental > 0) {
                var itemExperimental = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
                itemExperimental.attr("experimental", true);
                itemExperimental.attr("active", "false");
                itemExperimental.find("[name=filter-list-item-value]").text(app.label.static["experimental"]);
                itemExperimental.find("[name=filter-list-item-count]").text(numExperimental);
                propertiesFilterList.append(itemExperimental);
            }
        }

        //Languages Filter List
        var languagesFilterList = $("#data-search-result-templates").find("[name=filter-list]").clone();
        languagesFilterList.attr("filter-type", "language");
        var languagesFilterHeading = $("#data-search-result-templates").find("[name=filter-list-heading]").clone();
        languagesFilterList.append(languagesFilterHeading.text(app.label.static["languages"]));
        $.each(languagesFiltered, function (key, value) {
            var item = $("#data-search-result-templates").find("[name=filter-list-item]").clone();
            item.attr("lng-iso-code", value.LngIsoCode);
            item.attr("active", "false");
            item.find("[name=filter-list-item-value]").text(value.LngIsoName);
            item.find("[name=filter-list-item-count]").text(value.LngIsoCount);
            languagesFilterList.append(item);
        });


        //collapse navigation HTML
        $("#data-filter").append(languagesFilterList.get(0).outerHTML);
        if (app.data.isSearch || app.data.isCopyrightGoTo) {
            $("#data-filter").append(subjectFilterList.get(0).outerHTML + productsFilterList.get(0).outerHTML);
        }
        $("#data-filter").append(copyrightsFilterList.get(0).outerHTML);
        if (propertiesFilterList) {
            $("#data-filter").append(propertiesFilterList.get(0).outerHTML)
        }
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
            app.data.searchResult.callback.filterResults();
        });
        //Search results sort select Change event
        $("#data-filter").find("[name=search-results-sort-select]").once("change", function (e) {
            app.data.searchResult.callback.filterResults();
        });
        app.data.searchResult.callback.filterResults();
    }
    // Handle no data
    else {
        $("#data-search-row-desktop [name=no-search-results], #data-search-row-responsive [name=no-search-results]").show();
        $("#data-search-result-pagination [name=pagination], #data-search-result-pagination [name=pagination-toggle]").hide();
        $("#data-navigation").find(".navbar-collapse").collapse('show');
    }

    //run bootstrap toggle to show/hide toggle button
    bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
    if (!app.data.isSearch) {
        app.data.share(null, app.data.PrdCode);
    }
    else {
        $("#data-filter").find("[name=share]").hide()
    }

};


/**
* 
*/
app.data.searchResult.callback.filterResults = function () {
    //arrays of codes to show
    var subjectsSelected = [];
    var productsSelected = [];
    var copyrightsSelected = [];
    var languagesSelected = [];
    var archivedSelected = [];
    var underReservationSelected = [];
    var experimentalSelected = [];
    //subjectsSelected
    $("#data-filter").find("[filter-type=subject]").find("[name=filter-list-item]").each(function (index) {
        if ($(this).attr("active") == "true") {
            // Server return int. The jQuery.inArray is strict compare, see code below.
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

    //archivedSelected
    $("#data-filter").find("[filter-type=properties]").find("[name=filter-list-item]").each(function (index) {
        if ($(this).attr("active") == "true" && $(this).attr("archived")) {
            switch ($(this).attr("archived")) {
                case "true":
                    archivedSelected.push(true);
                    break;
                case "false":
                    archivedSelected.push(false);
                    break;
                default:
                    break;
            }
        }

        if ($(this).attr("active") == "true" && $(this).attr("under-reservation")) {
            switch ($(this).attr("under-reservation")) {
                case "true":
                    underReservationSelected.push(true);
                    break;
                case "false":
                    underReservationSelected.push(false);
                    break;
                default:
                    break;
            }
        }

        if ($(this).attr("active") == "true" && $(this).attr("experimental")) {
            switch ($(this).attr("experimental")) {
                case "true":
                    experimentalSelected.push(true);
                    break;
                case "false":
                    experimentalSelected.push(false);
                    break;
                default:
                    break;
            }
        }
    });

    var filteredResults = [];
    if (subjectsSelected.length || productsSelected.length || copyrightsSelected.length || languagesSelected.length || archivedSelected.length || underReservationSelected.length || experimentalSelected.length) {
        $.each(app.data.searchResult.result, function (key, value) {
            var count = 0;
            if (jQuery.inArray(value.SbjCode, subjectsSelected) != -1) {
                count++;
            }

            if (jQuery.inArray(value.PrcCode, productsSelected) != -1) {
                count++;
            }

            if (jQuery.inArray(value.CprCode, copyrightsSelected) != -1) {

                count++;
            }

            if (jQuery.inArray(value.LngIsoCode, languagesSelected) != -1) {
                count++;
            }

            if (jQuery.inArray(value.RlsArchiveFlag, archivedSelected) != -1) {
                count++;
            }

            if (jQuery.inArray(value.RlsReservationFlag, underReservationSelected) != -1) {
                count++;
            }

            if (jQuery.inArray(value.RlsExperimentalFlag, experimentalSelected) != -1) {
                count++;
            }
            if (count > 0) {
                filteredResults.push(value);
            }
        });
        app.data.searchResult.callback.sortResults(filteredResults);
    }
    else {
        app.data.searchResult.callback.sortResults(app.data.searchResult.result);
    }
};

/**
* 
* @param {*} filteredResults
*/
app.data.searchResult.callback.sortResults = function (filteredResults) {
    switch ($("#data-filter").find("[name=search-results-sort-select]").val()) {
        case C_APP_SORT_RELEVANCE:
            function sortByRelevance(array, key) {
                return array.sort(function (a, b) {
                    var x = a[key]; var y = b[key];
                    return ((x > y) ? -1 : ((x < y) ? 1 : 0));
                });
            }
            app.data.searchResult.callback.drawPagination(sortByRelevance(filteredResults, "Score"));
            break;
        case C_APP_SORT_ALPHABETICAL:
            function sortByAlphabetical(array, key) {
                return array.sort(function (a, b) {
                    var nameA = a[key].toUpperCase(); // ignore upper and lowercase
                    var nameB = b[key].toUpperCase(); // ignore upper and lowercase
                    if (nameA < nameB) {
                        return -1;
                    }
                    if (nameA > nameB) {
                        return 1;
                    }

                    // names must be equal
                    return 0;
                });
            }
            app.data.searchResult.callback.drawPagination(sortByAlphabetical(filteredResults, "MtrCode"));
            break;
        case C_APP_SORT_NEWEST: //make constant

            function orderByNewest(arr, dateProp) {
                return arr.slice().sort(function (a, b) {
                    return a[dateProp] > b[dateProp] ? -1 : 1;
                });
            }
            app.data.searchResult.callback.drawPagination(orderByNewest(filteredResults, "RlsLiveDatetimeFrom"));
            break;
        case C_APP_SORT_OLDEST:
            function orderByOldest(arr, dateProp) {
                return arr.slice().sort(function (a, b) {
                    return a[dateProp] < b[dateProp] ? -1 : 1;
                });
            }
            app.data.searchResult.callback.drawPagination(orderByOldest(filteredResults, "RlsLiveDatetimeFrom"));
            break;
        default:
            app.data.searchResult.callback.drawPagination(filteredResults);
            break;
    }
};

/**
* 
* @param {*} filteredResults
*/
app.data.searchResult.callback.drawPagination = function (filteredResults) {
    //remove latest releases table
    $("#data-latest-releases").remove();
    $("#data-accordion-collection-api").hide();
    $('#data-search-result-pagination').find("[name=pagination]").twbsPagination('destroy');
    var paginationNumber = $('#data-search-result-pagination-toggle').is(':checked') ? filteredResults.length : app.config.entity.data.pagination;
    var numPages = app.data.isSearch ? Math.ceil(filteredResults.length / paginationNumber) : 1;
    if (filteredResults.length <= app.config.entity.data.pagination && app.data.isSearch) {
        $("#data-search-result-pagination").hide();
    }
    else if (filteredResults.length > app.config.entity.data.pagination && app.data.isSearch) {
        $("#data-search-result-pagination").show();
    }
    $('#data-search-result-pagination').find("[name=pagination]").twbsPagination({
        totalPages: numPages,
        onPageClick: function (event, page) {
            var positionTostart = (page - 1) * paginationNumber;
            app.data.searchResult.callback.drawResults(app.data.isSearch ? filteredResults.slice(positionTostart, positionTostart + paginationNumber) : filteredResults);
        }
    });
};

/**
* 
* @param {*} paginatedResults
*/
app.data.searchResult.callback.drawResults = function (paginatedResults) {
    $("#data-dataset-row").find("[name=back-to-select-results]").hide();
    $("#data-search-row-desktop [name=archived-heading], #data-search-row-desktop [name=non-archived-heading]").hide();
    $("#data-search-row-desktop [name=search-results], #data-filter, #data-search-result-pagination [name=pagination], #data-search-result-pagination [name=pagination-toggle]").show();
    $("#data-search-row-desktop [name=search-results-non-archived], #data-search-row-desktop [name=search-results-archived]").empty();

    $.each(paginatedResults, function (key, entry) {
        var resultItem = $("#data-search-result-templates").find("[name=search-result-item]").clone(); //a Result Item
        resultItem.attr("mtr-code", entry.MtrCode).attr("lng-iso-code", entry.LngIsoCode).attr("href", app.config.url.application + C_COOKIE_LINK_TABLE + "/" + entry.MtrCode);
        resultItem.find("[name=mtr-title]").text(entry.MtrTitle);

        //release date & time   
        resultItem.find("[name=from-date]").text(entry.RlsLiveDatetimeFrom ? moment(entry.RlsLiveDatetimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.date.display) : "");
        resultItem.find("[name=from-time]").text(entry.RlsLiveDatetimeFrom ? moment(entry.RlsLiveDatetimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.time.display) : "");
        if (entry.RlsExceptionalFlag) {
            resultItem.find("[name=exceptional-flag]").removeClass("d-none");
        }

        //Matrix details
        resultItem.find("[name=mtr-code]").text(entry.MtrCode);

        if (entry.MtrOfficialFlag) {
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

        //Experimental flag
        if (entry.RlsExperimentalFlag) {
            resultItem.find("[name=experimental-flag]").removeClass("d-none");
        }

        //reservation flag
        if (entry.RlsReservationFlag) {
            resultItem.find("[name=reservation-flag], [name=under-reservation-header]").removeClass("d-none");
        }

        //analytical flag
        if (entry.RlsAnalyticalFlag) {
            resultItem.find("[name=analytical-flag]").removeClass("d-none");
        }

        //language
        resultItem.find("[name=language]").text(entry.LngIsoName);

        //dimension pill
        $.each(entry.classification, function (keyClassification, entryClassification) {
            var dimension = $("#data-search-result-templates").find("[name=dimension]").clone();
            dimension.text(entryClassification.ClsValue);
            resultItem.find("[name=dimensions]").append(dimension);
        });

        //frequency pill
        var frequency = $("#data-search-result-templates").find("[name=frequency]").clone();
        frequency.text(entry.FrqValue);

        resultItem.find("[name=dimensions]").append(frequency);

        //frequency span
        var frequencySpan = $("#data-search-result-templates").find("[name=frequency-span]").clone();
        frequencySpan.text(
            function () {
                var periodsArray = entry.period;
                if (periodsArray.length > 1) {
                    var prdValueMin = periodsArray[0];
                    var prdValueMax = periodsArray[periodsArray.length - 1];
                    return prdValueMin + " - " + prdValueMax;
                }
                else {
                    return periodsArray[0]
                }

            });
        resultItem.find("[name=dimensions]").append(frequencySpan);

        // copyright
        resultItem.find("[name=copyright]").text(entry.CprValue);
        if (app.data.isSearch) {
            $("#data-search-row-desktop").find("[name=search-results-non-archived]").append(resultItem);
        }
        else {
            if (entry.RlsArchiveFlag) {
                $("#data-search-row-desktop").find("[name=search-results-archived]").append(resultItem);
                $("#data-search-row-desktop [name=archived-heading]").show();
            }
            else {
                $("#data-search-row-desktop").find("[name=search-results-non-archived]").append(resultItem);
                $("#data-search-row-desktop [name=non-archived-heading]").show();
            }
        }



    });
    $("#data-search-row-desktop").find("[name=search-result-item]").once("click", function (e) {
        e.preventDefault();
        app.data.init($(this).attr("lng-iso-code"), $(this).attr("mtr-code"), null, $(this).attr("mtr-code"), false, true);
        $("#data-search-row-desktop [name=search-results], #data-filter, #data-search-result-pagination [name=pagination], #data-search-result-pagination [name=pagination-toggle]").hide();
        $("#data-dataset-row").find("[name=back-to-select-results]").show();
        $("#data-navigation").find(".navbar-collapse").collapse('hide');
        app.data.dataset.draw();
    });
    $('[data-toggle="tooltip"]').tooltip();

    //empty any previous datasets that were shown earlier
    $("#data-dataset-table-nav-content, #data-dataset-chart-nav-content, #data-dataset-map-nav-content").empty();
    $("#data-dataset-chart-nav-content, #data-dataset-map-nav-content").removeClass("show").removeClass("active");
    $("#data-dataset-table-nav-content").addClass("show").addClass("active");
    $("#data-dataset-chart-nav-tab, #data-dataset-map-nav-tab").removeClass("active");
    $("#data-dataset-table-nav-tab").addClass("active");

    $('#data-search-result-pagination-toggle').bootstrapToggle().once("change", app.data.searchResult.callback.filterResults);

};
//#endregion search results