using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace dotnet_rpg.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill{ id= 1, Name ="FireBall", Damage = 5},
                new Skill{ id= 2, Name ="Freeze", Damage = 2},
                new Skill{ id= 3, Name ="Plant", Damage = 6}
            );
        }
        public DbSet<Character> Characters => Set<Character>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Weapon> Weapons => Set<Weapon>();
        public DbSet<Skill> Skills => Set<Skill>();
    }
}