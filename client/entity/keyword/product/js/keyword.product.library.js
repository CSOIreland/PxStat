/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Add Namespace
// Add Namespace

// app.keyword is a parent namespace
app.keyword = app.keyword || {};
app.keyword.product = {};
app.keyword.product.ajax = {};
app.keyword.product.callback = {};
app.keyword.product.modal = {};
app.keyword.product.validation = {};


//#endregion

/**
 * Map API data to select dropdown data model
 * @param {*} dataAPI 
 */
app.keyword.product.mapDataSubject = function (dataAPI) {
  $.each(dataAPI, function (i, item) {
    // Add ID and NAME to the list
    dataAPI[i].id = item.SbjCode;
    dataAPI[i].text = item.SbjValue;
  });
  return dataAPI;
};

/**
 * Map API data to select dropdown data model
 * @param {*} dataAPI 
 */
app.keyword.product.mapDataProduct = function (dataAPI) {
  $.each(dataAPI, function (i, item) {
    // Add ID and NAME to the list
    dataAPI[i].id = item.PrcCode;
    dataAPI[i].text = item.PrcCode + " (" + item.PrcValue + ")";
  });
  return dataAPI;
};

//#region Init

/**
 * Update Drop Down Subject
 */
app.keyword.product.readSubjects = function () {
  //Get a full list of Subject. Call the API to get Subject names 
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Read",
    { SbjCode: null },
    "app.keyword.product.callback.readSubjects");
};

/**
 * Keyword Product callback read Subjects
 * @param {*} data 
 */
app.keyword.product.callback.readSubjects = function (data) {
  // Load Select2
  $("#keyword-product-container").find("[name=select-main-subject-search]").empty().append($("<option>")).select2({
    minimumInputLength: 0,
    allowClear: true,
    width: '100%',
    placeholder: app.label.static["start-typing"],
    data: app.keyword.product.mapDataSubject(data)
  });

  $("#keyword-product-container").find("[name=select-main-subject-search]").prop('disabled', false).focus();

  //Update Subject search Search functionality
  $("#keyword-product-container").find("[name=select-main-subject-search]").on('select2:select', function (e) {
    var selectedSubject = e.params.data;
    if (selectedSubject) {
      // Some item from your model is active!
      if (selectedSubject.id.toLowerCase() == $("#keyword-product-container").find("[name=select-main-subject-search]").val().toLowerCase()) {
        $("#keyword-product-modal-create").find("[name='sbj-code']").val(selectedSubject.SbjCode);
        $("#keyword-product-modal-update").find("[name='sbj-code']").val(selectedSubject.SbjCode);
        app.keyword.product.readProducts(selectedSubject.SbjCode);
      }
      else {
        $("#keyword-product-container").find("[name=select-main-product-search]").val("");
        $("#keyword-product-container").find("[name=select-main-product-search]").text("");
        $("#keyword-product-container").find("[name=select-main-product-search]").prop('disabled', true);
        $("#keyword-product-read").hide();
      }
    } else {
      // Nothing is active so it is a new value (or maybe empty value)
      $("#keyword-product-container").find("[name=select-main-product-search]").val("");
      $("#keyword-product-container").find("[name=select-main-product-search]").text("");
      $("#keyword-product-container").find("[name=select-main-product-search]").prop('disabled', true);
      $("#keyword-product-read").hide();
    }
  });
};

/**
 * Update Drop Down Product
 * @param {*} subjectCode: SbjCode 
 */
app.keyword.product.readProducts = function (subjectCode) {
  //Get a full list of Subject. Call the API to get Subject names 
  //API AJAX call
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Product_API.Read",
    { SbjCode: subjectCode },
    "app.keyword.product.callback.readProductsOnSuccess",
    null,
    "app.keyword.product.callback.readProductsOnError");
};

/**
 * Keyword Product callback read Products
 * @param {*} data 
 */
