/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
// Create Namespace
// app.keyword is a parent namespace
app.keyword = app.keyword || {};
app.keyword.search = {};
app.keyword.search = {};
app.keyword.search.ajax = {};
app.keyword.search.callback = {};
//#endregion

//#region Search Synonym

//Ajax call for synonym
app.keyword.search.ajax.searchSynonym = function () {

    var KsbValue = $("#keyword-search-synonym-request").find("[name=sym-value").val();
    //Check for value
    if (KsbValue != "") {
        api.ajax.jsonrpc.request(
            app.config.url.api.private,
            "PxStat.System.Navigation.Keyword_API.ReadSynonym",
            { "KrlValue": KsbValue },
            "app.keyword.search.callback.searchSynonym"
        );
    }
}

//Call back for synonyms
app.keyword.search.callback.searchSynonym = function (response) {
    if (response.error) {
        api.modal.error(response.error.message);
    } else if (response.data !== undefined) {


        //Clear list card
        $("#keyword-search-synonym-request").find("[name=synonym-card]").empty();

        //Get each language
        $.each(response.data, function (key, language) {
            //Clone card
            var languageCard = $("#keyword-search-template").find("[name=synonym-language-card]").clone();
            //Display the Language
            languageCard.find(".card-header").text(language.LngIsoName);

            //Check if any synonyms
            if (language.Synonym.length) {

                //Get Synonyms
                $.each(language.Synonym, function (key, synonym) {


                    var synonymItem = $("<li>", {
                        "class": "list-group-item",
                        "html": synonym
                    });
                    languageCard.find("[name=synonym-group]").append(synonymItem);

                });

            }
            else {
                //Display if no synonyms
                var synonymItem = $("<li>", {
                    "class": "list-group-item",
                    "html": app.label.static["no-synonyms"]
                });
                languageCard.find("[name=synonym-group]").append(synonymItem);
            }

            $("#keyword-search-synonym-request").find("[name=synonym-card]").append(languageCard).get(0).outerHTML;
            $("#keyword-search-synonym-request").find("[name=synonym-card]").show();

        });

    } else api.modal.exception(app.label.static["api-ajax-exception"]);
}


  //#endregion
