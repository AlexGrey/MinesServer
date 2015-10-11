using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using TestPhotonLib.Common;
using TestPhotonLib.Operations;

namespace TestPhotonLib {

    public class UnityClient : PeerBase {

        private readonly ILogger log = LogManager.GetCurrentClassLogger();
        public string CharacterName { get; set; }

        public UnityClient(IRpcProtocol protocol, IPhotonPeer unmanagedPeer) : base (protocol, unmanagedPeer) {
            log.Debug("Player connection ip:" + unmanagedPeer.GetRemoteIP());
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            World.Instance.RemoveClient(this);
            log.Debug("Disconnected!");
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            switch (operationRequest.OperationCode) {
                case (byte)OperationCode.Login:
                    var loginRequest = new Login(Protocol, operationRequest);

                    if (!loginRequest.IsValid) {
                        SendOperationResponse(loginRequest.GetResponse(ErrorCode.InvalidParameters), sendParameters);
                        return;
                    }

                    CharacterName = loginRequest.CharacterName;

                    if (World.Instance.IsContain(CharacterName)) {
                        SendOperationResponse(loginRequest.GetResponse(ErrorCode.NameIsExist), sendParameters);
                        return;
                    }

                    World.Instance.AddClient(this);

                    var response = new OperationResponse(operationRequest.OperationCode);
                    SendOperationResponse(response, sendParameters);

                    log.Info("user with name: " + CharacterName);
                    break;

                case (byte)OperationCode.SendChatMessage:
                    var chatRequest = new ChatMessage(Protocol, operationRequest);

                    if (!chatRequest.IsValid) {
                        SendOperationResponse(chatRequest.GetResponse(ErrorCode.InvalidParameters), sendParameters);
                        return;
                    }

                    string message = chatRequest.Message;
                    message = CharacterName + ": " + message;
                    Chat.Instance.AddMEssage(message);

                    var eventData = new EventData((byte)EventCode.ChatMessage);
                    eventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
                    eventData.SendTo(World.Instance.Clients, sendParameters);

                    break;
                case (byte)OperationCode.GetRecentChatMessage:
                    var eventDataMessages = new EventData((byte)EventCode.ChatMessage);
                    string lastMessages = Chat.Instance.GetRecentMessages().Aggregate((i, j) => i + "\r\n" + j);
                    eventDataMessages.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.ChatMessage, lastMessages } };
                    eventDataMessages.SendTo(new UnityClient[] { this }, sendParameters);
                    break;
                default:
                    log.Debug("Unknown Operation!" + operationRequest.OperationCode);
                    break;
            }
        }
    }
}
