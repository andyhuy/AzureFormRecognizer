namespace TLPDFGenerator.Model
{
    public class PdfModel
    {
        public string Logo { set; get; }

        public string LogoName { set; get; }

        public string PaymentDetails { set; get; }

        public string Message { set; get; }

        public string Currency { set; get; }

        public Order Order { set; get; }

        public bool ActiveTax { set; get; }

        public InvoiceInfo InvoiceInfo { set; get; }
    }

    public class InvoiceInfo
    {
        public string InvoiceNumber { set; get; }

        public DateTime DueDate { set; get; }

        public string Address { set; get; }

        public string Email { set; get; }

        public string PhoneNumber { set; get; }

        public string PayLink { set; get; }

        public string Name { set; get; }
    }

    public class Order
    {
        public Order()
        {
            Lines = new List<Line>();
        }

        public decimal Subtotal { set; get; }

        public decimal TaxPercent { set; get; }

        public decimal Tax { set; get; }

        public decimal Total { set; get; }

        public decimal AmountDue { set; get; }

        public List<Line> Lines { set; get; }
    }

    public class Line
    {
        public string Description { set; get; }

        public float Qty { set; get; }

        public decimal Tax { set; get; }

        public decimal Amount { set; get; }

        public decimal UnitPrice { set; get; }
    }
}