using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPhotonLib.Common {

    public enum OperationCode:byte {
        Login,
        SendChatMessage,
        GetRecentChatMessage,
    }
}
