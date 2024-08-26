using UnityEngine;
using UnityEngine.SceneManagement;

public class Fases : MonoBehaviour
{
    public string sceneToLoad;
    private void Start()
{
    Debug.Log("Script SceneTransition está ativo.");
}

    private void OnCollisionEnter(Collision collision)
{
    Debug.Log("Algo colidiu com a porta.");
}

    private void LoadScene()
    {
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log("Carregando a cena: " + sceneToLoad);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogError("Nome da cena não especificado.");
        }
    }
}
