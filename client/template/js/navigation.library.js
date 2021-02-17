
/*******************************************************************************
Custom JS application specific
****************************************************************************/
//#region Update Navigation User

//#region Namespaces 
app.navigation = {};

app.navigation.access = {};
app.navigation.access.ajax = {};
app.navigation.access.callback = {};

app.navigation.user = {};
app.navigation.user.prvCode = null;
app.navigation.user.isWindowsAccess = false;
app.navigation.user.isLoginAccess = false;
app.navigation.user.ajax = {};
app.navigation.user.callback = {};
app.navigation.user.validation = {};

app.navigation.language = {};
app.navigation.language.ajax = {};
app.navigation.language.callback = {};

//#endregion

//#region Access
app.navigation.access.ajax.ReadCurrentWindowsAccess = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.ReadCurrentWindowsAccess",
    null,
    "app.navigation.access.callback.ReadCurrentWindowsAccess_OnSuccess",
    null,
    "app.navigation.access.callback.ReadCurrentWindowsAccess_OnError",
    null,
    { async: false }
  );
};

/**
 * 
 * @param {*} data 
 */
app.navigation.access.callback.ReadCurrentWindowsAccess_OnSuccess = function (data) {
  if (data) {
    //Registered windows user within the network
    app.navigation.user.prvCode = data.PrvCode;
    app.navigation.user.isWindowsAccess = true;
  }
  else {
    //Un-registered windows user within the network
    app.navigation.user.isWindowsAccess = false;
  }
  app.navigation.access.callback.ReadCurrent();
};

app.navigation.access.callback.ReadCurrentWindowsAccess_OnError = function () {
  //check for logged in user
  app.navigation.access.ajax.ReadCurrentLoginAccess();
}

app.navigation.access.ajax.ReadCurrentLoginAccess = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.public,
    "PxStat.Security.Account_API.ReadCurrentLoginAccess",
    null,
    "app.navigation.access.callback.ReadCurrentLoginAccess_OnSuccess",
    null,
    "app.navigation.access.callback.ReadCurrentLoginAccess_OnError",
    null,
    { async: false }
  );
};

app.navigation.access.callback.ReadCurrentLoginAccess_OnSuccess = function (data) {
  if (data) {
    //logged in user
    app.navigation.user.prvCode = data.PrvCode;
    app.navigation.user.isLoginAccess = true;
  }
  else {
    //not logged in user
    app.navigation.user.isLoginAccess = false;
  }
  app.navigation.access.callback.ReadCurrent();
}

app.navigation.access.callback.ReadCurrentLoginAccess_OnError = function () {
  //attempted login from invalid logged in user
  //clean up session by logging out
  $('#modal-error').on('hide.bs.modal', function (event) {
    app.plugin.backbutton.check = false;
    api.cookie.session.end(app.config.url.api.jsonrpc.public, "PxStat.Security.Login_API.Logout");
  })
}

app.navigation.access.callback.ReadCurrent = function () {
  if (app.navigation.user.isWindowsAccess) {
    $("#nav-user").show();
    $("#nav-user-login").remove();
    $("#nav-user-logout").remove();
  }
  else if (app.navigation.user.isLoginAccess) {
    $("#nav-user").show();
    $("#nav-user-login").remove();
    $("#nav-user-logout").show();
    //overwrite private endpoint
    app.config.url.api.jsonrpc.private = app.config.url.api.jsonrpc.public;
    // Create the session cookie
    api.cookie.session.start(app.config.session.length, app.config.url.api.jsonrpc.public, "PxStat.Security.Login_API.Logout");
  }
  else {
    $("#nav-user").remove();
    $("#nav-user-login").show();
    $("#nav-user-logout").remove();
  }

  app.navigation.access.renderMenu(app.navigation.user.prvCode);
}

/**
 * Render the navigation
 * @param {*} data 
 */
app.navigation.access.renderMenu = function (PrvCode) {
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
app.navigation.setLayout = function (isDataEntity) {
  isDataEntity = isDataEntity || false;
  //empty and panel and navigation on all internal entities 
  $("#data-navigation").empty();
  $("#data-filter").empty();
  $("#panel").empty();
  $("#modal-entity").empty();
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

/**
 * Set the breadcrumb
 * breadcrumb[0] = text
 * breadcrumb[1] = goTo relative url
 * breadcrumb[2] = link to highlight
 * breadcrumb[4] = goTo params
 * breadcrumb[5] = goTo link
 */
app.navigation.setBreadcrumb = function (breadcrumb) {
  breadcrumb = breadcrumb || [];

  $("#breadcrumb-nav").find("[name=breadcrumb-list]").empty();

  //always start with home link
  var homeItem = $("#navigation-templates").find("[name=item]").clone();
  var homeLink = $("<a>", {
    "text": app.label.static["home"],
    "href": app.config.url.application
  }).get(0);

  homeItem.append(homeLink);

  homeLink.addEventListener("click", function (e) {
    e.preventDefault();
    api.content.goTo("entity/data/", "#nav-link-data");
  });

  $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(homeItem);

  $.each(breadcrumb, function (index, value) {
    if (value.length > 1) {
      var item = $("#navigation-templates").find("[name=item]").clone();
      var link = $("<a>", {
        "text": value[0],
        "href": value[5] || "#"
      }).get(0);
      item.append(link);
      link.addEventListener("click", function (e) {
        e.preventDefault();
        api.content.goTo(value[1], value[2], null, value[4]);
      });
      $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(item);
    }
    else {
      var item = $("#navigation-templates").find("[name=item]").clone();
      item.text(value[0])
      $("#breadcrumb-nav").find("[name=breadcrumb-list]").append(item);
    }
  });
};

/**
 * Set the page title
 */
app.navigation.setTitle = function (title) {
  title = title || null;
  // Set Document Title
  $("title").text(title ? title : app.config.title);
}

/**
 * Set the meta description
 */
app.navigation.setMetaDescription = function (description) {
  $("meta[name='description']").attr("content", description ? description : "");
}

/**
 * Check access against current user
 */
app.navigation.access.check = function (PrvCodeList) {
  PrvCodeList = PrvCodeList || [];
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.ReadCurrent",
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

  if (data) {
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
  if (data) {
    // Store for later user
    app.library.user.data.CcnUsername = data.CcnUsername;
    app.library.user.data.CcnDisplayName = data.CcnDisplayName;
    app.library.user.data.CcnPrvCode = data.PrvCode;
    // Set name on screen
    $("#nav-user").show();
    $("#nav-user").find("[name=name]").text(data.CcnDisplayName);

    // Show user details on click
    $("#nav-user").once("click", function (event) {
      app.library.user.modal.readCurrent();
    });
  } else {
    $("#nav-user").hide();
  }

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

    Cookies.set(C_COOKIE_LANGUAGE, app.label.language, app.config.plugin.jscookie.persistent);

    // Prevent backbutton check
    app.plugin.backbutton.check = false;
    // Force page reload
    window.location.href = window.location.pathname;
  });
};

//#endregion

