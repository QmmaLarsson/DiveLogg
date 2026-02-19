using DiveLogg.Models;
using Microsoft.EntityFrameworkCore;

namespace DiveLogg.Data
{
    public class DiveLoggContext : DbContext
    {
        public DiveLoggContext(DbContextOptions<DiveLoggContext> options) : base(options)
        {

        }
        //Tabeller
        public DbSet<Dive> Dive { get; set; }
        public DbSet<DiveParticipant> DiveParticipant { get; set; }
        public DbSet<Person> Person { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<PersonRole> PersonRole { get; set; }
        public DbSet<Group> Group { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            //Roll som länkas till person måste vara unik (inte dykare + dykare etc)
            modelBuilder.Entity<PersonRole>()
                .HasIndex(pr => new { pr.PersonId, pr.RoleId })
                .IsUnique();

            modelBuilder.Entity<PersonRole>()
                .HasOne(pr => pr.Person)
                .WithMany(p => p.PersonRoles)
                .HasForeignKey(pr => pr.PersonId);

            modelBuilder.Entity<PersonRole>()
                .HasOne(pr => pr.Role)
                .WithMany(r => r.PersonRoles)
                .HasForeignKey(pr => pr.RoleId);

            
            // Dive ↔ Person (DiveLeader)
            modelBuilder.Entity<Dive>()
                .HasOne(d => d.DiveLeader)
                .WithMany(p => p.LedDives)
                .HasForeignKey(d => d.DiveLeaderId)
                .OnDelete(DeleteBehavior.Restrict);

            
            // Dive ↔ Person (DiveParticipant)
            modelBuilder.Entity<DiveParticipant>()
                .HasOne(dp => dp.Dive)
                .WithMany(d => d.DiveParticipants)
                .HasForeignKey(dp => dp.DiveId);

            modelBuilder.Entity<DiveParticipant>()
                .HasOne(dp => dp.Person)
                .WithMany(p => p.DiveParticipants)
                .HasForeignKey(dp => dp.PersonId);

            modelBuilder.Entity<DiveParticipant>()
                .HasOne(dp => dp.Role)
                .WithMany(r => r.DiveParticipants)
                .HasForeignKey(dp => dp.RoleId);

            //Seed-data
            //Skapar fyra standardgrupper som alltid finns i databasen
            modelBuilder.Entity<Group>().HasData(
                new Group { Id = 1, Name = "Grupp 1" },
                new Group { Id = 2, Name = "Grupp 2" },
                new Group { Id = 3, Name = "Grupp 3" },
                new Group { Id = 4, Name = "Grupp 4" }
            );

            //Seed-data
            //Skapar tre standardroller som alltid finns i databasen
            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "Diver" },
                new Role { Id = 2, Name = "DiveLeader" },
                new Role { Id = 3, Name = "DiveSupport" }
            );
        }
    }
}