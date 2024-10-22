using System;

namespace PetProject
{
    public class Friend
    {
        public Guid UserId { set; get; }
        public Guid FriendUserId { set; get; }
        public FriendStatus? Status { set; get; }
        public DateTime CreatedDate { set; get; }
    }
}
