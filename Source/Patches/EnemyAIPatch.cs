using UnityEngine;
using HarmonyLib;
using GameNetcodeStuff;

namespace FuniPlugin
{

	[HarmonyPatch(typeof(EnemyAI))]
    internal class EnemyAIPatch
    {
		[HarmonyPatch(nameof(EnemyAI.CheckLineOfSightForPlayer))]
		[HarmonyPostfix]
		//Deletes the original if-statement that checks whether the player is inside enemy's field of view. Basically gives enemies 360 vision.
		//Makes enemy's vision range x10 larger.
		//Applies only to unfortunate players.
		static void CheckLineOfSightForPlayerPatch(ref int range , ref Transform ___eye, ref PlayerControllerB __result)
        {
			range *= 10;

			foreach(PlayerControllerB player in UnfortunatePlayer.players)
            {
				if (player.isPlayerDead)
					continue;

				Vector3 position = player.gameplayCamera.transform.position;
				if (Vector3.Distance(position, ___eye.position) < (float)range && !Physics.Linecast(___eye.position, position, StartOfRound.Instance.collidersAndRoomMaskAndDefault))
				{
					__result = player;
					return;
				}
            }		
		}


		[HarmonyPatch(nameof(EnemyAI.TargetClosestPlayer))]
		[HarmonyPostfix]
		//Deletes the if-statement checking whether the monster has a line of sight towards the closest player. Basically gives the monster a wallhack.
		//Applies only to unfortunate players.
		static void TargetClosestPlayerPatch(ref float bufferDistance, ref float ___mostOptimalDistance, ref PlayerControllerB ___targetPlayer, ref EnemyAI __instance, ref bool __result)
        {
			foreach (PlayerControllerB player in UnfortunatePlayer.players)
			{
				if (player.isPlayerDead)
					continue;

				___mostOptimalDistance = Vector3.Distance(__instance.transform.position, player.transform.position);
				if (__instance.PlayerIsTargetable(player) && player != null && bufferDistance > 0f && player != null && Mathf.Abs(___mostOptimalDistance - Vector3.Distance(__instance.transform.position, player.transform.position)) < bufferDistance)
				{
					___targetPlayer = player;
					__result = ___targetPlayer != null;
					return;
				}
			}
			__result = ___targetPlayer != null;
			return;
		}

    }
}