app.keyword.product.callback.readProductsOnSuccess = function (data) {
  $("#keyword-product-read").hide();

  // Load Select2
  $("#keyword-product-container").find("[name=select-main-product-search]").empty().append($("<option>")).select2({
    minimumInputLength: 0,
    allowClear: true,
    width: '100%',
    placeholder: app.label.static["start-typing"],
    data: app.keyword.product.mapDataProduct(data)

  });

  $("#keyword-product-container").find("[name=select-main-product-search]").prop('disabled', false).focus();

  //Update Subject search Search functionality
  $("#keyword-product-container").find("[name=select-main-product-search]").on('select2:select', function (e) {
    var selectedProduct = e.params.data;
    if (selectedProduct) {
      // Some item from your model is active!
      if (selectedProduct.id.toLowerCase() == $("#keyword-product-container").find("[name=select-main-product-search]").val().toLowerCase()) {
        //we do have SbjCode at Product selected.
        $("#keyword-product-modal-create").find("[name='prc-code']").val(selectedProduct.PrcCode);
        $("#keyword-product-modal-update").find("[name='prc-code']").val(selectedProduct.PrcCode);
        var sbjCode = $("#keyword-product-modal-create").find("[name='sbj-code']").val();
        $("#keyword-product-modal-create").find("[name='prc-value']").text(selectedProduct.PrcValue);
        $("#keyword-product-modal-update").find("[name='prc-value']").text(selectedProduct.PrcValue);
        app.keyword.product.ajax.read(sbjCode, selectedProduct.PrcCode);
        $("#keyword-product-read").fadeIn();
        //Scroll to Table
        $('html, body').animate({ scrollTop: $('#keyword-product-read').offset().top }, 1000);
      }
      else {
        $("#keyword-product-read").hide();
      }
    } else {
      // Nothing is active so it is a new value (or maybe empty value)
      $("#keyword-product-read").hide();
    }
  });
};

/**
 * Keyword Product callback read Products
 * @param {*} error 
 */
app.keyword.product.callback.readProductsOnError = function (error) {
  $("#keyword-product-read").hide();
};

//#endregion

//#region Read
/**
 * read Keywords Product data from api
 * @param  {} sbjCode
 * @param  {} prcCode
 */
app.keyword.product.ajax.read = function (sbjCode, prcCode) {
  // Get data from API and Draw the Data Table for product 
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Keyword_Product_API.Read",
    {
      "SbjCode": sbjCode,
      "PrcCode": prcCode
    },
    "app.keyword.product.callback.read");
};

/**
 * Callback function when the Product Read call is successful.
 * @param {*} data
 */
app.keyword.product.callback.read = function (data) {
  app.keyword.product.drawDataTable(data);
};

/**
 * Draw Callback for Datatable
 */
app.keyword.drawCallback = function () {
  $('[data-bs-toggle="tooltip"]').tooltip();

  // Display  the modal "modal-update-reason" on "[name=" + C_APP_NAME_LINK_EDIT + "]" click
  $("#keyword-product-read table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn"); //idn: KprCode
    var prcCode = $("#keyword-product-modal-update").find("[name='prc-code']").val();
    var sbjCode = $("#keyword-product-modal-update").find("[name='sbj-code']").val();
    var apiParams = { KprCode: idn, PrcCode: prcCode, SbjCode: sbjCode };
    app.keyword.product.readUpdate(apiParams);
  });

  //Call Synonyms when word is selected.
  $("td.details-request-control i.fa.plus-control").css({ "color": "forestgreen" });
  $("#keyword-product-read table").find("[name=" + C_APP_DATATABLE_EXTRA_INFO_LINK + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    var rowIndexObj = $("[" + C_APP_DATATABLE_ROW_INDEX + "='" + idn + "']");
    var dataTable = $("#keyword-product-read table").DataTable();
    var dataTableRow = dataTable.row(rowIndexObj);
    app.keyword.product.ajax.readSynonym(dataTableRow.data());
    $("#keyword-product-extra-info").modal("show");
  });

  // Display confirmation Modal on DELETE button click
  $("#keyword-product-read table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.keyword.product.modal.delete);
}

/**
 * Create DataTable and get JASON data
 * @param {*} data 
 */
app.keyword.product.drawDataTable = function (data) {
  if ($.fn.DataTable.isDataTable("#keyword-product-read table")) {
    app.library.datatable.reDraw("#keyword-product-read table", data);
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
            if (row.KprMandatoryFlag) {
              return app.library.html.locked(row.KprValue);
            }
            else {
              var attributes = { idn: row.KprCode, "kpr-value": row.KprValue }; //idn KprCode
              return app.library.html.link.edit(attributes, row.KprValue);
            }
          }
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
                }).get(0).outerHTML +
                " " + app.label.static["details"]
            }).get(0).outerHTML;
          }
        },
        {
          data: null,
          type: "natural",
          render: function (data, type, row) {
            return app.library.html.boolean(row.KprMandatoryFlag, true, true);
          }
        },
        {
          data: null,
          type: "natural",
          render: function (data, type, row) {
            return app.library.html.boolean(!row.KprSingularisedFlag, true, true);
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            var attributes = { idn: row.KprCode, "kpr-value": row.KprValue }; //idn KprCode
            if (row.KprMandatoryFlag) {
              return app.library.html.deleteButton(attributes, true);
            }
            return app.library.html.deleteButton(attributes, false);
          },
          "width": "1%"
        }
      ],
      drawCallback: function (settings) {
        app.keyword.drawCallback();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };

    $("#keyword-product-read table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.keyword.drawCallback();
    });


    //Create keyword-product CLICK EVENT. Create product-keyword
    $("#keyword-product-read").find("[name=button-create]").once("click", function (e) { app.keyword.product.modal.create(); });
  }
};

