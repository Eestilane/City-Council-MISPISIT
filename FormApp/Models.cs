using System;

namespace VotingSystemWinForms
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
    }

    public class Meeting
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class Project
    {
        public int Id { get; set; }
        public int MeetingNumber { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class Vote
    {
        public int Id { get; set; }
        public int ProjectNumber { get; set; }
        public int Deputy { get; set; }
        public string Result { get; set; }
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
    }
}