using CoinGecko.ApiEndPoints;
using CoinGecko.Entities.Response.Events;
using CoinGecko.Interfaces;
using System.Text.Json;

namespace CoinGecko.Clients
{
    public class EventsClient : BaseApiClient, IEventsClient
    {
        public EventsClient(HttpClient httpClient, JsonSerializerOptions serializerSettings) 
            : base(httpClient, serializerSettings) {
        }

        public EventsClient(HttpClient httpClient, JsonSerializerOptions serializerSettings, string apiKey) 
            : base(httpClient, serializerSettings, apiKey) {
        }

        public async Task<Events> GetEvents() {
            return await GetEvents(Array.Empty<string>(), Array.Empty<string>(), null, null, null, null).ConfigureAwait(false);
        }

        public async Task<Events> GetEvents(string[] countryCode, string[] type, string page, string upcommingEventsOnly,
            string fromDate, string toDate) {
            return await GetAsync<Events>(AppendQueryString(EventsApiEndPoints.Events,
                new Dictionary<string, object>
                {
                    {"country_code",string.Join(",",countryCode)},
                    {"type",string.Join(",",type)},
                    {"page",page},
                    {"upcoming_events_only",upcommingEventsOnly},
                    {"from_date",fromDate},
                    {"to_date",toDate}
                })).ConfigureAwait(false);
        }

        public async Task<EventCountry> GetEventCountry() {
            return await GetAsync<EventCountry>(
                AppendQueryString(EventsApiEndPoints.EventsCountries)).ConfigureAwait(false);
        }

        public async Task<EventTypes> GetEventTypes() {
            return await GetAsync<EventTypes>(AppendQueryString(EventsApiEndPoints.EventsTypes)).ConfigureAwait(false);
        }
    }
}