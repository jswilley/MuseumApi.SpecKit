using Microsoft.AspNetCore.Authorization;

namespace MuseumApi.Security;

/// <summary>
/// Defines authorization policies for the Museum API
/// </summary>
public static class Policies
{
    private static IConfiguration? configuration;

    /// <summary>
    /// Initialize policies with configuration
    /// Must be called during application startup
    /// </summary>
    public static void InitPolicies(IConfiguration _configuration)
    {
        configuration = _configuration;
    }

    public const string IsAdmin = "IsAdmin";
    public const string IsUser = "IsUser";

    /// <summary>
    /// Policy for administrative operations
    /// Requires membership in Administrators group
    /// </summary>
    public static AuthorizationPolicy AdminPolicy()
    {
        if (configuration == null)
            throw new InvalidOperationException("Policies must be initialized with InitPolicies() before use");

        return new GroupMembershipPolicyBuilder(configuration)
            .MemberOf(RoleGroup.Administrators)
            .Build();
    }

    /// <summary>
    /// Policy for standard user operations
    /// Requires membership in any role group (Users or Administrators)
    /// </summary>
    public static AuthorizationPolicy UserPolicy()
    {
        if (configuration == null)
            throw new InvalidOperationException("Policies must be initialized with InitPolicies() before use");

        return new GroupMembershipPolicyBuilder(configuration)
            .InAnyRoleGroup()
            .Build();
    }
}
