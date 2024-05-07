/*******************************************************************************
Open access
*******************************************************************************/

$(document).ready(function () {
    $('#modal-open-access-setup-1fa').on('hide.bs.modal', function (e) {
        // Force the reload of the application 

        window.location.href = window.location.pathname;
    })

    $('#modal-open-access-setup-2fa').on('hide.bs.modal', function (e) {
        // Force the reload of the application 

        window.location.href = window.location.pathname;
    });

    $("#modal-open-access-setup-1fa").find("[name=password-requirements]").popover({
        "html": true,
        "content": $("#modal-open-access-templates").find("[name=password-requirements]").clone().get(0).outerHTML,
        "template": $("#modal-open-access-templates").find("[name=popover-template]").clone().get(0).outerHTML,
    });

    $("#modal-open-access-setup-2fa").find("[name=authenticator-app-popover]").popover({
        "content": app.label.static["open-access-2fa-authenticator-app"]
    });

    $("#modal-open-access-setup-2fa").find("[name=smart-device-popover]").popover({
        "content": app.label.static["open-access-2fa-smart-device"]
    });

    $("#modal-open-access-setup-2fa").find("[name=secure-location-popover]").popover({
        "content": app.label.static["open-access-2fa-secure-location"]
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});