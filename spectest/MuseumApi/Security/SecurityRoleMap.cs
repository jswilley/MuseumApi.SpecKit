namespace MuseumApi.Security;

/// <summary>
/// Maps application roles to Active Directory groups or authentication providers
/// </summary>
public static class RoleGroup
{
    public const string Administrators = "MuseumAdmin";
    public const string Users = "MuseumUser";
}
