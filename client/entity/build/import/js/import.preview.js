$(document).ready(function () {

    $("#build-import-modal-preview .modal-title").html(app.build.import.file.name + " (" + app.library.utility.formatNumber(Math.ceil(app.build.import.file.size / 1024)) + "KB)");
    $("#build-import-modal-preview .modal-body pre code").html(app.build.import.file.content.UTF8);
    // Refresh Prism highlight
    Prism.highlightAll();
});
