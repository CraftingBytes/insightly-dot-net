using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InsightlyTest
{
	public class Approvals
	{
		public static void Verify(string text, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
		{
			string approvedText = null;
			string filePath = BuildApprovalFilePath(callerFilePath, callerMemberName);
			var approvedFilePath = filePath + ".approved.txt";
			var receivedFilePath = filePath + ".received.txt";
			var approvedFileExists = File.Exists(approvedFilePath);
			if (approvedFileExists)
			{
				approvedText = File.ReadAllText(approvedFilePath);
			}
			else
			{
				using (File.CreateText(approvedFilePath))
				{
				}
				approvedText = "";
			}

			if (approvedText != text)
			{
				File.WriteAllText(receivedFilePath, text);
				Process.Start(@"C:\Program Files (x86)\WinMerge\WinMergeU.exe", $"\"{receivedFilePath}\" \"{approvedFilePath}\"");
			}
			else
			{
				if (File.Exists(receivedFilePath))
				{
					File.Delete(receivedFilePath);
				}
			}
			Assert.AreEqual(approvedText, text);
		}

		public static void VerifyAll<T>(IEnumerable<T> enumerable, string itemName, [CallerFilePath] string callerFilePath = null, [CallerMemberName] string callerMemberName = null)
		{
			var builder = new StringBuilder();
			foreach (var itemIndex in enumerable.Select((item, idx) => new {item, idx}))
			{
				builder.AppendLine($"{itemName}[{itemIndex.idx}] = {itemIndex.item}");
			}
			Verify(builder.ToString(), callerFilePath, callerMemberName);
		}

		private static string BuildApprovalFilePath(string callerFilePath, string callerName)
		{
			var directory = Path.GetDirectoryName(callerFilePath);
			return Path.Combine(directory, callerName);
		}
	}
}