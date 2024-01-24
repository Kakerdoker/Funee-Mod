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

			MyLogger.Debug("There are " + UnfortunatePlayer.players.Count + " unfortunates:");
			foreach(PlayerControllerB player in UnfortunatePlayer.players)
			{
				MyLogger.Debug(player.playerUsername);
			}
		}

	}
}
