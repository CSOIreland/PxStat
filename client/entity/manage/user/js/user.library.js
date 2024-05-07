/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces definitions
// Create Namespace
app.user = {};
app.user.ajax = {};
app.user.modal = {};
app.user.callback = {};
app.user.validation = {};

//#endregion

/*******************************************************************************
Functions 
*******************************************************************************/
//#region Init

/**
 * Map API data to data model
 * @param {*} dataAPI 
 */
app.user.MapData = function (dataAPI) {
  $.each(dataAPI, function (i, item) {
    // Add ID and NAME to the list
    dataAPI[i].id = item.CcnUsername;
    dataAPI[i].text = item.CcnUsername + " (" + $.trim(item.CcnDisplayName) + ")";
  });
  return dataAPI;
};

/**
 * Populate Autocomplete search result of User
 * activeUserRecord - User record searched
 * @param {*} activeUserRecord
 */
app.user.UpdateFields = function (activeUserRecord) {
  //"This means it is only a partial match, you can either add a new item or take the active if you don't want new items"
  var activeUserRecord = activeUserRecord || null;
  if (activeUserRecord == null) {
    //Do not delete required for Member search functionality (select2)
    $("#user-modal-create-ad-user").find("[name=ccn-user-name-create]").text("");
    $("#user-modal-create-ad-user").find("[name=ccn-name-create]").text("");
    $("#user-modal-create-ad-user").find("[name=ccn-email-create]").text("");
    $(".serverValidationError .error").empty();
    $("#user-modal-create-ad-user").find("[name=user-select-create-search-error]").empty();
  }
  else {
    //Do not delete required for Member search functionality (select2)
    $("#user-modal-create-ad-user").find("[name=ccn-user-name-create]").text(activeUserRecord.CcnUsername);
    $("#user-modal-create-ad-user").find("[name=ccn-name-create]").text(activeUserRecord.CcnDisplayName);
    $("#user-modal-create-ad-user").find("[name=ccn-email-create]").text(activeUserRecord.CcnEmail);
    $(".serverValidationError .error").empty();
    $("#user-modal-create-ad-user").find("[name=user-select-create-search-error]").empty();
  }
};
//#endregion utility function

//#region Read User

/**
 * Call back from searchUser
 * @param  {} data
  */
app.user.callback.searchUser = function (data) {
  // Show modal
  $("#user-modal-create-ad-user").modal("show");

  // Load select2
  $("#user-modal-create-ad-user").find("[name=user-select-create-search]").empty().append($("<option>")).select2({
    dropdownParent: $('#user-modal-create-ad-user'),
    minimumInputLength: 0,
    allowClear: true,
    width: '100%',
    placeholder: app.label.static["start-typing"],
    data: app.user.MapData(data)
  });

  // Enable and Focus Search input
  $("#user-modal-create-ad-user").find("[name=user-select-create-search]").prop('disabled', false).focus();

  //Update User Search functionality
  $("#user-modal-create-ad-user").find("[name=user-select-create-search]").on('select2:select', function (e) {
    var selectedObject = e.params.data;
    if (selectedObject) {
      // Some item from your model is active!
      if (selectedObject.id.toLowerCase() == $("#user-modal-create-ad-user").find("[name=user-select-create-search]").val().toLowerCase()) {
        // This means the exact match is found. Use toLowerCase() if you want case insensitive match.
        app.user.UpdateFields(selectedObject);
      }
      else {
        app.user.UpdateFields(null);
      }
    } else {
      app.user.UpdateFields(null);
    }
  });
};

/**
 * Get data from API and Draw the Data Table for User. Ajax call.
 */
app.user.ajax.read = function () {
  // Get data from API and Draw the Data Table for User
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Read",
    null,
    "app.user.callback.read");
};

/**
 * Callback function when the User Read call is successful.
 * @param  {} data
  */
app.user.callback.read = function (data) {
  app.user.drawUserDataTable(data);
  $("#user-modal-create-ad-user").modal("hide");
};

/**
 * Draw Callback for Datatable
 */
app.user.drawCallback = function () {
  $('[data-bs-toggle="tooltip"]').tooltip();
  //Update User link click event
  $("#user-read-container table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    var apiParams = { CcnUsername: idn };
    // Get user data from api
    app.user.ajax.updateReadUser(apiParams);
  });
  //Delete User button click event. Passing function reference.
  $("#user-read-container table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.user.modal.delete);
}

