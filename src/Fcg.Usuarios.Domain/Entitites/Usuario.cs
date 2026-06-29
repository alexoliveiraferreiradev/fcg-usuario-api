using Fcg.Core.Abstractions.Common;
using Fcg.Core.Abstractions.Common.Exceptions;
using Fcg.Core.Abstractions.Resources;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.ValueObjects;

namespace Fcg.Usuarios.Domain.Entitites
{
    public class Usuario : AggregateRoot
    {
        protected Usuario()
        {
        }
        public Nome NomeUsuario { get; private set; }

        public Email EmailUsuario { get; private set; }

        public Senha Senha { get; private set; }

        public TipoUsuario Perfil { get; private set; }
        public bool Ativo { get; private set; }
        public DateTime DataCadastro { get; private set; }
        public DateTime DataAlteracao { get; private set; }
        public MotivoDesativacao? MotivoDesativacao { get; private set; }



        public Usuario(Nome nomeUsuario, Email emailUsuario, Senha senhaUsuario)
        {
            NomeUsuario = nomeUsuario;
            EmailUsuario = emailUsuario;
            Senha = senhaUsuario;
            Perfil = TipoUsuario.Jogador;
            Ativo = true;
            DataCadastro = DateTime.UtcNow;
            DataAlteracao = DataCadastro;
            ValidarEntidade();
        }



        protected override void ValidarEntidade()
        {
            AssertionConcern.AssertArgumentNotNull(NomeUsuario, MensagensDominio.UsuarioNomeObrigatorio);
            AssertionConcern.AssertArgumentNotNull(EmailUsuario, MensagensDominio.UsuarioEmailObrigatorio);
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

            AtualizarNomeUsuario(novoNome);
            AlterarSenha(novaSenha);
            DataAlteracao = DateTime.UtcNow;
        }

        public void AtualizarNomeUsuario(Nome nomeNovo)
        {
            if (NomeUsuario == nomeNovo) return;
            NomeUsuario = nomeNovo;
        }


        public void AlterarSenha(Senha novaSenha)
        {
            if (Senha == novaSenha) return;
            Senha = novaSenha;
        }

        public void RebaixarPerfil()
        {
            if (Perfil != TipoUsuario.Administrador)
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);

            Perfil = TipoUsuario.Jogador;
        }

        public void PromoverPerfil()
        {
            Perfil = TipoUsuario.Administrador;
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
