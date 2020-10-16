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
  app.navigation.breadcrumb.set([app.label.static["system"], app.label.static["configuration"]]);

  //Changing plus to minus
  $("#configuration-accordion").on('show.bs.collapse', function (e) {
    $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
  });
  //Changing minus to plus
  $("#configuration-accordion").on('hide.bs.collapse', function (e) {
    $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-plus-circle");
  });

  // Bind action to add button
  $("#configuration-read-container").find("[name='search-file']").once("click", app.configuration.modal.search);

  //Load Modal 
  api.content.load("#overlay", "entity/system/configuration/index.modal.html");
  app.configuration.ajax.getFiles();

  $("#configuration-modal-search").on('hide.bs.modal', function (e) {
    // clean up modal after closing
    $("#configuration-modal-search").find("[name=type]").empty();
    $("#configuration-modal-search").find("[name=source]").empty();
    $("#configuration-modal-search").find("[name=value]").empty();
  })

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

});