/**
 * Create User DataTable
 * @param  {} data
 */
app.user.drawUserDataTable = function (data) {
  if ($.fn.DataTable.isDataTable("#user-read-container table")) {
    app.library.datatable.reDraw("#user-read-container table", data);
  } else {

    var localOptions = {
      data: data,
      createdRow: function (row, data, dataIndex) {
        if (!data.CcnDisplayName) {
          $(row).addClass('table-danger');
        }
      },
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            return app.library.html.link.edit({ idn: row.CcnUsername }, row.CcnUsername);
          }
        },
        {

          data: null,
          render: function (data, type, row) {
            return row.CcnDisplayName || $("<i>", {
              "class": "fas fa-user-slash",
              "data-original-title": app.label.static["missing"],
              "data-bs-toggle": "tooltip"
            }).get(0).outerHTML;
          }
        },
        {
          data: null,
          render: function (data, type, row) {
            return row.CcnEmail ? app.library.html.email(row.CcnEmail) : row.CcnDisplayName || $("<i>", {
              "class": "fas fa-user-slash",
              "data-original-title": app.label.static["missing"],
              "data-bs-toggle": "tooltip"
            }).get(0).outerHTML;
          }
        },
        {
          data: null,
          render: function (data, type, row) {
            return row.CcnAdFlag ? app.library.html.parseStaticLabel("ad") : app.library.html.parseStaticLabel("local");
          }
        },
        {
          data: null,
          type: "natural",
          render: function (data, type, row) {
            return app.library.html.boolean(row.CcnNotificationFlag, true, true);
          }
        },
        {
          data: null,
          type: "natural",
          render: function (data, type, row) {
            return app.library.html.boolean(!row.CcnLockedFlag, true, true);
          }
        },
        {
          data: null,
          render: function (data, type, row) {
            return app.label.datamodel.privilege[row.PrvValue];
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            if (row.CcnUsername === app.library.user.data.CcnUsername) {
              return app.library.html.deleteButton({ idn: row.CcnUsername }, true);
            }
            else {
              return app.library.html.deleteButton({ idn: row.CcnUsername }, false);
            }
          },
          "width": "1%"
        }
      ],
      drawCallback: function (settings) {
        app.user.drawCallback();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };

    $("#user-read-container table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.user.drawCallback();
    });


    //Add User button click event
    $("#user-read-container").find("[name=create-ad-user]").once("click", function (e) {
      e.preventDefault();
      app.user.modal.createActiveDirectory();
    });

    //Add User button click event
    $("#user-read-container").find("[name=create-local-user]").once("click", function (e) {
      e.preventDefault();
      app.user.modal.createLocal();
    });
  }
};

/**
 * get all active directory users
*/
app.user.ajax.getActiveDirectoryUser = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.ActiveDirectory_API.Read",
    null,
    "app.user.callback.searchUser");
};

//#endregion

//#region Create User

/**
 * Get role types fom api to populate role type drop down
 */
app.user.ajax.createRoleType = function (selector) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Privilege_API.Read",
    null,
    "app.user.callback.createRoleType",
    selector);
};

/**
 * Fill dropdown for privilege types
 * @param {*} data  
 */
app.user.callback.createRoleType = function (data, selector) {
  $(selector).find("[name=user-select-create-type]").empty().append($('<option>', {
    disabled: "disabled",
    selected: "selected",
    value: "",
    text: app.label.static["select-uppercase"]
  }));
  //Fill rest of select with role types
  $.each(data, function (_key, entry) {
    $(selector).find("[name=user-select-create-type]").append(new Option(app.label.datamodel.privilege[this.PrvValue], this.PrvCode));
  });
};

/**
 * Show modal to Create User
 */
