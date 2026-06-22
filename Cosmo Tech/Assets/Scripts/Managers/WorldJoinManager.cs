using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldJoinManager : MonoBehaviour
{
    [SerializeField] private WorldGenerator worldGenerator;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private ResourceSpawningManager resourceSpawningManager;
    private bool hostStarted;
    void Start()
    {
        if (MenuManager.IsHost)
        {
            NetworkManager.Singleton.StartHost();
            worldGenerator.GenerateWorld();
            tileManager.GenerateTiles();
            return;
        }
        //wait until host is done generating world to start client
        NetworkManager.Singleton.StartClient();
    }
}
