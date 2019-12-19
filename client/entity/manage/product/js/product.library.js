/*******************************************************************************
Custom JS application specific
*******************************************************************************/
//#region Namespaces definitions
app.product = {};
app.product.modal = {};
app.product.validation = {};
app.product.ajax = {};
app.product.callback = {};
//#endregion

//#region Miscellaneous

/**
 * Map API data to select dropdown  data model
 * @param {*} dataAPI 
 */
app.product.mapData = function (dataAPI) {
  $.each(dataAPI, function (i, item) {
    // Create ID and NAME to the list
    dataAPI[i].id = item.SbjCode;
    dataAPI[i].text = item.SbjValue;
  });
  return dataAPI;
};
/**
 * Load Product list
 * @param {*} SbjCode 
 */
app.product.load = function (SbjCode) {
  SbjCode = SbjCode || $("#product-container").find("[name=select-main-subject-search]").val();

  /*  Multi-steps:
    *  1. Set the Value
    *  2. Trigger Change to display the set Value above
    *  3. Trigger type: 'select2:select' to load the Select2 object 
    */
  if (SbjCode) {
    $("#product-container").find("[name=select-main-subject-search]").val(SbjCode).trigger("change");
  }
  $("#product-container").find("[name=select-main-subject-search]").trigger({
    type: 'select2:select',
    params: {
      data: $("#product-container").find("[name=select-main-subject-search]").select2('data')[0]
    }
  });

};
//#endregion

//#region read
/**
 *Read subject from api to populate data select dropdown
 * @param {*} SbjCode 
 */
app.product.ajax.readSubject = function (SbjCode) {
  // Default params
  SbjCode = SbjCode || null;

  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      LngIsoCode: app.label.language.iso.code
    },
    "app.product.callback.readSubject",
    SbjCode
  );
};

/**
 * Callback from api Read Subject
 * @param  {} response
 * @param {*} SbjCode 
 */
app.product.callback.readSubject = function (response, SbjCode) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (response.data !== undefined) {
    // Load select2
    $("#product-container").find("[name=select-main-subject-search]").empty().append($("<option>")).select2({
      minimumInputLength: 0,
      allowClear: true,
      width: '100%',
      placeholder: app.label.static["start-typing"],
      data: app.product.mapData(response.data)
    });

    // Enable and Focus Search input
    $("#product-container").find("[name=select-main-subject-search]").prop('disabled', false).focus();

    $("#product-container").find("[name=select-main-subject-search]").on('select2:select', function (e) {
      var selectedObject = e.params.data;
      if (selectedObject) {
        // Some item from your model is active!
        if (selectedObject.id.toLowerCase() == $("#product-container").find("[name=select-main-subject-search]").val().toLowerCase()) {
          // This means the exact match is found. Use toLowerCase() if you want case insensitive match.
          // set values for Create Product button.
          $("#product-modal-create").find("[name='idn']").val(selectedObject.SbjCode);
          $("#product-card-read").find("[name=button-create]").attr("idn", selectedObject.SbjCode);
          var subject = { SbjCode: selectedObject.SbjCode, SbjValue: selectedObject.SbjValue };
          //Read Products for searched Subject.
          app.product.ajax.read(subject);
        }
        else {
          $("#product-card-read").hide();
          $("#product-release-modal").hide();
        }
      } else {
        // Nothing is active so it is a new value (or maybe empty value)
        $("#product-card-read").hide();
        $("#product-release-modal").hide();
      }
    });
    if (SbjCode) {
      app.product.load(SbjCode);
    }
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Get product from api based on selected subject
 * @param  {} subject
 */
app.product.ajax.read = function (subject) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Product_API.Read",
    {
      SbjCode: subject.SbjCode,
      LngIsoCode: app.label.language.iso.code
    },
    "app.product.callback.read");
};

/**
 * Call back from api to handle product
 * @param  {} response
 */
