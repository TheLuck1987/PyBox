using PyBox.Shared.Models.Script;

namespace PyBox.Shared.Services.Interfaces
{
	public interface IScriptDataService
	{
		public Task<IScriptDataServiceResponse> GetScripts();
		public Task<IScriptDataServiceResponse> CheckTitle(string title, int? id = null);
		public Task<IScriptDataServiceResponse> ToggleStatus(int id);
		public Task<IScriptDataServiceResponse> GetScript(int id);
		public Task<IScriptDataServiceResponse> CreateScript(ScriptDefinition scriptDefinition);
		public Task<IScriptDataServiceResponse> UpdateScript(int id, ScriptEdit scriptTableItem);
		public Task<IScriptDataServiceResponse> DeleteScript(int id);
	}
}
