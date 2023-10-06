using System.ComponentModel.DataAnnotations;

namespace PyBox.Shared.Models.Script
{
    public class ScriptDefinition
    {
        [Required]
        [MaxLength(50)]
        [DataType(DataType.Text)]
        public string? Title { get; set; }
        [DataType(DataType.Text)]
        [MaxLength(255)]
        public string? Description { get; set; }
    }
}
