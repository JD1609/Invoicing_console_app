using Invoicing.Interface.Billings;
using Invoicing.Interface.Files;
using Invoicing.Service.AcPro;
using Invoicing.Service.Files;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Invoicing
{
	internal class Program
	{
		private const bool hasHeaderRecord = true;
		private const int billingHourPrice = 1234;

		static void Main(string[] args)
		{
			try
			{
				IHost _host = Host
					.CreateDefaultBuilder(args)
					.ConfigureServices(SetupDependencyInjection)
					.Build();


				var service = _host.Services.GetRequiredService<IBillingService>();
				service.PrintResult();
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(ex.Message);
				Console.ResetColor();
			}

			Console.WriteLine("\n\nPress ENTER to exit...");
			Console.ReadKey();
		}

		private static void SetupDependencyInjection(IServiceCollection services)
		{
			var fileDestination = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

			services.AddScoped<IFileService, FileService>();
			services.AddScoped<IBillingService>(sp => new AcProService(sp.GetRequiredService<IFileService>(), billingHourPrice, fileDestination, hasHeaderRecord));
		}
	}
}
