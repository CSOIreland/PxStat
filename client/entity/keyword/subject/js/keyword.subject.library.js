/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
// Create Namespace

// app.keyword is a parent namespace
app.keyword = app.keyword || {};
app.keyword.subject = {};
app.keyword.subject.ajax = {};
app.keyword.subject.callback = {};
app.keyword.subject.modal = {};
app.keyword.subject.validation = {};


//#endregion

/**
 * Map API data to select dropdown  data model
 * @param {*} dataAPI 
 */
app.keyword.subject.mapDataSubject = function (dataAPI) {
  $.each(dataAPI, function (i, item) {
    // Add ID and NAME to the list
    dataAPI[i].id = item.SbjCode;
    dataAPI[i].text = item.SbjValue;
  });
  return dataAPI;
};

//#region Init
/**
 * Ajax
 */
app.keyword.subject.ajax.readSubject = function () {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Subject_API.Read",
    { SbjCode: null },
    "app.keyword.subject.callback.readSubject"
  );
};
/**
 * Callback
 * @param {*} data 
 */
app.keyword.subject.callback.readSubject = function (data) {
  //Load Select2
  $("#keyword-subject-container").find("select[name=select-main-subject-search]").empty().append($("<option>")).select2({
    minimumInputLength: 0,
    allowClear: true,
    width: '100%',
    placeholder: app.label.static["start-typing"],
    data: app.keyword.subject.mapDataSubject(data)
  });

  $("#keyword-subject-container").find("select[name=select-main-subject-search]").prop('disabled', false).focus();

  //Update Subject search Search functionality
  $("#keyword-subject-container").find("select[name=select-main-subject-search]").on('select2:select', function (e) {
    var selectedSubject = e.params.data;
    if (selectedSubject) {
      // Some item from your model is active!
      if (selectedSubject.id.toLowerCase() == $("#keyword-subject-container").find("select[name=select-main-subject-search]").val().toLowerCase()) {
        app.keyword.subject.ajax.read(selectedSubject.SbjCode);
      }
      else {
        $("#keyword-subject-read").hide();
      }
    } else {
      // Nothing is active so it is a new value (or maybe empty value)
      $("#keyword-subject-read").hide();
    }
  });
};

//#endregion

//#region Read
/**
 * Read keyword subject ajax
 * @param {*} selectedSubject
 */
app.keyword.subject.ajax.read = function (SbjCode) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_Subject_API.Read",
    { SbjCode: SbjCode },
    "app.keyword.subject.callback.read"
  );
};

/**
 * Callback Subject Ajax read data 
 * @param {*} data
 */
app.keyword.subject.callback.read = function (data) {
  $("#subject-keyword-card").show();
  $("#keyword-subject-read").fadeIn();

  app.keyword.subject.callback.drawDataTable(data);
};


/**
 * Draw Callback for Datatable
 */
app.keyword.drawCallback = function () {
  $('[data-toggle="tooltip"]').tooltip();

  //Call Synonyms when word is selected.
  $("td.details-request-control i.fa.plus-control").css({ "color": "forestgreen" });
  $("#keyword-subject-read table").find("[name=" + C_APP_DATATABLE_EXTRA_INFO_LINK + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    var rowIndexObj = $("[" + C_APP_DATATABLE_ROW_INDEX + "='" + idn + "']");
    var dataTable = $("#keyword-subject-read table").DataTable();
    var dataTableRow = dataTable.row(rowIndexObj);
    app.keyword.subject.ajax.synonym(dataTableRow.data());
    $("#keyword-subject-extra-info").modal("show");
  });

  //Update keyword event click
  $("#keyword-subject-read table").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    app.keyword.subject.ajax.readUpdate(idn);

  });



  //Delete keyword event click
  $("#keyword-subject-read table").find("[name=" + C_APP_NAME_LINK_DELETE + "]").once("click", app.keyword.subject.modal.delete);
}


/**
 * Callback draw Data Table Subject
 * @param {*} data
 */
