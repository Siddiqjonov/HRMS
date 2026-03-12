namespace HrManager.Domain.Constants;

public static class Policies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireHrManager = "RequireHrManager";
    public const string RequireEmployee = "RequireEmployee";
    public const string RequireAdminOrHrManager = "RequireAdminOrHrManager";
    public const string RequireEmployeeOrHrManager = "RequireEmployeeOrHrManager";
}
