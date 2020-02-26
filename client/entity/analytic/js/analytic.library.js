/*******************************************************************************
Custom JS application specific group.library.js
*******************************************************************************/
//#region Namespaces definitions
// Add Namespace Group Data Table
app.analytic = {};
app.analytic.ajax = {};
app.analytic.callback = {};
app.analytic.render = {};
app.analytic.validation = {};

app.analytic.dateFrom = moment().subtract(app.config.entity.analytic.dateRangePicker, 'days'); // Date type, not String
app.analytic.dateTo = moment(); // Date type, not String
//#endregion

//#region set up form
/**
 * Set up date range picker
 */
app.analytic.setDatePicker = function () {
    $("#analytic-date-range span").html(app.analytic.dateFrom.format(app.config.mask.date.display) + ' - ' + app.analytic.dateTo.format(app.config.mask.date.display));

    $("#analytic-date-range").daterangepicker({
        startDate: app.analytic.dateFrom,
        endDate: app.analytic.dateFrom,
        maxDate: new Date(),
        ranges: {
            [app.label.static["today"]]: [moment(), moment()],
            [app.label.static["yesterday"]]: [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
            [app.label.static["last-7-days"]]: [moment().subtract(6, 'days'), moment()],
            [app.label.static["last-30-days"]]: [moment().subtract(29, 'days'), moment()],
            [app.label.static["last-365-days"]]: [moment().subtract(364, 'days'), moment()],
            // [app.label.static["this-month"]]: [moment().startOf('month'), moment().endOf('month')],
            // [app.label.static["last-month"]]: [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
        },
        locale: app.label.plugin.daterangepicker
    }, function (start, end) {
        // Override default values with the selection
        app.analytic.dateFrom = start;
        app.analytic.dateTo = end;

        $("#analytic-date-range span").html(start.format(app.config.mask.date.display) + ' - ' + end.format(app.config.mask.date.display));
        $("#analytic-results").hide();
    });
}

/**
 * Get list of subjects
 */
app.analytic.ajax.readSubject = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Navigation.Subject_API.Read",
        { SbjCode: null },
        "app.analytic.callback.readSubject");
};

/**
 * Populate subjects dropdown
 * @param  {} data
 */
app.analytic.callback.readSubject = function (data) {
    $("#select-card").find("[name=select-subject]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.analytic.callback.mapSubjectData(data)
    });

    // Enable and Focus Seach input
    $("#select-card").find("[name=select-subject]").prop('disabled', false)

    $("#select-card").find("[name=select-subject]").on('select2:select', function (e) {
        $("#analytic-results").hide();
        app.analytic.ajax.readProduct();
    });

    $("#select-card").find("[name=select-subject]").on('select2:unselect', function (e) {
        $("#select-card").find("[name=select-product]").empty();
        // Disable product 
        $("#select-card").find("[name=select-product]").prop('disabled', true);
        $("#analytic-results").hide();
    });
};

/**
 * Return formatted option for select 
 * @param  {} dataAPI
 */
app.analytic.callback.mapSubjectData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.SbjCode;
        dataAPI[i].text = item.SbjValue + " (" + item.SbjValue + ")";
    });
    return dataAPI;
}

/**
 * Get list of products for the subject
 */
app.analytic.ajax.readProduct = function () {
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.System.Navigation.Product_API.Read",
        { SbjCode: SbjCode },
        "app.analytic.callback.readProduct");
};

/**
 * Populate products dropdown
 * @param  {} data
 */
app.analytic.callback.readProduct = function (data) {
    $("#select-card").find("[name=select-product]").empty().append($("<option>")).select2({
        minimumInputLength: 0,
        allowClear: true,
        width: '100%',
        placeholder: app.label.static["start-typing"],
        data: app.analytic.callback.mapProductData(data)
    });

    // Enable 
    $("#select-card").find("[name=select-product]").prop('disabled', false);

    $("#select-card").find("[name=select-product]").on('select2:select', function (e) {
        $("#analytic-results").hide();
        app.analytic.PrcCode = e.params.data.PrcCode;
    });

    $("#select-card").find("[name=select-product]").on('select2:unselect', function (e) {
        $("#analytic-results").hide();
        app.analytic.PrcCode = null;
    });
};

/**
 * Return formatted option for product dropdown
 * @param  {} dataAPI
 */
