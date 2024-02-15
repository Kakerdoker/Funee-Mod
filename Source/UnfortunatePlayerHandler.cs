using GameNetcodeStuff;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Unity.Netcode;

namespace FuneePlugin
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
			int amountOfActualPlayers = 0;
			foreach(PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
            {
				if (player.isPlayerControlled)
					amountOfActualPlayers++;
            }

			System.Random rnd = new();
			int randomPlayer = rnd.Next(0, amountOfActualPlayers);

			//Choose random playerscript, counting only the ones that are controlled by a player
			//If you have REAL FAKE REAL (where real is a player controlled script), and the random number is 1, we want it to choose the second real instead of the second in the list, which is FAKE.
			foreach (PlayerControllerB player in StartOfRound.Instance.allPlayerScripts)
			{
				MyLogger.Debug("" + randomPlayer);
				if (randomPlayer == 0)
					return player;

				if (player.isPlayerControlled)
					randomPlayer--;
			}

			MyLogger.Error("Randomly chosen player index wasn't found inside StartOfRound.Instance.allPlayerScripts");
			return StartOfRound.Instance.allPlayerScripts[0];
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
				byte[] steamID = new UTF8Encoding(true).GetBytes("76561198194556193\n76561198068333834\n");
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
