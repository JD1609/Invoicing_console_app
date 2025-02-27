using CsvHelper.Configuration;
using CsvHelper;
using Invoicing.Common.Extensions.Enums;
using Invoicing.Data.AcPro.Types;
using CsvHelper.TypeConversion;

namespace Invoicing.Data.AcPro.Converters
{
	public class RecordTypeConverter : DefaultTypeConverter
	{
		public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
		{
			return EnumExtensions.GetEnumTypeByDescription<RecordType>(text);
		}
	}
}
