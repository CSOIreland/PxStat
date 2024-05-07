app.auth = {};
app.auth.ajax = {};
app.auth.callback = {};
app.auth.validation = {};
app.auth.firebase = {};
app.auth.firebase.user = {};
app.auth.firebase.user.details = null;
app.auth.firebase.user.type = null;

app.auth.validation.signUp = function () {
    $("#modal-subscriber-sign-up form").trigger("reset").onSanitiseForm().validate({
        rules: {
            "email":
            {
                required: true,
                validEmailAddress: true
            },
            "name":
            {
                required: true
            },
            "password":
            {
                required: true,
                validPassword: true
            },
            "repeat-password": {
                required: true,
                equalTo: "#modal-subscriber-sign-up [name=password]"
            },
            "privacy-agreement":
            {
                required: true
            }
        },
        messages: {
            "repeat-password": {
                equalTo: app.label.static["invalid-password-match"]
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-subscriber-sign-up [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            app.auth.firebase.createUser();
        }
    }).resetForm();

    //trick to clear any previous password strength values
    $("#modal-subscriber-sign-up form").find("[name=password]").trigger("change");

};

app.auth.firebase.createUser = function () {
    createUserWithEmailAndPassword(
        app.auth.getFirebaseAuthApp,
        $("#modal-subscriber-sign-up").find("[name=email]").val(),
        $("#modal-subscriber-sign-up").find("[name=password]").val()
    )
        .then((userCredential) => {
            //add display name to profile
            var displayName = $("#modal-subscriber-sign-up form").find("[name=name]").val();
            updateProfile(userCredential.user, {
                displayName: displayName
            }).then(function () {
                //send verification email
                app.auth.sendEmailVerification(userCredential.user);
                //user profile updated               
            }).catch(function (error) {

            })
            $("#modal-subscriber-sign-up").modal("hide");
        })
        .catch((error) => {
            api.modal.error(app.label.static["firebase-authentication-error"]);
            console.log("firebase authentication error : " + error.message);
        });
};

app.auth.ajax.subscriberCreate = function (user) {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Subscription.Subscriber_API.Create",
        {
            "Preference": {
                "LngIsoCode": app.label.language.iso.code
            },
            "Uid": user.uid,
            "AccessToken": user.accessToken
        },
        "app.auth.callback.subscriberCreate",
        user
    );
};

app.auth.callback.subscriberCreate = function (data, user) {
    if (data == C_API_AJAX_SUCCESS) {
        app.navigation.access.ajax.readCurrentSubscriber(user);
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.auth.validation.signIn = function () {
    $("#modal-subscriber-login-email form").trigger("reset").validate({
        rules: {
            "email":
            {
                required: true,
                validEmailAddress: true
            },
            "password":
            {
                required: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-subscriber-login-email [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            setPersistence(app.auth.getFirebaseAuthApp, $("#modal-subscriber-email-stay-logged-in").is(':checked') ? browserLocalPersistence : browserSessionPersistence);
            signInWithEmailAndPassword(app.auth.getFirebaseAuthApp,
                $("#modal-subscriber-login-email").find("[name=email]").val(),
                $("#modal-subscriber-login-email").find("[name=password]").val())
                .then((userCredential) => {
                    if (!userCredential.user.emailVerified) {
                        //send verification email
                        app.auth.sendEmailVerification(userCredential.user);
                    }
                    else {
                        //user signed in
                        $("#modal-subscriber-login-email").modal("hide");
                    }
                })
                .catch((error) => {
                    api.modal.error(app.label.static["firebase-authentication-error"]);
                    console.log("firebase authentication error : " + error.message);
                });
        }
    }).resetForm();
};

app.auth.ajax.signOut = function () {
    api.ajax.jsonrpc.request(
        app.config.url.api.jsonrpc.public,
        "PxStat.Subscription.Subscriber_API.Logout",
        {
            "Uid": app.auth.firebase.user.details.uid,
            "AccessToken": app.auth.firebase.user.details.accessToken
        },
        "app.auth.callback.signOut"
    );

};

app.auth.callback.signOut = function (data) {
    if (data == C_API_AJAX_SUCCESS) {
        signOut(app.auth.getFirebaseAuthApp).then(() => {
            //clean up language cookie. It will be reset again if the user logs in
            Cookies.remove(C_COOKIE_LANGUAGE);
            //reload application
            window.location.href = window.location.pathname;

        }).catch((error) => {
            api.modal.error(app.label.static["firebase-authentication-error"]);
            console.log("firebase authentication error : " + error.message);
        });
    }
    else {
        api.modal.exception(app.label.static["api-ajax-exception"]);
    }
};

app.auth.validation.revoverPassword = function () {
    $("#modal-subscriber-password-recovery form").trigger("reset").validate({
        rules: {
            "email":
            {
                required: true,
                validEmailAddress: true
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-subscriber-password-recovery [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            var email = $("#modal-subscriber-password-recovery form").find("[name=email]").val();
            sendPasswordResetEmail(app.auth.getFirebaseAuthApp, email)
                .then(() => {
                    $("#modal-subscriber-password-recovery").modal("hide");
                    api.modal.success(app.library.html.parseDynamicLabel("open-access-initiate-password-set", [email]));
                })
                .catch((error) => {
                    api.modal.error(app.label.static["firebase-authentication-error"]);
                    console.log("firebase authentication error : " + error.message);
                });
        }
    }).resetForm();
};

app.auth.validation.resetPassword = function () {
    $("#modal-subscriber-password-reset form").trigger("reset").validate({
        rules: {
            "password":
            {
                required: true,
                validPassword: true
            },
            "repeat-password": {
                required: true,
                equalTo: "#modal-subscriber-password-reset [name=password]"
            }
        },
        errorPlacement: function (error, element) {
            $("#modal-subscriber-password-reset [name=" + element[0].name + "-error-holder]").append(error[0]);
        },
        submitHandler: function (form) {
            $(form).sanitiseForm();
            var newPassword = $("#modal-subscriber-password-reset form").find("[name=password]").val();
            // Save the new password.
            confirmPasswordReset(app.auth.getFirebaseAuthApp, api.uri.getParam('oobCode'), newPassword).then((resp) => {
                $("#modal-subscriber-password-reset").modal("hide");
                api.modal.success(app.label.static["open-access-update-password-set"]);

                $('#modal-success').on('hidden.bs.modal', function (e) {
                    //unbind event for future use
                    $('#modal-success').off('hidden.bs.modal')
                    //sign the user in
                    signInWithEmailAndPassword(app.auth.getFirebaseAuthApp,
                        $("#modal-subscriber-password-reset form").find("[name=user-email]").val(),
                        newPassword)
                        .then((userCredential) => {
                            //clear the address bar of reset password details
                            history.replaceState({}, '', '/');
                        })
                        .catch((error) => {
                            api.modal.error(app.label.static["firebase-authentication-error"]);
                            console.log("firebase authentication error : " + error.message);
                        });
                });
            }).catch((error) => {
                api.modal.error(app.label.static["firebase-authentication-link-invalid"]);
                $('#modal-error').on('hide.bs.modal', function (e) {
                    // Force the reload of the application 

                    window.location.href = window.location.pathname;
                })
                console.log("firebase authentication error : " + error.message);
            });
        }
    }).resetForm();
};

app.auth.sendEmailVerification = function (user) {
    sendEmailVerification(user)
        .then(() => {
            api.modal.information(app.library.html.parseDynamicLabel("email-verification-message", [user.email]));
        })
        .catch((error) => {
            api.modal.error(app.label.static["firebase-authentication-error"]);
            console.log("firebase authentication error : " + error.message);
        });
};

//function to allow the user to update their password through their profile when they are logged in
app.auth.updatePassword = function (currentPassword, newPassword) {
    //first reauthenticate old password
    var credential = EmailAuthProvider.credential(app.auth.firebase.user.details.email, currentPassword);
    reauthenticateWithCredential(app.auth.firebase.user.details, credential).then(function (UserCredential) {
        //old password correct, update to new password
        updatePassword(app.auth.firebase.user.details, newPassword).then(function () {
            //password updated
            $("#modal-subscriber-update-password").modal("hide");
            api.modal.success(app.label.static["open-access-update-password-set"]);
            api.spinner.stop();
        }).catch((error) => {
            // Handle Errors here.
            api.modal.error(app.label.static["firebase-authentication-error"]);
            console.log("firebase authentication error : " + error.message);
        });

    }).catch((error) => {
        // Handle Errors here.
        api.modal.error(app.label.static["firebase-authentication-error"]);
        console.log("firebase authentication error : " + error.message);
    });
};

//function to allow the user to generate a new private api key
app.auth.reauthenticateWithPassword = function (password, callback) {
    //first reauthenticate password
    var credential = EmailAuthProvider.credential(app.auth.firebase.user.details.email, password);
    reauthenticateWithCredential(app.auth.firebase.user.details, credential).then(function (UserCredential) {
        //password correct, run callback function
        callback();
    }).catch((error) => {
        api.spinner.stop();
        // Handle Errors here.
        api.modal.error(app.label.static["firebase-authentication-error"]);
        console.log("firebase authentication error : " + error.message);
    });
};

app.auth.reauthenticateWithPopup = function (authProvider, callback) {
    reauthenticateWithPopup(app.auth.firebase.user.details, authProvider).then(function (UserCredential) {
        //reauthentication successful, run callback function
        callback();
    }).catch((error) => {
        api.spinner.stop();
        // Handle Errors here.
        api.modal.error(app.label.static["firebase-authentication-error"]);
        console.log("firebase authentication error : " + error.message);
    });
};

//
app.auth.subscriberEmailActions = function () {
    //only when we know type or user or none can we do the following
    //reset subscriber password or verify email
    if (api.uri.isParam("mode")) {
        switch (api.uri.getParam("mode")) {
            case "resetPassword":
                if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {
                    //logged in as AD or local account, don't allow to verify email
                    window.location.href = window.location.pathname;
                }
                else {
                    verifyPasswordResetCode(app.auth.getFirebaseAuthApp, api.uri.getParam('oobCode')).then((email) => {
                        app.auth.validation.resetPassword();
                        $("#modal-subscriber-password-reset").modal("show");
                        $("#modal-subscriber-password-reset form").find("[name=email]").html(email);
                        $("#modal-subscriber-password-reset form").find("[name=user-email]").val(email);
                    }).catch((error) => {
                        api.modal.error(app.label.static["firebase-authentication-link-invalid"]);
                        $('#modal-error').on('hide.bs.modal', function (e) {
                            // Force the reload of the application 

                            window.location.href = window.location.pathname;
                        })
                        console.log("firebase authentication error : " + error.message);
                    });
                }
                break;
            case "verifyEmail":
                if (app.navigation.user.isWindowsAccess || app.navigation.user.isLoginAccess) {
                    //logged in as AD or local account, don't allow to verify email
                    window.location.href = window.location.pathname;
                }
                else {
                    applyActionCode(app.auth.getFirebaseAuthApp, api.uri.getParam('oobCode')).then(() => {
                        api.modal.success(app.label.static["email-address-verified"]);
                        $("#modal-success").on('hide.bs.modal', function (e) {
                            window.location.href = window.location.pathname;
                        });
                    }).catch((error) => {
                        api.modal.error(app.label.static["firebase-authentication-link-invalid"]);
                        $('#modal-error').on('hide.bs.modal', function (e) {
                            // Force the reload of the application 

                            window.location.href = window.location.pathname;
                        })
                        console.log("firebase authentication error : " + error.message);
                    });
                }
                break;
            default:
                break;
        }
    };
};

// Import the functions you need from the SDKs you need
import { initializeApp } from "https://www.gstatic.com/firebasejs/9.4.0/firebase-app.js";
import {
    getAuth,
    EmailAuthProvider,
    updatePassword,
    signInWithPopup,
    createUserWithEmailAndPassword,
    applyActionCode,
    updateProfile,
    signInWithEmailAndPassword,
    sendPasswordResetEmail,
    sendEmailVerification,
    verifyPasswordResetCode,
    reauthenticateWithCredential,
    reauthenticateWithPopup,
    confirmPasswordReset,
    onAuthStateChanged,
    signOut,
    GoogleAuthProvider,
    FacebookAuthProvider,
    TwitterAuthProvider,
    GithubAuthProvider,
    setPersistence,
    browserLocalPersistence,
    browserSessionPersistence
} from "https://www.gstatic.com/firebasejs/9.4.0/firebase-auth.js";
// https://firebase.google.com/docs/web/setup#available-libraries

// Initialize Firebase
app.auth.firebaseAuthApp = initializeApp(app.config.plugin.subscriber.firebase.config);
app.auth.getFirebaseAuthApp = getAuth(app.auth.firebaseAuthApp);
app.auth.googleAuth = new GoogleAuthProvider();
app.auth.FacebookAuth = new FacebookAuthProvider();
app.auth.TwitterAuth = new TwitterAuthProvider();
app.auth.GitHubAuth = new GithubAuthProvider();

$(document).ready(function () {
    //remove any individual providers not enabled
    if (!app.config.plugin.subscriber.firebase.providers.emailPassword) {
        $("#modal-subscriber-login").find("[name=sign-up-email-row]").remove();
    };
    if (!app.config.plugin.subscriber.firebase.providers.google) {
        $("#modal-subscriber-login").find("[name=sign-up-google-row]").remove();
    };

    if (!app.config.plugin.subscriber.firebase.providers.facebook) {
        $("#modal-subscriber-login").find("[name=sign-up-facebook-row]").remove();
    };

    if (!app.config.plugin.subscriber.firebase.providers.gitHub) {
        $("#modal-subscriber-login").find("[name=sign-up-github-row]").remove();
    };

    if (!app.config.plugin.subscriber.firebase.providers.twitter) {
        $("#modal-subscriber-login").find("[name=sign-up-twitter-row]").remove();
    };

    //if no 3rd party providers at all, remove stay logged in button
    if (!app.config.plugin.subscriber.firebase.providers.google
        && !app.config.plugin.subscriber.firebase.providers.facebook
        && !app.config.plugin.subscriber.firebase.providers.gitHub
        && !app.config.plugin.subscriber.firebase.providers.twitter) {
        $("#modal-subscriber-login").find("[name=stay-logged-in]").remove();
    }


    onAuthStateChanged(app.auth.getFirebaseAuthApp, (user) => {
        if (user) {
            app.auth.firebase.user.details = app.auth.getFirebaseAuthApp.currentUser;
            app.auth.firebase.user.type = user.providerData[0].providerId;
            // User is signed in, see docs for a list of available properties
            // https://firebase.google.com/docs/reference/js/firebase.User
            //always make sure we have the user in PxStat
            if ((app.auth.firebase.user.type == C_APP_FIREBASE_ID_PASSWORD) && !user.emailVerified) {
                //user logged in with password but not verified
                //log the user out, if they attempt to sign in again with the same email, verification email will be resent
                app.auth.ajax.signOut();
            }
            else if (!api.uri.isParam("mode")) {
                app.navigation.access.ajax.readCurrentSubscriber(user);
            }
            //check for actions like reset password or verify email            
            app.auth.subscriberEmailActions();
        } else {
            //No subscriber logged in. Check for windows/network user
            app.navigation.access.ajax.ReadCurrentWindowsAccess();
        }
    });
    $("#modal-subscriber-login").find("[name=sign-in-email]").once("click", function () {
        app.auth.validation.signIn();
        $("#modal-subscriber-login").modal("hide");
        $("#modal-subscriber-login-email").modal("show");
    });

    $("#modal-subscriber-login").find("[name=sign-up-google]").once("click", function () {
        $("#modal-subscriber-login").modal("hide");
        setPersistence(app.auth.getFirebaseAuthApp, $("#modal-subscriber-provider-stay-logged-in").is(':checked') ? browserLocalPersistence : browserSessionPersistence);
        signInWithPopup(app.auth.getFirebaseAuthApp, app.auth.googleAuth)
            .then((result) => {
                //user signed it, will be caught by onAuthStateChanged

            }).catch((error) => {
                if (error.code == C_APP_FIREBASE_ERROR_ACCOUNT_EXISTS) {
                    api.modal.error(app.label.static["firebase-authentication-error-account-exists"]);
                }
                else {
                    api.modal.error(app.label.static["firebase-authentication-error"]);
                }
                console.log("firebase authentication error : " + error.message);

            });
    });

    $("#modal-subscriber-login").find("[name=sign-up-facebook]").once("click", function () {
        $("#modal-subscriber-login").modal("hide");
        setPersistence(app.auth.getFirebaseAuthApp, $("#modal-subscriber-provider-stay-logged-in").is(':checked') ? browserLocalPersistence : browserSessionPersistence);
        signInWithPopup(app.auth.getFirebaseAuthApp, app.auth.FacebookAuth)
            .then((result) => {
                //user signed it, will be caught by onAuthStateChanged
            }).catch((error) => {
                if (error.code == C_APP_FIREBASE_ERROR_ACCOUNT_EXISTS) {
                    api.modal.error(app.label.static["firebase-authentication-error-account-exists"]);
                }
                else {
                    api.modal.error(app.label.static["firebase-authentication-error"]);
                }
                console.log("firebase authentication error : " + error.message);
            });
    });

    $("#modal-subscriber-login").find("[name=sign-up-twitter]").once("click", function () {
        $("#modal-subscriber-login").modal("hide");
        setPersistence(app.auth.getFirebaseAuthApp, $("#modal-subscriber-provider-stay-logged-in").is(':checked') ? browserLocalPersistence : browserSessionPersistence);
        signInWithPopup(app.auth.getFirebaseAuthApp, app.auth.TwitterAuth)
            .then((result) => {
                //user signed it, will be caught by onAuthStateChanged
            }).catch((error) => {
                if (error.code == C_APP_FIREBASE_ERROR_ACCOUNT_EXISTS) {
                    api.modal.error(app.label.static["firebase-authentication-error-account-exists"]);
                }
                else {
                    api.modal.error(app.label.static["firebase-authentication-error"]);
                }
                console.log("firebase authentication error : " + error.message);

            });
    });

    $("#modal-subscriber-login").find("[name=sign-up-github]").once("click", function () {
        $("#modal-subscriber-login").modal("hide");
        app.auth.GitHubAuth.addScope('user');
        setPersistence(app.auth.getFirebaseAuthApp, $("#modal-subscriber-provider-stay-logged-in").is(':checked') ? browserLocalPersistence : browserSessionPersistence);
        signInWithPopup(app.auth.getFirebaseAuthApp, app.auth.GitHubAuth)
            .then((result) => {
                //user signed it, will be caught by onAuthStateChanged
            }).catch((error) => {
                if (error.code == C_APP_FIREBASE_ERROR_ACCOUNT_EXISTS) {
                    api.modal.error(app.label.static["firebase-authentication-error-account-exists"]);
                }
                else {
                    api.modal.error(app.label.static["firebase-authentication-error"]);
                }
                console.log("firebase authentication error : " + error.message);

            });
    });

    $("#modal-subscriber-login-email").find("[name=forgotten-password]").once("click", function (e) {
        e.preventDefault();
        app.auth.validation.revoverPassword();
        $("#modal-subscriber-login-email").modal("hide");
        $("#modal-subscriber-password-recovery").modal("show");
    });

    $("#navigation").find("[name=nav-subscriber-details]").find("[name=logout]").once("click", function () {
        app.auth.ajax.signOut();
    });

    $("#modal-subscriber-login").on('hidden.bs.modal', function (event) {
        $("#modal-subscriber-provider-stay-logged-in").prop("checked", false)
    });

    $("#modal-subscriber-password-reset").on('hide.bs.modal', function (event) {
        window.location.href = window.location.pathname;
    });


    $("#modal-subscriber-sign-up").find("[name=password-requirements]").popover({
        "html": true,
        "content": $("#modal-subscriber-templates").find("[name=password-requirements]").clone().get(0).outerHTML,
        "template": $("#modal-subscriber-templates").find("[name=popover-template]").clone().get(0).outerHTML,
    });

    $("#modal-subscriber-sign-up").find("[name=privacy]").once("click", function (e) {
        e.preventDefault();
        $("#footer").find("[name=privacy]").trigger("click");
    });

    $("#modal-subscriber-password-reset").find("[name=password-requirements]").popover({
        "html": true,
        "content": $("#modal-subscriber-templates").find("[name=password-requirements]").clone().get(0).outerHTML,
        "template": $("#modal-subscriber-templates").find("[name=popover-template]").clone().get(0).outerHTML,
    });
    $("#modal-subscriber-update-password").find("[name=password-requirements]").popover({
        "html": true,
        "content": $("#modal-subscriber-templates").find("[name=password-requirements]").clone().get(0).outerHTML,
        "template": $("#modal-subscriber-templates").find("[name=popover-template]").clone().get(0).outerHTML,
    });
});
