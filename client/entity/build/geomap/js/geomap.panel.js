$(document).ready(function () {
    app.geomap.panel.ajax.readLayers();

    //remove add button if not administrator
    if (app.navigation.user.prvCode != C_APP_PRIVILEGE_ADMINISTRATOR) {
        $("#map-panel").find("[name=add-layer-footer]").remove();
    }
    else {
        $("#map-panel").find("[name=add-layer-footer]").show()
    }

    $("#map-panel").find("[name=add-layer]").once("click", app.geomap.panel.modal.addLayer);

})