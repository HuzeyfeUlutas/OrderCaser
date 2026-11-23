namespace OrderCase.Ordering.Api.Authentication;

public sealed class AuthenticationSettings
{
    public const string SectionName = "Authentication";

    public List<AuthenticationUser> Users { get; init; } = [];
}

public sealed class AuthenticationUser
{
    public string UserId { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}