app.analytic.callback.mapProductData = function (dataAPI) {
    $.each(dataAPI, function (i, item) {
        dataAPI[i].id = item.PrcCode;
        dataAPI[i].text = item.PrcCode + " (" + item.PrcValue + ")";
    });
    return dataAPI;
}

//#endregion

//#region get table data

/**
 * Read analytics
 */
app.analytic.ajax.readAnalytics = function () {
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.spinner.start();
    api.ajax.jsonrpc.request(
        app.config.url.api.private,
        "PxStat.Security.Analytic_API.Read",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readAnalytics",
        null,
        null,
        null,
        { async: false }
    );
    app.analytic.ajax.readBrowser(null, "#analytic-chart [name=browser-pie-chart]");
    app.analytic.ajax.readOs(null, "#analytic-chart [name=operating-system-pie-chart]");
    app.analytic.ajax.readReferrer(null, "#analytic-chart [name=referrer-column-chart]");
    app.analytic.ajax.readTimeline(null, "#analytic-chart [name=dates-line-chart]");
    app.analytic.ajax.readLanguage(null, "#analytic-chart [name=language-pie-chart]");
    app.analytic.ajax.readFormat(null, "#analytic-chart [name=format-pie-chart]");
    $("#analytic-results").fadeIn();

};

/**
 * Draw Callback for Datatable
 */
app.analytic.drawCallback = function () {
    $('[data-toggle="tooltip"]').tooltip();
    $("#analytic-data").find("[name=" + C_APP_NAME_LINK_EDIT + "]").once("click", function (e) {
        e.preventDefault();
        app.analytic.ajax.readTimeline($(this).attr("idn"), "#analytic-chart-modal [name=dates-line-chart]");
        app.analytic.ajax.readReferrer($(this).attr("idn"), "#analytic-chart-modal [name=referrer-column-chart]");
        app.analytic.ajax.readBrowser($(this).attr("idn"), "#analytic-chart-modal [name=browser-pie-chart]");
        app.analytic.ajax.readOs($(this).attr("idn"), "#analytic-chart-modal [name=operating-system-pie-chart]");
        app.analytic.ajax.readLanguage($(this).attr("idn"), "#analytic-chart-modal [name=language-pie-chart]");
        app.analytic.ajax.readFormat(null, "#analytic-chart-modal [name=format-pie-chart]");
        $("#matrix-chart-modal").find("[name=mtr-title]").text($(this).attr("idn") + " : " + $(this).attr("data-original-title"));
        $("#matrix-chart-modal").find("[name=date-range]").html(app.analytic.dateFrom.format(app.config.mask.date.display)
            + "    " + " - " + app.analytic.dateTo.format(app.config.mask.date.display));
        $("#matrix-chart-modal").modal("show");
    });
}

/**
 * Draw analytics datatable
 * @param  {} data
 */
app.analytic.callback.readAnalytics = function (data) {
    if ($.fn.dataTable.isDataTable("#analytic-data table")) {
        app.library.datatable.reDraw("#analytic-data table", data);
    } else {

        var localOptions = {
            data: data,
            columns: [
                {
                    data: null,
                    render: function (data, type, row) {
                        return app.library.html.link.edit({ idn: row.MtrCode }, row.MtrCode, row.MtrTitle);
                    }
                },
                { data: "SbjValue" },

                {
                    data: null,
                    render: function (data, type, row) {
                        return row.PrcCode + "(" + row.PrcValue + ")";

                    }
                },
                {
                    data: null,
                    render: function (data, type, row) {
                        return row.PublishDate ? moment(row.PublishDate).format(app.config.mask.datetime.display) : "";
                    }
                },
                { data: "NltBot" },
                { data: "NltM2m" },
                { data: "NltUser" },
                { data: "Total" }
            ],
            drawCallback: function (settings) {
                api.spinner.stop();
                app.analytic.drawCallback();
            },
            //Translate labels language
            language: app.label.plugin.datatable
        };
        $("#analytic-data table").DataTable($.extend(true, {}, app.config.plugin.datatable, localOptions)).on('responsive-display', function (e, datatable, row, showHide, update) {
            app.analytic.drawCallback();
        });
    }
    var totalBot = 0;
    var totalM2M = 0;
    var totalUsers = 0;
    $("#summary-card").find("[name=analytic-sum-bots]").text(function () {
        $.each(data, function (index, value) {
            totalBot = totalBot + value.NltBot;
        });
        return app.library.utility.formatNumber(totalBot)
    });
    $("#summary-card").find("[name=analytic-sum-m2m]").text(function () {
        $.each(data, function (index, value) {
            totalM2M = totalM2M + value.NltM2m;
        });
        return app.library.utility.formatNumber(totalM2M)
    });
    $("#summary-card").find("[name=analytic-sum-users]").text(function () {
        $.each(data, function (index, value) {
            totalUsers = totalUsers + value.NltUser;
        });
        return app.library.utility.formatNumber(totalUsers)
    });
    $("#summary-card").find("[name=analytic-sum-totals]").text(app.library.utility.formatNumber(totalBot + totalM2M + totalUsers));
};
//#endregion

