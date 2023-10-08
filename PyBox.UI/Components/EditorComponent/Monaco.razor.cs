using BlazorMonaco.Editor;

namespace PyBox.UI.Components.EditorComponent
{
    public partial class Monaco
    {
        private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
        {
            return new StandaloneEditorConstructionOptions
            {
                AutomaticLayout = true,
                Language = "csharp",
                Value = "public void SayHello()\n{\n   Console.WriteLine(\"Hello world!\");\n}"
            };
        }
        private async Task EditorOnDidInit()
        {
            await Global.SetTheme(JSRuntime, "vs-dark");
        }
    }
}
