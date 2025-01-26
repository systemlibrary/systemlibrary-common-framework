using System.Text.Json.Serialization;

namespace SystemLibrary.Common.Framework;

public class Employee
{
    public string FirstName { get; set; }
    [JsonIgnore]
    public string MiddleName { get; set; }
    public Employee Nested { get; set; }
    string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber;
    public DateTime BirthDate { get; set; }
    public DateTime MarriedDate { get; set; }
    public int Age { get; set; }
    public int Height { get; }
    int Width;

    public bool IsHired;
    public bool IsFired { get; set; }
    public List<Invoice> Invoices { get; set; }

    public BackgroundColor favoriteColor { get; set; }

    public static Employee Create(string name = "John")
    {
        Employee employee = new Employee();
        employee.FirstName = name + " nested";
        employee.Nested = new Employee();
        employee.Nested.FirstName = name + " nested2";
        employee.Nested.Nested = new Employee();
        employee.Nested.Nested.FirstName = name + " nested3";
        employee.Nested.Nested.Nested = new Employee();
        employee.Nested.Nested.Nested.FirstName = name + " nested4";

        employee.Nested.Nested.Nested.Nested = new Employee();
        employee.Nested.Nested.Nested.Nested.FirstName = name + " nested5";
        employee.Nested.Nested.Nested.Nested.Nested = new Employee();
        employee.Nested.Nested.Nested.Nested.Nested.FirstName = name + " nested6";

        return new Employee
        {
            FirstName = name,
            MiddleName = "A.",
            LastName = "Doe",
            Nested = employee,
            MarriedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").ToDateTime(),
            Email = name.ToLower() + ".doe@example.com",
            PhoneNumber = "123-456-" + Randomness.Int(1000, 9999),
            BirthDate = new DateTime(Randomness.Int(1990, 2020), 1, 1),
            Age = Randomness.Int(10, 99),
            IsHired = true,
            IsFired = false,
            Invoices = new List<Invoice>
            {
                new Invoice
                {
                    Id = Randomness.Int(10, 99999),
                    Title = "Invoice 001",
                    IsPaid = true,
                    Price = Randomness.Double(10, 99),
                    Amount = 150.75M,
                    BankAccountNumber = 123456789,
                    LinkedInvoice = null,
                    DateIssued = DateTime.Now
                },
                new Invoice
                {
                    Id = Randomness.Int(1, 99999),
                    Title = "Invoice 002",
                    IsPaid = false,
                    Price = Randomness.Double(10, 99),
                    Amount = 200.50M,
                    BankAccountNumber = 987654321,
                    LinkedInvoice = null,
                    DateIssued = DateTime.Now.AddDays(-5)
                }
            }
        };
    }

    public static List<Employee> CreateList(string name = "John")
    {
        var employee1 = Create(name + "1");
        var employee2 = Create(name + "2");

        return new List<Employee> { employee1, employee2 };
    }


}
