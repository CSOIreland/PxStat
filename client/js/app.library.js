/*******************************************************************************
Application - Library 
*******************************************************************************/
var app = app || {};

app.library = {};
app.library.utility = {};
app.library.datatable = {};
app.library.html = {};
app.library.html.link = {};
app.library.bootstrap = {};
app.library.message = {};

//#region DataTable

/**
 * Re-draw an existing DataTable with (new) data
 * @param {*} selector
 * @param {*} data
 */
app.library.datatable.reDraw = function (selector, data) {
  if (data != null) {
    $(selector)
      .DataTable()
      .clear()
      .rows.add(data)
      .draw();
  } else {
    //if no data, just clear table and redraw with no rows
    $(selector)
      .DataTable()
      .clear()
      .draw();
  }
};

/**
 * @param {*} datatableSelector
 * @param {*} callbackFunction
 * @param {*} postCallbackFunction
 */
app.library.datatable.showExtraInfo = function (datatableSelector, callbackFunction, postCallbackFunction) {
  postCallbackFunction = postCallbackFunction || null;
  //Bind click on datatable selector because of responsive classes
  $(datatableSelector).find("[name=" + C_APP_DATATABLE_EXTRA_INFO_LINK + "]").once('click', function (e) {
    e.preventDefault();
    var idn = $(this).attr("idn");
    var rowIndexObj = $("[" + C_APP_DATATABLE_ROW_INDEX + "='" + idn + "']");

    //get data to display
    var dataTable = $(datatableSelector).DataTable();
    var dataTableRow = dataTable.row(rowIndexObj);
    // Run callback
    api.modal.information(callbackFunction(dataTableRow.data()));
    // Run post callback
    if (postCallbackFunction) {
      postCallbackFunction(dataTableRow.data());
    }
    // Refresh Prism highlight
    Prism.highlightAll();
    // Bootstrap tooltip
    $('[data-toggle="tooltip"]').tooltip();
  });
};

//#endregion

//#region HTML

/**
 * Parse the Static Labels into the HTML context
 */
app.library.html.parseStaticLabel = function () {

  // Parse all Labels in the DOM
  $("[label]").each(function (index) {
    // Get the keyword from the attribute value
    var keyword = $(this).attr("label");

    // If the Keyword exists in the Dictionary, then set it
    $(this).html(app.label.static[keyword] ? app.label.static[keyword] : "[" + keyword + "]");

    // Remove the attribute to avoid double-parsing
    $(this).removeAttr("label");
  });

  // Parse all Label Tooltips in the DOM
  $("[label-tooltip]").each(function (index) {
    // Get the keyword from the attribute value
    var keyword = $(this).attr("label-tooltip");

    // If the Keyword exists in the Dictionary
    if (app.label.static[keyword]) {
      // If the data-original-title attribute exists
      $(this).attr("data-original-title", app.label.static[keyword]);
    } else {
      $(this).attr("data-original-title", keyword);
    }

    $(this).removeAttr();
  });
};
/**
 * Parse the Static Labels into the HTML context
 */
app.library.html.parseDynamicLabel = function (keyword, params) {
  params = params || [];

  var label = app.label.dynamic[keyword];
  // If the Keyword exists in the Dictionary, then set it
  if (label) {
    return label.sprintf(params);
  } else
    return "";
};

/**
 * Generate an HTML boolean element
 * @param {*} bool
 * @param {*} showColor
 * @param {*} showFalse
 */
app.library.html.boolean = function (bool, showColor, showFalse) {
  showColor = showColor || false;
  showFalse = showFalse || false;

  if (!showFalse && (bool == undefined || bool == null)) {
    return "";
  }

  if (bool) {
    return $("<i>", {
      class: "fas fa-check-circle" + (showColor ? " text-success" : "")
    }).get(0).outerHTML + " ";
  }

  if (showFalse) {
    return $("<i>", {
      "class": "fas fa-times-circle" + (showColor ? " text-danger" : "")
    }).get(0).outerHTML + " ";
  }

  return "";
};

/**
 * Generate an HTML Locked element with a tooltip
 * @param {*} textElement
 * */
app.library.html.locked = function (textElement) {
  return $("<span>", {
    "data-toggle": "tooltip",
    "data-placement": "right",
    "title": app.label.static["locked"],
    html:
      $("<i>", {
        class: "text-warning fas fa-lock"
      }).get(0).outerHTML +
      " " +
      textElement
  }).get(0).outerHTML;
};

