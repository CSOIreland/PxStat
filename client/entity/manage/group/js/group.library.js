/*******************************************************************************
Custom JS application specific group.library.js
*******************************************************************************/
//#region Namespaces definitions
// Add Namespace Group Data Table
app.group = {};
app.group.ajax = {};
app.group.modal = {};
app.group.callback = {};
app.group.validation = {};

// Add Namespace Members Group Data Table
app.group.membergroup = {};
app.group.membergroup.ajax = {};
app.group.membergroup.modal = {};
app.group.membergroup.callback = {};
app.group.membergroup.callback.member = {};
app.group.membergroup.validation = {};
//#endregion


//#region Miscellaneous
/**
 * Draw extra information after you click on description link
 * @param  {*} data
 */
app.group.childRowContact = function (data) {
    //clone template from html not reuse dynamically
    var contactGrid = $("#group-templates").find(".group-div-contact").clone();
    contactGrid.find(".group-div-contact-name").html(data.GrpContactName);
    contactGrid.find(".group-div-contact-phone").html(data.GrpContactPhone);
    contactGrid.find(".group-div-contact-email").html(app.library.html.email(data.GrpContactEmail));
    return contactGrid.show().get(0).outerHTML;
};

/**
 * Toggle accordion icons when showing/hiding panels 
 */
$(".collapse").on("shown.bs.collapse", function () {
    $(this).parent()
        .find(".accordion-icon")
        .removeClass()
        .addClass('accordion-icon fas fa-minus-circle');
});
$(".collapse").on("hidden.bs.collapse", function () {
    $(this).parent()
        .find(".accordion-icon")
        .removeClass()
        .addClass('accordion-icon fas fa-plus-circle');
});

/**
 * Map API data to searchUser data model
 * @param {*} dataAPI 
 */
app.group.MapData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        // Add ID and NAME to the list
        dataAPI[i].id = item.CcnUsername;
        dataAPI[i].text = $.trim(item.CcnUsername) + " (" + $.trim(item.CcnDisplayName) + ")";
    });
    return dataAPI;
};

/**
 * Populate Autocomplete search result of Member
 * activeMemberRecord - Member record searched
 * @param {*} activeMemberRecord
 */
app.group.UpdateFields = function (activeMemberRecord) {
    //"This means it is only a partial match, you can either add a new item or take the active if you don't want new items"
    var activeMemberRecord = activeMemberRecord || null;
    if (activeMemberRecord == null) {
        //No selected Member (group-input-add-member-search select2)
        $("#group-modal-add-member").find("[name=group-input-add-member-username]").text("");
        $("#group-modal-add-member").find("[name=group-input-add-member-name]").text("");
        $(".serverValidationError .error").empty();
        $("#group-modal-add-member").find("[name=group-input-add-member-search-error-holder]").empty();
    }
    else {
        //Selected Member (group-input-add-member-search select2)
        $("#group-modal-add-member").find("[name=group-input-add-member-username]").text(activeMemberRecord.CcnUsername);
        $("#group-modal-add-member").find("[name=group-input-add-member-name]").text(activeMemberRecord.CcnDisplayName);
        $(".serverValidationError .error").empty();
        $("#group-modal-add-member").find("[name=group-input-add-member-search-error-holder]").empty();
    }
};

//#endregion

//#region Read Group
/**
   * Get data from API and Draw the Data Table for Group. Ajax call. 
   */
app.group.ajax.read = function () {
    // Get data from API and Draw the Data Table for Group 
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.Read",
        { CcnUsername: null },
        "app.group.callbackGroupRead");
};

/**
 * Callback function when the Group Read call is successful.
 * @param {*} data
 */
app.group.callbackGroupRead = function (data) {
    // Draw datatable
    app.group.drawDataTable(data);
};

/**
 * Draw Callback for Datatable
 */
app.group.drawCallbackGroup = function () {
    //Edit Group link click
    $("#group-read table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var idn = $(this).attr("idn"); //groupCode
        app.group.ajax.readGroup(idn);
    });
    //Delete Group button click. Passing function reference.
    $("#group-read table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.group.modal.delete);
    // Extra Info
    app.library.datatable.showExtraInfo('#group-read table', app.group.childRowContact);

    // Click event "internalLink"
    $("#group-read table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        var callbackParam = {
            idn: $(this).attr("idn"),
            "GrpValue": $(this).attr("grp-value")
        };
        // Ajax read 
        app.group.ajax.readMatrixByGroup(callbackParam);

        $("#group-matrix-modal").modal("show");

    });
};

