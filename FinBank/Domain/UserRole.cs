namespace Domain;

public static class UserRole
{
    public const string Admin = "Admin";
    public const string Banker = "Banker";
    public const string Customer = "Customer";
    public const string AllRoles = Admin + "," + Banker + "," + Customer;

}
