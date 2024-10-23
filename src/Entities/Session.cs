using System;

namespace PetProject
{
    public class Session : BaseAuditEntity, IHasKey
    {
        public Guid SessionId { set; get; }
        public Guid UserId { set; get; }
        public string Token { set; get; }
        public DateTime ExpirationdDate { set; get; }

        public Guid GetKey()
        {
            return SessionId;
        }

        public void SetKey(Guid key)
        {
            SessionId = key;
        }
    }
}
