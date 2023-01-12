/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build.update.data = app.build.update.data || {};
app.build.update.data.report = app.build.update.data.report || {};

//#endregion


app.build.update.data.report.drawReport = function (data, frmType) {
    //create report as array of objects for rendering as datatable
    var report = [];

    $.each(data.report, function (index, value) {
        if (index > 0) {
            var record = {};
            $.each(data.report[0], function (i, v) {
                record[v] = value[i];
            });
            report.push(record);
        };
    });

    //display report
    var updatedRecords = [];
    var ignoredRecords = [];


    $.each(report, function (index, value) {
        if (value.updated) {
            updatedRecords.push(value)
        }
        else {
            ignoredRecords.push(value)
        }
    });

    var datatableHeadingsUpdated = [];
    var datatableHeadingsIgnored = [];

    if (ignoredRecords.length) {
        $.each(ignoredRecords[0], function (key, value) {
            datatableHeadingsIgnored.push(key)
        });

        $("#build-update-modal-view-report [name=ignored-records-card] [name=no-data-to-display]").hide();
        $("#build-update-modal-view-report [name=ignored-records-wrapper]").show();
        $("#build-update-modal-view-report [name=ignored-records-card] .card-footer").show();


        //build dynamic table header and columns array for datatable
        $("#build-update-modal-view-report [name=ignored-records] [name=header-row]").empty();
        var columnsIgnoredRecords = [];
        $.each(datatableHeadingsIgnored, function (key, value) {
            if (value != "updated") {
                if (value == "duplicate") {
                    var tableHeading = $("<th>", {
                        "html": app.label.static["duplicate"]
                    });
                    $("#build-update-modal-view-report [name=ignored-records] [name=header-row]").append(tableHeading);
                    columnsIgnoredRecords.push(
                        {
                            data: null,
                            type: "natural",
                            render: function (data, type, row) {
                                return app.library.html.boolean(row.duplicate, true, false);
                            }
                        }
                    );
                }
                else {
                    var tableHeading = $("<th>", {
                        "html": value
                    });
                    $("#build-update-modal-view-report [name=ignored-records] [name=header-row]").append(tableHeading);
                    columnsIgnoredRecords.push(
                        { data: value }
                    );
                }
            }

        });
        var localOptions = {
            data: ignoredRecords,
            columns: columnsIgnoredRecords,
            order: [[columnsIgnoredRecords.length - 1, "desc"]],
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
            app.build.update.data.report.downloadRecordsIgnoredCsv(ignoredRecords);
        });

    }
    else {
        $("#build-update-modal-view-report [name=ignored-records-card] [name=no-data-to-display]").show();
        $("#build-update-modal-view-report [name=ignored-records-wrapper]").hide();
        $("#build-update-modal-view-report [name=ignored-records-card] .card-footer").hide();
    }

    if (updatedRecords.length) {
        $.each(updatedRecords[0], function (key, value) {
            datatableHeadingsUpdated.push(key)
        });

        $("#build-update-modal-view-report [name=updated-records-card] [name=no-data-to-display]").hide();
        $("#build-update-modal-view-report [name=updated-records-wrapper]").show();
        $("#build-update-modal-view-report [name=updated-records-card] .card-footer").show();

        //build dynamic table header and columns array for datatable
        $("#build-update-modal-view-report [name=updated-records] [name=header-row]").empty();
        var columnsUpdatedRecords = [];
        $.each(datatableHeadingsUpdated, function (key, value) {
            if (value != "updated" && value != "duplicate") {
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
            app.build.update.data.report.downloadRecordsUpdatedCsv(updatedRecords);
        });
    }
    else {
        $("#build-update-modal-view-report [name=updated-records-card] [name=no-data-to-display]").show();
        $("#build-update-modal-view-report [name=updated-records-wrapper]").hide();
        $("#build-update-modal-view-report [name=updated-records-card] .card-footer").hide();
    }

    $("#build-update-modal-view-report").find("[name=download-file]").once("click", function (e) {
        app.build.update.callback.downloadFile(data.file, frmType);
    });

    $('#build-update-modal-view-report').modal('show');
};

app.build.update.data.report.downloadRecordsUpdatedCsv = function (updatedRecords) {
    var jsonToCSV = {
        "fields": [],
        "data": []
    };

    $.each(updatedRecords[0], function (key, value) {
        if (key != "updated" && key != "duplicate") {
            jsonToCSV.fields.push(key);
        }

    });

    $.each(updatedRecords, function (index, value) {
        var record = [];
        $.each(jsonToCSV.fields, function (index1, value1) {
            record.push(value[value1]);
        });
        jsonToCSV.data.push(record);
    });

    var fileName = app.label.static["records-to-be-updated"].replace(/ /g, "_").toLowerCase() + "_"
        + $("#build-update-properties [name=mtr-value]").val().trim()

    app.library.utility.download(fileName, Papa.unparse(jsonToCSV, { quotes: true }), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};

app.build.update.data.report.downloadRecordsIgnoredCsv = function (ignoredRecords) {
    var jsonToCSV = {
        "fields": [],
        "data": []
    };

    $.each(ignoredRecords[0], function (key, value) {
        if (key != "updated") {
            jsonToCSV.fields.push(key);
        }
    });

    $.each(ignoredRecords, function (index, value) {
        var record = [];
        $.each(jsonToCSV.fields, function (index1, value1) {
            record.push(value[value1]);
        });
        jsonToCSV.data.push(record);
    });

    //Replace "duplicate" header with translation
    var updatedPosition = jsonToCSV.fields.indexOf("duplicate");
    jsonToCSV.fields[updatedPosition] = app.label.static["duplicate"];

    var fileName = app.label.static["records-to-be-ignored"].replace(/ /g, "_").toLowerCase() + "_"
        + $("#build-update-properties [name=mtr-value]").val().trim()

    app.library.utility.download(fileName, Papa.unparse(jsonToCSV, { quotes: true }), C_APP_EXTENSION_CSV, C_APP_MIMETYPE_CSV);
};


