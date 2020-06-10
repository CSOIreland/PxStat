/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build.update.data = app.build.update.data || {};
app.build.update.data.report = app.build.update.data.report || {};

//#endregion


app.build.update.data.report.drawReport = function (data, frmType) {
    //display report
    var updatedRecords = [];
    var ignoredRecords = [];

    $.each(data.report, function (index, value) {
        if (value.updated) {
            updatedRecords.push(value)
        }
        else {
            ignoredRecords.push(value)
        }
    });

    var datatableHeadings = [];

    if (updatedRecords.length) {
        $.each(updatedRecords[0], function (key, value) {
            datatableHeadings.push(key);
        });
    }
    else {
        $.each(ignoredRecords[0], function (key, value) {
            datatableHeadings.push(key);
        });
    }

    //build dynamic table header and columns array for datatable
    $("#build-update-modal-view-report [name=ignored-records] [name=header-row]").empty();
    var columnsIgnoredRecords = [];

    $.each(datatableHeadings, function (key, value) {
        if (value != "updated") {
            var tableHeading = $("<th>", {
                "html": value
            });
            $("#build-update-modal-view-report [name=ignored-records] [name=header-row]").append(tableHeading);
            columnsIgnoredRecords.push(
                { data: value }
            );
        }

    });
    var localOptions = {
        data: ignoredRecords,
        columns: columnsIgnoredRecords,
        buttons: [{
            extend: 'csv',
            title: app.build.update.data.MtrCode + "." + app.label.static["records-to-be-ignored"].replace(/ /g, "_").toUpperCase() + "." + moment().format(app.config.mask.datetime.file)
        }],
        //Translate labels language
        language: app.label.plugin.datatable
    };

    if ($.fn.DataTable.isDataTable("#build-update-modal-view-report [name=ignored-records]")) {
        $("#build-update-modal-view-report [name=ignored-records]").DataTable().destroy();
        //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
    }

    $("#build-update-modal-view-report [name=ignored-records]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        $("#build-update-modal-view-report").find()
    });

    // Invoke DataTables CSV export
    // https://stackoverflow.com/questions/45515559/how-to-call-datatable-csv-button-from-custom-button
    $("#build-update-modal-view-report").find("[name=csv-ignored-records]").once("click", function () {
        $("#build-update-modal-view-report [name=ignored-records]").DataTable().button('.buttons-csv').trigger();
    });

    //build dynamic table header and columns array for datatable
    $("#build-update-modal-view-report [name=updated-records] [name=header-row]").empty();
    var columnsUpdatedRecords = [];
    $.each(datatableHeadings, function (key, value) {
        if (value != "updated") {
            var tableHeading = $("<th>", {
                "html": value
            });
            $("#build-update-modal-view-report [name=updated-records] [name=header-row]").append(tableHeading);
            columnsUpdatedRecords.push(
                { data: value }
            );
        }

    });
    var localOptions = {
        data: updatedRecords,
        columns: columnsUpdatedRecords,
        buttons: [{
            extend: 'csv',
            title: app.build.update.data.MtrCode + "." + app.label.static["records-to-be-updated"].replace(/ /g, "_").toUpperCase() + "." + moment().format(app.config.mask.datetime.file)
        }],
        //Translate labels language
        language: app.label.plugin.datatable
    };

    if ($.fn.DataTable.isDataTable("#build-update-modal-view-report [name=updated-records]")) {
        $("#build-update-modal-view-report [name=updated-records]").DataTable().destroy();
        //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
    }

    $("#build-update-modal-view-report [name=updated-records]").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        $("#build-update-modal-view-report").find()
    });

    // Invoke DataTables CSV export
    // https://stackoverflow.com/questions/45515559/how-to-call-datatable-csv-button-from-custom-button
    $("#build-update-modal-view-report").find("[name=csv-updated-records]").once("click", function () {
        $("#build-update-modal-view-report [name=updated-records]").DataTable().button('.buttons-csv').trigger();
    });

    $("#build-update-modal-view-report").find("[name=download-file]").once("click", function (e) {
        app.build.update.callback.downloadFile(data.file, frmType);
    });

    $('#build-update-modal-view-report').modal('show');
}
