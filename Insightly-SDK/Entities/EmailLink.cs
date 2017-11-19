using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class EmailLink
	{
		[JsonProperty(PropertyName = "EMAIL_LINK_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "EMAIL_ID")]
		public long EmailId { get; set; }

		[JsonProperty(PropertyName = "CONTACT_ID")]
		public long ContactId { get; set; }

		[JsonProperty(PropertyName = "ORGANISATION_ID")]
		public long OrganisationId { get; set; }

		[JsonProperty(PropertyName = "OPPORTUNITY_ID")]
		public long OpportunityId { get; set; }

		[JsonProperty(PropertyName = "PROJECT_ID")]
		public long ProjectId { get; set; }

		[JsonProperty(PropertyName = "LEAD_ID")]
		public long LeadId { get; set; }
	}
}
