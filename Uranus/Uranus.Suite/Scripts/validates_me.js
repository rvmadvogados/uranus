jQuery.validator.addMethod("strength", function (value) {
    return /^[A-Za-z0-9\d=!\-@._*]*$/.test(value) // consists of only these
        && /[a-z]/.test(value) // has a lowercase letter
        && /\d/.test(value) // has a digit
});

jQuery.validator.addMethod("duplicateKey", function (value, element, param) {

    var check_result = false;

    var key1 = $(param[0]).val(),
        key2 = $(param[1]).val();

    var dataString = {
        "Chave": key1,
        "ChaveOld": key2
    };

    jQuery.ajax({
        type: "POST",
        url: "/Cadastros/ValidarChaveAcesso",
        async: false,
        data: dataString,
        dataType: "json",
        success: function (data) {
            if (data["success"] == "1") {
                check_result = false;
            } else if (data["success"] == "0") {
                check_result = true;
            }
        }
    });

    return check_result;
});

jQuery.validator.addMethod("cnpj", function (value, element) {

    var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
    if (value.length == 0) {
        return false;
    }

    value = value.replace(/\D+/g, '');
    digitos_iguais = 1;

    for (i = 0; i < value.length - 1; i++)
        if (value.charAt(i) != value.charAt(i + 1)) {
            digitos_iguais = 0;
            break;
        }
    if (digitos_iguais)
        return false;

    tamanho = value.length - 2;
    numeros = value.substring(0, tamanho);
    digitos = value.substring(tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }
    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;
    if (resultado != digitos.charAt(0)) {
        return false;
    }
    tamanho = tamanho + 1;
    numeros = value.substring(0, tamanho);
    soma = 0;
    pos = tamanho - 7;
    for (i = tamanho; i >= 1; i--) {
        soma += numeros.charAt(tamanho - i) * pos--;
        if (pos < 2)
            pos = 9;
    }

    resultado = soma % 11 < 2 ? 0 : 11 - soma % 11;

    return (resultado == digitos.charAt(1));
}, "Por favor, insira um n&uacute;mero CNPJ v&aacute;lido.");

jQuery.validator.addMethod("check_exists", function (value, element, param) {

    var is_valid = true;

    if (value == $(param[0]).val()) {

        is_valid = false;

    }

    return is_valid;

}, 'Este CNPJ já encontra-se cadastrado.');

jQuery.validator.addMethod("validDate", function (value, element) {
    return this.optional(element) || moment(value, "DD/MM/YYYY").isValid();
}, "Por favor, insira uma data v&aacute;lida.");

(function () {

    "use strict";

    var loginMobileForm = {

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

            var loginMobileForm = $("#login-mobile-form"), url = loginMobileForm.attr("action");
            loginMobileForm.validate({
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

                                $("#login-alert-error").addClass("hidden");
                                $(location).attr('href', $(location).attr('href') + '/Agendas');

                            } else if (data.response == "permission") {

                                $("#login-alert-error").addClass("hidden");
                                $(location).attr('href', $(location).attr('href') + 'Login/Permission');


                            } else {

                                $("#login-alert-error").removeClass("hidden");

                                if (($("#login-alert-error").position().top - 80) < $(window).scrollTop()) {
                                    $("html, body").animate({
                                        scrollTop: $("#login-alert-error").offset().top - 80
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
                    CNPJ: {
                        required: true,
                        cnpj: true
                    },
                    Senha: {
                        required: true
                    }
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

    loginMobileForm.initialize();

})();

(function () {

    "use strict";

    var senhaForm = {

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

            var senhaForm = $("#senha-form"), url = senhaForm.attr("action");
            senhaForm.validate({
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

                                $("#SenhaAux").val("");
                                $("#ConfirmaSenhaAux").val("");

                            }
                        },
                        complete: function () {
                            submitButton.button("reset");
                        }
                    });
                },
                rules: {
                    ConfirmaSenhaAux: {
                        required: true,
                        minlength: 10,
                        equalTo: "#SenhaAux"
                    }
                },
                messages: {
                    ConfirmaSenhaAux: {
                        minlength: "Por favor, insira pelo menos 10 caracteres.",
                        equalTo: "Por favor, inserir a mesma senha"
                    }
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

    senhaForm.initialize();

})();

(function () {

    "use strict";

    var clienteForm = {

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

            var clienteForm = $("#cliente-form"), url = clienteForm.attr("action");
            clienteForm.validate({
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

                                //$("#login-alert-error").addClass("hidden");
                                //$(location).attr('href', $(location).attr('href') + 'Dashboard');

                            } else {

                                //$("#login-alert-error").removeClass("hidden");

                                //if (($("#login-alert-error").position().top - 80) < $(window).scrollTop()) {
                                //    $("html, body").animate({
                                //        scrollTop: $("#login-alert-error").offset().top - 80
                                //    }, 300);
                                //}

                            }
                        },
                        complete: function () {
                            submitButton.button("reset");
                        }
                    });
                },
                rules: {
                    DataNascimento: {
                        validDate: ["#DataNascimento"]
                    }
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

    clienteForm.initialize();

})();

(function () {

    "use strict";

    var clienteInfoForm = {

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

            var clienteInfoForm = $("#cliente-info-form"), url = clienteInfoForm.attr("action");
            clienteInfoForm.validate({
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

                                //$("#login-alert-error").addClass("hidden");
                                //$(location).attr('href', $(location).attr('href') + 'Dashboard');

                            } else {

                                //$("#login-alert-error").removeClass("hidden");

                                //if (($("#login-alert-error").position().top - 80) < $(window).scrollTop()) {
                                //    $("html, body").animate({
                                //        scrollTop: $("#login-alert-error").offset().top - 80
                                //    }, 300);
                                //}

                            }
                        },
                        complete: function () {
                            submitButton.button("reset");
                        }
                    });
                },
                rules: {
                    ClienteInfoCNPJ: {
                        required: true,
                        cnpj: true
                    }
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

    clienteInfoForm.initialize();

})();