app.product.callback.read = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data !== undefined) {
    app.product.drawDataTableProduct(response.data);
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.product.drawCallbackProduct = function () {
  //  Display  the modal Update product  on row click    
  $("#product-card-read table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var PrcCode = $(this).attr("idn"); //PrcCode
    app.product.modal.update(PrcCode);
  });
  // Click event "internalLink"
  $("#product-card-read table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    var callbackParam = {
      idn: $(this).attr("idn"),
      "PrcValue": $(this).attr("prc-value")
    };

    // Ajax read 
    app.product.ajax.readMatrixByProduct(callbackParam);

    $("#product-release-modal").modal("show");

  });
  // Click event delete - Display the modal to delete product
  $("#product-card-read table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", function () {
    var productToDelete = {
      idn: $(this).attr("idn"),
      PrcValue: $(this).attr("prc-value"),
      SbjCode: $(this).attr("sbj-code")
    };
    app.product.modal.delete(productToDelete);
  });


}

/**
 * Draw datatable of product
 * @param  {} data
 */
app.product.drawDataTableProduct = function (data) {
  if ($.fn.dataTable.isDataTable("#product-card-read table")) {
    app.library.datatable.reDraw("#product-card-read table", data);
  } else {

    var localOptions = {
      data: data,
      columns: [
        {
          data: null,
          render: function (_data, _type, row) {
            var attributes = { idn: row.PrcCode }; //idn PrcCode
            return app.library.html.link.edit(attributes, row.PrcCode); //IDN => PrcCode
          }
        },
        {
          data: "PrcValue",
        },
        {
          data: null,
          render: function (_data, _type, row) {
            var attributes = { idn: row.PrcCode, "prc-value": row.PrcValue }; //idn PrcCode
            return app.library.html.link.internal(attributes); //idn => PrcCode
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            var attributes = { idn: row.PrcCode, "prc-value": row.PrcValue, "sbj-code": row.SbjCode }; //idn PrcCode
            var deleteButton = app.library.html.deleteButton(attributes, false);
            if (row.PrcReleaseCount > 0) {
              deleteButton = app.library.html.deleteButton(attributes, true);
            }
            return deleteButton;
          },
          "width": "1%"
        }
      ],
      drawCallback: function (settings) {
        app.product.drawCallbackProduct();

      },
      //Translate labels language
      language: app.label.plugin.datatable
    };
    $("#product-card-read table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.product.drawCallbackProduct();
    });

    // Product must be first created in the default language
    if (app.label.language.iso.code == app.config.language.iso.code) {
      // Enable the button
      $("#product-card-read").find("[name=button-create]").prop("disabled", false);
      // Hide warning
      $("#product-card-read").find("[name=warning]").hide();
      // Create new subject button click event
      $("#product-card-read").find("[name=button-create]").once("click", function () {
        var idn = $("#product-card-read").find("[name=button-create]").attr("idn");
        app.product.ajax.readSubjectCreate(idn);
      });
    } else {
      // Disable the button
      $("#product-card-read").find("[name=button-create]").prop("disabled", true);
      // Show warning
      $("#product-card-read").find("[name=warning]").show();
    }
  }
  //smooth transition after changing subject
  $("#product-card-read").hide().fadeIn();
  $("#product-release-modal").hide();
};

//#endregion

//#region create
/**
 * getSubject for validation
 * @param  {} idn
 */
app.product.ajax.readSubjectCreate = function (idn) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      SbjCode: idn,
      LngIsoCode: app.label.language.iso.code
    },
    "app.product.callback.readSubjectCreate");
};

/**
 * Callback read Subject
 * @param  {} response
 */
app.product.callback.readSubjectCreate = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Reload
    app.product.load();
  }
  else if (response.data) {
    response.data = response.data[0];

    app.product.modal.create(response.data);
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Modal for create new product
 * @param  {} data
 */
app.product.modal.create = function (data) {
  $("#product-modal-create").find("[name=idn]").val(data.SbjCode);
  $("#product-modal-create").find("[name=product-modal-span-subject]").text(data.SbjValue);
  //validation create product
  app.product.validation.create();
  // Modal create show
  $("#product-modal-create").modal("show");
};

/**
 * Ajax call to create new product
 */
app.product.ajax.create = function () {
  var prcCode = $("#product-modal-create").find("[name=prc-code]").val();
  var prcValue = $("#product-modal-create").find("[name=prc-value]").val();
  var sbjCode = $("#product-modal-create").find("[name='idn']").val();
  var apiParam = {
    PrcCode: prcCode,
    PrcValue: prcValue,
    SbjCode: sbjCode
  };
  var callbackParam = {
    PrcValue: prcValue
  };
  // A Product is created always against the default Language
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Product_API.Create",
    apiParam,
    "app.product.callback.create",
    callbackParam,
    null,
    null,
    { async: false }
  );
};

