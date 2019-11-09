/*******************************************************************************
Custom JS application specific
*******************************************************************************/

/**
 * On page load
 */
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static.configuration, app.label.static.formats]);

  // Get data from API
  app.format.ajax.read();

  // Last to run
  app.library.html.parseStaticLabel();
});
