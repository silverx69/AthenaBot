﻿using CoinGecko.ApiEndPoints;
using CoinGecko.Entities.Response.Indexes;
using CoinGecko.Interfaces;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class IndexesClient : BaseApiClient, IIndexesClient
    {
        public IndexesClient(HttpClient httpClient, JsonSerializerOptions serializerSettings) : base(httpClient, serializerSettings)
        {
        }

        public IndexesClient(HttpClient httpClient, JsonSerializerOptions serializerSettings, string apiKey) : base(httpClient, serializerSettings, apiKey)
        {
        }

        public async Task<IReadOnlyList<IndexData>> GetIndexes()
        {
            return await GetIndexes(null, "").ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<IndexData>> GetIndexes(int? perPage, string page)
        {
            return await GetAsync<IReadOnlyList<IndexData>>(AppendQueryString(
                IndexesApiEndPointUrl.IndexesUrl, new Dictionary<string, object>
                {
                    {"per_page",perPage},
                    {"page",page}
                })).ConfigureAwait(false);
        }

        public async Task<IndexData> GetIndexById(string id)
        {
            return await GetAsync<IndexData>(AppendQueryString(
                IndexesApiEndPointUrl.IndexesWithId(id))).ConfigureAwait(false);
        }

        public async Task<IReadOnlyList<IndexList>> GetIndexList()
        {
            return await GetAsync<IReadOnlyList<IndexList>>(AppendQueryString(
                IndexesApiEndPointUrl.IndexesList)).ConfigureAwait(false);
        }
    }
}