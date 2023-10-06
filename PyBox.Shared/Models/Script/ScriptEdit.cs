namespace PyBox.Shared.Models.Script
{
    public class ScriptEdit : ScriptDefinition
    {
        public int ScriptId { get; init; }
        public string? ScriptText { get; set; }
        public bool Enabled { get; set; }
    }
}