/**
 * Ajax Read Release List
 * @param  {} callbackParam
 */
app.group.ajax.readMatrixByGroup = function (callbackParam) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Data.Matrix_API.ReadByGroup",
        {
            GrpCode: callbackParam.idn,
            LngIsoCode: app.label.language.iso.code
        },
        "app.group.callback.readMatrixByGroup",
        callbackParam);
};

/**
 * Handle data from Ajax
 * @param  {} data
 * @param  {} callbackParam
 */
app.group.callback.readMatrixByGroup = function (data, callbackParam) {
    app.group.drawMatrixByGroupDataTable(data, callbackParam);
};

app.group.drawMatrixByGroupDataTable = function (data, callbackParam) {
    $("#group-matrix-modal").find("[name=group-code]").text(callbackParam.idn);
    $("#group-matrix-modal").find("[name=group-value]").text(callbackParam.GrpValue);
    if ($.fn.dataTable.isDataTable("#group-matrix-modal table")) {
        app.library.datatable.reDraw("#group-matrix-modal table", data);
    } else {
        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        var attributes = { MtrCode: row.MtrCode };
                        return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
                    }
                }
            ],
            drawCallback: function (settings) {
                app.group.drawCallbackMatrix();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#group-matrix-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.group.drawCallbackMatrix();
        });
    }

}

app.group.drawCallbackMatrix = function () {
    $('[data-toggle="tooltip"]').tooltip();

    //Release version link click redirect to 
    $("#group-matrix-modal table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
        e.preventDefault();
        //Set the code
        var MtrCode = $(this).attr("MtrCode");

        $("#group-matrix-modal").modal("hide");

        //Wait for the modal to close
        $("#group-matrix-modal").on('hidden.bs.modal', function (e) {
            //Unbind the event for next call
            $("#group-matrix-modal").off('hidden.bs.modal');

            api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": MtrCode });
        })


    });
}


/**
 * draw table with data from api
 * @param {*} data 
 */
app.group.drawDataTable = function (data) {
    if ($.fn.dataTable.isDataTable("#group-read table")) {
        app.library.datatable.reDraw("#group-read table", data);
    } else {
        var localOptions = {
            // Add Row Index to feed the ExtraInfo modal 
            createdRow: function (row, dataRow, dataIndex) {
                $(row).attr(C_APP_DATATABLE_ROW_INDEX, dataIndex);
            },
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ idn: row.GrpCode }, row.GrpCode);
                    }
                },
                {
                    data: "GrpName"
                },
                {
                    data: null,
                    defaultContent: '',
                    sorting: false,
                    searchable: false,
                    "render": function (data, type, row, meta) {
                        return $("<a>", {
                            href: "#",
                            name: C_APP_DATATABLE_EXTRA_INFO_LINK,
                            "idn": meta.row,
                            html:
                                $("<i>", {
                                    "class": "fas fa-info-circle text-info"
                                }).get(0).outerHTML + " " + app.label.static["information"]
                        }).get(0).outerHTML;
                    }
                },
                {
                    data: "GrpContactName",
                    "visible": false,
                    "searchable": true
                },
                {
                    data: "GrpContactPhone",
                    "visible": false,
                    "searchable": true
                },
                {
                    data: "GrpContactEmail",
                    "visible": false,
                    "searchable": true
                },
                { data: "CcnCount" },
                {
                    data: null,
                    render: function (_data, _type, row) {
                        return app.library.html.link.internal({ idn: row.GrpCode, "grp-value": row.GrpName }, String(row.MtrCount));
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.GrpCode }, row.CcnCount > 0 || row.MtrCount > 0 ? true : false);
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.group.drawCallbackGroup();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#group-read table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.group.drawCallbackGroup();
        });
        // Display Modal Create/Add Group on Create Add Group button
        $("#group-read").find("[name=button-create]").once("click", app.group.modal.create);
    }
};

//#endregion 

//#region Create Group
/**
 * Display Modal Add Add Group on Create Add Group button
 */
