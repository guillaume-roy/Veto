using System.Text.RegularExpressions;
using Veto.Application.Exceptions;

namespace Veto.Application.Clients.UseCases
{
    internal class CreateClientUseCaseValidator
    {
        public void Validate(CreateClientUseCaseRequest request)
        {
            ValidateEmail(request.Email);
            ValidatePassword(request.Password);
        }

        private void ValidateEmail(string email)
        {
            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(email);

            if (!match.Success)
            {
                throw new UseCaseValidationException($"{nameof(email)} is invalid");
            }
        }

        private void ValidatePassword(string password)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");

            var isValidated = hasNumber.IsMatch(password) && hasUpperChar.IsMatch(password) && hasMinimum8Chars.IsMatch(password);

            if (!isValidated)
            {
                throw new UseCaseValidationException($"{nameof(password)} is invalid");
            }
        }
    }
}
