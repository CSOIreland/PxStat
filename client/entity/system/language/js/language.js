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
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["languages"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["languages"]);


  // Load Modal 
  api.content.load("#overlay", "entity/system/language/index.modal.html");

  // Draw the Datatable on load
  app.language.ajax.read();

  // Bind action to add button
  $("#language-read-container").find("[name='button-create']").once("click", function () {
    app.language.modal.create();
  });


  // Initiate all bootstrap tooltip
  $('[data-toggle="tooltip"]').tooltip();

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

});
