//-----------------------------------------------------------------------
// <copyright file="IRavenQueryProvider.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Client.Document;

namespace Raven.Client.Linq
{
	using System.Linq.Expressions;

	/// <summary>
	/// Extension for the built-in <see cref="IQueryProvider"/> allowing for Raven specific operations
	/// </summary>
	public interface IRavenQueryProvider : IQueryProvider
	{
		/// <summary>
		/// Callback to get the results of the query
		/// </summary>
		void AfterQueryExecuted(Action<QueryResult> afterQueryExecuted);

		/// <summary>
		/// Called externally to raise the after query executed callback
		/// </summary>
		void InvokeAfterQueryExecuted(QueryResult result);

		/// <summary>
		/// Customizes the query using the specified action
		/// </summary>
		void Customize(Action<IDocumentQueryCustomization> action);

		/// <summary>
		/// Gets the name of the index.
		/// </summary>
		/// <value>The name of the index.</value>
		string IndexName { get; }

		/// <summary>
		/// Get the query generator
		/// </summary>
		IDocumentQueryGenerator QueryGenerator { get; }

		/// <summary>
		/// Change the result type for the query provider
		/// </summary>
		IRavenQueryProvider For<S>();

#if !NET_3_5
		/// <summary>
		/// Convert the Linq query to a Lucene query
		/// </summary>
		/// <returns></returns>
		IAsyncDocumentQuery<T> ToAsyncLuceneQuery<T>(Expression expression);
#endif

		/// <summary>
		/// Set the fields to fetch
		/// </summary>
		HashSet<string> FieldsToFetch { get; }

#if !NET_3_5
		/// <summary>
		/// Register the query as a lazy query in the session and return a lazy
		/// instance that will evaluate the query only when needed
		/// </summary>
		Lazy<IEnumerable<T>> Lazily<T>();
#endif
	}
}
