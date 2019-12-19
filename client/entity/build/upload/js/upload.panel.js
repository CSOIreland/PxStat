/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

  //Load copyrights data
  app.build.upload_panel.copyright.ajax.read();
  //Load Languages data
  app.build.upload_panel.language.ajax.read();
  //Load Format data
  app.build.upload_panel.format.ajax.read();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
  //run bootstrap toggle to show/hide toggle button
  bsBreakpoints.toggle(bsBreakpoints.getCurrentBreakpoint());
});
