using AzureFormRecognizer.Web.Models;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult Index()
        {
            return View(sampleDatas);
        }
    }
}
