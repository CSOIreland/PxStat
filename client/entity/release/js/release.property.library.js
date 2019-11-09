/*******************************************************************************
Custom JS application specific
*******************************************************************************/

//#region Add Namespace
app.release = app.release || {};

app.release.property = {};
//#endregion

//#region Property
/**
* 
 */
app.release.property.render = function () {
    $("#release-property").hide().fadeIn();
    $("#release-property [name=rls-reservation-flag]").empty().html(app.library.html.boolean(app.release.RlsReservationFlag, true, true));
    $("#release-property [name=rls-archive-flag]").empty().html(app.library.html.boolean(app.release.RlsArchiveFlag, true, true));

}
//#endregion