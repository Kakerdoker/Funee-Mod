﻿using HarmonyLib;
using GameNetcodeStuff;

namespace FuneePlugin
{

	[HarmonyPatch(typeof(HoarderBugAI))]
	static internal class HoarderBugAIPatch
	{
		[HarmonyPrefix]
		[HarmonyPatch("DetectAndLookAtPlayers")]

		//If the hoarder bug looks at unfortunate player it is immediately angered. It returns to normal 1 second after it stops seeing the player.
		static void DetectAndLookAtPlayersPatch(ref PlayerControllerB ___watchingPlayer, ref PlayerControllerB ___angryAtPlayer, ref float ___angryTimer, ref EnemyAI __instance)
		{
			if(UnfortunatePlayer.players.Contains(___watchingPlayer))
            {
				__instance.SwitchToBehaviourState(2);
				___angryAtPlayer = ___watchingPlayer;
				if (___angryTimer < 1f)
					___angryTimer = 1f;
			}
		}




	}
}
