/*******************************************************************************
Application - Library 
*******************************************************************************/
var app = app || {};
app.library = app.library || {};

app.library.user = {};
app.library.user.data = {};
app.library.user.data.CcnUsername = null;
app.library.user.data.CcnDisplayName = null;
app.library.user.data.language = null;
app.library.user.modal = {};
app.library.user.modal.ajax = {};
app.library.user.modal.callback = {};
app.library.user.userchannels = [];

//#region User
/**
 * Get User data to read user details
 * @param {*} apiParam
 */
app.library.user.modal.ajax.read = function (apiParams) {
  // default params
  apiParams = apiParams || {};
  // Get data from API
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Read",
    apiParams,
    "app.library.user.modal.callback.read"
  );
};

/**
 * Populate User data to read user details
 * @param {*} data
 */
app.library.user.modal.callback.read = function (data) {
  if (data) {
    app.library.user.modal.callback.displayUser(data[0]);
  }

  // Handle no data
  else
    api.modal.information(app.label.static["api-ajax-nodata"]);
};

/**
 * Get User data to read current user details
 */
app.library.user.modal.ajax.readCurrent = function () {
  // Get data from API
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.ReadCurrent",
    null,
    "app.library.user.modal.callback.readCurrent"
  );
};

/**
 * Populate User data to read user details
 * @param {*} data
 */
app.library.user.modal.callback.readCurrent = function (data) {
  if (data) {
    app.library.user.modal.ajax.readUserChannels(data);
  }
  // Handle no data
  else
    api.modal.information(app.label.static["api-ajax-nodata"]);
};

app.library.user.modal.ajax.readUserChannels = function (data) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Subscription.Subscription_API.ChannelSubscriptionReadCurrent",
    {},
    "app.library.user.modal.callback.readUserChannels",
    data
  );
};

app.library.user.modal.callback.readUserChannels = function (response, userdata) {
  app.library.user.userchannels = response || [];
  app.library.user.modal.callback.displayUser(userdata);
};

app.library.user.modal.callback.displayUser = function (data) {
  if (!data.CcnAdFlag) {
    $("#modal-read-user").find("[name=reset-1fa-row]").show();
  }
  else {
    $("#modal-read-user").find("[name=reset-1fa-row]").hide();
    if (!app.config.security.adOpenAccess) {
      $("#modal-read-user").find("[name=reset-2fa-row]").hide();
    }
  }
  $("#modal-read-user").find("[name=ccn-username]").text(data.CcnUsername || "");
  $("#modal-read-user").find("[name=ccn-name]").text(data.CcnDisplayName || "");
  $("#modal-read-user").find("[name=prv-value]").text(app.label.datamodel.privilege[data.PrvValue]);
  $("#modal-read-user").find("[name=ccn-email]").html(app.library.html.email(data.CcnEmail));
  $("#modal-read-user").find("[name=type]").text(data.CcnAdFlag ? app.label.static["ad"] : app.label.static["local"]);

  if (data.PrvCode != C_APP_PRIVILEGE_MODERATOR) {
    //user is admin or power user, no groups, hide row using d-none
    $("#modal-read-user").find("[name=container-list]").hide();
  } else {
    app.library.user.modal.buildGroupList(data);
    $("#modal-read-user").find("[name=container-list]").show();
  }

  // Unbind change event
  $("#modal-read-user").find("[name=notification]").off("change");

  // If the user if the current one, then allow toggle notifications and subscriptions
  if (data.CcnUsername == app.library.user.data.CcnUsername) {
    //initiate toggle buttons
    $("#modal-read-user").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
      onlabel: app.label.static["true"],
      offlabel: app.label.static["false"],
      onstyle: "success text-light",
      offstyle: "warning text-dark",
      height: 38,
      style: "text-light",
      width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    });

    //Set state of bootstrapToggle button
    $("#modal-read-user").find("[name=notification]").bootstrapToggle(data.CcnNotificationFlag ? "on" : "off");
    // Enable the notification
    $("#modal-read-user").find("[name=notification]").bootstrapToggle("enable");
    // Bind change event on toggle notification
    $("#modal-read-user").find("[name=notification]").once("change", function () {
      app.library.user.modal.ajax.update();
    });
    // Hide the subscriptions row
    $("#modal-read-user").find("[name=subscription-row]").show();
  } else {
    //initiate toggle buttons
    $("#modal-read-user").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
      onlabel: app.label.static["true"],
      offlabel: app.label.static["false"],
      onstyle: "success text-light",
      offstyle: "warning text-dark",
      height: 38,
      style: "text-light",
      width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    });

    //Set state of bootstrapToggle button
    $("#modal-read-user").find("[name=notification]").bootstrapToggle(data.CcnNotificationFlag ? "on" : "off");
    // Disable the notification
    $("#modal-read-user").find("[name=notification]").bootstrapToggle("disable");
    // Hide the subscriptions row
    $("#modal-read-user").find("[name=subscription-row]").hide();
  }

  //subscriptions

  $("#modal-read-user").find("[name=subscription-toggle-wrapper]").empty();

  $.each(app.library.user.userchannels, function (index, value) {
    //see if user is subscribed to this channel
    var inputAttributes = {
      "type": "checkbox",
      "name": value.ChnCode,
      "checked": value.ChnSubscribed
    };


    $("#modal-read-user").find("[name=subscription-toggle-wrapper]").append(
      $("<div>", {
        "class": "checkbox pl-4",
        "html": $("<label>", {
          "html": $("<input>", inputAttributes).get(0).outerHTML + " " + value.ChnName
        }).get(0).outerHTML
      }));



    $("#modal-read-user").find("[name='" + value.ChnCode + "']").bootstrapToggle("destroy").bootstrapToggle({
      onlabel: app.label.static["on"],
      offlabel: app.label.static["off"],
      onstyle: "success text-light",
      offstyle: "warning text-dark",
      height: 38,
      style: "m-1 text-light",
      width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    }).once("change", function () {
      if ($(this).is(':checked')) {
        app.library.user.modal.ajax.channelSubscriptionCreate($(this).attr("name"));
      }
      else {
        app.library.user.modal.ajax.channelSubscriptionDelete($(this).attr("name"));
      }
    });
  });





  app.library.user.modal.ajax.channelSubscriptionCreate = function (chnCode) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.private,
      "PxStat.Subscription.Subscription_API.ChannelSubscriptionCreate",
      {
        "ChnCode": chnCode
      },
      "app.library.user.modal.callback.channelSubscriptionCreate"
    );

  };

  app.library.user.modal.callback.channelSubscriptionCreate = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
      api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
    }
  };

  app.library.user.modal.ajax.channelSubscriptionDelete = function (chnCode) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.private,
      "PxStat.Subscription.Subscription_API.ChannelSubscriptionDelete",
      {
        "ChnCode": chnCode
      },
      "app.library.user.modal.callback.channelSubscriptionDelete"
    );
  };

  app.library.user.modal.callback.channelSubscriptionDelete = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
      api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
    }
  };










  // Switch between the modals to avoid overlapping
  $("#modal-read-group").modal("hide");
  $("#modal-read-user").modal("show");

  $("#modal-read-user").find("[name=reset-1fa]").once("click", function () {
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-password-reset", [data.CcnEmail]), app.library.user.modal.ajax.initiateUpdate1FA, data.CcnEmail)
  });

  $("#modal-read-user").find("[name=reset-2fa]").once("click", function () {
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-2fa-reset", [data.CcnEmail]), app.library.user.modal.ajax.initiateUpdate2FA, data.CcnEmail)
  });
}

