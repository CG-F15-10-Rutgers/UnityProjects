using UnityEngine;
using System.Collections;

public class LevelSelect : MonoBehaviour {

    public GameObject loadingImage;

    public void loadLevel(int level)
    {
        loadingImage.SetActive(true);
        Application.LoadLevel(level);
    }
}
