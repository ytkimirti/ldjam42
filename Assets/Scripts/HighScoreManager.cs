using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class HighScoreManager : MonoBehaviour
{
    private const string privateCode = "PLACEPRIVATECODE";
    private const string publicCode = "5b77fa41191a8b0bcce0cdc2";
    private const string webURL = "https://dreamlo.com/lb/";

    public GameObject listElementPrefab;
    public Transform titleTransform;
    public Transform titleOpenPos;
    public float elementOffset;
    public Transform canvasTrans;

    private string nameList;
    private string scoreList;
    private int lowestScore;
    private bool loaded;

    Highscore[] currScores;

    public void Start()
    {
        //downloadHighScores();
    }

    public void showScores()
    {
        titleTransform.DOMoveY(titleOpenPos.position.y,1f);

        print(currScores.Length);

        int scoresCount = Mathf.Clamp(currScores.Length, 0, 10);

        for (int i = 0; i < scoresCount; i++)
        {
            int index = ((scoresCount - 1) - i);
            GameObject go = Instantiate(listElementPrefab,canvasTrans);
            go.transform.position = new Vector3(titleOpenPos.position.x, go.transform.position.y);
            go.GetComponent<TextMeshProUGUI>().text = currScores[index].username + " <color=yellow> " + currScores[index].score + "</color>";

            go.transform.DOMoveY(titleOpenPos.position.y + (elementOffset * (index + 2.7f)), 0.6f).SetDelay(index * 0.1f);
            go.transform.DOPunchScale(Vector3.one * 0.1f,0.3f,2).SetDelay((index * 0.2f) + 2f);
        }
    }

    public bool IsHighScore(int score)
    {
        if (!loaded)
        {
            return false;
        }
        return score > lowestScore;
    }

    public void SubmitHighScore(string name, int score)
    {
        StartCoroutine(UploadNewHighScore(name, score));
    }

    private IEnumerator UploadNewHighScore(string name, int score)
    {
        Debug.Log("Uploading high score...");
        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(name) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            downloadHighScores();
        }
        Debug.Log("Uploading high score!");

        //showScores();
    }

    // Use this for initialization
    private void downloadHighScores()
    {
        StartCoroutine("DownloadHighScoresFromDatabase");
    }

    private IEnumerator DownloadHighScoresFromDatabase()
    {
        Debug.Log("Getting scores!");

        WWW www = new WWW(webURL + publicCode + "/pipe/20");
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Failed to get scores. Retrying in the background.");
            Invoke("downloadHighScores", 30);
            yield break;
        }

        Highscore[] scores = FormatHighscores(www.text);

        currScores = scores;

        showScores();
        
        loaded = true;
        Debug.Log("Got scores!");
    }

    private Highscore[] FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        Highscore[] highscoresList = new Highscore[entries.Length];
        //print(entries[0]);
        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
        }
        lowestScore = entries.Length < 20 ? 0 : highscoresList[entries.Length - 1].score;
        return highscoresList;
    }
}

public struct Highscore
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}