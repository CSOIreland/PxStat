/*******************************************************************************
Custom JS application specific
*******************************************************************************/
// On page load
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["keywords"]], [app.label.static["subjects"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["keywords"] + " - " + app.label.static["subjects"]);
  app.navigation.setState("#nav-link-keyword-subject");


  // Load Modal
  api.content.load("#modal-entity", "entity/keyword/subject/index.modal.html");

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
    onlabel: app.label.static["true"],
    offlabel: app.label.static["false"],
    onstyle: "success text-light",
    offstyle: "warning text-dark",
    height: 38,
    style: "text-light",
    width: C_APP_TOGGLE_LENGTH
  });

  //run bootstrap toggle to show/hide toggle button
  app.library.bootstrap.getBreakPoint();

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
