using Microsoft.AspNetCore.Components;

namespace PyBox.UI.Components.ScriptsTable
{
	public partial class ScriptsStatusIcon
	{
		#region Inputs
		[Parameter]
		public bool Enabled { get; set; }
		[Parameter]
		public bool Deleting { get; set; } = false;
		#endregion
	}
}
