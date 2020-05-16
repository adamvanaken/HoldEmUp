using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Solitare.Network
{
    [DataContract]
    public class ServerMessage
    {
        [DataMember]
        public ServerMessageType Type;

        [DataMember]
        public string Value;
    }

    public enum ServerMessageType
    {
        Ack = 0,
        Update = 1
    }

}
