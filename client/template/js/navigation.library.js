
/*******************************************************************************
Custom JS application specific
****************************************************************************/
//#region Update Navigation User

//#region Namespaces 
app.navigation = {};
app.navigation.layout = {};
app.navigation.breadcrumb = {};

app.navigation.access = {};
app.navigation.access.ajax = {};
app.navigation.access.callback = {};

app.navigation.user = {};
app.navigation.user.ajax = {};
app.navigation.user.callback = {};
app.navigation.user.CcnUsername = null;
app.navigation.user.CcnName = null;

app.navigation.language = {};
app.navigation.language.ajax = {};
app.navigation.language.callback = {};

//#endregion

//#region Access
/**
 * Set access against current user
 */
app.navigation.access.ajax.set = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.ReadCurrentAccess",
    { CcnUsername: null },
    "app.navigation.access.callback.set",
    null,
    "app.navigation.access.render",
    null,
    { async: false }
  );
};

/**
 * Set the navigation
 * @param {*} data 
 */
app.navigation.access.callback.set = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];
    // Render the Navigation
    app.navigation.access.render(data.PrvCode)
  }
  else
    // Anonymous or Not Registered user
    app.navigation.user.remove();
};

/**
 * Render the navigation
 * @param {*} data 
 */
app.navigation.access.render = function (PrvCode) {
  PrvCode = PrvCode || null;

  // Show menu items according to user's privilege
  switch (PrvCode) {
    case C_APP_PRIVILEGE_ADMINISTRATOR:
      // Display current user's information
      app.navigation.user.ajax.read();

      //Dashboard
      $("#nav-link-dashboard").parent().show();
      // Releases
      $("#nav-link-release").parent().show();
      // Analytics
      $("#nav-link-analytic").parent().show();
      // Build
      $("#nav-link-build").parent().show();
      // Manage
      $("#nav-link-manage").parent().show();
      // Keywords
      $("#nav-link-keyword").parent().show();
      // System
      $("#nav-link-system").parent().show();
      break;
    case C_APP_PRIVILEGE_POWER_USER:
      // Display current user's information
      app.navigation.user.ajax.read();

      //Dashboard
      $("#nav-link-dashboard").parent().show();
      // Releases
      $("#nav-link-release").parent().show();
      // Analytics
      $("#nav-link-analytic").parent().show();
      // Build
      $("#nav-link-build").parent().show();
      // Manage
      $("#nav-link-manage").parent().show();
      // Keywords
      $("#nav-link-keyword").parent().show();
      // System
      $("#nav-link-system").parent().remove();
      break;
    case C_APP_PRIVILEGE_MODERATOR:
      // Display current user's information
      app.navigation.user.ajax.read();

      //Dashboard
      $("#nav-link-dashboard").parent().show();
      // Releases
      $("#nav-link-release").parent().show();
      // Analytics
      $("#nav-link-analytic").parent().show();
      // Build
      if (app.config.build.create.moderator
        || app.config.build.update.moderator
        || app.config.build.import.moderator) {
        $("#nav-link-build").parent().show();
      }
      else {
        $("#nav-link-build").parent().remove();
      }

      if (!app.config.build.create.moderator)
        $("#nav-link-create").remove();

      if (!app.config.build.update.moderator)
        $("#nav-link-update").remove();

      if (!app.config.build.import.moderator)
        $("#nav-link-import").remove();

      // Manage
      $("#nav-link-manage").parent().remove();
      // Keywords
      $("#nav-link-keyword").parent().remove();
      // System
      $("#nav-link-system").parent().remove();
      break;
    default:
      // Anonymous or Not Registered user
      app.navigation.user.remove();

      //Dashboard
      $("#nav-link-dashboard").parent().remove();
      // Releases
      $("#nav-link-release").parent().remove();
      // Analytics
      $("#nav-link-analytic").parent().remove();
      // Build
      $("#nav-link-build").parent().remove();
      // Manage
      $("#nav-link-manage").parent().remove();
      // Keywords
      $("#nav-link-keyword").parent().remove();
      // Keywords
      $("#nav-link-condfiguration").parent().remove();
      // System
      $("#nav-link-system").parent().remove();

      break;
  }
};

/**
 * Set up page layout
 */
app.navigation.layout.set = function (isDataEntity) {
  isDataEntity = isDataEntity || false;
  //empty and panel and navigation on all internal entities 
  $("#data-navigation").empty();
  $("#data-filter").empty();
  $("#panel").empty();
  $("#overlay").empty();
  $("#sidebar").removeClass("bg-sidebar").removeClass("col-sm-3").addClass("col-sm-4");
  $("#body").removeClass("col-sm-9").addClass("col-sm-8");

  switch (isDataEntity) {
    case true:
      //only data entity
      //any entity except data entity
      $("#body").removeClass("order-first").addClass("order-last").removeClass("col-sm-8").addClass("col-sm-9");
      $("#sidebar").removeClass("col-sm-4").addClass("col-sm-3").addClass("bg-sidebar");
      break;
    case false:
    default:
      //any entity except data entity
      $("#body").removeClass("order-last").addClass("order-first").removeClass("col-sm-9").addClass("col-sm-8");
      $("#sidebar").removeClass("col-sm-3").addClass("col-sm-4").removeClass("bg-sidebar");
      //hide responsive serach
      $("#data-search-row-responsive").hide();
      break;
  }
};


