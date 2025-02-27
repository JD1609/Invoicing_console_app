using Invoicing.Common.Extensions.Strings;
using Invoicing.Common.Utils;
using Invoicing.Data.AcPro;
using Invoicing.Data.Common;
using Invoicing.Interface.Billings;
using Invoicing.Interface.Files;
using System.Text;
using System.Text.RegularExpressions;

namespace Invoicing.Service.AcPro
{
	public class AcProService : IBillingService
	{
		private const string billingFilePattern = @"Billing\+Report\+(?<Month>leden|únor|březen|duben|květen|červen|červenec|srpen|září|říjen|listopad|prosinec)\+(?<Year>\d{4})\+(?<FirstName>[A-Za-zá-žÁ-Ž]+)\+(?<LastName>[A-Za-zá-žÁ-Ž]+).*\.csv$";
		private const string billingFileExtension = ".csv";

		private readonly IFileService _fileService;
		private readonly int _billingHourPrice;
		private readonly string _fileDestination;
		private readonly bool _hasHeaderRecord;

		private List<PrintMessageDefinition> prePrintMessages = [];
		private List<PrintMessageDefinition> postPrintMessages = [];

		#region Ctor
		public AcProService(IFileService fileService)
        {
			_fileService = fileService;
			_billingHourPrice = 0;
			_fileDestination = string.Empty;
			_hasHeaderRecord = false;
		}

		public AcProService(IFileService fileService, int billingHourPrice, string fileDestination, bool hasHeaderRecord = false)
		{
			_fileService = fileService;
			_billingHourPrice = billingHourPrice;
			_fileDestination = fileDestination;
			_hasHeaderRecord = hasHeaderRecord;
		}
		#endregion Ctor


		public string FindBillingFile()
		{
			Console.WriteLine("Finding billing file...");
			var regex = new Regex(billingFilePattern);

			var files = Directory
				.GetFiles(_fileDestination, $"*{billingFileExtension}")
				.Select(file =>
				{
					var fileName = Path.GetFileName(file);
					var match = regex.Match(fileName);
					if (match.Success)
					{
						return new
						{
							File = file,
							Year = int.Parse(match.Groups["Year"].Value),
							Month = GetMonthIndex(match.Groups["Month"].Value),
							CreationDate = File.GetCreationTime(file),
						};
					}

					return null;
				})
				.Where(f => f != null)
				.OrderByDescending(f => f!.Year)
				.ThenByDescending(f => f!.Month)
				.ThenByDescending(f => f!.CreationDate)
				.ToList();

			if (files.Count == 0)
			{
				throw new FileNotFoundException(GetFileNotFoundExceptionMessage());
			}
			else if (files.Count > 1)
			{
				prePrintMessages.AddRange(
				[
					new() { Message = "More than 1 file was found - the newest one was used", Color = ConsoleColor.DarkYellow, },
					new() { Message = $"Files found [{_fileDestination}]:", Color = ConsoleColor.DarkYellow },
					new() { Message = string.Join("\n", files.Select(f => $"\t- [{f!.CreationDate}] {Path.GetFileName(f!.File)}")), Color = ConsoleColor.DarkYellow },
				]);
			}


			return files.First()!.File;
		}

		public List<ProjectBillingHours> GetProjectsHours(string filePath, bool hasHeaderRecord = false)
		{
			Console.WriteLine("Loading file...");
			var result = new List<ProjectBillingHours>();

			var data = _fileService.ReadFile<BillingRecord, BillingRecordCsvMap>(filePath, hasHeaderRecord);
			var projectHours = data.GroupBy(d => d.Project);

            foreach (var project in projectHours)
            {
				result.Add(new() 
				{ 
					Project = project.Key ?? "<Unknown>", 
					BillingHours = project.Sum(r => r.Hours),
				});
			}

			return result;
        }

		public void PrintResult(int itemsSpaces = 15)
		{
			var filePath = FindBillingFile();
			var billingDetails = GetBillingDetails(filePath);
			var projectsHours = GetProjectsHours(filePath, _hasHeaderRecord);
			var resultRows = PrepareResult(projectsHours, billingDetails, itemsSpaces);

			PrintResult(resultRows);
		}

