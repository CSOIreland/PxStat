/*******************************************************************************
Custom JS application specific // 
*******************************************************************************/
//#region Namespaces definitions
app.language = {};
app.language.modal = {};
app.language.validation = {};
app.language.ajax = {};
app.language.callback = {};
//#endregion

//#region Read Language

/**
 *Call Ajax for read
 */
app.language.ajax.read = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.language.callback.read"
  );
};

/**
 * Callback for read
 * @param {*} response
 */
app.language.callback.read = function (response) {
  if (response.error) {
    // Handle the Error in the Response first
    app.language.drawDatatable();
    api.modal.error(response.error.message);
    $("#language-modal-create").modal("hide");
  } else if (response.data !== undefined) {
    // Handle the Data in the Response then
    app.language.drawDatatable(response.data);
    $("#language-modal-create").modal("hide");
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.language.drawCallback = function () {
  $('[data-toggle="tooltip"]').tooltip();

  // Update link
  $("#language-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    app.language.modal.update(idn);
  });
  // Delete action
  $("#language-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.language.modal.delete);

}

/**
 * Populate table
 * @param {*} data
 */
app.language.drawDatatable = function (data) {
  if ($.fn.dataTable.isDataTable("#language-read-container table")) {
    app.library.datatable.reDraw("#language-read-container table", data);
  } else {

    var localOptions = {
      data: data,
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            return app.library.html.link.edit({ idn: row.LngIsoCode }, row.LngIsoCode);
          }
        },
        { data: "LngIsoName" },
        { data: "RlsCount" },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            var deleteButton = app.library.html.deleteButton({ idn: row.LngIsoCode }, false);
            if (row.RlsCount > 0 || row.GrpReleaseCount > 0) {
              deleteButton = app.library.html.deleteButton({ idn: row.LngIsoCode }, true);
            }
            return deleteButton;
          },
          "width": "1%"
        },
      ],
      drawCallback: function (settings) {
        app.language.drawCallback();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };

    $("#language-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.language.drawCallback();
    });

  }
};

//#endregion

//#region Update Language
//modal update

/**
 * Open modal
 * * @param {*} idn
 */
app.language.modal.update = function (idn) {
  app.language.ajax.readUpdate(idn);
};

/**
 * Ajax read
 * @param {*} idn
 */
app.language.ajax.readUpdate = function (idn) {
  api.ajax.jsonrpc.request(
    app.config.url.api.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: idn },
    "app.language.callback.readUpdate",
    idn
  );
};

/**
 * * Callback read
 * @param  {} response
 * @param  {} idn 
 */
app.language.callback.readUpdate = function (response, idn) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.language.ajax.read();
  }
  else if (response.data) {
    response.data = response.data[0];

    // Define Validation for Edit
    app.language.validation.update();

    $("#language-modal-update form").find("[name=lng-iso-code]").text(idn);
    $("#language-modal-update form").find("[name=lng-iso-name]").val(response.data.LngIsoName);

    $("#language-modal-update").modal("show");
  }
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Define validation for update modal
 */
app.language.validation.update = function () {
  $("#language-modal-update").find("form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "lng-iso-name": {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#language-modal-update").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.language.ajax.update();
    }
  }).resetForm();
};

/**
 * Call server for update
 */
app.language.ajax.update = function () {
  var lngIsoCode = $("#language-modal-update").find("[name=lng-iso-code]").text();
  var lngIsoName = $("#language-modal-update").find("[name=lng-iso-name]").val();
  var apiParams = {
    LngIsoCode: lngIsoCode,
    LngIsoName: lngIsoName
  };
  var callbackParam = {
    LngIsoCode: lngIsoCode,
    LngIsoName: lngIsoName
  };
  //Ajax request Update
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Language_API.Update",
    apiParams,
    "app.language.callback.update",
    callbackParam,
    null,
    null,
    { async: false }
  );
};

/**
 * * Callback from server, after update
 * @param  {} response
 * @param  {} callbackParam
 *  
 */
app.language.callback.update = function (response, callbackParam) {
  $("#language-modal-update").modal("hide");
  // Reload the data
  app.language.ajax.read();
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.LngIsoCode]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};
//#endregion

//#region Create Language
/**
 *Clear previous values and show modal
 */
app.language.modal.create = function () {
  app.language.validation.create();
  $("#language-modal-create").modal("show");
};

/**
 * Define validation for modal Create
 */
app.language.validation.create = function () {
  //Sanitizing input
  $("#language-modal-create").find("form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "lng-iso-code": {
        lettersonly: true,
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_LOWERCASE, C_APP_REGEX_ALPHA);
          $(this).val(value);
          return value;
        }
      },
      "lng-iso-name": {
        required: true,
      }
    },
    errorPlacement: function (error, element) {
      $("#language-modal-create").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.language.ajax.create();
    }
  }).resetForm();
};

/**
 * Submit on create modal
 */
app.language.ajax.create = function () {
  var lngIsoCode = $("#language-modal-create").find("[name=lng-iso-code]").val();
  var lngIsoName = $("#language-modal-create").find("[name=lng-iso-name]").val();
  var apiParams = {
    LngIsoCode: lngIsoCode,
    LngIsoName: lngIsoName
  };
  var callbackParam = {
    LngIsoCode: lngIsoCode,
    LngIsoName: lngIsoName
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Language_API.Create",
    apiParams,
    "app.language.callback.create",
    callbackParam,
    null,
    null,
    { async: false }
  );
};

/**
 * Ajax call for create
 * @param {*} response
 * @param {*} callbackParam
 */
app.language.callback.create = function (response, callbackParam) {
  app.language.ajax.read();
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    $("#language-modal-create").modal("hide");
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.LngIsoCode]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion

//#region Delete
/**
 *Ajax call for delete
 * @param {*} idn
 */
app.language.ajax.delete = function (idn) {
  var apiParams = { LngIsoCode: idn };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Language_API.Delete",
    apiParams,
    "app.language.callback.delete",
    idn,
    null,
    null,
    { async: false }
  );
};

/**
 *Ajax callback for delete
 * @param {*} response
 * @param {*} idn
 */
app.language.callback.delete = function (response, idn) {
  app.language.ajax.read();
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [idn]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Show modal for delete
 */
app.language.modal.delete = function () {
  var idn = $(this).attr("idn");
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [idn]), app.language.ajax.delete, idn);
};
//#endregion