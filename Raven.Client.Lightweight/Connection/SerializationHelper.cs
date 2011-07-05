//-----------------------------------------------------------------------
// <copyright file="SerializationHelper.cs" company="Hibernating Rhinos LTD">
//     Copyright (c) Hibernating Rhinos LTD. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Raven.Abstractions.Data;
using Raven.Abstractions.Extensions;
using Raven.Json.Linq;

namespace Raven.Client.Connection
{
	///<summary>
	/// Helper method to do serialization from RavenJObject to JsonDocument
	///</summary>
	public static class SerializationHelper
	{
		///<summary>
		/// Translate a collection of RavenJObject to JsonDocuments
		///</summary>
		public static IEnumerable<JsonDocument> RavenJObjectsToJsonDocuments(IEnumerable<RavenJObject> responses)
		{
			return (from doc in responses
					let metadata = doc["@metadata"] as RavenJObject
					let _ = doc.Remove("@metadata")
					let key = Extract(metadata, "@id", string.Empty)
					let lastModified = Extract(metadata, "Last-Modified", DateTime.Now, (string d) => ConvertToUtcDate(d))
					let etag = Extract(metadata, "@etag", Guid.Empty, (string g) => new Guid(g))
					let nai = Extract(metadata, "Non-Authoritive-Information", false, (string b) => Convert.ToBoolean(b))
					select new JsonDocument
					{
						Key = key,
						LastModified = lastModified,
						Etag = etag,
						NonAuthoritiveInformation = nai,
						Metadata = metadata.FilterHeaders(isServerDocument: false),
						DataAsJson = doc,
					}).ToList();
		}

		///<summary>
		/// Translate a collection of RavenJObject to JsonDocuments
		///</summary>
		public static IEnumerable<JsonDocument> ToJsonDocuments(this IEnumerable<RavenJObject> responses)
		{
			return RavenJObjectsToJsonDocuments(responses);
		}

		///<summary>
		/// Translate a collection of RavenJObject to JsonDocuments
		///</summary>
		public static JsonDocument ToJsonDocument(this RavenJObject response)
		{
			return RavenJObjectsToJsonDocuments(new[] { response }).First();
		}

		static DateTime ConvertToUtcDate(string date)
		{
			return DateTime.SpecifyKind( DateTime.ParseExact(date, new[]{"r","o"}, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind), DateTimeKind.Utc);
		}

		static T Extract<T>(RavenJObject metadata, string key, T defaultValue = default(T))
		{
			return Extract<T, T>(metadata, key, defaultValue, t => t);
		}

		static TResult Extract<T, TResult>(RavenJObject metadata, string key, TResult defaultValue, Func<T, TResult> convert)
		{
			if (metadata == null) return defaultValue;
			if (!metadata.ContainsKey(key)) return defaultValue;

			var value = metadata[key].Value<object>();

			if(value is TResult)
				return (TResult) value;

			return convert(metadata[key].Value<T>());
		}

		/// <summary>
		/// Translate a result for a query
		/// </summary>
		public static QueryResult ToQueryResult(RavenJObject json, string etagHeader)
		{
			return new QueryResult
			{
				IsStale = Convert.ToBoolean(json["IsStale"].ToString()),
				IndexTimestamp = json.Value<DateTime>("IndexTimestamp"),
				IndexEtag = new Guid(etagHeader),
				Results = ((RavenJArray)json["Results"]).Cast<RavenJObject>().ToList(),
				Includes = ((RavenJArray)json["Includes"]).Cast<RavenJObject>().ToList(),
				TotalResults = Convert.ToInt32(json["TotalResults"].ToString()),
				IndexName = json.Value<string>("IndexName"),
				SkippedResults = Convert.ToInt32(json["SkippedResults"].ToString()),
			};
		}
	}
}
