using BepInEx;
using HarmonyLib;

namespace FuniPlugin
{

	[BepInPlugin(modGUID, "Funi", "1.0.0")]
	public class Main : BaseUnityPlugin
	{
		private const string modGUID = "Kakerdoker.Funi";
		private readonly Harmony harmony = new Harmony(modGUID);
		public static Main instance;

		private void Awake()
		{
			if (instance == null)
				instance = this;

			harmony.PatchAll(typeof(EnemyAIPatch));
			harmony.PatchAll(typeof(StartOfRoundPatch));
			harmony.PatchAll(typeof(FlowermanAIPatch));
			harmony.PatchAll(typeof(FlashlightItemPatch));


			MyLogger.Debug("Funi plugin is done patching.");
		}
	}
}
