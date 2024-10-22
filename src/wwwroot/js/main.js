'use strict';

const localVideo = document.getElementById('localVideo');
const remoteVideo = document.getElementById('remoteVideo');
var hubUrl = document.location.origin + '/connect';

var hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, signalR.HttpTransportType.WebSockets)
    .configureLogging(signalR.LogLevel.None)
    .build();

var configuration = {
    "iceServers": [{
        "url": "stun:stun.l.google.com:19302"
    }]
};

var peerConnection;

var localStream = null;

var users = [];
var user = null;
var caller = null;

// init functions

const eventInit = () => {
    document.getElementById('startMatchingButton').addEventListener('click', function () {
        startMatching();
    });

    document.getElementById('dropCallButton').addEventListener('click', function () {
        endCall();
    });

    document.getElementById('sendMsgButton').addEventListener('click', sendMsg);

    document.querySelector('.btn-icon-only.msg-btn-icon').addEventListener('click', function () {
        const notifyElement = document.querySelector('.notification-dot')
        if (notifyElement) {
            notifyElement.style.display = 'none'
        }
    })

    // Gán các nút toggle
    document.getElementById('toggleCameraButton').addEventListener('click', toggleCamera);

    document.getElementById('toggleMicrophoneButton').addEventListener('click', toggleMicrophone);
}

const userOnlineNumber = (userList) => {

    $('#usersdata li.user').remove();
    // Calculate the number of online users
    var onlineUserCount = userList.length;

    // Create the HTML to display the number of online users
    var onlineUsersHtml = '<div class="online-users-count">Number of avaiable users: ' + onlineUserCount + '</div>';

    // Append the HTML to the usersdata element
    $('#usersdata').html(onlineUsersHtml);
}

//const setupRTCConnection = async (targetUserConnectionId) => {

//    peerConnection.onicecandidate = event => {
//        if (event.candidate) {
//            hubConnection.invoke('sendData', JSON.stringify({ 'candidate': event.candidate }), targetUserConnectionId.connectionId)
//                .catch(err => console.error(err));
//        }
//    };

//    peerConnection.onnegotiationneeded = () => {
//        peerConnection.createOffer()
//            .then(description => {
//                return peerConnection.setLocalDescription(description);
//            })
//            .then(() => {
//                hubConnection.invoke('sendData', JSON.stringify({ 'sdp': peerConnection.localDescription }), targetUserConnectionId.connectionId)
//                    .catch(err => console.error(err));
//            })
//            .catch(err => console.error(err));
//    };
//}

//const handleSDP = async (sdp, targetUserConnectionId) => {
//    await peerConnection.setRemoteDescription(new RTCSessionDescription(sdp));
//    if (peerConnection.remoteDescription.type === 'offer') {
//        const answer = await peerConnection.createAnswer();
//        await peerConnection.setLocalDescription(answer);
//        hubConnection.invoke('SendData', JSON.stringify({ sdp: peerConnection.localDescription }), targetUserConnectionId.connectionId)
//            .catch(err => console.error(err));
//    }
//};

//const initPeerConnection = async (targetUserConnectionId) => {
//    peerConnection = new RTCPeerConnection(configuration);
//    localStream.getTracks().forEach(track => peerConnection.addTrack(track, localStream));

//    peerConnection.onicecandidate = event => {
//        if (event.candidate) {
//            hubConnection.invoke('sendData', JSON.stringify({ 'candidate': event.candidate }), targetUserConnectionId.connectionId)
//                .catch(err => console.error(err));
//        }
//    };

//    peerConnection.ontrack = event => {
//        remoteVideo.srcObject = event.streams[0];
//    };
//};

//const setupRTCConnection = async (targetUserConnectionId) => {
//    const offer = await peerConnection.createOffer();
//    await peerConnection.setLocalDescription(offer);
//    hubConnection.invoke('SendData', JSON.stringify({ sdp: peerConnection.localDescription }), targetUserConnectionId)
//        .catch(err => console.error(err));
//};

const initVideo = () => {
    remoteVideo.oncanplay = function () {
        loadingMessage.style.display = 'none';
    };

    remoteVideo.onerror = function () {
        loadingMessage.textContent = 'Video not available.';
    };
}

async function getMedia() {
    try {
        localStream = await navigator.mediaDevices.getUserMedia({
            video: true,
            audio: true,
        });

        localVideo.srcObject = localStream;

        peerConnection = new RTCPeerConnection(configuration);

        let tracks = localStream.getTracks();

        //peerConnection.ontrack = event => {
        //    const stream = event.streams[0];
        //    if (!remoteVideo.srcObject || remoteVideo.srcObject.id !== stream.id) {
        //        remoteVideo.srcObject = stream;
        //    }
        //};

        peerConnection.ontrack = event => {
            const stream = event.streams[0];
            if (remoteVideo.srcObject !== stream) {
                remoteVideo.srcObject = stream; // Cập nhật video với stream từ remote
            }
        };

        tracks.forEach(track => peerConnection.addTrack(track, localStream));

    } catch (error) {
        console.error('Error getting user media:', error);
    }
}

