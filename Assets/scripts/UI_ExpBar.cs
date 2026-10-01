using UnityEngine;
using UnityEngine.UI;

public class UI_ExpBar : MonoBehaviour
{
    [SerializeField] private Image _fillImage;
    [SerializeField] private float _fillSpeed = 8f;

    private float _targetFillAmount = 0f;

    void Start()
    {
        if (_fillImage == null)
            _fillImage = GetComponent<Image>();

        if (_fillImage.type != Image.Type.Filled)
            _fillImage.type = Image.Type.Filled;

        if (ExpManager.Instance != null)
        {
            // Подписываемся на изменение опыта
            ExpManager.Instance.OnExpChanged.AddListener(OnExpChanged);
            UpdateBar(); // Инициализируем
        }
    }

    void OnDestroy()
    {
        if (ExpManager.Instance != null)
        {
            ExpManager.Instance.OnExpChanged.RemoveListener(OnExpChanged);
        }
    }

    void Update()
    {
        if (Mathf.Abs(_fillImage.fillAmount - _targetFillAmount) > 0.001f)
        {
            _fillImage.fillAmount = Mathf.Lerp(_fillImage.fillAmount, _targetFillAmount, Time.deltaTime * _fillSpeed);
        }
    }

    private void OnExpChanged(int currentExp, int currentLevel)
    {
        UpdateBar();
    }

    private void UpdateBar()
    {
        if (ExpManager.Instance == null || _fillImage == null) return;

        // Используем исправленный метод
        _targetFillAmount = ExpManager.Instance.GetProgressPercentage();
    }

    // Для ручного обновления
    public void Refresh()
    {
        UpdateBar();
    }
}
