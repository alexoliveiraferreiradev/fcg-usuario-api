using Fcg.Usuarios.Domain.Common.Exceptions;

namespace Fcg.Usuarios.Domain.Common
{
    /// <summary>
    /// Fornece métodos utilitários para validação de argumentos e lançamento de exceções de domínio.
    /// </summary>
    public class AssertionConcern
    {
        /// <summary>
        /// Valida se o comprimento de uma string é menor ou igual a um valor máximo especificado, e lança uma exceção de domínio com a mensagem fornecida caso seja maior.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="maximum"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentLength(string stringValue, int maximum, string message)
        {
            int length = stringValue.Trim().Length;
            if (length > maximum)
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se o comprimento de uma string está dentro do intervalo especificado (inclusivo),
        /// e lança uma exceção de domínio com a mensagem fornecida caso esteja fora do intervalo.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentLength(string stringValue, int minimum, int maximum, string message)
        {
            if (minimum > maximum)
            {
                throw new DomainException("valor mínimo deve ter menos ou igual ao valor máximo", nameof(minimum));
            }

            if (stringValue == null)
            {
                throw new DomainException(message);
            }

            int length = stringValue.Trim().Length;
            if (length < minimum || length > maximum)
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se uma string é nula, vazia ou composta apenas por espaços em branco, e lança uma exceção de domínio com a mensagem fornecida caso seja.
        /// </summary>
        /// <param name="stringValue"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentEmpty(string stringValue, string message)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                throw new DomainException(message);
            }
        }
        /// <summary>
        /// Valida se um objeto é nulo e lança uma exceção de domínio com a mensagem fornecida caso seja nulo.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentNotNull(object object1, string message)
        {
            if (object1 == null)
            {
                throw new DomainException(message);
            }
        }
        /// <summary>
        /// Valida se dois objetos são iguais, utilizando o método Equals, e lança uma exceção de domínio com a mensagem fornecida caso não sejam iguais.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentEquals(object object1, object object2, string message)
        {
            if (!object1.Equals(object2))
            {
                throw new DomainException(message);
            }
        }
        /// <summary>
        /// Valida se dois objetos são diferentes, utilizando o método Equals, e lança uma exceção de domínio com a mensagem fornecida caso sejam iguais.
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentNotEquals(object object1, object object2, string message)
        {
            if (object1.Equals(object2))
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida a força da senha, verificando se ela tem pelo menos 8 caracteres, contém letras maiúsculas, minúsculas, números e caracteres especiais.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentPasswordStrenght(string password, string message)
        {
            int length = password.Length;
            bool hasUpper = false;
            bool hasLower = false;
            bool hasDigit = false;
            bool hasSpecial = false;

            for (int i = 0; i < length; i++)
            {
                char c = password[i];
                if (char.IsUpper(c)) hasUpper = true;
                else if (char.IsLower(c)) hasLower = true;
                else if (char.IsDigit(c)) hasDigit = true;
                else hasSpecial = true;

                if (hasUpper && hasLower && hasDigit && hasSpecial && length >= 8)
                {
                    return;
                }
            }

            if (length < 8 || !hasUpper || !hasLower || !hasDigit || !hasSpecial)
            {
                throw new DomainException(message);
            }

        }
        public static void AssertStateTrue(bool boolValue, string message)
        {
            if (!boolValue)
            {
                throw new DomainException(message);
            }
        }

        public static void AssertStateFalse(bool boolValue, string message)
        {
            if (!boolValue)
            {
                throw new DomainException(message);
            }
        }

        /// <summary>
        /// Valida se o formato do email é válido, verificando se ele contém um endereço de email válido
        /// e não possui pontos consecutivos.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentEmailFormat(string email, string message)
        {
            try
            {
                var emailAddress = new System.Net.Mail.MailAddress(email);
                if (email.Contains("..")) throw new DomainException(message);
                if (emailAddress.Address != email) throw new DomainException(message);
                if (!email.Contains(".")) throw new DomainException(message);
            }
            catch
            {
                throw new DomainException(message);
            }
        }
        /// <summary>
        /// Valida se o valor decimal é negativo, e lança uma exceção de domínio com a mensagem fornecida caso seja negativo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentValueFormat(decimal value, string message)
        {
            if (value < 0) throw new DomainException(message);
        }

        /// <summary>
        /// Valida se um valor inteiro está dentro de um intervalo especificado (inclusivo), e lança uma exceção de domínio com a mensagem fornecida caso esteja fora do intervalo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="minimum"></param>
        /// <param name="maximum"></param>
        /// <param name="message"></param>
        /// <exception cref="DomainException"></exception>
        public static void AssertArgumentRange(int value, int minimum, int maximum, string message)
        {
            if (value < minimum || value > maximum)
            {
                throw new DomainException(message);
            }
        }

        public static void AssertArgumentRealValues(string value, string message)
        {
            if (value.Trim().Equals("Nome do Usuário", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(message);
            }
            if (value.Trim().Equals("usuario@email.com.br", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(message);
            }
            if (value.Trim().Equals("Nome do Jogo", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(message);
            }
            if (value.Trim().Equals("Descrição do Jogo", StringComparison.OrdinalIgnoreCase))
            {
                throw new DomainException(message);
            }
        }
    }
}
