using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Users.Domain.Enum;
using Fcg.Users.Domain.ValueObjects;

namespace Fcg.Users.Domain.Entitites
{
    public class User : AggregateRoot
    {
        protected User()
        {
        }
        public Name Name { get; private set; }

        public Email Email { get; private set; }

        public Password Password { get; private set; }

        public UserRole Role { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        public DeactivationReason? DeactivationReason { get; private set; }



        public User(Name name, Email email, Password password)
        {
            Name = name;
            Email = email;
            Password = password;
            Role = UserRole.Player;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = CreatedAt;
            ValidateEntity();
        }



        protected override void ValidateEntity()
        {
            AssertionConcern.AssertArgumentNotNull(Name, MensagensDominio.UsuarioNomeObrigatorio);
            AssertionConcern.AssertArgumentNotNull(Email, MensagensDominio.UsuarioEmailObrigatorio);
            AssertionConcern.AssertArgumentNotNull(Password, MensagensDominio.UsuarioSenhaObrigatoria);
        }

        public void Deactivate(DeactivationReason reason)
        {
            if (!IsActive) throw new DomainException(MensagensDominio.UsuarioJaDesativado);

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
            DeactivationReason = reason;
        }
        public void DeactivateAccount()
        {
            if (!IsActive) throw new DomainException(MensagensDominio.UsuarioJaDesativado);

            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(Name newName, Password newPassword)
        {
            if (!IsActive) throw new DomainException(MensagensDominio.UsuarioInativo);

            UpdateName(newName);
            ChangePassword(newPassword);
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateName(Name newName)
        {
            if (Name == newName) return;
            Name = newName;
        }


        public void ChangePassword(Password newPassword)
        {
            if (Password == newPassword) return;
            Password = newPassword;
        }

        public void DemoteRole()
        {
            if (Role != UserRole.Admin)
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);

            Role = UserRole.Player;
        }

        public void PromoteRole()
        {
            Role = UserRole.Admin;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Reactivate()
        {
            if (IsActive) throw new DomainException(MensagensDominio.UsuarioAtivo);
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
            DeactivationReason = null;
        }
    }
}
