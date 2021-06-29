$(document).ready(function () {
    app.geomap.panel.ajax.readLayers();

    $("#map-panel").find("[name=add-layer]").once("click", app.geomap.panel.modal.addLayer);

})