//#endregion

//#region Create
/**
 * Show modal to Create Product
 */
app.keyword.product.modal.create = function () {
  //Validation on Create
  app.keyword.product.validation.create();
  app.keyword.product.ajax.getLanguagesCreate();
  $("#keyword-product-modal-create").find("[name=language-row]").show()

  $("#keyword-product-modal-create").find("[name=acronym-toggle]").bootstrapToggle('off');
  $("#keyword-product-modal-create").find("[name=acronym-toggle]").once("change", function () {
    $("#keyword-product-modal-create").find("[name=language-row]").toggle()
  });
  $("#keyword-product-modal-create").modal("show");
};

/**
*  Get languages data from API to populate language drop down for create.
*/
app.keyword.product.ajax.getLanguagesCreate = function () {
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.keyword.product.callback.getLanguagesCreate");
};

/**
*  Populate language drop down for create.
*/
app.keyword.product.callback.getLanguagesCreate = function (data) {
  $("#keyword-product-modal-create").find("[name=language]").empty().append($("<option>", {
    "text": app.label.static["select-uppercase"],
    "disabled": "disabled",
    "selected": "selected"
  }));
  $.each(data, function (key, value) {
    $("#keyword-product-modal-create").find("[name=language]").append($("<option>", {
      "value": value.LngIsoCode,
      "text": value.LngIsoName
    }));
  });
}

/**
 * Ajax call create  Product
 */
app.keyword.product.ajax.create = function () {
  var prcCode = $("#keyword-product-modal-create").find("[name='prc-code']").val();
  var sbjCode = $("#keyword-product-modal-create").find("[name='sbj-code']").val();
  var prcValue = $("#keyword-product-modal-create").find("[name='prc-value']").text();
  var kprValue = $("#keyword-product-modal-create").find("[name='kpr-value']").val();
  var apiParams = {
    KprValue: kprValue,
    PrcCode: prcCode,
    LngIsoCode: $("#keyword-product-modal-create [name=acronym-toggle]").is(':checked') ? null : $("#keyword-product-modal-create [name=language]").val()
  };

  var callbackParam = {
    KprValue: kprValue,
    PrcCode: prcCode,
    SbjCode: sbjCode,
    PrcValue: prcValue
  };
  // CAll Ajax to Create/Add Product. Do Redraw Data Table for Create/Add Product.
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Keyword_Product_API.Create",
    apiParams,
    "app.keyword.product.callback.createOnSuccess",
    callbackParam,
    "app.keyword.product.callback.createOnError",
    callbackParam,
    { async: false }
  );
};

/**
 * Create/Add Product to Table after Ajax success call
 */
/**
 * * Create/Add Product to Table after Ajax success call
 * @param  {} data
 * @param  {} callbackParam
 */
app.keyword.product.callback.createOnSuccess = function (data, callbackParam) {
  //Redraw Data Table Product with fresh data.
  var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
  var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
  app.keyword.product.ajax.read(SbjCode, PrcCode);

  // Hide modal
  $("#keyword-product-modal-create").modal("hide");

  if (data == C_API_AJAX_SUCCESS) {
    // Display Success Modal
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.KprValue]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Create/Add Product to Table after Ajax success call
 */
/**
 * * Create/Add Product to Table after Ajax success call
 * @param  {} error
 */
app.keyword.product.callback.createOnError = function (error) {
  //Redraw Data Table Product with fresh data.
  var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
  var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
  app.keyword.product.ajax.read(SbjCode, PrcCode);
  // Hide modal
  $("#keyword-product-modal-create").modal("hide");
};
//#endregion

//#region Update

/**
* 
* @param {*} apiParams
*/
app.keyword.product.readUpdate = function (apiParams) {
  // Get data from API and Draw the Data Table for Reason. Populate date to the modal "reason-modal-update"
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Keyword_Product_API.Read",
    apiParams,
    "app.keyword.product.callback.readUpdate");
};

/**
 * Populate Product data to Update Modal
 * @param {*} data 
 */
