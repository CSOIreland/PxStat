$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["upload"]]);
  // Load the side panel
  api.content.load("#panel", "entity/build/upload/index.panel.html");
  // Populate the Group dropdown
  app.build.upload.ajax.selectGroup();
  //define validation
  app.build.upload.validation.create();
  app.build.upload.validation.frequency();
  //Bind the cancel button
  $("#build-upload-container").find("[name=upload-btn-cancel]").once("click", app.build.upload.reset);
  //Bind the preview button
  $("#build-upload-container").find("[name=upload-btn-preview]").once("click", function () {
    if (app.build.upload.file.size > app.config.upload.threshold.soft) {
      var fileSizeKB = app.library.utility.formatNumber(Math.ceil(app.build.upload.file.size / 1024)) + " KB";
      api.modal.confirm(app.library.html.parseDynamicLabel("confirm-file", [fileSizeKB]), app.build.upload.preview);
    }
    else {
      // Preview file content
      app.build.upload.preview();
    }
  });
  //Bind the validate frequency button
  $("#build-upload-modal-frequency").find("[name=btn-validate]").once("click", function () {

  });

  // Initiate Drag n Drop plugin
  api.plugin.dragndrop.initiate(document, window);
  //set up date picker
  app.build.upload.setDataPicker();

  $('#build-upload-modal-preview').on('hide.bs.modal', function (e) {
    $("#build-upload-modal-preview .modal-title").empty();
    $("#build-upload-modal-preview .modal-body").empty();
  });

  $('#build-upload-modal-frequency').on('show.bs.modal', app.build.upload.ajax.getFrequencyCodes);

  // Set the max file-size in the Upload box
  $("#build-upload-container").find("[name=upload-file-max-size]").html(app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB").show();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
