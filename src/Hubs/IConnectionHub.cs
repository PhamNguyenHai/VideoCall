
namespace PetProject.Hubs
{
    using PetProject.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Channels;
    using System.Threading.Tasks;

    public interface IConnectionHub
    {
        Task UpdateOnlineUsers(List<User> userList);
        Task UpdateAvaiableStatus(bool status);
        Task CallAccepted(User acceptingUser);
        Task Matched(User target);
        Task CallDeclined(User decliningUser, string reason);
        Task IncomingCall(User callingUser);
        Task ReceiveData(string signal);
        Task UploadStream(ChannelReader<string> stream);
        Task CallEnded(User signalingUser, string signal);

        Task UpdateCameraStatus(User target, bool status);
        Task UpdateMicrophoneStatus(User target, bool status);
        Task ReceiveMsg(User sender, string msg, DateTime time);
    }
}