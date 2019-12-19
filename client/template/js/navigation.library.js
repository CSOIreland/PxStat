
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
    app.config.url.api.private,
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
 * @param {*} response 
 */
app.navigation.access.callback.set = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    // Anonymous or Not Registered user
    app.navigation.user.remove();
  } else if (response.data) {
    response.data = response.data[0];
    // Render the Navigation
    app.navigation.access.render(response.data.PrvCode)
  }
  else api.modal.exception(app.label.static["api-ajax-exception"]);
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
      // Configuration
      $("#nav-link-configuration").parent().show();
      // Keywords
      $("#nav-link-keyword").parent().show();
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
      $("#nav-link-tracing").remove();
      $("#nav-link-logging").remove();
      // Configuration
      $("#nav-link-configuration").parent().show();
      $("#nav-link-language").remove();
      $("#nav-link-format").remove();
      // Keywords
      $("#nav-link-keyword").parent().show();
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
      $("#nav-link-build").parent().show();
      // Manage
      $("#nav-link-manage").parent().remove();
      // Configuration
      $("#nav-link-configuration").parent().remove();
      // Keywords
      $("#nav-link-keyword").parent().remove();
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
      // Configuration
      $("#nav-link-configuration").parent().remove();
      // Keywords
      $("#nav-link-keyword").parent().remove();
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
      break;
  }
};


app.navigation.breadcrumb.set = function (breadcrumb) {
  breadcrumb = breadcrumb || [];
  //dataNavigation = dataNavigation || {};

  $("#breadcrumb-nav").find("[name=breadcrumb-list]").empty();
  var homeLink = $("#navigation-templates").find("[name=home]").clone();
  $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(homeLink);

  if ($.isArray(breadcrumb) && breadcrumb.length > 0) {
    $.each(breadcrumb, function (index, value) {
      var test = typeof value;
      if (typeof value === 'object' && value !== null) {
        var item = $("#navigation-templates").find("[name=link]").clone();
        var breadcrumbLink = $("<a>", {
          "text": value.text,
          "href": "#"
        }).get(0);
        breadcrumbLink.addEventListener("click", function (e) {
          e.preventDefault();
          api.content.goTo(value.goTo.pRelativeURL, value.goTo.pNav_link_SelectorToHighlight, null, value.goTo.pParams);
        });

        item.find("a").html(breadcrumbLink);
        $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(item);
      }
      else {
        var item = $("#navigation-templates").find("[name=item]").clone();
        item.html(String(value));
        $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(item);
      }
    });
  }



};
/**
 * Check access against current user
 */
app.navigation.access.check = function (PrvCodeList) {
  PrvCodeList = PrvCodeList || [];
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Security.Account_API.ReadCurrentAccess",
    null,
    "app.navigation.access.callback.check",
    PrvCodeList
  );
};
/**
 * Check access callback against current user
 * @param {*} response 
 * @param {*} PrvCodeList 
 */
app.navigation.access.callback.check = function (response, PrvCodeList) {
  PrvCodeList = PrvCodeList || [];
  // Add Administrator privilege by default;
  // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
  if ($.inArray(C_APP_PRIVILEGE_ADMINISTRATOR, PrvCodeList) == -1)
    PrvCodeList.push(C_APP_PRIVILEGE_ADMINISTRATOR);
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    // Force page reload
    window.location.href = window.location.pathname;
  } else if (response.data) {
    response.data = response.data[0];

    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(response.data.PrvCode, PrvCodeList) == -1) {
      // Force page reload
      window.location.href = window.location.pathname;
    }
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
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
    app.config.url.api.private,
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
 * @param {*} response 
 */
app.navigation.user.callback.read = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    app.navigation.user.remove();
  } else if (response.data) {
    response.data = response.data[0];

    // Store for later user
    app.library.user.data.CcnUsername = response.data.CcnUsername;
    app.library.user.data.CcnName = response.data.CcnName;
    app.library.user.data.CcnPrvCode = response.data.PrvCode;
    // Set name on screen
    $("#nav-user").find("[name=name]").text(response.data.CcnName);

    // Show user details on click
    $("#nav-user").on("click", function (event) {
      app.library.user.modal.readCurrent();
    });
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion


//#region language


/**
* Get languages
*/
app.navigation.language.ajax.read = function () {
  api.ajax.jsonrpc.request(app.config.url.api.public,
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
* @param {*} response 
*/
app.navigation.language.callback.read = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (response.data !== undefined) {
    // Empty dropdown
    $("#nav-language").find("[name=dropdown]").empty();

    // Populate the selection and the dropdown with alternative options
    $.each(response.data, function (_key, _entry) {
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

      Cookies.set(C_COOKIE_LANGUAGE, app.label.language, app.config.plugin.jscookie);

      // Force page reload
      window.location.href = window.location.pathname;
    });
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion