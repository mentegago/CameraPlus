using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CameraPlus.HarmonyPatches;

namespace CameraPlus.Utilities
{
    internal static class MultiplayerSession
    {
        internal static MultiplayerSessionManager SessionManager { get; private set; }
        internal static List<IConnectedPlayer> connectedPlayers;
        internal static bool ConnectedMultiplay;
        internal static MultiplayerPlayersManager playersManager = null;
        internal static List<Transform> LobbyAvatarPlaceList;
        internal static void Init(MultiplayerSessionManager sessionManager)
        {
            connectedPlayers = new List<IConnectedPlayer>();
            LobbyAvatarPlaceList = new List<Transform>();
            ConnectedMultiplay = false;
            SessionManager = sessionManager;

            SessionManager.connectedEvent += OnSessionConnected;
            SessionManager.disconnectedEvent += OnSessionDisconnected;
            SessionManager.playerConnectedEvent += OnSessionPlayerConnected;
            SessionManager.playerDisconnectedEvent += OnSessionPlayerDisconnected;
        }

        internal static void Close()
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
            connectedPlayers.Add(SessionManager.localPlayer);
#if DEBUG
            Logger.log.Info($"ConnectedPlayer---------------");
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.log.Info($"ConnectedPlayer {connectedPlayers[i].userName},{connectedPlayers[i].sortIndex}");
#endif
            if (Plugin.cameraController.rootConfig.MultiplayerProfile != "" && Plugin.cameraController.rootConfig.ProfileSceneChange)
                CameraUtilities.ProfileChange(Plugin.cameraController.rootConfig.MultiplayerProfile);
            LoadLobbyAvatarPlace();
        }
        private static void OnSessionDisconnected(DisconnectedReason reason)
        {
            ConnectedMultiplay = false;
            connectedPlayers.Clear();
            LobbyAvatarPlaceList.Clear();
            Logger.log.Info($"SessionManager Disconnected {reason}");
            if (Plugin.cameraController.rootConfig.MenuProfile != "" && Plugin.cameraController.rootConfig.ProfileSceneChange)
                CameraUtilities.ProfileChange(Plugin.cameraController.rootConfig.MenuProfile);
        }
        private static void OnSessionPlayerConnected(IConnectedPlayer player)
        {
            connectedPlayers.Add(player);
            connectedPlayers = connectedPlayers.OrderBy(pl => pl.sortIndex)
                    .ToList();
#if DEBUG
            Logger.log.Info($"ConnectedPlayer---------------");
            for (int i = 0; i < connectedPlayers.Count; i++)
                Logger.log.Info($"ConnectedPlayer {connectedPlayers[i].userName},{connectedPlayers[i].sortIndex}");
#endif
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
            Logger.log.Info($"SessionManager PlayerDisconnected {player.userName},{player.sortIndex}");
        }

        internal static void LoadLobbyAvatarPlace()
        {
            try
            {
                Transform LobbyOffset;
                LobbyAvatarPlaceList.Clear();
                if (!MultiplayerLobbyAvatarPlaceManagerPatch.Instance) return;
                LobbyAvatarPlaceList.Add(MultiplayerLobbyAvatarPlaceManagerPatch.Instance.transform);
                foreach (MultiplayerLobbyAvatarPlace multiLobbyAvatarPlace in MultiplayerLobbyAvatarPlaceManagerPatch.LobbyAvatarPlaces)
                {
                    LobbyOffset = multiLobbyAvatarPlace.transform;
                    LobbyAvatarPlaceList.Add(LobbyOffset);
                }
                if (LobbyAvatarPlaceList.Count <= 1)
                {
                    LobbyAvatarPlaceList.Clear();
                    return;
                }
                List<Transform> Tr = ShiftLobbyPositionList(LocalPlayerSortIndex());
                if (Tr != null)
                    LobbyAvatarPlaceList = Tr;
                else
                    Logger.log.Info($"LobbyAvatarPlace SortError");
                for (int i = 0; i < LobbyAvatarPlaceList.Count; i++)
                    Logger.log.Notice($"Find Sorted LobbyAvatarPlace {i}: {LobbyAvatarPlaceList[i].position.x},{LobbyAvatarPlaceList[i].position.y},{LobbyAvatarPlaceList[i].position.z}");
            }
            catch
            {
                Logger.log.Error($"Unable to LoadLobbyAvatarPlace");
            }
        }
        private static int LocalPlayerSortIndex()
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
        private static List<Transform> ShiftLobbyPositionList(int shiftValue)
        {
            if (shiftValue < 0 || shiftValue >= LobbyAvatarPlaceList.Count) return null;

            List<Transform> result = LobbyAvatarPlaceList;
            for (int i = 0; i < shiftValue; i++)
            {
                result.Insert(0, result[result.Count - 1]);
                result.RemoveAt(result.Count - 1);
            }
            return result;
        }
    }
}