/**
 * Generate an HTML tooltip
 * @param {*} textElement
 * */
app.library.html.tooltip = function (textElement, name) {
  return $("<span>", {
    "data-toggle": "tooltip",
    "data-placement": "right",
    "title": name,
    html: textElement
  }).get(0).outerHTML;
};

/**
 * Generate an HTML group privilege element
 * @param {*} isApprover
 */
app.library.html.groupRole = function (approveFlag) {
  if (approveFlag === true) {
    return $("<i>", {
      class: "app-html-group-role fas fa-user-check text-success",
      "data-toggle": "tooltip",
      "title": app.label.static["approver"]
    }).get(0).outerHTML; // + " " + entry.GrpName;
  } else {
    return $("<i>", {
      class: "app-html-group-role fas fa-user-edit text-orange",
      "data-toggle": "tooltip",
      "title": app.label.static["editor"]
    }).get(0).outerHTML; // + " " + entry.GrpName;
  }
};

/**
 * Generate an HTML email link
 * @param {*} email
 */
app.library.html.email = function (email) {
  if (!email) {
    return "";
  } else if (!email.match(C_APP_REGEX_EMAIL)) {
    // Check against regex before displaying an email link
    return email;
  } else {
    return $("<a>", {
      href: "mailto:" + email,
      html:
        $("<i>", {
          class: "fas fa-envelope"
        }).get(0).outerHTML +
        " " +
        email
    }).get(0).outerHTML;
  }
};



app.library.html.deleteButton = function (attributes, disabled) {

  disabled = disabled || false;

  var deleteButton = $("<button>", {
    class: "btn btn-outline-danger btn-sm delete-btn",
    name: C_APP_NAME_LINK_DELETE,
    html:
      $("<i>", {
        class: "fas fa-trash-alt"
      }).get(0).outerHTML + " " + app.label.static["delete"]
  });

  $.each(attributes, function (key, value) {
    deleteButton.attr(key, value);
  });

  if (disabled) {
    deleteButton.prop("disabled", true);
  }

  return deleteButton.get(0).outerHTML;

};
/**
 * Format bbcode as html
 * @param  {} bbCode
 */
app.library.html.parseBbCode = function (bbCode) {
  bbCode = bbCode || "";
  var tags = {
    "[b](.+?)[/b]": '<b>$1</b>',
    "[i](.+?)[/i]": '<i>$1</i>',
    "[u](.+?)[/u]": '<u>$1</u>',
    "[url=(.+?)](.*?)[/url]": function (match, href, text) {
      return $("<a>", {
        "href": href,
        "text": text,
        "target": "_blank"
      }).get(0).outerHTML;

    }
  };
  for (var tag in tags) {
    var regTag = tag.replace(/[\[\]]/g, '\\$&');
    var regExp = new RegExp(regTag, 'gi');

    bbCode = bbCode.replace(regExp, tags[tag]);
    bbCode = bbCode.replace(/\\n/g, "<br>");
  }

  return bbCode;

};

//#endregion
//#region HTML link

// Base constructor for HTML links to extend
app.library.html.link.baseConstructor = function (attributes, textElement, textTootip, iconElement) {
  textElement = textElement || "";
  textTootip = textTootip || null;
  // Init params  
  var linkParams = {
    html:
      $("<i>", {
        class: iconElement
      }).get(0).outerHTML +
      " " +
      textElement,

    "data-toggle": textTootip ? "tooltip" : null,
    title: textTootip
  };

  $.extend(true, linkParams, attributes);
  return $("<a>", linkParams).get(0).outerHTML;
};

app.library.html.link.edit = function (attributes, textElement, textTootip) {
  $.extend(true, attributes, {
    name: C_APP_NAME_LINK_EDIT,
    href: "#",
  });

  return app.library.html.link.baseConstructor(attributes, textElement, textTootip, "far fa-edit");
};

app.library.html.link.view = function (attributes, textElement, textTootip) {
  $.extend(true, attributes, {
    name: C_APP_NAME_LINK_VIEW,
    href: "#",
  });

  return app.library.html.link.baseConstructor(attributes, textElement, textTootip, "fas fa-eye");
};

app.library.html.link.geoJson = function (attributes, textElement, textTootip) {
  $.extend(true, attributes, {
    name: C_APP_NAME_LINK_GEOJSON,
    href: "#",
  });

  return app.library.html.link.baseConstructor(attributes, textElement, textTootip, "fas fa-eye");
};

