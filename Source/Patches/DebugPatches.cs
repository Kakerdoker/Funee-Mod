using UnityEngine;
using HarmonyLib;
using Unity.Netcode;
using GameNetcodeStuff;

namespace FuneePlugin
{
	/*
	 * Patches that make it easier for me to debug. Shouldn't be used during mod release.
	 */
	static internal class DebugPatches
	{

		static float timeOfNextUpdate = 0f;
		const float secondsForUpdate = 1f;
		[HarmonyPostfix]
		[HarmonyPatch(typeof(BaboonBirdAI))]
		[HarmonyPatch(nameof(BaboonBirdAI.Update))]
		static void BaboonUpdatePatch(ref BaboonBirdAI __instance, ref int ___aggressiveMode, ref float ___fightTimer, ref float ___fearLevel)
        {
			if(timeOfNextUpdate < Time.time)
            {
				timeOfNextUpdate = Time.time + secondsForUpdate;
				MyLogger.Debug($"aggressiveMode: {___aggressiveMode} | currentBehaviourStateIndex: {__instance.currentBehaviourStateIndex} | focusedThreatIsInView: {__instance.focusedThreatIsInView} | fightTimer: {___fightTimer} | fearLevel: {___fearLevel}");

			}
        }

	}
}
