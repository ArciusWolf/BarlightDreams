using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSetup : MonoBehaviour
{
    private void Start()
    {
        string scene = SceneManager.GetActiveScene().name;

        if (scene == "Level_Home")
        {
            transform.position = new Vector3(0f, -5f, 0f);
        }
    }
}
