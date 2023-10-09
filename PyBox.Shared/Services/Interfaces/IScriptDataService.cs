using PyBox.Shared.Models.Script;

namespace PyBox.Shared.Services.Interfaces
{
    public interface IScriptDataService
    {
        public Task<IScriptDataServiceResponse> GetScripts();
        public Task<IScriptDataServiceResponse> CheckTitle(ScriptCheckTitleUnique scriptCheckTitleUnique);
        public Task<IScriptDataServiceResponse> ToggleStatus(int id);
        public Task<IScriptDataServiceResponse> GetScript(int id);
        public Task<IScriptDataServiceResponse> CreateScript(ScriptDefinition scriptDefinition);
        public Task<IScriptDataServiceResponse> UpdateScript(int id, ScriptEdit scriptTableItem);
        public Task<IScriptDataServiceResponse> DeleteScript(int id);
        public Task<IScriptDataServiceResponse> CheckDependencies();
        public Task<IScriptDataServiceResponse> InstallDependencies();
    }
}
