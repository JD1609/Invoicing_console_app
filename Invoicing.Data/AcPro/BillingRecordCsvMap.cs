using Invoicing.Data.AcPro.Converters;
using DecimalConverter = Invoicing.Data.AcPro.Converters.DecimalConverter;
using DateOnlyConverter = Invoicing.Data.AcPro.Converters.DateOnlyConverter;
using CsvHelper.Configuration;
namespace Invoicing.Data.AcPro
{
	public class BillingRecordCsvMap : ClassMap<BillingRecord>
	{
		public BillingRecordCsvMap()
		{
			Map((br) => br.Date).TypeConverter<DateOnlyConverter>();
			Map((br) => br.Customer);
			Map((br) => br.Project);
			//Map((br) => br.CostCenter);
			Map((br) => br.Person);
			Map((br) => br.Hours).TypeConverter<DecimalConverter>();
			Map((br) => br.Type).TypeConverter<RecordTypeConverter>();
			Map((br) => br.Description);
			Map((br) => br.Requested);
			Map((br) => br.Comment);
			Map((br) => br.Item);
		}
	}
}
