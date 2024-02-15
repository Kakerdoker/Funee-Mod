using HarmonyLib;
using GameNetcodeStuff;

namespace FuneePlugin
{

	[HarmonyPatch(typeof(JesterAI))]
	static internal class JesterAIPatch
	{

		static float initialBeginCrankingTimer;
		static float initialPopUpTimer;

		//The idea here is that we take the timer before the game does all of the calculations and substract it from the value after the calculations, so we don't need to do all of them ourselvers.
		//Then we substarct the delta of both timers from the timers themselves resulting in the jester cranking two times faster if it is targeting an unfortunate player.
		[HarmonyPrefix]
		[HarmonyPatch(nameof(JesterAI.Update))]
		static void UpdatePrePatch(ref float ___beginCrankingTimer, ref float ___popUpTimer)
		{
			initialBeginCrankingTimer = ___beginCrankingTimer;
			initialPopUpTimer = ___popUpTimer;
		}

		[HarmonyPostfix]
		[HarmonyPatch(nameof(JesterAI.Update))]
		static void UpdatePostPatch(ref float ___beginCrankingTimer, ref float ___popUpTimer, ref PlayerControllerB ___targetPlayer)
		{
			if (UnfortunatePlayer.players.Contains(___targetPlayer))
            {
				float crankingDelta =  initialBeginCrankingTimer - ___beginCrankingTimer;
				float popUpDelta = initialPopUpTimer - ___popUpTimer;

				___beginCrankingTimer -= crankingDelta;
				___popUpTimer -= popUpDelta;
			}
		}


	}
}
