using UnityEngine;
using HarmonyLib;
using Unity.Netcode;
using GameNetcodeStuff;

namespace FuneePlugin
{

	[HarmonyPatch(typeof(CentipedeAI))]
	static internal class CentipedeAIPatch
	{

		static bool IsRayhitOnAnyUnfortunatePlayer(RaycastHit rayHit)
		{
			foreach (PlayerControllerB player in UnfortunatePlayer.players)
			{
				if (rayHit.transform == player.transform)
				{
					return true;
				}
			}
			return false;
		}


		//Makes the centipede jump down way quicker than it would normally, if it sees an unforunate player.
		[HarmonyPatch(nameof(CentipedeAI.Update))]
		[HarmonyPostfix]
		static void UpdatePatch(ref CentipedeAI __instance, ref int ___currentBehaviourStateIndex, ref bool ___clingingToCeiling, ref bool ___clingingToPlayer, ref bool ___triggeredFall)
		{

			//If the centipede is in a state that allows for it to jump down to players
			if (___clingingToCeiling && ___currentBehaviourStateIndex == 1)
			{
				//Do the ingame code for jumping down, but with the radius of the spherecast 2 times larger
				Ray ray = new Ray(__instance.transform.position, Vector3.down);

				if (Physics.SphereCast(ray, 5f, out RaycastHit rayHit, 60f, StartOfRound.Instance.playersMask) && !___clingingToPlayer && !Physics.Linecast(rayHit.transform.position, __instance.transform.position, StartOfRound.Instance.collidersAndRoomMask, QueryTriggerInteraction.Ignore) && !___triggeredFall)
				{
					if (IsRayhitOnAnyUnfortunatePlayer(rayHit))
					{
						___triggeredFall = true;
						__instance.TriggerCentipedeFallServerRpc(NetworkManager.Singleton.LocalClientId);
					}
				}
			}
		}


		//Make the centipedes BOLT instantly towards the unfortunate player
		[HarmonyPatch(nameof(CentipedeAI.Update))]
		[HarmonyPostfix]
		static void UpdatePatch2(ref CentipedeAI __instance)
		{
			if (UnfortunatePlayer.players.Contains(__instance.targetPlayer))
				__instance.agent.speed = 10f;
		}
	}
}