		private void PrintResult(List<string> resultRows) 
		{
			Console.Clear();

			// Pre-print messages
			if (prePrintMessages.Any())
			{
				foreach (var preMsg in prePrintMessages)
				{
					if (preMsg.Color.HasValue)
						Console.ForegroundColor = preMsg.Color.Value;

					Console.WriteLine(preMsg.Message);
					Console.ResetColor();
				}

				Console.WriteLine($"\n\n");
			}


			// Result
			foreach (var row in resultRows)
			{
				Console.WriteLine(row);
			}


			// Post-print messages
			if (postPrintMessages.Any())
			{
				Console.WriteLine($"\n\n");

				foreach (var postMsg in postPrintMessages)
				{
					if (postMsg.Color.HasValue)
						Console.ForegroundColor = postMsg.Color.Value;

					Console.WriteLine(postMsg.Message);
					Console.ResetColor();
				}
			}
		}

		private List<string> PrepareResult(List<ProjectBillingHours> projectsHours, BillingDetail billingDetail, int itemsSpaces)
		{
			Console.WriteLine("Preparing result...");
			var result = new List<string>();

			var maxLen = projectsHours.Max(ph => ph.Project.Length);
			var equalSpacer = StringExtensions.GetStringXTimes("=", maxLen + itemsSpaces + 30);

			var totalHours = projectsHours.Sum(t => t.BillingHours);
			var totalProfit = totalHours * _billingHourPrice;
			var totalWorkingDays = DateTimeUtils.GetWorkingDaysInCurrentMonth();
			var totalWorkingHours = DateTimeUtils.GetWorkingHoursInCurrentMonth();

			result.AddRange(
			[
				$"Billing for: {billingDetail.FirstName} {billingDetail.LastName}",
				$"Billing date: {billingDetail.Month} {billingDetail.Year}",
				$"Billing hour price: {_billingHourPrice}",
				Environment.NewLine,
				$"Total working hours in month: {totalWorkingHours} ({totalWorkingDays} days)",
				$"Expected earnings: {totalWorkingHours * _billingHourPrice}",
				Environment.NewLine,
				StringExtensions.FormatValuesRow(maxLen, itemsSpaces, "Project", "Hours", "Price"),
				equalSpacer,
			]);

			result.AddRange(projectsHours
				.Select(project => StringExtensions.FormatValuesRow(maxLen, itemsSpaces, project.Project, $"{project.BillingHours}", $"{project.BillingHours * _billingHourPrice}"))
			);

			result.AddRange(
			[
				Environment.NewLine,
				equalSpacer,
				StringExtensions.FormatValuesRow(maxLen, itemsSpaces, "Total", $"{totalHours}", $"{totalProfit}"),
			]);


			return result;
		}

		private BillingDetail GetBillingDetails(string fileName)
		{
			Match match = Regex.Match(fileName, billingFilePattern);
			if (match.Success)
			{
				var month = match.Groups["Month"].Value;
				var year = match.Groups["Year"].Value;

				var firstName = match.Groups["FirstName"].Value;
				var lastName = match.Groups["LastName"].Value;

				return new()
				{
					Month = month,
					Year = year,
					FirstName = firstName,
					LastName = lastName,
				};
			}

			return new();
		}

		private int GetMonthIndex(string monthName)
		{
			var months = new Dictionary<string, int>
			{
				{ "leden", 1 },
				{ "únor", 2 },
				{ "březen", 3 },
				{ "duben", 4 },
				{ "květen", 5 },
				{ "červen", 6 },
				{ "červenec", 7 },
				{ "srpen", 8 },
				{ "září", 9 },
				{ "říjen", 10 },
				{ "listopad", 11 },
				{ "prosinec", 12 }
			};

			return months.TryGetValue(monthName, out int value) ? value : 1;
		}

		private string GetFileNotFoundExceptionMessage()
		{
			var sb = new StringBuilder();
			sb.AppendLine("Billing file not found");
			sb.AppendLine("=======================");

			sb.AppendLine($"Searched destination:");
			sb.AppendLine($"\t- {_fileDestination}");

			sb.AppendLine();

			sb.AppendLine($"Searched pattern:");
			sb.AppendLine($"\t- {billingFilePattern}");

			sb.AppendLine();

			sb.AppendLine($"Searched extension:");
			sb.AppendLine($"\t- {billingFileExtension}");

			return sb.ToString();
		}
	}
}
