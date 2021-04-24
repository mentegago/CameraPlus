using System.Collections.Generic;
using HarmonyLib;
using CameraPlus.Utilities;

namespace CameraPlus.HarmonyPatches
{
    [HarmonyPatch(typeof(MultiplayerSessionManager),"Start")]
    internal class MultiplayerSessionManagerPatch
    {
        public static MultiplayerSessionManager Instance { get; private set; }
        static void Postfix(MultiplayerSessionManager __instance)
        {
            Instance = __instance;
            MultiplayerSession.Init(Instance);
            Logger.log.Info($"Success Find SessionManager");
        }
    }

    [HarmonyPatch(typeof(MultiplayerLobbyAvatarPlaceManager), nameof(MultiplayerLobbyAvatarPlaceManager.Activate))]
    internal class MultiplayerLobbyAvatarPlaceManagerPatch
    {
        public static MultiplayerLobbyAvatarPlaceManager Instance { get; private set; }
        public static List<MultiplayerLobbyAvatarPlace> LobbyAvatarPlaces = null;
        static void Postfix(MultiplayerLobbyAvatarPlaceManager __instance, List<MultiplayerLobbyAvatarPlace> ____allPlaces)
        {
            Instance = __instance;
            LobbyAvatarPlaces = ____allPlaces;
#if DEBUG
            Logger.log.Notice("Got MultiplayerLobbyAvatarPlaceManager");
#endif
        }
    }

    [HarmonyPatch(typeof(MultiplayerLobbyController), nameof(MultiplayerLobbyController.ActivateMultiplayerLobby))]
    internal class MultiplayerLobbyControllerPatch
    {
        public static MultiplayerLobbyController Instance { get; private set; }
        static void Postfix(MultiplayerLobbyController __instance)
        {
            Instance = __instance;
#if DEBUG
            Logger.log.Notice("Got MultiplayerLobbyController");
#endif
        }
    }
    [HarmonyPatch(typeof(MultiplayerPlayersManager), nameof(MultiplayerPlayersManager.BindPlayerFactories))]
    internal class MultiplayerPlayersManagerPatch
    {
        public static MultiplayerPlayersManager Instance { get; private set; }
        static void Postfix(MultiplayerPlayersManager __instance)
        {
            Instance = __instance;
#if DEBUG
            Logger.log.Notice("Got MultiplayerPlayersManager");
#endif
        }
    }
    [HarmonyPatch(typeof(MultiplayerScoreProvider), nameof(MultiplayerScoreProvider.Update))]
    internal class MultiplayerScoreProviderPatch
    {
        public static MultiplayerScoreProvider Instance = null;
        static void Postfix(MultiplayerScoreProvider __instance)
        {
            if (Instance == null)
            {
#if DEBUG
                Logger.log.Notice("Got MultiplayerScoreProvider");
#endif
                Instance = __instance;
            }
        }
    }
}