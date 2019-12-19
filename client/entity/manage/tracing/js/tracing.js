/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {
  // Entity with restricted access
  app.navigation.access.check();
  app.navigation.layout.set(false);
  app.navigation.breadcrumb.set([app.label.static["manage"], app.label.static["tracing"]]);

  app.tracing.ajax.readType();
  app.tracing.setDatePicker();
  app.tracing.validation.submit();

  //cancel click
  $("#tracing-input").find("[name=button-cancel]").once("click", function (e) {
    e.preventDefault();
    //reset date picker
    var datePicker = $("#tracing-input").find("[name=input-date-range]").data('daterangepicker');
    var start = moment().startOf('day');
    var end = moment();
    datePicker.setStartDate(start);
    datePicker.setEndDate(end);

    //reset Authentication select
    $("#tracing-input").find("[name=select-authentication-type]").val("");

    //reset optional inputs
    $("#tracing-input").find("[name=trc-username]").val("");
    $("#tracing-input").find("[name=trc-ip]").val("");
    $("#tracing-input").find(".error").empty();

    $("#tracing-result").hide();
  });

  // Bootstrap tooltip
  $('[data-toggle="tooltip"]').tooltip();
  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
  $("#tracing-input").find("[name='trc-ip']").attr('placeholder', app.label.static["sample-ip-address"]);

});


