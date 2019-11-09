$(document).ready(function () {

    $("#build-upload-modal-preview .modal-title").html(app.build.upload.file.name + " (" + app.library.utility.formatNumber(Math.ceil(app.build.upload.file.size / 1024)) + "KB)");
    $("#build-upload-modal-preview .modal-body pre code").html(app.build.upload.file.content.UTF8);
    // Refresh Prism highlight
    Prism.highlightAll();
});
