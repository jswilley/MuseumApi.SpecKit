using Microsoft.AspNetCore.Authorization;

namespace MuseumApi.Security;

/// <summary>
/// Builder for creating authorization policies based on group membership
/// Supports building policies with multiple group requirements using fluent API
/// </summary>
public class GroupMembershipPolicyBuilder
{
    private List<string> groups;
    private readonly string _securityEnvironment;

    public GroupMembershipPolicyBuilder(IConfiguration configuration)
    {
        groups = new List<string>();
        _securityEnvironment = configuration.GetValue<string>("SecurityEnvironment") ?? "";
    }

    /// <summary>
    /// Requires membership in any role group (Administrators or Users)
    /// </summary>
    public GroupMembershipPolicyBuilder InAnyRoleGroup()
    {
        groups = new List<string>
        {
            RoleGroup.Administrators + _securityEnvironment,
            RoleGroup.Users + _securityEnvironment,
        };
        return this;
    }

    /// <summary>
    /// Requires membership in a specific group
    /// </summary>
    public GroupMembershipPolicyBuilder MemberOf(string group)
    {
        groups = new List<string> { group + _securityEnvironment };
        return this;
    }

    /// <summary>
    /// Adds an alternative group (OR condition)
    /// </summary>
    public GroupMembershipPolicyBuilder Or(string group)
    {
        groups.Add(group + _securityEnvironment);
        return this;
    }

    /// <summary>
    /// Removes a group from the requirements (exclusion)
    /// </summary>
    public GroupMembershipPolicyBuilder Except(string group)
    {
        groups.Remove(group + _securityEnvironment);
        return this;
    }

    /// <summary>
    /// Builds the authorization policy
    /// </summary>
    public AuthorizationPolicy Build()
    {
        return new AuthorizationPolicyBuilder()
            .RequireRole(groups.OrderBy(a => a).Distinct())
            .RequireAuthenticatedUser()
            .Build();
    }
}