app.keyword.subject.callback.drawDataTable = function (data) {
  if ($.fn.dataTable.isDataTable("#keyword-subject-read table")) {
    app.library.datatable.reDraw("#keyword-subject-read table", data);
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
          render: function (_data, _type, row) {
            var attributes = { idn: row.KsbCode, KsbValue: row.KsbValue };
            if (row.KsbMandatoryFlag) {
              return app.library.html.locked(row.KsbValue);
            } else {
              return app.library.html.link.edit(attributes, row.KsbValue);
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
          render: function (_data, _type, row) {
            return app.library.html.boolean(row.KsbMandatoryFlag, true, true);
          }
        },
        {
          data: null,
          type: "natural",
          render: function (data, type, row) {
            return app.library.html.boolean(!row.KsbSingularisedFlag, true, true);
          }
        },
        {
          data: null,
          sorting: true,
          searchable: false,
          render: function (_data, _type, row) {
            var attributes = { idn: row.KsbCode, "ksb-value": row.KsbValue }; //idn KsbCode
            if (row.KsbMandatoryFlag) {
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
    $("#keyword-subject-read table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
      app.keyword.drawCallback();
    });

    //Create keyword event click
    $("#keyword-subject-read").find("[name=button-create]").once('click', function () {
      app.keyword.subject.modal.create();
    });
  }
};

//#endregion

//#region Update

/**
 * Ajax read Subject for update
 * @param {*} idn
 */
app.keyword.subject.ajax.readUpdate = function (idn) {
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_Subject_API.Read",
    { KsbCode: idn },
    "app.keyword.subject.callback.readUpdate");
};

/**
 * * Callback read
 * @param  {} data
 */
app.keyword.subject.callback.readUpdate = function (data) {
  if (data && Array.isArray(data) && data.length) {
    data = data[0];

    app.keyword.subject.modal.update(data);
  } else {
    api.modal.information(app.label.static["api-ajax-nodata"]);

    var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
    app.keyword.subject.ajax.read(selectedSubject.SbjCode);
  }
};

/**
 * Modal for update Keyword
 * @param  {} keywordRecord
 */
app.keyword.subject.modal.update = function (keywordRecord) {
  // Define Validation for Edit 
  app.keyword.subject.validation.update();
  //Populate values
  $("#keyword-subject-modal-update").find("[name=sbj-value]").empty().text(keywordRecord.SbjValue);
  $("#keyword-subject-modal-update").find("[name=ksb-value]").val(keywordRecord.KsbValue);
  $("#keyword-subject-modal-update").find("[name='ksb-code']").val(keywordRecord.KsbCode);
  //show modal
  app.keyword.subject.ajax.getLanguagesUpdate();

  if (keywordRecord.KsbSingularisedFlag) {
    $("#keyword-subject-modal-update").find("[name=acronym-toggle]").bootstrapToggle('off');
    $("#keyword-subject-modal-update").find("[name=language-row]").show();
  }
  else {
    $("#keyword-subject-modal-update").find("[name=acronym-toggle]").bootstrapToggle('on');
    $("#keyword-subject-modal-update").find("[name=language-row]").hide();
  };

  $("#keyword-subject-modal-update").find("[name=acronym-toggle]").once("change", function () {
    $("#keyword-subject-modal-update").find("[name=language-row]").toggle()
  });

  $("#keyword-subject-modal-update").modal("show");
};

/**
 * Call server for update
 */
app.keyword.subject.ajax.update = function () {
  var ksbCode = $("#keyword-subject-modal-update").find("[name='ksb-code']").val();
  var ksbValue = $("#keyword-subject-modal-update").find("[name=ksb-value]").val();
  var apiParams = {
    KsbCode: ksbCode,
    KsbValue: ksbValue,
    LngIsoCode: $("#keyword-subject-modal-update [name=acronym-toggle]").is(':checked') ? null : $("#keyword-subject-modal-update [name=language]").val()
  };
  var callbackParam = {
    KsbCode: ksbCode,
    KsbValue: ksbValue
  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_Subject_API.Update",
    apiParams,
    "app.keyword.subject.callback.updateOnSuccess",
    callbackParam,
    "app.keyword.subject.callback.updateOnError",
    null,
    { async: false }
  );
};

/**
 * Callback update Keyword for Subject
 * @param  {} data
 * @param  {} callbackParam
 */
app.keyword.subject.callback.updateOnSuccess = function (data, callbackParam) {
  $("#keyword-subject-modal-update").modal("hide");

  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  app.keyword.subject.ajax.read(selectedSubject.SbjCode);

  if (data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [callbackParam.KsbValue]));
  } else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

/**
 * Callback update Keyword for Subject
 * @param  {} error
 */
app.keyword.subject.callback.updateOnError = function (error) {
  $("#keyword-subject-modal-update").modal("hide");

  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  app.keyword.subject.ajax.read(selectedSubject.SbjCode);
};

/**
*  Get languages data from API to populate language drop down for create.
*/
app.keyword.subject.ajax.getLanguagesUpdate = function () {
  api.ajax.jsonrpc.request(app.config.url.api.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.keyword.subject.callback.getLanguagesUpdate");
};

/**
*  Populate language drop down for create.
*/
app.keyword.subject.callback.getLanguagesUpdate = function (data) {
  $("#keyword-subject-modal-update").find("[name=language]").empty().append($("<option>", {
    "text": app.label.static["select-uppercase"],
    "disabled": "disabled",
    "selected": "selected"
  }));
  $.each(data, function (key, value) {
    $("#keyword-subject-modal-update").find("[name=language]").append($("<option>", {
      "value": value.LngIsoCode,
      "text": value.LngIsoName
    }));
  });
}


/**
 * Update validation Keyword for Subject
 */
app.keyword.subject.validation.update = function () {
  $("#keyword-subject-modal-update form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "ksb-value": {
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
            return !$("#keyword-subject-modal-update [name=acronym-toggle]").is(':checked');
          }
        }
      }
    },
    errorPlacement: function (error, element) {
      $("#keyword-subject-modal-update").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.keyword.subject.ajax.update();
    }
  }).resetForm();
};
//#endregion

