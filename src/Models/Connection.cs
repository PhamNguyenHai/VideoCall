namespace PetProject.Models
{
    using System.Collections.Generic;

    public class Connection
    {
        public List<User> Users { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}
