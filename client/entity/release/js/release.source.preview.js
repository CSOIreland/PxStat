$(document).ready(function () {
    var fileSizeFormated = " (" + app.library.utility.formatNumber(Math.ceil(app.release.fileContent.length / 1024)) + "KB)";
    /*  var fileName = app.build.update.upload.file.content.source.name || "";
     
     var fileDataUTF8 = app.build.update.upload.file.content.source.UTF8;
     // Refresh the Prism highlight
     var fileSizeFormated = fileSize != null ? " (" + app.library.utility.formatNumber(Math.ceil(fileSize / 1024)) + "KB)" : "";*/
    $("#release-source-preview .modal-title").html(app.release.fileName + fileSizeFormated);
    $("#release-source-preview .modal-body pre code").html(app.release.fileContent).ready(Prism.highlightAll());
});