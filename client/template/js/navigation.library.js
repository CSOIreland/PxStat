
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
app.navigation.user.isSubscriberAccess = false;
app.navigation.user.isLocked = true;
app.navigation.user.ajax = {};
app.navigation.user.callback = {};
app.navigation.user.validation = {};

app.navigation.language = {};
app.navigation.language.ajax = {};
app.navigation.language.callback = {};

app.navigation.user.savedTables = null;

app.navigation.user.savedTablesOpen = false;

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
    app.navigation.user.prvCode = data.CcnLockedFlag ? null : data.PrvCode;
    app.navigation.user.isWindowsAccess = true;
    app.navigation.user.isLocked = data.CcnLockedFlag;
    app.library.user.data.language = data.LngIsoCode;
    app.navigation.user.ajax.getSavedTables();
  }
  else {
    //Un-registered windows user within the network
    app.navigation.user.isWindowsAccess = false;
    app.navigation.user.isLocked = false;
  }
  app.auth.subscriberEmailActions();
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
    app.navigation.user.prvCode = data.CcnLockedFlag ? null : data.PrvCode;
    app.navigation.user.isLoginAccess = true;
    app.navigation.user.isLocked = data.CcnLockedFlag;
    app.library.user.data.language = data.LngIsoCode;
    app.navigation.user.ajax.getSavedTables();
  }
  else {
    //not logged in user
    app.navigation.user.isLoginAccess = false;
    app.navigation.user.isLocked = false;
  }
  app.auth.subscriberEmailActions();
  app.navigation.access.callback.ReadCurrent();
}

app.navigation.access.callback.ReadCurrentLoginAccess_OnError = function () {
  //attempted login from invalid logged in user
  //clean up session by logging out
  $('#modal-error').on('hide.bs.modal', function (event) {

    api.cookie.session.end(app.config.url.api.jsonrpc.public, "PxStat.Security.Login_API.Logout");
  });
}

app.navigation.access.ajax.readCurrentSubscriber = function (user) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.public,
    "PxStat.Subscription.Subscriber_API.ReadCurrent",
    {
      "Preference": {
        "LngIsoCode": app.label.language.iso.code
      },
      "Uid": user.uid,
      "AccessToken": user.accessToken
    },
    "app.navigation.access.callback.readCurrentSubscriber",
    user
  );
};

app.navigation.access.callback.readCurrentSubscriber = function (data, user) {
  //subscribers never locked, does not apply to them
  app.navigation.user.isLocked = false;
  if (data) {
    app.navigation.user.isSubscriberAccess = true;
    app.library.user.data.language = data.LngIsoCode;
    switch (app.auth.firebase.user.type) {
      case C_APP_FIREBASE_ID_PASSWORD:
        $("#navigation").find("[name=nav-subscriber-details]").find("[name=subscriber-icon]").removeClass().addClass("fas fa-user me-2");
        break;
      case C_APP_FIREBASE_ID_GOOGLE:
        $("#navigation").find("[name=nav-subscriber-details]").find("[name=subscriber-icon]").removeClass().addClass("fab fa-google me-2");
        break;
      case C_APP_FIREBASE_ID_FACEBOOK:
        $("#navigation").find("[name=nav-subscriber-details]").find("[name=subscriber-icon]").removeClass().addClass("fab fa-facebook me-2");
        break;
      case C_APP_FIREBASE_ID_TWITTER:
        $("#navigation").find("[name=nav-subscriber-details]").find("[name=subscriber-icon]").removeClass().addClass("fab fa-twitter me-2");
        break;
      case C_APP_FIREBASE_ID_GITHUB:
        $("#navigation").find("[name=nav-subscriber-details]").find("[name=subscriber-icon]").removeClass().addClass("fab fa-github me-2");
        break;
      default:
        break;
    }
    $("#navigation").find("[name=nav-subscriber-details]").find("[name=name]").html(data.DisplayName || data.CcnEmail);
    app.navigation.user.ajax.getSavedTables();
  }
  else {
    //we have user from firebase but not yet in PxStat, so must be signed up
    switch (app.auth.firebase.user.type) {
      case C_APP_FIREBASE_ID_PASSWORD:
        if (user.emailVerified) {
          app.auth.ajax.subscriberCreate(user);
        }
        break;
      case C_APP_FIREBASE_ID_GOOGLE:
      case C_APP_FIREBASE_ID_FACEBOOK:
      case C_APP_FIREBASE_ID_TWITTER:
      case C_APP_FIREBASE_ID_GITHUB:
        app.auth.ajax.subscriberCreate(user);
        break;
      default:
        break;
    }

  }
  app.navigation.access.callback.ReadCurrent();
};

