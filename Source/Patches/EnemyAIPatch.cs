using UnityEngine;
using HarmonyLib;
using GameNetcodeStuff;

namespace FuniPlugin
{
	/*
	 * 
	 * These patches are copy pasted in-game code. 
	 * The trick relies on the fact that they happen after the original function, but only on unfortunate players.
	 * So even if there is a better target, as long as there is at least one unfortunate player they will have to suffer.
	 * 
	 */

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
			if (UnfortunatePlayer.players.Count == 0)
				return;

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
			return;
		}

		[HarmonyPatch(nameof(EnemyAI.GetClosestPlayer))]
		[HarmonyPostfix]
		//Checks for the closest player from unfortunate players and chooses them even if they arent actually the closest.
		static void GetClosestPlayerPatch(ref PlayerControllerB __result, ref EnemyAI __instance,ref float ___tempDist, ref float ___mostOptimalDistance, ref bool requireLineOfSight, ref bool cannotBeInShip, ref bool cannotBeNearShip)
		{

			if (UnfortunatePlayer.players.Count == 0)
				return;

			___mostOptimalDistance = Vector3.Distance(__instance.transform.position, UnfortunatePlayer.players[0].transform.position);
			foreach (PlayerControllerB player in UnfortunatePlayer.players)
			{
				if (!__instance.PlayerIsTargetable(player, cannotBeInShip))
				{
					continue;
				}
				if (cannotBeNearShip)
				{
					if (player.isInElevator)
					{
						continue;
					}
					bool flag = false;
					for (int j = 0; j < RoundManager.Instance.spawnDenialPoints.Length; j++)
					{
						if (Vector3.Distance(RoundManager.Instance.spawnDenialPoints[j].transform.position, player.transform.position) < 10f)
						{
							flag = true;
							break;
						}
					}
					if (flag)
					{
						continue;
					}
				}
				if (!requireLineOfSight || !Physics.Linecast(__instance.transform.position, player.transform.position, 256))
				{
					___tempDist = Vector3.Distance(__instance.transform.position, player.transform.position);
					if (___tempDist < ___mostOptimalDistance)
					{
						___mostOptimalDistance = ___tempDist;
						__result = player;
						return;
					}
				}
			}
			return;
		}


	}
}
