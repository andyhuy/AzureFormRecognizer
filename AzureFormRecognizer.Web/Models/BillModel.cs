namespace AzureFormRecognizer.Web.Models
{
    using System.ComponentModel.DataAnnotations;
    public class BillModel : BaseModel
    {
        public string BillUrl { get; set; }
        public List<KeyValuePair<string, string>> RawData { get; set; } = new List<KeyValuePair<string, string>>();
        public string Amount { get; set; }
        public string Date { get; set; }

        public bool GST { get; set; }
    }

    public class BaseModel
    {
        public int AccountCategory { get; set; } = 1;
        public int TransactionType { get; set; } = 1;
    }

    public class BillRequest : BaseModel
    {
        [Required(ErrorMessage = "Please choose bill")]
        [Display(Name = "Bill file")]
        public IFormFile BillFile { get; set; }
    }

    public class AzureSettings
    {
        public FormRecognizer FormRecognizer { get; set; }
    }

    public class FormRecognizer
    {
        public string EndPoint { get; set; }
        public string ApiKey { get; set; }
    }

}
