using TMPro;
using UnityEngine;

public class GameMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private GameObject titleText;
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject ModeList;
    [SerializeField] private GameObject ModeSelect;
    [SerializeField] private GameObject attackModeLevelButtons;
    [SerializeField] private GameObject defenseModeLevelButtons;

    [SerializeField] private TextMeshProUGUI startCounter;

    public TextMeshProUGUI StartCounter {
        get { return startCounter; }
    }

    public void SetModeListScreen(bool status) { ModeList.SetActive(status); }
    public void SetModeSelectScreen(bool status) { ModeSelect.SetActive(status); }

    public void SetGameOverScreen(bool status) { gameOverScreen.SetActive(status); }

    public void SetWinScreen(bool status) {  winScreen.SetActive(status); }

    public void SetAttackModeLevelButtons(bool status) {  attackModeLevelButtons.SetActive(status); }

    public void SetDefenseModeLevelButtons(bool status) {  defenseModeLevelButtons.SetActive(status); }  

    public void DestroyTitleText() { Destroy(titleText); }

    public void OnClickBack()
    {
        SetModeSelectScreen(true);
        SetAttackModeLevelButtons(false);
        SetDefenseModeLevelButtons(false);
    }

    public void OnSelectMode(int mode) {
        SetModeSelectScreen(false);
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

    public void OnClickExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