//#region browser

/**
 * Get browser analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readBrowser = function (MtrCode, selector) {
    MtrCode = MtrCode || null;
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Analytic_API.ReadBrowser",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readBrowser",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw browser pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readBrowser = function (data, selector) {
    app.analytic.render.readBrowser(data, selector);
};

//to be overridden 
app.analytic.render.readBrowser = function (data, selector) { };

//#endregion

//#region OS

/**
 * Get OS analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readOs = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Analytic_API.ReadOs",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readOs",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw Os pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readOs = function (data, selector) {
    app.analytic.render.readOs(data, selector);
};

//to be overridden 
app.analytic.render.readOs = function (data, selector) { };
//#endregion

//#region Referrer

/**
 * Get referrer analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readReferrer = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Analytic_API.ReadReferrer",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readReferrer",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw referrer chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readReferrer = function (data, selector) {
    app.analytic.render.readReferrer(data, selector);
};

//to be overridden 
/**
* 
* @param {*} data
* @param {*} selector
*/
app.analytic.render.readReferrer = function (data, selector) { };
//#endregion

//#region language

/**
 * Get language analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readLanguage = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Analytic_API.ReadLanguage",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readLanguage",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw language pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readLanguage = function (data, selector) {
    app.analytic.render.readLanguage(data, selector);
};

//to be overridden 
app.analytic.render.readLanguage = function (data, selector) { };
//#endregion

//#region timeline

/**
 * Get timeline analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readTimeline = function (MtrCode, selector) {
    MtrCode = MtrCode || null;

    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Analytic_API.ReadTimeline",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readTimeline",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw timeline chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readTimeline = function (data, selector) {
    app.analytic.render.readTimeline(data, selector);
};

//to be overridden 
/**
* 
* @param {*} data
* @param {*} selector
*/
app.analytic.render.readTimeline = function (data, selector) { };
//#endregion

//#region validation

/**
 * Validation
 */
app.analytic.validation.select = function () {

    $("#select-card").find("form").trigger("reset").validate({
        rules: {
            "nlt-masked-ip":
            {
                validIpMask: true
            },
        },
        errorPlacement: function (error, element) {
            $("#select-card").find("[name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.analytic.ajax.readAnalytics();
        }
    }).resetForm();
};

//#endregion

//#region format
/**
 * Get format analytics
 * @param  {} MtrCode
 * @param  {} selector
 */
app.analytic.ajax.readFormat = function (MtrCode, selector) {

    MtrCode = MtrCode || null;
    var SbjCode = $("#select-card").find("[name=select-subject]").val();
    if (SbjCode != null && SbjCode.length == 0) {
        SbjCode = null
    }

    var PrcCode = $("#select-card").find("[name=select-product]").val();
    if (PrcCode != null && PrcCode.length == 0) {
        PrcCode = null
    }

    api.ajax.jsonrpc.request(app.config.url.api.private,
        "PxStat.Security.Analytic_API.ReadFormat",
        {
            "DateFrom": app.analytic.dateFrom.format(app.config.mask.date.ajax),
            "DateTo": app.analytic.dateTo.format(app.config.mask.date.ajax),
            "SbjCode": SbjCode,
            "PrcCode": PrcCode,
            "MtrCode": MtrCode,
            "NltInternalNetworkMask": $("#select-card").find("[name=nlt-masked-ip]").val()
        },
        "app.analytic.callback.readFromat",
        selector,
        null,
        null,
        { async: false }
    );
}

/**
 * Draw fromat pie chart
 * @param  {} data
 * @param  {} selector
 */
app.analytic.callback.readFromat = function (data, selector) {
    app.analytic.render.readFormat(data, selector);
};

//to be overridden 
app.analytic.render.readFormat = function (data, selector) { };

//#endregion