//#region Delete

/**
 * Delete keyword modal 
 */
app.keyword.subject.modal.delete = function () {
  var idn = $(this).attr("idn");
  var ksbValue = $(this).attr("ksb-value");//row.KsbValue
  var deletedKeyword = {
    idn: idn,
    KsbValue: ksbValue
  };
  api.modal.confirm(app.library.html.parseDynamicLabel("confirm-delete", [ksbValue]), app.keyword.subject.ajax.delete, deletedKeyword, idn);
};

/**
 *  Delete keyword ajax
 * @param  {} deletedKeyword
 */
app.keyword.subject.ajax.delete = function (deletedKeyword) {
  var apiParams = { KsbCode: deletedKeyword.idn };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_Subject_API.Delete",
    apiParams,
    "app.keyword.subject.callback.deleteOnSuccess",
    deletedKeyword,
    "app.keyword.subject.callback.deleteOnError",
    null,
    { async: false }
  );
};

/**
 *Ajax callback for delete
 * @param  {} data
 * @param  {} deletedKeyword
 */
app.keyword.subject.callback.deleteOnSuccess = function (data, deletedKeyword) {
  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  app.keyword.subject.ajax.read(selectedSubject.SbjCode);

  if (data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-deleted", [deletedKeyword.KsbValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
 *Ajax callback for delete
 * @param  {} error
 */
app.keyword.subject.callback.deleteOnError = function (error) {
  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  app.keyword.subject.ajax.read(selectedSubject.SbjCode);
};
//#endregion

//#region Create
/**
 * Read keyword subject ajax
 */

/**
 *Clear previous values and show modal
 */
app.keyword.subject.modal.create = function () {
  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  $("#keyword-subject-modal-create").find("[name='sbj-value']").text(selectedSubject.SbjValue);
  $("#keyword-subject-modal-create").find("[name='sbj-code']").val(selectedSubject.SbjCode);
  app.keyword.subject.ajax.getLanguagesCreate();
  app.keyword.subject.validation.create();

  $("#keyword-subject-modal-create").find("[name=language-row]").show()

  $("#keyword-subject-modal-create").find("[name=acronym-toggle]").bootstrapToggle('off');
  $("#keyword-subject-modal-create").find("[name=acronym-toggle]").once("change", function () {
    $("#keyword-subject-modal-create").find("[name=language-row]").toggle()
  });

  $("#keyword-subject-modal-create").modal("show");
};

/**
*  Get languages data from API to populate language drop down for create.
*/
app.keyword.subject.ajax.getLanguagesCreate = function () {
  api.ajax.jsonrpc.request(app.config.url.api.public,
    "PxStat.System.Settings.Language_API.Read",
    { LngIsoCode: null },
    "app.keyword.subject.callback.getLanguagesCreate");
};

/**
*  Populate language drop down for create.
*/
app.keyword.subject.callback.getLanguagesCreate = function (data) {
  $("#keyword-subject-modal-create").find("[name=language]").empty().append($("<option>", {
    "text": app.label.static["select-uppercase"],
    "disabled": "disabled",
    "selected": "selected"
  }));
  $.each(data, function (key, value) {
    $("#keyword-subject-modal-create").find("[name=language]").append($("<option>", {
      "value": value.LngIsoCode,
      "text": value.LngIsoName
    }));
  });
}

/**
 * Create keyword validation
 */
app.keyword.subject.validation.create = function () {
  $("#keyword-subject-modal-create form").trigger("reset").onSanitiseForm().validate({
    onkeyup: function (element) {
      this.element(element);
    },
    rules: {
      "ksb-value": {
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
            return !$("#keyword-subject-modal-create [name=acronym-toggle]").is(':checked');
          }
        }
      }
    },
    errorPlacement: function (error, element) {
      $("#keyword-subject-modal-create").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.keyword.subject.ajax.create();
    }
  }).resetForm();
};

