namespace Invoicing.Common.Utils
{
	public static class DateTimeUtils
	{
		public static int GetWorkingHoursInCurrentMonth() => GetWorkingDaysInCurrentMonth() * 8;

		public static int GetWorkingDaysInCurrentMonth()
		{
			var today = DateTime.Today;
			var firstDay = new DateTime(today.Year, today.Month, 1);
			var lastDay = firstDay.AddMonths(1).AddDays(-1);

			return Enumerable.Range(0, (lastDay - firstDay).Days + 1)
							 .Select(i => firstDay.AddDays(i))
							 .Count(d => d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday);
		}
	}
}
