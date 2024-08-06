/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
    // Bind Parse button
    $("#build-create-upload-file-tab-content").find("[name=parse-source-file]").once("click", app.build.create.import.validate.ajax.read);
    // Bind Reset button
    $("#build-create-upload-file-tab-content").find("[name=import-file-reset]").once("click", app.build.create.import.reset);

    $("#build-create-upload-text-tab-content").find("[name=parse-source-file]").once("click", function () {
        app.build.create.file.import.content.UTF8 = $("#build-create-upload-text-tab-content").find("[name=text-content]").val().trim();
        var blob = new Blob([app.build.create.file.import.content.UTF8], { type: "text/plain" });
        var dataUrl = URL.createObjectURL(blob);
        var xhr = new XMLHttpRequest;
        xhr.responseType = 'blob';
        xhr.onload = function () {
            var recoveredBlob = xhr.response;


            // Read file into a Base64 string
            var readerBase64 = new FileReader();
            readerBase64.onload = function (e) {
                // Set the file's content
                app.build.create.file.import.content.Base64 = e.target.result;
                app.build.create.import.validate.ajax.read();
                api.spinner.stop();
            };
            readerBase64.readAsDataURL(recoveredBlob);
            readerBase64.addEventListener("loadstart", function (e) { api.spinner.start(); });
            readerBase64.addEventListener("error", function (e) { api.spinner.stop(); });
            readerBase64.addEventListener("abort", function (e) { api.spinner.stop(); });
            readerBase64.addEventListener("loadend", function (e) {
                $("#build-import-container-file-tab").prop("disabled", true);
                $("#build-import-container").find("[name=validate]").prop("disabled", false);
            });
        };
        xhr.open('GET', dataUrl);
        xhr.send();
    });

    $("#build-create-upload-text-tab-content").find("[name=text-content]").once("keyup change", function () {
        if ($(this).val().length) {
            $("#build-create-upload-text-tab-content").find("[name=parse-source-file]").prop("disabled", false);
        }
        else {
            $("#build-create-upload-text-tab-content").find("[name=parse-source-file]").prop("disabled", true);
        }
    });

    $("#build-create-upload-text-tab-content").find("[name=import-text-reset]").once("click", function () {
        $("#build-create-upload-text-tab-content").find("[name=text-content]").val("");
        $("#build-create-upload-text-tab-content").find("[name=parse-source-file]").prop("disabled", true);
    });
    // Bind Show event
    $("#build-create-initiate-setup").find("[name=import-show-modal]").once("click", function () {
        $('#build-create-import').modal("show");
    });
    // Bind Close event
    $('#build-create-import').on('hide.bs.modal', app.build.create.import.reset);

    $('#build-create-import').on('show.bs.modal', app.build.create.search.ajax.readMatrixList);
});




