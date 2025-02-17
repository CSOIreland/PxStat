/*******************************************************************************
Application - Library 
*******************************************************************************/
var app = app || {};
app.library = app.library || {};

app.library.subscriber = {};
app.library.subscriber.validation = {};
app.library.subscriber.ajax = {};
app.library.subscriber.callback = {};
app.library.subscriber.details = null;
app.library.subscriber.userchannels = [];

app.library.subscriber.ajax.readCurrent = function () {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscriber_API.ReadCurrent",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken,
      },
      "app.library.subscriber.callback.readCurrent"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.readCurrent = function (data) {
  if (data) {
    app.library.subscriber.details = data;
    app.library.subscriber.ajax.readUserChannels();
  }
  else {
    api.modal.exception(app.label.static["api-ajax-exception"]);
  }
};

app.library.subscriber.ajax.readUserChannels = function () {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscription_API.ChannelSubscriptionReadCurrent",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken,
      },
      "app.library.subscriber.callback.readUserChannels"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.readUserChannels = function (data) {
  app.library.subscriber.userchannels = data || [];
  app.library.subscriber.callback.drawUser();
};

app.library.subscriber.callback.drawUser = function () {
  switch (app.auth.firebase.user.type) {
    case C_APP_FIREBASE_ID_PASSWORD:
      $("#modal-read-subscriber").find("[name=update-password-row]").show();
      $("#modal-read-subscriber").find("[name=subscriber-icon]").removeClass().addClass("fas fa-user fa-lg mr-1");
      break;
    case C_APP_FIREBASE_ID_GOOGLE:
      $("#modal-read-subscriber").find("[name=update-password-row]").hide();
      $("#modal-read-subscriber").find("[name=subscriber-icon]").removeClass().addClass("fab fa-google fa-lg mr-1");
      break;
    case C_APP_FIREBASE_ID_FACEBOOK:
      $("#modal-read-subscriber").find("[name=update-password-row]").hide();
      $("#modal-read-subscriber").find("[name=subscriber-icon]").removeClass().addClass("fab fa-facebook fa-lg mr-1");
      break;
    case C_APP_FIREBASE_ID_TWITTER:
      $("#modal-read-subscriber").find("[name=update-password-row]").hide();
      $("#modal-read-subscriber").find("[name=subscriber-icon]").removeClass().addClass("fab fa-twitter fa-lg mr-1");
      break;
    case C_APP_FIREBASE_ID_GITHUB:
      $("#modal-read-subscriber").find("[name=update-password-row]").hide();
      $("#modal-read-subscriber").find("[name=subscriber-icon]").removeClass().addClass("fab fa-github fa-lg mr-1");
      break;
    default:
      $("#modal-read-subscriber").find("[name=update-password-row]").hide();
      break;
  };

  $("#modal-read-subscriber").find("[name=display-name]").html(app.library.subscriber.details.DisplayName || app.library.subscriber.details.CcnEmail);
  $("#modal-read-subscriber").find("[name=ccn-email]").html(app.library.subscriber.details.CcnEmail);

  $("#modal-read-subscriber").find("[name=subscription-toggle-wrapper]").empty();

  $.each(app.library.subscriber.userchannels, function (index, value) {
    //see if user is subscribed to this channel
    var inputAttributes = {
      "type": "checkbox",
      "name": value.ChnCode,
      "checked": value.ChnSubscribed
    };


    $("#modal-read-subscriber").find("[name=subscription-toggle-wrapper]").append(
      $("<div>", {
        "class": "checkbox pl-4",
        "html": $("<label>", {
          "html": $("<input>", inputAttributes).get(0).outerHTML + " " + value.ChnName
        }).get(0).outerHTML
      }));

    $("#modal-read-subscriber").find("[name='" + value.ChnCode + "']").bootstrapToggle("destroy").bootstrapToggle({
      onlabel: app.label.static["on"],
      offlabel: app.label.static["off"],
      onstyle: "success",
      offstyle: "warning text-dark",
      height: 38,
      style: "m-1 text-light",
      width: C_APP_TOGGLE_LENGTH //Depend on language translation.
    }).once("change", function () {
      if ($(this).is(':checked')) {
        app.library.subscriber.ajax.channelSubscriptionCreate($(this).attr("name"));
      }
      else {
        app.library.subscriber.ajax.channelSubscriptionDelete($(this).attr("name"));
      }
    });
  });

  $("#modal-read-subscriber-developer-key").text(app.library.subscriber.details.SbrKey);
  new ClipboardJS("#modal-read-subscriber-developer [name=copy-key]");

  $("#modal-read-subscriber").modal("show");

  $("#modal-read-subscriber").find("[name=update-password]").once("click", function () {
    app.library.subscriber.validation.updatePassword();
    $("#modal-subscriber-update-password").modal("show");
  });

  $("#modal-read-subscriber-developer").find("[name=generate-key]").once("click", function () {
    switch (app.auth.firebase.user.type) {
      case C_APP_FIREBASE_ID_PASSWORD:
        $("#modal-subscriber-confirm-password-api-key").modal("show");
        app.library.subscriber.validation.generateKeyPassword();
        break;
      case C_APP_FIREBASE_ID_GOOGLE:
        app.auth.reauthenticateWithPopup(app.auth.googleAuth, app.library.subscriber.ajax.updateKey);
        break;
      case C_APP_FIREBASE_ID_FACEBOOK:
        app.auth.reauthenticateWithPopup(app.auth.FacebookAuth, app.library.subscriber.ajax.updateKey);
        break;
      case C_APP_FIREBASE_ID_TWITTER:
        app.auth.reauthenticateWithPopup(app.auth.TwitterAuth, app.library.subscriber.ajax.updateKey);
        break;
      case C_APP_FIREBASE_ID_GITHUB:
        app.auth.reauthenticateWithPopup(app.auth.GitHubAuth, app.library.subscriber.ajax.updateKey);
        break;
      default:
        $("#modal-read-subscriber").find("[name=update-password-row]").hide();
        break;
    };
  });

  $("#modal-read-subscriber-manage").find("[name=delete-account]").once("click", function () {

    switch (app.auth.firebase.user.type) {
      case C_APP_FIREBASE_ID_PASSWORD:
        $("#modal-subscriber-confirm-password-delete-account").modal("show");
        app.library.subscriber.validation.deletePasswordUser();
        break;
      case C_APP_FIREBASE_ID_GOOGLE:
        api.modal.confirm(app.label.static["delete-your-account-warning"], app.library.subscriber.deletePopupAccount, app.auth.googleAuth);
        break;
      case C_APP_FIREBASE_ID_FACEBOOK:
        api.modal.confirm(app.label.static["delete-your-account-warning"], app.library.subscriber.deletePopupAccount, app.auth.FacebookAuth);
        break;
      case C_APP_FIREBASE_ID_TWITTER:
        api.modal.confirm(app.label.static["delete-your-account-warning"], app.library.subscriber.deletePopupAccount, app.auth.TwitterAuth);
        break;
      case C_APP_FIREBASE_ID_GITHUB:
        api.modal.confirm(app.label.static["delete-your-account-warning"], app.library.subscriber.deletePopupAccount, app.auth.GitHubAuth);
        break;
      default:
        $("#modal-read-subscriber").find("[name=update-password-row]").hide();
        break;
    };

  });
};

