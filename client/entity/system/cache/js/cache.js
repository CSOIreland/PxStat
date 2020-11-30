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
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["cache"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["cache"]);

  app.cache.ajax.read();

  //reload button
  $("#cache-read-container").find("[name='refresh-config']").once("click", function () {
    $('html, body').animate({ scrollTop: $('#cache-read-container').offset().top }, 1000);
    api.content.load("#body", "entity/system/cache/")
  });

  //flush cache button
  $("#cache-read-container").find("[name='flush-cache']").once("click", function () {
    api.modal.confirm(app.label.static["confirm-flush-cache"], app.cache.ajax.flushCache);
  });

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

});
