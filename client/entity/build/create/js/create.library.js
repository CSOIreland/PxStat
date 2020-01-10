/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
app.build = {};
app.build.create = {};
//#endregion

app.build.create.file = {};

app.build.create.file.import = {};
app.build.create.file.import.FrqCode = null;
app.build.create.file.import.FrqValue = null;
app.build.create.file.import.Signature = null;
app.build.create.file.import.content = {};
app.build.create.file.import.content.UTF8 = null;
app.build.create.file.import.content.Base64 = null;
app.build.create.file.import.content.JsonStat = [];

app.build.create.file.statistic = {};
app.build.create.file.statistic.content = {};
app.build.create.file.statistic.content.UTF8 = null;

app.build.create.file.statistic.content.data = {};
app.build.create.file.statistic.content.data.JSON = null;

app.build.create.file.classification = {};
app.build.create.file.period = {};
app.build.create.file.period.content = {};
app.build.create.file.period.content.data = {};
app.build.create.file.period.content.data.JSON = null;

app.build.create.file.classification.content = {};
app.build.create.file.classification.content.UTF8 = null;
app.build.create.file.period.content.UTF8 = null;
app.build.create.file.classification.content.data = {};
app.build.create.file.classification.content.data.JSON = null;

//#endregion
//#region upload files
/**
 *Read content of file
 * @param {*} file
 * @param {*} inputObject
 */
api.plugin.dragndrop.readFiles = function (files, inputObject) {
    var uploadDimension = inputObject.attr("id");
    // Read single file only
    var file = files[0];
    if (!file) {
        //clean up input if no files
        switch (uploadDimension) {
            case "build-create-import-file":
                app.build.create.import.cancel();
                return;

            case "build-create-upload-statistic-file":
                app.build.create.dimension.cancelStatisticUpload();
                return;

            case "build-create-upload-classification-file":
                app.build.create.dimension.cancelClassificationUpload();
                return;
            case "build-create-upload-periods-file":
                app.build.create.dimension.cancelPeriodUpload();
                return;
        }
        return;
    };

    var fileExt = file.name.match(/\.[0-9a-z]+$/i)[0];

    switch (uploadDimension) {
        //upload is csv for building dimensions
        case "build-create-upload-statistic-file":
            app.build.create.file.statistic.content.UTF8 = null;
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_CREATE_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                $("#build-create-upload-si").find("[name=upload-submit-statistics]").prop("disabled", true);
                return;
            };
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_CREATE_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                $("#build-create-upload-si").find("[name=upload-submit-statistics]").prop("disabled", true);
                return;
            };

            if (file.size > app.config.upload.threshold.hard) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.config.upload.threshold.hard]));
                // Disable Validate Button
                $("#build-create-upload-si").find("[name=upload-submit-statistics]").prop("disabled", true);
                return;
            };

            // info on screen 
            inputObject.parent().find("[name=upload-file-tip]").hide();
            inputObject.parent().find("[name=upload-file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.create.file.statistic.content.UTF8 = e.target.result;
            };
            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                $("#build-create-upload-si").find("[name=upload-submit-statistics]").prop("disabled", false);
                api.spinner.stop();
            });

            break;
        case "build-create-upload-classification-file":

            $("#build-create-upload-classification").find("[name=errors-card]").hide();
            $("#build-create-upload-classification").find("[name=errors]").empty();
            app.build.create.file.classification.content.UTF8 = null;

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_CREATE_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                $("#build-create-upload-classification").find("[name=upload-submit-classifications]").prop("disabled", true);
                return;
            };
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_CREATE_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                $("#build-create-upload-classification").find("[name=upload-submit-classifications]").prop("disabled", true);
                return;
            };

            if (file.size > app.config.upload.threshold.hard) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.config.upload.threshold.hard]));
                // Disable Validate Button
                $("#build-create-upload-classification").find("[name=upload-submit-classifications]").prop("disabled", true);
                return;
            };

            // info on screen 
            inputObject.parent().find("[name=upload-file-tip]").hide();
            inputObject.parent().find("[name=upload-file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.create.file.classification.content.UTF8 = e.target.result;
            };
            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                $("#build-create-upload-classification").find("[name=upload-submit-classifications]").prop("disabled", false);
                api.spinner.stop();
            });

            break;
        case "build-create-upload-periods-file":

            $("#build-create-upload-periods").find("[name=errors]").empty();
            $("#build-create-upload-periods").find("[name=errors-card]").hide();

            app.build.create.file.period.content.UTF8 = null;

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_CREATE_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                $("#build-create-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);
                return;
            };
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_CREATE_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                $("#build-create-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);
                return;
            };

            if (file.size > app.config.upload.threshold.hard) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.config.upload.threshold.hard]));
                // Disable Validate Button
                $("#build-create-upload-periods").find("[name=upload-submit-periods]").prop("disabled", true);
                return;
            };

            // info on screen 
            inputObject.parent().find("[name=upload-file-tip]").hide();
            inputObject.parent().find("[name=upload-file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.create.file.period.content.UTF8 = e.target.result;
            };
            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                $("#build-create-upload-periods").find("[name=upload-submit-periods]").prop("disabled", false);
                api.spinner.stop();
            });

            break;
        case "build-create-import-file":
            //clean up
            app.build.create.file.import.FrqCode = null;
            app.build.create.file.import.FrqValue = null;
            app.build.create.file.import.Signature = null;
            app.build.create.file.import.content.UTF8 = null;
            app.build.create.file.import.content.Base64 = null;

            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(fileExt.toLowerCase(), C_APP_UPDATEDATASET_FILE_ALLOWED_EXTENSION) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-extension", [fileExt]));
                $("#build-create-import").find("[name=parse-source-file]").prop("disabled", true);
                return;
            };
            // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
            if ($.inArray(file.type.toLowerCase(), C_APP_UPDATEDATASET_FILE_ALLOWED_TYPE) == -1) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-type", [file.type]));
                $("#build-create-import").find("[name=parse-source-file]").prop("disabled", true);
                return;
            };

            if (file.size > app.config.upload.threshold.hard) {
                // Show Error
                api.modal.error(app.library.html.parseDynamicLabel("error-file-size", [app.config.upload.threshold.hard]));
                // Disable Parse Button
                $("#build-create-import").find("[name=parse-source-file]").prop("disabled", true);
                return;
            };

            // info on screen 
            inputObject.parent().find("[name=upload-file-tip]").hide();
            inputObject.parent().find("[name=upload-file-name]").html(file.name + " (" + app.library.utility.formatNumber(Math.ceil(file.size / 1024)) + " KB)").show();

            // Read file into an UTF8 string
            var readerUTF8 = new FileReader();
            readerUTF8.onload = function (e) {
                app.build.create.file.import.content.UTF8 = e.target.result;
            };


            readerUTF8.readAsText(file);
            readerUTF8.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerUTF8.addEventListener("error", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerUTF8.addEventListener("loadend", function (e) {
                api.spinner.stop();
            });

            // Read file into a Base64 string
            var readerBase64 = new FileReader();
            readerBase64.onload = function (e) {
                // Set the file's content
                app.build.create.file.import.content.Base64 = e.target.result;
            };
            readerBase64.readAsDataURL(file);
            readerBase64.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerBase64.addEventListener("error", function (e) { api.spinner.stop(); });
            readerBase64.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerBase64.addEventListener("loadend", function (e) {
                // Enable Parse Button
                $("#build-create-import").find("[name=parse-source-file]").prop("disabled", false);
                api.spinner.stop();
            });

            break;
        default:
            break;
    };
};

//#endregion