/**
 * Keyword ajax create
 */
app.keyword.subject.ajax.create = function () {
  var sbjCode = $("#keyword-subject-modal-create").find("[name='sbj-code']").val();
  var ksbValue = $("#keyword-subject-modal-create").find("[name=ksb-value]").val();

  var apiParams = {
    KsbValue: ksbValue,
    SbjCode: sbjCode,
    LngIsoCode: $("#keyword-subject-modal-create [name=acronym-toggle]").is(':checked') ? null : $("#keyword-subject-modal-create [name=language]").val(),
    KsbMandatoryFlag: false
  };

  var callbackParam = {
    KsbValue: ksbValue,
    SbjCode: sbjCode,
  };

  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_Subject_API.Create",
    apiParams,
    "app.keyword.subject.callback.createOnSuccess",
    callbackParam,
    "app.keyword.subject.callback.createOnError",
    null,
    { async: false }
  );
};

/**
   * Create keyword
   * @param {*} data 
   * @param {*} callbackParam 
   */
app.keyword.subject.callback.createOnSuccess = function (data, callbackParam) {
  $("#keyword-subject-modal-create").modal("hide");

  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  app.keyword.subject.ajax.read(selectedSubject.SbjCode);

  if (data == C_APP_API_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-added", [callbackParam.KsbValue]));
  } else api.modal.exception(app.label.static["api-ajax-exception"]);
};

/**
   * Create keyword
   * @param {*} error 
   */
app.keyword.subject.callback.createOnError = function (error) {
  $("#keyword-subject-modal-create").modal("hide");

  var selectedSubject = $("#keyword-subject-container").find("select[name=select-main-subject-search]").select2('data')[0];
  app.keyword.subject.ajax.read(selectedSubject.SbjCode);
};
//#endregion

//#region Synonym Render

//Ajax call for synonym
app.keyword.subject.ajax.synonym = function (row) {
  var KsbValue = row.KsbValue;
  var callbackParam = {
    KsbValue: KsbValue,

  };
  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_API.ReadSynonym",
    { "KrlValue": KsbValue },
    "app.keyword.subject.callback.readSynonym",
    callbackParam);
}

//Call back for synonyms
app.keyword.subject.callback.readSynonym = function (data, callbackParam) {
  //Add name of keyword selected
  $("#keyword-subject-extra-info").find("[name=keyword]").html(callbackParam.KsbValue);
  //Clear list card
  $("#keyword-subject-extra-info").find("[name=synonym-card]").empty();

  //Get each language
  $.each(data, function (key, language) {
    //Clone card
    var languageCard = $("#keyword-subject-template").find("[name=synonym-language-card]").clone();
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
    }
    else {
      //Display if no synonyms
      var synonymItem = $("<li>", {
        "class": "list-group-item",
        "html": app.label.static["no-synonyms"]
      });
      languageCard.find("[name=synonym-group]").append(synonymItem);
    }

    $("#keyword-subject-extra-info").find("[name=synonym-card]").append(languageCard).get(0).outerHTML;

  });
}


//#endregion

//#region Search Synonym

//Ajax call for synonym
app.keyword.subject.ajax.searchSynonym = function () {

  var KsbValue = $("#keyword-subject-synonym-request").find("[name=sym-value").val();

  api.ajax.jsonrpc.request(
    app.config.url.api.private,
    "PxStat.System.Navigation.Keyword_API.ReadSynonym",
    { "KrlValue": KsbValue },
    "app.keyword.subject.callback.searchSynonym"
  );
}

//Call back for synonyms
app.keyword.subject.callback.searchSynonym = function (data) {
  //Clear list card
  $("#keyword-subject-synonym-request").find("[name=synonym-card]").empty();

  //Get each language
  $.each(data, function (key, language) {
    //Clone card
    var languageCard = $("#keyword-search-template").find("[name=synonym-language-card]").clone();
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
    }
    else {
      //Display if no synonyms
      var synonymItem = $("<li>", {
        "class": "list-group-item",
        "html": app.label.static["no-synonyms"]
      });
      languageCard.find("[name=synonym-group]").append(synonymItem);
    }

    $("#keyword-subject-synonym-request").find("[name=synonym-card]").append(languageCard).get(0).outerHTML;
    $("#keyword-subject-synonym-request").find("[name=synonym-card]").show();

  });
}


//#endregion


