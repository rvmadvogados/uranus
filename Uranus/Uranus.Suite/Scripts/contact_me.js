//$(function() {

//    $("input,textarea").jqBootstrapValidation({
//        preventSubmit: true,
//        submitError: function($form, event, errors) {
//            // additional error messages or events
//        },
//        submitSuccess: function($form, event) {
//            // Prevent spam click and default submit behaviour
//            $("#btnSubmit").attr("disabled", true);
//            event.preventDefault();

//            // get values from FORM
//            var name = $("input#name").val();
//            var email = $("input#email").val();
//            var phone = $("input#phone").val();
//            var message = $("textarea#message").val();
//            var firstName = name; // For Success/Failure Message
//            // Check for white space in name for Success/Fail message
//            if (firstName.indexOf(' ') >= 0) {
//                firstName = name.split(' ').slice(0, -1).join(' ');
//            }

//            $.ajax({
//                url: "../mail/contact_me.ashx",
//                type: "POST",
//                data: {
//                    name: name,
//                    phone: phone,
//                    email: email,
//                    message: message
//                },
//                cache: false,
//                success: function (data) {

//                    var json = $.parseJSON(data);

//                    if (json.response == "success") {
//                        // Enable button & show success message
//                        $("#btnSubmit").attr("disabled", false);
//                        $('#success').html("<div class='alert alert-success'>");
//                        $('#success > .alert-success').html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
//                            .append("</button>");
//                        $('#success > .alert-success')
//                            .append("<strong>Sua mensagem foi enviada com sucesso.</strong>");
//                        $('#success > .alert-success')
//                            .append('</div>');
//                    }
//                    else {
//                        // Fail message
//                        $('#success').html("<div class='alert alert-danger'>");
//                        $('#success > .alert-danger').html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
//                            .append("</button>");
//                        $('#success > .alert-danger').append("<strong>Desculpa " + firstName + ", parece que o meu servidor de correio n&atilde;o est&aacute; respondendo. Por favor tente de novo mais tarde!");
//                        $('#success > .alert-danger').append('</div>');
//                    }
//                    //clear all fields
//                    $('#contactForm').trigger("reset");
//                },
//                error: function() {
//                    // Fail message
//                    $('#success').html("<div class='alert alert-danger'>");
//                    $('#success > .alert-danger').html("<button type='button' class='close' data-dismiss='alert' aria-hidden='true'>&times;")
//                        .append("</button>");
//                    $('#success > .alert-danger').append("<strong>Desculpa " + firstName + ", parece que o meu servidor de correio n&atilde;o est&aacute; respondendo. Por favor tente de novo mais tarde!");
//                    $('#success > .alert-danger').append('</div>');
//                    //clear all fields
//                    $('#contactForm').trigger("reset");
//                },
//                rules: {
//                    phone: {
//                        required: true,
//                        minlength: 14
//                    },
//                },
//                messages: {
//                    phone: {
//                        minlength: "Por favor, insira o DDD e o telefone."
//                    },
//                },
//            })
//        },
//        filter: function () {
//            return $(this).is(":visible");
//        },
//    });

//    $("a[data-toggle=\"tab\"]").click(function(e) {
//        e.preventDefault();
//        $(this).tab("show");
//    });
//});

//// When clicking on Full hide fail/success boxes
//$('#name').focus(function () {
//    $('#success').html('');
//});

(function () {

    "use strict";

    var contactForm = {

        initialized: false,

        initialize: function () {

            if (this.initialized) return;
            this.initialized = true;

            this.build();
            this.events();

        },

        build: function () {

            this.validations();

        },

        events: function () {

        },

        validations: function () {

            var contactform = $("#contact-form"), url = contactform.attr("action");
            contactform.validate({
                submitHandler: function (form) {

                    // Loading State
                    var submitButton = $(this.submitButton);
                    submitButton.button("loading");

                    var dados = $(form).serialize();
                    // Ajax Submit
                    $.ajax({
                        type: "POST",
                        url: url,
                        data: dados,
                        dataType: "json",
                        success: function (data) {
                            if (data.response == "success") {

                                $("#contact-alert-success").removeClass("hidden");
                                $("#contact-alert-error").addClass("hidden");

                                // Reset Form
                                $("#contact-form .form-control")
                                                    .val("")
                                                    .blur()
                                                    .parent()
                                                    .removeClass("has-success")
                                                    .removeClass("has-error")
                                                    .find("label.error")
                                                    .remove();

                                if (($("#contact-alert-success").position().top - 80) < $(window).scrollTop()) {
                                    $("html, body").animate({
                                        scrollTop: $("#contact-alert-success").offset().top - 80
                                    }, 300);
                                }

                            } else {

                                $("#contact-alert-error").removeClass("hidden");
                                $("#contact-alert-success").addClass("hidden");

                                if (($("#contact-alert-error").position().top - 80) < $(window).scrollTop()) {
                                    $("html, body").animate({
                                        scrollTop: $("#contact-alert-error").offset().top - 80
                                    }, 300);
                                }

                            }
                        },
                        complete: function () {
                            submitButton.button("reset");
                        }
                    });
                },
                rules: {
                    name: {
                        required: true
                    },
                    email: {
                        required: true,
                        email: true
                    },
                    phone: {
                        required: true,
                        minlength: 14
                    },
                    message: {
                        required: true
                    }
                },
                messages: {
                    phone: {
                        required: "Por favor, insira seu n&uacute;mero de telefone com DDD.",
                        minlength: "Por favor, insira o DDD e o telefone."
                    },
                },
                highlight: function (element) {
                    $(element)
                                  .parent()
                                  .removeClass("has-success")
                                  .addClass("has-error");
                },
                success: function (element) {
                    $(element)
                                  .parent()
                                  .removeClass("has-error")
                                  .addClass("has-success")
                                  .find("label.error")
                                  .remove();
                }
            });

        }

    };

    contactForm.initialize();

})();
