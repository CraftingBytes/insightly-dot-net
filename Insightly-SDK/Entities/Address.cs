using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class Address
	{
		[JsonProperty(PropertyName = "ADDRESS_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "ADDRESS_TYPE")]
		public string Type { get; set; }

		[JsonProperty(PropertyName = "STREET")]
		public string Street { get; set; }

		[JsonProperty(PropertyName = "CITY")]
		public string City { get; set; }

		[JsonProperty(PropertyName = "STATE")]
		public string State { get; set; }

		[JsonProperty(PropertyName = "POSTCODE")]
		public string PostalCode { get; set; }

		[JsonProperty(PropertyName = "COUNTRY")]
		public string Country { get; set; }
	}
}
