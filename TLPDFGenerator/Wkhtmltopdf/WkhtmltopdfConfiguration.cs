using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Wkhtmltopdf.NetCore
{
    public static class WkhtmltopdfConfiguration
    {
        public static string RotativaPath { get; set; }

        public static IServiceCollection AddWkhtmltopdf(this IServiceCollection services, string wkhtmltopdfRelativePath = "App")
        {
            RotativaPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, wkhtmltopdfRelativePath);

            if (!Directory.Exists(RotativaPath))
            {
                throw new Exception("Folder containing wkhtmltopdf not found, searched for " + RotativaPath);
            }

            services.TryAddTransient<IGeneratePdf, GeneratePdf>();

            return services;
        }
    }
}