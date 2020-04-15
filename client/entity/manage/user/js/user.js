// HTML loaded
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["users"]]);

  // Load Modal
  api.content.load("#overlay", "entity/manage/user/index.modal.html");

  // Bootstrap tooltip
  $('[data-toggle="tooltip"]').tooltip();

  // Get data from API and Draw the Data Table for User
  app.user.ajax.read();

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
