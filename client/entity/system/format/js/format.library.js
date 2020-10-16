/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces definitions
app.format = {};
app.format.ajax = {};
app.format.callback = {};
//#endregion

//#region Read Language

/**
 *Call Ajax for read
 */
app.format.ajax.read = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Format_API.Read",
    { LngIsoCode: null },
    "app.format.callback.read"
  );
};

/**
 * Callback for read
 * @param {*} data
 */
app.format.callback.read = function (data) {
  app.format.callback.drawDataTable(data);
};

/**
 * Draw datatable after read from database
 * @param {*} data 
 */
app.format.callback.drawDataTable = function (data) {
  var localOptions = {
    data: data,
    columns: [
      { data: "FrmType" },
      { data: "FrmVersion" },
      {
        data: null,
        render: function (data, type, row) {
          return app.label.datamodel.format[row.FrmDirection.toLowerCase()];
        }
      }
    ],
    "order": [[1, "desc"]],
    language: app.label.plugin.datatable
  };
  $("#format-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions));
};
//#endregion
