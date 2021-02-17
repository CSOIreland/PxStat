/*******************************************************************************
Navigation
*******************************************************************************/

//#region Data

$(document).ready(function () {
  //#region Dashboard
  api.content.navigate(
    "#nav-link-dashboard",
    "entity/dashboard/",
    "#nav-link-dashboard"
  );
  //#endregion

  //#region Data
  api.content.navigate(
    "#nav-link-data",
    "entity/data/",
    "#nav-link-data"
  );
  //#endregion

  //#region Dashboard
  api.content.navigate(
    "#nav-link-dashboard",
    "entity/dashboard/",
    "#nav-link-dashboard"
  );
  //#endregion

  //#region Build
  api.content.navigate(
    "#nav-link-create",
    "entity/build/create/",
    "#nav-link-create",
    "#nav-link-build"
  );
  api.content.navigate(
    "#nav-link-update",
    "entity/build/update/",
    "#nav-link-update",
    "#nav-link-build"
  );

  //#endregion

  //#region import
  api.content.navigate(
    "#nav-link-import",
    "entity/build/import/",
    "#nav-link-import",
    "#nav-link-build"
  );
  //#endregion
  //#region import
  api.content.navigate(
    "#nav-link-widget",
    "entity/build/widget/",
    "#nav-link-widget",
    "#nav-link-build"
  );
  //#endregion

  //#region Release
  api.content.navigate(
    "#nav-link-release",
    "entity/release/",
    "#nav-link-release"
  );
  //#endregion

  //#region Analytics
  api.content.navigate(
    "#nav-link-analytic",
    "entity/analytic/",
    "#nav-link-analytic"
  );
  //#endregion

  //#region Manage
  api.content.navigate(
    "#nav-link-user",
    "entity/manage/user/",
    "#nav-link-user",
    "#nav-link-manage"
  );
  api.content.navigate(
    "#nav-link-group",
    "entity/manage/group/",
    "#nav-link-group",
    "#nav-link-manage"
  );

  api.content.navigate(
    "#nav-link-reason",
    "entity/manage/reason/",
    "#nav-link-reason",
    "#nav-link-manage"
  );

  api.content.navigate(
    "#nav-link-copyright",
    "entity/manage/copyright/",
    "#nav-link-copyright",
    "#nav-link-manage"
  );

  api.content.navigate(
    "#nav-link-keyword-subject",
    "entity/keyword/subject/",
    "#nav-link-keyword-subject",
    "#nav-link-keyword"
  );
  api.content.navigate(
    "#nav-link-keyword-product",
    "entity/keyword/product/",
    "#nav-link-keyword-product",
    "#nav-link-keyword"
  );
  api.content.navigate(
    "#nav-link-alert",
    "entity/manage/alert/",
    "#nav-link-alert",
    "#nav-link-manage"
  );
  api.content.navigate(
    "#nav-link-email",
    "entity/manage/email/",
    "#nav-link-email",
    "#nav-link-manage"
  );

  //#endregion

  //#region Configuration




  //#endregion

  //#region Keyword
  api.content.navigate(
    "#nav-link-subject",
    "entity/manage/subject/",
    "#nav-link-subject",
    "#nav-link-manage"
  );
  api.content.navigate(
    "#nav-link-product",
    "entity/manage/product/",
    "#nav-link-product",
    "#nav-link-manage"
  );
  api.content.navigate(
    "#nav-link-keyword-release",
    "entity/keyword/release/",
    "#nav-link-keyword-release",
    "#nav-link-keyword"
  );
  //#endregion

  //#region system
  api.content.navigate(
    "#nav-link-configuration",
    "entity/system/configuration/",
    "#nav-link-configuration",
    "#nav-link-system"
  );

  api.content.navigate(
    "#nav-link-cache",
    "entity/system/cache/",
    "#nav-link-cache",
    "#nav-link-system"
  );

  api.content.navigate(
    "#nav-link-performance",
    "entity/system/performance/",
    "#nav-link-performance",
    "#nav-link-system"
  );

  api.content.navigate(
    "#nav-link-format",
    "entity/system/format/",
    "#nav-link-format",
    "#nav-link-system"
  );

  api.content.navigate(
    "#nav-link-tracing",
    "entity/system/tracing/",
    "#nav-link-tracing",
    "#nav-link-system"
  );

  api.content.navigate(
    "#nav-link-logging",
    "entity/system/logging/",
    "#nav-link-logging",
    "#nav-link-system"
  );

  api.content.navigate(
    "#nav-link-language",
    "entity/system/language/",
    "#nav-link-language",
    "#nav-link-system"
  );
  //#endregion

  $("#nav-user-login").once("click", app.openAccess.modal.login);

  $("#nav-user-logout").once("click", function () {
    api.modal.confirm(app.label.static["open-access-logout-confirm"], app.openAccess.modal.logout)
  });


  // First check for windows/network user
  app.navigation.access.ajax.ReadCurrentWindowsAccess();

  // Set language dropdown
  app.navigation.language.ajax.read();
});
