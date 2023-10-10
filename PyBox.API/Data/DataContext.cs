using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PyBox.Shared.Models.Script;

namespace PyBox.API.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options)
		{
			if (!Database.CanConnect())
			{
				Database.EnsureCreated();
				ScriptEntity[] list = JsonConvert.DeserializeObject<ScriptEntity[]>(Properties.Resources.Examples)!;
				foreach (var script in list)
				{
					Scripts!.Add(
						new ScriptEntity()
						{
							CreatedAt = script.CreatedAt,
							DeletedAt = script.DeletedAt,
							UpdatedAt = script.UpdatedAt,
							Description = script.Description,
							Enabled = script.Enabled,
							ScriptText = script.ScriptText,
							Title = script.Title
						}
					);
					SaveChangesAsync();
				}
			}
		}
		public DbSet<ScriptEntity> Scripts { get; set; }

	}
}
