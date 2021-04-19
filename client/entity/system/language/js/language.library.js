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
    app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.language.callback.readOnSuccess",
    null,
    "app.language.callback.readOnError",
    null
  );
};

/**
 * Callback for read
 * @param {*} data
 */
app.language.callback.readOnSuccess = function (data) {
  app.language.drawDatatable(data);
  $("#language-modal-create").modal("hide");
};

/**
 * Callback for read
 * @param {*} error
 */
app.language.callback.readOnError = function (error) {
  app.language.drawDatatable();
  $("#language-modal-create").modal("hide");
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

  // Click event "internalLink"
  $("#language-read-container table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    var callbackParam = {
      idn: $(this).attr("idn"),
      "LngValue": $(this).attr("lng-value")
    };
    // Ajax read 
    app.language.ajax.readMatrixByLanguage(callbackParam);

    $("#language-matrix-modal").modal("show");

  });

  // Delete action
  $("#language-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.language.modal.delete);

}

/**
 * Ajax Read Release List
 * @param  {} callbackParam
 */
app.language.ajax.readMatrixByLanguage = function (callbackParam) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Data.Matrix_API.ReadByLanguage",
    {
      LngIsoCode: callbackParam.idn
    },
    "app.language.callback.readMatrixByLanguage",
    callbackParam);
};

/**
* Handle data from Ajax
* @param  {} data
* @param  {} callbackParam
*/
app.language.callback.readMatrixByLanguage = function (data, callbackParam) {
  app.language.drawMatrixByLanguageDataTable(data, callbackParam);
};

app.language.drawMatrixByLanguageDataTable = function (data, callbackParam) {
  $("#language-matrix-modal").find("[name=language-code]").text(callbackParam.idn);
  $("#language-matrix-modal").find("[name=language-value]").text(callbackParam.LngValue);
  if ($.fn.dataTable.isDataTable("#language-matrix-modal table")) {
    app.library.datatable.reDraw("#language-matrix-modal table", data);
  } else {
    var localOptions = {
      data: data,
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
            return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
          }
        }
      ],
      drawCallback: function (settings) {
        app.language.drawCallbackMatrix();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };
    $("#language-matrix-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.language.drawCallbackMatrix();
    });
  }

}

app.language.drawCallbackMatrix = function () {
  $('[data-toggle="tooltip"]').tooltip();

  //Release version link click redirect to 
  $("#language-matrix-modal table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    //Set the code
    var MtrCode = $(this).attr("MtrCode");

    $("#language-matrix-modal").modal("hide");

    //Wait for the modal to close
    $("#language-matrix-modal").on('hidden.bs.modal', function (e) {
      //Unbind the event for next call
      $("#language-matrix-modal").off('hidden.bs.modal');

      api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": MtrCode });
    })


  });
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
        {
          data: null,
          render: function (_data, _type, row) {
            return app.library.html.link.internal({ idn: row.LngIsoCode, "lng-value": row.LngIsoName }, String(row.MtrCount));
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            return app.library.html.deleteButton({ idn: row.LngIsoCode }, row.MtrCount > 0 ? true : false);
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
    app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: idn },
    "app.language.callback.readUpdate",
    idn
  );
};

/**
 * * Callback read
 * @param  {} data
 * @param  {} idn 
 */
app.language.callback.readUpdate = function (data, idn) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    // Define Validation for Edit
    app.language.validation.update();

    $("#language-modal-update form").find("[name=lng-iso-code]").text(idn);
    $("#language-modal-update form").find("[name=lng-iso-name]").val(data.LngIsoName);

    $("#language-modal-update").modal("show");
  }
  else {
    // Handle no data
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.language.ajax.read();
  }
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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Language_API.Update",
    apiParams,
    "app.language.callback.updateOnSuccess",
    callbackParam,
    "app.language.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * * Callback from server, after update
 * @param  {} data
 * @param  {} callbackParam
 *  
 */
app.language.callback.updateOnSuccess = function (data, callbackParam) {
  $("#language-modal-update").modal("hide");
  // Reload the data
  app.language.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.LngIsoCode]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * * Callback from server, after update
 * @param  {} error
 *  
 */
app.language.callback.updateOnError = function (error) {
  $("#language-modal-update").modal("hide");
  // Reload the data
  app.language.ajax.read();
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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Language_API.Create",
    apiParams,
    "app.language.callback.createOnSuccess",
    callbackParam,
    "app.language.callback.createOnError",
    null,
    { async: false }
  );
};

/**
 * Ajax call for create
 * @param {*} data
 * @param {*} callbackParam
 */
app.language.callback.createOnSuccess = function (data, callbackParam) {
  app.language.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    $("#language-modal-create").modal("hide");
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.LngIsoCode]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Ajax call for create
 * @param {*} error
 */
app.language.callback.createOnError = function (error) {
  app.language.ajax.read();
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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Language_API.Delete",
    apiParams,
    "app.language.callback.deleteOnSuccess",
    idn,
    "app.language.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
 *Ajax callback for delete
 * @param {*} data
 * @param {*} idn
 */
app.language.callback.deleteOnSuccess = function (data, idn) {
  app.language.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [idn]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 *Ajax callback for delete
 * @param {*} error
 */
app.language.callback.deleteOnError = function (error) {
  app.language.ajax.read();
};

/**
 * Show modal for delete
 */
app.language.modal.delete = function () {
  var idn = $(this).attr("idn");
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [idn]), app.language.ajax.delete, idn);
};
//#endregion