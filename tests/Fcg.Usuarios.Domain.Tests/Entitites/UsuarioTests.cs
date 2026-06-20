using System;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.ValueObjects;
using Xunit;

namespace Fcg.Usuarios.Domain.Tests.Entitites
{
    public class UsuarioTests
    {
        private Nome ObterNomeValido() => new Nome("John Doe");
        private Email ObterEmailValido() => new Email("john.doe@email.com");
        private Senha ObterSenhaValida() => new Senha("StrongPassword123!");

        #region Construtor Tests

        [Fact]
        public void Construtor_ComDadosValidos_DeveCriarUsuarioAtivoComPerfilJogador()
        {
            // Arrange
            var nome = ObterNomeValido();
            var email = ObterEmailValido();
            var senha = ObterSenhaValida();

            // Act
            var usuario = new Usuario(nome, email, senha);

            // Assert
            Assert.Equal(nome, usuario.NomeUsuario);
            Assert.Equal(email, usuario.EmailUsuario);
            Assert.Equal(senha, usuario.Senha);
            Assert.Equal(TipoUsuario.Jogador, usuario.Perfil);
            Assert.True(usuario.Ativo);
            Assert.Null(usuario.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - usuario.DataCadastro).TotalSeconds < 5);
            Assert.Equal(usuario.DataCadastro, usuario.DataAlteracao);
        }

        [Fact]
        public void Construtor_SemNome_DeveLancarDomainException()
        {
            // Arrange
            Nome nomeNulo = null!;
            var email = ObterEmailValido();
            var senha = ObterSenhaValida();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => new Usuario(nomeNulo, email, senha));
            Assert.Equal(MensagensDominio.UsuarioNomeObrigatorio, excecao.Message);
        }

        #endregion

        #region Desativar Tests

        [Fact]
        public void Desativar_ComMotivo_DeveDesativarEMarcarMotivo()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var motivo = MotivoDesativacao.Inatividade;

            // Act
            usuario.Desativar(motivo);

            // Assert
            Assert.False(usuario.Ativo);
            Assert.Equal(motivo, usuario.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - usuario.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Desativar_UsuarioJaDesativado_DeveLancarDomainException()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            usuario.Desativar(MotivoDesativacao.SolicitacaoDoUsuario);

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => usuario.Desativar(MotivoDesativacao.Inatividade));
            Assert.Equal(MensagensDominio.UsuarioJaDesativado, excecao.Message);
        }

        [Fact]
        public void DesativarConta_UsuarioAtivo_DeveDesativar()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act
            usuario.DesativarConta();

            // Assert
            Assert.False(usuario.Ativo);
            Assert.Null(usuario.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - usuario.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void DesativarConta_UsuarioJaDesativado_DeveLancarDomainException()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            usuario.DesativarConta();

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => usuario.DesativarConta());
            Assert.Equal(MensagensDominio.UsuarioJaDesativado, excecao.Message);
        }

        #endregion

        #region Atualizar Tests

        [Fact]
        public void Atualizar_ComDadosValidos_DeveAtualizarCampos()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            var novoNome = new Nome("Jane Doe");
            var novaSenha = new Senha("NewStrongPassword456!");

            // Act
            usuario.Atualizar(novoNome, novaSenha);

            // Assert
            Assert.Equal(novoNome, usuario.NomeUsuario);
            Assert.Equal(novaSenha, usuario.Senha);
            Assert.True((DateTime.UtcNow - usuario.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Atualizar_UsuarioInativo_DeveLancarDomainException()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            usuario.DesativarConta();

            var novoNome = new Nome("Jane Doe");
            var novaSenha = new Senha("NewStrongPassword456!");

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => usuario.Atualizar(novoNome, novaSenha));
            Assert.Equal(MensagensDominio.UsuarioInativo, excecao.Message);
        }

        #endregion

        #region Perfil Tests

        [Fact]
        public void PromoverPerfil_UsuarioJogador_DeveAlterarPerfilParaAdministrador()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act
            usuario.PromoverPerfil();

            // Assert
            Assert.Equal(TipoUsuario.Administrador, usuario.Perfil);
            Assert.True((DateTime.UtcNow - usuario.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void RebaixarPerfil_UsuarioAdministrador_DeveAlterarPerfilParaJogador()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            usuario.PromoverPerfil(); 

            // Act
            usuario.RebaixarPerfil();

            // Assert
            Assert.Equal(TipoUsuario.Jogador, usuario.Perfil);
        }

        [Fact]
        public void RebaixarPerfil_UsuarioJaJogador_DeveLancarDomainException()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => usuario.RebaixarPerfil());
            Assert.Equal(MensagensDominio.UsuarioPerfilRebaixarInvalido, excecao.Message);
        }

        #endregion

        #region Reativar Tests

        [Fact]
        public void Reativar_UsuarioInativo_DeveReativarELimparMotivo()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());
            usuario.Desativar(MotivoDesativacao.Inatividade);

            // Act
            usuario.Reativar();

            // Assert
            Assert.True(usuario.Ativo);
            Assert.Null(usuario.MotivoDesativacao);
            Assert.True((DateTime.UtcNow - usuario.DataAlteracao).TotalSeconds < 5);
        }

        [Fact]
        public void Reativar_UsuarioAtivo_DeveLancarDomainException()
        {
            // Arrange
            var usuario = new Usuario(ObterNomeValido(), ObterEmailValido(), ObterSenhaValida());

            // Act & Assert
            var excecao = Assert.Throws<DomainException>(() => usuario.Reativar());
            Assert.Equal(MensagensDominio.UsuarioAtivo, excecao.Message);
        }

        #endregion
    }
}
