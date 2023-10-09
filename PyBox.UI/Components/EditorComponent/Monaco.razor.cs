using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;

namespace PyBox.UI.Components.EditorComponent
{
	public partial class Monaco
	{
		[Parameter]
		public string? InText { private get; set; } = null;

		[Parameter]
		public EventCallback<string> ScriptChanged { get; set; }
		[Parameter]
		public EventCallback ScriptIsEquals { get; set; }

		private StandaloneCodeEditor? _editor = null;
		private string baseText = "#Remember that if you want to pass parameters to the script, you need to import the \"sys\" library (using sys).\n" +
					"#This way you will be able to access the parameters passed to the script via the \"sys.argv[<index>]\" variable\n\n#PS Remember that the first " +
					"parameter passed to the script will be at index \"1\" (sys.argv[1]) and not at index \"0\"\n\nimport sys\n\nprint(f'Hello {sys.argv[1]}')\n";
		private StandaloneEditorConstructionOptions editorConstructionOptions(StandaloneCodeEditor editor)
		{
			_editor = editor;
			return new StandaloneEditorConstructionOptions
			{
				AutomaticLayout = true,
				Language = "python",
				Value = string.IsNullOrWhiteSpace(InText) ? baseText : InText
			};
		}
		private async Task editorOnDidInit() => await Global.SetTheme(JSRuntime, "vs-dark");
		private async Task changed()
		{
			if (_editor == null)
				return;
			string outText = _editor == null ? "" : await _editor.GetValue();
			if (outText == baseText)
				return;
			if (outText != InText)
				await ScriptChanged.InvokeAsync(outText);
			else
				await ScriptIsEquals.InvokeAsync(outText);
		}
	}
}
