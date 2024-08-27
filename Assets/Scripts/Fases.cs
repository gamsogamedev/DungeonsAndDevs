using UnityEngine;
using UnityEngine.SceneManagement;

public class Fases : MonoBehaviour
{
    public string sceneToLoad;

    private void OnTriggerEnter2D(Collider2D other)
    {
        

        if (other.CompareTag("Player"))
        {
            LoadScene();
        }
        
    }

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {          
            SceneManager.LoadScene(sceneToLoad);
        }
       
    }
    public void Play() => SceneManager.LoadScene(sceneToLoad);
    public void Exit() => Application.Quit();
}
