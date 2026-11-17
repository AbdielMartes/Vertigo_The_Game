using System;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

namespace Vertigo.Client.UI.Utilities.Validation;

public static class RegistrationRules
{
    public const int UsernameMinimumLength = 3;
    public const int UsernameMaximumLength = 20;
    public const int EmailMaximumLength = 60;
    public const int PasswordMinimumLength = 8;
    public const int PasswordMaximumLength = 20;

    public static FieldValidationError ValidateUsername(string? username)
    {
        FieldValidationError error = FieldValidationError.None;
        string normalized = username?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            error = FieldValidationError.Required;
        }
        else if (normalized.Length < UsernameMinimumLength)
        {
            error = FieldValidationError.TooShort;
        }
        else if (normalized.Length > UsernameMaximumLength)
        {
            error = FieldValidationError.TooLong;
        }
        else if (!normalized.All(IsAllowedUsernameCharacter) || !normalized.Any(IsAsciiLetter))
        {
            error = FieldValidationError.InvalidFormat;
        }

        return error;
    }

    public static FieldValidationError ValidateEmail(string? email)
    {
        FieldValidationError error = FieldValidationError.None;
        string normalized = email?.Trim() ?? string.Empty;

        if (normalized.Length == 0)
        {
            error = FieldValidationError.Required;
        }
        else if (normalized.Length > EmailMaximumLength)
        {
            error = FieldValidationError.TooLong;
        }
        else if (!IsValidEmail(normalized))
        {
            error = FieldValidationError.InvalidFormat;
        }

        return error;
    }

    public static FieldValidationError ValidatePassword(string? password)
    {
        FieldValidationError error = FieldValidationError.None;

        if (string.IsNullOrEmpty(password))
        {
            error = FieldValidationError.Required;
        }
        else if (password.Length < PasswordMinimumLength)
        {
            error = FieldValidationError.TooShort;
        }
        else if (password.Length > PasswordMaximumLength)
        {
            error = FieldValidationError.TooLong;
        }
        else if (!password.All(IsAllowedPasswordCharacter)
                 || !password.Any(char.IsUpper)
                 || !password.Any(char.IsDigit)
                 || !password.Any(IsSpecialPasswordCharacter))
        {
            error = FieldValidationError.PasswordRequirementsNotMet;
        }

        return error;
    }

    public static FieldValidationError ValidateConfirmation(string? password, string? confirmation)
    {
        FieldValidationError error = FieldValidationError.None;

        if (string.IsNullOrEmpty(confirmation))
        {
            error = FieldValidationError.Required;
        }
        else if (!string.Equals(password, confirmation, StringComparison.Ordinal))
        {
            error = FieldValidationError.PasswordsDoNotMatch;
        }

        return error;
    }

    private static bool IsAllowedUsernameCharacter(char character)
    {
        UnicodeCategory category = char.GetUnicodeCategory(character);
        bool isLetter = char.IsLetter(character);
        bool isNumber = category == UnicodeCategory.DecimalDigitNumber;
        return isLetter || isNumber;
    }

    private static bool IsAsciiLetter(char character)
    {
        bool isLetter = char.IsAsciiLetter(character);
        return isLetter;
    }

    private static bool IsSpecialPasswordCharacter(char character)
    {
        bool isSpecial = AllowedPasswordSymbols.Contains(character);
        return isSpecial;
    }

    private static bool IsAllowedPasswordCharacter(char character)
    {
        bool isAllowed = char.IsAsciiLetterOrDigit(character)
            || IsSpecialPasswordCharacter(character);
        return isAllowed;
    }

    private const string AllowedPasswordSymbols = "!@#$%&*?_+-.";

    private static bool IsValidEmail(string email)
    {
        bool isValid = false;
        int separator = email.IndexOf('@');
        int lastSeparator = email.LastIndexOf('@');

        if (separator > 0 && separator == lastSeparator && separator < email.Length - 1
            && !email.Any(char.IsWhiteSpace))
        {
            string local = email[..separator];
            string domain = email[(separator + 1)..];
            string[] labels = domain.Split('.');
            bool validLocal = local.Length <= 64 && !local.StartsWith('.')
                && !local.EndsWith('.') && !local.Contains("..", StringComparison.Ordinal)
                && local.All(IsAllowedEmailLocalCharacter);
            bool validDomain = labels.Length >= 2 && labels.All(IsValidDomainLabel);

            if (validLocal && validDomain)
            {
                try
                {
                    MailAddress address = new(email);
                    isValid = string.Equals(address.Address, email, StringComparison.Ordinal);
                }
                catch (FormatException)
                {
                    isValid = false;
                }
            }
        }

        return isValid;
    }

    private static bool IsAllowedEmailLocalCharacter(char character)
    {
        bool isAllowed = char.IsAsciiLetterOrDigit(character)
            || character is '.' or '_' or '%' or '+' or '-';
        return isAllowed;
    }

    private static bool IsValidDomainLabel(string label)
    {
        bool valid = label.Length is >= 1 and <= 63
            && label[0] != '-' && label[^1] != '-'
            && label.All(character => char.IsAsciiLetterOrDigit(character) || character == '-');
        return valid;
    }
}
