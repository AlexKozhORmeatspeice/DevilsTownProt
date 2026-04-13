using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] private Transform winScreen;
    [SerializeField] private Transform loseScreen;

    public static EndScreen instance;

    void Awake()
    {
        instance = this;
        Hide();
    }

    public void ShowWin()
    {
        Time.timeScale = 0.0f;
        gameObject.SetActive(true);
        
        winScreen.gameObject.SetActive(true);
        loseScreen.gameObject.SetActive(false);
    }

    public void ShowLose()
    {
        Time.timeScale = 0.0f;
        gameObject.SetActive(true);
        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(true);
    }

    public void Hide()
    {
        Time.timeScale = 1.0f;
        gameObject.SetActive(false);

        winScreen.gameObject.SetActive(false);
        loseScreen.gameObject.SetActive(false);
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
