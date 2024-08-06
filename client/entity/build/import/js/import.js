$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check(app.config.build.import.moderator ? [C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER] : [C_APP_PRIVILEGE_POWER_USER]);

  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["build"]], [app.label.static["import"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["build"] + " - " + app.label.static["import"]);
  app.navigation.setState("#nav-link-import");
  // Get GoTo params
  app.build.import.goTo.fileImport = api.content.getParam("fileImport");
  // Load Modal
  api.content.load("#modal-entity", "entity/build/import/index.modal.html");

  // Load the side panel
  api.content.load("#panel", "entity/build/import/index.panel.html");

  // Populate the Group dropdown
  app.build.import.ajax.selectGroup();
  //define validation
  app.build.import.validation.create();
  //Bind the cancel button
  $("#build-import-container").find("[name=cancel]").once("click", app.build.import.reset);

  $("#build-import-container-text-content").find("[name=text-content]").once("keyup change", function () {
    var that = this;
    if ($(this).val().trim().length) {
      window.clearTimeout($(this).data('timer'));
      $(this).data('timer', window.setTimeout(function () {

        app.build.import.file.content.UTF8 = $(that).val().trim();
        var blob = new Blob([app.build.import.file.content.UTF8], { type: "text/plain" });
        var dataUrl = URL.createObjectURL(blob);
        var xhr = new XMLHttpRequest;
        xhr.responseType = 'blob';
        xhr.onload = function () {
          var recoveredBlob = xhr.response;


          // Read file into a Base64 string
          var readerBase64 = new FileReader();
          readerBase64.onload = function (e) {
            // Set the file's content
            app.build.import.file.content.Base64 = e.target.result;
          };
          readerBase64.readAsDataURL(recoveredBlob);
          readerBase64.addEventListener("loadstart", function (e) { api.spinner.start(); });
          readerBase64.addEventListener("error", function (e) { api.spinner.stop(); });
          readerBase64.addEventListener("abort", function (e) { api.spinner.stop(); });
          readerBase64.addEventListener("loadend", function (e) {
            $("#build-import-container-file-tab").prop("disabled", true);
            $("#build-import-container").find("[name=validate]").prop("disabled", false);
            api.spinner.stop();
          });
        };

        xhr.open('GET', dataUrl);
        xhr.send();

      }, 400));
    } else {
      $("#build-import-container-file-tab").prop("disabled", false);
      $("#build-import-container").find("[name=validate]").prop("disabled", true);
      window.clearTimeout($(this).data('timer'));
    }

  });

  $('#build-import-container-file-tab').on('shown.bs.tab', function (e) {
    $('#build-import-container').find('[name=preview]').show();
  })

  $('#build-import-container-text-tab').on('shown.bs.tab', function (e) {
    $('#build-import-container').find('[name=preview]').hide();
  })

  //Bind the preview button
  $("#build-import-container").find("[name=preview]").once("click", function () {
    if (app.build.import.file.size > app.config.transfer.threshold.soft) {
      var fileSizeKB = app.library.utility.formatNumber(Math.ceil(app.build.import.file.size / 1024)) + " KB";
      api.modal.confirm(app.library.html.parseDynamicLabel("confirm-preview", [fileSizeKB]), app.build.import.preview);
    }
    else {
      // Preview file content
      app.build.import.preview();
    }
  });

  if (app.build.import.goTo.fileImport) {
    $("#build-import-container-text-tab").tab('show');
    $("#build-import-container-text-content").find('[name="text-content"]').val(app.build.import.goTo.fileImport).keyup().attr('disabled', true);
    $("#build-import-container").find("[name=validate]").prop("disabled", false);
  }

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
  $("#build-import-container").find("[name=upload-file-max-size]").html(app.library.utility.formatNumber(Math.ceil(app.config.transfer.threshold.hard / 1024 / 1024)) + " MB").show();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
