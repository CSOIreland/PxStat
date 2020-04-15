/*******************************************************************************
Custom JS application specific
*******************************************************************************/
/**
 * On page load
 */
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check();
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set(["Configuration", "Languages"]);
  app.navigation.breadcrumb.set([app.label.static["configuration"], app.label.static["languages"]]);

  // Load Modal 
  api.content.load("#overlay", "entity/configuration/language/index.modal.html");

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
