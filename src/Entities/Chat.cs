using System;

namespace PetProject
{
    public class Chat
    {
        public Guid ChatId { set; get; }
        public Guid UserId { set; get; }
        public Guid FriendUserId { set; get; }
        public DateTime CreatedDate { set; get; }
        public DateTime ModifiedDate { set; get; }


    }
}
