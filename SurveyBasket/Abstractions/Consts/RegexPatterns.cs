namespace SurveyBasket.Abstractions.Consts;

public static class RegexPatterns
{
    public const string PasswordPattern = "(?=(.*[0-9]))(?=.*[\\!@#$%^&*()\\\\[\\]{}\\-_+=~`|:;\"'<>,./?])(?=.*[a-z])(?=(.*[A-Z]))(?=(.*)).{8,}";
}