app.keyword.product.callback.readUpdate = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];
    app.keyword.product.modal.update(data);
  } else {
    //Redraw Data Table Product with fresh data.
    var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
    var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
    app.keyword.product.ajax.read(SbjCode, PrcCode);

    api.modal.information(app.label.static["api-ajax-nodata"]);
  }
};

/**
 * Function to load Update Modal
 * @param {*} keywordRecord 
 */
app.keyword.product.modal.update = function (keywordRecord) {
  // Validate Update Product - Modal
  app.keyword.product.ajax.getLanguagesUpdate();
  app.keyword.product.validation.update();
  //Set fields values
  $("#keyword-product-modal-update").find("[name='kpr-value']").val(keywordRecord.KprValue);
  $("#keyword-product-modal-update").find("[name='kpr-code']").val(keywordRecord.KprCode);
  // Update Modal show
  if (keywordRecord.KprSingularisedFlag) {
    $("#keyword-product-modal-update").find("[name=acronym-toggle]").bootstrapToggle('off');
    $("#keyword-product-modal-update").find("[name=language-row]").show();
  }
  else {
    $("#keyword-product-modal-update").find("[name=acronym-toggle]").bootstrapToggle('on');
    $("#keyword-product-modal-update").find("[name=language-row]").hide();
  };

  $("#keyword-product-modal-update").find("[name=acronym-toggle]").once("change", function () {
    $("#keyword-product-modal-update").find("[name=language-row]").toggle()
  });

  $("#keyword-product-modal-update").modal('show');
};

/**
*  Get languages data from API to populate language drop down for create.
*/
app.keyword.product.ajax.getLanguagesUpdate = function () {
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.keyword.product.callback.getLanguagesUpdate");
};

/**
*  Populate language drop down for create.
*/
app.keyword.product.callback.getLanguagesUpdate = function (data) {
  $("#keyword-product-modal-update").find("[name=language]").empty().append($("<option>", {
    "text": app.label.static["select-uppercase"],
    "disabled": "disabled",
    "selected": "selected"
  }));
  $.each(data, function (key, value) {
    $("#keyword-product-modal-update").find("[name=language]").append($("<option>", {
      "value": value.LngIsoCode,
      "text": value.LngIsoName
    }));
  });
}




/**
 * Save updated Product via AJAX call.
 */
app.keyword.product.ajax.update = function () {
  //Get fields values at keyword-product-modal-update Modal
  var idn = $("#keyword-product-modal-update").find("[name='kpr-code']").val();
  var kprValue = $("#keyword-product-modal-update").find("[name='kpr-value']").val();
  var prcCode = $("#keyword-product-modal-update").find("[name='prc-code']").val();
  var sbjCode = $("#keyword-product-modal-update").find("[name='sbj-code']").val();
  var apiParams = {
    KprCode: idn,
    KprValue: kprValue,
    LngIsoCode: $("#keyword-product-modal-update [name=acronym-toggle]").is(':checked') ? null : $("#keyword-product-modal-update [name=language]").val()
  };
  var callbackParam = {
    KprCode: idn,
    KprValue: kprValue,
    PrcCode: prcCode,
    SbjCode: sbjCode
  };

  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Keyword_Product_API.Update",
    apiParams,
    "app.keyword.product.callback.updateOnSuccess",
    callbackParam,
    "app.keyword.product.callback.updateOnError",
    callbackParam,
    { async: false }
  );
};

/**
 * * Update Product Callback
 * After AJAX call.
 * @param  {} data
 * @param  {} callbackParam
 */
app.keyword.product.callback.updateOnSuccess = function (data, callbackParam) {
  //Redraw Data Table Product with fresh data.
  var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
  var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
  app.keyword.product.ajax.read(SbjCode, PrcCode);

  //Hide modal #keyword-product-modal-update
  $("#keyword-product-modal-update").modal("hide");

  if (data == C_API_AJAX_SUCCESS) {
    // Display Success Modal
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.KprValue]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * * Update Product Callback
 * After AJAX call.
 * @param  {} error
 */
app.keyword.product.callback.updateOnError = function (error) {
  //Redraw Data Table Product with fresh data.
  var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
  var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
  app.keyword.product.ajax.read(SbjCode, PrcCode);

  //Hide modal #keyword-product-modal-update
  $("#keyword-product-modal-update").modal("hide");
};

//#endregion

//#region Delete
//Delete one row from product
app.keyword.product.modal.delete = function () {
  var idn = $(this).attr("idn"); //idn: KprCode
  var kprValue = $(this).attr("kpr-value");//row.KprValue
  var deletetedKeyword = {
    idn: idn,
    KprValue: kprValue
  };
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [deletetedKeyword.KprValue]), app.keyword.product.ajax.delete, deletetedKeyword);
};

