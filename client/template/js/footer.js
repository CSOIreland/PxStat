/*******************************************************************************
Footer
*******************************************************************************/
$(document).ready(function () {
    // Set Version in Footer
    $("#footer").find("[name=version]").html(
        $("<a>", {
            "href": C_APP_URL_GITHUB_RELEASE_TAG.sprintf([C_APP_VERSION]),
            "text": 'PxStat ' + C_APP_VERSION,
            "target": "_blank",
            "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0).outerHTML
    );

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
        var link = $("<a>", {
            "href": "#",
            "text": app.label.static["privacy"],
            "name": "privacy"
        }).get(0);
        return link;
    }).append(" | ");

    // Append the (mandatory) Manage Cookie link
    $("#footer").find("[name=links]").append(function () {
        var link = $("<a>", {
            "href": "#",
            "text": app.label.static["manage-cookies"],
            "name": "cookie-manage"
        }).get(0);
        return link;
    }).append(" | ");

    // Append the (mandatory) GitHub link
    $("#footer").find("[name=links]").append(
        $("<a>", {
            "href": C_APP_URL_GITHUB,
            "text": "GitHub",
            "target": "_blank",
            "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0).outerHTML
    ).append(" | ");

    // Append the (mandatory) Report an Issue link
    $("#footer").find("[name=links]").append(
        $("<a>", {
            "href": C_APP_URL_GITHUB_REPORT_ISSUE,
            "text": app.label.static['report-issue'],
            "target": "_blank",
            "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0).outerHTML
    );

    // Bind Privacy click event
    $("#footer").find("[name=privacy]").once("click", function (e) {
        e.preventDefault();
        // Load the Privacy (language specific) into the Modal
        api.content.load("#modal-read-privacy .modal-body", "internationalisation/privacy/" + app.label.language.iso.code + ".html");
        $("#modal-read-privacy").modal("show").on("shown.bs.modal", function () {

            // Scroll to the top section
            $("#modal-read-privacy").clearQueue().animate({
                scrollTop: '+=' + $("#modal-read-privacy")[0].getBoundingClientRect().top
            }, 1000);
        });
    });

    // Bind Manage Cookie click event
    $("#footer").find("[name=cookie-manage]").once("click", function (e) {
        e.preventDefault();

        // Load the Privacy (language specific) into the Modal
        api.content.load("#modal-read-privacy .modal-body", "internationalisation/privacy/" + app.label.language.iso.code + ".html");
        $("#modal-read-privacy").modal("show").on("shown.bs.modal", function () {

            if (!Cookies.get(C_COOKIE_CONSENT)) {
                // Set default deny option if none yet
                app.plugin.cookiconsent.deny(false);
            }

            // Scroll to the Cookie section
            $("#modal-read-privacy").clearQueue().animate({
                scrollTop: '+=' + $("#modal-read-privacy").find("[name=cookies]")[0].getBoundingClientRect().top
            }, 1000);
        });
    });

    // Cookie Consent - Allow button
    $("#footer").find("[name=cookie-allow]").once("click", function () {
        app.plugin.cookiconsent.allow(true);
    });

    if (!Cookies.get(C_COOKIE_CONSENT)) {
        // Cookie Consent - Show Banner
        $("#footer").find("[name=cookie-banner]").fadeIn();
    } else if (Cookies.get(C_COOKIE_CONSENT) == app.plugin.cookiconsent.true) {
        // Cookie Consent - Allowed
        app.plugin.cookiconsent.allow()
    }

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});