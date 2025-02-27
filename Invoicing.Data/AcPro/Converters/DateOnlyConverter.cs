using CsvHelper.Configuration;
using CsvHelper;
using CsvHelper.TypeConversion;

namespace Invoicing.Data.AcPro.Converters
{
	public  class DateOnlyConverter : DefaultTypeConverter
	{
		public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
		{
			return DateOnly.Parse(text);
		}
	}
}
