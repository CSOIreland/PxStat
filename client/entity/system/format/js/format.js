/*******************************************************************************
Custom JS application specific
*******************************************************************************/

/**
 * On page load
 */
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["formats"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["formats"]);
  app.navigation.setState("#nav-link-format");

  // Get data from API
  app.format.ajax.read();

  //run bootstrap toggle to show/hide toggle button
  app.library.bootstrap.getBreakPoint();

  // Last to run
  app.library.html.parseStaticLabel();
});
