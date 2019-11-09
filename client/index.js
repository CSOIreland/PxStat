$(document).ready(function () {
  // Set Document Title
  $("title").text(app.config.title);
  // Set Document Language
  $("html").attr("lang", app.label.language.iso.code);

  if (!api.uri.getNoHeader())
    //  Get Header
    api.content.load("#header", "template/header.html");

  if (!api.uri.getNoNavbar())
    // Get Navigation
    api.content.load("#navigation", "template/navigation.html");

  if (api.uri.getBody())
    // Get Custom Body
    api.content.load("#body", api.uri.getBody());

  if (!api.uri.getNoFooter())
    // Get Footer
    api.content.load("#footer", "template/footer.html");

  // Get Alerts
  api.content.load("#alert", "entity/manage/alert/index.notice.html");

  // Get Modal
  api.content.load("#modal", "template/modal.html");

  // Get Spinner
  api.content.load("#spinner", "template/spinner.html");

  // Load a CookieLink or the Default page
  if (Cookies.get(C_COOKIE_LINK_SEARCH)) {
    app.library.utility.cookieLink(C_COOKIE_LINK_SEARCH, "Search", "entity/data/", "#nav-link-data");
  } else if (Cookies.get(C_COOKIE_LINK_PRODUCT)) {
    app.library.utility.cookieLink(C_COOKIE_LINK_PRODUCT, "PrcCode", "entity/data/", "#nav-link-data");
  } else if (Cookies.get(C_COOKIE_LINK_COPYRIGHT)) {
    app.library.utility.cookieLink(C_COOKIE_LINK_COPYRIGHT, "CprCode", "entity/data/", "#nav-link-data");
  } else if (Cookies.get(C_COOKIE_LINK_TABLE)) {
    app.library.utility.cookieLink(C_COOKIE_LINK_TABLE, "MtrCode", "entity/data/", "#nav-link-data");
  } else if (Cookies.get(C_COOKIE_LINK_RELEASE)) {
    app.library.utility.cookieLink(C_COOKIE_LINK_RELEASE, "RlsCode", "entity/release/", "#nav-link-release");
  } else {
    // Load default Entity
    api.content.goTo("entity/data/", "#nav-link-data");
  }


});
