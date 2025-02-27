using Invoicing.Interface.Files;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace Invoicing.Service.Files
{
	public class FileService : IFileService
	{
		#region Ctor
		public FileService()
		{
		}
		#endregion Ctor

		public List<T> ReadFile<T>(string filePath, bool hasHeaderRecord = false)
		{
			if (!File.Exists(filePath))
				throw new Exception($"File not exists '{filePath}'");

			var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = hasHeaderRecord,
			};

			using (var reader = new StreamReader(filePath))
			using (var csv = new CsvReader(reader, configuration))
			{
				var records = csv.GetRecords<T>();
				return records.ToList();
			}
		}

		public List<TData> ReadFile<TData, TMap>(string filePath, bool hasHeaderRecord = false) where TMap : ClassMap
		{
			if (!File.Exists(filePath))
				throw new Exception($"File not exists '{filePath}'");

			string? header = null;

			var configuration = new CsvConfiguration(CultureInfo.InvariantCulture)
			{
				HasHeaderRecord = hasHeaderRecord,
				ShouldSkipRecord = (args) => 
				{
					var row = args.Row;

					if (hasHeaderRecord)
					{
						if (row.Parser.Row == 1)
						{
							header = row.Parser.RawRecord;
							return false;
						}

						return args.Row.Parser.RawRecord.Trim() == header?.Trim();
					}

					return false;
				},
			};

			using (var reader = new StreamReader(filePath))
			using (var csv = new CsvReader(reader, configuration))
			{
				csv.Context.RegisterClassMap<TMap>();

				var records = csv.GetRecords<TData>();
				return records.ToList();
			}
		}
	}
}
