using Newtonsoft.Json;
using PyBox.Shared.Enums;
using PyBox.Shared.Models.Script;
using PyBox.Shared.Services.Interfaces;
using PyBox.Shared.Services.Utilities;

namespace PyBox.Shared.Services.Classes
{
    public class HttpScriptDataService : IScriptDataService
    {
        private readonly HttpHelper _httpHelper;
        public HttpScriptDataService(HttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }
        public async Task<IScriptDataServiceResponse> CheckTitle(ScriptCheckTitleUnique input)
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.POST, "scripts/check_titles", input);
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<bool>((string)data.Result);
            return data;
        }

        public async Task<IScriptDataServiceResponse> CreateScript(ScriptDefinition input)
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.POST, "scripts", input);
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<ScriptEdit>((string)data.Result);
            return data;
        }

        public async Task<IScriptDataServiceResponse> DeleteScript(int id)
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.DELETE, $"scripts/{id}");
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<bool>((string)data.Result);
            return data;
        }

        public async Task<IScriptDataServiceResponse> GetScript(int id)
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.GET, $"scripts/{id}");
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<ScriptEdit>((string)data.Result);
            return data;
        }

        public async Task<IScriptDataServiceResponse> GetScripts()
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.GET, "scripts");
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<List<ScriptView>>((string)data.Result);
            return data;
        }

        public async Task<IScriptDataServiceResponse> ToggleStatus(int id)
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.GET, $"scripts/toggle_status/{id}");
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<bool>((string)data.Result);
            return data;
        }

        public async Task<IScriptDataServiceResponse> UpdateScript(int id, ScriptEdit input)
        {
            var data = await _httpHelper.GetData(HttpHelperRequestMethod.PUT, $"scripts/{id}", input);
            if (data.ErrorLevel == WarningLevel.NO_WARNING && data.Result != null)
                data.Result = JsonConvert.DeserializeObject<ScriptEdit>((string)data.Result);
            return data;
        }
    }
}
