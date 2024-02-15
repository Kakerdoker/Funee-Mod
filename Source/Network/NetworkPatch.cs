using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using System.IO;
using System.Reflection;

namespace FuneePlugin
{
	internal static class NetworkPatch
	{

		public static GameObject networkPrefab;
		public static GameObject networker;

		[HarmonyPostfix]
		[HarmonyPatch(typeof(GameNetworkManager), "Start")]
		//Inserting custom networker into the game's network manager after it starts
		public static void AddPrefabPatch()
		{
			//Reading the asset file with the network prefab
			AssetBundle networkPrefabAsset = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "networkbundle"));
			//Getting my network prefab from the asset file
			networkPrefab = (GameObject)networkPrefabAsset.LoadAsset("FuneeNetworkHandler");
			//Adding a network handler to the prefab
			networkPrefab.AddComponent<UnfortunatePlayerNetworkHandler>();
			//Adding the network prefab to the game's network manager
			NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(StartOfRound), "Awake")]
		//Spawn the networker inside the scene.
		private static void PatchAwake()
		{
			if (networkPrefab != null && (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer))
			{
				//Make sure to delete it if one already exists to not spam the scene with a shitton networkers (The ship scene never gets unloaded so we have to do it manually)
				if (networker != null)
					GameObject.Destroy(networker);

				networker = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
				networker.GetComponent<NetworkObject>().Spawn();
			}
		}

	}
}