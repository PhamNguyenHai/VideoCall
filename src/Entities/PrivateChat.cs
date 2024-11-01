using System;
using System.ComponentModel.DataAnnotations;

namespace PetProject
{
    public class PrivateChat : BaseAuditEntity, IHasKey
    {
        public Guid ChatId { set; get; }
        public Guid UserId { set; get; }
        public Guid PartnerId { set; get; }

        public Guid GetKey()
        {
            return ChatId;
        }

        public void SetKey(Guid key)
        {
            ChatId = key;
        }
    }
}
