$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check(app.config.build.import.moderator ? [C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER] : [C_APP_PRIVILEGE_POWER_USER]);

  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["import"]]);

  // Load Modal
  api.content.load("#overlay", "entity/build/import/index.modal.html");

  // Load the side panel
  api.content.load("#panel", "entity/build/import/index.panel.html");

  // Populate the Group dropdown
  app.build.import.ajax.selectGroup();
  //define validation
  app.build.import.validation.create();
  //Bind the cancel button
  $("#build-import-container").find("[name=cancel]").once("click", app.build.import.reset);
  //Bind the preview button
  $("#build-import-container").find("[name=preview]").once("click", function () {
    if (app.build.import.file.size > app.config.upload.threshold.soft) {
      var fileSizeKB = app.library.utility.formatNumber(Math.ceil(app.build.import.file.size / 1024)) + " KB";
      api.modal.confirm(app.library.html.parseDynamicLabel("confirm-preview", [fileSizeKB]), app.build.import.preview);
    }
    else {
      // Preview file content
      app.build.import.preview();
    }
  });

  // Initiate Drag n Drop plugin
  api.plugin.dragndrop.initiate(document, window);
  //set up date picker
  app.build.import.setDatePicker();

  $('#build-import-modal-preview').on('hide.bs.modal', function (e) {
    $("#build-import-modal-preview .modal-title").empty();
    $("#build-import-modal-preview .modal-body").empty();
  });

  // Get the Frequecy Codes 
  app.build.import.ajax.getFrequencyCodes();

  $('#build-import-modal-frequency').on('show.bs.modal', function () {
    app.build.import.validation.frequency();
  });

  // Set the max file-size in the Upload box
  $("#build-import-container").find("[name=upload-file-max-size]").html(app.library.utility.formatNumber(Math.ceil(app.config.upload.threshold.hard / 1024 / 1024)) + " MB").show();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
