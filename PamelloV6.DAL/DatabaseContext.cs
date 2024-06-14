using Microsoft.EntityFrameworkCore;
using PamelloV6.Core.Abstract;
using PamelloV6.DAL.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

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

    public static class DatabaseExtensions {
		public static void Build<T>(this DbSet<T> set) where T : class, ITransformableToDTO {
			switch (typeof(T).Name) {
				case "UserEntity":
					(set as DbSet<UserEntity> ?? throw new Exception())
						.Include(user => user.OwnedPlaylists);
					break;
				case "PlaylistEntity":
					(set as DbSet<PlaylistEntity> ?? throw new Exception())
						.Include(playlist => playlist.Songs)
						.Include(playlist => playlist.Owner);
					break;
				case "SongEntity":
					(set as DbSet<SongEntity> ?? throw new Exception())
						.Include(song => song.Playlists)
						.Include(song => song.Episodes);
					break;
				case "EpisodeEntity":
					(set as DbSet<EpisodeEntity> ?? throw new Exception())
						.Include(episode => episode.Song);
					break;
			}
		}
	}
}
