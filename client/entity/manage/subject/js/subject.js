/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["subjects"]]);

  //display subject table
  app.subject.ajax.read();

  // Parse warning
  $("#subject-read-container [name=warning]").find("label").html(app.library.html.parseDynamicLabel("switch-to-the-default-language", [app.config.language.iso.name]).sprintf([app.config.language.iso.name]));

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
