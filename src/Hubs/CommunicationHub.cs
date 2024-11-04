using PetProject.Models;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.SignalR;
using PetProject.Repositories;
using AutoMapper;
using System.Security.Claims;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace PetProject
{
    public class CommunicationHub : Hub<ICommunicationHub>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        private static ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();

        public CommunicationHub(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public override async Task OnConnectedAsync()
        {
            // Lấy UserId từ cookie
            var userId = Context.GetHttpContext().Request.Cookies["UserId"];

            if (!string.IsNullOrEmpty(userId))
            {
                // Lưu trữ UserId và ConnectionId
                _connections[userId] = Context.ConnectionId;
            }

            await base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Lấy UserId từ cookie
            var userId = Context.GetHttpContext().Request.Cookies["UserId"];
            if (!string.IsNullOrEmpty(userId))
            {
                _connections.TryRemove(userId, out _); // Xóa khi ngắt kết nối
            }

            // Cập nhật vào cơ sở dữ liệu nếu cần
            return base.OnDisconnectedAsync(exception);
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

            if (_connections.TryGetValue(recipientId.ToString(), out var connectionId))
            {
                await Clients.Client(connectionId).ReceiveMsg(senderRes, msg, time);
            }
        }
    }
}