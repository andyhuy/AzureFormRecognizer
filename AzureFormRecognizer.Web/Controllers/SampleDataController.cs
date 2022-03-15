using AzureFormRecognizer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using TLPDFGenerator.Model;

namespace AzureFormRecognizer.Web.Controllers
{
    public class SampleDataController : Controller
    {
        List<SampleDataModel> sampleDatas = new List<SampleDataModel>()
        {
            new SampleDataModel(){Url="bills/bill1.PNG",Name="bill1",Type="Image/File"},
            new SampleDataModel(){Url="bills/bill2.PNG",Name="bill2",Type="Image/File"},
            new SampleDataModel(){Url="https://templates.invoicehome.com/invoice-template-us-classic-white-750px.png",Name="invoice-template-us-classic-white",Type="Image/Link"},
            new SampleDataModel(){Url="bills/form-recognizer-demo-sample91.pdf",Name="form-recognizer-demo-sample91",Type="PDF/File"},
            new SampleDataModel(){Url="bills/recognizer-demo-sample6.pdf",Name="recognizer-demo-sample6",Type="PDF/File"},
            new SampleDataModel(){Url="https://slicedinvoices.com/pdf/wordpress-pdf-invoice-plugin-sample.pdf",Name="recognizer-demo-sample6",Type="PDF/Link"},
            
        };
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SampleDataController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View(sampleDatas);
        }

        [HttpGet("pdf")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var client = new HttpClient();
                client.BaseAddress = new Uri("https://tlpdfgenerator.azurewebsites.net/");
                var model = await BuildTestModel();
                var json = JsonConvert.SerializeObject(model);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var pdf = await client.PostAsync("/api/pdf", data);
                if (pdf != null && pdf.IsSuccessStatusCode)
                {
                    var content = await pdf.Content.ReadAsByteArrayAsync();
                    return File(content, "application/octet-stream", $"Invoice_{DateTime.Now}.pdf");
                }

            }
            catch (Exception ex)
            {

            }

            return new BadRequestResult();
        }

        private async Task<PdfModel> BuildTestModel()
        {
            var request = new PdfModel();
            request.InvoiceInfo = new InvoiceInfo()
            {
                InvoiceNumber = "49F76324-0003",
                DueDate = DateTime.Now.AddDays(7),
                Address = "180 City Road, Southbank VIC 3006",
                Name = "Michael Kambouris",
                Email = "michaelkambo1982@gmail.com",
                PhoneNumber = "0416 657 072",
                PayLink = "https://www.google.com"
            };

            request.Order = new Order()
            {
                Subtotal = 149,
                TaxPercent = 10,
                Tax = decimal.Parse("14.90"),
                Total = decimal.Parse("163.90"),
                AmountDue = decimal.Parse("163.90"),
                Lines = new List<Line>()
                {
                    new Line(){Description = "Preparation and lodgements of 2017 Income Tax Return",Qty =1 ,UnitPrice = 149,Tax= 10,Amount= 149}
                }
            };
           
            string logoName = "Tax-Logo.png";
            string urlLogo = Path.Combine(_webHostEnvironment.WebRootPath, $"Logo/{logoName}");
            var logo = await System.IO.File.ReadAllBytesAsync(urlLogo);
            request.Logo = Convert.ToBase64String(logo);
            request.LogoName = logoName;
            request.ActiveTax = true;
            request.PaymentDetails = "BSB: 083-195 Account: 67 561 6057";
            request.Message = "Thank you for your business";
            request.Currency = "$";
            return request;
        }
    }
}