app.group.modal.create = function () {
    // Validate Create/Add Group - Modal - Add Group (Group Table)
    app.group.validation.create();
    // Create Group Modal show
    $("#group-modal-create-group").modal("show");
};

/**
 * Validate Create Group - Modal - (Group Table)
 */
app.group.validation.create = function () {
    //Sanitizing input GrpCode (allUppper, allLower, onlyAlpha, onlyNum)
    $("#group-modal-create-group form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "grp-code":
            {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHANUMERIC);
                    $(this).val(value);
                    return value;
                }
            },
            "grp-name": {
                required: true
            },
            "grp-contact-phone":
            {
                validPhoneNumber: true
            },
            "grp-contact-email":
            {
                required: true,
                validEmailAddress: true
            }
        },
        errorPlacement: function (error, element) {
            $("#group-modal-create-group [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.group.ajax.create();
        }
    }).resetForm(); // Validate Add Group - Modal - Add Group
};

/**
 * Crete Add Group to (Group Data Table)  
 */
app.group.ajax.create = function () {
    var grpCode = $("#group-modal-create-group").find("[name=grp-code]").val();
    var grpName = $("#group-modal-create-group").find("[name=grp-name]").val();
    var grpContactName = $("#group-modal-create-group").find("[name=grp-contact-name]").val();
    var grpContactPhone = $("#group-modal-create-group").find("[name=grp-contact-phone]").val();
    var grpContactEmail = $("#group-modal-create-group").find("[name=grp-contact-email]").val();
    // Create Group Modal hide
    $("#group-modal-create-group").modal("hide");
    var apiParams = {
        GrpCode: grpCode,
        GrpName: grpName,
        GrpContactName: grpContactName,
        GrpContactPhone: grpContactPhone,
        GrpContactEmail: grpContactEmail,
    };
    var callbackParam = {
        GrpCode: grpCode
    };
    // CAll Ajax to Create/Add User. Do Redraw Data Table for Create/Add User.
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.Create",
        apiParams,
        "app.group.callback.createOnSuccess",
        callbackParam,
        "app.group.callback.createOnError",
        null,
        { async: false }
    );
};

/**
 * Create/Add Group to Table after Ajax success call
 * @param {*} data
 * @param {*} callbackParam
 */
app.group.callback.createOnSuccess = function (data, callbackParam) {
    //Redraw Data Table for Create/Add User
    app.group.ajax.read();
    //hide the accordion
    $("#accordion-group").hide();
    //Close modal
    $("#group-modal-create-group").modal("hide");

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.GrpCode]));
    } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Create/Add Group to Table after Ajax success call
 * @param {*} error
 */
app.group.callback.createOnError = function (error) {
    //Redraw Data Table for Create/Add User
    app.group.ajax.read();
    //hide the accordion
    $("#accordion-group").hide();
    //Close modal
    $("#group-modal-create-group").modal("hide");
};
//#endregion 

//#region Delete Group

/**
 * // Display confirmation Modal on DELETE button click (Group Table)
 * @param {*} e
 */
app.group.modal.delete = function (e) {
    var groupToDelete = $(this).attr("idn"); //idn
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [groupToDelete]), app.group.ajax.delete, groupToDelete);
};

/**
 * Callback to Delete a specific entry at Group Table
 * @param {*} groupToDelete
 */
app.group.ajax.delete = function (groupToDelete) {
    // Call the API by passing the indemnificator to delete
    var apiParams = {
        GrpCode: groupToDelete
    };
    var callbackParam = {
        GrpCode: groupToDelete
    };

    // Call the API by passing the idn to delete Group from DB
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.Delete",
        apiParams,
        "app.group.callback.deleteOnSuccess",
        callbackParam,
        "app.group.callback.deleteOnError",
        null,
        { async: false }
    );
};

