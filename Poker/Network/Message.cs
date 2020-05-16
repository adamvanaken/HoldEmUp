using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Solitare.Network
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public MessageType Type;

        [DataMember]
        public string Value;

        [DataMember]
        public Guid Uuid;
    }

    public enum MessageType
    {
        PollState = 0,
        Join = 1,
        Bet = 2,
        Fold = 3,
        ShowHand = 4, 
        AdvanceFlop = 5,
        Leave = 6,
        DeclareWinner = 7
    }

}
