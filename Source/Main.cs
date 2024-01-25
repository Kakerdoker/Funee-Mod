using BepInEx;
using HarmonyLib;


namespace FuniPlugin
{
	[BepInPlugin(modGUID, "Funee", "1.0.0")]
	public class Main : BaseUnityPlugin
	{
		private const string modGUID = "Kakerdoker.Funee";
		private readonly Harmony harmony = new Harmony(modGUID);
		public static Main instance;

		private void Awake()
		{
			if (instance == null)
				instance = this;

			harmony.PatchAll(typeof(EnemyAIPatch));
			harmony.PatchAll(typeof(StartOfRoundPatch));
			harmony.PatchAll(typeof(JesterAIPatch));
			harmony.PatchAll(typeof(FlashlightItemPatch)); 
			harmony.PatchAll(typeof(HoarderBugAIPatch));



			MyLogger.Debug("Funee plugin is done patching.");
		}
	}
}
