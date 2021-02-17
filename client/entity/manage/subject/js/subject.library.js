/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Create Namespace
app.subject = {};
app.subject.ajax = {};
app.subject.callback = {};
app.subject.modal = {};
app.subject.validation = {};
//#endregion

//#region Read subject

/**
 * Ajax read call
 */
app.subject.ajax.read = function () {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      SbjCode: null,
      LngIsoCode: app.label.language.iso.code
    },
    "app.subject.callback.read");
};

/**
 * * Callback subject read
 * @param  {} data
 */
app.subject.callback.read = function (data) {
  app.subject.drawDataTable(data);
};

/**
 * Draw Callback for Datatable
 */
app.subject.drawCallback = function () {
  $('[data-toggle="tooltip"]').tooltip();
  // click event update
  $("#subject-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    app.subject.ajax.readUpdate(idn);
  });
  // click event redirect to entity/manage/product/
  $("#subject-read-container table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    var sbjValue = $(this).attr("sbj-value");
    var obj2send = { "SbjCode": idn, "SbjValue": sbjValue };
    api.content.goTo("entity/manage/product/", "#nav-link-product", "#nav-link-manage", obj2send);
  });
  // click event delete
  $("#subject-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
    var idn = $(this).attr("idn");
    var sbjValue = $(this).attr("sbj-value");
    app.subject.modal.delete(idn, sbjValue);
  });
}

/**
 * Draw table
 * @param {*} data
 */
app.subject.drawDataTable = function (data) {
  if ($.fn.dataTable.isDataTable("#subject-read-container table")) {
    app.library.datatable.reDraw("#subject-read-container table", data);
  } else {

    var localOptions = {
      data: data,
      columns: [
        {
          data: null,
          render: function (_data, _type, row) {
            var attributes = { idn: row.SbjCode, "sbj-value": row.SbjValue }; //idn SbjCode
            return app.library.html.link.edit(attributes, row.SbjValue);
          }
        },
        {
          data: null,
          render: function (_data, _type, row) {
            var attributes = { idn: row.SbjCode, "sbj-value": row.SbjValue }; //idn SbjCode
            var prcCount = row.PrcCount == undefined ? 0 : row.PrcCount;
            return app.library.html.link.internal(attributes, prcCount); //idn => SbjCode
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            var attributes = { idn: row.SbjCode, "sbj-value": row.SbjValue }; //idn SbjCode
            var deleteButton = app.library.html.deleteButton(attributes, false);
            if (row.PrcCount > 0) {
              deleteButton = app.library.html.deleteButton(attributes, true);
            }
            return deleteButton;
          },
          "width": "1%"
        }
      ],
      drawCallback: function (settings) {
        app.subject.drawCallback();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };

    // Subject must be first created in the default language
    if (app.label.language.iso.code == app.config.language.iso.code) {
      // Enable the button
      $("#subject-read-container").find("[name=button-create]").prop("disabled", false);
      // Hide warning
      $("#subject-read-container").find("[name=warning]").hide();
      // Create new subject button click event
      $("#subject-read-container").find("[name=button-create]").once("click", function () {
        app.subject.modal.create();
      });
    } else {
      // Disable the button
      $("#subject-read-container").find("[name=button-create]").prop("disabled", true);
      // Show warning
      $("#subject-read-container").find("[name=warning]").show();
    }

    $("#subject-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.subject.drawCallback();
    });

  }
};

//#endregion

//#region Create subject

/**
 * Show create new subject modal
 * */
app.subject.modal.create = function () {
  //validate Create subject form
  app.subject.validation.create();
  $("#subject-modal-create").modal("show");
};

/**
 * Validation for create new subject
 * */
app.subject.validation.create = function () {
  //allUppper, allLower, onlyAlpha, onlyNum
  $("#subject-modal-create form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "sbj-value": {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#subject-modal-create [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.subject.ajax.create();
    }
  }).resetForm();
};

/**
 * Ajax for create new subject
 */
