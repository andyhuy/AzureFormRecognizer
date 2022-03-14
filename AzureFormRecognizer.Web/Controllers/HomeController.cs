using Azure;
using Azure.AI.FormRecognizer;
using AzureFormRecognizer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AzureFormRecognizer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<AzureSettings> _azureSettings;
        public HomeController(IWebHostEnvironment webHostEnvironment, IOptions<AzureSettings> azureSettings)
        {
            _webHostEnvironment = webHostEnvironment;
            _azureSettings = azureSettings;
        }

        public async Task<IActionResult> Index()
        {

            var model = await GetModelAsync(null);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(BillRequest request)
        {
            var model = await GetModelAsync(request);

            return View(model);
        }

        ///<summary>
        ///https://www.youtube.com/watch?v=gZdJdCZTXYw
        ///https://fott-2-1.azurewebsites.net/
        ///https://docs.microsoft.com/en-us/samples/azure/azure-sdk-for-net/azure-form-recognizer-client-sdk-samples/
        ///https://azuresdkdocs.blob.core.windows.net/$web/dotnet/Azure.AI.FormRecognizer/3.1.0/index.html
        ///https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/formrecognizer/Azure.AI.FormRecognizer/samples/Sample_ModelCompose.md
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<BillModel> GetModelAsync(BillRequest? request)
        {
            BillModel bill = new BillModel();
            try
            {
                string endpoint = _azureSettings.Value.FormRecognizer.EndPoint;
                string apiKey = _azureSettings.Value.FormRecognizer.ApiKey;
                var credential = new AzureKeyCredential(apiKey);
                var client = new FormRecognizerClient(new Uri(endpoint), credential);
                string pathFile = await UploadedFile(request);
                if(request != null)
                {
                    bill.AccountCategory = request.AccountCategory;
                    bill.TransactionType = request.TransactionType;
                }

                using (FileStream stream = new FileStream(pathFile, FileMode.Open))
                {
                    var response = await client.StartRecognizeInvoicesAsync(stream).WaitForCompletionAsync();

                    foreach (var data in response.Value)
                    {
                        if (data.Fields.Any())
                        {
                            foreach (var item in data.Fields)
                            {
                                var key = item.Key;
                                var value = item.Value?.ValueData?.Text ?? string.Empty;
                                bill.RawData.Add(new KeyValuePair<string, string>(key, value));
                                //Console.WriteLine($"{key}-{value}");
                            }
                        }
                    }

                }
                bill.Amount = GetAmount(bill);
                bill.Date = GetDate(bill);
                bill.GST = CheckGST(bill);

            }
            catch (Exception)
            {

            }
            return bill;
        }

        private async Task<string> UploadedFile(BillRequest? model)
        {

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "bills");
            string uniqueFileName = string.Empty;

            if (model?.BillFile != null)
            {
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BillFile.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                if (!System.IO.File.Exists(filePath))
                {
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.BillFile.CopyToAsync(fileStream);
                    }
                }
                return filePath;
            }
            else
            {
                uniqueFileName = "bill1.png";
                return Path.Combine(uploadsFolder, uniqueFileName);
            }
        }
        #region Mapping

        private string GetAmount(BillModel bill)
        {
            if (bill.RawData.Any(x => x.Key.ToLower().Contains("invoicetotal")))
            {
                return bill.RawData.FirstOrDefault(x => x.Key.ToLower().Contains("invoicetotal")).Value;
            }
            else if (bill.RawData.Any(x => x.Key.ToLower().Contains("totalamount")))
            {
                return bill.RawData.FirstOrDefault(x => x.Key.ToLower().Contains("totalamount")).Value;
            }
            else if (bill.RawData.Any(x => x.Key.ToLower().Contains("balancedue")))
            {
                return bill.RawData.FirstOrDefault(x => x.Key.ToLower().Contains("balancedue")).Value;
            }
            else if (bill.RawData.Any(x => x.Key.ToLower().Contains("totaldue")))
            {
                return bill.RawData.FirstOrDefault(x => x.Key.ToLower().Contains("totaldue")).Value;
            }
            return string.Empty;
        }

        private string GetDate(BillModel bill)
        {
            if (bill.RawData.Any(x => x.Key.ToLower().Contains("invoicedate")))
            {
                return bill.RawData.FirstOrDefault(x => x.Key.ToLower().Contains("invoicedate")).Value;
            }
            else if (bill.RawData.Any(x => x.Key.ToLower().Contains("date")))
            {
                return bill.RawData.FirstOrDefault(x => x.Key.ToLower().Contains("date")).Value;
            }
            return string.Empty;
        }

        private bool CheckGST(BillModel bill)
        {
            if (bill.RawData.Any(x => x.Key.ToLower().Contains("tax")))
            {
                return true;
            }
            return false;
        }


        #endregion
    }
}