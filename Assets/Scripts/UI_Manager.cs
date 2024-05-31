using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private TMP_Text _scoreText;
    private string _scoreTextPrefix = "SCORE: ";
    [SerializeField] private TMP_Text _ammoText;
    private string _ammoTextPrefix = "AMMO: ";
    [SerializeField] private Image _livesImgDisplay;
    [SerializeField] private Sprite[] _livesSprites;
    [SerializeField] private TMP_Text _gameOverText;
    [SerializeField] private TMP_Text _restartText;
    [SerializeField] private float _textFlickerDelay = 0.25f;
    private Game_Manager _gameManager;
    [SerializeField] private Slider _thrusterSlider;
    [SerializeField] private Image _thrusterSliderFill;
    public TMP_Text waveIDDisplay;
    public TMP_Text _waveTimeDisplay;
    public GameObject _waveDisplay;
    public bool _waveEnded = false;


    private void Start()
    {
        _scoreText.text = _scoreTextPrefix + 0;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("Game_Manager").GetComponent<Game_Manager>();
        if (_gameManager == null)
        {
            Debug.LogError("No Game Manager Found!");
        }
        else
        {
            _gameManager.GameNotOver();
            Debug.Log("New Game Started!");
        }
        if (_thrusterSlider == null)
        {
            Debug.LogError("Thruster Slider is NULL!");
        }
        if (_thrusterSliderFill == null)
        {
            Debug.LogError("Thruster Slider Fill is NULL!");
        }
    }

    private void Update()
    {
        
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = _scoreTextPrefix + score;
    }

    public void UpdateLives(int currentLives)
    {
        if (currentLives > -1)
        {
            _livesImgDisplay.sprite = _livesSprites[currentLives];
        }
    }

    public void GameOverSequence()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        _gameManager.GameOver();
        StartCoroutine("GameOverTextFlicker");
    }

    IEnumerator GameOverTextFlicker()
    {
        while (true)
        {
            yield return new WaitForSeconds(_textFlickerDelay);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(_textFlickerDelay);
            _gameOverText.gameObject.SetActive(true);
        }
    }

    public void DisplayRestartText()
    {
        _restartText.gameObject.SetActive(true);
    }

    public void UpdateAmmo(int _playerAmmo, int _playerMaxAmmo)
    {
        _ammoText.text = _ammoTextPrefix + _playerAmmo.ToString() + " / " + _playerMaxAmmo.ToString();
    }

    public void UpdateThrusterSlider(float thrustValue)
    {
        if (thrustValue >= 0 && thrustValue <= 10)
        {
            _thrusterSlider.value = thrustValue;
        }
    }

    public void ThrustersSliderUsableColor(bool usableThrusters)
    {
        if (usableThrusters)
        {
            _thrusterSliderFill.color = Color.green;
        }
        else if (!usableThrusters)
        {
            _thrusterSliderFill.color = Color.red;
        }
    }

    public void WaveDisplayOn()
    {
        _waveDisplay.SetActive(true);
    }

    public void WavDisplayOff()
    {
        _waveDisplay.SetActive(false);
    }

    public void WaveIDUpdate(int waveID)
    {
        waveIDDisplay.text = "Wave: " + waveID.ToString();
    }

    public void WaveTimeUpdate(float _seconds)
    {
        float _waveTime = Mathf.RoundToInt(_seconds);
        _waveTimeDisplay.text = _waveTime.ToString();
        if (_waveTime > 0)
        {
            _waveEnded = false;
        }
        else
        {
            _waveEnded = true;
            StartCoroutine(WaveDisplayFlickerRoutine());
        }
    }

    private IEnumerator WaveDisplayFlickerRoutine()
    {
        while (_waveEnded)
        {
            yield return new WaitForSeconds(_textFlickerDelay);
            _waveDisplay.SetActive(false);
            yield return new WaitForSeconds(_textFlickerDelay);
            _waveDisplay.SetActive(true);
        }
    }
}
