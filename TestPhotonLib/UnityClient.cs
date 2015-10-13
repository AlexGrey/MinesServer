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

        public ChatInfo chatInfo;

        public UnityClient(IRpcProtocol protocol, IPhotonPeer unmanagedPeer) : base (protocol, unmanagedPeer) {
            log.Debug("Player connection ip:" + unmanagedPeer.GetRemoteIP());
        }

        protected override void OnDisconnect(DisconnectReason reasonCode, string reasonDetail) {
            AccountService.Instance.RemoveClient(this);
            log.Debug("Disconnected!");
        }

        protected override void OnOperationRequest(OperationRequest operationRequest, SendParameters sendParameters) {
            chatInfo = new ChatInfo(this, operationRequest, sendParameters);

            switch (operationRequest.OperationCode) {
                case (byte)OperationCode.Login:
                    chatInfo.TryAuthorization();
                    break;
                case (byte)OperationCode.SendChatMessage:
                    chatInfo.TrySendChatMessage();
                    break;
                case (byte)OperationCode.GetRecentChatMessage:
                    chatInfo.TrySendRecentChatMessage();
                    break;
                case (byte)OperationCode.GetAmountOfPlayers:
                    chatInfo.TrySendAmountOfPlayer();
                    break;
                default:
                    log.Debug("Unknown Operation!" + operationRequest.OperationCode);
                    break;
            }
        }
    }
}
