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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      "LngIsoCode": app.label.language.iso.code
    },
    "app.product.callback.readSubject",
    {
      SbjCode: SbjCode
    }
  );
};

/**
 * Callback from api Read Subject
 * @param  {} data
 * @param {*} SbjCode 
 */
app.product.callback.readSubject = function (data, params) {
  // Load select2
  $("#product-container").find("[name=select-main-subject-search]").empty().append($("<option>")).select2({
    minimumInputLength: 0,
    allowClear: true,
    width: '100%',
    placeholder: app.label.static["start-typing"],
    data: app.product.mapData(data)
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
        //update state
        app.navigation.replaceState("#nav-link-product", {
          "SbjCode": selectedObject.SbjCode
        })

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

  if (params.SbjCode) {
    app.product.load(params.SbjCode);
  }
};

/**
 * Get product from api based on selected subject
 * @param  {} subject
 */
app.product.ajax.read = function (subject) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Product_API.Read",
    {
      "SbjCode": subject.SbjCode,
      "LngIsoCode": app.label.language.iso.code
    },
    "app.product.callback.read");
};

/**
 * Call back from api to handle product
 * @param  {} data
 */
app.product.callback.read = function (data) {
  app.product.drawDataTableProduct(data);
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
  //initiate all copy to clipboard 
  new ClipboardJS("#product-card-read [name=product-url-copy-icon]");
  $('[data-bs-toggle="tooltip"]').tooltip();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
  // Click event "internalLink"
  $("#product-card-read table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    var callbackParam = {
      idn: $(this).attr("idn"),
      "PrcValue": $(this).attr("prc-value"),
      "associated": $(this).data("link-type") == "associated" ? true : false
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
            return app.library.html.link.external({ name: "product-url-" + row.PrcCode }, app.config.url.application + C_COOKIE_LINK_PRODUCT + "/" + row.PrcCode) + $("<i>", {
              "class": "far fa-copy fa-lg ms-2",
              "name": "product-url-copy-icon",
              "data-bs-toggle": "tooltip",
              "data-clipboard-target": "#product-card-read [name=product-url-" + row.PrcCode + "]",
              "data-bs-placement": "right",
              "title": "",
              "style": "cursor: grab",
              "data-clipboard-action": "copy",
              "label-tooltip": "copy-to-clipboard"
            }).get(0).outerHTML;
          }
        },
        {
          data: null,
          render: function (_data, _type, row) {
            return app.library.html.link.internal({ "data-link-type": "table", idn: row.PrcCode, "prc-value": row.PrcValue }, String(row.MtrCount));
          }
        },
        {
          data: null,
          render: function (_data, _type, row) {
            return app.library.html.link.internal({ "data-link-type": "associated", idn: row.PrcCode, "prc-value": row.PrcValue }, String(row.MtrAssociatedCount));
          }
        },
        {
          data: null,
          sorting: false,
          searchable: false,
          render: function (data, type, row) {
            return app.library.html.deleteButton({ idn: row.PrcCode, "prc-value": row.PrcValue, "sbj-code": row.SbjCode }, (row.MtrCount + row.MtrAssociatedCount) > 0 ? true : false);
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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      "SbjCode": idn,
      "LngIsoCode": app.label.language.iso.code
    },
    "app.product.callback.readSubjectCreate");
};

/**
 * Callback read Subject
 * @param  {} data
 */
app.product.callback.readSubjectCreate = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    app.product.modal.create(data);
  } else {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Reload
    app.product.load();
  }
}

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
  // A Product is created always against the default Language
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Product_API.Create",
    {
      "PrcCode": $("#product-modal-create").find("[name=prc-code]").val(),
      "PrcValue": $("#product-modal-create").find("[name=prc-value]").val(),
      "SbjCode": $("#product-modal-create").find("[name='idn']").val()
    },
    "app.product.callback.createOnSuccess",
    $("#product-modal-create").find("[name=prc-value]").val(),
    "app.product.callback.createOnError",
    null,
    { async: false }
  );
};

/**
 * Callback Ajax call to create new product
 * @param  {} data
 * @param  {} callbackParam
 */
app.product.callback.createOnSuccess = function (data, callbackParam) {
  // Hide modal
  $("#product-modal-create").modal("hide");

  // Refresh list of products
  app.product.load();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Callback Ajax call to create new product
 * @param  {} error
 */
app.product.callback.createOnError = function (error) {
  // Hide modal
  $("#product-modal-create").modal("hide");

  // Refresh list of products
  app.product.load();
};

//#endregion

//#region update

/**
 * Call modal to update product
 * @param  {} PrcCode
 */
app.product.modal.update = function (PrcCode) {
  app.product.ajax.readSubjectUpdate(PrcCode);
};

/**
 * Ajax read Subject for Update
 */
app.product.ajax.readSubjectUpdate = function (PrcCode) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Subject_API.Read",
    {
      "SbjCode": null,
      "LngIsoCode": app.label.language.iso.code
    },
    "app.product.callback.readSubjectUpdate",
    PrcCode);
};

/**
 * Callback read Subject for Update 
 * @param  {} data
 */
