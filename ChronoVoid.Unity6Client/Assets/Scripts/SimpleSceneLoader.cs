using UnityEngine;
using UnityEngine.SceneManagement;

namespace ChronoVoid.Client
{
    /// <summary>
    /// Simple scene loader for Unity 6 testing
    /// </summary>
    public class SimpleSceneLoader : MonoBehaviour
    {
        public void LoadLoginTestScene()
        {
            Debug.Log("Loading LoginTestScene for full API testing...");
            SceneManager.LoadScene("LoginTestScene");
        }
        
        public void LoadRealmListScene()
        {
            Debug.Log("Loading RealmListScene...");
            SceneManager.LoadScene("RealmListScene");
        }
        
        public void LoadNavigationScene()
        {
            Debug.Log("Loading NavigationScene...");
            SceneManager.LoadScene("NavigationScene");
        }
        
        public void LoadCreateRealmScene()
        {
            Debug.Log("Loading CreateRealmScene...");
            SceneManager.LoadScene("CreateRealmScene");
        }
    }
}