/**
* Callback from server for Delete Group
* @param {*} data
* @param {*} callbackParam
*/
app.group.callback.deleteOnSuccess = function (data, callbackParam) {
    //Redraw Data Table Group with fresh data.
    app.group.ajax.read();
    // Hide the group in case it was open
    $("#accordion-group").hide();

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [callbackParam.GrpCode]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
* Callback from server for Delete Group
* @param {*} error
*/
app.group.callback.deleteOnError = function (error) {
    //Redraw Data Table Group with fresh data.
    app.group.ajax.read();
    // Hide the group in case it was open
    $("#accordion-group").hide();
};

//#endregion Delete Group

//#region Read Members

/**
 * Get data from API and Draw the Data Table for Members Group
 * @param {*} groupCode 
 */
app.group.membergroup.ajax.read = function (groupCode) {
    // Get data from API and Draw the Data Table for Members Group. Group.GrpCode = membersgroups.GrpCode 
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private, "PxStat.Security.GroupAccount_API.Read", { GrpCode: groupCode }, "app.group.callbackMembersGroupRead", groupCode);
};

/**
 * * Callback function when the Members Group Read call is successful.
 * @param {*} data 
 * @param {*} groupCode 
 */
app.group.callbackMembersGroupRead = function (data, groupCode) {
    app.group.drawDataTableMembersGroup(data, groupCode);
};

/**
 * Draw Callback for Datatable
 */
app.group.drawCallbackGroupMember = function () {
    $('[data-toggle="tooltip"]').tooltip();
    // Edit Members Group link click 
    //Display  the modal "group-modal-update-group-member" on row click
    $("#group-members-read table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        var ccnUsername = $(this).attr("idn");
        var grpCode = $(this).attr("grp-code");
        var apiParams = {
            CcnUsername: ccnUsername,
            GrpCode: grpCode
        };
        var callbackParam = {
            CcnUsername: ccnUsername,
            GrpCode: grpCode
        };
        app.group.membergroup.readUpdate(apiParams, callbackParam);
    });
    //Delete Member Group button click
    // Display confirmation Modal on DELETE button click
    $("#group-members-read table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.group.membergroup.modal.delete);

}

/**
 * Create Members - Group DataTable and get JASON data
 * @param {*} data 
 * @param {*} groupCode 
 */
app.group.drawDataTableMembersGroup = function (data, groupCode) {
    if ($.fn.dataTable.isDataTable("#group-members-read table")) {
        app.library.datatable.reDraw("#group-members-read table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ idn: row.CcnUsername, "grp-code": row.GrpCode }, row.CcnUsername);
                    }
                },
                { data: "CcnDisplayName" },
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.groupRole(JSON.parse(row.GccApproveFlag));
                    }
                },
                {
                    data: null,
                    sorting: false,
                    searchable: false,
                    render: function (data, type, row) {
                        return app.library.html.deleteButton({ idn: row.CcnUsername, "grp-code": row.GrpCode }, false);
                    },
                    "width": "1%"
                }
            ],
            drawCallback: function (settings) {
                app.group.drawCallbackGroupMember();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#group-members-read table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.group.drawCallbackGroupMember();
        });
        //set currentGroup attribute on button to know which group to add member to
        $("#group-members-read").find("[name=button-create]").attr("current-group", groupCode);
        //Add Member Group button click
        // Display modal - member - add - to - group : Click at Add Member button
        $("#group-members-read").find("[name=button-create]").once("click", function () {
            app.group.membergroup.modal.create();
        });
    }
};// Create Members - Group DataTable and get JASON data

//#endregion 

//#region Update Group

/**
* 
* @param {*} groupCode
*/
app.group.ajax.readGroup = function (groupCode) {
    var apiParams = {
        GrpCode: groupCode
    };
    var callbackParam = {
        GrpCode: groupCode
    };
    // Get data from API and Draw the Data Table for Reason. Populate date to the modal "reason-modal-update"
    api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private, "PxStat.Security.Group_API.Read", apiParams, "app.group.callback.readgroup", callbackParam);
};

/**
 * Populate Group data for Update form
 * @param {*} data 
 */
app.group.callback.readgroup = function (data) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];

        app.group.readgroup(data);
    } else {
        // Handle no data
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Force reload
        app.group.ajax.read();
        $("#accordion-group").hide();
    }
};

/**
 * Display the group-table-update-form  
 * @param  {} groupRecord 
 */
