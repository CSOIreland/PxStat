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
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["performance"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["performance"]);

  app.performance.setDatePicker();

  //flush cache button
  $("#performance-read-container").find("[name='flush']").once("click", function () {
    api.modal.confirm(app.label.static["confirm-flush-performance"], app.performance.ajax.flushServer);
  });

  //reload button
  $("#performance-read-container").find("[name='refresh-performance']").once("click", function () {
    $('html, body').animate({ scrollTop: $('#performance-read-container').offset().top }, 1000);
    app.performance.ajax.read()
  });

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

});
