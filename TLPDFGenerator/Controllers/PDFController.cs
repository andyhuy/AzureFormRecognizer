using Microsoft.AspNetCore.Mvc;
using TLPDFGenerator.Model;
using Wkhtmltopdf.NetCore;

namespace TLPDFGenerator.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PDFController : Controller
    {
        private readonly ILogger<PDFController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IGeneratePdf _generatePdf;

        public PDFController(ILogger<PDFController> logger, IWebHostEnvironment webHostEnvironment, IGeneratePdf generatePdf)
        {
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _generatePdf = generatePdf;
        }

        [HttpPost(Name = "invoice")]
        public async Task<IActionResult> Invoice(PdfModel request)
        {
            try
            {
                if (request != null)
                {
                    var currentDirectory = Directory.GetCurrentDirectory();
                    string uploadsFolder = Path.Combine(currentDirectory, "wwwroot/Templates");
                    string uniqueFileName = "InvoiceTemplate.html";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        string htmlContent = System.IO.File.ReadAllText(filePath);
                        string template = await PDFBulider.BuildTemplate(htmlContent, currentDirectory, request, _webHostEnvironment);
                        var pdf = _generatePdf.GetPDF(template);
                        return File(pdf, "application/octet-stream", $"Invoice_{DateTime.Now}.pdf");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex.InnerException?.Message ?? "----");
            }

            return new BadRequestResult();
        }
    }
}