/**
 * AJAX call to Delete a specific entry 
 * On AJAX success delete (Do reload table)
 * @param {*} deletetedKeyword
 */
app.keyword.product.ajax.delete = function (deletetedKeyword) {
  // Get the indemnificator to delete
  var apiParams = { KprCode: deletetedKeyword.idn }; // idn KprCode value

  // Call the API by passing the idn to delete Product from DB
  api.ajax.jsonrpc.request(app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Keyword_Product_API.Delete",
    apiParams,
    "app.keyword.product.callback.deleteOnSuccess",
    deletetedKeyword,
    "app.keyword.product.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
 * Callback from server for Delete Product Keyword
 * @param {*} data
 * @param {*} deletetedKeyword
 */
app.keyword.product.callback.deleteOnSuccess = function (data, deletetedKeyword) {
  //Redraw Data Table Product with fresh data.
  var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
  var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
  app.keyword.product.ajax.read(SbjCode, PrcCode);

  if (data == C_API_AJAX_SUCCESS) {
    // Display Success Modal
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [deletetedKeyword.KprValue]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Callback from server for Delete Product Keyword
 * @param {*} error
 */
app.keyword.product.callback.deleteOnError = function (error) {
  //Redraw Data Table Product with fresh data.
  var SbjCode = $("#keyword-product-container").find("[name=select-main-subject-search]").val();
  var PrcCode = $("#keyword-product-container").find("[name=select-main-product-search]").val();
  app.keyword.product.ajax.read(SbjCode, PrcCode);
};

//#endregion

//#region Validation

/**
// Define validation for create
*/
app.keyword.product.validation.create = function () {
  $("#keyword-product-modal-create form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "kpr-value": {
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_LOWERCASE, C_APP_REGEX_ALPHANUMERIC_DIACRITIC);
          $(this).val(value);
          return value;
        }
      },
      "language": {
        required: {
          depends: function () {
            return !$("#keyword-product-modal-create [name=acronym-toggle]").is(':checked');
          }
        }
      }
    },
    errorPlacement: function (error, element) {
      $("#keyword-product-modal-create").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.keyword.product.ajax.create();
    }
  }).resetForm();
};

/**
// Define validation for update
*/
app.keyword.product.validation.update = function () {
  $("#keyword-product-modal-update form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "kpr-value": {
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_LOWERCASE, C_APP_REGEX_ALPHANUMERIC_DIACRITIC);
          $(this).val(value);
          return value;
        }
      },
      "language": {
        required: {
          depends: function () {
            return !$("#keyword-product-modal-create [name=acronym-toggle]").is(':checked');
          }
        }
      }
    },
    errorPlacement: function (error, element) {
      $("#keyword-product-modal-update").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.keyword.product.ajax.update();
    }
  }).resetForm();
};

//#endregion

//#regionsynonymrender

//Ajax call for synonym
app.keyword.product.ajax.readSynonym = function (row) {
  var KprValue = row.KprValue;
  var callbackParam = {
    KprValue: KprValue,

  };
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Keyword_API.ReadSynonym",
    { "KrlValue": KprValue },
    "app.keyword.product.callback.readSynonym",
    callbackParam);
}

//Call back for synonyms
app.keyword.product.callback.readSynonym = function (data, callbackParam) {
  //Add name of keyword selected
  $("#keyword-product-extra-info").find("[name=keyword]").html(callbackParam.KprValue);
  //Clear list card
  $("#keyword-product-extra-info").find("[name=synonym-card]").empty();

  //Get each language
  $.each(data, function (key, language) {
    //Clone card
    var languageCard = $("#keyword-product-template").find("[name=synonym-language-card]").clone();
    //Display the Language
    languageCard.find(".card-header").text(language.LngIsoName);

    //Check if any synonyms
    if (language.Synonym.length) {
      //Get Synonyms
      $.each(language.Synonym, function (key, synonym) {
        var synonymItem = $("<li>", {
          "class": "list-group-item",
          "html": synonym
        });

        languageCard.find("[name=synonym-group]").append(synonymItem);
      });

    } else {
      //Display if no synonyms
      var synonymItem = $("<li>", {
        "class": "list-group-item",
        "html": app.label.static["no-synonyms"]
      });
      languageCard.find("[name=synonym-group]").append(synonymItem);
    }

    $("#keyword-product-extra-info").find("[name=synonym-card]").append(languageCard).get(0).outerHTML;
  });
}


//#endregion