/*******************************************************************************
Open access
*******************************************************************************/

$(document).ready(function () {
    $("#modal-subscriber-login").find("[name=management-login]").once("click", function (e) {
        e.preventDefault();
        $("#modal-subscriber-login").modal("hide");
        app.openAccess.modal.login();
    });

    $("#modal-subscriber-login-email").find("[name=sign-up]").once("click", function (e) {
        app.auth.validation.signUp();
        $("#modal-subscriber-login-email").modal("hide");
        $("#modal-subscriber-sign-up").modal("show");
    });

    // Translate labels language (Last to run)
    app.library.html.parseStaticLabel();
});