const initSignalrHub = () => {

    //connecting to hub
    hubConnection.start().then(async () => {

        user = generateId();

        userJoin(user);

        await getMedia();

    }).catch(err => console.error(err));


    hubConnection.on('updateOnlineUsers', async (userList) => {
        //list online user
        users = userList;

        //show user online count
        userOnlineNumber(userList);
    });

    // Nhận dữ liệu từ hub
    hubConnection.on("ReceiveData", async (data) => {
        const message = JSON.parse(data);
        if (message.sdp) {
            await peerConnection.setRemoteDescription(new RTCSessionDescription(message.sdp));
            if (peerConnection.remoteDescription.type === 'offer') {
                const answer = await peerConnection.createAnswer();
                await peerConnection.setLocalDescription(answer);
                hubConnection.invoke('SendData', JSON.stringify({ 'sdp': peerConnection.localDescription }), user.connectionId);
            }
        } else if (message.candidate) {
            await peerConnection.addIceCandidate(new RTCIceCandidate(message.candidate));
        }
    });

    hubConnection.on('incomingCall', async (callingUser) => {
        //seting up caller
        caller = callingUser;
        $('#callmodal').attr('data-callinguser', `${callingUser.username}`);
        $('#callmodal .modal-body').text(`${callingUser.username} want to match...`);
        $('#callmodal').modal('show');

        let localStream = await navigator.mediaDevices.getUserMedia({
            video: true,
            audio: true,
        });
        remoteVideo.srcObject = localStream;

        document.getElementById('loadingSpinner').style.display = 'none';
    });

    hubConnection.on("UpdateAvaiableStatus", (isAvaible) => {
        document.getElementById('startMatchingButton').disabled = (isAvaible == false);
        document.getElementById('dropCallButton').disabled = (isAvaible == true);
    })

    hubConnection.on("matched", async (targetUserConnectionId) => {
            peerConnection.onicecandidate = event => {
                if (event.candidate) {
                    hubConnection.invoke('sendData', JSON.stringify({ 'candidate': event.candidate }), targetUserConnectionId.connectionId)
                        .catch(err => console.error(err));
                }
            };

            peerConnection.onnegotiationneeded = () => {
                peerConnection.createOffer()
                    .then(description => {
                        return peerConnection.setLocalDescription(description);
                    })
                    .then(() => {
                        hubConnection.invoke('sendData', JSON.stringify({ 'sdp': peerConnection.localDescription }), targetUserConnectionId.connectionId)
                            .catch(err => console.error(err));
                    })
                    .catch(err => console.error(err));
            };
    })

    hubConnection.on("CallEnded", async (callingUser) => {
        // Ngừng video của người đang gọi
        if (remoteVideo.srcObject) {
            let remoteTracks = remoteVideo.srcObject.getTracks();
            remoteTracks.forEach(track => track.stop()); // Dừng tất cả các track từ video remote
            remoteVideo.srcObject = null; // Xóa video source
        }

        alert(`${callingUser.username} was ended the call...`);
        console.log(`${callingUser.username} has hung up.`);
        msgContent.innerHTML = ''
    })

    hubConnection.on("ReceiveMsg", (sender, msg, time) => {
        const msgElementDOM = createMessageElement('received', sender?.username, getFormattedDate(new Date(time)), msg)
        msgContent.appendChild(msgElementDOM)
        scrollToBottom(msgContent)

        const modal = document.getElementById('messageModal');
        const isShow = modal.classList.contains('show');
        // Nếu không show modal thì hiển thị notify ở icon tin nhắn
        if (!isShow) {
            const notifyElement = document.querySelector('.notification-dot')
            if (notifyElement && notifyElement.style.display === 'none') {
                notifyElement.style.display = 'block'
            }
        }
    })

    hubConnection.on("UpdateCameraStatus", (targetUser, isEnabled) => {
        // Kiểm tra xem người dùng có phải là chính mình không
        if (targetUser?.connectionId !== user?.connectionId) {
            const remoteVideo = document.getElementById('remoteVideo');
            const remoteStream = remoteVideo.srcObject;

            if (remoteStream) {
                const videoTrack = remoteStream.getVideoTracks()[0];
                if (videoTrack) {
                    videoTrack.enabled = isEnabled; // Cập nhật trạng thái camera
                    console.log(`Camera for user ${connectionId} is now ${isEnabled ? 'on' : 'off'}`);
                }
            }
        }
    })
}

$(document).ready(function () {
    initSignalrHub();

    initVideo();

    eventInit();
});


const acceptCall = () => {
    hubConnection.invoke('AnswerCall', true, caller).catch(err => console.error(err));
    $('#callmodal').modal('hide');
};

const declineCall = () => {
    hubConnection.invoke('AnswerCall', false, caller).catch(err => console.error(err));
    $('#callmodal').modal('hide');
};

const matchingUser = () => {
    hubConnection.invoke("Matching").catch((err) => {
        console.error(err);
    });
}


const userJoin = (username) => {
    hubConnection.invoke("Join", username).catch((err) => {
        console.error(err);
    });
};

