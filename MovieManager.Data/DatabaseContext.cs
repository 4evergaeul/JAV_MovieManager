using MovieManager.ClassLibrary;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SQLite;
using System.IO;

namespace MovieManager.Data
{
    public class DatabaseContext: DbContext
    {

        public DatabaseContext(): base(new SQLiteConnection()
        {
            ConnectionString = new SQLiteConnectionStringBuilder() { DataSource = ConfigurationManager.AppSettings["DatabaseLocation"], ForeignKeys = true }.ConnectionString
        }, true) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Actor> Actors { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<MovieActors> MovieActors { get; set; }
        public DbSet<MovieGenres> MovieGenres { get; set; }
        public DbSet<MovieTags> MovieTags { get; set; }
        public DbSet<PlayList> PlayLists { get; set; }
        public DbSet<Tag> Tags { get; set; }
    }
}