app.navigation.access.callback.ReadCurrent = function () {
  if (app.navigation.user.isLocked) {
    $("#nav-user").remove();
    $("#nav-user-login").remove();
    $("#nav-user-logout").remove();
    $("#nav-user-locked").show();
  }

  else if (app.navigation.user.isWindowsAccess) {
    $("#nav-user").show();
    $("#nav-user-login").remove();
    $("#nav-user-logout").remove();
    $("#nav-user-locked").remove();
    $("#navigation").find("[name=nav-subscriber-details]").remove();
  }
  else if (app.navigation.user.isLoginAccess) {
    $("#nav-user").show();
    $("#nav-user-login").remove();
    $("#nav-user-logout").show();
    $("#nav-user-locked").remove();
    $("#navigation").find("[name=nav-subscriber-details]").remove();
    //overwrite private endpoint
    app.config.url.api.jsonrpc.private = app.config.url.api.jsonrpc.public;
    // Create the session cookie
    api.cookie.session.start(app.config.session.length, app.config.url.api.jsonrpc.public, "PxStat.Security.Login_API.Logout");
  }
  else if (app.navigation.user.isSubscriberAccess) {
    $("#nav-user-login").remove();
    $("#nav-user-logout").remove();
    $("#nav-user-locked").remove();
    $("#navigation").find("[name=nav-subscriber-details]").show();
    //overwrite private endpoint
    app.config.url.api.jsonrpc.private = app.config.url.api.jsonrpc.public;
  }
  else {
    $("#nav-user").remove();
    $("#nav-user-login").show();
    $("#nav-user-logout").remove();
    $("#nav-user-locked").remove();
  }

  app.navigation.access.renderMenu(app.navigation.user.prvCode);
  // Set language dropdown
  app.navigation.language.ajax.read(app.navigation.user.prvCode);
};

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
      //save queries
      if (app.config.plugin.subscriber.enabled) {
        $("#nav-link-saved-query").parent().show();
      };
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
      // Report
      $("#nav-link-report").parent().show();
      // Keywords
      $("#nav-link-keyword").parent().show();
      // System
      $("#nav-link-system").parent().show();
      break;
    case C_APP_PRIVILEGE_POWER_USER:
      // Display current user's information
      app.navigation.user.ajax.read();
      //save queries
      if (app.config.plugin.subscriber.enabled) {
        $("#nav-link-saved-query").parent().show();
      };
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
      // Report
      $("#nav-link-report").parent().show();
      // Keywords
      $("#nav-link-keyword").parent().show();
      // System
      $("#nav-link-system").parent().remove();
      break;
    case C_APP_PRIVILEGE_MODERATOR:
      // Display current user's information
      app.navigation.user.ajax.read();
      //save queries
      if (app.config.plugin.subscriber.enabled) {
        $("#nav-link-saved-query").parent().show();
      };
      //Dashboard
      $("#nav-link-dashboard").parent().show();
      // Releases
      $("#nav-link-release").parent().show();
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
      // Report
      $("#nav-link-report").parent().show();
      // Keywords
      $("#nav-link-keyword").parent().remove();
      // System
      $("#nav-link-system").parent().remove();
      break;
    default:
      if (app.navigation.user.isSubscriberAccess) {
        $("#nav-link-saved-query").parent().show();
      }
      else {
        $("#nav-link-saved-query").parent().hide();
      }
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
      // Report
      $("#nav-link-report").parent().remove();
      // Keywords
      $("#nav-link-keyword").parent().remove();
      // Keywords
      $("#nav-link-configuration").parent().remove();
      // System
      $("#nav-link-system").parent().remove();

      break;
  };

  // Get existing Cookie Language
  if (Cookies.getJSON(C_COOKIE_LANGUAGE)) {
    //check if user preferred language is different from cookie language
    if (app.library.user.data.language != Cookies.getJSON(C_COOKIE_LANGUAGE).iso.code) {
      $("#nav-language").find("[name=dropdown] a[code=" + app.library.user.data.language + "]").trigger("click");
    }
  }
  else {
    //no cookie, set language to user preference
    $("#nav-language").find("[name=dropdown] a[code=" + app.library.user.data.language + "]").trigger("click");
  }
};

