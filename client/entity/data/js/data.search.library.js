/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region namespaces
app.data = app.data || {};

app.data.search = {};
app.data.search.ajax = {};
app.data.search.callback = {};
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
    api.ajax.jsonrpc.request(app.config.url.api.public,
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
* @param {*} response
*/
app.data.search.callback.readNav = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    }
    else if (response.data !== undefined) {
        $("#data-navigation").find("[name=search-results-items]").empty();
        $.each(response.data, function (key, sbjValue) {
            var subject = $("#data-search-templates").find("[name=navigation-subject-list]").clone();
            subject.find("[name=navigation-subject-link]").html($("<span>", {
                html: $("<i>", {
                    class: "far fa-folder mr-2"
                }).get(0).outerHTML + sbjValue.SbjValue
            })
            );
            $.each(sbjValue.product, function (key, prcValue) {
                var product = $("#data-search-templates").find("[name=navigation-product-item]").clone();
                product.attr("prc-code", prcValue.PrcCode);
                product.attr("lng-iso-code", app.label.language.iso.code);
                product.find("[name=product-name]").text(prcValue.PrcValue);
                product.find("[name=product-release-count]").text(prcValue.PrcReleaseCount);
                subject.find("[name=navigation-product-list]").append(product);
            });
            $("#data-navigation").find("[name=search-results-items]").append(subject);
        });
        //EVENT ASSIGN
        // Add Click even for selecting Subject-Product at Browse Subjects menu.
        $("#data-navigation").find("[name=navigation-product-item]").once("click", function (e) {
            e.preventDefault();
            //clear search box input
            $("#data-search-input").find("[name=search-input]").val('');
            //empty data screens
            $("#matrix-selection-placeholder, #panel, #data-view-container").empty();
            $("#data-accordion-api").hide();
            $("#data-latest-releases").remove();
            $("#data-accordion-collection-api").hide();
            var apiParams = {
                "PrcCode": $(this).attr("prc-code"),
                "LngIsoCode": $(this).attr("lng-iso-code")
            };
            app.data.metadata.ajax.readNavigationResults(apiParams);
            //Collapse navigation
            $("#data-navigation").find(".navbar-collapse").collapse('hide');
        });
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion get navigation