using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPhotonLib.Common.CustomEventArgs {
    public class AmountOfPlayersEventArgs:EventArgs {

        public int AmountOfPlayers { get; private set; }

        public AmountOfPlayersEventArgs(int amount) {
            AmountOfPlayers = amount;
        }
    }
}