/**
 * Set up page layout
 */
app.navigation.setLayout = function (isDataEntity) {
  isDataEntity = isDataEntity || false;
  //empty and panel and navigation on all internal entities 
  $("#data-navigation").find("[name=menu]").empty();
  $("#data-navigation").find("[name=favourite-tables]").hide();
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
      $("#data-navigation").find("[name=favourite-tables]").show();
      break;
    case false:
    default:
      //any entity except data entity
      $("#body").removeClass("order-last").addClass("order-first").removeClass("col-sm-9").addClass("col-sm-8");
      $("#sidebar").removeClass("col-sm-3").addClass("col-sm-4").removeClass("bg-sidebar");
      //hide responsive serach
      $("#data-search-row-responsive").hide();
      $("#data-navigation").find("[name=favourite-tables]").hide();
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
 * Set state for managing history
 * @param {*} state 
 */
app.navigation.setState = function (triggerSelector, state) {
  if (window._popState) {
    window._popState = false;
    return
  }
  state = state || {};
  state.triggerSelector = triggerSelector;
  history.pushState(state, null, window.location.href);

}

/**
 * Replace state for managing history
 * @param {*} state 
 */
app.navigation.replaceState = function (triggerSelector, state) {
  state = state || {};
  state.triggerSelector = triggerSelector;
  history.replaceState(state, null, window.location.href);

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
    "app.navigation.access.callback.success",
    PrvCodeList,
    "app.navigation.access.callback.error",
  );
};
/**
 * Check success access callback against current user
 * @param {*} data 
 * @param {*} PrvCodeList 
 */
app.navigation.access.callback.success = function (data, PrvCodeList) {
  PrvCodeList = PrvCodeList || [];
  // Add Administrator privilege by default;
  // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
  if ($.inArray(C_APP_PRIVILEGE_ADMINISTRATOR, PrvCodeList) == -1)
    PrvCodeList.push(C_APP_PRIVILEGE_ADMINISTRATOR);

  if (data) {
    // Wondering why == -1 ? Then go to https://api.jquery.com/jQuery.inArray/
    if ($.inArray(data.PrvCode, PrvCodeList) == -1) {
      // Prevent backbutton check

      // Force page reload
      window.location.href = window.location.pathname;
    }
  } else {
    // Prevent backbutton check

    // Force page reload
    window.location.href = window.location.pathname;
  }
};

/**
 * Check error access callback against current user
 * @param {*} data 
 * @param {*} PrvCodeList 
 */
app.navigation.access.callback.error = function () {
  // Prevent backbutton check

  // Force page reload
  window.location.href = window.location.pathname;
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
      app.library.user.modal.ajax.readCurrent(true);
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
app.navigation.language.ajax.read = function (prvCode) {
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.navigation.language.callback.read",
    prvCode,
    null,
    null,
    { async: false });
};