app.product.callback.readSubjectUpdate = function (data, PrcCode) {
  if (data && Array.isArray(data) && data.length) {
    // Product validation for update
    app.product.validation.update();

    // Load select2
    $("#product-modal-update").find("[name=select-update-subject-search]").empty().append($("<option>")).select2({
      dropdownParent: $('#product-modal-update'),
      minimumInputLength: 0,
      allowClear: true,
      width: '100%',
      placeholder: app.label.static["start-typing"],
      data: app.product.mapData(data)
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
    app.product.ajax.readUpdate(PrcCode);
  }
  // Handle no data
  else {
    api.modal.information(app.label.static["api-ajax-nodata"]);
  }
};

/**
 * Get product to update details from api
 * @param  {} PrcCode
 */
app.product.ajax.readUpdate = function (PrcCode) {
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Product_API.Read",
    {
      "PrcCode": PrcCode,
      "LngIsoCode": app.label.language.iso.code
    },
    "app.product.callback.readUpdate");
};

/**
 * Populate modal with product details
 * @param  {} data
 */
app.product.callback.readUpdate = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];
    app.product.modal.readUpdate(data);
  } else {
    api.modal.information(app.label.static["api-ajax-nodata"]);
    // Reload
    app.product.load();
  }
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
  // Change app.config.language.iso.code to the selected one
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Product_API.Update",
    {
      "SbjCode": $("#product-modal-update").find("[name=select-update-subject-search]").val(),
      "PrcCode": $("#product-modal-update").find("[name=idn]").val(),
      "PrcValue": $("#product-modal-update").find("[name=prc-value]").val(),
      "PrcCodeNew": $("#product-modal-update").find("[name=prc-code]").val(),
      "LngIsoCode": app.label.language.iso.code
    },
    "app.product.callback.updateOnSuccess",
    $("#product-modal-update").find("[name=prc-value]").val(),
    "app.product.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * Callback after update
 * @param  {} data
 * @param  {} callbackParam
 */
app.product.callback.updateOnSuccess = function (data, callbackParam) {
  // Hide modal
  $("#product-modal-update").modal("hide");

  // Refresh list of products
  app.product.load();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Callback after update
 * @param  {} error
 */
app.product.callback.updateOnError = function (error) {
  // Hide modal
  $("#product-modal-update").modal("hide");

  // Refresh list of products
  app.product.load();
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
    app.config.url.api.jsonrpc.private,
    "PxStat.System.Navigation.Product_API.Delete",
    {
      "PrcCode": productToDelete.idn
    },
    "app.product.callback.deleteOnSuccess",
    productToDelete,
    "app.product.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
 * Product delete
 * @param {*} data 
 * @param {*} productToDelete 
 */
app.product.callback.deleteOnSuccess = function (data, productToDelete) {
  // Refresh list of products
  app.product.load();

  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [productToDelete.PrcValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 * Product delete
 * @param {*} error 
 */
app.product.callback.deleteOnError = function (error) {
  // Refresh list of products
  app.product.load();
};
//#endregion

//#region readMatrixByProduct

/**
 * Ajax Read Release List
 * @param  {} callbackParam
 */
app.product.ajax.readMatrixByProduct = function (callbackParam) {
  api.ajax.jsonrpc.request(
    app.config.url.api.jsonrpc.private,
    "PxStat.Data.Matrix_API.ReadByProduct",
    {
      "PrcCode": callbackParam.idn,
      "LngIsoCode": app.label.language.iso.code,
      "AssociatedOnly": callbackParam.associated
    },
    "app.product.callback.readMatrixByProduct",
    callbackParam);
};

/**
 * Handle data from Ajax
 * @param  {} data
 * @param  {} callbackParam
 */
app.product.callback.readMatrixByProduct = function (data, callbackParam) {
  app.product.drawReleaseByProductDataTable(data, callbackParam);
};

/**
 * Draw Callback for Datatable
 */
app.product.drawCallbackRelease = function () {
  $('[data-bs-toggle="tooltip"]').tooltip();

  //Release version link click redirect to 
  $("#product-release-modal table").find("[name=" + C_APP_NAME_LINK_INTERNAL + "]").once("click", function (e) {
    e.preventDefault();
    //Set the code
    var MtrCode = $(this).attr("MtrCode");

    //Wait for the modal to close
    $("#product-release-modal").on('hidden.bs.modal', function (e) {
      //Unbind the event for next call
      $("#product-release-modal").off('hidden.bs.modal');

      api.content.goTo("entity/release", null, "#nav-link-release", { "MtrCode": MtrCode });
    }).modal("hide");


  });
}

/**
 * read release list by product callback
 * @param {*} data 
 * @param  {} callbackParam
 */
app.product.drawReleaseByProductDataTable = function (data, callbackParam) {
  $("#product-release-modal").find("[name=product-code]").text(callbackParam.idn);
  $("#product-release-modal").find("[name=product-value]").text(callbackParam.PrcValue);
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
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHANUMERIC);
          $(this).val(value);
          return value;
        }
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
        required: true,
        normalizer: function (value) {
          value = value.sanitise(C_SANITISE_UPPERCASE, C_APP_REGEX_ALPHANUMERIC);
          $(this).val(value);
          return value;
        }
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