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
        public static MultiplayerPlayersManager playersManager = null;
        public static MultiplayerLobbyController LobbyContoroller = null;
        public static List<Transform> LobbyAvatarPlaceList;

        public static void Init()
        {
            connectedPlayers = new List<IConnectedPlayer>();
            LobbyAvatarPlaceList = new List<Transform>();
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
            //playersManager = Resources.FindObjectsOfTypeAll<MultiplayerPlayersManager>().FirstOrDefault();

            LobbyContoroller = Resources.FindObjectsOfTypeAll<MultiplayerLobbyController>().FirstOrDefault();
            if (LobbyContoroller == null)
                Logger.Log($"Unable to get LobbyContoroller", LogLevel.Error);
            connectedPlayers.Add(SessionManager.localPlayer);
            Logger.Log($"ConnectedPlayer---------------", LogLevel.Info);
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.Log($"ConnectedPlayer {connectedPlayers[i].userName},{connectedPlayers[i].sortIndex}", LogLevel.Info);

            if (Plugin.Instance._rootConfig.MultiplayerProfile != "" && Plugin.Instance._rootConfig.ProfileSceneChange)
                Plugin.Instance._profileChanger.ProfileChange(Plugin.Instance._rootConfig.MultiplayerProfile);
            LoadLobbyAvatarPlace();
        }
        private static void OnSessionDisconnected(DisconnectedReason reason)
        {
            ConnectedMultiplay = false;
            connectedPlayers.Clear();
            LobbyAvatarPlaceList.Clear();
            Logger.Log($"SessionManager Disconnected {reason}", LogLevel.Info);
            if (Plugin.Instance._rootConfig.MenuProfile != "" && Plugin.Instance._rootConfig.ProfileSceneChange)
                Plugin.Instance._profileChanger.ProfileChange(Plugin.Instance._rootConfig.MenuProfile);
        }
        private static void OnSessionPlayerConnected(IConnectedPlayer player)
        {
            //if (playersManager==null)
            //    playersManager = Resources.FindObjectsOfTypeAll<MultiplayerPlayersManager>().FirstOrDefault();

            connectedPlayers.Add(player);
            connectedPlayers = connectedPlayers.OrderBy(pl => pl.sortIndex)
                    .ToList();

            Logger.Log($"ConnectedPlayer---------------", LogLevel.Info);
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.Log($"ConnectedPlayer {connectedPlayers[i].userName},{connectedPlayers[i].sortIndex}", LogLevel.Info);
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

        public static void LoadLobbyAvatarPlace()
        {
            try
            {
                Transform LobbyOffset;
                LobbyAvatarPlaceList.Clear();
                foreach (MultiplayerLobbyAvatarPlace multiLobbyAvatarPlace in Resources.FindObjectsOfTypeAll<MultiplayerLobbyAvatarPlace>())
                {
                    if (multiLobbyAvatarPlace.isActiveAndEnabled)
                    {
                        LobbyOffset = multiLobbyAvatarPlace.transform;
                        LobbyAvatarPlaceList.Add(LobbyOffset);
                    }
                }
                LobbyAvatarPlaceList = LobbyAvatarPlaceList.GroupBy(p => p.position)
                                                    .Select(g => g.First())
                                                    .ToList();
                if (LobbyAvatarPlaceList.Count <= 1)
                {
                    LobbyAvatarPlaceList.Clear();
                    return;
                }
                List<Transform> SortAvatarPlacetList = LobbyAvatarPlaceList.OrderBy(tr => tr.position.z).ToList();
                LobbyAvatarPlaceList.Clear();
                for (int i=0; i < SortAvatarPlacetList.Count; i++)
                {
                    if (SortAvatarPlacetList[i].position.x >= 0)
                        LobbyAvatarPlaceList.Add(SortAvatarPlacetList[i]);
                }
                for (int i = SortAvatarPlacetList.Count-1; i > 0; i--)
                {
                    if (SortAvatarPlacetList[i].position.x < 0)
                        LobbyAvatarPlaceList.Add(SortAvatarPlacetList[i]);
                }

                List<Transform> Tr= ShiftLobbyPositionList(LocalPlayerSortIndex());
                if (Tr != null) LobbyAvatarPlaceList = Tr;
                else
                    Logger.Log($"LobbyAvatarPlace SortError", LogLevel.Info);
                for (int i = 0; i < LobbyAvatarPlaceList.Count; i++)
                    Logger.Log($"Find LobbyAvatarPlace {i}: {LobbyAvatarPlaceList[i].position.x},{LobbyAvatarPlaceList[i].position.y},{LobbyAvatarPlaceList[i].position.z}", LogLevel.Notice);
            }
            catch {
                Logger.Log($"Unable to LoadLobbyAvatarPlace", LogLevel.Error);
            }
        }
        public static int LocalPlayerSortIndex()
        {
            int result = 0;
            foreach (IConnectedPlayer player in connectedPlayers)
            {
                if (player.isMe)
                {
                    result = player.sortIndex;
                    break;
                }
            }
            return result;
        }
        public static List<Transform> ShiftLobbyPositionList(int shiftValue)
        {
            if (shiftValue < 0 || shiftValue >= LobbyAvatarPlaceList.Count) return null;

            List<Transform> result = LobbyAvatarPlaceList;
            for (int i=0; i < shiftValue ; i++)
            {
                result.Insert(0,result[result.Count-1]);
                result.RemoveAt(result.Count-1);
            }
            return result;
        }
    }
}
