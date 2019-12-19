/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.reason = {};
app.release.reason.ajax = {};
app.release.reason.callback = {};
app.release.reason.validation = {};
//#endregion

//#region Reason
/**
* 
 * @param {*} RsnCode
 */
app.release.reason.ajax.readCode = function (RsnCode) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Reason_API.Read",
    {
      RsnCode: RsnCode,
      LngIsoCode: app.label.language.iso.code
    },
    "app.release.reason.callback.readCode");
};

/**
* 
 * @param {*} response
 */
app.release.reason.callback.readCode = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
  } else if (response.data) {
    response.data = response.data[0];

    $("#release-reason-modal-create [name=rsn-value-internal]").html(app.library.html.parseBbCode(response.data.RsnValueInternal));
    $("#release-reason-modal-create [name=rsn-value-external]").html(app.library.html.parseBbCode(response.data.RsnValueExternal));
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 *
 */
app.release.reason.readCodeList = function () {
  app.release.reason.ajax.readCodeList();
};

/**
 * 
 */
app.release.reason.ajax.readCodeList = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Settings.Reason_API.Read",
    { LngIsoCode: app.label.language.iso.code },
    "app.release.reason.callback.readCodeList");
};

/**
* 
 * @param {*} response
 */
app.release.reason.callback.readCodeList = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data !== undefined) {
    $("#release-reason-modal-create").find("[name=rsn-code]").empty().append($('<option>', {
      disabled: "disabled",
      selected: "selected",
      value: "",
      text: app.label.static["select-uppercase"]
    }));

    $.each(response.data, function (index, row) {
      var option = $("<option>", {
        "value": row.RsnCode,
        "text": row.RsnCode,
        "class": "text-capitalize"
      });
      $("#release-reason-modal-create [name=rsn-code]").append(option);
    });
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.release.reason.changeCode = function () {
  var RsnCode = $("#release-reason-modal-create [name=rsn-code] option:selected").val();
  app.release.reason.ajax.readCode(RsnCode);
};

/**
 * 
 */
app.release.reason.toggle = function () {
  // Enable buttons if WIP and Workflow not in progress
  if (app.release.isWorkInProgress && !app.release.isAwaitingResponse && !app.release.isAwaitingSignOff) {
    $("#release-reason [name=add-reason]").prop("disabled", false);
    $("#release-reason-modal-create :submit").prop("disabled", false);
    $("#release-reason-modal-update :submit").prop("disabled", false);
    $("#release-reason table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").prop("disabled", false);
  } else {
    $("#release-reason [name=add-reason]").prop("disabled", true);
    $("#release-reason-modal-create :submit").prop("disabled", true);
    $("#release-reason-modal-update :submit").prop("disabled", true);
    $("#release-reason table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").prop("disabled", true);
  }
};

/**
 * 
 */
app.release.reason.render = function () {
  app.release.reason.toggle();
  app.release.reason.ajax.readList();
};

/**
 * 
 */
app.release.reason.ajax.readList = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Data.ReasonRelease_API.Read",
    {
      RlsCode: app.release.RlsCode,
      LngIsoCode: app.label.language.iso.code
    },
    "app.release.reason.callback.readList",
    null,
    null,
    null,
    { async: false }
  );
};

/**
 * Draw Callback for Datatable
 */
app.release.reason.drawCallback = function () {
  // Display confirmation Modal on DELETE button click
  $("#release-reason table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function (e) {
    var idn = $(this).attr("idn");
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [idn]),
      app.release.reason.ajax.delete,
      idn

    );

  });
  // Display the modal on click
  $("#release-reason table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    app.release.reason.update(idn);
  });
  app.library.datatable.showExtraInfo('#release-reason table', app.release.reason.drawExtraInformation);
  // Refresh toggle post drawing
  app.release.reason.toggle();
}

/**
* 
 * @param {*} response
 */
app.release.reason.callback.readList = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data !== undefined) {
    var data = response.data;

    // Populate Release Reason
    $("#release-reason").hide().fadeIn();

    if ($.fn.dataTable.isDataTable("#release-reason table")) {
      app.library.datatable.reDraw("#release-reason table", data);
    } else {
      var localOptions = {
        // Add Row Index to feed the ExtraInfo modal 
        createdRow: function (row, dataRow, dataIndex) {
          $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
        },
        data: data,
        columns: [
          {
            data: null,
            render: function (data, type, row) {
              return app.library.html.link.edit({ idn: row.RsnCode }, row.RsnCode);
            }
          },
          {
            data: null,
            defaultContent: '',
            sorting: false,
            searchable: false,
            "render": function (data, type, row, meta) {
              return $("<a>", {
                href: "#",
                name: C_APP_DATATABLE_EXTRA_INFO_LINK,
                "idn": meta.row,
                html:
                  $("<i>", {
                    "class": "fas fa-info-circle text-info"
                  }).get(0).outerHTML + " " + app.label.static["description"]
              }).get(0).outerHTML;
            }
          },
          {
            data: "RsnValueInternal",
            "visible": false,
            "searchable": true
          },
          {
            data: "RsnValueExternal",
            "visible": false,
            "searchable": true
          },
          {
            data: null,
            sorting: false,
            searchable: false,
            render: function (data, type, row) {
              // Disable button if WIP
              return app.library.html.deleteButton({ idn: row.RsnCode }, app.release.isWorkInProgress || app.release.isAwaitingResponse || app.release.isAwaitingSignOff ? false : true);
            },
            "width": "1%"
          }],
        drawCallback: function (settings) {
          app.release.reason.drawCallback();

        },
        //Translate labels language
        language: app.label.plugin.datatable
      };
      $("#release-reason table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
        app.release.reason.drawCallback();
      });
    }
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
 * @param {*} d
 */
