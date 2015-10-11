using ExitGames.Logging;
using ExitGames.Logging.Log4Net;
using log4net.Config;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TestPhotonLib.Common.CustomEventArgs;

namespace TestPhotonLib {
    public class MyServer : ApplicationBase {

        private readonly ILogger log = LogManager.GetCurrentClassLogger();

        protected override PeerBase CreatePeer(InitRequest initRequest) {
            return new UnityClient(initRequest.Protocol, initRequest.PhotonPeer);
        }

        protected override void Setup() {
            // log4net
            log4net.GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine(this.ApplicationRootPath, "log");
            var configFileInfo = new FileInfo(Path.Combine(this.BinaryPath, "log4net.config"));
            if (configFileInfo.Exists) {
                LogManager.SetLoggerFactory(Log4NetLoggerFactory.Instance);
                XmlConfigurator.ConfigureAndWatch(configFileInfo);
            }

            log.Debug("Server is ready!");
        }

        protected override void TearDown() {
            log.Debug("Server is stop!");
        }
    }
}
