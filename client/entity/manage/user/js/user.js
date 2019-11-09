// HTML loaded
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["users"]]);

  // Bootstrap tooltip
  $('[data-toggle="tooltip"]').tooltip();

  // Get data from API and Draw the Data Table for User
  app.user.ajax.read();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
