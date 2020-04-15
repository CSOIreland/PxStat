/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["releases"]]);
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_MODERATOR, C_APP_PRIVILEGE_POWER_USER]);

  // Get GoTo params
  app.release.goTo.MtrCode = api.content.getParam("MtrCode");
  app.release.goTo.RlsCode = api.content.getParam("RlsCode");

  // Clear Overlay and load dependencies (loading order relevant) - must be after GoTo
  $("#overlay").empty();
  // Search
  api.content.load("#release-container-search", "entity/release/index.search.html");
  // Information
  api.content.load("#release-container-information", "entity/release/index.information.html");
  // Navigation
  api.content.load("#overlay", "entity/release/index.navigation.modal.html", null, true);
  // Property
  api.content.load("#release-container-property", "entity/release/index.property.html");
  // Source
  api.content.load("#overlay", "entity/release/index.source.modal.html", null, true);
  api.content.load("#release-container-source", "entity/release/index.source.html");
  // Comparison
  api.content.load("#overlay", "entity/release/index.comparison.modal.html", null, true);
  // Reason
  api.content.load("#overlay", "entity/release/index.reason.modal.html", null, true);
  api.content.load("#release-container-reason", "entity/release/index.reason.html");
  // Comment
  api.content.load("#overlay", "entity/release/index.comment.modal.html", null, true);
  api.content.load("#release-container-comment", "entity/release/index.comment.html");
  // Workflow
  api.content.load("#release-container-workflow", "entity/release/index.workflow.html");
  api.content.load("#overlay", "entity/release/index.workflow.modal.request.html", null, true);
  api.content.load("#overlay", "entity/release/index.workflow.modal.response.html", null, true);
  api.content.load("#overlay", "entity/release/index.workflow.modal.signoff.html", null, true);
  api.content.load("#release-container-workflow-history", "entity/release/index.workflow.history.html");

  // Load the side panel
  api.content.load("#panel", "entity/release/index.panel.html");

  //initiate all text areas as tinyMCE
  app.library.utility.initTinyMce();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
