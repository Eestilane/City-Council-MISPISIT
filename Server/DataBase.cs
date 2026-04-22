using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;

namespace Server
{
    public class Deputy
    {
        public int Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string District { get; set; }
        public string Party { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }

    public class Meeting
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public virtual ICollection<Project> Projects { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public int MeetingNumber { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }

        public virtual Meeting Meeting { get; set; }

        public virtual ICollection<Vote> Votes { get; set; }
    }

    public class Vote
    {
        public int Id { get; set; }
        public int ProjectNumber { get; set; }
        public int Deputy { get; set; }
        public string Result { get; set; }

        public virtual Project Project { get; set; }
        public virtual Deputy DeputyNavigation { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class VotingDbContext : DbContext
    {
        public DbSet<Deputy> Deputies { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Vote> Votes { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "votingDB.mdf");
            string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={dbFilePath};Integrated Security=True;Connect Timeout=30";
            optionsBuilder.UseSqlServer(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasOne(p => p.Meeting)
                .WithMany(m => m.Projects)
                .HasForeignKey(p => p.MeetingNumber);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.Project)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.ProjectNumber);

            modelBuilder.Entity<Vote>()
                .HasOne(v => v.DeputyNavigation)
                .WithMany(d => d.Votes)
                .HasForeignKey(v => v.Deputy);
        }
    }
}