app.user.modal.createActiveDirectory = function () {
  app.user.validation.createActiveDirectory();
  //Flush the Modal user-modal-create-ad-user. Do not delete. Required for User Select2 functionality.
  $("#user-modal-create-ad-user").find("[name=ccn-user-name-create]").empty();
  $("#user-modal-create-ad-user").find("[name=ccn-name-create]").empty();
  $("#user-modal-create-ad-user").find("[name=ccn-email-create]").empty();
  //initiate toggle buttons
  $("#user-modal-create-ad-user").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
    onlabel: app.label.static["true"],
    offlabel: app.label.static["false"],
    onstyle: "success text-light",
    offstyle: "warning text-dark",
    height: 38,
    style: "text-light",
    width: C_APP_TOGGLE_LENGTH //Depend on language translation.
  });

  //Create User Role Type
  app.user.ajax.createRoleType("#user-modal-create-ad-user");
  // Call the API to get AD user names  
  app.user.ajax.getActiveDirectoryUser();
};

/**
 *  Validation function for Create User
 */
app.user.validation.createActiveDirectory = function () {
  $("#user-modal-create-ad-user form").trigger("reset").validate({
    rules: {
      "ccn-user-name-create":
      {
        required: true,
      },
      "ccn-name-create":
      {
        required: true,
      },
      "ccn-email-create":
      {
        required: true,
      },
      "user-select-create-type":
      {
        required: true,
        notEqualTo: ""
      },
      "user-select-create-search":
      {
        required: true,
        notEqualTo: ""
      }
    },
    errorPlacement: function (error, element) {
      $("#user-modal-create-ad-user [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.user.ajax.createActiveDirectory();
    }
  }).resetForm();
};

/**
 * Create User Ajax call
 *
 */
app.user.ajax.createActiveDirectory = function () {
  //check for demo site
  if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
    api.modal.error(app.label.static["demo-site-restricted-access"]);
    return
  }
  // CAll Ajax to Create User. Do Redraw Data Table for Create User.
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.CreateAD",
    {
      "CcnUsername": $("#user-modal-create-ad-user").find("[name=ccn-user-name-create]").text(),
      "PrvCode": $("#user-modal-create-ad-user").find("[name=user-select-create-type] option:selected").val(),
      "CcnNotificationFlag": $("#user-modal-create-ad-user").find("[name=notification]").prop('checked')
    },
    "app.user.callback.createActiveDirectoryOnSuccess",
    $("#user-modal-create-ad-user").find("[name=user-select-create-search] option:selected").val(),
    "app.user.callback.createActiveDirectoryOnError",
    null,
    { async: false }
  );
};

/**
 * Create User to Table after Ajax success call
 * @param  {} data
 * @param  {} callbackParam
  */
app.user.callback.createActiveDirectoryOnSuccess = function (data, username) {
  //Redraw Data Table for Create User
  app.user.ajax.read();
  //Close modal
  $("#user-modal-create-ad-user").modal("hide");

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [username]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Create User to Table after Ajax success call
 * @param  {} error
  */
app.user.callback.createActiveDirectoryOnError = function (error) {
  //Redraw Data Table for Create User
  app.user.ajax.read();
  //Close modal
  $("#user-modal-create-ad-user").modal("hide");
};


//#region set up local user

/**
 * Show modal to Create User
 */
app.user.modal.createLocal = function () {
  app.user.validation.createLocal();
  $("#user-modal-create-local-user").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
    onlabel: app.label.static["true"],
    offlabel: app.label.static["false"],
    onstyle: "success text-light",
    offstyle: "warning text-dark",
    height: 38,
    style: "text-light",
    width: C_APP_TOGGLE_LENGTH //Depend on language translation.
  });

  //Create User Role Type
  app.user.ajax.createRoleType("#user-modal-create-local-user");

  $("#user-modal-create-local-user").modal("show");

};

/**
 *  Validation function for Create User
 */
app.user.validation.createLocal = function () {
  $("#user-modal-create-local-user form").trigger("reset").validate({
    rules: {
      "ccn-name-create":
      {
        required: true,
      },
      "ccn-email-create":
      {
        required: true,
        validEmailAddress: true
      },
      "user-select-create-type":
      {
        required: true,
        notEqualTo: ""
      }
    },
    errorPlacement: function (error, element) {
      $("#user-modal-create-local-user [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.user.ajax.createLocal();
    }
  }).resetForm();
};

/**
 * Create User Ajax call
 *
 */
app.user.ajax.createLocal = function () {
  // CAll Ajax to Create User. Do Redraw Data Table for Create User.
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.CreateLocal",
    {
      "CcnEmail": $("#user-modal-create-local-user").find("[name=ccn-email-create]").val(),
      "CcnDisplayName": $("#user-modal-create-local-user").find("[name=ccn-name-create]").val(),
      "PrvCode": $("#user-modal-create-local-user").find("[name=user-select-create-type] option:selected").val(),
      "CcnNotificationFlag": $("#user-modal-create-local-user").find("[name=notification]").prop('checked')
    },
    "app.user.callback.createLocalOnSuccess",
    $("#user-modal-create-local-user").find("[name=ccn-email-create]").val(),
    "app.user.callback.createLocalOnError",
    null,
    { async: false }
  );
};