app.library.html.link.analytic = function (attributes, textElement, textTootip) {
  $.extend(true, attributes, {
    name: C_APP_NAME_LINK_ANALYTIC,
    href: "#",
  });

  return app.library.html.link.baseConstructor(attributes, textElement, textTootip, "far fa-chart-bar");
};

app.library.html.link.external = function (attributes, url, textTootip) {
  textTootip = textTootip || null;

  $.extend(true, attributes, {
    href: url,
    target: "_blank",
  });

  return app.library.html.link.baseConstructor(attributes, url, textTootip, "fas fa-link");
};

app.library.html.link.internal = function (attributes, textElement, textTootip) {
  textTootip = textTootip || null;

  $.extend(true, attributes, {
    name: C_APP_NAME_LINK_INTERNAL,
    href: "#"
  });

  return app.library.html.link.baseConstructor(attributes, textElement, textTootip, "far fa-arrow-alt-circle-right");
};

/**
 * Generate an HTML element with the link to the Modal User
 * @param {*} username
 */
app.library.html.link.user = function (username) {
  var userLink = $("<a>", {
    idn: username,
    "text": username,
    "href": "#"
  }).get(0);
  userLink.addEventListener("click", function (e) {
    e.preventDefault();
    app.library.user.modal.read({ CcnUsername: username });
  });

  return userLink;
};

/**
 * Generate an HTML element with the link to the Modal User
 * @param {*} username
 */
app.library.html.link.group = function (grpCode) {
  var groupLink = $("<a>", {
    idn: grpCode,
    "html": grpCode,
    "href": "#"
  }).get(0);
  groupLink.addEventListener("click", function (e) {
    e.preventDefault();
    app.library.group.modal.read(grpCode);
  });

  return groupLink;
};


//#endregion

//#region Bootstrap

/**
 * Toggle Icons in the Bootstrap accordions
 */
app.library.bootstrap.accordianToggleIcon = function () {
  $(".collapse").on("shown.bs.collapse", function () {
    // First, set all to plus
    $(this)
      .parent()
      .parent()
      .find(".collapse")
      .parent()
      .find("button")
      .find("i")
      .removeClass()
      .addClass("fas fa-plus-circle");
    // Then set only the shown elements to minus
    $(this)
      .parent()
      .parent()
      .find(".collapse.show")
      .parent()
      .find("button")
      .find("i")
      .removeClass()
      .addClass("fas fa-minus-circle");
  });

  $(".collapse").on("hidden.bs.collapse", function () {
    // First, set all to plus
    $(this)
      .parent()
      .parent()
      .find(".collapse")
      .parent()
      .find("button")
      .find("i")
      .removeClass()
      .addClass("fas fa-plus-circle");
    // Then set only the shown elements to minus
    $(this)
      .parent()
      .parent()
      .find(".collapse.show")
      .parent()
      .find("button")
      .find("i")
      .removeClass()
      .addClass("fas fa-minus-circle");
  });
};
//#endregion

//#region Message

//#endregion


//#region Utility

/**
 * Return randon number with optional prefix
 * @param  {} prefix
 */
app.library.utility.randomGenerator = function (prefix) {
  prefix = prefix || "";
  return prefix + Math.floor(Math.random() * 999999999) + 1;
};

/**
 * format DateTime
 * @param  {*} dateTimeValue
 */
app.library.utility.formatDatetime = function (dateTimeValue) {
  var dateTimeFormated = "";
  var dateTimeValue = dateTimeValue == undefined ? "" : dateTimeValue;
  if (moment(dateTimeValue).isValid()) {
    // do something because moment was valid
    dateTimeFormated = moment(dateTimeValue).format(app.config.mask.datetime.display);
  } else {
    //  do something else because it was invalid
    dateTimeFormated = "";
  }
  return dateTimeFormated;
};
/**
 * format any number
 * @param  {} number
 * @param  {} decimalSeparator
 * @param  {} thousandSeparator
  */
