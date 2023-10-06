using PyBox.Shared.Enums;
using PyBox.Shared.Services.Interfaces;

namespace PyBox.Shared.Services.Classes
{
    public class ScriptDataServiceResponse : IScriptDataServiceResponse
    {
        public object? Result { get; set; }
        public string? Errors { get; init; }
        public WarningLevel ErrorLevel { get; init; }
        public ScriptDataServiceResponse() { }
        public ScriptDataServiceResponse(object? result, string? errors, WarningLevel errorLevel)
        {
            Result = result;
            Errors = errors;
            ErrorLevel = errorLevel;
        }
    }
}