/**
 * Create User to Table after Ajax success call
 * @param  {} data
 * @param  {} callbackParam
  */
app.user.callback.createLocalOnSuccess = function (data, username) {
  //Redraw Data Table for Create User
  app.user.ajax.read();
  //Close modal
  $("#user-modal-create-local-user").modal("hide");

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [username]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Create User to Table after Ajax success call
 * @param  {} error
  */
app.user.callback.createLocalOnError = function (error) {
  //Redraw Data Table for Create User
  app.user.ajax.read();
  //Close modal
  $("#user-modal-create-ad-user").modal("hide");
};

//#endregionset up local user


//#region reset 1fa and 2fa
app.user.ajax.initiateUpdate1FA = function (email) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Login_API.InitiateUpdate1FA",
    {
      "CcnEmail": email
    },
    "app.user.callback.initiateUpdate1FA",
    email);
}

app.user.callback.initiateUpdate1FA = function (data, email) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-password-set", [email]));
    $("#user-modal-update").modal("hide");
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
}

app.user.ajax.initiateUpdate2FA = function (email) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Login_API.InitiateUpdate2FA",
    {
      "CcnEmail": email
    },
    "app.user.callback.initiateUpdate2FA",
    email);
}

app.user.callback.initiateUpdate2FA = function (data, email) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-2fa-set", [email]))
    $("#user-modal-update").modal("hide");
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
}

//#endregion reset 1fa and 2fa

//#region Update User

/**
 *  Get role types fom api to populate role type drop down
 * @param  {} selectedPrvCode
 */
app.user.ajax.updateRoleType = function (selectedPrvCode) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Privilege_API.Read",
    null,
    "app.user.callback.updateRoleType",
    selectedPrvCode);
};

/**
 * Fill dropdown for privilege types
 * @param {*} data 
 * @param {*} selectedPrvCode 
 */
app.user.callback.updateRoleType = function (data, selectedPrvCode) {
  $("#user-modal-update").find("[name=user-select-update-type]").empty(); //clear previous list of user types
  //fill rest of select with role types
  $.each(data, function (_key, entry) {
    $("#user-modal-update").find("[name=user-select-update-type]").append(new Option(app.label.datamodel.privilege[this.PrvValue], this.PrvCode));
  });
  //set default to be selected
  $("#user-modal-update").find("[name=user-select-update-type]").val(selectedPrvCode);
};

/**
 * set up update modal
 * @param  {*} userRecord 
 */
