using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {
    public GameObject optionsDisplay;
    public GameObject infoDisplay;
    public GameObject background;
    public GameObject confirmationBackground;
    public GameObject confirmationDisplay;
    public AudioSource clickButtonSound;

    public Text soundText;
    public Text infoText;
    public Text iconText;

    private DataController dataController;
    public Animator mainMenuAnimator;
    private int mainMenuStateHash;

    private void Start() {
        dataController = FindObjectOfType<DataController>();
        mainMenuStateHash = Animator.StringToHash("MainMenuState");
        
        if (dataController.mainMenuAnimationState == 1) {
            mainMenuAnimator.SetInteger(mainMenuStateHash, 1);
        }
        //Change the intro animation 
        dataController.mainMenuAnimationState = 1;
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (infoDisplay.activeSelf) {
                StartCoroutine(OpenPanel(infoDisplay, 0.4f, false));
            }
            else if (confirmationBackground.activeSelf) {
                StartCoroutine(OpenConfirmationPanel(confirmationDisplay, 0.4f, false));
            }
            else if (optionsDisplay.activeSelf) {
                StartCoroutine(OpenPanel(optionsDisplay, 0.4f, false));
            }
            else {
                Application.Quit();
            }
        }

        if (PlayerPrefs.HasKey(soundText.text)) {
            int optionState = PlayerPrefs.GetInt(soundText.text);
            if (optionState == 1) {
                AudioListener.volume = 1;
            }
            else {
                AudioListener.volume = 0;
            }
        }
    }

    public void StartNewGame() {
        mainMenuAnimator.SetInteger(mainMenuStateHash, 2);
        StartCoroutine(LoadNewScene(0.4f, "NewGameMenu"));
    }

    public void OpenOptions() {
        clickButtonSound.Play();
        StartCoroutine(OpenPanel(optionsDisplay, 0.4f, true));
    }

    public void SaveOptions() {
        clickButtonSound.Play();
        StartCoroutine(OpenPanel(optionsDisplay, 0.4f, false));
    }

    public void OpenInfo() {
        clickButtonSound.Play();
        StartCoroutine(OpenPanel(infoDisplay, 0.4f, true));
    }

    public void CloseInfo() {
        clickButtonSound.Play();
        StartCoroutine(OpenPanel(infoDisplay, 0.4f, false));
    }

    public void OpenConfirmation() {
        clickButtonSound.Play();
        StartCoroutine(OpenConfirmationPanel(confirmationDisplay, 0.4f, true));
    }

    public void CloseConfirmation() {
        clickButtonSound.Play();
        StartCoroutine(OpenConfirmationPanel(confirmationDisplay, 0.4f, false));
    }

    public void ResetProgress() {
        int sound = 1;
        int learnMore = 1;
        int showArtwork = 1;

        if (PlayerPrefs.HasKey(soundText.text)) {
            sound = PlayerPrefs.GetInt(soundText.text);
        }
        if (PlayerPrefs.HasKey(infoText.text)) {
            learnMore = PlayerPrefs.GetInt(infoText.text);
        }
        if (PlayerPrefs.HasKey(iconText.text)) {
            showArtwork = PlayerPrefs.GetInt(iconText.text);
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt(soundText.text, sound);
        PlayerPrefs.SetInt(infoText.text, learnMore);
        PlayerPrefs.SetInt(iconText.text, showArtwork);

        dataController.LoadPlayerProgress();
        StartCoroutine(OpenConfirmationPanel(confirmationDisplay, 0.4f, false));
    }

    private IEnumerator LoadNewScene(float time,string s) {
        clickButtonSound.Play();
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(s);
    }

    private IEnumerator OpenPanel(GameObject panel, float time, bool b) {
        if (b) {
            background.SetActive(b);
            panel.SetActive(b);
            mainMenuAnimator.SetInteger(mainMenuStateHash, 3);
            yield return new WaitForSeconds(time);
        }
        else {
            mainMenuAnimator.SetInteger(mainMenuStateHash, 4);
            yield return new WaitForSeconds(time);
            background.SetActive(b);
            panel.SetActive(b);
        } 
    }

    private IEnumerator OpenConfirmationPanel(GameObject panel, float time, bool b) {
        if (b) {
            confirmationBackground.SetActive(b);
            panel.SetActive(b);
            mainMenuAnimator.SetInteger(mainMenuStateHash, 5);
            yield return new WaitForSeconds(time);
        }
        else {
            mainMenuAnimator.SetInteger(mainMenuStateHash, 6);
            yield return new WaitForSeconds(time);
            confirmationBackground.SetActive(b);
            panel.SetActive(b);
        }
    }

    public void OpenEtwinning() {
        Application.OpenURL("https://twinspace.etwinning.net/52491/home");
    }

    public void OpenFacebook() {
        Application.OpenURL("https://www.facebook.com/ErasmusFlyingMythology/");
    }

    public void OpenInstagram() {
        Application.OpenURL("https://www.instagram.com/p/Bb42A5vngLD/");
    }

    public void OpenProgramPage() {
        Application.OpenURL("https://flyingmythology.wordpress.com");
    }
}
