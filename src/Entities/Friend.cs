using System;

namespace PetProject
{
    public class Friend : BaseAuditEntity
    {
        public Guid UserId { set; get; }
        public Guid FriendUserId { set; get; }
        public FriendStatus? Status { set; get; }
    }
}
