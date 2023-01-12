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
app.library.utility.asyncControlCounter = {};

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
app.library.html.parseStaticLabel = function (keyword) {
  keyword = keyword || null;

  if (keyword) {
    // If the Keyword exists in the Dictionary, then return it
    return app.label.static[keyword] ? app.label.static[keyword] : "[" + keyword + "]";
  }
  else {
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

    // Parse all Label popover in the DOM
    $("[label-popover]").each(function (index) {
      // Get the keyword from the attribute value
      var keyword = $(this).attr("label-popover");

      // If the Keyword exists in the Dictionary
      if (app.label.help[keyword]) {
        // If the data-original-title attribute exists
        $(this).attr("data-content", app.label.help[keyword]);
      } else {
        $(this).attr("data-content", keyword);
      }

      $(this).removeAttr("label-popover");
      $(this).popover({
        html: true
      });
    });
  }

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
  }

  // Parse carriage return
  return bbCode.replace(/(\r\n|\n|\r)/g, "<br>");
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
    app.library.user.modal.ajax.read({ CcnUsername: username });
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
app.library.utility.formatNumber = function (number, precision) {
  precision = precision !== undefined ? precision : undefined;
  var decimalSeparator = app.library.utility.decimalSeparator();
  var thousandSeparator = app.library.utility.thousandSeparator();

  if ("number" !== typeof number && "string" !== typeof number)
    return number;

  if (isNaN(number)) { //output any non number as html
    return "string" === typeof number ? number.replace(/</g, "&lt;").replace(/>/g, "&gt;").replace(/"/g, "&quot;") : number;
  }
  else {
    floatNumber = parseFloat(number);
  }

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

app.library.utility.decimalSeparator = function () {
  var n = 1.1;
  return n.toLocaleString().substring(1, 2);
}

app.library.utility.thousandSeparator = function () {
  var n = 1000;
  return n.toLocaleString().substring(1, 2);
}

/**
 * Compare 2 arrays to see if they are the same. 
 *  */
app.library.utility.arraysEqual = function (array1, array2, order) {
  order = order || false;
  var isEqual = true;

  //first check if lengths are the same
  if (array1.length != array2.length) {
    isEqual = false
    return isEqual;
  }

  //Array lengths are the same, now check that contents are the same

  $.each(array1, function (index, value) {
    //For each item in array1, find out how many times it appears, must be the same in array2, else return false
    var countArray1 = 0;
    var positionArray1 = null;
    for (var i = 0; i < array1.length; i++) {
      if (array1[i] === value) {
        countArray1++;
        positionArray1 = i;
      }
    };

    var countArray2 = 0;
    var positionArray2 = null;
    for (var x = 0; x < array2.length; x++) {
      if (array2[x] === value) {
        countArray2++;
        positionArray2 = x;
      }
    };

    //count must be the same for item in both arrays, else return false
    if (countArray1 != countArray2) {
      isEqual = false;
      //break out of each loop
      return false;
    }

    //position must be the same for item in both arrays, else return false
    if (order && positionArray1 != positionArray2) {
      isEqual = false;
      //break out of each loop
      return false;
    }

  });
  //all items must match, return true
  return isEqual;
}

/**
 * Implement a cookieLink
 */
app.library.utility.cookieLink = function (cookie, goTo, relativeURL, nav_link_SelectorToHighlight, nav_menu_SelectorToHighlight, params) {
  nav_link_SelectorToHighlight = nav_link_SelectorToHighlight || null;
  nav_menu_SelectorToHighlight = nav_menu_SelectorToHighlight || null;
  params = params || null;

  // Check a Cookie is set
  if (Cookies.get(cookie)) {

    // Map the goTo params
    var goToParams = {};
    goToParams[goTo] = Cookies.get(cookie);
    goToParams[C_APP_GOTO_PARAMS] = params;

    // Remove the Cookie
    Cookies.remove(cookie, app.config.plugin.jscookie.persistent);

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
  var itemsLowerCase = [];
  $.each(items, function (index, value) {
    itemsLowerCase.push(value.trim().toLowerCase())
  });
  return (new Set(itemsLowerCase)).size !== itemsLowerCase.length;
};

/**
 * Download a dynamic resource
 */
app.library.utility.download = function (fileName, fileData, fileExtension, mimeType, isBase64) {
  mimeType = mimeType || null;
  isBase64 = isBase64 || false;

  if (isBase64) {
    // split by the ;base64, definition
    var dataStruct = fileData.split(';base64,');
    // Convert data Array/Byte
    fileData = atob(dataStruct[1]);
    // Convert data String/Array Byte
    fileData = fileData.s2ab();
    // remove the data: definition
    mimeType = dataStruct[0].substring(5);
  }

  var blob = new Blob([fileData], { type: mimeType });
  var downloadUrl = URL.createObjectURL(blob);
  var a = document.createElement("a");
  a.href = downloadUrl;
  a.download = fileName + '.' + fileExtension;
  if (document.createEvent) {
    // https://developer.mozilla.org/en-US/docs/Web/API/Document/createEvent
    var event = document.createEvent('MouseEvents');
    event.initEvent('click', true, true);
    a.dispatchEvent(event);
  }
  else {
    a.click();
  }
};

/**
 * Check and block deprecated IE browser
 */
app.library.utility.isIE = function () {
  if (window.navigator.userAgent.match(/MSIE|Trident/) == null) {
    return false;
  } else {
    $("#modal-information").attr("data-backdrop", "static");
    $("#modal-information").find("button").remove();
    api.modal.information(app.label.static["ie-information"]);
    return true;
  }
};

/** 
 * Simulate an async sleep. 
 * The parent outer function must be async
 * **/
app.library.utility.sleep = function (ms) {
  ms = ms || 400;
  return new Promise(resolve => setTimeout(resolve, ms));
}

/** 
 * asyncController to check for the existance of a controller before proceeding to callback 
 * input control must be the string version of a variable
 * after parse with eval to check it
 * **/
app.library.utility.asyncController = function (control, callbackFunction, callbackParams, id) {
  callbackParams = callbackParams || null;
  id = id || app.library.utility.randomGenerator();
  app.library.utility.asyncControlCounter[id] = app.library.utility.asyncControlCounter[id] || Date.now();

  if (app.library.utility.asyncControlCounter[id] < (Date.now() + 180000)) {
    if (eval(control)) {
      delete app.library.utility.asyncControlCounter[id];
      callbackFunction(callbackParams);
      return;
    }
    setTimeout(function () {
      app.library.utility.asyncController(control, callbackFunction, callbackParams, id)
    }, 100);
  }

};

app.library.utility.trimPapaParse = function (data) {
  $(data).each(function (keyObject, dataObject) {
    $.each(dataObject, function (key, property) {
      dataObject[key] = property.trim();
    });
  });
  return data;
};

//#endregion