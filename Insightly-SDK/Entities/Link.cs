using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class Link
	{
		[JsonProperty(PropertyName = "LINK_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "CONTACT_ID")]
		public long? ContactId { get; set; }

		[JsonProperty(PropertyName = "OPPORTUNITY_ID")]
		public long? OpportunityId { get; set; }

		[JsonProperty(PropertyName = "ORGANISATION_ID")]
		public long? OrganisationId { get; set; }

		[JsonProperty(PropertyName = "PROJECT_ID")]
		public long? ProjectId { get; set; }

		[JsonProperty(PropertyName = "SECOND_PROJECT_ID")]
		public long? SecondProjectId { get; set; }

		[JsonProperty(PropertyName = "SECOND_OPPORTUNITY_ID")]
		public long? SecondOpportunityId { get; set; }

		[JsonProperty(PropertyName = "ROLE")]
		public string Role { get; set; }

		[JsonProperty(PropertyName = "DETAILS")]
		public string Details { get; set; }
	}
}
