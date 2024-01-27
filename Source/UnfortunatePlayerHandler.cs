using GameNetcodeStuff;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Unity.Netcode;

namespace FuniPlugin
{
	public static class UnfortunatePlayer
	{
		public static List<PlayerControllerB> players;
		public static List<ulong> steamIds = new();

		const string steamIDPath = "players.txt";

		public static void Synchronize()
		{
			if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
			{
				HostInit();

				UnfortunatePlayerNetworkHandler.Instance.ClearSteamIDListClientRpc();
				foreach (PlayerControllerB player in players)
				{
					UnfortunatePlayerNetworkHandler.Instance.AddSteamIDToListClientRpc(player.playerSteamId);
				}

				UnfortunatePlayerNetworkHandler.Instance.InitializeClientRpc();
			}

		}
		static void HostInit()
		{
			CreateStandardFileIfDoesntExist(steamIDPath);
			steamIds = GetSteamIDsFromFile(steamIDPath);
			players = GetUnfortunatePlayers(steamIds);
			IfThereAreNoUnfortunatePlayersMakeOnePersonInLobbyUnfortunate();
			LogPlayers();
		}

		public static void ClientInit()
        {
			players = GetUnfortunatePlayers(steamIds);
			LogPlayers();
		}

		public static void LogPlayers()
        {
			foreach (PlayerControllerB player in players)
				MyLogger.Debug(player.playerUsername);
		}

		static void IfThereAreNoUnfortunatePlayersMakeOnePersonInLobbyUnfortunate()
		{
			if (players.Count == 0)
			{
				PlayerControllerB randomPlayer = GetRandomPlayer();
				players = new() { randomPlayer };

				MakeStandardFile();
				AppendNewIDInFile(randomPlayer.playerSteamId);
			}
		}

		static PlayerControllerB GetRandomPlayer()
		{
			System.Random rnd = new();
			int max = StartOfRound.Instance.allPlayerObjects.Length;
			int randomPlayer = rnd.Next(0, max);
			PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[randomPlayer].GetComponent<PlayerControllerB>();
			return player;
		}

		static List<PlayerControllerB> GetUnfortunatePlayers(List<ulong> steamIds)
		{
			List<PlayerControllerB> unfortunatePlayers = new();

			foreach (GameObject playerObject in StartOfRound.Instance.allPlayerObjects)
			{
				PlayerControllerB player = playerObject.GetComponent<PlayerControllerB>();
				if (steamIds.Contains(player.playerSteamId))
				{
					unfortunatePlayers.Add(player);
				}
			}
			return unfortunatePlayers;
		}

		private static void MakeStandardFile()
		{
			using (FileStream fs = File.Open(steamIDPath,FileMode.Create))
			{
				byte[] steamID = new UTF8Encoding(true).GetBytes("76561198194556193\n76561198068333834\n76561198052138845\n");
				fs.Write(steamID, 0, steamID.Length);
			}
		}

		private static void AppendNewIDInFile(ulong steamId)
		{
			using (StreamWriter fs = new StreamWriter(steamIDPath, true))
			{
				fs.WriteLine(steamId.ToString()+"\n");
			}
		}

		private static void CreateStandardFileIfDoesntExist(string path)
		{
			if (!File.Exists(path))
			{
				MakeStandardFile();
			}
		}

		private static List<ulong> GetSteamIDsFromFile(string path)
		{
			List<ulong> steamIDs = new();
			foreach (string line in File.ReadLines(path))
			{
				if (ulong.TryParse(line, out ulong id))
					if (line.Length == 17)
					{
						steamIDs.Add(id);
					}
			}

			return steamIDs;
		}
	}
}
