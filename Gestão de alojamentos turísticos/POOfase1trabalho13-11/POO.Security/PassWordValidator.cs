namespace POO.Utils.Security
{
    public static class PasswordValidator
    {
        /// <summary>
        /// Valida se uma password cumpre os requisitos mínimos de segurança.
        /// </summary>
        public static bool Validar(string password, out string mensagemErro)
        {
            mensagemErro = string.Empty;

            if (string.IsNullOrWhiteSpace(password))
            {
                
                return false;
            }

            if (password.Length < 6)
            {
                
                return false;
            }

            // verificar se contém pelo menos um número
            if (!System.Linq.Enumerable.Any(password, char.IsDigit))
            {
                
                return false;
            }

            return true;
        }
    }
}