using Photon.SocketServer;
using Photon.SocketServer.Rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestPhotonLib.Common;
using TestPhotonLib.Operations;

namespace TestPhotonLib {
    public class ChatInfo {

        UnityClient UnityClient { get; set; }
        OperationRequest OperationRequest { get; set; }
        SendParameters SendParameters { get; set; }

        public ChatInfo(UnityClient client, OperationRequest operationRequest, SendParameters sendParameters) {
            UnityClient = client;
            OperationRequest = operationRequest;
            SendParameters = sendParameters;
        }

        public void TryAuthorization() {
            var loginRequest = new Login(UnityClient.Protocol, OperationRequest);
            UnityClient.CharacterName = loginRequest.CharacterName;

            if (!loginRequest.IsValid) {
                UnityClient.SendOperationResponse(loginRequest.GetResponse(ErrorCode.InvalidParameters), SendParameters);
                return;
            }

            if (AccountService.Instance.IsContain(UnityClient.CharacterName)) {
                UnityClient.SendOperationResponse(loginRequest.GetResponse(ErrorCode.NameIsExist), SendParameters);
                return;
            }

            AccountService.Instance.AddClient(UnityClient);

            var response = new OperationResponse(OperationRequest.OperationCode);
            UnityClient.SendOperationResponse(response, SendParameters);
        }

        public void TrySendChatMessage() {
            var chatRequest = new ChatMessage(UnityClient.Protocol, OperationRequest);

            if (!chatRequest.IsValid) {
                UnityClient.SendOperationResponse(chatRequest.GetResponse(ErrorCode.InvalidParameters), SendParameters);
                return;
            }

            string message = chatRequest.Message;
            message = UnityClient.CharacterName + ": " + message;
            ChatLobby.Instance.AddMessage(message);

            var eventData = new EventData((byte)EventCode.ChatMessage);
            eventData.Parameters = new Dictionary<byte, object>() { { (byte)ParameterCode.ChatMessage, message } };
            eventData.SendTo(AccountService.Instance.Clients, SendParameters);
        }

        public void TrySendRecentChatMessage() {
            var eventDataMessages = new EventData((byte)EventCode.ChatMessage);
            string lastMessages = ChatLobby.Instance.GetRecentMessages().Aggregate((i, j) => i + "\r\n" + j);
            eventDataMessages.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.ChatMessage, lastMessages } };
            eventDataMessages.SendTo(new UnityClient[] { UnityClient }, SendParameters);
        }

        public void TrySendAmountOfPlayer() {
            var eventAmountOfPlayer = new EventData((byte)EventCode.AmountOfPlayers);
            int amountOfPlayers = AccountService.Instance.GetAllClient();
            eventAmountOfPlayer.Parameters = new Dictionary<byte, object> { { (byte)ParameterCode.AmountOfPlayer, amountOfPlayers } };
            eventAmountOfPlayer.SendTo(AccountService.Instance.Clients, SendParameters);
        }

    }
}
