using HarmonyLib;
using UnityEngine;

namespace FuniPlugin
{

	[HarmonyPatch(typeof(FlashlightItem))]
	static internal class FlashlightItemPatch
	{
		[HarmonyPatch(nameof(FlashlightItem.Update))]
		[HarmonyPostfix]
		//Make flashlight 30% worse if you are an unfortunate player
		static void UpdatePatch(ref FlashlightItem __instance, ref Light ___flashlightBulb)
        {
			
			if (UnfortunatePlayer.players.Contains(__instance.playerHeldBy))
            {
				___flashlightBulb.intensity *= 0.7f;
			}	
			
        }
	}
}
