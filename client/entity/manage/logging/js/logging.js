/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check();
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["logging"]]);

  app.logging.setDatePicker();

  //Click at the "daterangepicker" Apply button event.
  $("#logging-input").find("[name=input-date-range]").on('apply.daterangepicker', function (ev, picker) {
    //alert("New date range selected: '" + picker.startDate.format('YYYY-MM-DD') + "' to '" + picker.endDate.format('YYYY-MM-DD') + "'");
    app.logging.ajax.read();
  });
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});