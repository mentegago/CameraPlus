using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using LogLevel = IPA.Logging.Logger.Level;

namespace CameraPlus
{
    public static class MultiplayerSession
    {
        public static MultiplayerSessionManager SessionManager { get; private set; }
        public static List<IConnectedPlayer> connectedPlayers;
        public static bool ConnectedMultiplay;
        public static void Init()
        {
            connectedPlayers = new List<IConnectedPlayer>();
            ConnectedMultiplay = false;
            SessionManager = Resources.FindObjectsOfTypeAll<MultiplayerSessionManager>().FirstOrDefault();
            if (SessionManager == null)
                Logger.Log($"Unable to get MultiplayerSessionManager", LogLevel.Error);
            else
            {
                Logger.Log($"Success Find SessionManager", LogLevel.Info);

                SessionManager.connectedEvent += OnSessionConnected;
                SessionManager.disconnectedEvent += OnSessionDisconnected;
                SessionManager.playerConnectedEvent += OnSessionPlayerConnected;
                SessionManager.playerDisconnectedEvent += OnSessionPlayerDisconnected;
            }
        }

        public static void Close()
        {
            ConnectedMultiplay = false;
            if (SessionManager != null)
            {
                SessionManager.connectedEvent -= OnSessionConnected;
                SessionManager.disconnectedEvent -= OnSessionDisconnected;
                SessionManager.playerConnectedEvent -= OnSessionPlayerConnected;
                SessionManager.playerDisconnectedEvent -= OnSessionPlayerDisconnected;
            }
        }

        private static void OnSessionConnected()
        {
            ConnectedMultiplay = true;
            connectedPlayers.Clear();
            Logger.Log($"SessionManager Connected", LogLevel.Info);
        }
        private static void OnSessionDisconnected(DisconnectedReason reason)
        {
            ConnectedMultiplay = false;
            connectedPlayers.Clear();
            Logger.Log($"SessionManager Disconnected {reason}", LogLevel.Info);
        }
        private static void OnSessionPlayerConnected(IConnectedPlayer player)
        {
            connectedPlayers.Insert(0, player);
            connectedPlayers = connectedPlayers.OrderBy(pl => pl.isMe)
                    .ThenBy(pl => pl.sortIndex)
                    .ToList();
            Logger.Log($"--ConnectionList--");
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.Log($"{connectedPlayers[i].sortIndex},{connectedPlayers[i].userName}");
            Logger.Log($"SessionManager PlayerConnected {player.userName},{player.sortIndex}", LogLevel.Info);
        }
        private static void OnSessionPlayerDisconnected(IConnectedPlayer player)
        {
            foreach (IConnectedPlayer p in connectedPlayers.ToArray())
            {
                if (p.userId == player.userId)
                {
                    connectedPlayers.Remove(p);
                    break;
                }
            }
            Logger.Log($"SessionManager PlayerDisconnected {player.userName},{player.sortIndex}", LogLevel.Info);
        }
    }
}
