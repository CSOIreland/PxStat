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
 * Validation validPhoneNumber
 */
jQuery.validator.addMethod("validPhoneNumber", function (value, element) {
  var pattern = new RegExp(app.config.regex.phone.pattern);
  return this.optional(element) || pattern.test(value);
}, app.label.dynamic["invalid-format"].sprintf([app.config.regex.phone.placeholder]));


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
Application - Plugin - load tinyMce library with key https://www.tiny.cloud/
*******************************************************************************/

//Load dynamically the source of TinyMce by using the API Key
loadTinyMce();
function loadTinyMce() {
  var tinyMce = document.createElement('script');
  tinyMce.src = app.config.plugin.tinymce.apiURL.sprintf([app.config.plugin.tinymce.apiKey]);
  document.head.appendChild(tinyMce);
}

/*******************************************************************************
Application - Plugin - load ShareThis library with key https://sharethis.com/
*******************************************************************************/

//Load dynamically the source of TinyMce by using the API Key
loadShareThis();
function loadShareThis() {
  var shareThis = document.createElement('script');
  shareThis.src = app.config.plugin.sharethis.apiURL.sprintf([app.config.plugin.sharethis.apiKey]);
  document.head.appendChild(shareThis);
}

/*******************************************************************************
Application - Plugin - JQuery extensions
*******************************************************************************/

//Unbind all events prior to binding a new event using .on
(function ($) {
  $.fn.once = function () {
    return this.off(arguments[0]).on(arguments[0], arguments[1]);
  };
})(jQuery);

/*******************************************************************************
Application - Plugin - Datatable
*******************************************************************************/

// Extend the datatable configuration with the language parameters
$.extend(true, app.config.plugin.datatable, app.label.datatable);

/*******************************************************************************
Application - Plugin - Highcharts
*******************************************************************************/

if (app.config.plugin.highcharts.enabled
  && !jQuery.isEmptyObject(app.label.plugin.highcharts.lang)) {
  Highcharts.setOptions({
    // Extend the Highcharts lang 
    lang: app.label.plugin.highcharts.lang,
    credits: app.config.plugin.highcharts.credits,
    exporting: {
      buttons: {
        contextButton: {
          text: app.label.static["download"],
          onclick: function () {
            window._avoidbeforeunload = true;
            this.exportChart();
          }
        }
      }
    }
  });
}

/*******************************************************************************
Application - Plugin - Google Charts
*******************************************************************************/
google.charts.load('current', { 'packages': ['corechart'], 'language': app.label.language.iso.code });

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

    //only if the modal is in the overlay do we change the print screen
    //api modals are ignored and page is printed as normal
    if (jQuery.inArray("overlay", parentIds) != -1) {
      $('#alert, #header, #navigation, #body, #sidebar, #panel, #footer, #modal, #spinner').addClass('d-print-none');
    }
  });

  $('body').on('hide.bs.modal', function (e) {
    var parents = $("#" + e.target.id).parents();
    var parentIds = [];
    $.each(parents, function (key, value) {
      parentIds.push(value.id);
    });
    //only if the modal is in the overlay do we change the print screen
    //api modals are ignored and page is printed as normal
    if (jQuery.inArray("overlay", parentIds) != -1) {
      $('#alert, #header, #navigation, #body, #sidebar, #panel, #footer, #modal, #spinner').removeClass('d-print-none');
    }
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
Application - Plugin - Bootstrap breakpoint
*******************************************************************************/
$(document).ready(function () {
  bsBreakpoints.toggle = function (breakPoint) {

    //display breakpoint in hidden div in footer
    $("#footer").find("[name=bs-breakpoint]").text(breakPoint);

    switch (breakPoint) {
      case "xSmall":
      case "small":
      case "medium":
        //side panel for all entities
        if (!$("#panel").is(':empty')) {
          $("#panel").hide();
          $("#panel-toggle").show();
          $("#panel-toggle").find("i").removeClass().addClass("fas fa-plus-circle");
        }
        else {
          $("#panel-toggle").hide();
        }

        //move search input
        if ($("#data-search-row-desktop").is(":visible") || $("#data-search-row-responsive").is(":visible")) {
          $("#data-search-row-responsive").show();
          $("#data-search-row-desktop").hide();
        }

        //Collapse data navigation always on small
        $("#data-navigation").find(".navbar-collapse").collapse('hide');

        //if search results on page
        if ($("#data-metadata-row").find("[name=search-results]").is(":visible") && !$("#data-metadata-row").find("[name=search-results]").is(":empty")) {
          $("#data-filter-toggle").show();
          $("#data-filter").hide();
          $("#data-filter-toggle").find("i").removeClass().addClass("fas fa-plus-circle");
        }

        //if in data views
        if (!$("#data-dataview-selected-table").is(":empty")) {
          $("#data-filter-toggle").hide();
        }

        break;
      case "large":
      case "xLarge":
      default:
        //default position for search input
        if ($("#data-search-row-desktop").is(":visible") || $("#data-search-row-responsive").is(":visible")) {
          $("#data-search-row-desktop").show();
          $("#data-search-row-responsive").hide();
        };

        if ($("#panel").is(":empty")) {
          $("#data-navigation").find(".navbar-collapse").collapse('show');
        }

        //always show panel
        $("#panel").show();

        //never show toggle buttons
        $("#panel-toggle").hide();
        $("#data-filter-toggle").hide();

        //if search results on page
        if ($("#data-metadata-row").find("[name=search-results]").is(":visible") && !$("#data-metadata-row").find("[name=search-results]").is(":empty")) {
          $("#data-filter").show();
          $("#data-navigation").find(".navbar-collapse").collapse('hide');
        };
        break;
    }
  }
});

/*******************************************************************************
Application - Plugin - Datatable data sorting
*******************************************************************************/
jQuery.extend(jQuery.fn.dataTableExt.oSort, {
  "data-asc": function (a, b) {
    a = a.toString().replace(new RegExp(app.config.separator.thousand.display, 'g'), "");
    b = b.toString().replace(new RegExp(app.config.separator.thousand.display, 'g'), "");
    return jQuery.fn.dataTableExt.oSort["natural-nohtml-asc"](a, b);
  },
  "data-desc": function (a, b) {
    a = a.toString().replace(new RegExp(app.config.separator.thousand.display, 'g'), "");
    b = b.toString().replace(new RegExp(app.config.separator.thousand.display, 'g'), "");
    return jQuery.fn.dataTableExt.oSort["natural-nohtml-desc"](a, b);
  }
});

/*******************************************************************************
Application - Plugin - Back button detection
*******************************************************************************/
window.addEventListener("beforeunload", function (event) {
  if (!window._avoidbeforeunload) {
    // Cancel the event as stated by the standard.
    event.preventDefault();
    // Chrome requires returnValue to be set.
    event.returnValue = '';
  }

  // reset anyway
  window._avoidbeforeunload = false
});


/*******************************************************************************
Application - Plugin - Cookie consent
*******************************************************************************/
$(document).ready(function () {
  // Set the options from the config and the label
  window.cookieconsent.initialise($.extend(true, {}, app.config.plugin.cookieConsent, app.label.plugin.cookieConsent));

  // Bind load
  $(".cc-link").one('click', function (e) {
    e.preventDefault();

    // Load the Privacy (language specific) into the Modal
    api.content.load("#modal-read-privacy .modal-body", "internationalisation/privacy/" + app.label.language.iso.code + ".html");
    $("#modal-read-privacy").modal("show");
  });
});