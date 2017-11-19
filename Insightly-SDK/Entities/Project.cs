using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace InsightlySDK.Entities
{
	public class Project
	{
		[JsonProperty(PropertyName = "PROJECT_ID")]
		public long Id { get; set; }

		[JsonProperty(PropertyName = "PROJECT_NAME")]
		public string Name { get; set; }

		[JsonProperty(PropertyName = "STATUS")]
		public string Status { get; set; }

		[JsonProperty(PropertyName = "PROJECT_DETAILS")]
		public string Details { get; set; }

		[JsonProperty(PropertyName = "OPPORTUNITY_ID")]
		public long? OpportunityId { get; set; }

		[JsonProperty(PropertyName = "STARTED_DATE")]
		public DateTime? StartedDate { get; set; }

		[JsonProperty(PropertyName = "COMPLETED_DATE")]
		public DateTime? CompletedDate { get; set; }

		[JsonProperty(PropertyName = "IMAGE_URL")]
		public string ImageUrl { get; set; }

		[JsonProperty(PropertyName = "RESPONSIBLE_USER_ID")]
		public long? ResponsibleUserId { get; set; }

		[JsonProperty(PropertyName = "OWNER_USER_ID")]
		public long OwnerUserId { get; set; }

		[JsonProperty(PropertyName = "DATE_CREATED_UTC")]
		public DateTime DateCreatedUtc { get; set; }

		[JsonProperty(PropertyName = "DATE_UPDATED_UTC")]
		public DateTime DateUpdatedUtc { get; set; }

		[JsonProperty(PropertyName = "CATEGORY_ID")]
		public long? CategoryId { get; set; }

		[JsonProperty(PropertyName = "PIPELINE_ID")]
		public long? PipelineId { get; set; }

		[JsonProperty(PropertyName = "STAGE_ID")]
		public long? StageId { get; set; }

		[JsonProperty(PropertyName = "VISIBLE_TO")]
		public string VisibleTo { get; set; }

		[JsonProperty(PropertyName = "VISIBLE_TEAM_ID")]
		public long? VisibleTeamId { get; set; }

		[JsonProperty(PropertyName = "VISIBLE_USER_IDS")]
		public string VisibleUserIds { get; set; }

		[JsonProperty(PropertyName = "CUSTOMFIELDS")]
		public List<CustomField> CustomFieldArray { get; set; }

		private IDictionary<string, object> _customFieldDictionary;
		public IDictionary<string, object> CustomFieldDictionary {
			get
			{
				if (_customFieldDictionary == null)
				{
					_customFieldDictionary = CustomFieldArray.ToDictionary(cf => cf.Name, cf => cf.Value);
				}
				return _customFieldDictionary;
			}
		}

		[JsonProperty(PropertyName = "TAGS")]
		public List<Tag> Tags { get; set; }

		[JsonProperty(PropertyName = "LINKS")]
		public List<Link> Links { get; set; }

		[JsonProperty(PropertyName = "EMAILLINKS")]
		public List<EmailLink> EmailLinks { get; set; }

		public long? OrganisationId
		{
			get { return Links.First(l => l.OrganisationId != null).OrganisationId; }
		}

	}
}