/**
 * Callback Ajax call to create new product
 * @param  {} response
 * @param  {} callbackParam
 */
app.product.callback.create = function (response, callbackParam) {
  // Hide modal
  $("#product-modal-create").modal("hide");

  // Refresh list of products
  app.product.load();

  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.PrcValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

//#endregion

//#region update

/**
 * Call modal to update product
 * @param  {} PrcCode
 */
app.product.modal.update = function (PrcCode) {
  app.product.ajax.readSubjectUpdate();
  app.product.ajax.readUpdate(PrcCode);
};

/**
 * Ajax read Subject for Update
 */
app.product.ajax.readSubjectUpdate = function () {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      SbjCode: null,
      LngIsoCode: app.label.language.iso.code
    },
    "app.product.callback.readSubjectUpdate");
};

/**
 * Callback read Subject for Update 
 * @param  {} response
 */
app.product.callback.readSubjectUpdate = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
  }
  else if (response.data) {
    // Product validation for update
    app.product.validation.update();

    // Load select2
    $("#product-modal-update").find("[name=select-update-subject-search]").empty().append($("<option>")).select2({
      dropdownParent: $('#product-modal-update'),
      minimumInputLength: 0,
      allowClear: true,
      width: '100%',
      placeholder: app.label.static["start-typing"],
      data: app.product.mapData(response.data)
    });

    // Enable and Focus Search input
    $("#product-modal-update").find("[name=select-update-subject-search]").prop('disabled', false).focus();

    $("#product-modal-update").find("[name=select-update-subject-search]").on('select2:select', function (e) {
      var selectedObject = e.params.data;
      if (selectedObject) {
        // Some item from your model is active!
        if (selectedObject.id.toLowerCase() == $("#product-modal-update").find("[name=select-update-subject-search]").val().toLowerCase()) {
          // This means the exact match is found. Use toLowerCase() if you want case insensitive match.
          //Do not delete required for Subject search functionality (select2)
          $("#product-modal-update").find("[name=select-update-subject-search-error-holder]").empty();
        }
      }
    });
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Get product to update details from api
 * @param  {} PrcCode
 */
app.product.ajax.readUpdate = function (PrcCode) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Product_API.Read",
    {
      PrcCode: PrcCode,
      LngIsoCode: app.label.language.iso.code
    },
    "app.product.callback.readUpdate");
};

/**
 * Populate modal with product details
 * @param  {} response
 */
app.product.callback.readUpdate = function (response) {
  if (response.error) {
    api.modal.error(response.error.message);
  }
  else if (!response.data || (Array.isArray(response.data) && !response.data.length)) {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Reload
    app.product.load();
  }
  else if (response.data) {
    response.data = response.data[0];
    app.product.modal.readUpdate(response.data);
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Display Modal Update product
 * @param {*} productRecord
 */
app.product.modal.readUpdate = function (productRecord) {
  //Update values
  $("#product-modal-update").find("[name=idn]").val(productRecord.PrcCode);
  $("#product-modal-update").find("[name=prc-code]").val(productRecord.PrcCode);
  $("#product-modal-update").find("[name=prc-value]").val(productRecord.PrcValue);
  $("#product-modal-update").find("[name=select-update-subject-search]").val(productRecord.SbjCode).trigger("change").trigger({
    type: 'select2:select',
    params: {
      data: $("#product-modal-update").find("[name=select-update-subject-search]").select2('data')[0]
    }
  });
  //Show Modal Update
  $("#product-modal-update").modal("show");
};

/**
 * Ajax to update product
 */
app.product.ajax.update = function () {
  var sbjCode = $("#product-modal-update").find("[name=select-update-subject-search]").val(); // User selected drop down 
  var prcCode = $("#product-modal-update").find("[name=idn]").val();
  var prcCodeNew = $("#product-modal-update").find("[name=prc-code]").val();
  var prcValue = $("#product-modal-update").find("[name=prc-value]").val();
  // Change app.config.language.iso.code to the selected one
  var apiParams = {
    SbjCode: sbjCode,
    PrcCode: prcCode,
    PrcValue: prcValue,
    PrcCodeNew: prcCodeNew,
    LngIsoCode: app.label.language.iso.code
  };
  var callbackParam = {
    PrcValue: prcValue
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Product_API.Update",
    apiParams,
    "app.product.callback.update",
    callbackParam,
    null,
    null,
    { async: false }
  );
};

/**
 * Callback after update
 * @param  {} response
 * @param  {} callbackParam
 */
app.product.callback.update = function (response, callbackParam) {
  // Hide modal
  $("#product-modal-update").modal("hide");

  // Refresh list of products
  app.product.load();

  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.PrcValue]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};
//#endregion

//#region delete

/**
 * Modal delete 
 * @param {*} productToDelete 
 */
app.product.modal.delete = function (productToDelete) {
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [productToDelete.PrcValue]), app.product.ajax.delete, productToDelete);
};

