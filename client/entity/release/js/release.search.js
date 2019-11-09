/*******************************************************************************
Custom JS application specific
*******************************************************************************/
$(document).ready(function () {

  // Initialise the list of Matrices
  app.release.search.readMatrixList();
  // Translate labels language (Last to run)
  app.library.html.parseStaticLabel();
});

