/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["products"]]);

  //On page load hide Subject search and "Products of Subject" table. Display only "Releases of Product" Table 
  var sbjCode = api.content.getParam("SbjCode");

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
  $("#product-card-read [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-to-the-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
