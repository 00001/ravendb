//-----------------------------------------------------------------------
// <copyright file="IIndexingStorageActions.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Raven.Abstractions.Data;
using Raven.Database.Data;

namespace Raven.Database.Storage
{
	public interface IIndexingStorageActions : IDisposable
	{
		void SetCurrentIndexStatsTo(string index);

		void IncrementIndexingAttempt();
		void IncrementSuccessIndexing();
		void IncrementIndexingFailure();
		void DecrementIndexingAttempt();

		void IncrementReduceIndexingAttempt();
		void IncrementReduceSuccessIndexing();
		void IncrementReduceIndexingFailure();
		void DecrementReduceIndexingAttempt();

		IEnumerable<IndexStats> GetIndexesStats();
		void AddIndex(string name, bool createMapReduce);
		void DeleteIndex(string name);


		IndexFailureInformation GetFailureRate(string index);

		void UpdateLastIndexed(string index, Guid etag, DateTime timestamp);
		void UpdateLastReduced(string index, Guid etag, DateTime timestamp);
		void TouchIndexEtag(string index);
	}
}
