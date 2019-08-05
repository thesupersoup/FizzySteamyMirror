using UnityEngine;
using System;
using Steamworks;
using Steamworks.Data;

namespace Mirror.FizzySteam
{
    public class Common
    {

        protected enum SteamChannels : int
        {
            SEND_DATA,
            SEND_INTERNAL = 100
        }

        protected enum InternalMessages : byte
        {
            CONNECT,
            ACCEPT_CONNECT,
            DISCONNECT
        }

        public static float secondsBetweenPolls = 0.03333f;

        //this is a callback from steam that gets registered and called when the server receives new connections
        // private Callback<P2PSessionRequest_t> callback_OnNewConnection = null;
        //this is a callback from steam that gets registered and called when the ClientConnect fails
        // private Callback<P2PSessionConnectFail_t> callback_OnConnectFail = null;

        readonly static protected byte[] connectMsgBuffer = new byte[] { (byte)InternalMessages.CONNECT };
        readonly static protected byte[] acceptConnectMsgBuffer = new byte[] { (byte)InternalMessages.ACCEPT_CONNECT };
        readonly static protected byte[] disconnectMsgBuffer = new byte[] { (byte)InternalMessages.DISCONNECT };
        public static P2PSend[] channels;
		P2Packet? packet = null;

        readonly static protected uint maxPacketSize = 1048576;
        readonly protected byte[] receiveBufferInternal = new byte[1];

        protected void Deinitialize()
        {
			SteamNetworking.OnP2PSessionRequest -= OnNewConnection;
			SteamNetworking.OnP2PConnectionFailed -= OnConnectFail;
		}

        protected virtual void Initialize()
        {
            Debug.Log("initialise");
            /*
            nextConnectionID = 1;

            steamConnectionMap = new SteamConnectionMap();

            steamNewConnections = new Queue<int>();

            serverReceiveBufferPendingConnectionID = -1;
            serverReceiveBufferPending = null;
            */

            if ( Steamworks.SteamClient.IsValid )
            {
				SteamNetworking.OnP2PSessionRequest += OnNewConnection;
				SteamNetworking.OnP2PConnectionFailed += OnConnectFail;

				/*if (callback_OnNewConnection == null)
                {
                    Debug.Log("initialise callback 1");
                    callback_OnNewConnection = Callback<P2PSessionRequest_t>.Create(OnNewConnection);
                }
                if (callback_OnConnectFail == null)
                {
                    Debug.Log("initialise callback 2");

                    callback_OnConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnConnectFail);
                }*/
            }
            else
            {
                Debug.LogError("STEAM NOT Initialized so couldnt integrate with P2P");
                return;
            }
        }

        protected void OnNewConnection( SteamId id )
        {
            Debug.Log("OnNewConnection");
            OnNewConnectionInternal( id );
        }

        protected virtual void OnNewConnectionInternal( SteamId id ) { Debug.Log("OnNewConnectionInternal"); }

        protected virtual void OnConnectFail( SteamId id, P2PSessionError error )
        {
            Debug.Log("OnConnectFail " + id + " " + error );
            throw new Exception("Failed to connect");
        }

        protected void SendInternal( SteamId host, byte[] msgBuffer )
        {
            if ( !Steamworks.SteamClient.IsValid )
            {
                throw new ObjectDisposedException("Steamworks");
            }
            SteamNetworking.SendP2PPacket(host, msgBuffer, msgBuffer.Length, (int)SteamChannels.SEND_INTERNAL, P2PSend.Reliable);
        }

        protected bool ReceiveInternal(out uint readPacketSize, out SteamId clientSteamID)
        {
			readPacketSize = 0;
			clientSteamID = new SteamId();

            if ( !Steamworks.SteamClient.IsValid )
            {
                throw new ObjectDisposedException("Steamworks");
            }

			packet = SteamNetworking.ReadP2PPacket((int)SteamChannels.SEND_INTERNAL);

			if( packet != null )
			{
				readPacketSize = (uint)packet.Value.Data.Length;
				clientSteamID = packet.Value.SteamId;
			}

			return packet != null;
        }

        protected void Send( SteamId host, byte[] msgBuffer, P2PSend sendType, int channel)
        {
            if ( !Steamworks.SteamClient.IsValid )
            {
                throw new ObjectDisposedException("Steamworks");
            }
            if (channel >= channels.Length) {
                channel = 0;
            }
            SteamNetworking.SendP2PPacket(host, msgBuffer, msgBuffer.Length, channel, sendType );
        }

        protected bool Receive(out uint readPacketSize, out SteamId clientSteamID, out byte[] receiveBuffer, int channel)
        {
			readPacketSize = 0;
			clientSteamID = new SteamId();
			receiveBuffer = null;

            if ( !Steamworks.SteamClient.IsValid )
			{
                throw new ObjectDisposedException("Steamworks");
            }

            if ( SteamNetworking.IsP2PPacketAvailable( channel ) )
            {
				packet = SteamNetworking.ReadP2PPacket( channel );

				if( packet != null )
				{
					readPacketSize = (uint)packet.Value.Data.Length;
					clientSteamID = packet.Value.SteamId;
					receiveBuffer = packet.Value.Data;
				}

                return packet != null;
            }

            return false;
        }

        protected void CloseP2PSessionWithUser( SteamId clientSteamID )
        {
            if ( !Steamworks.SteamClient.IsValid )
            {
                throw new ObjectDisposedException("Steamworks");
            }
            SteamNetworking.CloseP2PSessionWithUser( clientSteamID );
        }

        public uint GetMaxPacketSize( P2PSend sendType )
        {
            switch (sendType)
            {
                case P2PSend.Unreliable:
                case P2PSend.UnreliableNoDelay:
                    return 1200; //UDP like - MTU size.

                case P2PSend.Reliable:
                case P2PSend.ReliableWithBuffering:
                    return maxPacketSize; //Reliable message send. Can send up to 1MB of data in a single message.

                default:
                    Debug.LogError("Unknown type so uknown max size");
                    return 0;
            }

        }

        protected P2PSend channelToSendType(int channelId)
        {
            if (channelId >= channels.Length) {
                Debug.LogError("Unknown channel id, please set it up in the component, will now send reliably");
                return P2PSend.Reliable;
            }
            return channels[channelId];
        }

    }
}
