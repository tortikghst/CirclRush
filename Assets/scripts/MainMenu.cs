using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{
    [Header("Кнопки (опционально)")]
    public Button playButton;
    public Button shopButton;
    public Button exitButton;

    void Start()
    {
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        AudioManager.Instance?.PlayMusic(AudioManager.Instance.menuMusic);
        
        // Подписываем кнопки на звуки
        if (playButton != null)
        {
            playButton.onClick.AddListener(PlayGame);
            AddHoverSound(playButton);
        }
        if (shopButton != null)
        {
            shopButton.onClick.AddListener(OpenShop);
            AddHoverSound(shopButton);
        }
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(ExitGame);
            AddHoverSound(exitButton);
        }
    }

    void AddHoverSound(Button button)
    {
        var trigger = button.gameObject.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((data) => { AudioManager.Instance?.PlayButtonHover(); });
        trigger.triggers.Add(entry);
    }

    public void PlayGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("Game");
    }

    public void OpenShop()
    {
        AudioManager.Instance?.PlayButtonClick();
        SceneManager.LoadScene("magazine");
    }

    public void ExitGame()
    {
        AudioManager.Instance?.PlayButtonClick();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}