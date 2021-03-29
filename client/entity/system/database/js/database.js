/*******************************************************************************
Custom JS application specific
*******************************************************************************/
/**
 * On page load
 */
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check();
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["database"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["database"]);

  // Load Modal 
  api.content.load("#modal-entity", "entity/system/database/index.modal.html");
  app.database.ajax.read();


  //reload button
  $("#database-read-container").find("[name='reload-index']").once("click", function () {
    // force page reload
    api.content.load("#body", "entity/system/database/");
  });

  //reorganise button
  $("#database-read-container").find("[name='reorganise-index']").once("click", function () {
    api.modal.confirm(app.label.static["confirm-reorganise-index-all"], app.database.ajax.reorganise);

  });

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

});
