using UnityEngine;
using UnityEngine.Networking;
using Assets.Classes;

namespace Assets.Scripts
{
    public class ClientScript : MonoBehaviour
    {

        public void Start()
        {
            NetworkTransport.Init();

            var config = new ConnectionConfig();
            _reliableChannel = config.AddChannel(QosType.Reliable);
            var topology = new HostTopology(config, 1); // Only connect once

#if UNITY_EDITOR
            _mHostId = NetworkTransport.AddHostWithSimulator(topology, 200, 400);
#else
        _mHostId = NetworkTransport.AddHost(topology);
#endif
        }

        Rect _windowRect = new Rect(500, 20, 100, 50);
        string _ipField = System.Net.IPAddress.Loopback.ToString();
        string _portField = "25000";
        byte _reliableChannel;
        int _mHostId = -1;
        int _mConnectionId;
        Vector2 _scrollPos;
        string _sizeField = "32";
        string _receiveLabel;

        public void OnGUI()
        {
            _windowRect = GUILayout.Window(GetInstanceID(), _windowRect, MyWindow, "Client Window");
        }

        void MyWindow(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("IP");
            _ipField = GUILayout.TextField(_ipField);
            GUILayout.Label("Port");
            _portField = GUILayout.TextField(_portField);
            if (GUILayout.Button("Connect"))
            {
                byte error;
                var connectionId = NetworkTransport.Connect(_mHostId, _ipField, int.Parse(_portField), 0, out error);
                if (connectionId != 0) // Could go over total connect count
                    _mConnectionId = connectionId;
            }
            if (GUILayout.Button("Disconnect"))
            {
                byte error;
                var ret = NetworkTransport.Disconnect(_mHostId, _mConnectionId, out error);
                print("Disconnect " + ret + " error " + error);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Size");
            _sizeField = GUILayout.TextField(_sizeField);
            if (GUILayout.Button("Send"))
            {
                //TODO: test message
                Message testMsg = new Message(Enum_Action.Trigger);
                testMsg.AddData((byte)Enum_Trigger.Klappen);

                byte error;

                // Just send junk
                var ret = NetworkTransport.Send(_mHostId, _mConnectionId, _reliableChannel, testMsg.GetBuffer(), testMsg.GetSize(), out error);
                print("Send " + ret + " error " + error);
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.Height(200.0f), GUILayout.Width(400.0f));
            GUILayout.Label(_receiveLabel);
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear"))
            {
                _receiveLabel = "";
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        public void Update()
        {
            if (_mHostId == -1)
                return;
            int connectionId;
            int channelId;
            var buffer = new byte[1500];
            int receivedSize;
            byte error;
            var networkEvent = NetworkTransport.ReceiveFromHost(_mHostId, out connectionId, out channelId, buffer, 1500, out receivedSize, out error);
            if (networkEvent == NetworkEventType.Nothing)
                return;
            _receiveLabel += string.Format("{0} connectionId {1} channelId {2} receivedSize {3}\n", networkEvent, connectionId, channelId, receivedSize);
        }

        public void SendTrigger(Enum_Trigger trigger)
        {
            var msg = new Message(Enum_Action.Trigger);
            msg.AddData((byte)trigger);
            byte error;
            NetworkTransport.Send(_mHostId, _mConnectionId, _reliableChannel, msg.GetBuffer(), msg.GetSize(), out error);
        }
    }
}
