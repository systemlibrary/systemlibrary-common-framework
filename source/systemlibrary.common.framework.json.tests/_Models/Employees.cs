namespace SystemLibrary.Common.Framework;

public class Employees
{
    public List<Employee> EmployeeList { get; set; }
    public IEnumerable<Employee> EmployeeIEnumerable { get; set; }
    public Employee[] EmployeeArray { get; set; }

    List<Employee> EmployeeListPrivate { get; set; }
    IEnumerable<Employee> EmployeeIEnumerablePrivate { get; set; }
    Employee[] EmployeeArrayPrivate { get; set; }

    public List<Employee> EmployeeListField;
    public IEnumerable<Employee> EmployeeIEnumerableField;
    public Employee[] EmployeeArrayField;

    List<Employee> EmployeeListPrivateField;
    IEnumerable<Employee> EmployeeIEnumerablePrivateField;
    Employee[] EmployeeArrayPrivateField;
}
