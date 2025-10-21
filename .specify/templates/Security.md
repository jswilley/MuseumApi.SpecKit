## **Policy Creation**
There are 2 group policies that need to be created User and Administrator.
The code for creating these policies would be similar to below:

### **Policy Builds**
in the Security folder something to define the policies in a class
 public static class Policies
 {
     private static IConfiguration configuration;
     public static void InitPolicies(IConfiguration _configuration)
     {
         configuration = _configuration;
     }

     public const string IsAdmin = "IsAdmin";
     public const string IsUser = "IsUser";

     public static AuthorizationPolicy AdminPolicy()
     {
         return new GroupMembershipPolicyBuilder(configuration)
             .MemberOf(RoleGroup.Administrators)                
             .Build();
     }

     public static AuthorizationPolicy UserPolicy()
     {
         return new GroupMembershipPolicyBuilder(configuration)
             .InAnyRoleGroup()
             .Build();
     }
 }

 in the security folder something to set roles.  The ones in quotes are the AD roles/Entra roles
   public static class RoleGroup
  {
      public const string Administrators = "MuseumAdmin";
      public const string Users = "MuseumUser";
      
  }

  in the security folder somthing to tie things together
  using Microsoft.AspNetCore.Authorization;


    public class GroupMembershipPolicyBuilder
    {
        private List<string> groups;
        private string _securityEnvironment { get; }
        public GroupMembershipPolicyBuilder(IConfiguration configuration)
        {
            groups = new List<string>();
            _securityEnvironment = configuration.GetValue<string>("SecurityEnvironment");
        }

        public GroupMembershipPolicyBuilder InAnyRoleGroup()
        {
            groups = new List<string>
            {
                RoleGroup.Administrators + _securityEnvironment,
                RoleGroup.Users + _securityEnvironment,
            };
            return this;
        }
        public GroupMembershipPolicyBuilder MemberOf(string group)
        {
            groups = new List<string> { group + _securityEnvironment };
            return this;
        }
        public GroupMembershipPolicyBuilder Or(string group)
        {
            groups.Add(group + _securityEnvironment);
            return this;
        }
        public GroupMembershipPolicyBuilder Except(string group)
        {
            groups.Remove(group + _securityEnvironment);
            return this;
        }
        public AuthorizationPolicy Build()
        {
            return new AuthorizationPolicyBuilder().RequireRole(groups.OrderBy(a => a).Distinct()).RequireAuthenticatedUser().Build();
        }
    }

### **Policy Registration**
in the program.cs file code to register the policies.
static void ConfigurePolicies(WebApplicationBuilder builder, IConfiguration configuration)
{
    HttpClientPolicyConfiguration policyConfigs = new();
        Policies.InitPolicies(configuration);

        builder.Services.AddAuthorization(config =>
        {
            config.AddPolicy(Policies.IsAdmin, Policies.AdminPolicy());
            config.DefaultPolicy = Policies.UserPolicy();

            // When no policy is specified.
            config.FallbackPolicy = Policies.UserPolicy();
        });
    
}


## **CORS Policy**
this will need to go in program.cs.  The code below doesn't restrict access.

builder.Services.AddCors(options => {
    options.AddPolicy("CorsPolicy", builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});
