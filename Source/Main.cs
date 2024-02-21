using BepInEx;
using HarmonyLib;
using UnityEngine;
using System.Reflection;

namespace FuneePlugin
{
	[BepInPlugin(modGUID, "Funee", "1.0.3")]
	public class Main : BaseUnityPlugin
	{
		private const string modGUID = "Kakerdoker.Funee";
		private readonly Harmony harmony = new Harmony(modGUID);
		public static Main instance;


		private void Awake()
		{
			UnityNetcodePatch();

			if (instance == null)
				instance = this;

			harmony.PatchAll(typeof(NetworkPatch));
			harmony.PatchAll(typeof(EnemyAIPatch));
			harmony.PatchAll(typeof(StartOfRoundPatch));
			harmony.PatchAll(typeof(JesterAIPatch));
			harmony.PatchAll(typeof(FlashlightItemPatch));
			harmony.PatchAll(typeof(HoarderBugAIPatch));
			harmony.PatchAll(typeof(CentipedeAIPatch));
			harmony.PatchAll(typeof(BaboonBirdAIPatch));

			//harmony.PatchAll(typeof(DebugPatches)); 

			MyLogger.Debug("Funee plugin is done patching.");
		}

		static void UnityNetcodePatch()
		{
			var types = Assembly.GetExecutingAssembly().GetTypes();
			foreach (var type in types)
			{
				var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
				foreach (var method in methods)
				{
					var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
					if (attributes.Length > 0)
					{
						method.Invoke(null, null);
					}
				}
			}
		}
	}
}
