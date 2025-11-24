namespace Domain;

public class Employee
{
    public Guid EmployeeId { get; set; }
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string? Country { get; set; }
    public DateTime? Birthday { get; set; }
    public string? Address { get; set; }

    public ICollection<Transfer> ReviewedTransfers { get; set; } = new List<Transfer>();
}