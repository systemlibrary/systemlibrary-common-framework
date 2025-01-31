namespace SystemLibrary.Common.Framework;

public class Invoice
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsPaid { get; set; }
    public double Price { get; set; }
    public decimal Amount { get; set; }
    public long BankAccountNumber { get; set; }
    public Invoice LinkedInvoice { get; set; }
    public DateTime DateIssued { get; set; }
}