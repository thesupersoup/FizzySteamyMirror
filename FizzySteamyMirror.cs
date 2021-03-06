using UnityEngine;
using Steamworks;
using System;

namespace Mirror.FizzySteam
{
    [HelpURL("https://vis2k.github.io/Mirror/Transports/Fizzy")]
    public class FizzySteamyMirror : Transport
    {
        protected FizzySteam.Client client = new FizzySteam.Client();
        protected FizzySteam.Server server = new FizzySteam.Server();
        public float messageUpdateRate = 0.03333f;
        public P2PSend[] channels = new P2PSend[2] { P2PSend.Reliable, P2PSend.Unreliable };

        private void Start()
        {
            Common.secondsBetweenPolls = messageUpdateRate;
            if (channels == null) {
                channels = new P2PSend[2] { P2PSend.Reliable, P2PSend.Unreliable };
            }
            channels[0] = P2PSend.Reliable;
            Common.channels = channels;
        }

        public FizzySteamyMirror()
        {
            // dispatch the events from the server
            server.OnConnected += (id) => OnServerConnected?.Invoke(id);
            server.OnDisconnected += (id) => OnServerDisconnected?.Invoke(id);
            server.OnReceivedData += (id, data) => OnServerDataReceived?.Invoke(id, new ArraySegment<byte>(data));
            server.OnReceivedError += (id, exception) => OnServerError?.Invoke(id, exception);

            // dispatch events from the client
            client.OnConnected += () => OnClientConnected?.Invoke();
            client.OnDisconnected += () => OnClientDisconnected?.Invoke();
            client.OnReceivedData += (data) => OnClientDataReceived?.Invoke(new ArraySegment<byte>(data));
            client.OnReceivedError += (exception) => OnClientError?.Invoke(exception);

            Debug.Log("FizzySteamyMirror initialized!");
        }

        // client
        public override bool ClientConnected() { return client.Connected; }
        public override void ClientConnect(string address) { client.Connect(address); }
        public override bool ClientSend(int channelId, byte[] data) { return client.Send(data, channelId); }
        public override void ClientDisconnect() { client.Disconnect(); }

        // server
        public override bool ServerActive() { return server.Active; }
        public override void ServerStart()
        {
            server.Listen();
        }

        public virtual void ServerStartWebsockets(string address, int port, int maxConnections)
        {
            Debug.LogError("FizzySteamyMirror.ServerStartWebsockets not possible!");
        }

        public override bool ServerSend(int connectionId, int channelId, byte[] data) { return server.Send(connectionId, data, channelId); }

        public override bool ServerDisconnect(int connectionId)
        {
            return server.Disconnect(connectionId);
        }

        public override string ServerGetClientAddress(int connectionId) { return server.ServerGetClientAddress(connectionId); }
        public override void ServerStop() { server.Stop(); }

        // common
        public override void Shutdown()
        {
            client.Disconnect();
            server.Stop();
        }

        public override int GetMaxPacketSize(int channelId)
        {
            switch (channelId)
            {
                case Channels.DefaultUnreliable:
                    return 1200; //UDP like - MTU size.

                case Channels.DefaultReliable:
                    return 1048576; //Reliable message send. Can send up to 1MB of data in a single message.

                default:
                    Debug.LogError("Unknown channel so uknown max size");
                    return 0;
            }
        }
    }
}
