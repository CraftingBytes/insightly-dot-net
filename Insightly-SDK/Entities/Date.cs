using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class Date
	{
		[JsonProperty(PropertyName = "DATE_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "OCCASION_NAME")]
		public string OccasionName { get; set; }

		[JsonProperty(PropertyName = "OCCASION_DATE")]
		public DateTime OccasionDate { get; set; }

		[JsonProperty(PropertyName = "REPEAT_YEARLY")]
		public bool IsRepeatYearly { get; set; }

		[JsonProperty(PropertyName = "CREATE_TASK_YEARLY")]
		public bool IsCreateTaskYearly { get; set; }
	}
}