app.group.readgroup = function (groupRecord) {
    //Reload members group table with AJAX  call for GrpCode parameter value
    app.group.membergroup.ajax.read(groupRecord.GrpCode); // idn => GrpCode
    $("#group-members-read").find("[name=button-create]").attr("current-group", groupRecord.GrpCode);
    $("#group-form-update-group").find("[name=reset]").attr("current-group", groupRecord.GrpCode);
    //var groupformeditlabel = groupRecord.GrpCode + " - Edit";
    $("#group-form-update-group").find("[name=group-span-edit-group-title]").text(groupRecord.GrpCode + " - " + app.label.static["edit"]);
    //var membersgrouplabel = groupRecord.GrpCode + " - Members";
    $("#accordion-group").find("[name=group-span-members-title]").text(groupRecord.GrpCode + " - " + app.label.static["members"]);
    // Validate Edit Group at form  "group-table-edit-form"
    app.group.validation.update();
    $("#group-form-update-group").find("[name=idn]").val(groupRecord.GrpCode);
    $("#group-form-update-group").find("[name=grp-code]").val(groupRecord.GrpCode);
    $("#group-form-update-group").find("[name=grp-name]").val(groupRecord.GrpName);
    $("#group-form-update-group").find("[name=grp-contact-name]").val(groupRecord.GrpContactName);
    $("#group-form-update-group").find("[name=grp-contact-phone]").val(groupRecord.GrpContactPhone);
    $("#group-form-update-group").find("[name=grp-contact-email]").val(groupRecord.GrpContactEmail);

    $("#accordion-group").hide().fadeIn();
    //Expand the "group-table-update-form"
    //$("#collapse-one-group-table-update-form").collapse('show');
    //Expand the "members-group-table"
    $("#collapse-two-members-group-table").collapse('show');
    //Scroll to "name" field.
    $('html, body').animate({ scrollTop: $('#accordion-group #headingTwo').offset().top }, 1000);
    $("#group-form-update-group").find("[name=reset]").once("click", function (event) {
        app.group.ajax.readGroup($("#group-form-update-group").find("[name=reset]").attr("current-group"));
        $("#collapse-one-group-table-update-form").collapse('hide');
    });
};

/**
 * Validate Edit Group at Form  "group-table-update-form"
 */
app.group.validation.update = function () {
    //Sanitizing input GrpCode (allUppper, allLower, onlyAlpha, onlyNum) 
    $("#group-form-update-group form").trigger("reset").onSanitiseForm().validate({
        onkeyup: function (element) {
            this.element(element);
        },
        rules: {
            "grp-code":
            {
                required: true,
                normalizer: function (value) {
                    value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHANUMERIC);
                    $(this).val(value);
                    return value;
                }
            },
            "grp-name": {
                required: true
            },
            "grp-contact-phone":
            {
                validPhoneNumber: true
            },
            "grp-contact-email":
            {
                required: true,
                validEmailAddress: true
            },
        },
        errorPlacement: function (error, element) {
            $("#group-form-update-group [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.group.ajax.update();
        }
    }).resetForm();
};

/**
 * Update the Group at form 
 */
app.group.ajax.update = function () {
    //New code or same as "currentGroup"
    var grpCodeNew = $("#group-form-update-group").find("[name=grp-code]").val();
    //Old code (current)
    var currentGroup = $("#group-form-update-group").find("[name=idn]").val();
    var grpName = $("#group-form-update-group").find("[name=grp-name]").val();
    var grpContactName = $("#group-form-update-group").find("[name=grp-contact-name]").val();
    var grpContactPhone = $("#group-form-update-group").find("[name=grp-contact-phone]").val();
    var grpContactEmail = $("#group-form-update-group").find("[name=grp-contact-email]").val();
    var apiParams = {
        GrpCodeNew: grpCodeNew,
        GrpCodeOld: currentGroup,
        GrpName: grpName,
        GrpContactName: grpContactName,
        GrpContactPhone: grpContactPhone,
        GrpContactEmail: grpContactEmail
    };
    var callbackParam = {
        GrpCodeNew: grpCodeNew,
        GrpCodeOld: currentGroup,
    };
    //Ajax call to add Add Group 
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Group_API.Update",
        apiParams,
        "app.group.callback.updateOnSuccess",
        callbackParam,
        "app.group.callback.updateOnError",
        null,
        { async: false }
    );
};

/**
 * Update Group Callback
 * After AJAX call.
 * @param  {} data 
 * @param  {} callbackParam 
 */
