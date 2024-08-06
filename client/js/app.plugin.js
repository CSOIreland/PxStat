/*******************************************************************************
Application - Plugin 
*******************************************************************************/
var app = app || {};

app.plugin = {};

/*******************************************************************************
Application - Plugin - sanitise
*******************************************************************************/

// Sanitise constants for Casing
const C_SANITISE_UPPERCASE = "C_SANITISE_UPPERCASE";
const C_SANITISE_LOWERCASE = "C_SANITISE_LOWERCASE";
const C_SANITISE_NOCASE = "C_SANITISE_NOCASE";

String.prototype.sanitise = String.prototype.sanitise || function (pCase, pRegex, pNoTrim) {
  pCase = pCase || C_SANITISE_NOCASE;
  pRegex = pRegex || null;
  pNoTrim = pNoTrim || false;

  var value = this;

  if (!pNoTrim) {
    //trim the value
    value = value.trim();
  }
  if (pRegex) {
    // Sanitise according to the regex      
    value = value.replace(pRegex, "");
  }
  // Format case accordingly
  switch (pCase) {
    case C_SANITISE_UPPERCASE:
      value = value.toUpperCase(value);
      break;
    case C_SANITISE_LOWERCASE:
      value = value.toLowerCase(value);
      break;
    case C_SANITISE_NOCASE:
    default:
      // Do nothing
      break;
  }

  return value;
};

/*******************************************************************************
Application - Plugin - Moment for Datatable
*******************************************************************************/
$.fn.dataTable.moment(app.config.mask.datetime.display);

/*******************************************************************************
Application - Plugin - onSanitise
*******************************************************************************/

/**
 * Sanitise a form on event
 */
jQuery.fn.onSanitiseForm = function (pEvent, pHtmlEntities) {
  pEvent = pEvent || "keyup change";
  pHtmlEntities = pHtmlEntities || false;

  if (!pHtmlEntities) {
    this.find("input, textarea").each(function () {
      if (!$(this).is(':file')) {
        $(this).off(pEvent).bind(pEvent, function () {
          //strip HTML
          this.value = this.value.replace(C_APP_REGEX_NOHTML, "");
          //convert HTML entities
          this.value = $(this).html(this.value).text();
        });
      }
    });
  }
  return this;
};

/**
 * Sanitise a form on submit
 */
jQuery.fn.sanitiseForm = function () {
  this.find("input, textarea").each(function () {
    if (!$(this).is(':file')) {
      //Trim
      this.value = this.value.trim();
    }
  });

  return this;
};
/*******************************************************************************
Application - Plugin - Extend JQuery Validator - https://jqueryvalidation.org/
*******************************************************************************/

/**
 * Validation required fields
 */
jQuery.validator.addMethod("required", function (value, element) {
  value = $.trim(value);
  return value.length ? true : false;
}, app.label.static["mandatory"]);


/**
 * Validation validEmailAddress
 */
jQuery.validator.addMethod("validEmailAddress", function (value, element) {
  return this.optional(element) || C_APP_REGEX_EMAIL.test(value);
}, app.label.static["invalid-format"]);

/**
 * Validation validMatrix
 */
jQuery.validator.addMethod("validMatrix", function (value, element) {
  return this.optional(element) || new RegExp(app.config.regex["matrix-name"]).test(value);
}, app.label.static["invalid-format"]);

/**
 * Validation validPhoneNumber
 */
jQuery.validator.addMethod("validPhoneNumber", function (value, element) {
  var pattern = new RegExp(app.config.regex.phone.pattern);
  return this.optional(element) || pattern.test(value);
}, app.label.dynamic["invalid-format"].sprintf([app.config.regex.phone.placeholder]));

/**
 * Validation password
 */
jQuery.validator.addMethod("validPassword", function (value, element) {
  var pattern = new RegExp(app.config.regex.password);
  var result = this.optional(element) || pattern.test(value);
  if (!result) {
    $("a[name=password-requirements]").popover('show');
  }
  else {
    $("a[name=password-requirements]").popover('hide');
  }
  return result
}, app.label.static["invalid-password"]);


/**
 * Validation ip Mask
 */
jQuery.validator.addMethod("validIpMask", function (value, element) {
  return this.optional(element) || C_APP_REGEX_IP_MASK.test(value);
}, app.label.static["invalid-ip-mask"]);

/**
 * Validation ip address
 */
jQuery.validator.addMethod("validIp", function (value, element) {
  return this.optional(element) || C_APP_REGEX_IP.test(value);
}, app.label.static["invalid-ip-address"]);


