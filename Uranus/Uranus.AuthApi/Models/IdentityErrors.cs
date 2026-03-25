using Microsoft.AspNetCore.Identity;

namespace Uranus.AuthApi.Models
{
  
    public class CustomIdentityErrorDescriber : IdentityErrorDescriber
    {
        public override IdentityError DefaultError()
            => new IdentityError { Code = nameof(DefaultError), Description = "Ocorreu um erro desconhecido." };


        public override IdentityError PasswordTooShort(int length)
            => new IdentityError { Code = nameof(PasswordTooShort), Description = $"A senha deve ter pelo menos {length} caracteres." };

        public override IdentityError PasswordRequiresNonAlphanumeric()
            => new IdentityError { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "A senha deve conter ao menos um caractere não alfanumérico." };

        public override IdentityError PasswordRequiresDigit()
            => new IdentityError { Code = nameof(PasswordRequiresDigit), Description = "A senha deve conter ao menos um dígito (0-9)." };

        public override IdentityError PasswordRequiresLower()
            => new IdentityError { Code = nameof(PasswordRequiresLower), Description = "A senha deve conter ao menos uma letra minúscula." };

        public override IdentityError PasswordRequiresUpper()
            => new IdentityError { Code = nameof(PasswordRequiresUpper), Description = "A senha deve conter ao menos uma letra maiúscula." };

        public override IdentityError InvalidUserName(string userName)
            => new IdentityError { Code = nameof(InvalidUserName), Description = $"O nome de usuário '{userName}' é inválido." };

        public override IdentityError DuplicateUserName(string userName)
            => new IdentityError { Code = nameof(DuplicateUserName), Description = $"O nome de usuário '{userName}' já está em uso." };

        public override IdentityError InvalidRoleName(string role)
            => new IdentityError { Code = nameof(InvalidRoleName), Description = $"O nome de perfil '{role}' é inválido." };

        public override IdentityError DuplicateRoleName(string role)
            => new IdentityError { Code = nameof(DuplicateRoleName), Description = $"O nome de perfil '{role}' já está em uso." };

        public override IdentityError UserAlreadyHasPassword()
            => new IdentityError { Code = nameof(UserAlreadyHasPassword), Description = "O usuário já possui uma senha definida." };

        public override IdentityError UserLockoutNotEnabled()
            => new IdentityError { Code = nameof(UserLockoutNotEnabled), Description = "O bloqueio de usuário não está habilitado." };

        public override IdentityError UserAlreadyInRole(string role)
            => new IdentityError { Code = nameof(UserAlreadyInRole), Description = $"O usuário já pertence ao perfil '{role}'." };

        public override IdentityError UserNotInRole(string role)
            => new IdentityError { Code = nameof(UserNotInRole), Description = $"O usuário não pertence ao perfil '{role}'." };

        public override IdentityError PasswordMismatch()
            => new IdentityError { Code = nameof(PasswordMismatch), Description = "Senha incorreta." };

        public override IdentityError LoginAlreadyAssociated()
            => new IdentityError { Code = nameof(LoginAlreadyAssociated), Description = "Já existe um login associado a este usuário." };

        public override IdentityError InvalidToken()
            => new IdentityError { Code = nameof(InvalidToken), Description = "Token inválido." };

        public override IdentityError RecoveryCodeRedemptionFailed()
            => new IdentityError { Code = nameof(RecoveryCodeRedemptionFailed), Description = "O código de recuperação não pôde ser utilizado." };

    }
}