app.group.callback.updateOnSuccess = function (data, callbackParam) {
    //Redraw  Data Table
    app.group.ajax.read();
    //hide the accordion
    $("#accordion-group").hide();

    if (data == C_API_AJAX_SUCCESS) {
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.GrpCodeOld]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
 * Update Group Callback
 * After AJAX call.
 * @param  {} error
 */
app.group.callback.updateOnError = function (error) {
    //Redraw  Data Table
    app.group.ajax.read();
    //hide the accordion
    $("#accordion-group").hide();
};

//#endregion

//#region Delete members 
/**
 * Display confirmation Modal on DELETE button click
 */
app.group.membergroup.modal.delete = function () {
    var idn = $(this).attr("idn");

    var deleteObj = {
        CcnUsername: idn,
        GrpCode: $(this).attr("grp-code")
    };
    api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [idn]), app.group.membergroup.ajax.delete, deleteObj);
};

/**
 * Callback to Delete a specific entry -  Members Group Table
 * @param {*} rowToDelete
 */
app.group.membergroup.ajax.delete = function (rowToDelete) {
    // Get the indemnificator to delete
    var apiParams = {
        CcnUsername: rowToDelete.CcnUsername,
        GrpCode: rowToDelete.GrpCode
    };
    var callbackParam = {
        CcnUsername: rowToDelete.CcnUsername,
        GrpCode: rowToDelete.GrpCode
    };
    // Call the API by passing the idn to delete
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.GroupAccount_API.Delete",
        apiParams,
        "app.group.membergroup.callback.deleteOnSuccess",
        callbackParam,
        "app.group.membergroup.callback.deleteOnError",
        callbackParam,
        { async: false }
    );
};

/**
 * Callback from server for Delete Member of group (Members Group)
 * @param {*} data
 * @param {*} callbackParam
 */
app.group.membergroup.callback.deleteOnSuccess = function (data, callbackParam) {
    app.group.ajax.read(); //Number of member change for Group.    //Redraw Data Table Members of Group with fresh data.
    app.group.membergroup.ajax.read(callbackParam.GrpCode);

    if (data == C_API_AJAX_SUCCESS) {
        // Display Success Modal
        api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [callbackParam.CcnUsername]));
    }
    // Handle Exception
    else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Callback from server for Delete Member of group (Members Group)
 * @param {*} error
 * @param {*} callbackParam
 */
app.group.membergroup.callback.deleteOnError = function (error, callbackParam) {
    app.group.ajax.read(); //Number of member change for Group.    //Redraw Data Table Members of Group with fresh data.
    app.group.membergroup.ajax.read(callbackParam.GrpCode);
};


//#endregion

//#region Update Members 

/**
 * Member Group read User for Update 
 * @param {*} apiParams
 * @param {*} callbackParam
 */
app.group.membergroup.readUpdate = function (apiParams, callbackParam) {
    // Get data from API and Draw the Data Table for Reason. Populate date to the modal "reason-modal-update"
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.GroupAccount_API.Read",
        apiParams,
        "app.group.membergroup.callback.readUpdate",
        callbackParam);
};

/**
 * Update Member/User fields.
 * @param {*} data 
 * @param {*} callbackParam
  */
app.group.membergroup.callback.readUpdate = function (data, callbackParam) {
    if (data && Array.isArray(data) && data.length) {
        data = data[0];

        app.group.membergroup.modal.update(data);
    } else {
        api.modal.information(app.label.static["api-ajax-nodata"]);
        // Force reload
        app.group.membergroup.ajax.read(callbackParam.GrpCode);
    }
};

/**
 * Display  the modal "group-modal-update-group-member" on row click
 * @param {*} userRecord 
 */
app.group.membergroup.modal.update = function (userRecord) {
    //Flush the modal
    $("#group-modal-update-group-member").find("[name=gcc-approve-flag]").prop('checked', false);
    // Validate Update Edit Member Group (Modal)
    app.group.membergroup.validation.update();
    // Populate data to the modal "group-modal-update-group-member"
    $("#group-modal-update-group-member").find("[name=grp-code]").val(userRecord.GrpCode);
    $("#group-modal-update-group-member").find("[name=ccn-user-name]").text(userRecord.CcnUsername);
    $("#group-modal-update-group-member").find("[name=ccn-name]").text(userRecord.CcnDisplayName);
    $("#group-modal-update-group-member").find("[name=ccn-email]").html(app.library.html.email(userRecord.CcnEmail));

    //Set state of bootstrapToggle button.
    if (userRecord.GccApproveFlag == true) {
        $("#group-modal-update-group-member").find("[name=gcc-approve-flag]").bootstrapToggle('on');
    } else {
        $("#group-modal-update-group-member").find("[name=gcc-approve-flag]").bootstrapToggle('off');
    }
    //Modal Member Group update show
    $("#group-modal-update-group-member").modal("show");
};

