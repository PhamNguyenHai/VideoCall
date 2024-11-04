using System;

namespace PetProject
{
    public class FriendRelationship
    {
        public UserViewModel User { get; set; }
        public FriendStatus? Status { get; set; }
        public Guid RequestSender { get; set; }
    }
}