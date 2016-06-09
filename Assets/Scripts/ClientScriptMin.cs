using System;
using Assets.Classes;
using Assets.Enums;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class ClientScriptMin : MonoBehaviour
    {
        private const string HostIp = "145.93.137.84";
        private const string PortField = "25000";

        private int _connectionId;

        public void Start()
        {
            NetworkTransport.Init();
            
            // create the configuration for the Client > Server connection
            // add 1 reliable channel to ensure that all message will arrive
            // create a topology for the host that allows a maximum of 1 connection.
            var config = new ConnectionConfig();
            _reliableChannel = config.AddChannel(QosType.Reliable);
            var topology = new HostTopology(config, 1);

            // Create a host for sending and recieving messages (simulate this if in unity editor)
#if UNITY_EDITOR
            _mHostId = NetworkTransport.AddHostWithSimulator(topology, 200, 400);
#else
        _mHostId = NetworkTransport.AddHost(topology);
#endif

            byte error;
            _connectionId = NetworkTransport.Connect(_mHostId, HostIp, int.Parse(PortField), 0, out error);
            if (_connectionId != 0) // Could go over total connect count
                _mConnectionId = _connectionId;
        }

        byte _reliableChannel;
        int _mHostId = -1;
        int _mConnectionId;
        
        public void Update()
        {
            if (_mHostId == -1) return;

            int connectionId, channelId, receivedSize;
            var buffer = new byte[64];
            byte error;
            var networkEvent = NetworkTransport.ReceiveFromHost(_mHostId, out connectionId, out channelId, buffer, 64, out receivedSize, out error);

            if (networkEvent == NetworkEventType.DataEvent)
            {
                MessageLogic(new Message(buffer, receivedSize));
            }
        }

        private static void MessageLogic(Message message)
        {
            switch (message.GetCommand())
            {
                case Enum_Action.Visual:
                    ProcessVisual((Visuals_Enum)message.data[0]);
                    break;
                default:
                    // client should only receive visual commands
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static void ProcessVisual(Visuals_Enum visual)
        {
            VisualProcessor.Instance.Display(visual);
        }

        public void SendTrigger(Enum_Trigger trigger)
        {
            var msg = new Message(Enum_Action.Trigger);
            msg.AddData((byte)trigger);
            byte error;
            NetworkTransport.Send(_mHostId, _mConnectionId, _reliableChannel, msg.GetBuffer(), msg.GetSize(), out error);
        }

        public void SendPosition(Vector3 position)
        {
            throw new NotImplementedException();

            var msg = new Message(Enum_Action.Position);
            msg.AddData((byte)position.x); // x, y and z should be 8 bytes each
            msg.AddData((byte)position.y);
            msg.AddData((byte)position.z);
            byte error;
            NetworkTransport.Send(_mHostId, _mConnectionId, _reliableChannel, msg.GetBuffer(), msg.GetSize(), out error);
        }
    }
}