const callUser = (connectionId) => {
    hubConnection.invoke('call', {
        "connectionId": connectionId
    });
};

const endCall = (connectionId) => {
    // Ngừng video của người gọi
    if (remoteVideo.srcObject) {
        let remoteTracks = remoteVideo.srcObject.getTracks();
        remoteTracks.forEach(track => track.stop()); // Dừng tất cả các track từ video remote
        remoteVideo.srcObject = null; // Xóa video source
    }

    hubConnection.invoke('hangUp').catch((err) => {
        console.error(err);
    });

    msgContent.innerHTML = ''
};

const startMatching = () => {
    //document.getElementById('loadingMessage').style.display = 'none';
    loadingMessage.textContent = 'Matching, please wait...';
    document.getElementById('loadingSpinner').style.display = 'block';
    matchingUser()
}

const generateId = () => {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
};

function toggleCamera() {
    const btnIcon = toggleCameraButton.querySelector('.icon.bi');
    const classItem = btnIcon.classList.contains('bi-camera-video-fill')
    if (classItem) {
        btnIcon.classList.replace('bi-camera-video-fill', 'bi-camera-video-off-fill')
    }
    else {
        btnIcon.classList.replace('bi-camera-video-off-fill', 'bi-camera-video-fill')
    }

    const videoTrack = localStream.getVideoTracks()[0];
    if (videoTrack) {
        const isEnabled = !videoTrack.enabled;
        videoTrack.enabled = isEnabled; // Đảo ngược trạng thái hiện tại
        hubConnection.invoke('ToggleCamera', isEnabled);
        console.log(`Camera is now ${videoTrack.enabled ? 'on' : 'off'}`);
    }
}

function toggleMicrophone() {
    const btnIcon = toggleMicrophoneButton.querySelector('.icon.bi');
    const classItem = btnIcon.classList.contains('bi-mic-fill')
    if (classItem) {
        btnIcon.classList.replace('bi-mic-fill', 'bi-mic-mute-fill')
    }
    else {
        btnIcon.classList.replace('bi-mic-mute-fill', 'bi-mic-fill')
    }

    const audioTrack = localStream.getAudioTracks()[0];
    if (audioTrack) {
        const isEnabled = !audioTrack.enabled;
        audioTrack.enabled = isEnabled; // Đảo ngược trạng thái hiện tại
        hubConnection.invoke('ToggleMicrophone', isEnabled);

        console.log(`Microphone is now ${audioTrack.enabled ? 'on' : 'off'}`);
    }
}

const sendMsg = () => {
    const message = sendContentMsg.value
    if (message == '') {
        alert("Vui lòng nhập tin nhắn")
    }
    else {
        const curDate = new Date()
        const msgElementDOM = createMessageElement('sent', 'You', getFormattedDate(curDate), message)
        msgContent.appendChild(msgElementDOM)
        scrollToBottom(msgContent)
        sendContentMsg.value = ""

        hubConnection.invoke("SendMsg", message, curDate).catch((err) => {
            console.error(err);
        });
    }
}

function getFormattedDate(date) {
    const now = date;

    const day = String(now.getDate()).padStart(2, '0'); // Ngày
    const month = String(now.getMonth() + 1).padStart(2, '0'); // Tháng (tháng bắt đầu từ 0)
    const year = now.getFullYear(); // Năm

    const hours = String(now.getHours()).padStart(2, '0'); // Giờ (24 giờ)
    const minutes = String(now.getMinutes()).padStart(2, '0'); // Phút

    // Định dạng chuỗi theo yêu cầu
    return `${day}/${month}/${year} ${hours}:${minutes}`;
}

function scrollToBottom(element) {
    // Kiểm tra xem phần tử có đang ở dưới cùng không
    const isAtBottom = element.scrollHeight - element.scrollTop === element.clientHeight;

    // Nếu không ở dưới cùng, di chuyển xuống cuối
    if (!isAtBottom) {
        element.scrollTop = element.scrollHeight; // Di chuyển thanh cuộn xuống cuối
    }
}

const createMessageElement = (type, sender, time, content) => {
    // Tạo phần tử div cho tin nhắn
    const messageDiv = document.createElement('div');
    messageDiv.classList.add('message', type);

    // Tạo phần tử strong cho tên người gửi
    const senderStrong = document.createElement('strong');
    senderStrong.textContent = sender + ': ';

    // Tạo phần tử span cho thời gian
    const timestampSpan = document.createElement('span');
    timestampSpan.classList.add('timestamp');
    timestampSpan.textContent = `(${time})`;

    // Tạo phần tử div cho nội dung tin nhắn
    const msgContentDiv = document.createElement('div');
    msgContentDiv.classList.add('msg-content');
    msgContentDiv.textContent = content;

    // Xây dựng cấu trúc DOM
    messageDiv.appendChild(senderStrong);
    messageDiv.appendChild(timestampSpan);
    messageDiv.appendChild(msgContentDiv);

    return messageDiv; // Trả về đối tượng DOM
}