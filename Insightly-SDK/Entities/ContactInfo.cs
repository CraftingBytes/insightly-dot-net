using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class ContactInfo
	{
		[JsonProperty(PropertyName = "CONTACT_INFO_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "TYPE")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "SUBTYPE")]
		public string SubType { get; set; }

		[JsonProperty(PropertyName = "LABEL")]
		public string Label { get; set; }

		[JsonProperty(PropertyName = "DETAIL")]
		public string Detail { get; set; }
	}
}
