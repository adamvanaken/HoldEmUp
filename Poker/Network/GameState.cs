using Cards;
using Solitare.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Solitare.Network
{
    [DataContract]
    public class GameState
    {
        [DataMember]
        public Player[] Players;

        [DataMember]
        public int Pot;

        [DataMember]
        public int PendingPot;

        [DataMember]
        public Card[] Flop;

        [DataMember]
        public int Round;

        public GameState()
        {

        }
    }
}