app.library.utility.formatNumber = function (number, decimalSeparator, thousandSeparator, precision) { //create global function  
  decimalSeparator = decimalSeparator || app.config.separator.decimal.display;
  thousandSeparator = thousandSeparator || app.config.separator.thousand.display;
  precision = precision !== undefined ? precision : undefined;

  if ("number" !== typeof number && "string" !== typeof number)
    return number;

  floatNumber = parseFloat(number);
  if (isNaN(floatNumber))
    //output any non number as html
    return "string" === typeof number ? number.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;") : number;

  if (precision !== undefined) {
    floatNumber = floatNumber.toFixed(precision);
  }
  else {
    floatNumber = floatNumber.toString();
  }

  var parts = floatNumber.split(".");
  var wholeNumber = parts[0].toString();
  var decimalNumber = parts[1] !== undefined ? parts[1].toString() : undefined;
  return (thousandSeparator ? wholeNumber.toString().replace(/\B(?=(\d{3})+(?!\d))/g, thousandSeparator) : wholeNumber) + (decimalNumber !== undefined ? decimalSeparator + decimalNumber : "");
};

/**
 * Initiate tinyMce
 */
app.library.utility.initTinyMce = function () {
  //if any editor exists destroy before init new text area
  if (tinymce.editors.length) {
    tinymce.remove();
  }

  $(document).on('focusin', function (e) {
    //fix for link field not editable in bootstrap modal
    //see https://stackoverflow.com/questions/18111582/tinymce-4-links-plugin-modal-in-not-editable
    if ($(e.target).closest(".tox-dialog").length) {
      e.stopImmediatePropagation();
    }
  });

  // Add language
  tinymce.addI18n(app.label.language.iso.code, app.label.plugin.tinyMCE);

  return tinymce.init({
    selector: 'textarea',
    paste_as_text: true,
    menubar: false,
    plugins: 'bbcode autolink link paste',
    toolbar: 'bold italic underline | link',
    browser_spellcheck: true,
    link_title: false,
    target_list: false,
    default_link_target: "_blank",
    language: app.label.language.iso.code,
    setup: function (editor) {
      // Let the editor save every change to the textarea
      editor.once('change', function () {
        // ID is dynamically generated by TinyMce, get out if the ID is not ready yet
        if (!tinymce.get(this.id))
          return;

        var content = tinymce.get(this.id).getContent();
        if (C_APP_REGEX_BBCODE_NOT_ALLOWED.test(content)) {
          tinymce.get(this.id).setContent(content.replace(C_APP_REGEX_BBCODE_NOT_ALLOWED, ""));
        };

        tinymce.triggerSave();
      });

      // Do nothing when submitting the form
      editor.on('submit', function (e) {
        return false;
      });
    }
  });
};

/**
 * Compare 2 arrays to see if they are the same. Position not important 
 */
app.library.utility.arraysEqual = function (array1, array2) {

  var isEqual = true;

  //first check if lengths are the same
  if (array1.length != array2.length) {
    isEqual = false
    return isEqual;
  }

  //Array lengths are the same, now chenk that contents are the same

  $.each(array1, function (index, value) {
    //For each item in array1, find out how many times it appears, must be the same in array2, else return false
    //order doesn't matter
    var countArray1 = 0;
    for (var i = 0; i < array1.length; i++) {
      if (array1[i] === value) {
        countArray1++;
      }
    };

    var countArray2 = 0;
    for (var x = 0; x < array2.length; x++) {
      if (array2[x] === value) {
        countArray2++;
      }
    };

    //count must be the same for item in both arrays, else return false
    if (countArray1 != countArray2) {
      isEqual = false
      return isEqual;
    }

  });
  //all items must match, return true
  return isEqual;
}

/**
 * Implement a cookieLink
 */
app.library.utility.cookieLink = function (cookie, goTo, relativeURL, nav_link_SelectorToHighlight, nav_menu_SelectorToHighlight) {
  nav_link_SelectorToHighlight = nav_link_SelectorToHighlight || null;
  nav_menu_SelectorToHighlight = nav_menu_SelectorToHighlight || null;

  // Check a Cookie is set
  if (Cookies.get(cookie)) {

    // Map the goTo params
    var goToParams = {};
    goToParams[goTo] = Cookies.get(cookie);

    // Remove the Cookie
    Cookies.remove(cookie, app.config.plugin.jscookie);

    // Loading the required entity
    api.content.goTo(relativeURL, nav_link_SelectorToHighlight, nav_menu_SelectorToHighlight, goToParams);
  }
  return false;
};
/**
 * Check for duplicate items in an array
 */
app.library.utility.arrayHasDuplicate = function (items) {
  //normalise items
  $.each(items, function (index, value) {
    value = value.trim().toLowerCase();
  });

  var counts = [];
  for (var i = 0; i <= items.length; i++) {
    if (counts[items[i]] === undefined) {
      counts[items[i]] = 1;
    } else {
      return true;
    }
  }
  return false;
};
//#endregion