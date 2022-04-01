/*******************************************************************************
Footer
*******************************************************************************/
$(document).ready(function () {

    //Set footer content
    app.footer.render.contact();
    app.footer.render.links();
    app.footer.render.social();
    app.footer.render.watermark();

    //smoother rendering of dynamic footer
    $("#footer .footer").fadeIn(1000);

    $('[data-toggle="tooltip"]').tooltip();

    // Bind Privacy click event
    $("#footer").find("[name=privacy]").once("click", function (e) {
        e.preventDefault();
        // Load the Privacy (language specific) into the Modal
        api.content.load("#modal-read-privacy .modal-body", "internationalisation/privacy/" + app.label.language.iso.code + ".html");
        $("#modal-read-privacy").modal("show");

    });

    $("#modal-read-privacy").once("shown.bs.modal", function () {
        history.replaceState({}, '', window.location.origin + window.location.pathname + "?privacy");
        // Scroll to the top section
        $("#modal-read-privacy").clearQueue().animate({
            scrollTop: '+=' + $("#modal-read-privacy")[0].getBoundingClientRect().top
        }, 1000);
    });

    $("#modal-read-privacy").once("hide.bs.modal", function () {
        if (api.uri.isParam("privacy")) {
            history.replaceState({}, '', window.location.origin + window.location.pathname)
        }
    });


    //init bootstrap breatpoints for toggle panel
    $(window).on('init.bs.breakpoint', function (e) {
        bsBreakpoints.toggle(e.breakpoint);
    });

    $(window).on('new.bs.breakpoint', function (e) {
        bsBreakpoints.toggle(e.breakpoint);
    });
    bsBreakpoints.init();

    $(window).on("resize", function () {
        $("#content").css("padding-bottom", $("#footer .footer").outerHeight(true) + "px")
    });

    if (api.uri.isParam("privacy")) {
        $("#footer").find("[name=privacy]").trigger("click");
    }

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});