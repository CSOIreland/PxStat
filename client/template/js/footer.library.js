
/*******************************************************************************
Custom JS application specific
****************************************************************************/

//#region Namespaces 
app.footer = {};
app.footer.render = {};

//Set Contact info
app.footer.render.contact = function () {
  $("#footer [name=phone]").html(app.config.template.footer.contact.phone).attr("href", "tel:" + app.config.template.footer.contact.phone);
  $("#footer [name=email]").html(app.config.template.footer.contact.email).attr("href", "mailto:" + app.config.template.footer.contact.email);
  $("#footer address").text(app.config.template.footer.contact.address);
};

//Set Links
app.footer.render.links = function () {
  $.each(app.config.template.footer.links, function (index, value) {
    if (value.text) {
      $("#footer [name=footer-links]").append($("<li>", {
        "html": $("<a>", {
          "href": value.url,
          "text": value.text,
          "target": "_blank",
          "class": "text-light",
          "rel": "noreferrer" // Best practice for cross-origin links
        }).get(0).outerHTML
      }));
    }
  });

  $("#footer [name=footer-links]").append($("<li>", {
    "html": $("<a>", {
      "href": "/?privacy",
      "text": app.label.static["privacy"],
      "name": "privacy",
      "class": "text-light"
    }).get(0).outerHTML
  }));

  $("#footer [name=footer-links]").append($("<li>", {
    "html": $("<a>", {
      "href": "#",
      "text": app.label.static["manage-cookies"],
      "name": "cookie-manage",
      "class": "text-light",
    }).get(0).outerHTML
  }));

  $("#footer [name=footer-links]").append($("<li>", {
    "html": $("<a>", {
      "href": C_APP_URL_GITHUB_REPORT_ISSUE,
      "text": app.label.static["report-issue"],
      "target": "_blank",
      "class": "text-light",
      "rel": "noreferrer" // Best practice for cross-origin links
    }).get(0).outerHTML
  }));

  // Set Version in Footer
  $("#footer").find("[name=version]").html(
    $("<a>", {
      "href": C_APP_URL_GITHUB,
      "text": 'PxStat ' + C_APP_VERSION,
      "class": "text-light",
      "target": "_blank",
      "rel": "noreferrer" // Best practice for cross-origin links
    }).get(0).outerHTML
  );
};

//Set Social Media
app.footer.render.social = function () {
  $.each(app.config.template.footer.social, function (index, value) {
    $("#footer [name=footer-social]").append($("<a>", {
      "href": value.url,
      "target": "_blank",
      "aria-label": app.label.static[value.label],
      "rel": "noreferrer" // Best practice for cross-origin links
    }).append($("<i>", {
      "class": value.icon + " fa-3x me-2 text-white",
      "title": app.label.static[value.label],
      "data-bs-toggle": "tooltip",
      "data-bs-placement": "top"
    })).get(0).outerHTML);
  });

};

app.footer.render.watermark = function () {
  $("#footer").find("[name=logo-link]").attr("href", app.config.template.footer.watermark.url);
  $("#footer").find("[name=logo]").attr("alt", app.config.template.footer.watermark.alt);
  $("#footer").find("[name=logo]").attr("src", app.config.template.footer.watermark.src);
}
//#endregion