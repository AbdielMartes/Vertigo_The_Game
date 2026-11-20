using Vertigo.Registration;

namespace Vertigo.Client.UnitTests.Validation;

public sealed class RegistrationRulesTests
{
    [Theory]
    [InlineData("Ab3", FieldValidationError.None)]
    [InlineData("Niño2026", FieldValidationError.None)]
    [InlineData("A", FieldValidationError.TooShort)]
    [InlineData("", FieldValidationError.Required)]
    [InlineData("Juan Perez", FieldValidationError.InvalidFormat)]
    [InlineData("Juan_23", FieldValidationError.InvalidFormat)]
    [InlineData("Juan!", FieldValidationError.InvalidFormat)]
    [InlineData("12345", FieldValidationError.InvalidFormat)]
    [InlineData("Ñ123", FieldValidationError.InvalidFormat)]
    [InlineData("a123", FieldValidationError.None)]
    [InlineData("A123", FieldValidationError.None)]
    [InlineData("a<script>", FieldValidationError.InvalidFormat)]
    public void ValidateUsername_Formats_ReturnsExpected(string value, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidateUsername(value));
    }

    [Fact]
    public void ValidateUsername_Boundaries_EnforcesMaximum()
    {
        Assert.Equal(FieldValidationError.None, RegistrationRules.ValidateUsername(new string('A', 20)));
        Assert.Equal(FieldValidationError.TooLong, RegistrationRules.ValidateUsername(new string('A', 21)));
    }

    [Theory]
    [InlineData("user@example.com", FieldValidationError.None)]
    [InlineData("user+tag@example.com", FieldValidationError.None)]
    [InlineData("first.last@example.com", FieldValidationError.None)]
    [InlineData(".user@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user.@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("first..last@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@example", FieldValidationError.InvalidFormat)]
    [InlineData("userexample.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@.com", FieldValidationError.InvalidFormat)]
    [InlineData("user@domain..com", FieldValidationError.InvalidFormat)]
    [InlineData("user @example.com", FieldValidationError.InvalidFormat)]
    [InlineData("", FieldValidationError.Required)]
    [InlineData("user'<script>@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user;drop@example.com", FieldValidationError.InvalidFormat)]
    [InlineData("user\\name@example.com", FieldValidationError.InvalidFormat)]
    public void ValidateEmail_Formats_ReturnsExpected(string value, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidateEmail(value));
    }

    [Fact]
    public void ValidateEmail_OverMaximumLength_ReturnsTooLong()
    {
        string suffix = "@example.com";
        string maximum = new string('a', 70 - suffix.Length) + suffix;
        Assert.Equal(FieldValidationError.None, RegistrationRules.ValidateEmail(maximum));
        Assert.Equal(FieldValidationError.TooLong, RegistrationRules.ValidateEmail("a" + maximum));
    }

    [Theory]
    [InlineData("Clave2026!", FieldValidationError.None)]
    [InlineData("Clave2026#", FieldValidationError.None)]
    [InlineData("Clave2026ñ", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("clave2026!", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Claveabc!", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave1!", FieldValidationError.TooShort)]
    [InlineData("", FieldValidationError.Required)]
    [InlineData("Clave2026<", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave2026;", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave2026'", FieldValidationError.PasswordRequirementsNotMet)]
    [InlineData("Clave2026!\\", FieldValidationError.PasswordRequirementsNotMet)]
    public void ValidatePassword_Requirements_ReturnsExpected(string value, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidatePassword(value));
    }

    [Fact]
    public void ValidatePassword_Boundaries_EnforcesMaximum()
    {
        string validMaximum = "A1!" + new string('b', 17);
        Assert.Equal(FieldValidationError.None, RegistrationRules.ValidatePassword(validMaximum));
        Assert.Equal(FieldValidationError.TooLong, RegistrationRules.ValidatePassword(validMaximum + "b"));
    }

    [Theory]
    [InlineData("Clave2026!", "Clave2026!", FieldValidationError.None)]
    [InlineData("Clave2026!", "clave2026!", FieldValidationError.PasswordsDoNotMatch)]
    [InlineData("Clave2026!", "", FieldValidationError.Required)]
    public void ValidateConfirmation_Comparisons_ReturnsExpected(
        string password, string confirmation, FieldValidationError expected)
    {
        Assert.Equal(expected, RegistrationRules.ValidateConfirmation(password, confirmation));
    }
}
