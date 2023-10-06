using Microsoft.AspNetCore.Components;
using PyBox.Shared.Models.Script;

namespace PyBox.UI.Components.ScriptsTable
{
    public partial class ScriptsTableLine
    {
        #region Inputs
        [Parameter]
        public ScriptView Item { get; set; }
        #endregion

        #region Outputs

        #endregion
    }
}