jQuery.validator.addMethod("notEqual", function (value, element, param) {
  return this.optional(element) || value != $(param).val();
}, app.label.static["statistic-error-message"]);

/**
 * Validation dimension code
 */
jQuery.validator.addMethod("validDimensionCode", function (value, element) {
  var isValid = true;

  if (jQuery.inArray(value.toUpperCase(), [C_APP_CSV_VALUE, C_APP_CSV_UNIT, C_APP_CSV_STATISTIC]) < 0) {
    isValid = false;
  }

  return this.optional(element) || !isValid;
}, "Invalid Dimension Code");


/*******************************************************************************
Application - Plugin - Extend JQuery Validator - https://jqueryvalidation.org/ - translate messages
*******************************************************************************/

$.extend(true, jQuery.validator.messages, {
  "url": app.label.static["valid-url"]
});

/*******************************************************************************
Application - Plugin - load ShareThis library with key https://sharethis.com/
*******************************************************************************/
app.plugin.sharethis = {};

//Load dynamically the source of ShareThis by using the API Key
app.plugin.sharethis.load = function (drawShareThis) {
  drawShareThis = drawShareThis || false;
  if (!window.__sharethis__ && app.config.plugin.sharethis.enabled) {
    //Load dynamically ShareThis
    jQuery.ajax({
      "url": app.config.plugin.sharethis.apiURL.sprintf([app.config.plugin.sharethis.apiKey]),
      "dataType": "script",
      "async": false,
      "success": function () {
        if (app.data && app.data.MtrCode && $("#data-dataset-selected-table").is(":visible")) { //in dataset view
          app.data.share(app.data.MtrCode, null);
        }
        if (app.data && !app.data.isSearch && app.data.PrdCode && $("#data-filter").is(":visible")) {
          app.data.share(null, app.data.PrdCode);
        }

      },
      "error": function (jqXHR, textStatus, errorThrown) {
        console.log("ShareThis failed to load: " + errorThrown)
      }
    });
  }
};

/*******************************************************************************
Application - Plugin - Datatable
*******************************************************************************/

// Extend the datatable configuration with the language parameters
$.extend(true, app.config.plugin.datatable, app.label.datatable);

/*******************************************************************************
Application - Plugin - Bootstrap Modal
*******************************************************************************/
// For printing the overlay isolated from anything in the background.
$(document).ready(function () {
  $('body').on('show.bs.modal', function (e) {
    var parents = $("#" + e.target.id).parents();
    var parentIds = [];
    $.each(parents, function (key, value) {
      parentIds.push(value.id);
    });

    //update non modal divs to no print css
    $('#cookie, #alert, #header, #navigation, #content, #footer, #spinner').addClass('d-print-none');
  });

  $('body').on('hide.bs.modal', function (e) {
    var parents = $("#" + e.target.id).parents();
    var parentIds = [];
    $.each(parents, function (key, value) {
      parentIds.push(value.id);
    });

    //update non modal divs to print css
    $('#cookie, #alert, #header, #navigation, #content, #footer, #spinner').removeClass('d-print-none');

    //catch all listener to reset password fields if they exist in the modal
    $(this).find("[name=hide-password]").trigger("click");
  });
});

// For modal over modal scenario such as confirm. 
// When top modal closed, we need to be able to scroll on existing modal by adding modal-open class to body
// https://stackoverflow.com/questions/28077066/bootstrap-modal-issue-scrolling-gets-disabled comment 34
$(document).ready(function () {
  $('body').on('hidden.bs.modal', function () {
    if ($('.modal.show').length > 0) {
      $('body').addClass('modal-open');
    }
  });
});

/*******************************************************************************
Application - Plugin - Datatable data sorting
*******************************************************************************/
jQuery.extend(jQuery.fn.dataTableExt.oSort, {
  "data-asc": function (a, b) {
    a = a.toString().replace(app.config.entity.data.datatable.null, -9999999999).replaceAll(app.library.utility.thousandSeparator(), "").replaceAll(app.library.utility.decimalSeparator(), ".");
    b = b.toString().replace(app.config.entity.data.datatable.null, -9999999999).replaceAll(app.library.utility.thousandSeparator(), "").replaceAll(app.library.utility.decimalSeparator(), ".");
    return jQuery.fn.dataTableExt.oSort["natural-nohtml-asc"](a, b);
  },
  "data-desc": function (a, b) {
    a = a.toString().replace(app.config.entity.data.datatable.null, -9999999999).replaceAll(app.library.utility.thousandSeparator(), "").replaceAll(app.library.utility.decimalSeparator(), ".");
    b = b.toString().replace(app.config.entity.data.datatable.null, -9999999999).replaceAll(app.library.utility.thousandSeparator(), "").replaceAll(app.library.utility.decimalSeparator(), ".");
    return jQuery.fn.dataTableExt.oSort["natural-nohtml-desc"](a, b);
  }
});


