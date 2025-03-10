/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["reasons"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["reasons"]);
  app.navigation.setState("#nav-link-reason");

  // Load Modal 
  api.content.load("#modal-entity", "entity/manage/reason/index.modal.html");

  //Get data from API and Draw the Data Table for Reason
  app.reason.ajax.read();

  // Initiate all text areas as tinyMCE
  app.plugin.tinyMce.initiate();

  // Parse warning
  $("#reason-table-read-container [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
}); 