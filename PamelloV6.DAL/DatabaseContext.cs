using Microsoft.EntityFrameworkCore;
using PamelloV6.DAL.Entity;

namespace PamelloV6.DAL
{
	public class DatabaseContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
		public DbSet<SongEntity> Songs { get; set; }
		public DbSet<EpisodeEntity> Episodes { get; set; }
		public DbSet<PlaylistEntity> Playlists { get; set; }

        public DatabaseContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=C:\.PamelloV6Data\data.db");
        }
	}
}
