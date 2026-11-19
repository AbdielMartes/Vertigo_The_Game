namespace Vertigo.Registration;

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
