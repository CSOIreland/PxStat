/*******************************************************************************
Application - Library 
*******************************************************************************/
var app = app || {};
app.library = app.library || {};

app.library.group = {};
app.library.group.modal = {};
app.library.group.modal.callback = {};

//#region Group
/**
 * Get User data to read user details
 * @param {*} GrpCode
 */
app.library.group.modal.read = function (GrpCode) {
  // Get data from API
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Security.Group_API.Read",
    { "GrpCode": GrpCode },
    "app.library.group.modal.callback.read"
  );
};

/**
 * Populate User data to read user details
 * @param {*} data
 */
app.library.group.modal.callback.read = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    $("#modal-read-group").find("[name=grp-code]").empty().text(data.GrpCode);
    $("#modal-read-group").find("[name=grp-name]").empty().text(data.GrpName);
    $("#modal-read-group").find("[name=grp-contact-name]").empty().text(data.GrpContactName);
    $("#modal-read-group").find("[name=grp-contact-email]").empty().html(app.library.html.email(data.GrpContactEmail));
    $("#modal-read-group").find("[name=grp-contact-phone]").empty().html(app.library.html.email(data.GrpContactPhone));

    // Get data from API
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.private,
      "PxStat.Security.GroupAccount_API.Read",
      { "GrpCode": data.GrpCode },
      "app.library.group.modal.callback.readAccountList"
    );
  }
  // Handle no data
  else
    api.modal.information(app.label.static["api-ajax-nodata"]);
};

/**
 * @param {*} data
 */
app.library.group.modal.callback.readAccountList = function (data) {
  //Flush the list
  $("#modal-read-group").find(".list-group").empty(); // Do not delete.
  // Generate links for list of the Users for the Group
  $.each(data, function (key, row) {
    var userIconClass;
    var userTooltipTitle;
    if (row.GccApproveFlag) {
      //set class for icon depending on approver or not
      userIconClass = "fas fa-user-check text-success";
      userTooltipTitle = app.label.static["approver"];
    } else {
      userIconClass = "fas fa-user-edit text-orange";
      userTooltipTitle = app.label.static["editor"];
    }
    //Create User Link.
    var linkUser = $("<a>", {
      idn: row.CcnUsername,
      href: "#",
      html: $("<i>", {
        "data-toggle": "tooltip",
        "data-placement": "top",
        "title": "", //userTooltipTitle,
        "data-original-title": userTooltipTitle,
        "class": userIconClass
      }).get(0).outerHTML + " " + row.CcnUsername
    }).get(0);
    linkUser.addEventListener("click", function (e) {
      e.preventDefault();
      app.library.user.modal.ajax.read({ CcnUsername: row.CcnUsername });
    });
    var li = $("<li>", {
      class: "list-group-item"
    }).html(linkUser);
    $("#modal-read-group .list-group").append(li);
  });
  //Bootstrap tooltip
  $('[data-toggle="tooltip"]').tooltip();
  // Switch between the modals to avoid overlapping
  $("#modal-read-user").modal("hide");
  $("#modal-read-group").modal("show");
};

//#endregion

