using HarmonyLib;
using GameNetcodeStuff;

namespace FuniPlugin
{

	[HarmonyPatch(typeof(StartOfRound))]
	static internal class StartOfRoundPatch
	{

		//Initialize the list of unfortunate players after game starts.
		[HarmonyPatch(nameof(StartOfRound.StartGame))]
		[HarmonyPostfix]
		static void StartGamePatch()
		{
			#if DEBUG
			UnfortunatePlayer.players = new() { StartOfRound.Instance.allPlayerObjects[0].GetComponent<PlayerControllerB>() };
			#else
			UnfortunatePlayer.Init();
			#endif

			MyLogger.Debug("There are " + UnfortunatePlayer.players.Count + " unfortunates:");
			foreach(PlayerControllerB player in UnfortunatePlayer.players)
			{
				MyLogger.Debug(player.playerUsername);
			}
		}

	}
}
