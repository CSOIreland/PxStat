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

  // Load dependencies
  api.content.load("#release-container-search", "entity/release/index.search.html");
  api.content.load("#release-container-information", "entity/release/index.information.html");
  api.content.load("#release-container-navigation", "entity/release/index.navigation.html");
  api.content.load("#release-container-property", "entity/release/index.property.html");
  api.content.load("#release-container-source", "entity/release/index.source.html");
  api.content.load("#release-container-comparison", "entity/release/index.comparison.html");
  api.content.load("#release-container-reason", "entity/release/index.reason.html");
  api.content.load("#release-container-comment", "entity/release/index.comment.html");
  api.content.load("#release-container-workflow", "entity/release/index.workflow.html");
  api.content.load("#release-container-workflow-modal-request", "entity/release/index.workflow.modal.request.html");
  api.content.load("#release-container-workflow-modal-response", "entity/release/index.workflow.modal.response.html");
  api.content.load("#release-container-workflow-modal-signoff", "entity/release/index.workflow.modal.signoff.html");
  api.content.load("#release-container-workflow-history", "entity/release/index.workflow.history.html");

  // Load the side panel
  api.content.load("#panel", "entity/release/index.panel.html");

  //initiate all text areas as tinyMCE
  app.library.utility.initTinyMce();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
