using System.Threading.Tasks;
using System;

namespace PetProject
{
    public interface ICommunicationHub
    {
        Task ReceiveMsg(UserViewModel sender, string msg, DateTime time);
    }
}
