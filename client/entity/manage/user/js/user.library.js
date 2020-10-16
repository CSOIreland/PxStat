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
    dataAPI[i].text = item.CcnUsername + " (" + $.trim(item.CcnName) + ")";
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
    $("#user-modal-create").find("[name=ccn-user-name-create]").text("");
    $("#user-modal-create").find("[name=ccn-name-create]").text("");
    $("#user-modal-create").find("[name=ccn-email-create]").text("");
    $(".serverValidationError .error").empty();
    $("#user-modal-create").find("[name=user-select-create-search-error]").empty();
  }
  else {
    //Do not delete required for Member search functionality (select2)
    $("#user-modal-create").find("[name=ccn-user-name-create]").text(activeUserRecord.CcnUsername);
    $("#user-modal-create").find("[name=ccn-name-create]").text(activeUserRecord.CcnName);
    $("#user-modal-create").find("[name=ccn-email-create]").text(activeUserRecord.CcnEmail);
    $(".serverValidationError .error").empty();
    $("#user-modal-create").find("[name=user-select-create-search-error]").empty();
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
  $("#user-modal-create").modal("show");

  // Load select2
  $("#user-modal-create").find("[name=user-select-create-search]").empty().append($("<option>")).select2({
    dropdownParent: $('#user-modal-create'),
    minimumInputLength: 0,
    allowClear: true,
    width: '100%',
    placeholder: app.label.static["start-typing"],
    data: app.user.MapData(data)
  });

  // Enable and Focus Search input
  $("#user-modal-create").find("[name=user-select-create-search]").prop('disabled', false).focus();

  //Update User Search functionality
  $("#user-modal-create").find("[name=user-select-create-search]").on('select2:select', function (e) {
    var selectedObject = e.params.data;
    if (selectedObject) {
      // Some item from your model is active!
      if (selectedObject.id.toLowerCase() == $("#user-modal-create").find("[name=user-select-create-search]").val().toLowerCase()) {
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
  $("#user-modal-create").modal("hide");
};

/**
 * Draw Callback for Datatable
 */
app.user.drawCallback = function () {
  $('[data-toggle="tooltip"]').tooltip();
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
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            return app.library.html.link.edit({ idn: row.CcnUsername }, row.CcnUsername);
          }
        },
        { data: "CcnName" },
        {
          data: null,
          render: function (data, type, row) {
            return app.library.html.email(row.CcnEmail);
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
          render: function (data, type, row) {
            return app.label.datamodel.privilege[row.PrvValue];
          }
        },
        {
          data: null,
          type: "natural",
          render: function (data, type, row) {
            var valid = !row.CcnEmail || !row.CcnName ? false : true
            return app.library.html.boolean(valid, true, true);
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
    $("#user-read-container").find("[name=button-create]").once("click", function () {
      app.user.modal.create();
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
app.user.ajax.createRoleType = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Privilege_API.Read",
    null,
    "app.user.callback.createRoleType");
};

/**
 * Fill dropdown for privilege types
 * @param {*} data  
 */
app.user.callback.createRoleType = function (data) {
  $("#user-modal-create").find("[name=user-select-create-type]").empty().append($('<option>', {
    disabled: "disabled",
    selected: "selected",
    value: "",
    text: app.label.static["select-uppercase"]
  }));
  //Fill rest of select with role types
  $.each(data, function (_key, entry) {
    $("#user-modal-create").find("[name=user-select-create-type]").append(new Option(app.label.datamodel.privilege[this.PrvValue], this.PrvCode));
  });
};

/**
 * Show modal to Create User
 */
app.user.modal.create = function () {
  app.user.validation.create();
  //Flush the Modal user-modal-create. Do not delete. Required for User Select2 functionality.
  $("#user-modal-create").find("[name=ccn-user-name-create]").empty();
  $("#user-modal-create").find("[name=ccn-name-create]").empty();
  $("#user-modal-create").find("[name=ccn-email-create]").empty();
  //initiate toggle buttons
  $("#user-modal-create").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
    on: app.label.static["true"],
    off: app.label.static["false"],
    onstyle: "success",
    offstyle: "warning",
    width: C_APP_TOGGLE_LENGTH //Depend on language translation.
  });

  //Create User Role Type
  app.user.ajax.createRoleType();
  // Call the API to get AD user names  
  app.user.ajax.getActiveDirectoryUser();
};

/**
 *  Validation function for Create User
 */
app.user.validation.create = function () {
  $("#user-modal-create form").trigger("reset").validate({
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
      $("#user-modal-create [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.user.ajax.create();
    }
  }).resetForm();
};

/**
 * Create User Ajax call
 *
 */
app.user.ajax.create = function () {
  var ccnUsername = $("#user-modal-create").find("[name=ccn-user-name-create]").text();
  var ccnName = $("#user-modal-create").find("[name=ccn-name-create]").text();
  var ccnEmail = $("#user-modal-create").find("[name=ccn-email-create]").text();
  var userTypeText = $("#user-modal-create").find("[name=user-select-create-type] option:selected").text();
  var userTypeVal = $("#user-modal-create").find("[name=user-select-create-type] option:selected").val();
  var usernamecreate = $("#user-modal-create").find("[name=user-select-create-search] option:selected").val();
  var notifFlag = $("#user-modal-create").find("[name=notification]").prop('checked');
  var apiParams = {
    CcnUsername: ccnUsername,
    CcnName: ccnName,
    CcnEmail: ccnEmail,
    PrvValue: userTypeText,
    PrvCode: userTypeVal,
    CcnNotificationFlag: notifFlag
  };
  var callbackParam = {
    CcnUsername: usernamecreate,
  };
  // CAll Ajax to Create User. Do Redraw Data Table for Create User.
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Create",
    apiParams,
    "app.user.callback.createOnSuccess",
    callbackParam,
    "app.user.callback.createOnError",
    null,
    { async: false }
  );
};

/**
 * Create User to Table after Ajax success call
 * @param  {} data
 * @param  {} callbackParam
  */
app.user.callback.createOnSuccess = function (data, callbackParam) {
  //Redraw Data Table for Create User
  app.user.ajax.read();
  //Close modal
  $("#user-modal-create").modal("hide");

  if (data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.CcnUsername]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Create User to Table after Ajax success call
 * @param  {} error
  */
app.user.callback.createOnError = function (error) {
  //Redraw Data Table for Create User
  app.user.ajax.read();
  //Close modal
  $("#user-modal-create").modal("hide");
};

//#endregion

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
  //Flush the modal. Do not delete required for Member search functionality (select2)
  $("#user-modal-update").find("[name=ccn-user-name-update]").empty();
  $("#user-modal-update").find("[name=ccn-name-update]").empty();
  $("#user-modal-update").find("[name=ccn-email-update]").empty();
  //initiate toggle buttons
  $("#user-modal-update").find("[name=notification]").bootstrapToggle("destroy").bootstrapToggle({
    on: app.label.static["true"],
    off: app.label.static["false"],
    onstyle: "success",
    offstyle: "warning",
    width: C_APP_TOGGLE_LENGTH //Depend on language translation.
  });
  // Add validation for Update User
  app.user.validation.update();
  $(".list-group").empty(); //empty group list from previous user viewed. DO not delete.
  $("#user-modal-update").find("[name=ccn-user-name-update]").text(userRecord.CcnUsername);
  $("#user-modal-update").find("[name=ccn-name-update]").text(userRecord.CcnName);
  var emailLink = app.library.html.email(userRecord.CcnEmail);
  $("#user-modal-update").find("[name=ccn-email-update]").html(emailLink);

  //Set state of bootstrapToggle button.
  if (userRecord.CcnNotificationFlag == true) {

    $("#user-modal-update").find("[name=notification]").bootstrapToggle('on');
  }
  else {

    $("#user-modal-update").find("[name=notification]").bootstrapToggle('off');
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
        "data-toggle": "tooltip",
        "data-placement": "top",
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
    $('[data-toggle="tooltip"]').tooltip();
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
  //Get fields values at user-modal-update Modal
  var ccnUsername = $("#user-modal-update").find("[name=ccn-user-name-update]").text();
  var userType = $("#user-modal-update").find("[name=user-select-update-type] option:selected").val();
  var notifFlag = $("#user-modal-update").find("[name=notification]").prop('checked');
  var apiParams = {
    CcnUsername: ccnUsername,
    PrvCode: userType,
    CcnNotificationFlag: notifFlag
  };
  var callbackParam = {
    CcnUsername: ccnUsername,
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Account_API.Update",
    apiParams,
    "app.user.callback.updateOnSuccess",
    callbackParam,
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
app.user.callback.updateOnSuccess = function (data, callbackParam) {
  $("#user-modal-update").modal("hide");
  // Force reload
  app.user.ajax.read();

  if (data == C_APP_API_SUCCESS) {
    //Clear fields at user-modal-update Modal. Do not delete. Required for User Select2 functionality.
    $("#user-modal-update").find("[name=ccn-user-name-update]").text("");
    $("#user-modal-update").find("[name=ccn-name-update]").text("");
    $("#user-modal-update").find("[name=ccn-email-update]").text("");
    $("#user-modal-update").find("[name=user-select-update-type] option:selected").text("");
    $("#user-modal-update").find("[name=user-select-update-type] option:selected").val("");
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.CcnUsername]));
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

  // Get the indemnificator to delete
  var apiParams = { CcnUsername: idn };
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

  if (data == C_APP_API_SUCCESS) {
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