app.library.subscriber.deletePopupAccount = function (authProvider) {
  app.auth.reauthenticateWithPopup(authProvider, app.library.subscriber.ajax.deleteCurrentUser);
};


app.library.subscriber.validation.updatePassword = function () {
  $("#modal-subscriber-update-password form").trigger("reset").validate({
    rules: {
      "current-password":
      {
        required: true,
        validPassword: true
      },
      "new-password": {
        required: true
      },
      "repeat-new-password": {
        required: true,
        equalTo: "#modal-subscriber-update-password [name=new-password]"
      }
    },
    messages: {
      "repeat-new-password": {
        equalTo: app.label.static["invalid-password-match"]
      }
    },
    errorPlacement: function (error, element) {
      $("#modal-subscriber-update-password [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.auth.updatePassword(
        $("#modal-subscriber-update-password form").find("[name=current-password]").val(),
        $("#modal-subscriber-update-password form").find("[name=new-password]").val()
      );
      api.spinner.start();
    }
  }).resetForm();
  //trick to clear any previous password strength values
  $("#modal-subscriber-update-password form").find("[name=new-password]").trigger("change");
};

app.library.subscriber.validation.generateKeyPassword = function () {
  $("#modal-subscriber-confirm-password-api-key form").trigger("reset").validate({
    rules: {
      "password":
      {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#modal-subscriber-confirm-password-api-key [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.auth.reauthenticateWithPassword(
        $("#modal-subscriber-confirm-password-api-key form").find("[name=password]").val(), app.library.subscriber.ajax.updateKey
      );
      api.spinner.start();
    }
  }).resetForm();
};

