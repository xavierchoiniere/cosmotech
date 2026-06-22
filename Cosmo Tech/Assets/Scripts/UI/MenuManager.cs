using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static bool IsHost = false;

    public void HostGame()
    {
        IsHost = true;
        SceneManager.LoadScene("Main Planet");
    }

    public void JoinGame()
    {
        IsHost = false;
        SceneManager.LoadScene("Main Planet");
    }
}