app.user.modal.update = function (userRecord) {
  if (!userRecord.CcnAdFlag) {
    $("#user-modal-update").find("[name=update-1fa-row]").show();
  }
  else {
    $("#user-modal-update").find("[name=update-1fa-row").hide();
    if (!app.config.security.adOpenAccess) {
      $("#user-modal-update").find("[name=update-2fa-row").hide();
    }
  }

  //Flush the modal. Do not delete required for Member search functionality (select2)
  $("#user-modal-update").find("[name=ccn-user-name-update]").empty();
  $("#user-modal-update").find("[name=ccn-name-update]").empty();
  $("#user-modal-update").find("[name=ccn-email-update]").empty();
  //initiate toggle buttons
  $("#user-modal-update").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
    onlabel: app.label.static["true"],
    offlabel: app.label.static["false"],
    onstyle: "success text-light",
    offstyle: "warning text-dark",
    height: 38,
    style: "text-light",
    width: C_APP_TOGGLE_LENGTH //Depend on language translation.
  });

  $("#user-modal-update").find("[name=locked]").bootstrapToggle("destroy").bootstrapToggle({
    onlabel: app.label.static["true"],
    offlabel: app.label.static["false"],
    onstyle: "success text-light",
    offstyle: "warning text-dark",
    height: 38,
    style: "text-light",
    width: C_APP_TOGGLE_LENGTH //Depend on language translation.
  });
  // Add validation for Update User
  app.user.validation.update();
  $(".list-group").empty(); //empty group list from previous user viewed. DO not delete.
  $("#user-modal-update").find("[name=ccn-user-name-update]").text(userRecord.CcnUsername);
  $("#user-modal-update").find("[name=type]").text(userRecord.CcnAdFlag ? app.label.static["ad"] : app.label.static["local"]);
  $("#user-modal-update").find("[name=ccn-name-update]").text(userRecord.CcnDisplayName);
  var emailLink = app.library.html.email(userRecord.CcnEmail);
  $("#user-modal-update").find("[name=ccn-email-update]").html(emailLink);

  //Set state of bootstrapToggle button.
  if (userRecord.CcnNotificationFlag == true) {

    $("#user-modal-update").find("[name=notification]").bootstrapToggle('on');
  }
  else {

    $("#user-modal-update").find("[name=notification]").bootstrapToggle('off');
  }

  //Set state of bootstrapToggle button.
  if (userRecord.CcnLockedFlag == true) {

    $("#user-modal-update").find("[name=locked]").bootstrapToggle('off');
    $("#user-modal-update").find("[name=update-1fa]").prop("disabled", true);
    $("#user-modal-update").find("[name=update-2fa]").prop("disabled", true);
  }
  else {

    $("#user-modal-update").find("[name=locked]").bootstrapToggle('on');
    $("#user-modal-update").find("[name=update-1fa]").prop("disabled", false);
    $("#user-modal-update").find("[name=update-2fa]").prop("disabled", false);
  }


  // Update User role Type
  app.user.ajax.updateRoleType(userRecord.PrvCode);

  if (userRecord.PrvCode != C_APP_PRIVILEGE_MODERATOR) {
    //user is admin or power user, no groups, hide row using d-none
    $("#user-modal-update").find("[name=container-list]").hide();
  } else {
    app.user.modal.buildGroupList(userRecord);
    $("#user-modal-update").find("[name=container-list]").show();
  }

  $("#user-modal-update").modal("show");

  $("#user-modal-update").find("[name=update-1fa]").once("click", function (e) {
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-password-reset", [userRecord.CcnEmail]), function () {
      app.user.ajax.initiateUpdate1FA(userRecord.CcnEmail);
    });
  });

  $("#user-modal-update").find("[name=update-2fa]").once("click", function (e) {
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-2fa-reset", [userRecord.CcnEmail]), function () {
      app.user.ajax.initiateUpdate2FA(userRecord.CcnEmail);
    });
  })
};




/**
* 
* @param {*} data
*/
app.user.modal.buildGroupList = function (data) {
  //Flush the list
  $("#user-modal-update .list-group").empty(); // Do not delete.
  // Generate links for list of the Groups for the User
  $.each(data.UserGroups, function (key, value) {
    var userIconClass;
    var userTooltipTitle;
    if (value.GccApproveFlag) {
      //set class for icon depending on approver or not
      userIconClass = "fas fa-user-check text-success";
      //set title for Bootstrap tooltip
      userTooltipTitle = app.label.static["approver"];
    } else {
      //set class for icon depending on editor or not
      userIconClass = "fas fa-user-edit text-orange";
      //set title for Bootstrap tooltip
      userTooltipTitle = app.label.static["editor"];
    }
    //Create Group Link.
    var linkGroup = $("<a>", {
      idn: value.GrpCode,
      href: "#",
      html: $("<i>", {
        class: userIconClass,
        "data-bs-toggle": "tooltip",
        "data-bs-placement": "top",
        "title": "",//userTooltipTitle,
        "data-original-title": userTooltipTitle
      }).get(0).outerHTML + " " + value.GrpName
    }).get(0);
    //Add Group Link Click Event.
    linkGroup.addEventListener("click", function (e) {
      e.preventDefault();
      var GrpCode = $(this).attr("idn");
      $("#user-modal-update").modal("hide");
      $("#user-modal-update").one('hidden.bs.modal', function (e) {
        api.content.goTo("entity/manage/group/", "#nav-link-group", "#nav-link-manage", { "GrpCode": GrpCode });
      });
    });
    //Create Group Link's.
    var li = $("<li>", {
      class: "list-group-item"
    }).html(linkGroup);
    //Add Group Link's.
    $("#user-modal-update .list-group").append(li);
    //Bootstrap tooltip
    $('[data-bs-toggle="tooltip"]').tooltip();
  });
};

