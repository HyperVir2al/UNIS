using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Universities.Models;

namespace Universities.Data
{
    public class UnisContext : DbContext
    {
        public DbSet<University> Universitys => Set<University>();
        public DbSet<Group> Groups => Set<Group>();
        public DbSet<Student> Students => Set<Student>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=unis.db");
        }
    }
}