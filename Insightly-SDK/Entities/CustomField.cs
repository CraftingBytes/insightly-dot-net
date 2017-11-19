using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class CustomField
	{
		[JsonProperty(PropertyName = "CUSTOM_FIELD_ID")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "FIELD_VALUE")]
		public object Value { get; set; }
	}
}
