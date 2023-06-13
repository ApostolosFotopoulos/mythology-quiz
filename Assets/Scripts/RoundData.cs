using UnityEngine;

[System.Serializable]
public class RoundData {
    public string name;
    //public int timeLimitInSeconds;
    public int questionsToShow;
    public int pointsAddedForCorrectAnswer;
    public string pictureText;
    public QuestionData[] questions;

    public RoundData(int questionsLength) {
        //Debug.Log("RoundData: Main constructor");
        name = "";
        //timeLimitInSeconds = data.timeLimitInSeconds;
        questionsToShow = 10;
        pointsAddedForCorrectAnswer = 1;
        pictureText = "";
        questions = new QuestionData[questionsLength];
    }

    public RoundData(RoundData data) {
        //Debug.Log("RoundData: Copy constructor");
        name = data.name;
        //timeLimitInSeconds = data.timeLimitInSeconds;
        questionsToShow = data.questionsToShow;
        pointsAddedForCorrectAnswer = data.pointsAddedForCorrectAnswer;
        pictureText = data.pictureText;
        questions = new QuestionData[data.questions.Length];

        for (int i = 0; i < data.questions.Length; i++) {
            questions[i] = new QuestionData(data.questions[i]);
        }
    }
}
