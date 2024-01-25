using GameNetcodeStuff;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace FuniPlugin
{
	public static class UnfortunatePlayer
	{
		public static List<PlayerControllerB> players;
		const string steamIDPath = "players.txt";

		public static void Init()
		{
			CreateStandardFileIfDoesntExist(steamIDPath);
			List<ulong> steamIds = GetSteamIDsFromFile(steamIDPath);
			players = GetUnfortunatePlayers(steamIds);
			IfThereAreNoUnfortunatePlayersMakeOnePersonInLobbyUnfortunate();
		}

		static void IfThereAreNoUnfortunatePlayersMakeOnePersonInLobbyUnfortunate()
		{
			if (players.Count == 0)
			{
				MyLogger.Debug("There are no unfortunate players. Making a random one unfortunate.");
				PlayerControllerB randomPlayer = GetRandomPlayer();
				players = new() { randomPlayer };

				MakeStandardFile();
				AppendNewIDInFile(randomPlayer.playerSteamId);
			}
		}

		static PlayerControllerB GetRandomPlayer()
		{
			System.Random rnd = new(2137);
			int max = StartOfRound.Instance.allPlayerObjects.Length;
			int randomPlayer = rnd.Next(0, max);
			PlayerControllerB player = StartOfRound.Instance.allPlayerObjects[randomPlayer].GetComponent<PlayerControllerB>();
			MyLogger.Debug("Got a random player! " + player.playerUsername);
			return player;
		}

		static List<PlayerControllerB> GetUnfortunatePlayers(List<ulong> steamIds)
		{
			List<PlayerControllerB> unfortunatePlayers = new();

			foreach (GameObject playerObject in StartOfRound.Instance.allPlayerObjects)
			{
				PlayerControllerB player = playerObject.GetComponent<PlayerControllerB>();
				MyLogger.Debug("Checking if " + player.playerSteamId + " is in lobby.");
				if (steamIds.Contains(player.playerSteamId))
				{
					unfortunatePlayers.Add(player);
					MyLogger.Debug("Got an unfortunate player: " + player.playerUsername);
				}
			}
			return unfortunatePlayers;
		}

		private static void MakeStandardFile()
		{
			using (FileStream fs = File.Open(steamIDPath,FileMode.Create))
			{
				MyLogger.Debug("Created standard file!");
				byte[] steamID = new UTF8Encoding(true).GetBytes("76561198194556193\n76561198068333834\n76561198052138845\n");
				fs.Write(steamID, 0, steamID.Length);
			}
		}

		private static void AppendNewIDInFile(ulong steamId)
		{
			using (StreamWriter fs = new StreamWriter(steamIDPath, true))
			{
				MyLogger.Debug("Appended " + steamId.ToString() + " to file.");
				fs.WriteLine(steamId.ToString()+"\n");
			}
		}

		private static void CreateStandardFileIfDoesntExist(string path)
		{
			if (!File.Exists(path))
			{
				MyLogger.Debug("File doesnt exist. Creating standard file.");
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
						MyLogger.Debug("Sucessfully read steamID " + id + "from the file.");
					}
			}

			return steamIDs;
		}
	}
}
