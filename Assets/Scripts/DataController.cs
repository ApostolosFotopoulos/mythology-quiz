using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text.RegularExpressions;

public class DataController : MonoBehaviour {
    public RoundData[] allRoundData;
    public PlayerProgress playerProgress;
    public AudioSource backgroundMusic;

    //Animator states.
    public int mainMenuAnimationState;

    //Number of rounds.
    private const int numberOfRounds = 8;
    private string gameDataFileName = "data.json";
    public int chosenRound;

    //Round images.
    public Sprite[] images;

    void Start() {
        DontDestroyOnLoad(backgroundMusic);
        DontDestroyOnLoad(gameObject);
        LoadGameData();
        LoadPlayerProgress();

        mainMenuAnimationState = 0;
        SceneManager.LoadScene("MainMenu");
    }

    private void LoadGameData() {
        //Path.Combine combines strings into a file path.
        //Application.StreamingAssets points to Assets/StreamingAssets in the Editor,
        //and the StreamingAssets folder in a build.
        string filePath;
        if (Application.platform == RuntimePlatform.Android) {
            filePath = "jar:file://" + Application.dataPath + "!/assets/" + gameDataFileName;
            WWW www = new WWW(filePath);
            byte[] dataAsBytes = www.bytes;

            //Read the json from the file into a string.
            string dataAsJson = System.Text.Encoding.Default.GetString(dataAsBytes);
            //Pass the json to JsonUtility, and tell it to create a GameData object from it.
            GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

            allRoundData = loadedData.allRoundData;
        }
        else if (Application.platform == RuntimePlatform.IPhonePlayer) {
            filePath = Application.dataPath + "/Raw";
        }
        else {
            filePath = Path.Combine(Application.streamingAssetsPath, gameDataFileName);

            if (File.Exists(filePath)) {
                // Read the json from the file into a string
                string dataAsJson = File.ReadAllText(filePath);
                // Pass the json to JsonUtility, and tell it to create a GameData object from it
                GameData loadedData = JsonUtility.FromJson<GameData>(dataAsJson);

                allRoundData = loadedData.allRoundData;
            }
            else {
                Debug.LogError("Cannot load game data!");
                Application.Quit();
            }
        }
    }

    public RoundData GetCurrentRoundData(int roundNumber) {
        //Get the current round number.
        RoundData roundDataTmp = new RoundData (allRoundData[roundNumber]);
        Shuffle(roundDataTmp.questions);
        return roundDataTmp;
    }

    //Algorithm to mix the questions.
    private void Shuffle(QuestionData[] questions) {
        //Knuth shuffle algorithm
        for (int t = 0; t < questions.Length; t++) {
            QuestionData tmp = questions[t];
            int r = Random.Range(t, questions.Length);
            questions[t] = questions[r];
            questions[r] = tmp;
        }
    }

    public Sprite getRoundImage() {
        return images[chosenRound];
    }

    public void SubmitNewPlayerScore(int newScore, int roundNumber) {
        //If newScore is greater than playerProgress.highestScore, update playerProgress
        //with the new value and call SavePlayerProgress().

        //Debug.Log(roundName+"  submiting new score  " + newScore+"  old score:"+ playerProgress.highscore[roundNumber]);
        if (newScore > playerProgress.highscore[roundNumber]) {
            playerProgress.highscore[roundNumber] = newScore;
            SavePlayerProgress(roundNumber);
        }
    }

    public int GetHighestPlayerScore(int roundNumber) {
        return playerProgress.highscore[roundNumber];
    }

    public void LoadPlayerProgress() {
        playerProgress = new PlayerProgress(numberOfRounds);

        //If PlayerPrefs contains a key called "highestScore", set the value of 
        //playerProgress.highestScore using the value associated with that key.
        for (int i = 0; i < numberOfRounds; i++) {
            if (PlayerPrefs.HasKey("highscore"+i)) {
                playerProgress.highscore[i] = PlayerPrefs.GetInt("highscore"+i);
                //Debug.Log("highscore" + i + "  " + playerProgress.highscore[i]);
            }
        } 
    }

    private void SavePlayerProgress(int roundNumber) {
        //Save the value playerProgress.highestScore to PlayerPrefs, with a key of "highestScore".

        //Debug.Log("Round"+ roundNumber+"  saving new score with key: "+ "highscore" + roundNumber);
        PlayerPrefs.SetInt("highscore"+ roundNumber, playerProgress.highscore[roundNumber]);
    }
}