using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace Invoicing.Data.AcPro.Converters
{
	public class DecimalConverter : DefaultTypeConverter
	{
		public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
		{
			return decimal.Parse(text);
		}
	}
}
