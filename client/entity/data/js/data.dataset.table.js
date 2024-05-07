/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    app.data.dataset.table.drawDimensions();
    app.data.dataset.table.drawFormat();
    app.data.dataset.table.drawPivotDropdown();
    if (app.data.isLive) {
        $("#data-dataset-table-nav-content").find("[name=confidential-data-warning]").hide();
    }
    else {
        $("#data-dataset-table-nav-content").find("[name=confidential-data-warning]").show();
    }
    //show codes
    $('#data-dataset-table-code-toggle').bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["false"],
        offlabel: app.label.static["true"],
        onstyle: "tertiary",
        offstyle: "neutral",
        width: C_APP_TOGGLE_LENGTH
    });


    if (!app.config.plugin.subscriber.enabled) {
        $("#data-dataset-table-result").find("[name=save-query]").remove()
    };

    $('#data-dataset-table-accordion-collapse-widget [name=auto-update], #data-dataset-table-accordion-collapse-widget [name=fluid-time], #data-dataset-table-api-data-connector-content [name=fluid-time], #data-dataset-table-accordion-collapse-widget [name=link-to-wip], #data-dataset-table-accordion-collapse-widget [name=include-copyright], #data-dataset-table-accordion-collapse-widget [name=include-link], #data-dataset-table-accordion-collapse-widget [name=include-title], #data-dataset-table-accordion-collapse-widget [name=include-pagination], #data-dataset-table-accordion-collapse-widget [name=include-buttons], #data-dataset-table-accordion-collapse-widget [name=include-search], #data-dataset-table-accordion-collapse-widget [name=include-responsive]').bootstrapToggle("destroy").bootstrapToggle({
        onlabel: app.label.static["true"],
        offlabel: app.label.static["false"],
        onstyle: "success text-light",
        offstyle: "warning text-dark",
        height: 38,
        style: "text-light",
        width: C_APP_TOGGLE_LENGTH
    });

    $("#data-dataset-table-api-data-connector-content [name=fluid-time]").once("change", app.data.dataset.table.buildApiParams);

    $("#data-dataset-table-accordion-collapse-widget [name=custom-config]").val(JSON.stringify({ "options": {} }));
    app.data.dataset.table.callback.formatJson();
    $("#data-dataset-table-accordion-collapse-widget [name=valid-json-object]").hide();

    //reset
    $("#data-dataset-table-nav-content").find("[name=reset]").once("click", function () {
        app.data.dataset.table.pivot.dimensionCode = null;
        app.data.dataset.table.drawPivotDropdown();
        app.data.dataset.table.drawDimensions();
    });

    //show data
    $("#data-dataset-table-nav-content").find("[name=show-data]").once("click", function () {
        if (app.data.dataset.table.selectionCount >= app.config.entity.data.threshold.hard) {
            app.data.dataset.table.confirmHardThreshold(app.library.html.parseDynamicLabel("error-read-exceeded", [app.library.utility.formatNumber(app.data.dataset.table.selectionCount), app.config.entity.data.threshold.hard]));
        }
        else if (app.data.dataset.table.selectionCount >= app.config.entity.data.threshold.soft) {
            app.data.dataset.table.confirmSoftThreshold(app.library.html.parseDynamicLabel("confirm-read", [app.library.utility.formatNumber(app.data.dataset.table.selectionCount)]), app.data.dataset.table.ajax.data);
        }
        else {
            //AJAX call get Data Set
            app.data.dataset.table.ajax.data();
        }
    });

    $("#data-dataset-table-accordion-collapse-widget").find("[name=title-value]").val(app.data.dataset.metadata.jsonStat.label.trim());


    $("#data-dataset-table-accordion-collapse-widget").find('input[name="title-value"]').once('keyup', function () {
        if ($(this).val().trim().length) {
            window.clearTimeout($(this).data('timer'));
            $(this).data('timer', window.setTimeout(function () {
                app.data.dataset.table.callback.drawSnippetCode(true)
            }, 400));
        } else {
            app.data.dataset.table.callback.drawSnippetCode(true); //maybe clear input
            window.clearTimeout($(this).data('timer'));
        }
    });


    $('#data-dataset-table-accordion').on('shown.bs.collapse', function (e) {
        if (app.data.isModal) {
            $('#data-view-modal').animate({
                scrollTop: '+=' + $("#" + e.target.id).parent()[0].getBoundingClientRect().top
            }, 1000);
        }
        else {
            $('html, body').animate({
                scrollTop: $("#" + e.target.id).parent().offset().top
            }, 1000);
        }
    });

    $("#data-dataset-table-result").find("[name=save-query]").once("click", function () {
        //check that we have a user to save the table against
        if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess || app.navigation.user.isSubscriberAccess) {
            app.data.dataset.table.saveQuery.drawSaveQueryModal();
        }
        else {
            $("#modal-subscriber-login").modal("show");
        }

    });


    new ClipboardJS("#data-dataset-table-accordion [name=copy-api-info], #data-dataset-table-accordion [name=copy-api-object], #data-dataset-table-accordion [name=copy-snippet-code]");
    app.library.html.parseDynamicPopover("#data-dataset-table-accordion-collapse-widget [label-popover-dynamic=link-to-wip]", "link-to-wip", [app.config.corsDomain]);
    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
    $('[data-bs-toggle="tooltip"]').tooltip();
});