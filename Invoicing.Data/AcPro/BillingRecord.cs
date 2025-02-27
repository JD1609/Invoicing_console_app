using CsvHelper.Configuration.Attributes;
using Invoicing.Data.AcPro.Types;

namespace Invoicing.Data.AcPro
{
	public class BillingRecord
	{
		[Index(0)]
		public DateOnly Date { get; set; }

		[Index(1)]
		public string? Customer { get; set; }

		[Index(2)]
		public string? Project { get; set; }

		[Index(3)]
		public string? CostCenter { get; set; }

		[Index(4)]
		public string? Person { get; set; }

		[Index(5)]
		public decimal Hours { get; set; }

		[Index(6)]
		public RecordType Type { get; set; }

		[Index(7)]
		public string? Description { get; set; }

		[Index(8)]
		public string? Requested { get; set; }

		[Index(9)]
		public string? Comment { get; set; }

		[Index(10)]
		public int Item { get; set; }
	}
}
