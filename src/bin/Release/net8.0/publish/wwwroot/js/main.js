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

var users = [];
var user = null;
var caller = null;

// init functions

const eventInit = () => {
	document.getElementById('startMatchingButton').addEventListener('click', function() {
		startMatching();
	});

	document.getElementById('dropCallButton').addEventListener('click', function() {
		endCall();
	});
}

const userOnlineNumber = (userList)=>{

    $('#usersdata li.user').remove();
    // Calculate the number of online users
    var onlineUserCount = userList.length;

    // Create the HTML to display the number of online users
    var onlineUsersHtml = '<div class="online-users-count">Number of avaiable users: ' + onlineUserCount + '</div>';

    // Append the HTML to the usersdata element
    $('#usersdata').html(onlineUsersHtml);
}

const setupRTCConnection = async (targetUserConnectionId) => {

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
}

const initVideo = ()=>{
    remoteVideo.oncanplay = function() {
		loadingMessage.style.display = 'none';
	};

	remoteVideo.onerror = function() {
		loadingMessage.textContent = 'Video not available.';
	};
}

async function getMedia() {
    try {
        let localStream = await navigator.mediaDevices.getUserMedia({
            video: true,
            audio: true,
        });
    
        localVideo.srcObject = localStream;

        peerConnection = new RTCPeerConnection(configuration);

        let tracks = localStream.getTracks();

        peerConnection.ontrack = event => {
            const stream = event.streams[0];
            if (!remoteVideo.srcObject || remoteVideo.srcObject.id !== stream.id) {
                remoteVideo.srcObject = stream;
            }
        };

        tracks.forEach(track => peerConnection.addTrack(track, localStream));

    } catch (error) {
      console.error('Error getting user media:', error);
    }
  }

const initSignalrHub = ()=>{

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

	hubConnection.on("ReceiveData", data => {

        const message = JSON.parse(data);
    
        if (message.sdp) {
            peerConnection.setRemoteDescription(new RTCSessionDescription(message.sdp))
                .then(() => {
                    if (peerConnection.remoteDescription.type === 'offer') {
                        return peerConnection.createAnswer();
                    }
                })
                .then(answer => {
                    return peerConnection.setLocalDescription(answer);
                })
                .then(() => {
                    hubConnection.invoke('sendData', JSON.stringify({ 'sdp': peerConnection.localDescription }), targetUserConnectionId.connectionId)
                        .catch(err => console.error(err));
                })
                .catch(err => console.error(err));
        } else if (message.candidate) {
            peerConnection.addIceCandidate(new RTCIceCandidate(message.candidate))
                .catch(err => console.error(err));
        }
    });


	hubConnection.on('incomingCall', (callingUser) => {
        //seting up caller
		caller = callingUser;
		$('#callmodal').attr('data-callinguser', `${callingUser.username}`);
		$('#callmodal .modal-body').text(`${callingUser.username} want to match...`);
		$('#callmodal').modal('show');
	});
    
    hubConnection.on("UpdateAvaiableStatus", (isAvaible)=>{
        document.getElementById('startMatchingButton').disabled = (isAvaible == false);
        document.getElementById('dropCallButton').disabled = (isAvaible == true);
    })

    hubConnection.on("matched",async (targetUserConnectionId)=>{
        //await setupRTCConnection(targetUserConnectionId);
        let localStream = await navigator.mediaDevices.getUserMedia({
            video: true,
            audio: true,
        });
    
        remoteVideo.srcObject = localStream;
    })
}

$(document).ready(function() {

	eventInit();

    initVideo();
	
    initSignalrHub();

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
	hubConnection.invoke('hangUp');
};


const startMatching = () => {
	document.getElementById('loadingMessage').style.display = 'none';
	matchingUser()
}

const generateId = () => {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
};

