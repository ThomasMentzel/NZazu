﻿using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using NZazu.Contracts.Suggest;

namespace NZazu.JsonSerializer.RestSuggestor
{
    public class ElasticSearchSuggestions : IProvideSuggestions
    {
        private readonly IDictionary<string, JProperty> _fieldsPropertyCache = new Dictionary<string, JProperty>();
        private readonly IDictionary<string, Uri> _baseAddressCache = new Dictionary<string, Uri>();
        private readonly IRestClient _client;

        public ElasticSearchSuggestions(IRestClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public IEnumerable<string> For(string prefix, string connection)
        {
            if (!connection.StartsWith("e:")) return Enumerable.Empty<string>();

            _client.BaseAddress = GetBaseAddress(connection);

            var requestBody = CreateRequest(prefix, connection);
            var response = _client.Post("_search", requestBody).Result;
            return GetSuggestions(response, prefix);
        }

        private JToken CreateRequest(string prefix, string connection)
        {
            var fieldsProperty = GetFieldsProperty(connection);

            return new JObject(
                fieldsProperty,
                new JProperty("query", new JObject(
                        new JProperty("multi_match", new JObject(
                            fieldsProperty,
                            new JProperty("type", "phrase_prefix"),
                            new JProperty("query", prefix))))
                ));
        }


        internal IEnumerable<string> GetSuggestions(JToken response, string prefix)
        {
            var fields = response.SelectTokens("hits.hits[*].fields");
            var valueArrays = fields.SelectMany(x => x.Values()).OfType<JArray>();
            var values = valueArrays.SelectMany(x => x.ToObject<List<string>>());

            return values.Where(x => x.StartsWith(prefix, StringComparison.InvariantCultureIgnoreCase)).Distinct();
        }

        private Uri GetBaseAddress(string connection)
        {
            if (_baseAddressCache.ContainsKey(connection))
                return _baseAddressCache[connection];

            var uri = new Uri(connection.Substring(2).Split('|')[0]);
            _baseAddressCache.Add(connection, uri);
            return uri;
        }

        private JProperty GetFieldsProperty(string connection)
        {
            if (_fieldsPropertyCache.ContainsKey(connection))
                return _fieldsPropertyCache[connection];

            var fields = connection.Substring(2).Split('|')[1].Split(',');
            // ReSharper disable once CoVariantArrayConversion
            var property = new JProperty("fields", new JArray(fields));
            _fieldsPropertyCache.Add(connection, property);
            return property;
        }
    }
}