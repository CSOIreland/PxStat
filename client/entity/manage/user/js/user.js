// HTML loaded
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check([C_APP_PRIVILEGE_POWER_USER]);
  app.navigation.setLayout(false);
  app.navigation.setBreadcrumb([[app.label.static["manage"]], [app.label.static["users"]]]);
  app.navigation.setMetaDescription();
  app.navigation.setTitle(app.label.static["manage"] + " - " + app.label.static["users"]);
  app.navigation.setState("#nav-link-user");

  // Load Modal
  api.content.load("#modal-entity", "entity/manage/user/index.modal.html");

  // Bootstrap tooltip
  $('[data-bs-toggle="tooltip"]').tooltip();

  // Get data from API and Draw the Data Table for User
  app.user.ajax.read();

  $('#user-modal-create-local-user').on('hide.bs.modal', function (e) {
    //Flush the Modal user-modal-create-ad-user. Do not delete. Required for User Select2 functionality.
    $("#user-modal-create-local-user").find("[name=ccn-name-create]").val("");
    $("#user-modal-create-local-user").find("[name=ccn-email-create]").val("");
  })

  //run bootstrap toggle to show/hide toggle button
  app.library.bootstrap.getBreakPoint();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});
