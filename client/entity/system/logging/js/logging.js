/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check();
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["logging"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["logging"]);
  app.navigation.setState("#nav-link-logging");


  app.logging.setDatePicker();

  //Click at the "daterangepicker" Apply button event.
  $("#logging-input").find("[name=input-date-range]").on('apply.daterangepicker', function (ev, picker) {
    app.logging.ajax.read();
  });
  //run bootstrap toggle to show/hide toggle button
  app.library.bootstrap.getBreakPoint();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});