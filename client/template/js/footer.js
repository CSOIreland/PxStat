/*******************************************************************************
Footer
*******************************************************************************/
$(document).ready(function () {
    // Set Version in Footer
    $("#footer").find("[name=version]").html(C_APP_VERSION);

    // Append footer links
    var numLinks = app.config.url.footer.length;
    $.each(app.config.url.footer, function (index, value) {
        var link = $("<a>", {
            "href": value.href,
            "text": value.text,
            "target": "_blank",
            "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0).outerHTML;

        $("#footer").find("[name=links]").append(link).append(" | ");
    });

    // Append the (mandatory) Privacy link
    $("#footer").find("[name=links]").append(function () {
        var linkPrivacy = $("<a>", {
            "href": "#",
            "text": app.label.static.privacy,
            "name": "privacy", // Used by the CookieConsent plugin
            "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0);
        linkPrivacy.addEventListener("click", function (e) {
            e.preventDefault();

            // Load the Privacy (language specific) into the Modal
            api.content.load("#modal-read-privacy .modal-body", "internationalisation/privacy/" + app.label.language.iso.code + ".html");
            $("#modal-read-privacy").modal("show");
        });
        return linkPrivacy;
    }).append(" | ");

    // Append the (mandatory) GitHub link
    $("#footer").find("[name=links]").append(
        $("<a>", {
            "href": C_APP_URL_GITHUB,
            "text": "GitHub",
            "target": "_blank",
            "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0).outerHTML
    );
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});