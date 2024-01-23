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
			CreateFileIfDoesntExist(steamIDPath);
			List<ulong> steamIds = GetSteamIDsFromFile(steamIDPath);
			players = GetUnfortunatePlayers(steamIds);
		}

		static List<PlayerControllerB> GetUnfortunatePlayers(List<ulong> steamIds)
		{
			List<PlayerControllerB> unfortunatePlayers = new();

			foreach (GameObject playerObject in StartOfRound.Instance.allPlayerObjects)
			{
				PlayerControllerB player = playerObject.GetComponent<PlayerControllerB>();
				if (steamIds.Contains(player.playerSteamId))
					unfortunatePlayers.Add(player);
			}
			return unfortunatePlayers;
		}

		private static void CreateFileIfDoesntExist(string path)
        {
			if (!File.Exists(path))
			{
				using (FileStream fs = File.Create(path))
				{
					byte[] steamID = new UTF8Encoding(true).GetBytes("76561198194556193\n76561198068333834");
					fs.Write(steamID, 0, steamID.Length);
				}
			}
		}

		private static List<ulong> GetSteamIDsFromFile(string path)
        {
			List<ulong> steamIDs = new();
			foreach (string line in File.ReadLines(path))
			{
				if (ulong.TryParse(line, out ulong id))
					if (line.Length == 17)
						steamIDs.Add(id);   
			}

			return steamIDs;
		}
	}
}
