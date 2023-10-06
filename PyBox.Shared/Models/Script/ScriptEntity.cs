using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PyBox.Shared.Models.Script
{
    public class ScriptEntity
    {
        [Key]
        public int ScriptId { get; init; }
        [Required]
        [Column(TypeName = "varchar(50)")]
        public string? Title { get; set; }
        [Column(TypeName = "varchar(255)")]
        public string? Description { get; set; }
        [Column(TypeName = "text")]
        public string? ScriptText { get; set; }
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }

        public ScriptView GetView() => new ScriptView
        {
            ScriptId = ScriptId,
            Title = Title,
            Description = Description,
            ScriptText = ScriptText,
            Enabled = Enabled,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };

        public ScriptEdit GetEdit() => new()
        {
            ScriptId = ScriptId,
            Title = Title,
            Description = Description,
            ScriptText = ScriptText,
            Enabled = Enabled
        };

        public ScriptDefinition GetDefinition() => new()
        {
            Title = Title,
            Description = Description
        };
    }
}
