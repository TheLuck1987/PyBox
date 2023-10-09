using Microsoft.EntityFrameworkCore;
using PyBox.Shared.Models.Script;

namespace PyBox.API.Data
{
	public class DataContext : DbContext
	{
		public DataContext(DbContextOptions<DataContext> options) : base(options) { }
		public DbSet<ScriptEntity> Scripts { get; set; }

	}
}
