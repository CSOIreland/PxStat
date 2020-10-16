/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces
app.data = app.data || {};
//#endregion

/**
 * Run the Share module
 */
app.data.share = function (MtrCode) {


    var share = $("#data-share-row").find("[name=share-template]").clone();
    share.attr("id", "data-share");
    share.find("[name=link]").attr("id", "data-share-table-link");
    share.find("[name=copy-link-info]").attr("data-clipboard-target", "#data-share-table-link");

    // Set the URL to the Table
    var tableURL = app.config.url.application + C_COOKIE_LINK_TABLE + "/" + MtrCode;
    //Link url for display only
    share.find("[name=link]").val(tableURL);
    //Link for sharing
    share.find("[name=button]").attr("data-url", tableURL);
    // Append to side panel
    $("#panel").find("[name=matrix-notes]").find("[name=share]").html(share);

    // Check if the module is enabled
    if (window.__sharethis__) {
        $("#panel").find("[name=sharethis-container]").show();
        //Call to reinitialize the sharethis buttons every time the page is loaded.
        window.__sharethis__.initialize();
    };
}