/**
* Fill dropdown for Language
* @param {*} data 
*/
app.navigation.language.callback.read = function (data, prvCode) {
  prvCode = prvCode || null;
  //if only one language, no need for language dropdown
  if (data.length == 1) {
    //add rounded corners to buttons
    $("#nav-user, #nav-user-login").addClass("rounded");
    $("#nav-language").remove();
    return
  }

  if ($("#nav-language").find("[name=dropdown] a[code=" + app.label.language.iso.code + "]").length
    && !Boolean($("#nav-language").find("[name=dropdown] a[code=" + app.label.language.iso.code + "]").attr("exists"))) {
    var clickedLanguageExists = false;
  }
  else {
    var clickedLanguageExists = true;
  }

  // Empty dropdown
  $("#nav-language").find("[name=dropdown]").empty();

  // Populate the selection and the dropdown with alternative options
  $.each(data, function (key, value) {

    if (app.label.language.iso.code == value.LngIsoCode) {

      if (!clickedLanguageExists) {
        $("#nav-language").find("[name=selection]").addClass("bg-danger")
      }

      $("#nav-language").find("[name=selection]").text(value.LngIsoName);
    } else {
      $("#nav-language").find("[name=dropdown]").append(
        $("<a>", {
          href: "#",
          class: "dropdown-item",
          code: value.LngIsoCode,
          name: value.LngIsoName,
          text: value.LngIsoName,
          style: "display:none;"
        }).get(0).outerHTML);
    }


    //check for client language
    $.getJSON(C_APP_URL_PXLANGUAGEPLUGINS + value.LngIsoCode + ".json")
      .done(function (json) {
        //fetch server language
        app.navigation.language.ajax.checkServer(
          {
            "lngIsoCode": value.LngIsoCode,
            "lngIsoName": value.LngIsoName,
            "prvCode": prvCode

          }
        )
      })
      .fail(function (jqxhr, textStatus, error) {
        //if power user or admin, 
        if (prvCode == C_APP_PRIVILEGE_ADMINISTRATOR || prvCode == C_APP_PRIVILEGE_POWER_USER) {
          $("#nav-language").find("[name=dropdown] a[code=" + value.LngIsoCode + "]").addClass("text-danger").show();
        }
        else {
          $("#nav-language").find("[name=dropdown] a[code=" + value.LngIsoCode + "]").remove();
        }
      });
  });


  //Add event to language dropdown menu
  $("#nav-language").find("[name=dropdown] a").once('click', function () {
    //overriding language object
    app.label.language.iso.code = $(this).attr('code');
    app.label.language.iso.name = $(this).attr('name');

    if ($(this).attr('exists') && $(this).attr('exists').length) {
      //set cookie language
      Cookies.setJSON(C_COOKIE_LANGUAGE, app.label.language, app.config.plugin.jscookie.persistent);
      //switch between user types
      if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {
        //local or AD user
        //update account via api
        app.navigation.language.ajax.updateUserLanguage(app.label.language.iso);
      }
      else if (app.auth.firebase.user.details) {
        //update account via api for subscriber user
        app.navigation.language.ajax.updateSubscriberLanguage(app.label.language.iso);
      }
      else {
        //anonymous user 
        // Prevent backbutton check

        // Force page reload
        window.location.href = window.location.pathname;
      }
    }
    else {
      app.navigation.language.ajax.read(prvCode);
    }
  });
};


app.navigation.language.ajax.checkServer = function (params) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.CheckIsLngIsoCode",
    {
      "LngIsoCode": params.lngIsoCode
    },
    "app.navigation.language.callback.checkServer",
    params,
    null,
    null,
    { async: false }
  );
};

app.navigation.language.callback.checkServer = function (data, params) {
  if (data) {
    $("#nav-language").find("[name=dropdown] a[code=" + params.lngIsoCode + "]").attr("exists", "true").show();
  }
  else {
    //if power user or admin, 
    if (params.prvCode == C_APP_PRIVILEGE_ADMINISTRATOR || params.prvCode == C_APP_PRIVILEGE_POWER_USER) {
      $("#nav-language").find("[name=dropdown] a[code=" + params.lngIsoCode + "]").addClass("text-danger").show();
      if (app.label.language.iso.code == value.LngIsoCode) {
        $("#nav-language").find("[name=selection]").addClass("text-danger");
      }
    }
    else {
      $("#nav-language").find("[name=dropdown] a[code=" + params.lngIsoCode + "]").remove();
    }
  }
};

app.navigation.language.ajax.updateUserLanguage = function (languageSelected) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.UpdateCurrent",
    {
      "LngIsoCode": languageSelected.code
    },
    "app.navigation.language.callback.updateUserLanguage",
    languageSelected
  );
};

app.navigation.language.callback.updateUserLanguage = function (data) {
  if (data == C_API_AJAX_SUCCESS) {
    // Prevent backbutton check

    // Force page reload
    window.location.href = window.location.pathname;
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

app.navigation.language.ajax.updateSubscriberLanguage = function (languageSelected) {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.private,
      "PxStat.Subscription.Subscriber_API.Update",
      {
        "LngIsoCode": languageSelected.code,
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken,
      },
      "app.navigation.language.callback.updateSubscriberLanguage",
      languageSelected
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.navigation.language.callback.updateSubscriberLanguage = function (data, languageSelected) {
  if (data == C_API_AJAX_SUCCESS) {
    // Prevent backbutton check

    // Force page reload
    window.location.href = window.location.pathname;
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

//#endregion

//#region saved tables

//this gets called if we have a subscriber, an AD user or a local user
app.navigation.user.ajax.getSavedTables = function () {
  if (app.navigation.user.isWindowsAccess) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.private,
      "PxStat.Subscription.Subscription_API.TableSubscriptionReadCurrent",
      {
        "LngIsoCode": app.label.language.iso.code
      },
      "app.navigation.user.callback.getSavedTables"
    );
  }
  else if (app.navigation.user.isLoginAccess) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscription_API.TableSubscriptionReadCurrent",
      {
        "LngIsoCode": app.label.language.iso.code
      },
      "app.navigation.user.callback.getSavedTables"
    );
  }
  else if (app.navigation.user.isSubscriberAccess) {
    app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
      api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Subscription.Subscription_API.TableSubscriptionReadCurrent",
        {
          "Uid": app.auth.firebase.user.details.uid,
          "AccessToken": accessToken,
          "LngIsoCode": app.label.language.iso.code
        },
        "app.navigation.user.callback.getSavedTables"
      );
    }).catch(tokenerror => {
      api.modal.error(tokenerror);
    });
  }
};

