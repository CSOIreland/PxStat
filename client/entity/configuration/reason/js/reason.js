/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static.configuration, app.label.static.reasons]);

  //Get data from API and Draw the Data Table for Reason
  app.reason.ajax.read();

  // Initiate all text areas as tinyMCE
  app.library.utility.initTinyMce();

  // Parse warning
  $("#reason-table-read-container [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
}); 