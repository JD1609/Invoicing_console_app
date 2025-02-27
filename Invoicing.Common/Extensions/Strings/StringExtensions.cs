using System.Text;

namespace Invoicing.Common.Extensions.Strings
{
	public static class StringExtensions
	{
		public static string FormatValuesRow(int maxValueLength, int spaces, params string[] @params)
		{
			var sb = new StringBuilder(@params.FirstOrDefault());
			
			while (sb.Length < (maxValueLength + spaces))
			{
				sb.Append(" ");
			}

			foreach (var param in @params.Skip(1))
			{
				var spacesWithoutValue = spaces - param.Length;
				sb.Append(param);
				sb.Append(GetStringXTimes(" ", spacesWithoutValue));
			}

			return sb.ToString();
		}

		public static string GetStringXTimes(string s,  int times)
		{
			var sb = new StringBuilder();

			while (sb.Length < times)
				sb.Append(s);

			return sb.ToString();
		}
	}
}