app.library.subscriber.ajax.updateKey = function () {
  //must be firebase user, get fresh token
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscriber_API.UpdateKey",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken
      },
      "app.library.subscriber.callback.updateKey"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.updateKey = function (data) {
  if (data == C_API_AJAX_SUCCESS) {
    app.library.subscriber.ajax.getNewKey();
  }
};

app.library.subscriber.ajax.getNewKey = function () {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscriber_API.ReadCurrent",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken,
      },
      "app.library.subscriber.callback.getNewKey"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.getNewKey = function (data) {
  if (data) {
    app.library.subscriber.details = data;
    $("#modal-read-subscriber-developer-key").text(app.library.subscriber.details.SbrKey);
    $("#modal-subscriber-confirm-password-api-key").modal("hide");
    api.modal.success(app.label.static["subscriber-developer-key-generated"]);
    api.spinner.stop();
  }
};

app.library.subscriber.ajax.channelSubscriptionCreate = function (chnCode) {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscription_API.ChannelSubscriptionCreate",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken,
        "ChnCode": chnCode
      },
      "app.library.subscriber.callback.channelSubscriptionCreate"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.channelSubscriptionCreate = function (data) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
  }
};

app.library.subscriber.ajax.channelSubscriptionDelete = function (chnCode) {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscription_API.ChannelSubscriptionDelete",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken,
        "ChnCode": chnCode
      },
      "app.library.subscriber.callback.channelSubscriptionDelete"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.channelSubscriptionDelete = function (data) {
  if (data == C_API_AJAX_SUCCESS) {
    api.modal.success(app.library.html.parseDynamicLabel("success-record-updated", [""]));
  }
};

app.library.subscriber.validation.deletePasswordUser = function () {
  $("#modal-subscriber-confirm-password-delete-account form").trigger("reset").validate({
    rules: {
      "password":
      {
        required: true
      }
    },
    errorPlacement: function (error, element) {
      $("#modal-subscriber-confirm-password-delete-account [name=" + element[0].name + "-error-holder]").append(error[0]);
    },
    submitHandler: function (form) {
      $(form).sanitiseForm();
      app.auth.reauthenticateWithPassword(
        $("#modal-subscriber-confirm-password-delete-account form").find("[name=password]").val(), app.library.subscriber.ajax.deleteCurrentUser
      );
      api.spinner.start();
    }
  }).resetForm();
};

app.library.subscriber.ajax.deleteCurrentUser = function () {
  app.auth.firebase.user.details.getIdToken(true).then(function (accessToken) {
    api.ajax.jsonrpc.request(
      app.config.url.api.jsonrpc.public,
      "PxStat.Subscription.Subscriber_API.Delete",
      {
        "Uid": app.auth.firebase.user.details.uid,
        "AccessToken": accessToken
      },
      "app.library.subscriber.callback.deleteCurrentUser"
    );
  }).catch((error) => {
    // An error happened.
    api.modal.error(app.label.static["firebase-authentication-error"]);
    console.log("firebase authentication error : " + error.message);
  });
};

app.library.subscriber.callback.deleteCurrentUser = function (data) {
  if (data == C_API_AJAX_SUCCESS) {
    //redirect to home page

    window.location.href = window.location.pathname;
  }
};