app.navigation.breadcrumb.set = function (breadcrumb) {
  breadcrumb = breadcrumb || [];
  var title = "";

  $("#breadcrumb-nav").find("[name=breadcrumb-list]").empty();

  // Clone Home link
  var homeLink = $("#navigation-templates").find("[name=home]").clone();
  $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(homeLink);

  if ($.isArray(breadcrumb) && breadcrumb.length > 0) {
    $.each(breadcrumb, function (index, value) {
      var test = typeof value;
      if (typeof value === 'object' && value !== null) {
        var linkItem = $("#navigation-templates").find("[name=link]").clone();
        var breadcrumbLink = $("<a>", {
          "text": value.text,
          "href": "#"
        }).get(0);
        breadcrumbLink.addEventListener("click", function (e) {
          e.preventDefault();
          api.content.goTo(value.goTo.pRelativeURL, value.goTo.pNav_link_SelectorToHighlight, null, value.goTo.pParams);
        });

        linkItem.html(breadcrumbLink);
        $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(linkItem);

        title += '/' + value.text;
      }
      else {
        var staticItem = $("#navigation-templates").find("[name=item]").clone();
        staticItem.html(String(value));
        $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(staticItem);

        title += '/' + String(value);
      }
    });
  }
  // Set Document Title
  $("title").text(app.config.title + title);
};

/**
 * Check access against current user
 */
app.navigation.access.check = function (PrvCodeList) {
  PrvCodeList = PrvCodeList || [];
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.ReadCurrentAccess",
    null,
    "app.navigation.access.callback.check",
    PrvCodeList
  );
};
/**
 * Check access callback against current user
 * @param {*} data 
 * @param {*} PrvCodeList 
 */
app.navigation.access.callback.check = function (data, PrvCodeList) {
  PrvCodeList = PrvCodeList || [];
  // Add Administrator privilege by default;
  // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
  if ($.inArray(C_APP_PRIVILEGE_ADMINISTRATOR, PrvCodeList) == -1)
    PrvCodeList.push(C_APP_PRIVILEGE_ADMINISTRATOR);

  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(data.PrvCode, PrvCodeList) == -1) {
      // Prevent backbutton check
      app.plugin.backbutton.check = false;
      // Force page reload
      window.location.href = window.location.pathname;
    }
  } else {
    // Prevent backbutton check
    app.plugin.backbutton.check = false;
    // Force page reload
    window.location.href = window.location.pathname;
  }
};
//#endregion

//#region User 

/**
 * Remove User data from Navigation Bar
 */
app.navigation.user.remove = function () {
  $("#nav-user").remove();
};

/**
 * Read User data to Navigation Bar
 */
app.navigation.user.ajax.read = function () {

  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.ReadCurrent",
    { CcnUsername: null },
    "app.navigation.user.callback.read",
    null,
    null,
    null,
    { async: false }
  );
};

/**
 * Populate User data to Navigation Bar
 * @param {*} data 
 */
app.navigation.user.callback.read = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    // Store for later user
    app.library.user.data.CcnUsername = data.CcnUsername;
    app.library.user.data.CcnName = data.CcnName;
    app.library.user.data.CcnPrvCode = data.PrvCode;
    // Set name on screen
    $("#nav-user").find("[name=name]").text(data.CcnName);

    // Show user details on click
    $("#nav-user").on("click", function (event) {
      app.library.user.modal.readCurrent();
    });
  } else
    app.navigation.user.remove();
};

//#endregion


//#region language


/**
* Get languages
*/
app.navigation.language.ajax.read = function () {
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.navigation.language.callback.read",
    null,
    null,
    null,
    { async: false });
};

/**
* Fill dropdown for Language
* @param {*} data 
*/
app.navigation.language.callback.read = function (data) {
  // Empty dropdown
  $("#nav-language").find("[name=dropdown]").empty();

  // Populate the selection and the dropdown with alternative options
  $.each(data, function (_key, _entry) {
    if (app.label.language.iso.code == this.LngIsoCode) {
      $("#nav-language").find("[name=selection]").text(this.LngIsoName);
    } else {
      $("#nav-language").find("[name=dropdown]").append(
        $("<a>", {
          href: "#",
          class: "dropdown-item",
          code: this.LngIsoCode,
          name: this.LngIsoName,
          text: this.LngIsoName
        }).get(0).outerHTML);
    }
  });

  //Add event to language dropdown menu
  $("#nav-language").find("[name=dropdown] a").once('click', function () {

    // Set the selected language
    app.label.language.iso.code = $(this).attr('code');
    app.label.language.iso.name = $(this).attr('name');

    // Update Document Language
    $("html").attr("lang", app.label.language.iso.code);

    Cookies.set(C_COOKIE_LANGUAGE, app.label.language, app.config.plugin.jscookie);

    // Prevent backbutton check
    app.plugin.backbutton.check = false;
    // Force page reload
    window.location.href = window.location.pathname;
  });
};

//#endregion