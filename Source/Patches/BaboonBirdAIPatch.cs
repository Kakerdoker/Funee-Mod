using UnityEngine;
using HarmonyLib;
using GameNetcodeStuff;

namespace FuneePlugin
{

	[HarmonyPatch(typeof(BaboonBirdAI))]
	static internal class BaboonBirdAIPatch
	{

		static bool isLockedOnUnfortunate = false;
		static PlayerControllerB unfortunatePlayer = null;

		//If the baboon is locked on a player it will chase them until they die or enter the building.
		[HarmonyPostfix]
		[HarmonyPatch(nameof(BaboonBirdAI.DoAIInterval))]
		static void DoAIIntervalPatch(ref BaboonBirdAI __instance, ref PlayerControllerB ___targetPlayer, ref int ___aggressiveMode, ref float ___fightTimer, ref float ___timeSinceFighting)
        {
			if (unfortunatePlayer == null || !isLockedOnUnfortunate)
				return;

			if(unfortunatePlayer.isPlayerDead || unfortunatePlayer.isInsideFactory)
            {
				isLockedOnUnfortunate = false;
				unfortunatePlayer = null;
				return;
			}

			//Set all the variables needed for the baboon to kick some ass.
			if (___aggressiveMode != 2)
			{
				___aggressiveMode = 2;
				__instance.SetAggressiveModeServerRpc(2);
			}
			___targetPlayer = unfortunatePlayer;
			___fightTimer = 15f;
			___timeSinceFighting = 15f;
			__instance.SetDestinationToPosition(unfortunatePlayer.transform.position);
		}

		//If the baboon is angered by an unfortunate player, it will now be locked onto that player.
		[HarmonyPostfix]
		[HarmonyPatch(nameof(BaboonBirdAI.SetAggressiveModeServerRpc))]
		static void SetAggressiveModeServerRpcPatch(ref BaboonBirdAI __instance, ref int mode)
		{
			if (mode != 2)
				return;

			Transform threatTransform = __instance.focusedThreat?.threatScript?.GetThreatTransform();

			if (threatTransform == null)
				return;

			PlayerControllerB player = UnfortunatePlayer.players.Find(p => (p.transform == threatTransform));

			if (player != null)
			{
				unfortunatePlayer = player;
				isLockedOnUnfortunate = true;
			}
		}



	}
}
