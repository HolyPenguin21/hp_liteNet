using System.Collections;
using UnityEngine.SceneManagement;

public class SceneToLoad
{
    public string sceneToLoad { get; set; }

    public void Implementation()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
