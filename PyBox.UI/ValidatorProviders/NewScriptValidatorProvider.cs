using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PyBox.Shared.Models.Script;

namespace PyBox.UI.ValidatorProviders
{
	public class NewScriptValidatorProvider : ComponentBase
	{

		[CascadingParameter]
		public EditContext? EditContext { get; set; }

		protected override void OnInitialized()
		{
			var messages = new ValidationMessageStore(EditContext!);

			EditContext!.OnValidationRequested += (sender, eventArgs)
				=> ValidateModel((EditContext)sender!, messages);
		}

		private void ValidateModel(EditContext editContext, ValidationMessageStore messages)
		{
			messages.Clear();
			if (editContext.Model != null)
			{
				var value = editContext.Model as ScriptDefinition;
				if (string.IsNullOrWhiteSpace(value?.Title))
				{
					messages.Add(editContext.Field("Title"), "Title is required");
				}
				else if (value.Title.Length > 50)
				{
					messages.Add(editContext.Field("Title"), "Title cannot exceed 50 characters");
				}
				if (value?.Description != null && value.Description.Length > 255)
				{
					messages.Add(editContext.Field("Description"), "Description cannot exceed 50 characters");
				}
			}
		}
	}
}
