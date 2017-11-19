using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class OrganisationLink
	{
		[JsonProperty(PropertyName = "ORG_LINK_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "FIRST_ORGANISATION_ID")]
		public long FirstOrganisationId { get; set; }

		[JsonProperty(PropertyName = "SECOND_ORGANISATION_ID")]
		public long SecondOrganisationId { get; set; }

		[JsonProperty(PropertyName = "RELATIONSHIP_ID")]
		public long RelationshipId { get; set; }

		[JsonProperty(PropertyName = "DETAILS")]
		public string Details { get; set; }
	}
}
