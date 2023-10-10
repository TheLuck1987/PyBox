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
		private string baseText = "#Remember: the parameters will be accessible from the script by calling the \"args[<index>]\" array.\n\n" +
						"print(f'Hello {args[0]}')\n";
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
