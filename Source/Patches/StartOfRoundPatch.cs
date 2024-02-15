using HarmonyLib;


namespace FuneePlugin
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
