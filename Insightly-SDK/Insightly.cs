using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace InsightlySDK{
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
	public class Insightly{
		static string version = "v2.2";
		/// <summary>
		/// Initializes a new instance of the <see cref="InsightlySDK.Insightly"/> class.
		/// </summary>
		/// <param name='api_key'>
		/// The API key for the account to be accessed.
		/// </param>
		public Insightly(String api_key){
			this.api_key = api_key;
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
		public JArray GetComments(int id){

			return this.Get($"/{version}/Comments/" + id).AsJson<JArray>();
		}
		
		/// <summary>
		/// Delete a comment.
		/// </summary>
		/// <param name='id'>
		/// ID of object to delete
		/// </param>
		public void DeleteComment(int id){
			this.Delete($"/{version}/Comments/" + id).AsString();
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
		/// <param name='owner_user_id'>
		/// Owner's user id
		/// </param>
		/// <param name='comment_id'>
		/// Optional comment id (provide this if updating an existing comment)
		/// </param>
		/// <exception cref='ArgumentException'>
		/// Thrown if body is null or zero-length.
		/// </exception>
		public JObject UpdateComment(string body, int owner_user_id, int? comment_id=null){
			if((body == null) || (body.Length < 1)){
				throw new ArgumentException("Comment body cannot be empty.");
			}
			
			JObject data = new JObject();
			data["BODY"] = body;
			data["OWNER_USER_ID"] = owner_user_id;
			if(comment_id != null){
				data["COMMENT_ID"] = comment_id;
			}
			
			return this.Put($"/{version}/Comments").WithBody(data).AsJson<JObject>();
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
		/// <param name='order_by'>
		/// Name of field(s) by which to order the result set.
		/// </param>
		public JArray GetContacts(List<int> ids=null, string email=null, string tag=null,
		                          List<string> filters=null, int? top=null, int? skip=null, string order_by=null){
			var request = this.Get($"/{version}/Contacts");
			BuildODataQuery(request, filters: filters, top: top, skip: skip, order_by: order_by);
			if(ids != null){
				request.WithQueryParam("ids", ids);
			}
			if(email != null){
				request.WithQueryParam("email", email);
			}
			if(tag != null){
				request.WithQueryParam("tag", tag);
			}
			return request.AsJson<JArray>();
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
		public JObject GetContact(int id){
			return this.Get($"/{version}/Contacts/" + id).AsJson<JObject>();
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
		public JObject AddContact(JObject contact){
			var request = this.Request($"/{version}/Contacts");
			
			if((contact["CONTACT_ID"] != null) && (contact["CONTACT_ID"].Value<int>() > 0)){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(contact).AsJson<JObject>();
		}
		
		/// <summary>
		/// Deletes a contact, identified by its id.
		/// </summary>
		/// <param name='id'>
		/// CONTACT_ID of the contact to be deleted.
		/// </param>
		public void DeleteContact(int id){
			this.Delete($"/{version}/Contacts/" + id).AsString();
		}

		/// <summary>
		/// Get emails for a contact.
		/// </summary>
		/// <returns>
		/// Emails belonging to specified contact.
		/// </returns>
		/// <param name='contact_id'>
		/// A contact's CONTACT_ID
		/// </param>
		public JArray GetContactEmails(int contact_id){
			return this.Get($"/{version}/Contacts/" + contact_id + "/Emails")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get notes for a contact.
		/// </summary>
		/// <returns>
		/// Notes belonging to specified contact.
		/// </returns>
		/// <param name='contact_id'>
		/// A contact's CONTACT_ID.
		/// </param>
		public JArray GetContactNotes(int contact_id){
			return this.Get($"/{version}/Contacts/" + contact_id + "/Notes")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get tasks for a contact.
		/// </summary>
		/// <returns>
		/// Tasks belonging to contact.
		/// </returns>
		/// <param name='contact_id'>
		/// A contact's CONTACT_ID.
		/// </param>
		public JArray GetContactTasks(int contact_id){
			return this.Get($"/{version}/Contacts/" + contact_id + "/Tasks")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get a list of countries recognized by Insightly.
		/// </summary>
		/// <returns>
		/// The countries recognized by Insightly.
		/// </returns>
		public JArray GetCountries(){
			return this.Get($"/{version}/Countries").AsJson<JArray>();
		}
		
		/// <summary>
		/// Get the currencies recognized by Insightly
		/// </summary>
		/// <returns>
		/// The currencies recognized by Insightly.
		/// </returns>
		public JArray GetCurrencies(){
			return this.Get($"/{version}/Currencies").AsJson<JArray>();
		}
		
		/// <summary>
		/// Gets a list of custom fields.
		/// </summary>
		/// <returns>
		/// The custom fields.
		/// </returns>
		public JArray GetCustomFields(){
			return this.Get ($"/{version}/CustomFields").AsJson<JArray>();
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
		public JObject GetCustomField(int id){
			return this.Get ($"/{version}/CustomFields/" + id).AsJson<JObject>();
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
		/// <param name='order_by'>
		/// OData orderby parameter.
		/// </param>
		/// <param name='filters'>
		/// OData filters.
		/// </param>
		public JArray GetEmails(int? top=null, int? skip=null,
		                        string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Emails");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			return request.AsJson<JArray>();
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
		public JObject GetEmail(int id){
			return this.Get($"/{version}/Emails/" + id).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete an email.
		/// </summary>
		/// <param name='id'>
		/// ID of email to delete.
		/// </param>
		public void DeleteEmail(int id){
			this.Delete($"/{version}/Emails/" + id).AsString();
		}
		
		/// <summary>
		/// Get comments for an email
		/// </summary>
		/// <returns>
		/// List of email comments
		/// </returns>
		/// <param name='email_id'>
		/// Email id.
		/// </param>
		public JArray GetEmailComments(int email_id){
			return this.Get ($"/{version}/Emails/" + email_id + "/Comments").AsJson<JArray>();
		}
		
		/// <summary>
		/// Add a comment to an existing email.
		/// </summary>
		/// <returns>
		/// The comment, as returned by the server.
		/// </returns>
		/// <param name='email_id'>
		/// ID of email to which the comment will be added.
		/// </param>
		/// <param name='body'>
		/// Comment body.
		/// </param>
		/// <param name='owner_user_id'>
		/// Owner's user id.
		/// </param>
		public JObject AddCommentToEmail(int email_id, string body, int owner_user_id){
			var data = new JObject();
			data["BODY"] = body;
			data["OWNER_USER_ID"] = owner_user_id;
			return this.Post($"/{version}/Emails/" + email_id + "/Comments")
				.WithBody(data).AsJson<JObject>();
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
		/// <param name='order_by'>
		/// If provided, specified field(s) to order results on.
		/// </param>
		/// <param name='filters'>
		/// If provided, specifies a list of OData filters.
		/// </param>
		public JArray GetEvents(int? top=null, int? skip=null,
		                        string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Events");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			return request.AsJson<JArray>();
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
		public JObject GetEvent(int id){
			return this.Get($"/{version}/Events/" + id).AsJson<JObject>();
		}
		
		/// <summary>
		/// Add/update an event in the calendar.
		/// </summary>
		/// <returns>
		/// The new/updated event, as returned by the server.
		/// </returns>
		/// <param name='the_event'>
		/// The event to add/update.
		/// </param>
		public JObject AddEvent(JObject the_event){
			var request = Request($"/{version}/Events");
			
			if((the_event["EVENT_ID"] != null) && (the_event["EVENT_ID"].Value<int>() > 0)){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(the_event).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete an event.
		/// </summary>
		/// <param name='id'>
		/// ID of the event to be deleted.
		/// </param>
		public void DeleteEvent(int id){
			this.Delete($"/{version}/Events/" + id).AsString();
		}

		/// <summary>
		/// Get file categories.
		/// </summary>
		/// <returns>
		/// The file categories for this account.
		/// </returns>
		public JArray GetFileCategories(){
			return this.Get($"/{version}/FileCategories").AsJson<JArray>();
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
		public JObject GetFileCategory(int id){
			return this.Get($"/{version}/FileCategories/" + id).AsJson<JObject>();
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
		public JObject AddFileCategory(JObject category){
			var request = this.Request($"/{version}/FileCategories");
			
			if((category["CATEGORY_ID"] != null) && (category["CATEGORY_ID"].Value<int>() > 0)){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(category).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a file category.
		/// </summary>
		/// <param name='id'>
		/// CATEGORY_ID of the file category to delete.
		/// </param>
		public void DeleteFileCategory(int id){
			this.Delete($"/{version}/FileCategories/" + id).AsString();
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
		/// <param name='order_by'>
		/// Order notes by specified field(s)
		/// </param>
		/// <param name='filters'>
		/// List of OData filters to apply to results.
		/// </param>
		public JArray GetNotes(int? top=null, int? skip=null,
		                       string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Notes");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			return request.AsJson<JArray>();
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
		public JObject GetNote(int id){
			return this.Get($"/{version}/Notes/" + id).AsJson<JObject>();
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
		public JObject AddNote(JObject note){
			var request = this.Request($"/{version}/Notes");
			
			if(IsValidId(note["NOTE_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(note).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a note.
		/// </summary>
		/// <param name='id'>
		/// NOTE_ID of note to delete.
		/// </param>
		public void DeleteNote(int id){
			this.Delete($"/{version}/Notes/" + id).AsString();
		}
		
		/// <summary>
		/// Get comments attached to a note.
		/// </summary>
		/// <returns>
		/// The note comments.
		/// </returns>
		/// <param name='note_id'>
		/// <c>NOTE_ID</c> of desired note.
		/// </param>
		public JObject GetNoteComments(int note_id){
			return this.Get($"/{version}/Notes/" + note_id + "/Comments")
				.AsJson<JObject>();
		}
		
		/// <summary>
		/// Attach a comment to a note.
		/// </summary>
		/// <returns>
		/// Result from server.
		/// </returns>
		/// <param name='note_id'>
		/// <c>NOTE_ID</c> of note to which comment will be attached.
		/// </param>
		/// <param name='comment'>
		/// The comment to attach to the note.
		/// </param>
		public JObject AddNoteComment(int note_id, JObject comment){
			return this.Post($"/{version}/Notes/" + note_id + "/Comments")
				.WithBody(comment).AsJson<JObject>();
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
		/// <param name='order_by'>
		/// Order opportunities by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filters to apply to query.
		/// </param>
		public JArray GetOpportunities(int? top=null, int? skip=null,
		                               string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Opportunities");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			return request.AsJson<JArray>();
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
		public JObject GetOpportunity(int id){
			return this.Get($"/{version}/Opportunities/" + id).AsJson<JObject>();
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
		public JObject AddOpportunity(JObject opportunity){
			var request = this.Request($"/{version}/Opportunities");
			
			if(IsValidId(opportunity["OPPORTUNITY_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(opportunity).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete an opportunity.
		/// </summary>
		/// <param name='id'>
		/// OPPORTUNITY_ID of opportunity to delete.
		/// </param>
		public void DeleteOpportunity(int id){
			this.Delete($"/{version}/Opportunities/" + id).AsString();
		}
		
		/// <summary>
		/// Get history of states and reasons for an opportunity.
		/// </summary>
		/// <returns>
		/// History of states and reasons associated with specified opportunity.
		/// </returns>
		/// <param name='opportunity_id'>
		/// OPPORTUNITY_ID of opportunity to get history of.
		/// </param>
		public JArray GetOpportunityStateHistory(int opportunity_id){
			return this.Get($"/{version}/Opportunities/" + opportunity_id + "/StateHistory")
				.AsJson<JArray>();
		}

		/// <summary>
		/// Get the emails linked to an opportunity.
		/// </summary>
		/// <returns>
		/// The emails associated with specified opportunity.
		/// </returns>
		/// <param name='opportunity_id'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public JArray GetOpportunityEmails(int opportunity_id){
			return this.Get($"/{version}/Opportunities/" + opportunity_id + "/Emails")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get notes linked to an opportunity
		/// </summary>
		/// <returns>
		/// The notes associated with specified opportunity.
		/// </returns>
		/// <param name='opportunity_id'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public JArray GetOpportunityNotes(int opportunity_id){
			return this.Get($"/{version}/Opportunities/" + opportunity_id + "/Notes")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get tasks linked to an opportunity.
		/// </summary>
		/// <returns>
		/// The tasks associated with specified opportunity.
		/// </returns>
		/// <param name='opportunity_id'>
		/// OPPORTUNITY_ID of desired opportunity.
		/// </param>
		public JArray GetOpportunityTasks(int opportunity_id){
			return this.Get($"/{version}/Opportunities/" + opportunity_id + "/Tasks")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get a list of opportunity categories.
		/// </summary>
		/// <returns>
		/// The opportunity categories.
		/// </returns>
		public JArray GetOpportunityCategories(){
			return this.Get($"/{version}/OpportunityCategories")
				.AsJson<JArray>();
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
		public JObject GetOpportunityCategoriy(int id){
			return this.Get($"/{version}/OpportunityCategories/" + id)
				.AsJson<JObject>();
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
		public JObject AddOpportunityCategory(JObject category){
			var request = this.Request($"/{version}/OpportunityCategories");
			
			if(IsValidId(category["CATEGORY_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(category).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete an opportunity category.
		/// </summary>
		/// <param name='id'>
		/// CATEGORY_ID of opportunity category to delete.
		/// </param>
		public void DeleteOpportunityCategory(int id){
			this.Delete($"/{version}/OpportunityCategories/" + id).AsString();
		}
		
		/// <summary>
		/// Get a list of opportunity state reasons.
		/// </summary>
		/// <returns>
		/// The opportunity state reasons.
		/// </returns>
		public JArray GetOpportunityStateReasons(){
			return this.Get($"/{version}/OpportunityStateReasons")
				.AsJson<JArray>();
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
		/// <param name='order_by'>
		/// Order results by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filters statements to apply.
		/// </param>
		public JArray GetOrganizations(List<int> ids=null, string domain=null, string tag=null,
		                               int? top=null, int? skip=null, string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Organisations");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			
			if(domain != null){
				request.WithQueryParam("domain", domain);
			}
			if(tag != null){
				request.WithQueryParam("tag", tag);
			}
			if(ids != null){
				request.WithQueryParam("ids", String.Join(",", ids));
			}
			
			return request.AsJson<JArray>();
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
		public JObject GetOrganization(int id){
			return this.Get($"/{version}/Organisations/" + id).AsJson<JObject>();
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
		public JObject AddOrganization(JObject organization){
			var request = this.Request($"/{version}/Organisations");
			
			if(IsValidId(organization["ORGANISATION_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(organization).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete an organization.
		/// </summary>
		/// <param name='id'>
		/// <c>ORGANISATION_ID</c> of organization to delete.
		/// </param>
		public void DeleteOrganization(int id){
			this.Delete($"/{version}/Organisations/" + id).AsString();
		}

		/// <summary>
		/// Get emails attached to an organization.
		/// </summary>
		/// <returns>
		/// The organization's emails.
		/// </returns>
		/// <param name='organization_id'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public JArray GetOrganizationEmails(int organization_id){
			return this.Get($"/{version}/Organisations/" + organization_id + "/Emails")
				.AsJson<JArray>();
		}

		/// <summary>
		/// Get notes attached to an organization.
		/// </summary>
		/// <returns>
		/// The organization's notes.
		/// </returns>
		/// <param name='organization_id'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public JArray GetOrganizationNotes(int organization_id){
			return this.Get($"/{version}/Organisations/" + organization_id + "/Notes")
				.AsJson<JArray>();
		}

		/// <summary>
		/// Get tasks attached to an organization.
		/// </summary>
		/// <returns>
		/// The organization's tasks.
		/// </returns>
		/// <param name='organization_id'>
		/// <c>ORGANISATION_ID</c> of desired organization.
		/// </param>
		public JArray GetOrganizationTasks(int organization_id){
			return this.Get($"/{version}/Organisations/" + organization_id + "/Tasks")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get a list of pipelines.
		/// </summary>
		/// <returns>
		/// The pipelines.
		/// </returns>
		public JArray GetPipelines(){
			return this.Get($"/{version}/Pipelines").AsJson<JArray>();
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
		public JObject GetPipeline(int id){
			return this.Get($"/{version}/Pipelines/" + id).AsJson<JObject>();
		}
		
		/// <summary>
		/// Get the pipeline stages.
		/// </summary>
		/// <returns>
		/// The pipeline stages.
		/// </returns>
		public JArray GetPipelineStages(){
			return this.Get($"/{version}/PipelineStages").AsJson<JArray>();
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
		public JObject GetPipelineStage(int id){
			return this.Get("v2.1/PipelineStages/" + id).AsJson<JObject>();
		}
		
		/// <summary>
		/// Get list project categories.
		/// </summary>
		/// <returns>
		/// The project categories.
		/// </returns>
		public JArray GetProjectCategories(){
			return this.Get($"/{version}/ProjectCategories").AsJson<JArray>();
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
		public JObject GetProjectCategory(int id){
			return this.Get($"/{version}/ProjectCategories/" + id).AsJson<JObject>();
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
		public JObject AddProjectCategory(JObject category){
			var request = this.Request($"/{version}/ProjectCategories");
			
			if(IsValidId(category["CATEGORY_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(category).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a project category.
		/// </summary>
		/// <param name='id'>
		/// <c>CATEGORY_ID</c> of category to be deleted.
		/// </param>
		public void DeleteProjectCategory(int id){
			this.Delete($"/{version}/ProjectCategories/" + id).AsString();
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
		/// <param name='order_by'>
		/// Order results by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filter statements to apply.
		/// </param>
		public JArray GetProjects(int? top=null, int? skip=null,
		                          string order_by = null, List<string> filters=null){
			var request = this.Get($"/{version}/Projects");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			return request.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get a project.
		/// </summary>
		/// <returns>
		/// The project.
		/// </returns>
		/// <param name='id'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public JObject GetProject(int id){
			return this.Get($"/{version}/Projects/" + id).AsJson<JObject>();
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
		public JObject AddProject(JObject project){
			var request = this.Request($"/{version}/Projects");
			
			if(IsValidId(project["PROJECT_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(project).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a project.
		/// </summary>
		/// <param name='id'>
		/// <c>PROJECT_ID</c> of project to be deleted.
		/// </param>
		public void DeleteProject(int id){
			this.Delete($"/{version}/Projects/" + id).AsString();
		}
		
		/// <summary>
		/// Get the emails attached to a project.
		/// </summary>
		/// <returns>
		/// The project's emails.
		/// </returns>
		/// <param name='project_id'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public JArray GetProjectEmails(int project_id){
			return this.Get($"/{version}/Projects/" + project_id + "/Emails")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get the notes attached to a project.
		/// </summary>
		/// <returns>
		/// The project's notes.
		/// </returns>
		/// <param name='project_id'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public JArray GetProjectNotes(int project_id){
			return this.Get($"/{version}/Projects/" + project_id + "/Notes")
				.AsJson<JArray>();
		}

		/// <summary>
		/// Get the tasks attached to a project.
		/// </summary>
		/// <returns>
		/// The project's tasks.
		/// </returns>
		/// <param name='project_id'>
		/// <c>PROJECT_ID</c> of desired project.
		/// </param>
		public JArray GetProjectTasks(int project_id){
			return this.Get($"/{version}/Projects/" + project_id + "/Tasks")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get a list of relationships.
		/// </summary>
		/// <returns>
		/// The relationships.
		/// </returns>
		public JArray GetRelationships(){
			return this.Get($"/{version}/Relationships").AsJson<JArray>();
		}
		
		/// <summary>
		/// Get tags associated with a parent object.
		/// </summary>
		/// <returns>
		/// List of tags.
		/// </returns>
		/// <param name='parent_id'>
		/// The id of the parent object.
		/// </param>
		public JArray GetTags(int parent_id){
			return this.Get($"/{version}/Tags/" + parent_id).AsJson<JArray>();
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
		/// <param name='order_by'>
		/// Orders results by specified field(s)
		/// </param>
		/// <param name='filters'>
		/// List of OData filter statements to apply.
		/// </param>
		public JArray GetTasks(List<int> ids=null, int? top=null, int? skip=null,
		                       string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Tasks");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			
			if((ids != null) && (ids.Count > 0)){
				request.WithQueryParam("ids", String.Join(",", ids));
			}
			
			return request.AsJson<JArray>();
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
		public JObject GetTask(int id){
			return this.Get($"/{version}/Tasks/" + id).AsJson<JObject>();
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
		public JObject AddTask(JObject task){
			var request = this.Request($"/{version}/Tasks");
			
			if(IsValidId(task["TASK_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(task).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a task.
		/// </summary>
		/// <param name='id'>
		/// <c>TASK_ID</c> of task to delete.
		/// </param>
		public void DeleteTask(int id){
			this.Delete($"/{version}/Tasks/" + id).ToString();
		}
		
		
		/// <summary>
		/// Get comments attached to a task.
		/// </summary>
		/// <returns>
		/// The task's comments.
		/// </returns>
		/// <param name='task_id'>
		/// <c>TASK_ID</c> of desired task.
		/// </param>
		public JArray GetTaskComments(int task_id){
			return this.Get($"/{version}/Tasks/" + task_id + "/Comments").AsJson<JArray>();
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
		/// <param name='task_id'>
		/// <c>TASK_ID</c> of task to which the comment will be added.
		/// </param>
		/// <param name='comment'>
		/// The comment to add.
		/// </param>
		public JObject AddTaskComment(int task_id, JObject comment){
			return this.Post($"/{version}/Tasks/" + task_id + "/Comments").WithBody(comment).AsJson<JObject>();
		}
		
		/// <summary>
		/// Get members of a team.
		/// </summary>
		/// <returns>
		/// The team's members.
		/// </returns>
		/// <param name='team_id'>
		/// <c>TEAM_ID</c> of desired team.
		/// </param>
		public JArray GetTeamMembers(int team_id){
			return this.Get($"/{version}/TeamMembers/teamid=" + team_id).AsJson<JArray>();
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
		public JObject GetTeamMember(int id){
			return this.Get($"/{version}/TeamMembers/" + id).AsJson<JObject>();
		}
		
		/// <summary>
		/// Add a team member.
		/// </summary>
		/// <returns>
		/// The new team member, as returned by the server.
		/// </returns>
		/// <param name='team_member'>
		/// The team member to add.
		/// </param>
		public JObject AddTeamMember(JObject team_member){
			return this.Post($"/{version}/TeamMembers").WithBody(team_member).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a team member.
		/// </summary>
		/// <param name='id'>
		/// Id of team member to be deleted.
		/// </param>
		public void DeleteTeamMember(int id){
			this.Delete($"/{version}/TeamMembers/" + id).AsString();
		}
		
		/// <summary>
		/// Update a team member.
		/// </summary>
		/// <returns>
		/// The updated team member, as returned by the server.
		/// </returns>
		/// <param name='team_member'>
		/// The team member to update.
		/// </param>
		public JObject UpdateTeamMember(JObject team_member){
			return this.Put($"/{version}/TeamMembers").WithBody(team_member).AsJson<JObject>();
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
		/// <param name='order_by'>
		/// Order teams by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// List of OData filter statements to apply.
		/// </param>/
		public JArray GetTeams(int? top=null, int? skip=null,
		                       string order_by=null, List<string> filters=null){
			var request = this.Get($"/{version}/Teams");
			BuildODataQuery(request, top: top, skip: skip, order_by: order_by, filters: filters);
			return request.AsJson<JArray>();
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
		public JObject GetTeam(int id){
			return this.Get($"/{version}/Teams/" + id).AsJson<JObject>();
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
		public JObject AddTeam(JObject team){
			var request = this.Request($"/{version}/Teams");
			
			if(IsValidId(team["TEAM_ID"])){
				request.WithMethod(HTTPMethod.PUT);
			}
			else{
				request.WithMethod(HTTPMethod.POST);
			}
			
			return request.WithBody(team).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete a team.
		/// </summary>
		/// <param name='id'>
		/// <c>TEAM_ID</c> of team to delete.
		/// </param>
		public void DeleteTeam(int id){
			this.Delete($"/{version}/Teams/" + id).AsString();
		}
		
		/// <summary>
		/// Get a list of users for this account.
		/// </summary>
		/// <returns>
		/// This account's users.
		/// </returns>
		public JArray GetUsers(){
			return this.Get ($"/{version}/Users/").AsJson<JArray>();
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
		public JObject GetUser(int id){
			return this.Get($"/{version}/Users/" + id).AsJson<JObject>();
		}
		
		public void Test(int? top=null){
			Console.WriteLine("Testing API .....");
			
			Console.WriteLine("Testing authentication");
			
			int passed = 0;
			int failed = 0;
			
			var currencies = this.GetCurrencies();
			if(currencies.Count > 0){
				Console.WriteLine("Authentication passed...");
				passed += 1;
			}
			else{
				failed += 1;
			}
			
			int user_id = 0;
			try{
				var users = this.GetUsers();
				JObject user = users[0].Value<JObject>();
				user_id = user["USER_ID"].Value<int>();
				Console.WriteLine("PASS: GetUsers, found " + users.Count + " users.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetUsers()");
				failed += 1;
			}
			
			// Test GetContacts()
			try{
				var contacts = this.GetContacts(order_by: "DATE_UPDATED_UTC desc", top: top);
				Console.WriteLine("PASS: GetContacts(), found " + contacts.Count + " contacts.");
				passed += 1;
				
				if(contacts.Count > 0){
					JObject contact = contacts[0].Value<JObject>();
					int contact_id = contact["CONTACT_ID"].Value<int>();
					
					// Test GetContactEmails()
					try{
						var emails = this.GetContactEmails(contact_id);
						Console.WriteLine("PASS: GetContactEmails(), found " + emails.Count + " emails.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetContactEmails()");
						failed += 1;
					}
					
					// Test GetContactNotes()
					try{
						var notes = this.GetContactNotes(contact_id);
						Console.WriteLine("PASS: GetContactNotes(), found " + notes.Count + " notes.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetContactNotes()");
						failed += 1;
					}

					// Test GetContactTasks()
					try{
						var tasks = this.GetContactTasks(contact_id);
						Console.WriteLine("PASS: GetContactTasks(), found " + tasks.Count + " tasks.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetContactTasks()");
						failed += 1;
					}
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetContacts()");
				failed += 1;
			}
            
			// Test AddContact()
			try{
				var contact = new JObject();
				contact["SALUTATION"] = "Mr";
				contact["FIRST_NAME"] = "Testy";
				contact["LAST_NAME"] = "McTesterson";

                var contactInfos = new JArray();
                var email = new JObject();
                email["TYPE"] = "EMAIL";
                email["LABEL"] = "Personal";
                email["DETAIL"] = "test@example.com";
                contactInfos.Add(email);
                contact["CONTACTINFOS"] = contactInfos;

                contact = this.AddContact(contact);
				Console.WriteLine("PASS: AddContact()");
				passed += 1;

                // Test GetContacts() by email
                try
                {
                    var contacts = this.GetContacts(null, "test@example.com");

                    if(contacts.Count != 1)
                    {
                        throw new Exception();
                    }

                    Console.WriteLine("PASS: GetContacts() by email, found " + contacts.Count + " contacts.");
                    passed += 1;
                }
                catch (Exception){
                    Console.WriteLine("FAIL: GetContacts() by email");
                    failed += 1;
                }

                // Test DeleteContact()
                try
                {
					this.DeleteContact(contact["CONTACT_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteContact()");
					passed += 1;
				}
				catch(Exception){
					Console.WriteLine("FAIL: DeleteContact()");
					failed += 1;
				}
			}
			catch(Exception ex){
				Console.WriteLine("FAIL: AddContact()");
				failed += 1;
				throw ex;
			}
			
			// Test GetCountries()
			try{
				var countries = this.GetCountries();
				Console.WriteLine("PASS: GetCountries(), found " + countries.Count + " countries.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetCountries()");
				failed += 1;
			}
			
			// Test GetCurrencies()
			try{
				currencies = this.GetCurrencies();
				Console.WriteLine("PASS: GetCurrencies(), found " + currencies.Count + " currencies.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetCurrencies()");
				failed += 1;
			}
			
			// Test GetCustomFields()
			try{
				var custom_fields = this.GetCustomFields();
				Console.WriteLine("PASS: GetCustomFields(), found " + custom_fields.Count + " custom fields.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine ("FAIL: GetCustomFields()");
				failed += 1;
			}
			
			// Test GetEmails()
			try{
				var emails = this.GetEmails();
				Console.WriteLine("PASS: GetEmails(), found " + emails.Count + " emails.");
				passed += 1;
			}
			catch{
				Console.WriteLine("FAIL: GetEmails()");
				failed += 1;
			}
			
			// Test GetEvents()
			try{
				var events = this.GetEvents(top: top);
				Console.WriteLine("PASS: GetEvents(), found " + events.Count + " events.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetEvents()");
				failed += 1;
			}
			
			// Test AddEvent()
			try{
				var _event = new JObject();
				_event["TITLE"] = "Test Event";
				_event["LOCATION"] = "Somewhere";
				_event["DETAILS"] = "Details";
				_event["START_DATE_UTC"] = "2014-07-12 12:00:00";
				_event["END_DATE_UTC"] = "2014-07-12 13:00:00";
				_event["OWNER_USER_ID"] = user_id;
				_event["ALL_DAY"] = false;
				_event["PUBLICLY_VISIBLE"] = true;
				_event = this.AddEvent(_event);
				Console.WriteLine("PASS: AddEvent");
				passed += 1;
				
				// Test DeleteEvent()
				try{
					this.DeleteEvent(_event["EVENT_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteEvent()");
					passed += 1;
				}
				catch(Exception){
					Console.WriteLine("FAIL: DeleteEvent()");
					failed += 1;
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: AddEvent");
				failed += 1;
			}
			
			// Test GetFileCategories()
			try{
				var categories = this.GetFileCategories();
				Console.WriteLine("PASS: GetFileCategories(), found " + categories.Count + " categories.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetFileCategories()");
				failed += 1;
			}
			
			// Test AddFileCategory()
			try{
				var category = new JObject();
				category["CATEGORY_NAME"] = "Test Category";
				category["ACTIVE"] = true;
				category["BACKGROUND_COLOR"] = "000000";
				category = this.AddFileCategory(category);
				Console.WriteLine("PASS: AddFileCategory()");
				passed += 1;
				
				// Test DeleteFileCategory()
				try{
					var category_id = category["CATEGORY_ID"].Value<int>();
					this.DeleteFileCategory(category_id);
					Console.WriteLine("PASS: DeleteFileCategory()");
					passed += 1;
				}
				catch(Exception){
					Console.WriteLine("FAIL: DeleteFileCategory()");
					failed += 1;
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: AddFileCategory()");
				failed += 1;
			}
			
			// Test GetNotes()
			try{
				var notes = this.GetNotes();
				Console.WriteLine("PASS: GetNotes(), found " + notes.Count + " notes.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetNotes()");
				failed += 1;
			}
			
			// Test GetOpportunities
			try{
				var opportunities = this.GetOpportunities(order_by: "DATE_UPDATED_UTC desc", top: top);
				Console.WriteLine("PASS: GetOpportunities(), found " + opportunities.Count + " opportunities.");
				passed += 1;
				
				if(opportunities.Count > 0){
					var opportunity = opportunities[0];
					int opportunity_id = opportunity["OPPORTUNITY_ID"].Value<int>();
					
					// Test GetOpportunityEmails()
					try{
						var emails = this.GetOpportunityEmails(opportunity_id);
						Console.WriteLine("PASS: GetOpportunityEmails(), found " + emails.Count + " emails.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOpportunityEmails()");
						failed += 1;
					}
					
					// Test GetOpportunityNotes()
					try{
						var notes = this.GetOpportunityNotes(opportunity_id);
						Console.WriteLine("PASS: GetOpportunityNotes(), found " + notes.Count + " notes.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOpportunityNotes()");
						failed += 1;
					}
					
					// Test GetOpportunityTasks()
					try{
						var tasks = this.GetOpportunityTasks(opportunity_id);
						Console.WriteLine("PASS: GetOpportunityTasks(), found " + tasks.Count + " tasks.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOpportunityTasks()");
						failed += 1;
					}
					
					// Test GetOpportunityStateHistory()
					try{
						var history = this.GetOpportunityStateHistory(opportunity_id);
						Console.WriteLine("PASS: GetOpportunityStateHistory(), found " + history.Count + " states in history.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOpportunityStateHistory()");
						failed += 1;
					}
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetOpportunities()");
				failed += 1;
			}
			
			// Test GetOpportunityCategories()
			try{
				var categories = this.GetOpportunityCategories();
				Console.WriteLine("PASS: GetOpportunityCategories(), found " + categories.Count + " categories.");
				passed += 1;				
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetOpportunityCategories()");
				failed += 1;
			}
			
			// Test AddOpportunityCategory()
			try{
				var category = new JObject();
				category["CATEGORY_NAME"] = "Test Category";
				category["ACTIVE"] = true;
				category["BACKGROUND_COLOR"] = "000000";
				category = this.AddFileCategory(category);
				Console.WriteLine("PASS: AddOpportuntityCategory()");
				passed += 1;
				
				// Test DeleteOpportunityCategory()
				try{
					this.DeleteOpportunityCategory(category["CATEGORY_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteOpportunityCategory()");
					passed += 1;
				}
				catch(Exception){
					Console.WriteLine("FAIL: DeleteOpportunityCategory()");
					failed += 1;
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: AddOpportunityCategory()");
				failed += 1;
			}
			
			// Test GetOpportunityStateReasons()
			try{
				var reasons = this.GetOpportunityStateReasons();
				Console.WriteLine("PASS: GetOpportunityStateReasons(), found " + reasons.Count + " reasons.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetOpportunityStateReasons()");
				failed += 1;
			}
			
			// Test GetOrganizations()
			try{
				var organizations = this.GetOrganizations(top: top, order_by: "DATE_UPDATED_UTC desc");
				Console.WriteLine("PASS: GetOrganizations(), found " + organizations.Count + " organizations.");
				passed += 1;
				
				if(organizations.Count > 0){
					var organization = organizations[0];
					int organization_id = organization["ORGANISATION_ID"].Value<int>();
					
					// Test GetOrganizationEmails();
					try{
						var emails = this.GetOrganizationEmails(organization_id);
						Console.WriteLine("PASS: GetOrganizationEmails(), found " + emails.Count + " emails.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOrganizationEmails()");
						failed += 1;
					}
					
					// Test GetOrganizationNotes();
					try{
						var notes = this.GetOrganizationNotes(organization_id);
						Console.WriteLine("PASS: GetOrganizationNotes(), found " + notes.Count + " notes.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOrganizationNotes()");
						failed += 1;
					}
					
					// Test GetOrganizationTasks();
					try{
						var tasks = this.GetOrganizationTasks(organization_id);
						Console.WriteLine("PASS: GetOrganizationTasks(), found " + tasks.Count + " tasks.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetOrganizationTasks()");
						failed += 1;
					}
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetOrganizations()");
				failed += 1;
			}
			
			// Test AddOrganization
			try{
				var organization = new JObject();
				organization["ORGANISATION_NAME"] = "Foo Corp";
				organization["BACKGROUND"] = "Details";
				organization = this.AddOrganization(organization);
				Console.WriteLine("PASS: AddOrganization()");
				passed += 1;
				
				// Test DeleteOrganization()
				try{
					this.DeleteOrganization(organization["ORGANISATION_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteOrganization()");
					passed += 1;
				}
				catch(Exception){
					Console.WriteLine("FAIL: DeleteOrganization()");
					failed += 1;
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: AddOrganization()");
				failed += 1;
			}
			
			// Test GetPipelines()
			try{
				var pipelines = this.GetPipelines();
				Console.WriteLine("PASS: GetPipelines(), found " + pipelines.Count + " pipelines.");
				passed += 1;
			}catch(Exception){
				Console.WriteLine("FAIL: GetPipelines()");
				failed += 1;
			}
			
			// Test GetPipelineStages()
			try{
				var stages = this.GetPipelineStages();
				Console.WriteLine("PASS: GetPipelineStages(), found " + stages.Count + " pipeline stages.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetPipelineStages()");
				failed += 1;
			}
			
			// Test GetProjects()
			try{
				var projects = this.GetProjects(top: top, order_by: "DATE_UPDATED_UTC desc");
				Console.WriteLine("PASS: GetProjects(), found " + projects.Count + " projects.");
				passed += 1;
				
				if(projects.Count > 0){
					var project = projects[0];
					int project_id = project["PROJECT_ID"].Value<int>();
					
					// Test GetProjectEmails
					try{
						var emails = this.GetProjectEmails(project_id);
						Console.WriteLine("PASS: GetProjectEmails(), found " + emails.Count + " emails.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetProjectEmails()");
						failed += 1;
					}
					
					// Test GetProjectNotes
					try{
						var notes = this.GetProjectNotes(project_id);
						Console.WriteLine("PASS: GetProjectNotes(), found " + notes.Count + " notes.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetProjectNotes()");
						failed += 1;
					}
					
					// Test GetProjectTasks
					try{
						var emails = this.GetProjectTasks(project_id);
						Console.WriteLine("PASS: GetProjectTasks(), found " + emails.Count + " tasks.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetProjectTasks()");
						failed += 1;
					}
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetProjects()");
				failed += 1;
			}
			
			// Test GetProjectCategories
			try{
				var categories = this.GetProjectCategories();
				Console.WriteLine("PASS: GetProjectCategories(), found " + categories.Count + " categories.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetProjectCategories()");
				failed += 1;
			}
			
			// Test AddProjectCategory()
			try{
				var category = new JObject();
				category["CATEGORY_NAME"] = "Test Category";
				category["ACTIVE"] = true;
				category["BACKGROUND_COLOR"] = "000000";
				category = this.AddProjectCategory(category);
				Console.WriteLine("PASS: AddProjectCategory()");
				passed += 1;
				
				// Test DeleteProjectCategory()
				try{
					this.DeleteProjectCategory(category["CATEGORY_ID"].Value<int>());
					Console.WriteLine("PASS: DeleteProjectCategory()");
					passed += 1;
				}
				catch(Exception){
					Console.WriteLine("FAIL: DeleteProjectCategory()");
					failed += 1;
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: AddProjectCategory()");
				failed += 1;
			}
			
			// Test GetRelationships()
			try{
				var relationships = this.GetRelationships();
				Console.WriteLine("PASS: getRelationships(), found " + relationships.Count + " relationships.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetTasks()");
				failed += 1;
			}
			
			// Test GetTasks()
			try{
				var tasks = this.GetTasks(top: top, order_by: "DUE_DATE desc");
				Console.WriteLine("PASS: GetTasks(), found " + tasks.Count + " tasks.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetTasks()");
				failed += 1;
			}
			
			// Test GetTeams
			try{
				var teams = this.GetTeams();
				Console.WriteLine("PASS: GetTeams(), found " + teams.Count + " teams.");
				passed += 1;

				if(teams.Count > 0){
					var team = teams[0];
					
					// Test GetTeamMembers
					try{
						var team_members = this.GetTeamMembers(team["TEAM_ID"].Value<int>());
						Console.WriteLine("PASS: GetTeamMembers(), found " + team_members.Count + " team members.");
						passed += 1;
					}
					catch(Exception){
						Console.WriteLine("FAIL: GetTeamMembers()");
						failed += 1;
					}
				}
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetTeams()");
				failed += 1;
			}
			
			if(failed > 0){
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
		/// <param name='order_by'>
		/// (Optional) If provided, results will be order by specified field(s).
		/// </param>
		/// <param name='filters'>
		/// A list of filters to apply.
		/// </param>
		private InsightlyRequest BuildODataQuery(InsightlyRequest request, int? top=null, int? skip=null,
		                                         string order_by=null, List<string> filters=null){
			if(top != null){
				request.Top(top.Value);
			}
			if(skip != null){
				request.Skip(skip.Value);
			}
			if(order_by != null){
				request.OrderBy(order_by);
			}
			if(filters != null){
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
		private bool IsValidId(JToken id){
			return ((id != null) && (id.Value<int>() > 0));
		}
		
		private InsightlyRequest Get(string url_path){
			return new InsightlyRequest(this.api_key, url_path);
		}
		
		private InsightlyRequest Put(string url_path){
			return (new InsightlyRequest(this.api_key, url_path)).WithMethod(HTTPMethod.PUT);
		}
		
		private InsightlyRequest Post(string url_path){
			return (new InsightlyRequest(this.api_key, url_path)).WithMethod(HTTPMethod.POST);
		}

		private InsightlyRequest Delete(string url_path){
			return (new InsightlyRequest(this.api_key, url_path)).WithMethod(HTTPMethod.DELETE);
		}
		
		private InsightlyRequest Request(string url_path){
			return new InsightlyRequest(this.api_key, url_path);
		}

		private String api_key;
	}
}

