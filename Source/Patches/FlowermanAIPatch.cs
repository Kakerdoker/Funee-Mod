using HarmonyLib;
using GameNetcodeStuff;

namespace FuniPlugin
{

	[HarmonyPatch(typeof(FlowermanAI))]
    internal class FlowermanAIPatch
    {

		[HarmonyPrefix]
		[HarmonyPatch(nameof(FlowermanAI.AddToAngerMeter))]
        //Makes the unfortunate player anger the bracken 30x faster.
        static void AddToAngerPatch(ref float amountToAdd, ref PlayerControllerB ___targetPlayer)
        {
            if(UnfortunatePlayer.players.Contains(___targetPlayer))
                amountToAdd *= 30f;
        }

    }
}
