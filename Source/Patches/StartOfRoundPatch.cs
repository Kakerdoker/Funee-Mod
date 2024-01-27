using HarmonyLib;


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
			UnfortunatePlayer.Synchronize();
		}
	}
}
