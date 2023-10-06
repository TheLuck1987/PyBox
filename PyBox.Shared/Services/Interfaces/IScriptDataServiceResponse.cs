using PyBox.Shared.Enums;

namespace PyBox.Shared.Services.Interfaces
{
    public interface IScriptDataServiceResponse
    {
        public object? Result { get; set; }
        public string? Errors { get; init; }
        public WarningLevel ErrorLevel { get; init; }
    }
}