/**
 * Validate Edit Member To Group - "form"
 */
app.group.membergroup.validation.update = function () {
    $("#group-modal-update-group-member").find("form").trigger("reset").validate({
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.group.membergroup.ajax.update();
        }
    }).resetForm();
};

/**
 * Update Member in the Group - Members - Group Table
 */
app.group.membergroup.ajax.update = function () {
    var ccnUsername = $("#group-modal-update-group-member").find("[name=ccn-user-name]").text();
    var gccApproveFlag = $("#group-modal-update-group-member").find("[name=gcc-approve-flag]").prop('checked');
    var grpCode = $("#group-modal-update-group-member").find("[name=grp-code]").val();

    var apiParams = {
        CcnUsername: ccnUsername,
        GccApproveFlag: gccApproveFlag,
        GrpCode: grpCode
    };
    var callbackParam = {
        CcnUsername: ccnUsername,
        GrpCode: grpCode
    };
    // CAll Ajax to Edit. Get the fresh new data. Redraw table
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.GroupAccount_API.Update",
        apiParams,
        "app.group.membergroup.callback.updateOnSuccess",
        callbackParam,
        "app.group.membergroup.callback.updateOnError",
        null,
        { async: false }
    );
};

/**
 * Update Member (at to the Group) Callback
 * After AJAX call.
 * @param {*} data 
 * @param {*} callbackParam 
 */
app.group.membergroup.callback.updateOnSuccess = function (data, callbackParam) {
    //Redraw Members Group DataTable with fresh data
    app.group.membergroup.ajax.read(callbackParam.GrpCode);
    //Modal hide
    $("#group-modal-update-group-member").modal("hide");

    if (data == C_API_AJAX_SUCCESS) {
        //Clear fields at modal-update-user Modal
        $("#group-modal-update-group-member").find("[name=ccn-user-name]").text("");
        $("#group-modal-update-group-member").find("[name=grp-name]").text("");
        $("#group-modal-update-group-member").find("[name=gcc-approve-flag]").prop('checked', false); //default value
        api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.CcnUsername]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
 * Update Member (at to the Group) Callback
 * After AJAX call.
 * @param {*} error 
 * @param {*} callbackParam 
 */
app.group.membergroup.callback.updateOnError = function (error, callbackParam) {
    //Redraw Members Group DataTable with fresh data
    app.group.membergroup.ajax.read(callbackParam.GrpCode);
    //Modal hide
    $("#group-modal-update-group-member").modal("hide");
};
//#endregion

//#region Create Members
/**
 * Add Member Group button click
 * Display modal - member - add - to - group : Click at Add Member button
 */
app.group.membergroup.modal.create = function () {
    //Flush the modal
    // Clear previous search - do not delete required for Member search functionality (select2)
    var currentGroup = $("#group-members-read").find("[name=button-create]").attr("current-group");
    $("#group-modal-add-member").find("[name=group-input-add-member-username]").empty(); //CcnUsername:
    $("#group-modal-add-member").find("[name=group-input-add-member-name]").empty(); //name:
    $("#group-modal-add-member").find("[name=namecreate]").empty(); // CcnDisplayName:
    //Set member approve flag  to default value.
    $("#group-modal-add-member").find("[name=group-input-add-member-approve-flag]").bootstrapToggle('off');
    //Flush error labels - do not delete required for Member search functionality (select2)
    $(".error").empty();
    $("#group-modal-add-member").find("[name=group-input-add-member-search-error-holder]").empty();
    // Call the API to get AD user names
    // Only a MODERATOR can be member of a group. 
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.Account_API.Read",
        { CcnUsername: null, "PrvCode": C_APP_PRIVILEGE_MODERATOR },
        "app.group.membergroup.callback.searchUser",
        { CcnUsername: null, "PrvCode": C_APP_PRIVILEGE_MODERATOR },
        null,
        null,
        { async: false } // We need this for Modal Create, even is API Read Method .
    );

    //Username Search functionality for Member (Group) - Create only
    $("#group-modal-add-member").find("[name=group-input-add-member-search]").on('select2:select', function (e) {
        var current = e.params.data;
        if (current) {
            // Some item from your model is active!
            if (current.id.toLowerCase() == $("#group-modal-add-member").find("[name=group-input-add-member-search]").val().toLowerCase()) {
                app.group.UpdateFields(current);
            } else {
                //"This means it is only a partial match, you can either add a new item or take the active if you don't want new items"
                app.group.UpdateFields(null);
            }
        } else {
            // Nothing is active so it is a new value (or maybe empty value)
            app.group.UpdateFields(null);
        }
    });
    //Set focus on first field
    $("#group-modal-add-member").modal("show");
};

