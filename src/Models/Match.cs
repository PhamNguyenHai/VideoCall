namespace PetProject.Models
{
    using System;

    public class Match
    {
        public User From { get; set; }
        public User To { get; set; }
        public DateTime CallStartTime { get; set; }
    }
}