/**
 * Get selected user data from api. 
 * @param  {} apiParams
 */
app.user.ajax.updateReadUser = function (apiParams) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Read",
    apiParams,
    "app.user.callback.updateReadUser");
};

/**
 * Populate User data to Update Modal
 * @param {*} data 
 */
app.user.callback.updateReadUser = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    app.user.modal.update(data);
  } else {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Force reload
    app.user.ajax.read();
  }
};

/**
 * Validation function for Update User
 */
app.user.validation.update = function () {
  $("#user-modal-update form").trigger("reset").validate({
    rules: {
      "ccn-user-name-update":
      {
        required: true
      },
      "ccn-name-update":
      {
        required: true,
      },
      "ccn-email-update":
      {
        required: true,
      },
      "user-select-update-type":
      {
        required: true,
        notEqualTo: ""
      }
    },
    errorPlacement: function (error, element) {
      $("#user-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.user.ajax.update();
    }
  }).resetForm();
};

/**
 * Ajax call Update User
 * Save updated User via AJAX call.
 */
app.user.ajax.update = function () {
  //check for demo site
  if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
    api.modal.error(app.label.static["demo-site-restricted-access"]);
    return
  }
  //Get fields values at user-modal-update Modal
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Update",
    {
      "CcnUsername": $("#user-modal-update").find("[name=ccn-user-name-update]").text(),
      "PrvCode": $("#user-modal-update").find("[name=user-select-update-type] option:selected").val(),
      "CcnNotificationFlag": $("#user-modal-update").find("[name=notification]").prop('checked'),
      "CcnLockedFlag": !$("#user-modal-update").find("[name=locked]").prop('checked')
    },
    "app.user.callback.updateOnSuccess",
    $("#user-modal-update").find("[name=ccn-user-name-update]").text(),
    "app.user.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * Update User Callback
 * After AJAX call.
 * @param {*} data 
 * @param {*} callbackParam
 */
app.user.callback.updateOnSuccess = function (data, username) {
  $("#user-modal-update").modal("hide");
  // Force reload
  app.user.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    //Clear fields at user-modal-update Modal. Do not delete. Required for User Select2 functionality.
    $("#user-modal-update").find("[name=ccn-user-name-update]").text("");
    $("#user-modal-update").find("[name=ccn-name-update]").text("");
    $("#user-modal-update").find("[name=ccn-email-update]").text("");
    $("#user-modal-update").find("[name=user-select-update-type] option:selected").text("");
    $("#user-modal-update").find("[name=user-select-update-type] option:selected").val("");
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [username]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Update User Callback
 * After AJAX call.
 * @param {*} error 
 */
app.user.callback.updateOnError = function (error) {
  $("#user-modal-update").modal("hide");
  // Force reload
  app.user.ajax.read();
};
//#endregion
//#region Delete User
/**
 * Delete User 
 */
app.user.modal.delete = function () {
  var idn = $(this).attr("idn");
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [idn]), app.user.ajax.delete, idn);
};

/**
 * AJAX call to Delete a specific entry 
 * On AJAX success delete (Do reload table)
 * @param {*} idn
 */
app.user.ajax.delete = function (idn) {
  //check for demo site
  if (app.config.security.demo && app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
    api.modal.error(app.label.static["demo-site-restricted-access"]);
    return
  }
  // Call the API by passing the idn to delete User from DB
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Delete",
    { CcnUsername: idn },
    "app.user.callback.deleteOnSuccess",
    idn,
    "app.user.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
* Callback from server for Delete User
* @param {*} data
* @param {*} idn
*/
app.user.callback.deleteOnSuccess = function (data, idn) {
  //Redraw Data Table User with fresh data.
  app.user.ajax.read();

  if (data == C_API_AJAX_SUCCESS) {
    // Display Success Modal
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [idn]));
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* Callback from server for Delete User
* @param {*} error
*/
app.user.callback.deleteOnError = function (error) {
  //Redraw Data Table User with fresh data.
  app.user.ajax.read();
};

//#endregion

