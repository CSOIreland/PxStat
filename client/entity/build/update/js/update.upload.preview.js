$(document).ready(function () {

    var fileName = app.build.update.upload.file.content.source.name || "";
    var fileSize = app.build.update.upload.file.content.source.size;
    var fileDataUTF8 = app.build.update.upload.file.content.source.UTF8;
    // Refresh the Prism highlight
    var fileSizeFormated = fileSize != null ? " (" + app.library.utility.formatNumber(Math.ceil(fileSize / 1024)) + "KB)" : "";
    $("#build-update-modal-preview-source .modal-title").html(fileName + fileSizeFormated);
    $("#build-update-modal-preview-source .modal-body pre code").html(fileDataUTF8).ready(Prism.highlightAll());
});