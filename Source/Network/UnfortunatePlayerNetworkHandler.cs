using Unity.Netcode;

namespace FuniPlugin
{
    public class UnfortunatePlayerNetworkHandler : NetworkBehaviour
    {
        public static UnfortunatePlayerNetworkHandler Instance { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                Instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            Instance = this;

            base.OnNetworkSpawn();
        }

        [ClientRpc]
        public void ClearSteamIDListClientRpc()
        {
            if (!NetworkManager.Singleton.IsHost)
                UnfortunatePlayer.steamIds.Clear();
        }

        [ClientRpc]
        public void AddSteamIDToListClientRpc(ulong steamID)
        {
            if (!NetworkManager.Singleton.IsHost)
                UnfortunatePlayer.steamIds.Add(steamID);
        }

        [ClientRpc]
        public void InitializeClientRpc()
        {
            if(!NetworkManager.Singleton.IsHost)
                UnfortunatePlayer.ClientInit();
        }


    }
}
