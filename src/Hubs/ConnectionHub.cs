namespace PetProject.Hubs
{
    using PetProject.Models;
    using Microsoft.AspNetCore.SignalR;
    using Microsoft.VisualBasic;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Channels;
    using System.Threading.Tasks;
    public class ConnectionHub : Hub<IConnectionHub>
    {
        private readonly List<User> _users;                 // người dùng trong ứng dụng
        private readonly List<Connection> _connections;     // đại diện cho một kết nối, có thể chứa nhiều người dùng. Có thể hỗ trợ kết nối phức tạp như gọi nhóm, etc
        private readonly List<Match> _calls;                // thông tin các call đang diễn ra

        public ConnectionHub(List<User> users, List<Connection> userCalls, List<Match> calls)
        {
            _users = users;
            _connections = userCalls;
            _calls = calls;
        }

        /// <summary>
        /// Hàm thực hiện matching người dùng cho cuộc gọi
        /// </summary>
        public async Task Matching()
        {

            var targetConnectionId = _users.FirstOrDefault(u => u.ConnectionId != Context.ConnectionId && GetConnection(u.ConnectionId) == null);
            
            var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (targetConnectionId == null)
            {
                await Clients.Caller.CallDeclined(targetConnectionId, "nobody match.");
                return;
            }

            await Clients.Client(targetConnectionId.ConnectionId).IncomingCall(callingUser);

            _calls.Add(new Match
            {
                From = callingUser,
                To = targetConnectionId,
                CallStartTime = DateTime.Now
            });

            await Clients.Client(Context.ConnectionId).Matched(targetConnectionId);
            
        }

        /// <summary>
        /// Hàm thực hiện join người dùng vào app
        /// </summary>
        /// <param name="username">username người dùng</param>
        public async Task Join(string username)
        {
            _users.Add(new User
            {
                Username = username,
                ConnectionId = Context.ConnectionId
            });

            await UpdateOnlineUsers();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await HangUp();

            _users.RemoveAll(u => u.ConnectionId == Context.ConnectionId);

            await UpdateOnlineUsers();

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Hàm thực hiện trả lời cuộc gọi
        /// </summary>
        /// <param name="acceptCall">kết quả trả lời</param>
        /// <param name="targetConnectionId">người trả lời đang hướng đến</param>
        public async Task AnswerCall(bool acceptCall, User targetConnectionId)
        {
            var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
            var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId.ConnectionId);

            if (callingUser == null)
            {
                return;
            }

            if (targetUser == null)
            {
                await Clients.Caller.CallEnded(targetConnectionId, "The user has left.");
                return;
            }

            if (!acceptCall)
            {
                await Clients.Client(targetConnectionId.ConnectionId).CallDeclined(callingUser, $"{callingUser.Username} did not accept your call.");
                return;
            }

            var callCount = _calls.RemoveAll(c => c.To.ConnectionId == callingUser.ConnectionId && c.From.ConnectionId == targetUser.ConnectionId);
            if (callCount < 1)
            {
                await Clients.Caller.CallEnded(targetConnectionId, $"{targetUser.Username} has already end the call.");
                return;
            }

            // Check if user is in another call
            if (GetConnection(targetUser.ConnectionId) != null)
            {
                await Clients.Caller.CallDeclined(targetConnectionId, $"{targetUser.Username} is in another call.");
                return;
            }

            // Remove all the other offers for the call initiator, in case they have multiple calls out
            _calls.RemoveAll(c => c.From.ConnectionId == targetUser.ConnectionId);

            _connections.Add(new Connection
            {
                Users = new List<User> { callingUser, targetUser }
            });

            await Clients.Client(targetConnectionId.ConnectionId).CallAccepted(callingUser);

            await UpdateOnlineUsers();

        }

        /// <summary>
        /// Hàm thực hiện kết thúc cuộc gọi
        /// </summary>
        public async Task HangUp()
        {
            var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (callingUser == null)
            {
                return;
            }

            var currentCall = GetConnection(callingUser.ConnectionId);

            if (currentCall != null)
            {
                foreach (var user in currentCall.Users.Where(u => u.ConnectionId != callingUser.ConnectionId))
                {
                    await Clients.Client(user.ConnectionId).CallEnded(callingUser, $"{callingUser.Username} has hung up.");
                }

                currentCall.Users.RemoveAll(u => u.ConnectionId == callingUser.ConnectionId);
                if (currentCall.Users.Count < 2)
                {
                    _connections.Remove(currentCall);
                }
            }

            _calls.RemoveAll(c => c.From.ConnectionId == callingUser.ConnectionId);

            await UpdateOnlineUsers();
            
        }

        public async Task SendData(string data, string targetConnectionId)
        {
            try
            {
                var callingUser = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);
                var targetUser = _users.SingleOrDefault(u => u.ConnectionId == targetConnectionId);

                if (callingUser == null || targetUser == null)
                {
                    return;
                }

                //var userCall = GetConnection(callingUser.ConnectionId);
                //if (userCall != null && userCall.Users.Exists(u => u.ConnectionId == targetUser.ConnectionId))
                //{
                //    await Clients.Client(targetConnectionId).ReceiveData(callingUser, data);
                //}

                await Clients.Client(targetConnectionId).ReceiveData(data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SendData: {ex.Message}");
            }
            
        }

        // update online user sẽ update số người đg chưa có connection nào trên giao diện
        // update avaiable sẽ thực hiện update các nút trên giao diện



        /// <summary>
        /// Hàm thực hiện update trạng thái online với những user 
        /// </summary>
        private async Task UpdateOnlineUsers()
        {
            // Lấy ra những người dùng đang không trong connection nào
            var avaiableUser = _users.Where(x => !_connections.Any( y => y.Users.Any(z => z.ConnectionId == x.ConnectionId)));

            await Clients.All.UpdateOnlineUsers(avaiableUser.ToList());

            _users.ForEach(async u => {
                await UpdateAvaiableStatus(u.ConnectionId);
            });
        }

        /// <summary>
        /// Hàm thực hiện update trạng thái avaiable cho người dùng ứng với connectionId (Tức là người dùng có đang không trong connections nào không)
        /// </summary>
        /// <param name="connectionId">Kết nối người dùng</param>
        private async Task UpdateAvaiableStatus(string connectionId)
        {
            bool isAvaiable = true;
            _connections.ForEach(x =>{
                isAvaiable =  !(x.Users.Any(y => y.ConnectionId == connectionId));
            });

            await Clients.Client(connectionId).UpdateAvaiableStatus(isAvaiable);
        }

        /// <summary>
        /// Phương thức thực hiện lấy connection ứng với connectionId
        /// </summary>
        /// <param name="connectionId">Kết nối người dùng</param>
        /// <returns>Đối tượng Connection</returns>
        private Connection GetConnection(string connectionId)
        {
            var matchingCall = _connections.FirstOrDefault(uc => uc.Users.FirstOrDefault(u => u.ConnectionId == connectionId) != null);
            return matchingCall;
        }

        public async Task ToggleCamera(bool isEnabled)
        {
            var user = _users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                user.IsCameraEnabled = isEnabled; // Cập nhật trạng thái camera

                var currentConnection = GetConnection(user.ConnectionId);
                if (currentConnection != null)
                {
                    foreach (var participant in currentConnection.Users)
                    {
                        if (participant.ConnectionId != user.ConnectionId)
                        {
                            await Clients.Client(participant.ConnectionId).UpdateCameraStatus(user, isEnabled);
                        }
                    }
                }
            }
        }

        public async Task ToggleMicrophone(bool isEnabled)
        {
            var user = _users.FirstOrDefault(u => u.ConnectionId == Context.ConnectionId);
            if (user != null)
            {
                user.IsMicrophoneEnabled = isEnabled; // Cập nhật trạng thái microphone

                var currentConnection = GetConnection(user.ConnectionId);
                if (currentConnection != null)
                {
                    foreach (var participant in currentConnection.Users)
                    {
                        if (participant.ConnectionId != user.ConnectionId)
                        {
                            await Clients.Client(participant.ConnectionId).UpdateMicrophoneStatus(user, isEnabled);
                        }
                    }
                }
            }
        }

        public async Task SendMsg(string msg, DateTime time)
        {
            var sender = _users.SingleOrDefault(u => u.ConnectionId == Context.ConnectionId);

            if (sender == null)
            {
                return;
            }

            var userCallConnection = GetConnection(sender.ConnectionId);
            if (userCallConnection != null && userCallConnection.Users.Count > 1)
            {
                var target = userCallConnection.Users.FirstOrDefault(user => user.ConnectionId != sender.ConnectionId);

                userCallConnection.Messages.Add(new Message()
                {
                    Content = msg,
                    IsRead = false,
                    Sender = sender,
                    Timestamp = time
                });
                await Clients.Client(target.ConnectionId).ReceiveMsg(sender, msg, time);
            }
        }
    }
}
