/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["products"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["products"]);

  // GoTo
  var sbjCode = api.content.getParam("SbjCode");

  // Load Modal - must be after GoTo
  api.content.load("#overlay", "entity/manage/product/index.modal.html");

  //Hide table when no subject selected
  $("#product-container").find("[name=select-main-subject-search]").on('select2:clear', function (e) {

    $("#product-card-read").hide();
  });

  //Read the Subject for the select-main-subject-search (to search Subjects).
  if (sbjCode) {
    app.product.ajax.readSubject(sbjCode);
  }
  else {
    app.product.ajax.readSubject();
  }

  // Parse warning
  $("#product-card-read [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));
  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
