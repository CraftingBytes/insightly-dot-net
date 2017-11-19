using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InsightlySDK.Entities;
using Newtonsoft.Json.Linq;

namespace InsightlySDK
{
	/// <summary>
	/// This class comprises the public interface for the .NET Insightly SDK.
	/// It provides user-friendly access to the the Insightly REST API
	/// by generating HTTPS requests for common usage patterns,
	/// and parsing the result from the server.
	/// </summary>
	/// <exception cref='ArgumentException'>
	/// Is thrown when an argument passed to a method is invalid.
	/// </exception>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
	public class Insightly
	{
		static string version = "v2.2";

		private readonly String _apiKey;

		/// <summary>
		/// Initializes a new instance of the <see cref="InsightlySDK.Insightly"/> class.
		/// </summary>
		/// <param name='apiKey'>
		/// The API key for the account to be accessed.
		/// </param>
		public Insightly(String apiKey)
		{
			_apiKey = apiKey;
		}

		/// <summary>
		/// Get comments for an object.
		/// </summary>
		/// <returns>
		/// Comments associated with object identified by id
		/// </returns>
		/// <param name='id'>
		/// ID of object to get comments of
		/// </param>
		public async Task<JArray> GetCommentsAsync(int id)
		{

			return await Get($"/{version}/Comments/" + id).AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Delete a comment.
		/// </summary>
		/// <param name='id'>
		/// ID of object to delete
		/// </param>
		public async Task DeleteComment(int id)
		{
			await Delete($"/{version}/Comments/" + id).AsStringAsync();
		}

		/// <summary>
		/// Creates or updates a comment.
		/// If you are updating an existing comment,
		/// be sure to include the optional comment_id parameter.
		/// </summary>
		/// <returns>
		/// The comment, as returned by the server
		/// </returns>
		/// <param name='body'>
		/// Comment body
		/// </param>
		/// <param name='ownerUserId'>
		/// Owner's user id
		/// </param>
		/// <param name='commentId'>
		/// Optional comment id (provide this if updating an existing comment)
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown if body is null or zero-length.
		/// </exception>
		public async Task<JObject> UpdateCommentAsync(string body, int ownerUserId, int? commentId = null)
		{
			if (string.IsNullOrEmpty(body))
			{
				throw new ArgumentException("Comment body cannot be empty.");
			}

			JObject data = new JObject
			{
				["BODY"] = body,
				["OWNER_USER_ID"] = ownerUserId
			};
			if (commentId != null)
			{
				data["COMMENT_ID"] = commentId;
			}

			return await Put($"/{version}/Comments").WithBody(data).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Gets a list of contacts matching specified query parameters.
		/// </summary>
		/// <returns>
		/// List of contacts.
		/// </returns>
		/// <param name='ids'>
		/// List of contact IDs,
		/// indicating a specific set of contacts to return.
		/// </param>
		/// <param name='email'>
		/// Return contacts with matching email address.
		/// </param>
		/// <param name='tag'>
		/// Return contacts associated with specified tag or keyword.
		/// </param>
		/// <param name='filters'>
		/// List of OData filters
		/// </param>
		/// <param name='top'>
		/// Return no more than N contacts.
		/// </param>
		/// <param name='skip'>
		/// Skip the first N contacts.
		/// </param>
		/// <param name='orderBy'>
		/// Name of field(s) by which to order the result set.
		/// </param>
		public async Task<JArray> GetContactsAsync(List<int> ids = null, string email = null, string tag = null,
								  List<string> filters = null, int? top = null, int? skip = null, string orderBy = null)
		{
			var request = Get($"/{version}/Contacts");
			BuildODataQuery(request, filters: filters, top: top, skip: skip, orderBy: orderBy);
			if (ids != null)
			{
				request.WithQueryParam("ids", ids);
			}
			if (email != null)
			{
				request.WithQueryParam("email", email);
			}
			if (tag != null)
			{
				request.WithQueryParam("tag", tag);
			}
			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Gets the contact with specified id.
		/// </summary>
		/// <returns>
		/// Contact with matching id.
		/// </returns>
		/// <param name='id'>
		/// CONTACT_ID of desired contact.
		/// </param>
		public async Task<JObject> GetContact(int id)
		{
			return await Get($"/{version}/Contacts/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/Update a contact on Insightly.
		/// </summary>
		/// <returns>
		/// The new/updated contact, as returned by the server.
		/// </returns>
		/// <param name='contact'>
		/// JObject describing the contact.
		/// If the CONTACT_ID property is ommitted / null / 0,
		/// then a new contact will be created.
		/// Otherwise, the contact with that id will be updated.
		/// </param>
		public async Task<JObject> AddContactAsync(JObject contact)
		{
			var request = Request($"/{version}/Contacts");

			if (contact["CONTACT_ID"] != null && contact["CONTACT_ID"].Value<int>() > 0)
			{
				request.WithMethod(HttpMethod.Put);
			}
			else
			{
				request.WithMethod(HttpMethod.Post);
			}

			return await request.WithBody(contact).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Deletes a contact, identified by its id.
		/// </summary>
		/// <param name='id'>
		/// CONTACT_ID of the contact to be deleted.
		/// </param>
		public async Task DeleteContactAsync(int id)
		{
			await Delete($"/{version}/Contacts/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get emails for a contact.
		/// </summary>
		/// <returns>
		/// Emails belonging to specified contact.
		/// </returns>
		/// <param name='contactId'>
		/// A contact's CONTACT_ID
		/// </param>
		public async Task<JArray> GetContactEmailsAsync(int contactId)
		{
			return await Get($"/{version}/Contacts/" + contactId + "/Emails")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get notes for a contact.
		/// </summary>
		/// <returns>
		/// Notes belonging to specified contact.
		/// </returns>
		/// <param name='contactId'>
		/// A contact's CONTACT_ID.
		/// </param>
		public async Task<JArray> GetContactNotesAsync(int contactId)
		{
			return await Get($"/{version}/Contacts/" + contactId + "/Notes")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get tasks for a contact.
		/// </summary>
		/// <returns>
		/// Tasks belonging to contact.
		/// </returns>
		/// <param name='contactId'>
		/// A contact's CONTACT_ID.
		/// </param>
		public async Task<JArray> GetContactTasksAsync(int contactId)
		{
			return await Get($"/{version}/Contacts/" + contactId + "/Tasks")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a list of countries recognized by Insightly.
		/// </summary>
		/// <returns>
		/// The countries recognized by Insightly.
		/// </returns>
		public async Task<JArray> GetCountriesAsync()
		{
			return await Get($"/{version}/Countries").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get the currencies recognized by Insightly
		/// </summary>
		/// <returns>
		/// The currencies recognized by Insightly.
		/// </returns>
		public async Task<JArray> GetCurrenciesAsync()
		{
			return await Get($"/{version}/Currencies").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Gets a list of custom fields.
		/// </summary>
		/// <returns>
		/// The custom fields.
		/// </returns>
		public async Task<JArray> GetCustomFieldsAsync()
		{
			return await Get($"/{version}/CustomFields").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Gets details for a custom field, identified by its id.
		/// </summary>
		/// <returns>
		/// The custom field.
		/// </returns>
		/// <param name='id'>
		/// Custom field id.
		/// </param>
		public async Task<JObject> GetCustomFieldAsync(int id)
		{
			return await Get($"/{version}/CustomFields/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get emails
		/// </summary>
		/// <returns>
		/// List of emails
		/// </returns>
		/// <param name='top'>
		/// OData top parameter.
		/// </param>
		/// <param name='skip'>
		/// OData skip parameter.
		/// </param>
		/// <param name='orderBy'>
		/// OData orderby parameter.
		/// </param>
		/// <param name='filters'>
		/// OData filters.
		/// </param>
		public async Task<JArray> GetEmailsAsync(
			int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Emails");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);
			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get specified the email.
		/// </summary>
		/// <returns>
		/// The email with matching id.
		/// </returns>
		/// <param name='id'>
		/// ID of email to get.
		/// </param>
		public async Task<JObject> GetEmailAsync(int id)
		{
			return await Get($"/{version}/Emails/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete an email.
		/// </summary>
		/// <param name='id'>
		/// ID of email to delete.
		/// </param>
		public async Task DeleteEmailAsync(int id)
		{
			await Delete($"/{version}/Emails/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get comments for an email
		/// </summary>
		/// <returns>
		/// List of email comments
		/// </returns>
		/// <param name='emailId'>
		/// Email id.
		/// </param>
		public async Task<JArray> GetEmailCommentsAsync(int emailId)
		{
			return await Get($"/{version}/Emails/" + emailId + "/Comments").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Add a comment to an existing email.
		/// </summary>
		/// <returns>
		/// The comment, as returned by the server.
		/// </returns>
		/// <param name='emailId'>
		/// ID of email to which the comment will be added.
		/// </param>
		/// <param name='body'>
		/// Comment body.
		/// </param>
		/// <param name='ownerUserId'>
		/// Owner's user id.
		/// </param>
		public async Task<JObject> AddCommentToEmailAsync(int emailId, string body, int ownerUserId)
		{
			var data = new JObject
			{
				["BODY"] = body,
				["OWNER_USER_ID"] = ownerUserId
			};
			return await Post($"/{version}/Emails/" + emailId + "/Comments")
				.WithBody(data).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get a calendar of upcoming events.
		/// </summary>
		/// <returns>
		/// Events matching query.
		/// </returns>
		/// <param name='top'>
		/// If provided, return maximum of N records.
		/// </param>
		/// <param name='skip'>
		/// If provided, skip the first N records.
		/// </param>
		/// <param name='orderBy'>
		/// If provided, specified field(s) to order results on.
		/// </param>
		/// <param name='filters'>
		/// If provided, specifies a list of OData filters.
		/// </param>
		public async Task<JArray> GetEventsAsync(
			int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Events");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);
			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get and event.
		/// </summary>
		/// <returns>
		/// An event.
		/// </returns>
		/// <param name='id'>
		/// EVENT_ID of the desired event.
		/// </param>
		public async Task<JObject> GetEvent(int id)
		{
			return await Get($"/{version}/Events/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update an event in the calendar.
		/// </summary>
		/// <returns>
		/// The new/updated event, as returned by the server.
		/// </returns>
		/// <param name='theEvent'>
		/// The event to add/update.
		/// </param>
		public async Task<JObject> AddEventAsync(JObject theEvent)
		{
			var request = Request($"/{version}/Events");

			if (theEvent["EVENT_ID"] != null && theEvent["EVENT_ID"].Value<int>() > 0)
			{
				request.WithMethod(HttpMethod.Put);
			}
			else
			{
				request.WithMethod(HttpMethod.Post);
			}

			return await request.WithBody(theEvent).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete an event.
		/// </summary>
		/// <param name='id'>
		/// ID of the event to be deleted.
		/// </param>
		public async Task DeleteEventAsync(int id)
		{
			await Delete($"/{version}/Events/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get file categories.
		/// </summary>
		/// <returns>
		/// The file categories for this account.
		/// </returns>
		public async Task<JArray> GetFileCategoriesAsync()
		{
			return await Get($"/{version}/FileCategories").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a file category.
		/// </summary>
		/// <returns>
		/// File category with specified id.
		/// </returns>
		/// <param name='id'>
		/// CATEGORY_ID of desired category.
		/// </param>
		public async Task<JObject> GetFileCategoryAsync(int id)
		{
			return await Get($"/{version}/FileCategories/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update a file category.
		/// </summary>
		/// <returns>
		/// The new/updated file category, as returned by the server.
		/// </returns>
		/// <param name='category'>
		/// The category to add/update.
		/// </param>
		public async Task<JObject> AddFileCategoryAsync(JObject category)
		{
			var request = Request($"/{version}/FileCategories");

			if ((category["CATEGORY_ID"] != null) && (category["CATEGORY_ID"].Value<int>() > 0))
			{
				request.WithMethod(HttpMethod.Put);
			}
			else
			{
				request.WithMethod(HttpMethod.Post);
			}

			return await request.WithBody(category).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a file category.
		/// </summary>
		/// <param name='id'>
		/// CATEGORY_ID of the file category to delete.
		/// </param>
		public async Task DeleteFileCategoryAsync(int id)
		{
			await Delete($"/{version}/FileCategories/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get a list of notes created by the user.
		/// </summary>
		/// <returns>
		/// Notes matching specified criteria.
		/// </returns>
		/// <param name='top'>
		/// Return first N notes.
		/// </param>
		/// <param name='skip'>
		/// Skip the first N notes.
		/// </param>
		/// <param name='orderBy'>
		/// Order notes by specified field(s)
		/// </param>
		/// <param name='filters'>
		/// List of OData filters to apply to results.
		/// </param>
		public async Task<JArray> GetNotesAsync(
			int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Notes");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);
			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a note.
		/// </summary>
		/// <returns>
		/// The note with matching <c>id</c>.
		/// </returns>
		/// <param name='id'>
		/// <c>NOTE_ID</c> of desired note.
		/// </param>
		public async Task<JObject> GetNoteAsync(int id)
		{
			return await Get($"/{version}/Notes/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update a note.
		/// </summary>
		/// <returns>
		/// The new/updated note, as returned by the server.
		/// </returns>
		/// <param name='note'>
		/// The note object to add or update.
		/// </param>
		public async Task<JObject> AddNoteAsync(JObject note)
		{
			var request = Request($"/{version}/Notes");
			request.WithMethod(IsValidId(note["NOTE_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(note).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a note.
		/// </summary>
		/// <param name='id'>
		/// NOTE_ID of note to delete.
		/// </param>
		public async Task DeleteNoteAsync(int id)
		{
			await Delete($"/{version}/Notes/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get comments attached to a note.
		/// </summary>
		/// <returns>
		/// The note comments.
		/// </returns>
		/// <param name='noteId'>
		/// <c>NOTE_ID</c> of desired note.
		/// </param>
		public async Task<JObject> GetNoteCommentsAsync(int noteId)
		{
			return await Get($"/{version}/Notes/" + noteId + "/Comments")
				.AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Attach a comment to a note.
		/// </summary>
		/// <returns>
		/// Result from server.
		/// </returns>
		/// <param name='noteId'>
		/// <c>NOTE_ID</c> of note to which comment will be attached.
		/// </param>
		/// <param name='comment'>
		/// The comment to attach to the note.
		/// </param>
		public async Task<JObject> AddNoteCommentAsync(int noteId, JObject comment)
		{
			return await Post($"/{version}/Notes/" + noteId + "/Comments")
				.WithBody(comment).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get opportunities matching specified criteria.
		/// </summary>
		/// <returns>
		/// Opportunities matching specified criteria.
		/// </returns>
		/// <param name='top'>
		/// Return first N opportunities.
		/// </param>
		/// <param name='skip'>
		/// Skip first N opportunities.
		/// </param>
		/// <param name='orderBy'>
		/// Order opportunities by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filters to apply to query.
		/// </param>
		public async Task<JArray> GetOpportunitiesAsync(
			int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Opportunities");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);
			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get an opportunity.
		/// </summary>
		/// <returns>
		/// The opportunity with matching id.
		/// </returns>
		/// <param name='id'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public async Task<JObject> GetOpportunityAsync(int id)
		{
			return await Get($"/{version}/Opportunities/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update an opportunity.
		/// </summary>
		/// <returns>
		/// The new/updated opportunity, as returned by the server.
		/// </returns>
		/// <param name='opportunity'>
		/// The opportunity to add/update.
		/// </param>
		public async Task<JObject> AddOpportunityAsync(JObject opportunity)
		{
			var request = Request($"/{version}/Opportunities");
			request.WithMethod(IsValidId(opportunity["OPPORTUNITY_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(opportunity).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete an opportunity.
		/// </summary>
		/// <param name='id'>
		/// OPPORTUNITY_ID of opportunity to delete.
		/// </param>
		public async Task DeleteOpportunityAsync(int id)
		{
			await Delete($"/{version}/Opportunities/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get history of states and reasons for an opportunity.
		/// </summary>
		/// <returns>
		/// History of states and reasons associated with specified opportunity.
		/// </returns>
		/// <param name='opportunityId'>
		/// OPPORTUNITY_ID of opportunity to get history of.
		/// </param>
		public async Task<JArray> GetOpportunityStateHistoryAsync(int opportunityId)
		{
			return await Get($"/{version}/Opportunities/" + opportunityId + "/StateHistory")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get the emails linked to an opportunity.
		/// </summary>
		/// <returns>
		/// The emails associated with specified opportunity.
		/// </returns>
		/// <param name='opportunityId'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public async Task<JArray> GetOpportunityEmailsAsync(int opportunityId)
		{
			return await Get($"/{version}/Opportunities/" + opportunityId + "/Emails")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get notes linked to an opportunity
		/// </summary>
		/// <returns>
		/// The notes associated with specified opportunity.
		/// </returns>
		/// <param name='opportunityId'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public async Task<JArray> GetOpportunityNotesAsync(int opportunityId)
		{
			return await Get($"/{version}/Opportunities/" + opportunityId + "/Notes")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get tasks linked to an opportunity.
		/// </summary>
		/// <returns>
		/// The tasks associated with specified opportunity.
		/// </returns>
		/// <param name='opportunityId'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public async Task<JArray> GetOpportunityTasksAsync(int opportunityId)
		{
			return await Get($"/{version}/Opportunities/" + opportunityId + "/Tasks")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a list of opportunity categories.
		/// </summary>
		/// <returns>
		/// The opportunity categories.
		/// </returns>
		public async Task<JArray> GetOpportunityCategoriesAsync()
		{
			return await Get($"/{version}/OpportunityCategories")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get an opportunity categoriy.
		/// </summary>
		/// <returns>
		/// The specified opportunity categoriy.
		/// </returns>
		/// <param name='id'>
		/// CATEGORY_ID of desired opportunity category.
		/// </param>
		public async Task<JObject> GetOpportunityCategoryAsync(int id)
		{
			return await Get($"/{version}/OpportunityCategories/" + id)
				.AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update an opportunity category.
		/// </summary>
		/// <returns>
		/// The new/updated opportunity category, as returned by the server.
		/// </returns>
		/// <param name='category'>
		/// The category to add/update.
		/// </param>
		public async Task<JObject> AddOpportunityCategoryAsync(JObject category)
		{
			var request = Request($"/{version}/OpportunityCategories");
			request.WithMethod(IsValidId(category["CATEGORY_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(category).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete an opportunity category.
		/// </summary>
		/// <param name='id'>
		/// CATEGORY_ID of opportunity category to delete.
		/// </param>
		public async Task DeleteOpportunityCategoryAsync(int id)
		{
			await Delete($"/{version}/OpportunityCategories/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get a list of opportunity state reasons.
		/// </summary>
		/// <returns>
		/// The opportunity state reasons.
		/// </returns>
		public async Task<JArray> GetOpportunityStateReasonsAsync()
		{
			return await Get($"/{version}/OpportunityStateReasons")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get organizations matching specified criteria.
		/// </summary>
		/// <returns>
		/// The organizations that match the query.
		/// </returns>
		/// <param name='ids'>
		/// List of ids of organizations to return.
		/// </param>
		/// <param name='domain'>
		/// Domain.
		/// </param>
		/// <param name='tag'>
		/// Tag.
		/// </param>
		/// <param name='top'>
		/// Return first N organizations.
		/// </param>
		/// <param name='skip'>
		/// Skip first N organizations.
		/// </param>
		/// <param name='orderBy'>
		/// Order results by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filters statements to apply.
		/// </param>
		public async Task<List<Organisation>> GetOrganizationsAsync(
			List<int> ids = null, string domain = null, string tag = null, int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Organisations");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);

			if (domain != null)
			{
				request.WithQueryParam("domain", domain);
			}
			if (tag != null)
			{
				request.WithQueryParam("tag", tag);
			}
			if (ids != null)
			{
				request.WithQueryParam("ids", String.Join(",", ids));
			}

			return await request.AsJsonAsync<List<Organisation>>();
		}

		/// <summary>
		/// Get an organization.
		/// </summary>
		/// <returns>
		/// Matching organization.
		/// </returns>
		/// <param name='id'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public async Task<Organisation> GetOrganizationAsync(int id)
		{
			return await Get($"/{version}/Organisations/" + id).AsJsonAsync<Organisation>();
		}

		/// <summary>
		/// Add/update an organization
		/// </summary>
		/// <returns>
		/// The new/updated organization, as returned by the server.
		/// </returns>
		/// <param name='organization'>
		/// Organization to add/update.
		/// </param>
		public async Task<JObject> AddOrganizationAsync(JObject organization)
		{
			var request = Request($"/{version}/Organisations");
			request.WithMethod(IsValidId(organization["ORGANISATION_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(organization).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete an organization.
		/// </summary>
		/// <param name='id'>
		/// <c>ORGANISATION_ID</c> of organization to delete.
		/// </param>
		public async Task DeleteOrganizationAsync(int id)
		{
			await Delete($"/{version}/Organisations/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get emails attached to an organization.
		/// </summary>
		/// <returns>
		/// The organization's emails.
		/// </returns>
		/// <param name='organizationId'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public async Task<JArray> GetOrganizationEmailsAsync(long organizationId)
		{
			return await Get($"/{version}/Organisations/" + organizationId + "/Emails")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get notes attached to an organization.
		/// </summary>
		/// <returns>
		/// The organization's notes.
		/// </returns>
		/// <param name='organizationId'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public async Task<JArray> GetOrganizationNotesAsync(long organizationId)
		{
			return await Get($"/{version}/Organisations/" + organizationId + "/Notes")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get tasks attached to an organization.
		/// </summary>
		/// <returns>
		/// The organization's tasks.
		/// </returns>
		/// <param name='organizationId'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public async Task<JArray> GetOrganizationTasksAsync(long organizationId)
		{
			return await Get($"/{version}/Organisations/" + organizationId + "/Tasks")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a list of pipelines.
		/// </summary>
		/// <returns>
		/// The pipelines.
		/// </returns>
		public async Task<JArray> GetPipelinesAsync()
		{
			return await Get($"/{version}/Pipelines").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a pipeline.
		/// </summary>
		/// <returns>
		/// The pipeline.
		/// </returns>
		/// <param name='id'>
		/// Pipeline id.
		/// </param>
		public async Task<JObject> GetPipelineAsync(int id)
		{
			return await Get($"/{version}/Pipelines/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get the pipeline stages.
		/// </summary>
		/// <returns>
		/// The pipeline stages.
		/// </returns>
		public async Task<JArray> GetPipelineStagesAsync()
		{
			return await Get($"/{version}/PipelineStages").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a pipeline stage.
		/// </summary>
		/// <returns>
		/// The pipeline stage.
		/// </returns>
		/// <param name='id'>
		/// Pipeline stage's id.
		/// </param>
		public async Task<JObject> GetPipelineStageAsync(int id)
		{
			return await Get("v2.1/PipelineStages/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get list project categories.
		/// </summary>
		/// <returns>
		/// The project categories.
		/// </returns>
		public async Task<JArray> GetProjectCategoriesAsync()
		{
			return await Get($"/{version}/ProjectCategories").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a project category.
		/// </summary>
		/// <returns>
		/// The project category.
		/// </returns>
		/// <param name='id'>
		/// <c>CATEGORY_ID</c> of desired category.
		/// </param>
		public async Task<JObject> GetProjectCategoryAsync(int id)
		{
			return await Get($"/{version}/ProjectCategories/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update a project category.
		/// </summary>
		/// <returns>
		/// The new/update project category, as returned by the server.
		/// </returns>
		/// <param name='category'>
		/// The category to add/update.
		/// </param>
		public async Task<JObject> AddProjectCategoryAsync(JObject category)
		{
			var request = Request($"/{version}/ProjectCategories");
			request.WithMethod(IsValidId(category["CATEGORY_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(category).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a project category.
		/// </summary>
		/// <param name='id'>
		/// <c>CATEGORY_ID</c> of category to be deleted.
		/// </param>
		public async Task DeleteProjectCategoryAsync(int id)
		{
			await Delete($"/{version}/ProjectCategories/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get projects matching specified criteria.
		/// </summary>
		/// <returns>
		/// Matching projects.
		/// </returns>
		/// <param name='top'>
		/// Return the first N projects.
		/// </param>
		/// <param name='skip'>
		/// Skip the first N projects.
		/// </param>
		/// <param name='orderBy'>
		/// Order results by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filter statements to apply.
		/// </param>
		public async Task<List<Project>> GetProjectsAsync(
			int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Projects");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);
			return await request.AsJsonAsync<List<Project>>();
		}

		/// <summary>
		/// Get a project.
		/// </summary>
		/// <returns>
		/// The project.
		/// </returns>
		/// <param name='projectId'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public async Task<Project> GetProjectAsync(long projectId)
		{
			return await Get($"/{version}/Projects/" + projectId).AsJsonAsync<Project>();
		}

		/// <summary>
		/// Add/update a project.
		/// </summary>
		/// <returns>
		/// The new/updated project, as returned by the server.
		/// </returns>
		/// <param name='project'>
		/// Project to add/update.
		/// </param>
		public async Task<JObject> AddProjectAsync(JObject project)
		{
			var request = Request($"/{version}/Projects");
			request.WithMethod(IsValidId(project["PROJECT_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(project).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a project.
		/// </summary>
		/// <param name='id'>
		/// <c>PROJECT_ID</c> of project to be deleted.
		/// </param>
		public async Task DeleteProjectAsync(int id)
		{
			await Delete($"/{version}/Projects/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get the emails attached to a project.
		/// </summary>
		/// <returns>
		/// The project's emails.
		/// </returns>
		/// <param name='projectId'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public async Task<JArray> GetProjectEmailsAsync(long projectId)
		{
			return await Get($"/{version}/Projects/" + projectId + "/Emails")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get the notes attached to a project.
		/// </summary>
		/// <returns>
		/// The project's notes.
		/// </returns>
		/// <param name='projectId'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public async Task<JArray> GetProjectNotesAsync(long projectId)
		{
			return await Get($"/{version}/Projects/" + projectId + "/Notes")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get the tasks attached to a project.
		/// </summary>
		/// <returns>
		/// The project's tasks.
		/// </returns>
		/// <param name='projectId'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public async Task<JArray> GetProjectTasksAsync(long projectId)
		{
			return await Get($"/{version}/Projects/" + projectId + "/Tasks")
				.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a list of relationships.
		/// </summary>
		/// <returns>
		/// The relationships.
		/// </returns>
		public async Task<JArray> GetRelationshipsAsync()
		{
			return await Get($"/{version}/Relationships").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get tags associated with a parent object.
		/// </summary>
		/// <returns>
		/// List of tags.
		/// </returns>
		/// <param name='parentId'>
		/// The id of the parent object.
		/// </param>
		public async Task<JArray> GetTagsAsync(int parentId)
		{
			return await Get($"/{version}/Tags/" + parentId).AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get tasks matching specified criteria.
		/// </summary>
		/// <returns>
		/// The tasks.
		/// </returns>
		/// <param name='ids'>
		/// List of <c>TASK_ID</c>s identifying specific tasks to get.
		/// </param>
		/// <param name='top'>
		/// Return first N tasks.
		/// </param>
		/// <param name='skip'>
		/// Skip first N tasks.
		/// </param>
		/// <param name='orderBy'>
		/// Orders results by specified field(s)
		/// </param>
		/// <param name='filters'>
		/// List of OData filter statements to apply.
		/// </param>
		public async Task<JArray> GetTasksAsync(
			List<int> ids = null, int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Tasks");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);

			if ((ids != null) && (ids.Count > 0))
			{
				request.WithQueryParam("ids", String.Join(",", ids));
			}

			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a task.
		/// </summary>
		/// <returns>
		/// The task.
		/// </returns>
		/// <param name='id'>
		/// <c>TASK_ID</c> of desired task.
		/// </param>
		public async Task<JObject> GetTaskAsync(int id)
		{
			return await Get($"/{version}/Tasks/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update a task.
		/// </summary>
		/// <returns>
		/// The new/update task, as returned by the server.
		/// </returns>
		/// <param name='task'>
		/// The task to create/update.
		/// </param>
		public async Task<JObject> AddTaskAsync(JObject task)
		{
			var request = Request($"/{version}/Tasks");
			request.WithMethod(IsValidId(task["TASK_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(task).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a task.
		/// </summary>
		/// <param name='id'>
		/// <c>TASK_ID</c> of task to delete.
		/// </param>
		public async Task DeleteTaskAsync(int id)
		{
			await Delete($"/{version}/Tasks/" + id).AsStringAsync();
		}


		/// <summary>
		/// Get comments attached to a task.
		/// </summary>
		/// <returns>
		/// The task's comments.
		/// </returns>
		/// <param name='taskId'>
		/// <c>TASK_ID</c> of desired task.
		/// </param>
		public async Task<JArray> GetTaskCommentsAsync(int taskId)
		{
			return await Get($"/{version}/Tasks/" + taskId + "/Comments").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Add a comment to a task.
		/// 
		/// The comment object should have the following fields:
		/// * BODY = comment text
		/// * OWNER_USER_ID = the comment author's numeric user id
		/// </summary>
		/// <returns>
		/// The new comment, as returned by the server.
		/// </returns>
		/// <param name='taskId'>
		/// <c>TASK_ID</c> of task to which the comment will be added.
		/// </param>
		/// <param name='comment'>
		/// The comment to add.
		/// </param>
		public async Task<JObject> AddTaskCommentAsync(int taskId, JObject comment)
		{
			return await Post($"/{version}/Tasks/" + taskId + "/Comments").WithBody(comment).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get members of a team.
		/// </summary>
		/// <returns>
		/// The team's members.
		/// </returns>
		/// <param name='teamId'>
		/// <c>TEAM_ID</c> of desired team.
		/// </param>
		public async Task<JArray> GetTeamMembersAsync(int teamId)
		{
			return await Get($"/{version}/TeamMembers/teamid=" + teamId).AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a team member's details.
		/// </summary>
		/// <returns>
		/// The team member.
		/// </returns>
		/// <param name='id'>
		/// Desired team member's id.
		/// </param>
		public async Task<JObject> GetTeamMemberAsync(int id)
		{
			return await Get($"/{version}/TeamMembers/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add a team member.
		/// </summary>
		/// <returns>
		/// The new team member, as returned by the server.
		/// </returns>
		/// <param name='teamMember'>
		/// The team member to add.
		/// </param>
		public async Task<JObject> AddTeamMemberAsync(JObject teamMember)
		{
			return await Post($"/{version}/TeamMembers").WithBody(teamMember).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a team member.
		/// </summary>
		/// <param name='id'>
		/// Id of team member to be deleted.
		/// </param>
		public async Task DeleteTeamMemberAsync(int id)
		{
			await Delete($"/{version}/TeamMembers/" + id).AsStringAsync();
		}

		/// <summary>
		/// Update a team member.
		/// </summary>
		/// <returns>
		/// The updated team member, as returned by the server.
		/// </returns>
		/// <param name='teamMember'>
		/// The team member to update.
		/// </param>
		public async Task<JObject> UpdateTeamMemberAsync(JObject teamMember)
		{
			return await Put($"/{version}/TeamMembers").WithBody(teamMember).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Get teams matching specified criteria.
		/// </summary>
		/// <returns>
		/// Matching teams.
		/// </returns>
		/// <param name='top'>
		/// Return first N teams.
		/// </param>
		/// <param name='skip'>
		/// Skip first N teams.
		/// </param>
		/// <param name='orderBy'>
		/// Order teams by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filter statements to apply.
		/// </param>/
		public async Task<JArray> GetTeamsAsync(
			int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			var request = Get($"/{version}/Teams");
			BuildODataQuery(request, top: top, skip: skip, orderBy: orderBy, filters: filters);
			return await request.AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a team.
		/// </summary>
		/// <returns>
		/// The team.
		/// </returns>
		/// <param name='id'>
		/// <c>TEAM_ID</c> of desired team.
		/// </param>
		public async Task<JObject> GetTeamAsync(int id)
		{
			return await Get($"/{version}/Teams/" + id).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Add/update a team.
		/// </summary>
		/// <returns>
		/// The new/updated team, as returned by the server.
		/// </returns>
		/// <param name='team'>
		/// Team.
		/// </param>
		public async Task<JObject> AddTeamAsync(JObject team)
		{
			var request = Request($"/{version}/Teams");
			request.WithMethod(IsValidId(team["TEAM_ID"]) ? HttpMethod.Put : HttpMethod.Post);
			return await request.WithBody(team).AsJsonAsync<JObject>();
		}

		/// <summary>
		/// Delete a team.
		/// </summary>
		/// <param name='id'>
		/// <c>TEAM_ID</c> of team to delete.
		/// </param>
		public async Task DeleteTeamAsync(int id)
		{
			await Delete($"/{version}/Teams/" + id).AsStringAsync();
		}

		/// <summary>
		/// Get a list of users for this account.
		/// </summary>
		/// <returns>
		/// This account's users.
		/// </returns>
		public async Task<JArray> GetUsersAsync()
		{
			return await Get($"/{version}/Users/").AsJsonAsync<JArray>();
		}

		/// <summary>
		/// Get a user.
		/// </summary>
		/// <returns>
		/// The user.
		/// </returns>
		/// <param name='id'>
		/// Desired user's numeric id.
		/// </param>
		public async Task<JObject> GetUserAsync(int id)
		{
			return await Get($"/{version}/Users/" + id).AsJsonAsync<JObject>();
		}

		public async Task Test(int? top = null)
		{
			Console.WriteLine("Testing API .....");

			Console.WriteLine("Testing authentication");

			int failed = 0;

			var currencies = await GetCurrenciesAsync();
			if (currencies.Count > 0)
			{
				Console.WriteLine("Authentication passed...");
			}
			else
			{
				failed += 1;
			}

			int userId = 0;
			try
			{
				var users = await GetUsersAsync();
				JObject user = users[0].Value<JObject>();
				userId = user["USER_ID"].Value<int>();
				Console.WriteLine("PASS: GetUsers, found " + users.Count + " users.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetUsers()");
				failed += 1;
			}

			// Test GetContacts()
			try
			{
				var contacts = await GetContactsAsync(orderBy: "DATE_UPDATED_UTC desc", top: top);
				Console.WriteLine("PASS: GetContacts(), found " + contacts.Count + " contacts.");

				if (contacts.Count > 0)
				{
					JObject contact = contacts[0].Value<JObject>();
					int contactId = contact["CONTACT_ID"].Value<int>();

					// Test GetContactEmails()
					try
					{
						var emails = await GetContactEmailsAsync(contactId);
						Console.WriteLine("PASS: GetContactEmails(), found " + emails.Count + " emails.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetContactEmails()");
						failed += 1;
					}

					// Test GetContactNotes()
					try
					{
						var notes = await GetContactNotesAsync(contactId);
						Console.WriteLine("PASS: GetContactNotes(), found " + notes.Count + " notes.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetContactNotes()");
						failed += 1;
					}

					// Test GetContactTasks()
					try
					{
						var tasks = await GetContactTasksAsync(contactId);
						Console.WriteLine("PASS: GetContactTasks(), found " + tasks.Count + " tasks.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetContactTasks()");
						failed += 1;
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetContacts()");
				failed += 1;
			}

			// Test AddContact()
			try
			{
				var contact = new JObject
				{
					["SALUTATION"] = "Mr",
					["FIRST_NAME"] = "Testy",
					["LAST_NAME"] = "McTesterson"
				};

				var contactInfos = new JArray();
				var email = new JObject
				{
					["TYPE"] = "EMAIL",
					["LABEL"] = "Personal",
					["DETAIL"] = "test@example.com"
				};
				contactInfos.Add(email);
				contact["CONTACTINFOS"] = contactInfos;

				contact = await AddContactAsync(contact);
				Console.WriteLine("PASS: AddContact()");

				// Test GetContacts() by email
				try
				{
					var contacts = await GetContactsAsync(null, "test@example.com");

					if (contacts.Count != 1)
					{
						throw new Exception();
					}

					Console.WriteLine("PASS: GetContacts() by email, found " + contacts.Count + " contacts.");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: GetContacts() by email");
					failed += 1;
				}

				// Test DeleteContact()
				try
				{
					await DeleteContactAsync(contact["CONTACT_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteContact()");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: DeleteContact()");
					failed += 1;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: AddContact()");
				throw;
			}

			// Test GetCountries()
			try
			{
				var countries = await GetCountriesAsync();
				Console.WriteLine("PASS: GetCountries(), found " + countries.Count + " countries.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetCountries()");
				failed += 1;
			}

			// Test GetCurrencies()
			try
			{
				currencies = await GetCurrenciesAsync();
				Console.WriteLine("PASS: GetCurrencies(), found " + currencies.Count + " currencies.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetCurrencies()");
				failed += 1;
			}

			// Test GetCustomFields()
			try
			{
				var customFields = await GetCustomFieldsAsync();
				Console.WriteLine("PASS: GetCustomFields(), found " + customFields.Count + " custom fields.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetCustomFields()");
				failed += 1;
			}

			// Test GetEmails()
			try
			{
				var emails = await GetEmailsAsync();
				Console.WriteLine("PASS: GetEmails(), found " + emails.Count + " emails.");
			}
			catch
			{
				Console.WriteLine("FAIL: GetEmails()");
				failed += 1;
			}

			// Test GetEvents()
			try
			{
				var events = await GetEventsAsync(top: top);
				Console.WriteLine("PASS: GetEvents(), found " + events.Count + " events.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetEvents()");
				failed += 1;
			}

			// Test AddEvent()
			try
			{
				var _event = new JObject
				{
					["TITLE"] = "Test Event",
					["LOCATION"] = "Somewhere",
					["DETAILS"] = "Details",
					["START_DATE_UTC"] = "2014-07-12 12:00:00",
					["END_DATE_UTC"] = "2014-07-12 13:00:00",
					["OWNER_USER_ID"] = userId,
					["ALL_DAY"] = false,
					["PUBLICLY_VISIBLE"] = true
				};
				_event = await AddEventAsync(_event);
				Console.WriteLine("PASS: AddEvent");

				// Test DeleteEvent()
				try
				{
					await DeleteEventAsync(_event["EVENT_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteEvent()");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: DeleteEvent()");
					failed += 1;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: AddEvent");
				failed += 1;
			}

			// Test GetFileCategories()
			try
			{
				var categories = await GetFileCategoriesAsync();
				Console.WriteLine("PASS: GetFileCategories(), found " + categories.Count + " categories.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetFileCategories()");
				failed += 1;
			}

			// Test AddFileCategory()
			try
			{
				var category = new JObject
				{
					["CATEGORY_NAME"] = "Test Category",
					["ACTIVE"] = true,
					["BACKGROUND_COLOR"] = "000000"
				};
				category = await AddFileCategoryAsync(category);
				Console.WriteLine("PASS: AddFileCategory()");

				// Test DeleteFileCategory()
				try
				{
					var categoryId = category["CATEGORY_ID"].Value<int>();
					await DeleteFileCategoryAsync(categoryId);
					Console.WriteLine("PASS: DeleteFileCategory()");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: DeleteFileCategory()");
					failed += 1;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: AddFileCategory()");
				failed += 1;
			}

			// Test GetNotes()
			try
			{
				var notes = await GetNotesAsync();
				Console.WriteLine("PASS: GetNotes(), found " + notes.Count + " notes.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetNotes()");
				failed += 1;
			}

			// Test GetOpportunities
			try
			{
				var opportunities = await GetOpportunitiesAsync(orderBy: "DATE_UPDATED_UTC desc", top: top);
				Console.WriteLine("PASS: GetOpportunities(), found " + opportunities.Count + " opportunities.");

				if (opportunities.Count > 0)
				{
					var opportunity = opportunities[0];
					int opportunityId = opportunity["OPPORTUNITY_ID"].Value<int>();

					// Test GetOpportunityEmails()
					try
					{
						var emails = await GetOpportunityEmailsAsync(opportunityId);
						Console.WriteLine("PASS: GetOpportunityEmails(), found " + emails.Count + " emails.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOpportunityEmails()");
						failed += 1;
					}

					// Test GetOpportunityNotes()
					try
					{
						var notes = await GetOpportunityNotesAsync(opportunityId);
						Console.WriteLine("PASS: GetOpportunityNotes(), found " + notes.Count + " notes.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOpportunityNotes()");
						failed += 1;
					}

					// Test GetOpportunityTasks()
					try
					{
						var tasks = await GetOpportunityTasksAsync(opportunityId);
						Console.WriteLine("PASS: GetOpportunityTasks(), found " + tasks.Count + " tasks.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOpportunityTasks()");
						failed += 1;
					}

					// Test GetOpportunityStateHistory()
					try
					{
						var history = await GetOpportunityStateHistoryAsync(opportunityId);
						Console.WriteLine("PASS: GetOpportunityStateHistory(), found " + history.Count + " states in history.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOpportunityStateHistory()");
						failed += 1;
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetOpportunities()");
				failed += 1;
			}

			// Test GetOpportunityCategories()
			try
			{
				var categories = await GetOpportunityCategoriesAsync();
				Console.WriteLine("PASS: GetOpportunityCategories(), found " + categories.Count + " categories.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetOpportunityCategories()");
				failed += 1;
			}

			// Test AddOpportunityCategory()
			try
			{
				var category = new JObject
				{
					["CATEGORY_NAME"] = "Test Category",
					["ACTIVE"] = true,
					["BACKGROUND_COLOR"] = "000000"
				};
				category = await AddFileCategoryAsync(category);
				Console.WriteLine("PASS: AddOpportuntityCategory()");

				// Test DeleteOpportunityCategory()
				try
				{
					await DeleteOpportunityCategoryAsync(category["CATEGORY_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteOpportunityCategory()");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: DeleteOpportunityCategory()");
					failed += 1;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: AddOpportunityCategory()");
				failed += 1;
			}

			// Test GetOpportunityStateReasons()
			try
			{
				var reasons = await GetOpportunityStateReasonsAsync();
				Console.WriteLine("PASS: GetOpportunityStateReasons(), found " + reasons.Count + " reasons.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetOpportunityStateReasons()");
				failed += 1;
			}

			// Test GetOrganizations()
			try
			{
				var organizations = await GetOrganizationsAsync(top: top, orderBy: "DATE_UPDATED_UTC desc");
				Console.WriteLine("PASS: GetOrganizations(), found " + organizations.Count + " organizations.");

				if (organizations.Count > 0)
				{
					var organization = organizations[0];
					long organizationId = organization.Id;

					// Test GetOrganizationEmails();
					try
					{
						var emails = await GetOrganizationEmailsAsync(organizationId);
						Console.WriteLine("PASS: GetOrganizationEmails(), found " + emails.Count + " emails.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOrganizationEmails()");
						failed += 1;
					}

					// Test GetOrganizationNotes();
					try
					{
						var notes = await GetOrganizationNotesAsync(organizationId);
						Console.WriteLine("PASS: GetOrganizationNotes(), found " + notes.Count + " notes.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOrganizationNotes()");
						failed += 1;
					}

					// Test GetOrganizationTasks();
					try
					{
						var tasks = await GetOrganizationTasksAsync(organizationId);
						Console.WriteLine("PASS: GetOrganizationTasks(), found " + tasks.Count + " tasks.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetOrganizationTasks()");
						failed += 1;
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetOrganizations()");
				failed += 1;
			}

			// Test AddOrganization
			try
			{
				var organization = new JObject
				{
					["ORGANISATION_NAME"] = "Foo Corp",
					["BACKGROUND"] = "Details"
				};
				organization = await AddOrganizationAsync(organization);
				Console.WriteLine("PASS: AddOrganization()");

				// Test DeleteOrganization()
				try
				{
					await DeleteOrganizationAsync(organization["ORGANISATION_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteOrganization()");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: DeleteOrganization()");
					failed += 1;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: AddOrganization()");
				failed += 1;
			}

			// Test GetPipelines()
			try
			{
				var pipelines = await GetPipelinesAsync();
				Console.WriteLine("PASS: GetPipelines(), found " + pipelines.Count + " pipelines.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetPipelines()");
				failed += 1;
			}

			// Test GetPipelineStages()
			try
			{
				var stages = await GetPipelineStagesAsync();
				Console.WriteLine("PASS: GetPipelineStages(), found " + stages.Count + " pipeline stages.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetPipelineStages()");
				failed += 1;
			}

			// Test GetProjects()
			try
			{
				var projects = await GetProjectsAsync(top: top, orderBy: "DATE_UPDATED_UTC desc");
				Console.WriteLine("PASS: GetProjects(), found " + projects.Count + " projects.");

				if (projects.Count > 0)
				{
					var project = projects[0];
					long projectId = project.Id;

					// Test GetProjectEmails
					try
					{
						var emails = await GetProjectEmailsAsync(projectId);
						Console.WriteLine("PASS: GetProjectEmails(), found " + emails.Count + " emails.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetProjectEmails()");
						failed += 1;
					}

					// Test GetProjectNotes
					try
					{
						var notes = await GetProjectNotesAsync(projectId);
						Console.WriteLine("PASS: GetProjectNotes(), found " + notes.Count + " notes.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetProjectNotes()");
						failed += 1;
					}

					// Test GetProjectTasks
					try
					{
						var emails = await GetProjectTasksAsync(projectId);
						Console.WriteLine("PASS: GetProjectTasks(), found " + emails.Count + " tasks.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetProjectTasks()");
						failed += 1;
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetProjects()");
				failed += 1;
			}

			// Test GetProjectCategories
			try
			{
				var categories = await GetProjectCategoriesAsync();
				Console.WriteLine("PASS: GetProjectCategories(), found " + categories.Count + " categories.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetProjectCategories()");
				failed += 1;
			}

			// Test AddProjectCategory()
			try
			{
				var category = new JObject
				{
					["CATEGORY_NAME"] = "Test Category",
					["ACTIVE"] = true,
					["BACKGROUND_COLOR"] = "000000"
				};
				category = await AddProjectCategoryAsync(category);
				Console.WriteLine("PASS: AddProjectCategory()");

				// Test DeleteProjectCategory()
				try
				{
					await DeleteProjectCategoryAsync(category["CATEGORY_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteProjectCategory()");
				}
				catch (Exception)
				{
					Console.WriteLine("FAIL: DeleteProjectCategory()");
					failed += 1;
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: AddProjectCategory()");
				failed += 1;
			}

			// Test GetRelationships()
			try
			{
				var relationships = await GetRelationshipsAsync();
				Console.WriteLine("PASS: getRelationships(), found " + relationships.Count + " relationships.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetTasks()");
				failed += 1;
			}

			// Test GetTasks()
			try
			{
				var tasks = await GetTasksAsync(top: top, orderBy: "DUE_DATE desc");
				Console.WriteLine("PASS: GetTasks(), found " + tasks.Count + " tasks.");
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetTasks()");
				failed += 1;
			}

			// Test GetTeams
			try
			{
				var teams = await GetTeamsAsync();
				Console.WriteLine("PASS: GetTeams(), found " + teams.Count + " teams.");

				if (teams.Count > 0)
				{
					var team = teams[0];

					// Test GetTeamMembers
					try
					{
						var teamMembers = await GetTeamMembersAsync(team["TEAM_ID"].Value<int>());
						Console.WriteLine("PASS: GetTeamMembers(), found " + teamMembers.Count + " team members.");
					}
					catch (Exception)
					{
						Console.WriteLine("FAIL: GetTeamMembers()");
						failed += 1;
					}
				}
			}
			catch (Exception)
			{
				Console.WriteLine("FAIL: GetTeams()");
				failed += 1;
			}

			if (failed > 0)
			{
				throw new Exception(failed + " Tests failed!");
			}

			Console.WriteLine("All tests passed!");
		}

		/// <summary>
		/// Adds OData query parameters to request.
		/// </summary>
		/// <returns>
		/// request, with specified OData query parameters added
		/// </returns>
		/// <param name='request'>
		/// InsightlyRequest to which the query parameters will be added.
		/// </param>
		/// <param name='top'>
		/// (Optional) If provided, specifies the limit
		/// to the size of the result set returned.
		/// </param>
		/// <param name='skip'>
		/// (Optional) If provided, specified the number of items to skip.
		/// </param>
		/// <param name='orderBy'>
		/// (Optional) If provided, results will be order by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// A list of filters to apply.
		/// </param>
		private InsightlyRequest BuildODataQuery(
			InsightlyRequest request, int? top = null, int? skip = null, string orderBy = null, List<string> filters = null)
		{
			if (top != null)
			{
				request.Top(top.Value);
			}
			if (skip != null)
			{
				request.Skip(skip.Value);
			}
			if (orderBy != null)
			{
				request.OrderBy(orderBy);
			}
			if (filters != null)
			{
				request.Filters(filters);
			}

			return request;
		}

		/// <summary>
		/// Check if <c>id</c> represents a valid object id.
		/// </summary>
		/// <returns>
		/// <c>true</c> if <c>id</c> is not null and non-zero; otherwise, <c>false</c>.
		/// </returns>
		/// <param name='id'>
		/// JToken object representing the object id to check.
		/// </param>
		private bool IsValidId(JToken id)
		{
			return (id != null) && id.Value<int>() > 0;
		}

		private InsightlyRequest Get(string urlPath)
		{
			return new InsightlyRequest(_apiKey, urlPath);
		}

		private InsightlyRequest Put(string urlPath)
		{
			return (new InsightlyRequest(_apiKey, urlPath)).WithMethod(HttpMethod.Put);
		}

		private InsightlyRequest Post(string urlPath)
		{
			return (new InsightlyRequest(_apiKey, urlPath)).WithMethod(HttpMethod.Post);
		}

		private InsightlyRequest Delete(string urlPath)
		{
			return (new InsightlyRequest(_apiKey, urlPath)).WithMethod(HttpMethod.Delete);
		}

		private InsightlyRequest Request(string urlPath)
		{
			return new InsightlyRequest(_apiKey, urlPath);
		}
	}
}