/**
 * Build Group List for the User
 * @param {*} data
 */
app.library.user.modal.buildGroupList = function (data) {
  //Flush the list
  $("#modal-read-user .list-group").empty(); //Do not delete.
  // Generate links for list of the Groups for the User
  $.each(data.UserGroups, function (key, value) {
    var userIconClass;
    if (value.GccApproveFlag) {
      //set class for icon depending on approver or not
      userIconClass = "fas fa-user-check text-success";
      //set title for Bootstrap tooltip
      userTooltipTitle = app.label.static["approver"];
    } else {
      userIconClass = "fas fa-user-edit text-orange";
      //set title for Bootstrap tooltip
      userTooltipTitle = app.label.static["editor"];
    }
    //Create Group Link.
    var linkGroup = $("<a>", {
      idn: value.GrpCode,
      href: "#",
      html: $("<i>", {
        "data-bs-toggle": "tooltip",
        "data-bs-placement": "top",
        "title": "", //userTooltipTitle,
        "data-original-title": userTooltipTitle,
        class: userIconClass
      }).get(0).outerHTML + " " + value.GrpName
    }).get(0);
    linkGroup.addEventListener("click", function (e) {
      e.preventDefault();
      app.library.group.modal.read(value.GrpCode);
    });
    var li = $("<li>", {
      class: "list-group-item"
    }).html(linkGroup);
    $("#modal-read-user .list-group").append(li);
    //Bootstrap tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();
  });
};

//#endregion

//#region Update
/**
 * Ajax call Update User
 * Save updated User via AJAX call.
 */
app.library.user.modal.ajax.update = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.UpdateCurrent",
    { CcnNotificationFlag: $("#modal-read-user").find("[name=notification]").prop('checked') },
    "app.library.user.modal.callback.update",
    null,
    "app.library.user.modal.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * Update User Callback
 * After AJAX call.
 * @param {*} data 
 */
app.library.user.modal.callback.update = function (data) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
  }
  // Handle exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Update User Callback on Error
 * After AJAX call.
 * @param {*} error 
 */
app.library.user.modal.callback.updateOnError = function (error) {
  // Hide modal
  $("#modal-read-user").modal("hide");
  // Force reload
  app.library.user.modal.ajax.readCurrent();
};
//#endregion


//#region reset 1FA
app.library.user.modal.ajax.initiateUpdate1FA = function (email) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Login_API.InitiateUpdate1FA_Current",
    {
      "LngIsoCode": app.label.language.iso.code
    },
    "app.library.user.modal.callback.initiateUpdate1FA",
    email
  );
}

app.library.user.modal.callback.initiateUpdate1FA = function (data, email) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-password-set", [email]))
    $("#modal-read-user").modal("hide");
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
}
//#endregion

//#region reset 1FA
app.library.user.modal.ajax.initiateUpdate2FA = function (email) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Login_API.InitiateUpdate2FA_Current",
    {
      "LngIsoCode": app.label.language.iso.code
    },
    "app.library.user.modal.callback.initiateUpdate2FA",
    email
  );
}

app.library.user.modal.callback.initiateUpdate2FA = function (data, email) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-2fa-set", [email]))
    $("#modal-read-user").modal("hide");
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
}
//#endregion


