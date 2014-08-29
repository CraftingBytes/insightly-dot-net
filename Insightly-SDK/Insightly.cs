using System;
using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace InsightlySDK{
	public class Insightly{
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
			return this.Get("/v2.1/Comments/" + id).AsJson<JArray>();
		}
		
		/// <summary>
		/// Delete a comment.
		/// </summary>
		/// <param name='id'>
		/// ID of object to delete
		/// </param>
		public void DeleteComment(int id){
			this.Delete("/v2.1/Comments/" + id).AsString();
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
			
			return this.Put("/v2.1/Comments").WithBody(data).AsJson<JObject>();
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
			var request = this.Get("/v2.1/Contacts");
			BuildODataQuery(request, filters: filters, top: top, skip: skip, order_by: order_by);
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
			return this.Get("/v2.1/Contacts/" + id).AsJson<JObject>();
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
			var request = this.Request("/v2.1/Contacts");
			
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
			this.Delete("/v2.1/Contacts/" + id).AsString();
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
			return this.Get("/v2.1/Contacts/" + contact_id + "/Emails")
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
			return this.Get("/v2.1/Contacts/" + contact_id + "/Notes")
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
			return this.Get("/v2.1/Contacts/" + contact_id + "/Tasks")
				.AsJson<JArray>();
		}
		
		/// <summary>
		/// Get a list of countries recognized by Insightly.
		/// </summary>
		/// <returns>
		/// The countries recognized by Insightly.
		/// </returns>
		public JArray GetCountries(){
			return this.Get("/v2.1/Countries").AsJson<JArray>();
		}
		
		/// <summary>
		/// Get the currencies recognized by Insightly
		/// </summary>
		/// <returns>
		/// The currencies recognized by Insightly.
		/// </returns>
		public JArray GetCurrencies(){
			return this.Get("/v2.1/Currencies").AsJson<JArray>();
		}
		
		/// <summary>
		/// Gets a list of custom fields.
		/// </summary>
		/// <returns>
		/// The custom fields.
		/// </returns>
		public JArray GetCustomFields(){
			return this.Get ("/v2.1/CustomFields").AsJson<JArray>();
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
			return this.Get ("/v2.1/CustomFields/" + id).AsJson<JObject>();
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
			var request = this.Get("/v2.1/Emails");
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
			return this.Get("/v2.1/Emails/" + id).AsJson<JObject>();
		}
		
		/// <summary>
		/// Delete an email.
		/// </summary>
		/// <param name='id'>
		/// ID of email to delete.
		/// </param>
		public void DeleteEmail(int id){
			this.Delete("/v2.1/Emails/" + id).AsString();
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
			return this.Get ("/v2.1/Emails/" + email_id + "/Comments").AsJson<JArray>();
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
			return this.Post("/v2.1/Emails/" + email_id + "/Comments")
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
			var request = this.Get("/v2.1/Events");
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
			return this.Get("/v2.1/Events/" + id).AsJson<JObject>();
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
			var request = Request("/v2.1/Events");
			
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
			this.Delete("/v2.1/Events/" + id).AsString();
		}

		/// <summary>
		/// Get file categories.
		/// </summary>
		/// <returns>
		/// The file categories for this account.
		/// </returns>
		public JArray GetFileCategories(){
			return this.Get("/v2.1/FileCategories").AsJson<JArray>();
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
			return this.Get("/v2.1/FileCategories/" + id).AsJson<JObject>();
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
			var request = this.Request("/v2.1/FileCategories");
			
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
			this.Delete("/v2.1/FileCategories/" + id).AsString();
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
			var request = this.Get("/v2.1/Notes");
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
			return this.Get("/v2.1/Notes/" + id).AsJson<JObject>();
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
			var request = this.Request("/v2.1/Notes");
			
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
			this.Delete("/v2.1/Notes/" + id).AsString();
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
			return this.Get("/v2.1/Notes/" + note_id + "/Comments")
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
			return this.Post("/v2.1/Notes/" + note_id + "/Comments")
				.WithBody(comment).AsJson<JObject>();
		}
		
		public JArray GetUsers(){
			return this.Get ("/v2.1/Users/").AsJson<JArray>();
		}
		
		public JObject GetUser(int id){
			return this.Get("/v2.1/Users/" + id).AsJson<JObject>();
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
				contact = this.AddContact(contact);
				Console.WriteLine("PASS: AddContact()");
				passed += 1;
				
				// Test DeleteContact()
				try{
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
			
			try{
				var notes = this.GetNotes();
				Console.WriteLine("PASS: GetNotes(), found " + notes.Count + " notes.");
				passed += 1;
			}
			catch(Exception){
				Console.WriteLine("FAIL: GetNotes()");
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

