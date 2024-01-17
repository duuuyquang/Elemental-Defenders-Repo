using TMPro;
using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject ingameScreen;
    [SerializeField] private GameObject modeList;
    [SerializeField] private GameObject modeSelect;
    [SerializeField] private GameObject attackModeLevelButtons;
    [SerializeField] private GameObject defenseModeLevelButtons;
    [SerializeField] private GameObject soundOption;
    [SerializeField] private GameObject backgroundCheckmark;

    [SerializeField] private TextMeshProUGUI startCounter;
    [SerializeField] private TMP_Dropdown themeDropdown;

    SoundController soundController;

    public TextMeshProUGUI StartCounter {
        get { return startCounter; }
    }

    private void Start()
    {
        soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
    }

    public void SetPauseScreen(bool status)
    {
        pauseScreen.SetActive(status);
    }

    public void SetSoundOption(bool status) { soundOption.SetActive(status); }

    public void SetStartScreen(bool status) { startScreen.SetActive(status); }

    public void SetInGameScreen(bool status) { ingameScreen.SetActive(status); }

    public void SetModeListScreen(bool status) { modeList.SetActive(status); }
    public void SetModeSelectScreen(bool status) { modeSelect.SetActive(status); }

    public void SetGameOverScreen(bool status) { gameOverScreen.SetActive(status); }

    public void SetWinScreen(bool status) {  winScreen.SetActive(status); }

    public void SetAttackModeLevelButtons(bool status) { attackModeLevelButtons.SetActive(status); }

    public void SetDefenseModeLevelButtons(bool status) {  defenseModeLevelButtons.SetActive(status); }  

    public void DestroyTitleText() { Destroy(titleText); }

    public void OnClickBack()
    {
        SetModeSelectScreen(true);
        SetAttackModeLevelButtons(false);
        SetDefenseModeLevelButtons(false);
        soundController.PlayButtonClick();
    }

    public void OnSelectMode(int mode) {
        SetModeSelectScreen(false);
        soundController.PlayButtonClick();
        switch (mode)
        {
            case GameManager.MODE_ATTACK:
                SetAttackModeLevelButtons(true);
                break;
            case GameManager.MODE_DEFENSE:
                SetDefenseModeLevelButtons(true);
                break;
        }
    }

    public void SetCurrentThemeDropdownVal(int value)
    {
        themeDropdown.value = value;
    }

    public void SetCurrentBackgroundSoundChecker(bool isTurnOn)
    {
        backgroundCheckmark.SetActive(isTurnOn);
    }

    public void OnClickExit()
    {
        soundController.PlayButtonClick();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
