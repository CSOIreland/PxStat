$(document).ready(function () {

    var fileName = app.build.create.dimension.upload.file.name || "";
    var fileSize = app.build.create.dimension.upload.file.size;
    var fileDataUTF8 = app.build.create.dimension.upload.file.content.UTF8;
    // Refresh the Prism highlight
    var fileSizeFormated = fileSize != null ? " (" + app.library.utility.formatNumber(Math.ceil(fileSize / 1024)) + "KB)" : "";
    $("#create-modal-file-preview").find(".modal-title").html(fileName + fileSizeFormated);
    $("#create-modal-file-preview").find(".modal-body pre code").html(fileDataUTF8).ready(Prism.highlightAll());
    $("#upload-indicators-file-data-view-button").prop("disabled", false);
});