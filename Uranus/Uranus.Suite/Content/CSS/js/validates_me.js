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

jQuery.validator.addMethod("cpf", function (value, element) {

    if (value.length > 14) {
        return true;
    }

    value = jQuery.trim(value);

    value = value.replace('.', '');
    value = value.replace('.', '');
    cpf = value.replace('-', '');
    while (cpf.length < 11) cpf = "0" + cpf;
    var expReg = /^0+$|^1+$|^2+$|^3+$|^4+$|^5+$|^6+$|^7+$|^8+$|^9+$/;
    var a = [];
    var b = new Number;
    var c = 11;
    for (i = 0; i < 11; i++) {
        a[i] = cpf.charAt(i);
        if (i < 9) b += (a[i] * --c);
    }
    if ((x = b % 11) < 2) { a[9] = 0 } else { a[9] = 11 - x }
    b = 0;
    c = 11;
    for (y = 0; y < 10; y++) b += (a[y] * c--);
    if ((x = b % 11) < 2) { a[10] = 0; } else { a[10] = 11 - x; }
    if ((cpf.charAt(9) != a[9]) || (cpf.charAt(10) != a[10]) || cpf.match(expReg)) return false;

    return true;
}, "Por favor, insira um n&uacute;mero CPF v&aacute;lido.");

jQuery.validator.addMethod("cnpj", function (value, element) {

    if (value.length <= 14) {
        return true;
    }

    var numeros, digitos, soma, i, resultado, pos, tamanho, digitos_iguais;
    //if (value.length == 0) {
    //    return false;
    //}

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

jQuery.validator.addMethod("empresa_required", function (value, element) {

    if (value == 0 || typeof value === "undefined") {

        $("#EmpresaValidate").removeClass("hidden");

    }
    else {

        $("#EmpresaValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione uma Empresa.");

jQuery.validator.addMethod("funcionario_required", function (value, element) {

    if (value == 0) {

        $("#FuncionarioValidate").removeClass("hidden");

    }
    else {

        $("#FuncionarioValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Funcion&aacute;rio.");

jQuery.validator.addMethod("tipo_required", function (value, element) {

    if (value == 0) {

        $("#TipoValidate").removeClass("hidden");

    }
    else {

        $("#TipoValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Tipo.");

jQuery.validator.addMethod("categoria_required", function (value, element) {

    if (value == 0 || typeof value === "undefined") {

        $("#CategoriaValidate").removeClass("hidden");

    }
    else {

        $("#CategoriaValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Tipo de Categoria.");

jQuery.validator.addMethod("atributo_required", function (value, element) {

    if (value == null || typeof value === "undefined") {

        $("#AtributosValidate").removeClass("hidden");

    }
    else {

        $("#AtributosValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um ou mais Tipos de Atributos.");

jQuery.validator.addMethod("classe_required", function (value, element) {

    if (value == 0 || typeof value === "undefined") {

        $("#ClasseValidate").removeClass("hidden");

    }
    else {

        $("#ClasseValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Tipo de Classe.");

jQuery.validator.addMethod("familia_required", function (value, element) {

    if (value == 0 || typeof value === "undefined") {

        $("#FamiliaValidate").removeClass("hidden");

    }
    else {

        $("#FamiliaValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Tipo de Fam&iacute;lia.");

jQuery.validator.addMethod("grupo_required", function (value, element) {

    if (value == 0 || typeof value === "undefined") {

        $("#GrupoValidate").removeClass("hidden");

    }
    else {

        $("#GrupoValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Tipo de Grupo.");

jQuery.validator.addMethod("area_required", function (value, element) {

    if (value == 0 || typeof value === "undefined") {

        $("#AreaValidate").removeClass("hidden");

    }
    else {

        $("#AreaValidate").addClass("hidden");
        return true;

    }

}, "Por favor, selecione um Tipo de &Aacute;rea.");

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

                                //alert('Teste...');
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
                ignore: 'input[type=hidden]',
                rules: {
                    Empresa: {
                        empresa_required: true
                    },
                    CPFCNPJ: {
                        required: true,
                        cpf: true,
                        cnpj: true
                        //check_exists: ["#ClienteCNPJAux"]
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

    var funcionarioForm = {

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

            var funcionarioForm = $("#funcionario-form"), url = funcionarioForm.attr("action");
            funcionarioForm.validate({
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

                                //alert('Teste...');
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
                ignore: 'input[type=hidden]',
                rules: {
                    Empresa: {
                        empresa_required: true
                    },
                    CPF: {
                        required: true,
                        cpf: true
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

    funcionarioForm.initialize();

})();

(function () {

    "use strict";

    var vendedorForm = {

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

            var vendedorForm = $("#vendedor-form"), url = vendedorForm.attr("action");
            vendedorForm.validate({
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

                                //alert('Teste...');
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
                ignore: 'input[type=hidden]',
                rules: {
                    Funcionario: {
                        funcionario_required: true
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

    vendedorForm.initialize();

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

                                //alert('Teste...');
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

(function () {

    "use strict";

    var atributoForm = {

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

            var atributoForm = $("#atributo-form"), url = atributoForm.attr("action");
            atributoForm.validate({
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

                                //alert('Teste...');
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
                ignore: 'input[type=hidden]',
                rules: {
                    Tipo: {
                        tipo_required: true
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

    atributoForm.initialize();

})();

(function () {

    "use strict";

    var produtoForm = {

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

            var produtoForm = $("#produto-form"), url = produtoForm.attr("action");
            produtoForm.validate({
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

                                //alert('Teste...');
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
                ignore: 'input[type=hidden]',
                rules: {
                    Empresa: {
                        empresa_required: true
                    },
                    Categoria: {
                        categoria_required: true
                    },
                    Atributos: {
                        atributo_required: true
                    },
                    Classe: {
                        classe_required: true
                    },
                    Familia: {
                        familia_required: true
                    },
                    Grupo: {
                        grupo_required: true
                    },
                    Area: {
                        area_required: true
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

    produtoForm.initialize();

})();

(function () {

    "use strict";

    var fornecedorForm = {

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

            var fornecedorForm = $("#fornecedor-form"), url = fornecedorForm.attr("action");
            fornecedorForm.validate({
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

                                //alert('Teste...');
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
                ignore: 'input[type=hidden]',
                rules: {
                    Empresa: {
                        empresa_required: true
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

    fornecedorForm.initialize();

})();