/**
 * Ajax call delete product
 * @param {*} productToDelete 
 */
app.product.ajax.delete = function (productToDelete) {
  // A Product is deleted always against the default Language
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Product_API.Delete",
    { PrcCode: productToDelete.idn },
    "app.product.callback.delete",
    productToDelete,
    null,
    null,
    { async: false }
  );
};

/**
 * Product delete
 * @param {*} response 
 * @param {*} productToDelete 
 */
app.product.callback.delete = function (response, productToDelete) {
  // Refresh list of products
  app.product.load();

  if (response.error) {
    api.modal.error(response.error.message);
  } else if (response.data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [productToDelete.PrcValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};
//#endregion

//#region readMatrixByProduct

/**
 * Ajax Read Release List
 * @param  {} callbackParam
 */
app.product.ajax.readMatrixByProduct = function (callbackParam) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.Data.Matrix_API.ReadByProduct",
    {
      PrcCode: callbackParam.idn,
      LngIsoCode: app.label.language.iso.code
    },
    "app.product.callback.readMatrixByProduct",
    callbackParam);
};

/**
 * Handle data from Ajax
 * @param  {} response
 * @param  {} callbackParam
 */
app.product.callback.readMatrixByProduct = function (response, callbackParam) {
  if (response.error) {
    // Handle the Error in the Response first
    api.modal.error(response.error.message);
  } else if (response.data !== undefined) {
    // Handle the Data in the Response then
    app.product.drawReleaseByProductDataTable(response.data, callbackParam);
  }
  // Handle Exception
  else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Draw Callback for Datatable
 */
app.product.drawCallbackRelease = function () {
  $('[data-toggle="tooltip"]').tooltip();

  //Release version link click redirect to 
  $("#product-release-modal table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    //Set the code
    var MtrCode = $(this).attr("MtrCode");

    $("#product-release-modal").modal("hide");

    //Wait for the modal to close
    $("#product-release-modal").on('hidden.bs.modal', function (e) {
      //Unbind the event for next call
      $("#product-release-modal").off('hidden.bs.modal');

      api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": MtrCode });
    })


  });
}

/**
 * read release list by product callback
 * @param {*} data 
 * @param  {} callbackParam
 */
app.product.drawReleaseByProductDataTable = function (data, callbackParam) {
  $("#product-release-modal").find("[name=product-release-modal-header]").text(callbackParam.PrcValue);
  if ($.fn.dataTable.isDataTable("#product-release-modal table")) {
    app.library.datatable.reDraw("#product-release-modal table", data);
  } else {
    var localOptions = {
      data: data,
      columns: [
        {
          data: null,
          render: function (data, type, row) {
            var attributes = { idn: row.RlsCode, MtrCode: row.MtrCode };
            return app.library.html.link.internal(attributes, row.MtrCode, row.MtrTitle);
          }
        }
      ],
      drawCallback: function (settings) {
        app.product.drawCallbackRelease();
      },
      //Translate labels language
      language: app.label.plugin.datatable
    };
    $("#product-release-modal table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.product.drawCallbackRelease();
    });
  }

};

//#endregion

//#region validation
/**
 * Create new product validation
 * */
app.product.validation.create = function () {
  $("#product-modal-create form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "prc-code": {
        required: true
      },
      "prc-value": {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#product-modal-create [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.product.ajax.create();
    }
  }).resetForm();
};

/**
 * Validation for update
 */
app.product.validation.update = function () {
  $("#product-modal-update form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "prc-code": {
        required: true
      },
      "prc-value": {
        required: true
      },
      "select-update-subject-search": {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#product-modal-update [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.product.ajax.update();
    }
  }).resetForm();
};
//#endregion