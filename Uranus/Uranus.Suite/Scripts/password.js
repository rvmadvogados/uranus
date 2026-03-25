/**
 * @author Òscar Casajuana a.k.a. elboletaire <elboletaire at underave dot net>
 * @link https://github.com/elboletaire/password-strength-meter
 */
// eslint-disable-next-line
; (function ($) {
    'use strict';

    var Password = function ($object, options) {
        var defaults = {
            shortPass: 'A senha é muito curta',
            badPass: 'Fraco, tente combinar letras e números',
            goodPass: 'Médio, tente usar caracteres especiais',
            strongPass: 'Senha forte',
            containsUsername: 'A senha contém o nome de usuário',
            enterPass: 'Digite sua senha',
            showPercent: false,
            showText: true,
            animate: true,
            animateSpeed: 'fast',
            username: false,
            usernamePartialMatch: true,
            minimumLength: 4
        };

        options = $.extend({}, defaults, options);

        /**
         * Returns strings based on the score given.
         *
         * @param int score Score base.
         * @return string
         */
        function scoreText(score) {
            if (score === -1) {
                return options.shortPass;
            }
            if (score === -2) {
                return options.containsUsername;
            }

            score = score < 0 ? 0 : score;

            if (score < 34) {
                return options.badPass;
            }
            if (score < 68) {
                return options.goodPass;
            }

            return options.strongPass;
        }

        /**
         * Returns a value between -2 and 100 to score
         * the user's password.
         *
         * @param  string password The password to be checked.
         * @param  string username The username set (if options.username).
         * @return int
         */
        function calculateScore(password, username) {
            // Identity rules
            var minLength = 8;
            var hasUpper = /[A-Z]/.test(password);
            var hasLower = /[a-z]/.test(password);
            var hasDigit = /[0-9]/.test(password);
            var hasSpecial = /[!@\$%\^&*(),.?":{}|<>\[\]\\\/;'`~\-_=+]/.test(password);
            var hasSpace = /\s/.test(password);

            if (password.length < minLength) {
                return -1; // Muito curta
            }
            if (hasSpace) {
                return -1; // Não pode conter espaços
            }
            if (!(hasUpper && hasLower && hasDigit && hasSpecial)) {
                return 0; // Não atende aos requisitos mínimos
            }
            if (options.username) {
                if (password.toLowerCase() === username.toLowerCase()) {
                    return -2;
                }
                if (options.usernamePartialMatch && username.length) {
                    var user = new RegExp(username.toLowerCase());
                    if (password.toLowerCase().match(user)) {
                        return -2;
                    }
                }
            }
            // Se chegou aqui, atende todos os requisitos do Identity
            return 100;
        }

        function scoreText(score) {
            if (score === -1) {
                return 'A senha deve ter no mínimo 8 caracteres, conter maiúscula, minúscula, número e caractere especial, e não pode conter espaços.';
            }
            if (score === -2) {
                return options.containsUsername;
            }
            if (score < 100) {
                return 'A senha não atende aos requisitos mínimos do sistema.';
            }
            return 'Senha válida!';
        }

        /**
         * Checks for repetition of characters in
         * a string
         *
         * @param int rLen Repetition length.
         * @param string str The string to be checked.
         * @return string
         */
        function checkRepetition(rLen, str) {
            var res = "", repeated = false;
            for (var i = 0; i < str.length; i++) {
                repeated = true;
                for (var j = 0; j < rLen && (j + i + rLen) < str.length; j++) {
                    repeated = repeated && (str.charAt(j + i) === str.charAt(j + i + rLen));
                }
                if (j < rLen) {
                    repeated = false;
                }
                if (repeated) {
                    i += rLen - 1;
                    repeated = false;
                }
                else {
                    res += str.charAt(i);
                }
            }
            return res;
        }

        /**
         * Initializes the plugin creating and binding the
         * required layers and events.
         *
         * @return void
         */
        function init() {
            var shown = true;
            var $text = options.showText;
            var $percentage = options.showPercent;
            var $graybar = $('<div>').addClass('pass-graybar');
            var $colorbar = $('<div>').addClass('pass-colorbar');
            var $insert = $('<div>').addClass('pass-wrapper').append(
                $graybar.append($colorbar)
            );

            $object.parent().addClass('pass-strength-visible');
            if (options.animate) {
                $insert.css('display', 'none');
                shown = false;
                $object.parent().removeClass('pass-strength-visible');
            }

            if (options.showPercent) {
                $percentage = $('<span>').addClass('pass-percent').text('0%');
                $insert.append($percentage);
            }

            if (options.showText) {
                $text = $('<span>').addClass('pass-text').html(options.enterPass);
                $insert.append($text);
            }

            $object.after($insert);

            $object.keyup(function () {
                var username = options.username || '';
                if (username) {
                    username = $(username).val();
                }

                var score = calculateScore($object.val(), username);
                $object.trigger('password.score', [score]);
                var perc = score < 0 ? 0 : score;
                $colorbar.css({
                    backgroundPosition: "0px -" + perc + "px",
                    width: perc + '%'
                });

                if (options.showPercent) {
                    $percentage.html(perc + '%');
                }

                if (options.showText) {
                    var text = scoreText(score);
                    if (!$object.val().length && score <= 0) {
                        text = options.enterPass;
                    }

                    if ($text.html() !== $('<div>').html(text).html()) {
                        $text.html(text);
                        $object.trigger('password.text', [text, score]);
                    }
                }
            });

            if (options.animate) {
                $object.focus(function () {
                    if (!shown) {
                        $insert.slideDown(options.animateSpeed, function () {
                            shown = true;
                            $object.parent().addClass('pass-strength-visible');
                        });
                    }
                });

                $object.blur(function () {
                    if (!$object.val().length && shown) {
                        $insert.slideUp(options.animateSpeed, function () {
                            shown = false;
                            $object.parent().removeClass('pass-strength-visible')
                        });
                    }
                });
            }

            return this;
        }

        return init.call(this);
    }

    // Bind to jquery
    $.fn.password = function (options) {
        return this.each(function () {
            new Password($(this), options);
        });
    };
})(jQuery);