app.release.reason.drawExtraInformation = function (d) {
  var requestGrid = $("#release-reason [name=extra-information]").clone();
  requestGrid.removeAttr('name');
  requestGrid.find("[name=rsn-value-internal]").empty().html(app.library.html.parseBbCode(d.RsnValueInternal));
  requestGrid.find("[name=rsn-value-external]").empty().html(app.library.html.parseBbCode(d.RsnValueExternal));
  requestGrid.find("[name=cmm-value]").empty().html(app.library.html.parseBbCode(d.CmmValue));
  return requestGrid.show().get(0).outerHTML;
};

/**
* 
 * @param {*} RsnCode
 */
app.release.reason.ajax.read = function (RsnCode) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Data.ReasonRelease_API.Read",
    {
      RlsCode: app.release.RlsCode,
      RsnCode: RsnCode,
      LngIsoCode: app.label.language.iso.code
    },
    "app.release.reason.callback.read"
  );
};

/**
* 
 * @param {*} response
 */
app.release.reason.callback.read = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.release.reason.ajax.readCodeList();
  } else if (response.data) {
    response.data = response.data[0];

    $("#release-reason-modal-update [name=rsn-code]").html(response.data.RsnCode).attr("idn", response.data.RsnCode);
    $("#release-reason-modal-update [name=rsn-value-internal]").html(app.library.html.parseBbCode(response.data.RsnValueInternal));
    $("#release-reason-modal-update [name=rsn-value-external]").html(app.library.html.parseBbCode(response.data.RsnValueExternal));

    var tinyMceId = $("#release-reason-modal-update [name=cmm-value]").attr("id");
    tinymce.get(tinyMceId).setContent(response.data.CmmValue);

    $("#release-reason-modal-update").modal("show");
  }
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* 
 * @param {*} idn
 */
app.release.reason.update = function (idn) {
  app.release.reason.validation.update();
  app.release.reason.ajax.read(idn);
};

/**
 * 
 */
app.release.reason.validation.update = function () {
  $("#release-reason-modal-update form").trigger("reset").validate({
    ignore: [],
    rules: {
      "cmm-value": {
        required: function (element) {
          tinymce.triggerSave();
          return true;
        }
      }
    },
    errorPlacement: function (error, element) {
      $("#release-reason-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.release.reason.ajax.update();
      $("#release-reason-modal-update").modal("hide");
    }
  }).resetForm();
};

/**
 * 
 */
app.release.reason.ajax.update = function () {
  var RsnCode = $("#release-reason-modal-update [name=rsn-code]").attr("idn");

  var tinyMceId = $("#release-reason-modal-update [name=cmm-value]").attr("id");
  var CmmValue = tinymce.get(tinyMceId).getContent();

  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Data.ReasonRelease_API.Update",
    { "RlsCode": app.release.RlsCode, "RsnCode": RsnCode, "CmmValue": CmmValue, "LngIsoCode": app.label.language.iso.code },
    "app.release.reason.callback.update",
    RsnCode,
    null,
    null,
    { async: false });
};

/**
* 
 * @param {*} response
 * @param {*} RsnCode
 */
app.release.reason.callback.update = function (response, RsnCode) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [RsnCode]));
    app.release.reason.ajax.readList();
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
* 
 * @param {*} RsnCode
 */
app.release.reason.ajax.delete = function (RsnCode) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Data.ReasonRelease_API.Delete",
    { RlsCode: app.release.RlsCode, RsnCode: RsnCode, "LngIsoCode": app.label.language.iso.code },
    "app.release.reason.callback.delete",
    { RsnCode: RsnCode },
    null,
    null,
    { async: false }
  );
};

/**
* 
 * @param {*} response
 * @param {*} params
 */
app.release.reason.callback.delete = function (response, params) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    // Refresh datatable
    app.release.reason.ajax.readList();
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [params.RsnCode]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * 
 */
app.release.reason.create = function () {
  // Reset input required for "rsn-code" Select2 drop down functionality (Do not delete.)
  $("#release-reason-modal-create [name=rsn-code]").val("");
  $("#release-reason-modal-create [name=rsn-value-internal]").html("");
  $("#release-reason-modal-create [name=rsn-value-external]").html("");
  // Validation and reset form.
  app.release.reason.validation.create();
  $("#release-reason-modal-create").modal("show");
};

/**
 * 
 */
app.release.reason.validation.create = function () {
  $("#release-reason-modal-create form").trigger("reset").validate({
    ignore: [],
    rules: {
      "rsn-code": {
        required: true
      },
      "cmm-value": {
        required: function (element) {
          tinymce.triggerSave();
          return true;
        }
      }
    },
    errorPlacement: function (error, element) {
      $("#release-reason-modal-create [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.release.reason.ajax.create();
    }
  }).resetForm();
};

/**
 * 
 */
app.release.reason.ajax.create = function () {
  var RsnCode = $("#release-reason-modal-create [name=rsn-code] option:selected").val();
  var tinyMceId = $("#release-reason-modal-create [name=cmm-value]").attr("id");
  var CmmValue = tinymce.get(tinyMceId).getContent();

  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Data.ReasonRelease_API.Create",
    { "RlsCode": app.release.RlsCode, "RsnCode": RsnCode, "CmmValue": CmmValue, "LngIsoCode": app.label.language.iso.code },
    "app.release.reason.callback.create",
    RsnCode,
    null,
    null,
    { async: false });
};

/**
* 
 * @param {*} response
 * @param {*} RsnCode
 */
app.release.reason.callback.create = function (response, RsnCode) {
  $("#release-reason-modal-create").modal("hide");
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    app.release.reason.ajax.readList();
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [RsnCode]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};
//#endregion