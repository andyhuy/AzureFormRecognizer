namespace Wkhtmltopdf.NetCore
{
    public interface IGeneratePdf
    {
        byte[] GetPDF(string html);
    }
}