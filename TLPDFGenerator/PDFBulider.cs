using System.Text;
using TLPDFGenerator.Model;

namespace TLPDFGenerator
{
    public static class PDFBulider
    {
        public static async Task<string> BuildTemplate(string htmlContent, string currentDirectory, PdfModel model, IWebHostEnvironment webHostEnvironment)
        {
            var t1 = BuildInvoiceInfo(model);
            var t2 = BuildLogo(currentDirectory, model, webHostEnvironment);
            var t3 = BuildOrderLineHeader(model);
            var t4 = BuildOrderLine(model);
            var t5 = BuildOrder(model);
            await Task.WhenAll(t1, t2, t3, t4, t5);
            htmlContent = htmlContent.Replace("{{InvoiceInfo}}", t1.Result)
                .Replace("{{Logo}}", t2.Result)
                .Replace("{{OrderLineHeader}}", t3.Result)
                .Replace("{{OrderLine}}", t4.Result)
                .Replace("{{Order}}", t5.Result)
                .Replace("{{Currency}}", model.Currency);
            return await Task.FromResult(htmlContent);
        }

        private static async Task<string> BuildInvoiceInfo(PdfModel model)
        {
            string invoiceInfo = @"Invoice number {{InvoiceNumber}}<br />
        Date of issue {{CurrentDate}}<br />
        Date due {{DueDate}}<br /><br />

        Rideshare Tax Pty Ltd<br />
        ABN: 25 586 989 458<br />
        180 City Road, Southbank VIC 3006<br />
        accounts@ridesharetax.com.au<br />
        1300 743 382<br /><br />

        Bill to<br />
        {{Name}}<br />
        {{Address}}<br />
        {{Email}}<br />
        {{PhoneNumber}}<br /><br />

        <a href=""{{PayLink}}"">{{Currency}}{{AmountDue}} due {{DueDate}}</a><br />";

            if (model?.InvoiceInfo != null)
            {
                invoiceInfo = invoiceInfo.Replace("{{InvoiceNumber}}", model.InvoiceInfo.InvoiceNumber)
                    .Replace("{{CurrentDate}}", DateTime.Now.ToShortDateString())
                    .Replace("{{Name}}", model.InvoiceInfo.Name)
                    .Replace("{{DueDate}}", model.InvoiceInfo.DueDate.ToShortDateString())
                    .Replace("{{Address}}", model.InvoiceInfo.InvoiceNumber)
                    .Replace("{{Email}}", model.InvoiceInfo.Email)
                    .Replace("{{PhoneNumber}}", model.InvoiceInfo.PhoneNumber)
                    .Replace("{{PayLink}}", model.InvoiceInfo.PayLink)
                    .Replace("{{AmountDue}}", model.Order.AmountDue.ToString());
                return await Task.FromResult(invoiceInfo);
            }
            return string.Empty;
        }

        private static async Task<string> BuildLogo(string currentDirectory, PdfModel model, IWebHostEnvironment webHostEnvironment)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Logo)) return string.Empty;
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "Logo");
                string filePath = Path.Combine(uploadsFolder, $"{Guid.NewGuid()}_{model.LogoName}");
                await File.WriteAllBytesAsync(filePath, Convert.FromBase64String(model.Logo));
                string logo = @"<img src=""{{LogoUrl}}"" alt=""Logo"" />";
                logo = logo.Replace("{{LogoUrl}}", filePath);
                return await Task.FromResult(logo);

            }
            catch(Exception ex)
            {

            }
            return string.Empty;
            
        }

        private static async Task<string> BuildOrderLineHeader(PdfModel model)
        {
            string orderLineHeader = string.Empty;
            if (model.ActiveTax)
            {
                orderLineHeader = @" <th>Description</th>
            <th>Qty</th>
            <th>Unit price</th>
            <th>Tax</th>
            <th>Amount</th>";
            }
            else
            {
                orderLineHeader = @" <th>Description</th>
            <th>Qty</th>
            <th>Unit price</th>
            <th>Amount</th>";
            }
            return await Task.FromResult(orderLineHeader);
        }

        private static async Task<string> BuildOrderLine(PdfModel model)
        {
            if (model?.Order?.Lines != null && model.Order.Lines.Any())
            {
                var orderLine = new StringBuilder();
                string line = string.Empty;
                if (model.ActiveTax)
                {
                    line = @"<td>{{Description}}
            </td>
            <td>{{Qty}}</td>
            <td>{{Currency}}{{UnitPrice}}</td>
            <td>{{Tax}}%</td>
            <td>{{Currency}}{{Amount}}</td>";
                }
                else
                {
                    line = @"<td>{{Description}}
            </td>
            <td>{{Qty}}</td>
            <td>{{Currency}}{{UnitPrice}}</td>
            <td>{{Currency}}{{Amount}}</td>";
                }

                foreach (var item in model.Order.Lines)
                {
                    string data = string.Empty;
                    if (model.ActiveTax)
                    {
                        data = line.Replace("{{Description}}", item.Description)
                            .Replace("{{Qty}}", item.Qty.ToString())
                            .Replace("{{UnitPrice}}", item.UnitPrice.ToString())
                            .Replace("{{Tax}}", item.Tax.ToString())
                            .Replace("{{Amount}}", item.Amount.ToString());
                    }
                    else
                    {
                        data = line.Replace("{{Description}}", item.Description)
                           .Replace("{{Qty}}", item.Qty.ToString())
                           .Replace("{{UnitPrice}}", item.UnitPrice.ToString())
                           .Replace("{{Amount}}", item.Amount.ToString());
                    }
                    orderLine.AppendLine(data);
                }
                return await Task.FromResult(orderLine.ToString());
            }
            return string.Empty;
        }

        private static async Task<string> BuildOrder(PdfModel model)
        {
            var order = new StringBuilder();
            string subtotal = @"
        <tr>
            <td colspan=""2"" align=""right"">Subtotal : </td>
            <td>{{Currency}}{{Subtotal}}</td>
        </tr>
        ";
            string tax = @"<tr>
            <td colspan=""2"" align=""right"">Tax ({{TaxPercent}}%) : </td>
            <td>{{Currency}}{{Tax}}</td>
        </tr>";

            string total = @"<tr>
            <td colspan=""2"" align=""right"">Total :</td>
            <td>{{Currency}}{{Total}}</td>
        </tr>
        <tr>
            <td colspan=""2"" align=""right"">Amount Due :</td>
            <td>{{Currency}}{{AmountDue}}</td>
        </tr>
        <tr>
            <td colspan=""2"" align=""right"">PAYMENT DETAILS :</td>
            <td>{{PaymentDetails}}</td>
        </tr>
        <tr>
            <td colspan=""2"" align=""right"">MESSAGE :</td>
            <td>{{Message}}</td>
        </tr>";

            subtotal = subtotal.Replace("{{Subtotal}}", model.Order.Subtotal.ToString());

            total = total.Replace("{{Total}}", model.Order.Total.ToString())
                .Replace("{{AmountDue}}", model.Order.AmountDue.ToString())
                .Replace("{{PaymentDetails}}", model.PaymentDetails)
                .Replace("{{Message}}", model.Message);
            order.AppendLine(subtotal);
            order.AppendLine(total);
            if (model.ActiveTax)
            {
                tax = tax.Replace("{{TaxPercent}}", model.Order.TaxPercent.ToString())
                    .Replace("{{Tax}}", model.Order.Tax.ToString());
                order.AppendLine(tax);
            }
            return await Task.FromResult(order.ToString());
        }
    }
}