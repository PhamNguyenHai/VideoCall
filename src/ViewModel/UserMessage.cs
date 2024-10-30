using System.Collections;
using System.Collections.Generic;

namespace PetProject.ViewModel
{
    public class UserMessage
    {
        public UserViewModel Partner { get; set; }
        public List<PrivateMessageViewModel> Messages { get; set; } = new List<PrivateMessageViewModel>();
        public FriendStatus? FriendStatus { get; set; }
    }
}
