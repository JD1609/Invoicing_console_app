using Invoicing.Data.Common;

namespace Invoicing.Interface.Billings
{
	public interface IBillingService
    {
		string FindBillingFile();
		List<ProjectBillingHours> GetProjectsHours(string filePath, bool hasHeaderRecord = false);
        void PrintResult(int itemsSpaces = 15);
	}
}
