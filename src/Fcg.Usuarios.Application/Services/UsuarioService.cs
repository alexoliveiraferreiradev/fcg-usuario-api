using AutoMapper;
using Fcg.Usuarios.Applicatio.Services.Interfaces;
using Fcg.Usuarios.Application.Dtos;
using Fcg.Usuarios.Domain.Common;
using Fcg.Usuarios.Domain.Common.Exceptions;
using Fcg.Usuarios.Domain.Common.Interfaces;
using Fcg.Usuarios.Domain.Constants;
using Fcg.Usuarios.Domain.Entitites;
using Fcg.Usuarios.Domain.Enum;
using Fcg.Usuarios.Domain.Repositories.Interfaces;
using Fcg.Usuarios.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Fcg.Usuarios.Application.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UsuarioService> _logger;
        public UsuarioService(IUsuarioRepository usuarioRepository, IPasswordHasher passwordHasher, IMapper mapper, ILogger<UsuarioService> logger)
        {
            _usuarioRepository = usuarioRepository;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UsuarioResponse> PromoverParaAdmin(Guid id)
        {
            _logger.LogInformation("Iniciando processo de promoção para Administrador do usuário {UsuarioId}.", id);
            var usuario = await _usuarioRepository.ObterPorId(id);
            if (usuario == null)
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);

            if (!usuario.Ativo)
                throw new DomainException(MensagensDominio.UsuarioInativo);

            if (usuario.Perfil.Equals(TipoUsuario.Administrador))
            {
                _logger.LogWarning("Operação abortada: O usuário alvo {IdUsuarioRebaixar} já possui o perfil base de adminitrado.", id);
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            usuario.PromoverPerfil(usuario);

            await _usuarioRepository.Atualizar(usuario);

            _logger.LogInformation("Privilégios do usuário {UsuarioId} elevados para Administrador com sucesso no banco de dados.", id);

            var response = _mapper.Map<UsuarioResponse>(usuario);

            _logger.LogInformation("Processo de elevação de privilégio finalizado para o usuário {UsuarioId}.", id);

            return response;
        }

        public async Task<UsuarioResponse> RebaixarParaJogador(Guid idUsuarioRebaixar, Guid idOperador)
        {
            _logger.LogInformation("Iniciando processo de rebaixamento de perfil. Operador: {IdOperador}, Alvo: {IdUsuarioRebaixar}.", idOperador, idUsuarioRebaixar);
            if (idUsuarioRebaixar == idOperador)
            {
                _logger.LogWarning("Tentativa de auto-rebaixamento bloqueada para o operador {IdOperador}.", idOperador);
                throw new DomainException(MensagensDominio.OperacaoRebaixarInvalida);
            }

            var usuario = await _usuarioRepository.ObterPorId(idUsuarioRebaixar);

            if (usuario == null)
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);

            if (!usuario.Ativo)
                throw new DomainException(MensagensDominio.UsuarioInativo);

            if (usuario.Perfil.Equals(TipoUsuario.Jogador))
            {
                _logger.LogWarning("Operação abortada: O usuário alvo {IdUsuarioRebaixar} já possui o perfil base de Jogador.", idUsuarioRebaixar);
                throw new DomainException(MensagensDominio.UsuarioPerfilRebaixarInvalido);
            }

            usuario.RebaixarPerfil();

            await _usuarioRepository.Atualizar(usuario);

            _logger.LogInformation("Privilégios do usuário {IdUsuarioRebaixar} reduzidos para Jogador no banco de dados com sucesso.", idUsuarioRebaixar);

            var response = _mapper.Map<UsuarioResponse>(usuario);

            _logger.LogInformation("Processo de rebaixamento de perfil finalizado com sucesso. Alvo: {IdUsuarioRebaixar}.", idUsuarioRebaixar);

            return response;
        }

        public async Task<UsuarioResponse> CadastrarUsuario(CriaUsuarioRequest request)
        {
            var nomeVO = new Nome(request.Nome);
            var emailVO = new Email(request.Email);
            if (await _usuarioRepository.VerificaEmailCadastrado(request.Email))
            {
                _logger.LogWarning("Cadastro abortado: O e-mail {EmailUsuario} já está cadastrado no sistema.", request.Email);
                throw new DomainException(MensagensDominio.EmailJaCadastrado);
            }
            if (await _usuarioRepository.VerificaNomeCadastrado(request.Nome))
            {
                _logger.LogWarning("Cadastro abortado: O nome de usuário {NomeUsuario} já está em uso.", request.Nome);
                throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            }

            ValidaSenhas(request.Senha, request.ConfirmacaoSenha);

            var senhaCifrada = new Senha(_passwordHasher.HashPassword(request.Senha));

            var usuario = new Usuario(nomeVO, emailVO, senhaCifrada);
            await _usuarioRepository.Adicionar(usuario);

            _logger.LogInformation("Usuário {UsuarioId} cadastrado com sucesso no banco de dados com o perfil base.", usuario.Id);

            _logger.LogInformation("Processo de cadastro finalizado. Usuário {UsuarioId} pronto para login.", usuario.Id);

            return _mapper.Map<UsuarioResponse>(usuario);
        }

        private static void ValidaSenhas(string senhaRequest, string confirmacaoSenhaRequest)
        {
            AssertionConcern.AssertArgumentEmpty(senhaRequest, MensagensDominio.UsuarioSenhaObrigatoria);
            AssertionConcern.AssertArgumentLength(senhaRequest, 8, 60, MensagensDominio.SenhaTamanhoInvalido);
            AssertionConcern.AssertArgumentPasswordStrenght(senhaRequest, MensagensDominio.UsuarioSenhaFraca);
            AssertionConcern.AssertArgumentEquals(senhaRequest, confirmacaoSenhaRequest, MensagensDominio.UsuarioSenhaConfirmacaoDiferente);

        }

        public async Task<UsuarioResponse> AtualizarUsuario(Guid id, UpdateUsuarioRequest request)
        {
            _logger.LogInformation("Iniciando a atualização de perfil do usuário {UsuarioId}.", id);

            var usuario = await _usuarioRepository.ObterPorId(id);
            if (usuario == null) throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);

            if (await _usuarioRepository.VerificaNomeCadastradoParaAlteracao(id, request.NomeUsuario))
            {
                _logger.LogWarning("Atualização abortada para o usuário {UsuarioId}: O nome de usuário {NomeUsuario} já pertence a outra conta.", id, request.NomeUsuario);
                throw new DomainException(MensagensDominio.NomeUsuarioJaCadastrado);
            }

            ValidaSenhas(request.SenhaUsuario, request.ConfirmacaoSenha);

            var novaSenhaCriptografa = new Senha(_passwordHasher.HashPassword(request.SenhaUsuario));

            var novoUsuarioVO = new Nome(request.NomeUsuario);

            usuario.Atualizar(novoUsuarioVO, novaSenhaCriptografa);

            await _usuarioRepository.Atualizar(usuario);

            _logger.LogInformation("Dados do usuário {UsuarioId} atualizados com sucesso no banco de dados.", id);

            _logger.LogInformation("Processo de atualização finalizado para o usuário {UsuarioId}.", id);

            return _mapper.Map<UsuarioResponse>(usuario);
        }

        public async Task<UsuarioResponse> ObterPorId(Guid id)
        {
            return _mapper.Map<UsuarioResponse>(await _usuarioRepository.ObterPorId(id));
        }

        public async Task<IEnumerable<UsuarioResponse>> ObterTodos()
        {
            return _mapper.Map<IEnumerable<UsuarioResponse>>(await _usuarioRepository.ObterTodos());
        }

        public async Task Desativar(DesativaUsuarioRequest deletaUsuarioRequest, Guid idOperador)
        {
            _logger.LogInformation("Iniciando processo de inativação de usuário. Operador: {IdOperador}, Alvo: {IdUsuarioAlvo}, Motivo: '{Motivo}'.",
                    idOperador, deletaUsuarioRequest.Id, deletaUsuarioRequest.MotivoDelecao);

            if (idOperador == deletaUsuarioRequest.Id)
            {
                _logger.LogWarning("Tentativa de auto-inativação bloqueada para o operador {IdOperador}.", idOperador);
                throw new DomainException(MensagensDominio.OperacaoDesativarInvalida);
            }

            var admin = await _usuarioRepository.ObterPorId(idOperador);
            if (admin == null)
            {
                _logger.LogWarning("Operação abortada: Operador {IdOperador} não encontrado no banco de dados.", idOperador);
                throw new DomainException(MensagensDominio.AdminNaoEncontrado);
            }
            var usuario = await _usuarioRepository.ObterPorId(deletaUsuarioRequest.Id);
            if (usuario == null)
            {
                _logger.LogWarning("Operação abortada: Usuário alvo {IdUsuarioAlvo} não encontrado.", deletaUsuarioRequest.Id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            usuario.Desativar(deletaUsuarioRequest.MotivoDelecao);

            await _usuarioRepository.Atualizar(usuario);

            _logger.LogInformation("Usuário {IdUsuarioAlvo} inativado com sucesso no banco de dados pelo operador {IdOperador}.", deletaUsuarioRequest.Id, idOperador);

            _logger.LogInformation("Processo de inativação finalizado com sucesso para o usuário alvo {IdUsuarioAlvo}.", deletaUsuarioRequest.Id);
        }

        public async Task DesativarConta(Guid id)
        {
            _logger.LogInformation("Iniciando processo de inativação da conta do usuário {UsuarioId}.", id);
            var usuario = await _usuarioRepository.ObterPorId(id);
            if (usuario == null)
            {
                _logger.LogWarning("Operação abortada: Usuário {UsuarioId} não encontrado.", id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }

            if (usuario.Perfil == TipoUsuario.Administrador)
            {
                _logger.LogInformation("O usuário {UsuarioId} é um Administrador. Validando regra de segurança de infraestrutura...", id);
                var existeOutroAdmin = await _usuarioRepository.VerificaMaisDeUmAdminCadastrado();
                if (!existeOutroAdmin)
                {
                    _logger.LogWarning("Operação de inativação bloqueada: O usuário {UsuarioId} tentou inativar a conta, mas ele é o ÚNICO Administrador ativo no sistema.", id);
                    throw new DomainException(MensagensDominio.OperacaoDesativarAdminInvalida);
                }
            }
            usuario.DesativarConta();

            await _usuarioRepository.Atualizar(usuario);

            _logger.LogInformation("Conta do usuário {UsuarioId} inativada com sucesso no banco de dados.", id);
        }

        public async Task<UsuarioResponse> Autenticar(LoginRequest request)
        {
            _logger.LogInformation("Iniciando tentativa de autenticação para o e-mail {Email}.", request.Email);
            var usuario = await _usuarioRepository.ObterPorEmail(request.Email);
            if (usuario == null)
            {
                _logger.LogWarning("Falha de autenticação: E-mail {Email} não encontrado no banco de dados.", request.Email);
                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            if (!usuario.Ativo)
            {
                _logger.LogWarning("Falha de autenticação: A conta vinculada ao e-mail {Email} (ID: {UsuarioId}) encontra-se inativa.", request.Email, usuario.Id);
                throw new DomainException(MensagensDominio.UsuarioInativo);
            }
            bool senhaValida = _passwordHasher.VerifyPassword(request.Senha, usuario.Senha.Hash);
            if (!senhaValida)
            {
                _logger.LogWarning("Falha de autenticação: Senha inválida fornecida para o e-mail {Email} (ID: {UsuarioId}).", request.Email, usuario.Id);
                throw new DomainException(MensagensDominio.CrendenciasInvalidas);
            }

            _logger.LogInformation("Autenticação realizada com sucesso para o usuário {UsuarioId}.", usuario.Id);

            return _mapper.Map<UsuarioResponse>(usuario);
        }

        public async Task Reativar(Guid id)
        {
            _logger.LogInformation("Iniciando processo de reativação da conta do usuário {UsuarioId}.", id);
            var usuario = await _usuarioRepository.ObterPorId(id);
            if (usuario == null)
            {
                _logger.LogWarning("Operação de reativação abortada: Usuário {UsuarioId} não encontrado no banco de dados.", id);
                throw new DomainException(MensagensDominio.UsuarioNaoEncontrado);
            }
            usuario.Reativar();

            await _usuarioRepository.Atualizar(usuario);

            _logger.LogInformation("Processo concluído: Conta do usuário {UsuarioId} reativada com sucesso no banco de dados.", id);
        }

        public async Task<UsuarioResponse?> ObterPorEmail(string emailUsuario)
        {
            var email = new Email(emailUsuario);
            return _mapper.Map<UsuarioResponse>(await _usuarioRepository.ObterPorEmail(email.Valor));
        }
    }
}
