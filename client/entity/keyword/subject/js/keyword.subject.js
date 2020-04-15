/*******************************************************************************
Custom JS application specific
*******************************************************************************/
// On page load
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static.keywords, app.label.static.subjects]);

  // Load Modal
  api.content.load("#overlay", "entity/keyword/subject/index.modal.html");

  // Load the side panel
  api.content.load("#panel", "entity/keyword/search/index.html");

  //Hide table when no subject selected
  $("#keyword-subject-container").find("select[name=select-main-subject-search]").on('select2:clear', function (e) {
    $("#keyword-subject-read").hide();
  });

  $("#keyword-subject-read").hide();


  //Ajax call for data
  app.keyword.subject.ajax.readSubject();

  $("#keyword-subject-modal-create [name=acronym-toggle], #keyword-subject-modal-update [name=acronym-toggle]").bootstrapToggle("destroy").bootstrapToggle({
    on: app.label.static["true"],
    off: app.label.static["false"],
    onstyle: "primary",
    offstyle: "warning",
    width: C_APP_TOGGLE_LENGTH
  });

  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
