namespace Vertigo.Client.UI.Utilities.Validation;

public enum FieldValidationError
{
    None,
    Required,
    TooShort,
    TooLong,
    InvalidFormat,
    PasswordRequirementsNotMet,
    PasswordsDoNotMatch
}