app.subject.ajax.create = function () {
  var sbjValue = $("#subject-modal-create").find("[name=sbj-value]").val();
  var apiParams = {
    SbjValue: sbjValue
  };
  var callbackParam = {
    SbjValue: sbjValue
  };
  $("#subject-modal-create").modal("hide");
  // A Subject is created always against the default Language
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Create",
    apiParams,
    "app.subject.callback.createOnSuccess",
    callbackParam,
    "app.subject.callback.createOnError",
    null,
    { async: false }
  );
};

/**
 * callback for create new subject
 * @param  {} data
 * @param  {} callbackParam
 */
app.subject.callback.createOnSuccess = function (data, callbackParam) {
  //Refresh the table
  app.subject.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.SbjValue]));
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * callback for create new subject
 * @param  {} error
 */
app.subject.callback.createOnError = function (error) {
  //Refresh the table
  app.subject.ajax.read();
};

//#endregion

//#region Update subject
/**
 * get Update details from ajax
 * @param  {} idn 
 */
app.subject.ajax.readUpdate = function (idn) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      SbjCode: idn,
      LngIsoCode: app.label.language.iso.code
    },
    "app.subject.callback.readUpdate");
};

/**
 * Show modal after ajax call
 * @param  {} data
 */
app.subject.callback.readUpdate = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    //validate Update subject form
    app.subject.validation.update();
    //Display of Modal update
    app.subject.modal.update(data);
  } else {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.subject.ajax.read();
  }
};

/**
 * Display of Modal update
 * @param {*} subjectRecord
 */
app.subject.modal.update = function (subjectRecord) {
  $("#subject-modal-update").find("[name='idn']").val(subjectRecord.SbjCode);
  $("#subject-modal-update").find("[name=sbj-value]").val(subjectRecord.SbjValue);
  $("#subject-modal-update").modal("show");
};

/**
 * Validation of update
 */
app.subject.validation.update = function () {
  $("#subject-modal-update form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "sbj-value": {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#subject-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.subject.ajax.update();
    }
  }).resetForm();
};

/**
 * ajax to update
 */
app.subject.ajax.update = function () {
  // Change app.config.language.iso.code to the selected one
  var sbjCode = $("#subject-modal-update").find("[name='idn']").val();
  var sbjValue = $("#subject-modal-update").find("[name=sbj-value]").val();
  var apiParams = {
    SbjCode: sbjCode,
    SbjValue: sbjValue,
    LngIsoCode: app.label.language.iso.code
  };
  var callbackParam = {
    SbjValue: sbjValue
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Update",
    apiParams,
    "app.subject.callback.updateOnSuccess",
    callbackParam,
    "app.subject.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * callback after ajax update
 * @param  {} data
 * @param  {} callbackParam
 */
app.subject.callback.updateOnSuccess = function (data, callbackParam) {
  $("#subject-modal-update").modal("hide");
  //Refresh the table
  app.subject.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.SbjValue]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * callback after ajax update
 * @param  {} error
 */
app.subject.callback.updateOnError = function (error) {
  $("#subject-modal-update").modal("hide");
  //Refresh the table
  app.subject.ajax.read();
};

//#endregion

//#region Delete subject

/**
 * Modal to confirm delete
 * @param  {} idn
 * @param  {} SbjValue
 */
app.subject.modal.delete = function (idn, SbjValue) {
  var objToSend = {
    idn: idn,
    SbjValue: SbjValue
  };
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [SbjValue]), app.subject.ajax.delete, objToSend);
};

/**
 * Ajax to delete
 * @param  {} objToSend 
 */
app.subject.ajax.delete = function (objToSend) {
  var apiParams = {
    SbjCode: objToSend.idn
  };
  var callbackParam = {
    SbjValue: objToSend.SbjValue
  };
  // A Subject is deleted always against the default Language
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Delete",
    apiParams,
    "app.subject.callback.deleteOnSuccess",
    callbackParam,
    "app.subject.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
 * Callback after delete
 * @param  {} data
 * @param  {} callbackParam
 */
app.subject.callback.deleteOnSuccess = function (data, callbackParam) {
  //Refresh the table
  app.subject.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [callbackParam.SbjValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Callback after delete
 * @param  {} error
 */
app.subject.callback.deleteOnError = function (error) {
  //Refresh the table
  app.subject.ajax.read();
};
//#endregion
