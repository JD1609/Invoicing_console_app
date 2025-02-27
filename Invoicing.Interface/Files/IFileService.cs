using CsvHelper.Configuration;

namespace Invoicing.Interface.Files
{
	public interface IFileService
	{
		List<TData> ReadFile<TData>(string filePath, bool hasHeaderRecord = false);
		public List<TData> ReadFile<TData, TMap>(string filePath, bool hasHeaderRecord = false) where TMap : ClassMap;

	}
}
