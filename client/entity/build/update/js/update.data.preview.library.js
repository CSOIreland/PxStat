/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build.update.data = app.build.update.data || {};
app.build.update.data.preview = app.build.update.data.preview || {};

//#endregion


app.build.update.data.preview.drawCsvData = function () {
    var fileName = app.build.update.upload.file.content.data.name || "";
    var fileSize = app.build.update.upload.file.content.data.size;
    var fileSizeFormated = fileSize != null ? " (" + app.library.utility.formatNumber(Math.ceil(fileSize / 1024)) + "KB)" : "";

    var csvHeader = app.build.update.upload.file.content.data.JSON.meta.fields;
    //build dynamic table header and columns array for datatable
    $("#build-update-modal-preview-data [name=header-row]").empty();
    var columns = [];
    $.each(csvHeader, function (index, header) {
        var tableHeading = $("<th>", {
            "html": header
        });
        $("#build-update-modal-preview-data [name=header-row]").append(tableHeading);
        columns.push(
            { data: header }
        );
    });
    var localOptions = {
        data: app.build.update.upload.file.content.data.JSON.data,
        columns: columns,
        //Translate labels language
        language: app.label.plugin.datatable
    };

    if ($.fn.DataTable.isDataTable("#build-update-modal-preview-data [name=datatable]")) {
        $("#build-update-modal-preview-data").find("[name=datatable]").DataTable().destroy();
        //cannot use redraw as columns are dynamically created depending on the matrix. Have to destroy and re-initiate 
    }

    $("#build-update-modal-preview-data [name=datatable]").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        $("#build-update-modal-preview-data").find()
    });
    $("#build-update-modal-preview-data .modal-title").html(fileName + fileSizeFormated);

}
