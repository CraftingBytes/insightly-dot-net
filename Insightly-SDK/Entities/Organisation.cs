using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class Organisation
	{
		[JsonProperty(PropertyName = "ORGANISATION_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "ORGANISATION_NAME")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "BACKGROUND")]
		public string Background { get; set; }

		[JsonProperty(PropertyName = "IMAGE_URL")]
		public string ImageUrl { get; set; }

		[JsonProperty(PropertyName = "OWNER_USER_ID")]
		public long OwnerUserId { get; set; }

		[JsonProperty(PropertyName = "DATE_CREATED_UTC")]
		public DateTime DateCreatedUtc { get; set; }

		[JsonProperty(PropertyName = "DATE_UPDATED_UTC")]
		public DateTime DateUpdatedUtc { get; set; }

		[JsonProperty(PropertyName = "VISIBLE_TO")]
		public string VisibleTo { get; set; }

		[JsonProperty(PropertyName = "VISIBLE_TEAM_ID")]
		public long? VisibleTeamId { get; set; }

		[JsonProperty(PropertyName = "VISIBLE_USER_IDS")]
		public string VisibleUserIds { get; set; }

		[JsonProperty(PropertyName = "CUSTOMFIELDS")]
		public List<CustomField> CustomFields { get; set; }

		[JsonProperty(PropertyName = "ADDRESSES")]
		public List<Address> Addresses { get; set; }

		[JsonProperty(PropertyName = "CONTACTINFOS")]
		public List<ContactInfo> ContactInfos { get; set; }

		[JsonProperty(PropertyName = "DATES")]
		public List<Date> Dates { get; set; }

		[JsonProperty(PropertyName = "TAGS")]
		public List<Tag> Tags { get; set; }

		[JsonProperty(PropertyName = "LINKS")]
		public List<Link> Links { get; set; }

		[JsonProperty(PropertyName = "ORGANISATIONLINKS")]
		public List<OrganisationLink> OrganisationLinks { get; set; }

		[JsonProperty(PropertyName = "EMAILLINKS")]
		public List<EmailLink> EmailLinks { get; set; }

		public override string ToString()
		{
			return $"{Id} {Name}";
		}
	}
}
