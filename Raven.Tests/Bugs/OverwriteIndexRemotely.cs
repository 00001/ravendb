//-----------------------------------------------------------------------
// <copyright file="OverwriteIndexRemotely.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using Raven.Abstractions.Indexing;
using Raven.Client.Document;
using Raven.Database.Extensions;
using Raven.Database.Indexing;
using Raven.Database.Server;
using Xunit;

namespace Raven.Tests.Bugs
{
	public class OverwriteIndexRemotely : RemoteClientTest, IDisposable
	{
		private readonly string path;
		private readonly int port;

		public OverwriteIndexRemotely()
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
		public void CanOverwriteIndex()
		{
			using (var server = GetNewServer(port, path))
			{
				var store = new DocumentStore { Url = "http://localhost:" + port };
				store.Initialize();

				store.DatabaseCommands.PutIndex("test",
												new IndexDefinition
												{
													Map = "from doc in docs select new { doc.Name }"
												}, overwrite: true);


				store.DatabaseCommands.PutIndex("test",
												new IndexDefinition
												{
													Map = "from doc in docs select new { doc.Name }"
												}, overwrite: true);

				store.DatabaseCommands.PutIndex("test",
												new IndexDefinition
												{
													Map = "from doc in docs select new { doc.Email }"
												}, overwrite: true);

				store.DatabaseCommands.PutIndex("test",
												new IndexDefinition
												{
													Map = "from doc in docs select new { doc.Email }"
												}, overwrite: true);
			}
		}
	}
}