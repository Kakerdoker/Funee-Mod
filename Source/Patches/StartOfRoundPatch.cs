using HarmonyLib;
using GameNetcodeStuff;

namespace FuniPlugin
{

	[HarmonyPatch(typeof(StartOfRound))]
    internal class StartOfRoundPatch
    {

        //Initialize the list of unfortunate players after game starts.
		[HarmonyPatch(nameof(StartOfRound.StartGame))]
		[HarmonyPostfix]
        static void StartGamePatch()
        {
            UnfortunatePlayer.Init();

            MyLogger.mls.LogInfo("There are " + UnfortunatePlayer.players.Count + " unfortunates:");
            foreach(PlayerControllerB player in UnfortunatePlayer.players)
            {
                MyLogger.mls.LogInfo(player.playerUsername);
            }
        }

    }
}
