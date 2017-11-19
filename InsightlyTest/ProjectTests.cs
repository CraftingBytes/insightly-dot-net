using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InsightlySDK;
using InsightlySDK.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace InsightlyTest
{
	[TestClass]
	public class ProjectTests
	{
		private Insightly _insightly;

		[TestInitialize]
		public void Setup()
		{
			const string insightlyApiKey = "9f66fa80-48d0-461d-b18e-ee1aaf3c4925";

			_insightly = new Insightly(insightlyApiKey);
		}

		[TestMethod]
		public async Task TestGetProjects()
		{
			List<Project> projects = await _insightly.GetProjectsAsync();
			Approvals.Verify(JsonConvert.SerializeObject(projects, Formatting.Indented));
		}

		[TestMethod]
		public async Task TestGetProject()
		{
			Project project = await _insightly.GetProjectAsync(4369591);
			Approvals.Verify(JsonConvert.SerializeObject(project, Formatting.Indented));
		}
	}
}