/**
 * Populate data to searchUser
 * @param {*} data 
  */
app.group.membergroup.callback.searchUser = function (data) {
    // Load select2
    $("#group-modal-add-member").find("[name=group-input-add-member-search]").empty().append($("<option>")).select2({
        dropdownParent: $('#group-modal-add-member'),
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.group.MapData(data)
    });

    // Enable and Focus Search input
    $("#group-modal-add-member").find("[name=group-input-add-member-search]").prop('disabled', false).focus();
    // Validate Create/Add Member - Modal - "group-modal-add-member"
    app.group.membergroup.validation.create();
};

/**
 * Validate Add Member - Modal - "group-modal-add-member"
 */
app.group.membergroup.validation.create = function () {
    $("#group-modal-add-member form").trigger("reset").validate({
        rules: {
            "group-input-add-member-username":
            {
                required: true
            },
            "group-input-add-member-name":
            {
                required: true,
            },
            "group-input-add-member-search":
            {
                required: true,
                notEqualTo: ""
            }
        },
        errorPlacement: function (error, element) {
            $("#group-modal-add-member [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.group.membergroup.ajax.create();
        }
    }).resetForm();
};

/**
 * Add Member to Group to Table "group-members-read table"
 */
app.group.membergroup.ajax.create = function () {
    var usernamecreate = $("#group-modal-add-member").find("[name=group-input-add-member-username]").text();
    var grpapproveflagcreate = $("#group-modal-add-member").find("[name=group-input-add-member-approve-flag]").prop('checked');
    var currentGroup = $("#group-members-read").find("[name=button-create]").attr("current-group");
    var apiParams = {
        CcnUsername: usernamecreate,
        GrpCode: currentGroup,
        GccApproveFlag: grpapproveflagcreate
    };
    var callbackParam = {
        CcnUsername: usernamecreate,
        GrpCode: currentGroup
    };
    // Do not Redraw table for Add User
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.private,
        "PxStat.Security.GroupAccount_API.Create",
        apiParams,
        "app.group.membergroup.callback.createOnSuccess",
        callbackParam,
        "app.group.membergroup.callback.createOnError",
        callbackParam,
        { async: false }
    );
};

/**
 * Create/Add Member to Group after Ajax success call
 * @param {*} data 
 * @param {*} callbackParam 
 */
app.group.membergroup.callback.createOnSuccess = function (data, callbackParam) {
    app.group.ajax.read(); // Number of member change for Group.    
    app.group.membergroup.ajax.read(callbackParam.GrpCode);

    $("#group-modal-add-member").modal("hide");

    if (data == C_API_AJAX_SUCCESS) {
        //clear fields at Modal group-modal-add-member
        $("#group-modal-add-member").find("[name=group-input-add-member-username]").text("");
        $("#group-modal-add-member").find("[name=group-input-add-member-name]").text("");
        $("#group-modal-add-member").find("[name=group-input-add-member-approve-flag]").bootstrapToggle('off');
        api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.CcnUsername]));
    } else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

/**
 * Create/Add Member to Group after Ajax success call
 * @param {*} error
 * @param {*} callbackParam
 */
app.group.membergroup.callback.createOnError = function (error, callbackParam) {
    app.group.ajax.read(); // Number of member change for Group.    
    app.group.membergroup.ajax.read(callbackParam.GrpCode);

    $("#group-modal-add-member").modal("hide");
};

//#endregion
