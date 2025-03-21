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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Copyright_API.Read",
    { CprCode: null },
    "app.copyright.callback.read"
  );
};

/**
 * Callback from server after reading data
 * @param  {} data
 */
app.copyright.callback.read = function (data) {
  app.copyright.drawDatatable(data);
};

/**
 * Draw Callback for Datatable
 */
app.copyright.drawCallback = function () {

  // Display Update Modal
  $("#copyright-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    app.copyright.modal.update(idn);
  });
  //initiate all copy to clipboard 
  new ClipboardJS("#copyright-read-container [name=copyright-url-copy-icon]");
  $('[data-bs-toggle="tooltip"]').tooltip();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

  // Click event "internalLink"
  $("#copyright-read-container table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    var callbackParam = {
      idn: $(this).attr("idn"),
      "CprValue": $(this).attr("cpr-value")
    };
    // Ajax read 
    app.copyright.ajax.readMatrixByCopyright(callbackParam);

    $("#copyright-matrix-modal").modal("show");

  });

  // Display Delete Modal
  $("#copyright-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.copyright.modal.delete);
};

/**
 * Ajax Read Release List
 * @param  {} callbackParam
 */
app.copyright.ajax.readMatrixByCopyright = function (callbackParam) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Data.Matrix_API.ReadByCopyright",
    {
      CprCode: callbackParam.idn,
      LngIsoCode: app.label.language.iso.code
    },
    "app.copyright.callback.readMatrixByCopyright",
    callbackParam);
};

/**
* Handle data from Ajax
* @param  {} data
* @param  {} callbackParam
*/
app.copyright.callback.readMatrixByCopyright = function (data, callbackParam) {
  app.copyright.drawMatrixByCopyrightDataTable(data, callbackParam);
};

app.copyright.drawMatrixByCopyrightDataTable = function (data, callbackParam) {
  $("#copyright-matrix-modal").find("[name=copyright-code]").text(callbackParam.idn);
  $("#copyright-matrix-modal").find("[name=copyright-value]").text(callbackParam.CprValue);
  if ($.fn.dataTable.isDataTable("#copyright-matrix-modal table")) {
    app.library.datatable.reDraw("#copyright-matrix-modal table", data);
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
        app.copyright.drawCallbackMatrix();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };
    $("#copyright-matrix-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.copyright.drawCallbackMatrix();
    });
  }

}

app.copyright.drawCallbackMatrix = function () {
  $('[data-bs-toggle="tooltip"]').tooltip();

  //Release version link click redirect to 
  $("#copyright-matrix-modal table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    //Set the code
    var MtrCode = $(this).attr("MtrCode");

    $("#copyright-matrix-modal").modal("hide");

    //Wait for the modal to close
    $("#copyright-matrix-modal").on('hidden.bs.modal', function (e) {
      //Unbind the event for next call
      $("#copyright-matrix-modal").off('hidden.bs.modal');

      api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": MtrCode });
    })


  });
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
          },
          "width": "1%"
        },
        {
          data: null,
          render: function (_data, _type, row) {
            return app.library.html.link.external({ name: "copyright-url-" + row.CprCode }, app.config.url.application + C_COOKIE_LINK_COPYRIGHT + "/" + row.CprCode) + $("<i>", {
              "class": "far fa-copy fa-lg ms-2",
              "name": "copyright-url-copy-icon",
              "data-bs-toggle": "tooltip",
              "data-clipboard-target": "#copyright-read-container [name=copyright-url-" + row.CprCode + "]",
              "data-bs-placement": "right",
              "title": "",
              "style": "cursor: grab",
              "data-clipboard-action": "copy",
              "label-tooltip": "copy-to-clipboard"
            }).get(0).outerHTML;
          },
          "width": "1%"
        },
        {
          data: null,
          render: function (_data, _type, row) {
            return app.library.html.link.internal({ idn: row.CprCode, "cpr-value": row.CprValue }, String(row.MtrCount));
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            if (row.MtrCount > 0) {
              return app.library.html.deleteButton({ idn: row.CprCode }, true);
            }
            else {
              return app.library.html.deleteButton({ idn: row.CprCode }, false);
            }
          },
          "width": "1%"
        }],
      drawCallback: function (settings) {
        app.copyright.drawCallback();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };

    $("#copyright-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
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
    submitHandler: function (form) {
      $(form).sanitiseForm();
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
  //check for demo site
  if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
    api.modal.error(app.label.static["demo-site-restricted-access"]);
    return
  }
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Copyright_API.Create",
    apiParams,
    "app.copyright.callback.createOnSuccess",
    callbackParam,
    "app.copyright.callback.createOnError",
    null,
    { async: false }
  );
};

/**
 * Callback form server after adding data
 * @param {*} data
 * @param {*} callbackParam
 */
app.copyright.callback.createOnSuccess = function (data, callbackParam) {
  app.copyright.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    $("#copyright-modal-create").modal("hide");
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.CprValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Callback form server after adding data
 * @param {*} error
 * @param {*} callbackParam
 */
app.copyright.callback.createOnError = function (error) {
  app.copyright.ajax.read();
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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Copyright_API.Read",
    { CprCode: idn },
    "app.copyright.callback.readUpdate",
    idn
  );
};

/**
 * Set up modal for update
 * @param  {} data
 * @param  {} idn
 */
app.copyright.callback.readUpdate = function (data, idn) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];
    //Add validation and reset form 
    app.copyright.validation.update();
    //Store OLD value of idn (CprCode) in attribute.
    $("#copyright-modal-update").find("form [name=idn]").val(idn);
    $("#copyright-modal-update").find("[name=cpr-code]").val(idn);
    //Update fields values
    $("#copyright-modal-update").find("[name=cpr-value]").val(data.CprValue);
    $("#copyright-modal-update").find("[name=cpr-url]").val(data.CprUrl);
    //Show modal
    $("#copyright-modal-update").modal("show");
  }
  else {
    // Handle no data
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.copyright.ajax.read();
  }
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
    submitHandler: function (form) {
      $(form).sanitiseForm();
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
  //check for demo site
  if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
    api.modal.error(app.label.static["demo-site-restricted-access"]);
    return
  }
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Copyright_API.Update",
    apiParams,
    "app.copyright.callback.updateOnSuccess",
    callbackParam,
    "app.copyright.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * Callback after update
 * @param  {} data
 * @param  {} callbackParam
 */
app.copyright.callback.updateOnSuccess = function (data, callbackParam) {
  app.copyright.ajax.read();
  $("#copyright-modal-update").modal("hide");

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.CprCodeOld]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Callback after update
 * @param  {} error
 * @param  {} callbackParam
 */
app.copyright.callback.updateOnError = function (error) {
  app.copyright.ajax.read();
  $("#copyright-modal-update").modal("hide");
};

//#endregion

//#region Delete copyright

/**
 * Show modal for delete
 */
app.copyright.modal.delete = function () {
  var idn = $(this).attr("idn");
  api.modal.confirm(
    app.library.html.parseDynamicLabel("confirm-delete", [idn]),
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
  //check for demo site
  if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
    api.modal.error(app.label.static["demo-site-restricted-access"]);
    return
  }
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Settings.Copyright_API.Delete",
    apiParams,
    "app.copyright.callback.deleteOnSuccess",
    idn,
    "app.copyright.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
 * Callback for delete
 * @param {*} data
 * @param {*} idn
 */
app.copyright.callback.deleteOnSuccess = function (data, idn) {
  app.copyright.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [idn]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Callback for delete
 * @param {*} error
 */
app.copyright.callback.deleteOnError = function (error) {
  app.copyright.ajax.read();
};

//#endregion
