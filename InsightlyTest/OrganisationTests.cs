using System.Collections.Generic;
using System.Threading.Tasks;
using InsightlySDK;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using InsightlySDK.Entities;
using Newtonsoft.Json;

namespace InsightlyTest
{
	[TestClass]
	public class OrganisationTests
	{
		private Insightly _insightly;

		[TestInitialize]
		public void Setup()
		{
			const string insightlyApiKey = "9f66fa80-48d0-461d-b18e-ee1aaf3c4925";

			_insightly = new Insightly(insightlyApiKey);
		}

		[TestMethod]
		public async Task TestGetOrganisations()
		{
			List<Organisation> organisations = await _insightly.GetOrganizationsAsync();
			Approvals.Verify(JsonConvert.SerializeObject(organisations, Formatting.Indented));
		}

		[TestMethod]
		public async Task TestGetOrganisation()
		{
			Organisation organisation = await _insightly.GetOrganizationAsync(102917271);
			Approvals.Verify(JsonConvert.SerializeObject(organisation, Formatting.Indented));
		}
	}
}
