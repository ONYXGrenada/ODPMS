namespace ODPMS.Models;

public class Settings
{
    public CompanySettings CompanySettings { get; set; } = new CompanySettings();
    public TicketSettings TicketSettings { get; set; } = new TicketSettings();
    public ReceiptSettings ReceiptSettings { get; set; } = new ReceiptSettings();
}

public class CompanySettings
{
    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string CompanyEmail { get; set; }
    public string CompanyPhone { get; set; }
    public string CompanyLogo { get; set; }
}

public class TicketSettings
{
    public string TicketMessage { get; set; }
    public string TicketDisclaimer { get; set; }
}

public class ReceiptSettings
{
    public string PrinterCOMPort { get; set; }
    public bool DefaultPrintReceipt { get; set; }
    public string ReceiptMessage { get; set; }
    public string ReceiptDisclaimer { get; set; }
}