app.navigation.user.callback.getSavedTables = function (data) {
  app.navigation.user.savedTables = data || null;
  app.navigation.user.callback.drawSavedTables();
};

app.navigation.user.callback.drawSavedTables = function () {
  var savedTablesCard = $("#navigation-templates").find("[name=favourite-tables-card]").clone();
  savedTablesCard.find(".card-body").attr("id", "navigation-favourite-tables");
  savedTablesCard.find(".card-header a").attr("href", "#navigation-favourite-tables");

  $("#data-navigation").find("[name=favourite-tables]").empty().append(savedTablesCard);

  if (app.navigation.user.savedTablesOpen) {
    $("#navigation-favourite-tables").collapse("show");
  };
  $("#data-navigation").find("[name=favourite-tables]").find("[name=favourite-tables-card]").show();

  if ($.fn.dataTable.isDataTable("#data-navigation [name=favourite-tables] table")) {
    app.library.datatable.reDraw("#data-navigation [name=favourite-tables] table", app.navigation.user.savedTables);
  } else {
    var localOptions = {
      data: app.navigation.user.savedTables,
      "order": [[3, "desc"]],
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            var attributes = {
              "mtr-code": row.RsbTable, MtrCode: row.MtrCode, "data-bs-toggle": "tooltip",
              "text": row.RsbTable,
              "title": row.MtrTitle
            };
            return app.library.html.link.internal(attributes, row.RsbTable);
          }
        },
        {
          data: "MtrTitle",
          "visible": false,
          "searchable": true
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            if (row.RlsExceptionalFlag) {
              return $("<i>", {
                "class": "fas fa-exclamation-triangle text-warning",
                "data-bs-toggle": "tooltip",
                "data-bs-placement": "top",
                "title": app.label.static["exceptional-release"],
              }).get(0).outerHTML;
            }
            else {
              return null;
            }
          },
          "width": "1%"
        },
        {
          data: null,
          render: function (data, type, row) {
            return row.RlsDatetimeFrom ? moment(row.RlsDatetimeFrom, app.config.mask.datetime.ajax).format(app.config.mask.datetime.display) : "";

          }
        }],
      drawCallback: function (settings) {
        app.navigation.user.callback.drawCallbackSavedTables();
      },
      //Translate labels language
      language: app.label.plugin.datatable,
      "pagingType": "numbers"
    };
    $("#data-navigation [name=favourite-tables] table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.navigation.user.callback.drawCallbackSavedTables();
    });
  };

  $("#navigation-favourite-tables").on('hidden.bs.collapse', function (e) {
    $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-plus-circle");
    app.navigation.user.savedTablesOpen = false;
  });

  $("#navigation-favourite-tables").on('shown.bs.collapse', function (e) {
    $("#" + e.target.id).parent().find(".card-header i").removeClass().addClass("fas fa-minus-circle");
    app.navigation.user.savedTablesOpen = true;
  });
};

app.navigation.user.callback.drawCallbackSavedTables = function () {
  $("#data-navigation [name=favourite-tables] table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    $("#data-latest-releases").remove();
    $("#data-accordion-collection-api").hide();
    $("#data-navigation").find("[name=menu]").find(".navbar-collapse").collapse("hide");
    $("#data-search-row-desktop [name=search-results], #data-filter, #data-search-result-pagination [name=pagination], #data-search-result-pagination [name=pagination-toggle]").hide();
    $("#data-dataset-row").find("[name=back-to-select-results]").show();
    app.data.init(app.label.language.iso.code, $(this).attr("mtr-code"), null, $(this).attr("mtr-code"), false, true);
    app.data.dataset.draw();
  });
  $('[data-bs-toggle="tooltip"]').tooltip();
};
//#endregion

