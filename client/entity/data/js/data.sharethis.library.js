/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces
app.data = app.data || {};
//#endregion

/**
 * Run the ShareThis module
 */
app.data.sharethis = function (RlsCode) {

    // Check if the module is enabled
    if (!app.config.plugin.sharethis.enabled)
        return;


    var shareThis = $("#data-sharethis-row").find("[name=sharethis-template]").clone();
    shareThis.attr("id", "data-sharethis");
    shareThis.find("[name=link]").attr("id", "data-sharethis-table-link");
    shareThis.find("[name=copy-link-info]").attr("data-clipboard-target", "#data-sharethis-table-link");


    // Set the URL to the Table
    var tableURL = app.config.url.application + C_COOKIE_LINK_TABLE + "/" + RlsCode;

    //Link url for display only
    shareThis.find("[name=link]").val(tableURL);

    //Link for sharing
    shareThis.find("[name=button]").attr("data-url", tableURL);


    $("#panel").find("[name=matrix-notes]").append(shareThis);





    //Call to reinitialize the sharethis buttons every time the page is loaded.
    if (window.__sharethis__) {
        window.__sharethis__.initialize();

    };
}

