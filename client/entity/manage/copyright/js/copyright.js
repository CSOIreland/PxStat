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
  app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["copyrights"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["copyrights"]);

  // Load Modal 
  api.content.load("#modal-entity", "entity/manage/copyright/index.modal.html");

  // Bind add button for add modal
  $("#copyright-read-container").find("[name='button-create']").once("click", function () {
    app.copyright.modal.create();
  });
  // Get data from API
  app.copyright.ajax.read();
  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
