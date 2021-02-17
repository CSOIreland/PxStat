/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["keywords"]], [app.label.static["products"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["keywords"] + " - " + app.label.static["products"]);

  // Load Modal
  api.content.load("#modal-entity", "entity/keyword/product/index.modal.html");

  // Load the side panel
  api.content.load("#panel", "entity/keyword/search/index.html");

  //Clear the product and table when no subject is selected
  $("#keyword-product-container").find("[name=select-main-subject-search]").on("select2:clear", function (e) {
    $("#keyword-product-container").find("[name=select-main-product-search]").empty().select2();
    $("#keyword-product-container").find("[name=select-main-product-search]").prop('disabled', true);
    $("#keyword-product-read").hide();
  });

  //Clear the table when no product is selected
  $("#keyword-product-container").find("[name=select-main-product-search]").on('select2:clear', function (e) {
    $("#keyword-product-read").hide();
  });

  //hide product-keyword Data Table
  $("#keyword-product-read").hide();

  //Update DropDown Subject
  app.keyword.product.readSubjects();

  $("#keyword-product-modal-create [name=acronym-toggle], #keyword-product-modal-update [name=acronym-toggle]").bootstrapToggle("destroy").bootstrapToggle({
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