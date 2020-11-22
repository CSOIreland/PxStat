/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces
app.data = app.data || {};
//#endregion

/**
 * Run the Share module
 */
app.data.share = function (MtrCode, PrdCode) {
    MtrCode = MtrCode || null;
    PrdCode = PrdCode || null;

    var share = $("#data-share-row").find("[name=share-template]").clone();
    share.attr("id", "data-share");
    share.find("[name=link]").attr("id", "data-share-table-link");
    share.find("[name=copy-link-info]").attr("data-clipboard-target", "#data-share-table-link");
    //set share card title
    if (MtrCode) {
        share.find("[name=card-header]").text(app.label.static["share-table"]);
    }
    if (PrdCode) {
        share.find("[name=card-header]").text(app.label.static["share-product"]);
    }

    // Set the URL to the Table
    var tableURL = app.config.url.application;
    if (MtrCode) {
        tableURL += C_COOKIE_LINK_TABLE + "/" + MtrCode;
    }
    if (PrdCode) {
        tableURL += C_COOKIE_LINK_PRODUCT + "/" + PrdCode;
    }

    //Link url for display only
    share.find("[name=link]").val(tableURL);
    //Link for sharing
    share.find("[name=button]").attr("data-url", tableURL);
    // Append to side panel
    if (MtrCode) {
        $("#panel").find("[name=matrix-notes]").find("[name=share]").html(share);
    }

    if (PrdCode) {
        $("#data-filter").find("[name=share]").show().html(share);
    }

    // Check if the module is enabled
    if (window.__sharethis__) {
        if (MtrCode) {
            $("#panel").find("[name=sharethis-container]").show();
        }
        if (PrdCode) {
            $("#data-filter").find("[name=sharethis-container]").show();
        }

        //Call to reinitialize the sharethis buttons every time the page is loaded.
        window.__sharethis__.initialize();
    };
}

