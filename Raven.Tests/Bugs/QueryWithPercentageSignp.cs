//-----------------------------------------------------------------------
// <copyright file="QueryWithPercentageSignp.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Linq;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Database.Extensions;
using Raven.Database.Indexing;
using Raven.Database.Server;
using Xunit;

namespace Raven.Tests.Bugs
{
	public class QueryWithPercentageSignp : RemoteClientTest, IDisposable
	{
		private readonly string path;
		private readonly int port;

		public QueryWithPercentageSignp()
		{
			port = 8079;
			path = GetPath("TestDb");
			NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(8079);
		}

		#region IDisposable Members

		public void Dispose()
		{
			IOExtensions.DeleteDirectory(path);
		}

		#endregion

		[Fact]
		public void CanQueryUsingPercentageSign()
		{
			using (var server = GetNewServer(port, path))
			{
				var store = new DocumentStore {Url = "http://localhost:8079"}.Initialize();

				store.DatabaseCommands.PutIndex("Tags/Count",
												new IndexDefinition
												{
													Map = "from tag in docs.Tags select new { tag.Name, tag.UserId }"
												});

				using(var session = store.OpenSession())
				{
					var userId = "users/1";
					var tag = "24%";
					session.Query<TagCount>("Tags/Count").FirstOrDefault(x => x.Name == tag && x.UserId == userId);
				}
			}
		}

		public class TagCount
		{
			public string Name { get; set; }
			public string UserId { get; set; }
		}
	}
}
