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
        public Nome NomeUser { get; private set; }

        public Email EmailUser { get; private set; }

        public Senha Senha { get; private set; }

        public TipoUser Perfil { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime DataAlteracao { get; private set; }
        public MotivoDesativacao? MotivoDesativacao { get; private set; }



        public User(Nome nomeUser, Email emailUser, Senha senhaUser)
        {
            NomeUser = nomeUser;
            EmailUser = emailUser;
            Senha = senhaUser;
            Perfil = TipoUser.Jogador;
            Ativo = true;
            DataCadastro = DateTime.UtcNow;
            DataAlteracao = DataCadastro;
            ValidarEntidade();
        }



        protected override void ValidarEntidade()
        {
            AssertionConcern.AssertArgumentNotNull(NomeUser, MensagensDominio.UsuarioNomeObrigatorio);
            AssertionConcern.AssertArgumentNotNull(EmailUser, MensagensDominio.UsuarioEmailObrigatorio);
            AssertionConcern.AssertArgumentNotNull(Senha, MensagensDominio.UsuarioSenhaObrigatoria);
        }

        public void Desativar(MotivoDesativacao motivo)
        {
            if (!Ativo) throw new DomainException(MensagensDominio.UsuarioJaDesativado);

            Ativo = false;
            DataAlteracao = DateTime.UtcNow;
            MotivoDesativacao = motivo;
        }
        public void DesativarConta()
        {
            if (!Ativo) throw new DomainException(MensagensDominio.UsuarioJaDesativado);

            Ativo = false;
            DataAlteracao = DateTime.UtcNow;
        }

        public void Atualizar(Nome novoNome, Senha novaSenha)
        {
            if (!Ativo) throw new DomainException(MensagensDominio.UsuarioInativo);

            AtualizarNomeUser(novoNome);
            AlterarSenha(novaSenha);
            DataAlteracao = DateTime.UtcNow;
        }

        public void AtualizarNomeUser(Nome nomeNovo)
        {
            if (NomeUser == nomeNovo) return;
            NomeUser = nomeNovo;
        }


        public void AlterarSenha(Senha novaSenha)
        {
            if (Senha == novaSenha) return;
            Senha = novaSenha;
        }

        public void RebaixarPerfil()
        {
            if (Perfil != TipoUser.Administrador)
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);

            Perfil = TipoUser.Jogador;
        }

        public void PromoverPerfil()
        {
            Perfil = TipoUser.Administrador;
            DataAlteracao = DateTime.UtcNow;
        }

        public void Reativar()
        {
            if (Ativo) throw new DomainException(MensagensDominio.UsuarioAtivo);
            Ativo = true;
            DataAlteracao = DateTime.UtcNow;
            MotivoDesativacao = null;
        }
    }
}
