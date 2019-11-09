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
    app.config.url.api.private,
    "PxStat.System.Settings.Format_API.Read",
    { LngIsoCode: null },
    "app.format.callback.read"
  );
};

/**
 * Callback for read
 * @param {*} response
 */
app.format.callback.read = function (response) {
  if (response.error) {
    // Handle the Error in the Response first
    app.format.callback.drawDataTable(response.data);
    api.modal.error(response.error.message);
  } else if (response.data !== undefined) {
    // Handle the Data in the Response then
    app.format.callback.drawDataTable(response.data);
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
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
      { data: "FrmVersion" }
    ],
    "order": [[1, "desc"]],
    language: app.label.plugin.datatable
  };
  $("#format-read-container table").DataTable(jQuery.extend({}, app.config.plugin.datatable, localOptions));
};
//#endregion
