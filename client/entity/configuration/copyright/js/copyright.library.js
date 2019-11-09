/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
// Add Namespace
app.copyright = {};
app.copyright.ajax = {};
app.copyright.callback = {};
app.copyright.modal = {};
app.copyright.validation = {};
//#endregion

//#region Read copyright

/**
 * Read data table form server
 */
app.copyright.ajax.read = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Copyright_API.Read",
    { CprCode: null },
    "app.copyright.callback.read"
  );
};

/**
 * Callback from server after reading data
 * @param  {} response
 */
app.copyright.callback.read = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data !== undefined) {
    app.copyright.drawDatatable(response.data);
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.copyright.drawCallback = function () {
  $('[data-toggle="tooltip"]').tooltip();

  // Display Update Modal
  $("#copyright-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    app.copyright.modal.update(idn);
  });

  // Display Delete Modal
  $("#copyright-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.copyright.modal.delete);
}

/**
 * Draw DataTable
 * @param {*} data
 */
app.copyright.drawDatatable = function (data) {
  if ($.fn.dataTable.isDataTable("#copyright-read-container table")) {
    app.library.datatable.reDraw("#copyright-read-container table", data);
  } else {
    var localOptions = {
      data: data,
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            return app.library.html.link.edit({ idn: row.CprCode }, row.CprCode);
          }
        },
        { data: "CprValue" },
        {
          data: null,
          render: function (data, type, row) {
            return app.library.html.link.external({}, row.CprUrl);
          }
        },
        { data: "RlsCount" },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            var deleteButton = app.library.html.deleteButton({ idn: row.CprCode }, false);
            if (row.RlsCount > 0 || row.GrpReleaseCount > 0) {
              deleteButton = app.library.html.deleteButton({ idn: row.CprCode }, true);
            }
            return deleteButton;
          },
          "width": "1%"
        }
      ],
      drawCallback: function (settings) {
        app.copyright.drawCallback();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };

    $("#copyright-read-container table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.copyright.drawCallback();
    });
  }
};

//#endregion

//#region Create copyright

/**
 * Open modal for new data
 */
app.copyright.modal.create = function () {
  //Define Validation and reset form
  app.copyright.validation.create();
  $("#copyright-modal-create").modal("show");
};

/**
 * Define validation for modal create
 */
app.copyright.validation.create = function () {
  //Validation
  $("#copyright-modal-create").find("form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "cpr-code": {
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHA);
          $(this).val(value);
          return value;
        }
      },
      "cpr-value": {
        required: true
      },
      "cpr-url": {
        required: true,
        url: true
      }
    },
    errorPlacement: function (error, element) {
      $("#copyright-modal-create").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function () {
      app.copyright.ajax.create();
      $("#copyright-modal-create").modal("hide");
    }
  }).resetForm();
};

/**
 * Send new data to server
 */
app.copyright.ajax.create = function () {
  var cprCode = $("#copyright-modal-create").find("[name=cpr-code]").val();
  var cprValue = $("#copyright-modal-create").find("[name=cpr-value]").val();
  var cprUrl = $("#copyright-modal-create").find("[name=cpr-url]").val();
  var apiParams = {
    CprCode: cprCode,
    CprValue: cprValue,
    CprUrl: cprUrl
  };
  var callbackParam = {
    CprCode: cprCode,
    CprValue: cprValue,
    CprUrl: cprUrl
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Copyright_API.Create",
    apiParams,
    "app.copyright.callback.create",
    callbackParam,
    null,
    null,
    { async: false }
  );
};

/**
 * Callback form server after adding data
 * @param {*} response
 * @param {*} callbackParam
 */
app.copyright.callback.create = function (response, callbackParam) {
  app.copyright.ajax.read();
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    $("#copyright-modal-create").modal("hide");
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.CprValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion

//#region Update copyright

/**
 * Update modal
 * @param  {} idn
 */
app.copyright.modal.update = function (idn) {
  app.copyright.ajax.readUpdate(idn);
};

/**
 * Ajax read of update copyright
 * @param  {} idn
 */
app.copyright.ajax.readUpdate = function (idn) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Copyright_API.Read",
    { CprCode: idn },
    "app.copyright.callback.readUpdate",
    idn
  );
};

/**
 * Set up modal for update
 * @param  {} response
 * @param  {} idn
 */
app.copyright.callback.readUpdate = function (response, idn) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.copyright.ajax.read();
  }
  else if (response.data) {
    response.data = response.data[0];
    //Add validation and reset form 
    app.copyright.validation.update();
    //Store OLD value of idn (CprCode) in attribute.
    $("#copyright-modal-update").find("form [name=idn]").val(idn);
    $("#copyright-modal-update").find("[name=cpr-code]").val(idn);
    //Update fields values
    $("#copyright-modal-update").find("[name=cpr-value]").val(response.data.CprValue);
    $("#copyright-modal-update").find("[name=cpr-url]").val(response.data.CprUrl);
    //Show modal
    $("#copyright-modal-update").modal("show");
  }
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Validation for update
 */
app.copyright.validation.update = function () {
  //Sanitizing input
  $("#copyright-modal-update").find("form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "cpr-code": {
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHA);
          $(this).val(value);
          return value;
        }
      },
      "cpr-value": {
        required: true
      },
      "cpr-url": {
        required: true,
        url: true
      }
    },
    errorPlacement: function (error, element) {
      $("#copyright-modal-update").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function () {
      app.copyright.ajax.update();
    }
  }).resetForm();
};

/**
 * Ajax for update /CprCodeOld,CprCodeNew,CprValue,CprUrl
 */
app.copyright.ajax.update = function () {
  var cprCodeOld = $("#copyright-modal-update").find("form [name=idn]").val();
  var cprCodeNew = $("#copyright-modal-update").find("[name=cpr-code]").val();
  var cprValue = $("#copyright-modal-update").find("[name=cpr-value]").val();
  var cprUrl = $("#copyright-modal-update").find("[name=cpr-url]").val();
  var apiParams = {
    CprCodeOld: cprCodeOld,
    CprCodeNew: cprCodeNew,
    CprValue: cprValue,
    CprUrl: cprUrl
  };
  var callbackParam = {
    CprCodeOld: cprCodeOld,
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Copyright_API.Update",
    apiParams,
    "app.copyright.callback.update",
    callbackParam,
    null,
    null,
    { async: false }
  );
};

/**
 * Callback after update
 * @param  {} response
 * @param  {} callbackParam
 */
app.copyright.callback.update = function (response, callbackParam) {
  app.copyright.ajax.read();
  $("#copyright-modal-update").modal("hide");
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.CprCodeOld]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

//#endregion

//#region Delete copyright

/**
 * Show modal for delete
 */
app.copyright.modal.delete = function () {
  var idn = $(this).attr("idn");
  api.modal.confirm(
    app.library.html.parseDynamicLabel("confirm-delete-record", [idn]),
    app.copyright.ajax.delete,
    idn
  );
};

/**
 * Ajax for delete
 * @param {*} idn
 */
app.copyright.ajax.delete = function (idn) {
  var apiParams = {
    CprCode: idn
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Copyright_API.Delete",
    apiParams,
    "app.copyright.callback.delete",
    idn,
    null,
    null,
    { async: false }
  );
};

/**
 * Callback for delete
 * @param {*} response
 * @param {*} idn
 */
app.copyright.callback.delete = function (response, idn) {
  app.copyright.ajax.read();
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [idn]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion
