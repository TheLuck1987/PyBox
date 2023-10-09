using PyBox.Shared.Enums;
using PyBox.Shared.Services.Classes;
using System.Net;
using System.Net.Http.Json;

namespace PyBox.Shared.Services.Utilities
{
    public class HttpHelper
    {
        private readonly HttpClient _http;
        public HttpHelper(HttpClient http)
        {
            _http = http;
        }
        public async Task<ScriptDataServiceResponse> GetData(HttpHelperRequestMethod method, string url, object? parameters = null)
        {
            try
            {
                HttpResponseMessage data;
                switch (method)
                {
                    case HttpHelperRequestMethod.POST:
                        data = await _http.PostAsJsonAsync(url, parameters);
                        break;
                    case HttpHelperRequestMethod.PUT:
                        data = await _http.PutAsJsonAsync(url, parameters);
                        break;
                    case HttpHelperRequestMethod.DELETE:
                        data = await _http.DeleteAsync(url);
                        break;
                    default:
                        data = await _http.GetAsync(url);
                        break;
                }
                return await GetResponse(data);
            }
            catch (Exception ex)
            {
                return new(null, ex.Message, WarningLevel.ERROR);
            }
        }
        public async Task<ScriptDataServiceResponse> GetResponse(HttpResponseMessage data)
        {
            HttpStatusCode[] badCodes = new HttpStatusCode[]
            {
                HttpStatusCode.InternalServerError,
                HttpStatusCode.NotImplemented,
                HttpStatusCode.BadGateway,
                HttpStatusCode.ServiceUnavailable,
                HttpStatusCode.GatewayTimeout,
                HttpStatusCode.HttpVersionNotSupported,
                HttpStatusCode.VariantAlsoNegotiates,
                HttpStatusCode.InsufficientStorage,
                HttpStatusCode.LoopDetected,
                HttpStatusCode.NotExtended,
                HttpStatusCode.NetworkAuthenticationRequired
            };
            try
            {
                if (data == null)
                    return new(null, "Unhandled Error", WarningLevel.ERROR);
                string? json = data.Content != null ? await data.Content.ReadAsStringAsync() : null;
                if (data.IsSuccessStatusCode)
                    return new(json, null, WarningLevel.NO_WARNING);
                return new(
                    null,
                    json,
                    badCodes.Contains(data.StatusCode) ? WarningLevel.ERROR : WarningLevel.WARNING
                );
            }
            catch (Exception ex)
            {
                return new(null, ex.ToString(), WarningLevel.ERROR);
            }
        }
    }

    public enum HttpHelperRequestMethod
    {
        GET,
        POST,
        PUT,
        DELETE
    }
}
