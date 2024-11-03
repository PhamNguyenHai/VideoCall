using PetProject.Models;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.SignalR;
using PetProject.Repositories;
using AutoMapper;

namespace PetProject
{
    public class CommunicationHub : Hub<ICommunicationHub>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CommunicationHub(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task SendMsg(Guid senderId, Guid recipientId, string msg, DateTime time)
        {
            var sender = await _userRepository.FindByIdAsync(senderId);

            if (sender == null)
            {
                return;
            }

            var recipient = await _userRepository.FindByIdAsync(recipientId);
            if (recipient == null)
            {
                return;
            }

            var senderRes = _mapper.Map<UserViewModel>(sender);
            await Clients.User(recipientId.ToString()).ReceiveMsg(senderRes, msg, time);
        }
    }
}