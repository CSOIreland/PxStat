/*******************************************************************************
Custom JS application specific
*******************************************************************************/
/**
 * On page load
 */
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check();
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["system"]], [app.label.static["configuration"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["system"] + " - " + app.label.static["configuration"]);
  app.navigation.setState("#nav-link-configuration");


  // Bind action to add button
  $("#configuration-read-container").find("[name='search-file']").once("click", app.configuration.modal.search);

  $("#configuration-application-accordion").find("[name=edit]").once("click", function () {
    //disable all accordion open/close 
    //only option is to update or cancel
    $("#configuration-application-accordion .accordion-button").prop('disabled', true);

    switch ($(this).data("config")) {
      case "client":
        $("#configuration-client-config").find("[name=config-obj-wrapper]").hide();
        $("#configuration-client-config").find("[name=edit]").hide();
        $("#configuration-client-config").find("[name=update]").show();
        $("#configuration-client-config").find("[name=cancel]").show();
        $("#configuration-client-config").find("[name=config-obj-edit]").val($("#configuration-client-config").find("[name=config-obj]").text()).show();
        $("#configuration-client-config").find("[name=copy-snippet-code]").hide();
        $("#configuration-client-config").find("[name=copy-snippet-code-edit]").show();
        new ClipboardJS("#configuration-client-config [name=copy-snippet-code-edit]");

        break;
      case "global":
        $("#configuration-global-config").find("[name=config-obj-wrapper]").hide();
        $("#configuration-global-config").find("[name=edit]").hide();
        $("#configuration-global-config").find("[name=update]").show();
        $("#configuration-global-config").find("[name=cancel]").show();
        $("#configuration-global-config").find("[name=config-obj-edit]").val($("#configuration-global-config").find("[name=config-obj]").text()).show();
        $("#configuration-global-config").find("[name=copy-snippet-code]").hide();
        $("#configuration-global-config").find("[name=copy-snippet-code-edit]").show();
        new ClipboardJS("#configuration-global-config [name=copy-snippet-code-edit]");

        break;
      case "server":
        $("#configuration-server-config").find("[name=config-obj-wrapper]").hide();
        $("#configuration-server-config").find("[name=edit]").hide();
        $("#configuration-server-config").find("[name=update]").show();
        $("#configuration-server-config").find("[name=cancel]").show();
        $("#configuration-server-config").find("[name=config-obj-edit]").val($("#configuration-server-config").find("[name=config-obj]").text()).show();
        $("#configuration-server-config").find("[name=copy-snippet-code]").hide();
        $("#configuration-server-config").find("[name=copy-snippet-code-edit]").show();
        new ClipboardJS("#configuration-server-config [name=copy-snippet-code-edit]");

        break;
      default:
        break;
    }
  });

  $("#configuration-application-accordion").find("[name=schema]").once("click", function () {
    switch ($(this).data("config")) {
      case "client":
        app.configuration.ajax.getSchemaRead("client");
        break;
      case "global":
        app.configuration.ajax.getSchemaRead("global");
        break;
      case "server":
        app.configuration.ajax.getSchemaRead("server");
        break;
      default:
        break;
    }
  });

  $("#configuration-application-accordion").find("[name=cancel]").once("click", function () {
    //reload entity to abort any actions
    api.content.load("#body", "entity/system/configuration/");
  });

  $("#configuration-application-accordion").find("[name=update]").once("click", function () {
    app.configuration.validation($(this).data("config"));
  });

  $("#configuration-read-api-content").find("[name=cancel]").once("click", function () {
    //reload entity to abort any actions
    api.content.load("#body", "entity/system/configuration/");
  });

  $("#configuration-read-api-content").find("[name=update]").once("click", function () {
    api.modal.confirm(
      app.label.static["update-api-config-confirm"],
      app.configuration.ajax.updateApiConfig
    );
  });

  //Load Modal 
  api.content.load("#modal-entity", "entity/system/configuration/index.modal.html");
  app.configuration.ajax.getAppVersion();
  app.configuration.ajax.getClientConfig();
  app.configuration.ajax.getGlobalConfig();
  app.configuration.ajax.getServerConfig();
  app.configuration.ajax.getApiVersion();
  app.configuration.ajax.getApiConfig();

  $("#configuration-modal-search").on('hide.bs.modal', function (e) {
    // clean up modal after closing
    $("#configuration-modal-search").find("[name=type]").empty();
    $("#configuration-modal-search").find("[name=source]").empty();
    $("#configuration-modal-search").find("[name=value]").empty();
  })

  //run bootstrap toggle to show/hide toggle button
  app.library.bootstrap.getBreakPoint();

  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();

});