/*******************************************************************************
Application - Plugin - Cookie consent
*******************************************************************************/
app.plugin.cookiconsent = {};
app.plugin.cookiconsent.true = "true";
app.plugin.cookiconsent.false = "false";

app.plugin.cookiconsent.allow = function (drawShareThis) {
  drawShareThis = drawShareThis || false;
  // Set to TRUE the Cookie Consent
  Cookies.set(C_COOKIE_CONSENT, app.plugin.cookiconsent.true, app.config.plugin.jscookie.persistent);
  // Load ShareThis following Cookie Consent
  app.plugin.sharethis.load(drawShareThis);
  // Hide the banner
  $("#cookie").find("[name=cookie-banner]").fadeOut();
};

app.plugin.cookiconsent.deny = function (reload) {
  reload = reload || false;
  // Set to FALSE the Cookie Consent
  Cookies.set(C_COOKIE_CONSENT, app.plugin.cookiconsent.false, app.config.plugin.jscookie.persistent);

  if (reload) {
    // Prevent back-button check

    // Force page reload in order to unload (not set at all) cookies from different domains (i.e. sharethis)
    window.location.href = window.location.pathname;
  } else {
    $("#cookie").find("[name=cookie-banner]").fadeOut();
  }
};

/*******************************************************************************
Application - Plugin - Tiny MCE
*******************************************************************************/

app.plugin.tinyMce = {};

/**
 * Configure tinyMce once
 */
$(document).ready(function () {
  if (tinymce) {
    $(document).once('focusin', function (e) {
      //fix for link field not editable in bootstrap modal
      //see https://stackoverflow.com/questions/18111582/tinymce-4-links-plugin-modal-in-not-editable
      if ($(e.target).closest(".tox-dialog").length) {
        e.stopImmediatePropagation();
      }
    });

    // Add language
    tinymce.addI18n(app.label.language.iso.code, app.label.plugin.tinyMCE);
  }
});

/**
 * Initiate tinyMce on demand
 */
app.plugin.tinyMce.initiate = function (stripDoubleQuotes) {
  stripDoubleQuotes = stripDoubleQuotes || false;
  //if any editor exists destroy before init new text area
  if (tinymce.editors.length) {
    tinymce.remove();
  }

  return tinymce.init({
    //https://www.tiny.cloud/docs-3x/reference/configuration/Configuration3x@force_br_newlines/
    force_br_newlines: true,
    force_p_newlines: false,
    forced_root_block: false,

    selector: 'textarea[type="tinymce"]',
    min_height: 100,
    paste_as_text: true,
    menubar: false,
    plugins: 'bbcode autolink link paste',
    toolbar: 'bold italic underline | link',
    browser_spellcheck: true,
    link_title: false,
    target_list: false,
    default_link_target: "_blank",
    language: app.label.language.iso.code,
    entity_encoding: "raw",
    setup: function (editor) {
      // Trigger save on focus out so that cursor focus doesn't move
      // Do NOT use the "change" event because it is indirectly called by tinyMce to parse HTML characters 
      editor.on('focusout', function () {
        var isSanitised = false;

        var content = tinymce.get(this.id).getContent();
        if (C_APP_REGEX_BBCODE_NOT_ALLOWED.test(content)) {
          isSanitised = true;
          tinymce.get(this.id).setContent(content.sanitise(null, C_APP_REGEX_BBCODE_NOT_ALLOWED, true));
        };

        var content = tinymce.get(this.id).getContent();
        if (stripDoubleQuotes && C_APP_REGEX_NODOUBLEQUOTE.test(content)) {
          isSanitised = true;
          tinymce.get(this.id).setContent(content.sanitise(null, C_APP_REGEX_NODOUBLEQUOTE, true));
        }

        // Save all (anyway)
        tinymce.triggerSave();
        // Get sanitised content
        content = tinymce.get(this.id).getContent();

        // Format the structured information message
        if (isSanitised) {
          contentPreview = $("<p>", {
            class: "bg-default p-2 my-2",
            html: app.library.html.parseBbCode(content)
          }).get(0).outerHTML;

          api.modal.information(app.library.html.parseDynamicLabel("disallowed-tinymce-characters", [contentPreview]));

          // Refresh Prism highlight
          Prism.highlightAll();
        }
      });

      // Do nothing when submitting the form
      editor.on('submit', function (e) {
        return false;
      });
    },
  });
};
