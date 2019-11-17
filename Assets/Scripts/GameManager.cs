using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {


    public int currScore;
    public HeartBarController heartBar;
    public GrenadeBarController grenadeBar;
    public TargetController targetEffect;
    public PlayerInput player;
    public bool gameEnded = false;
    [Space]
    [Header("Sprites")]
    public Sprite heartTokenSprite;
    public Sprite sawTokenSprite;
    public Sprite grenadeTokenSprite;
    public Sprite shieldTokenSprite;

    [Header("UI")]
    public TMP_InputField nameInput;
    public GameObject bossLevelText;
    public TextMeshProUGUI currScoreText;
    public TextMeshProUGUI highScoreText;
    public Transform topTextTrans;
    public Transform topTextTargetPos;
    public Transform currScoreTarget;
    public Transform gameOverWindowTrans;
    public Transform gameOverTarget;
    [Space]
    public Transform allTextTrans;
    public Animator blackAnimator;
    bool gameClosing = false;
    public string[] endgameTexts;
    public TextMeshProUGUI endGameUIText;

    [Header("Boss")]
    public GameObject boss;
    public GameObject bossEnemies;
    public float bossScore;
    public float bossScoreIncrement;
    public Transform bossSlider;
    int currBoss;

    [Header("Weapon")]

    public TokenController weaponToken;
    public Sprite defaultWeaponSprite;
    public Sprite pumpbubleSprite;
    public Sprite machinebubleSprite;
    public Sprite rocketlauncherSprite;

    EnemySpawner spawner;
    GameObject defaultEnemyPrefab;
    HighScoreManager highScoreManager;

    bool submitted = false;

    [Space]

    public float shaveAmount = 0.2f;
    [HideInInspector]
    public List<Transform> currBullets = new List<Transform>();
    [HideInInspector]
    public List<EnemyInput> currEnemy = new List<EnemyInput>();

    public static GameManager main;
	void Awake () {
        main = this;
	}

    public void spawnNextWeapon()
    {
        if (currBoss == 1)
        {
            weaponToken.type = "pumpbuble";
        }
        else if (currBoss == 2)
        {
            weaponToken.type = "machinebuble";
        }
        else if (currBoss == 3)
        {
            weaponToken.type = "rocketlauncher";
        }
        else
            return;

        weaponToken.pickType();
        weaponToken.respawn();
    }

    void Start()
    {
        highScoreManager = GetComponent<HighScoreManager>();

        currScoreText.transform.DOMoveY(currScoreTarget.position.y, 1f);

        spawner = GetComponent<EnemySpawner>();

        defaultEnemyPrefab = spawner.enemyPrefab;
        
    }

    public void submitScore()
    {
        if (submitted)
            return;

        submitted = true;

        highScoreManager.SubmitHighScore(nameInput.text,currScore);
        gameOverWindowTrans.DOMoveY(2000, 1f);

        PlayerPrefs.SetString("name", nameInput.text);
    }

    public void showScores()
    {
        highScoreManager.showScores();
    }

    public void endGame()
    {
        endGameUIText.text = endgameTexts[Random.Range(0, endgameTexts.Length)];

        GetComponent<AudioSource>().Stop();
        targetEffect.gameObject.SetActive(false);
        gameEnded = true;

        loadHighScore();

        StartCoroutine(endGameUI());
    }

    IEnumerator endGameUI()
    {
        yield return new WaitForSeconds(1f);

        topTextTrans.DOMoveY(topTextTargetPos.position.y, 0.5f);

        yield return new WaitForSeconds(0.2f);
        
        
        gameOverWindowTrans.DOMoveY(gameOverTarget.position.y, 0.5f);

        nameInput.text = PlayerPrefs.GetString("name");
    }

    IEnumerator restartScene()
    {

        if (gameClosing)
            yield return null;

        gameClosing = true;

        blackAnimator.SetTrigger("close");

        allTextTrans.DOMoveY(900, 0.5f);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(0);
    }

    public void explosion(Vector2 pos, float radius)
    {
        foreach(EnemyInput e in currEnemy)
        {
            if (!e)
                continue;
            if (Vector2.Distance(pos, e.transform.position) < radius)
                e.die();
        }
    }

    public void shaveBullets(float amount)
    {
        foreach(Transform t in currBullets)
        {
            if (!t)
                continue;
            if (t.localScale.x < 0.07f || t.localScale.x - amount <= 0.07f)
                Destroy(t.gameObject);
            else
                t.DOScale(t.localScale - (Vector3.one * amount),0.20f).SetEase(Ease.OutQuad);
        }
    }

    public void addScore(int score)
    {
        currScore += score;
        currScoreText.text = currScore.ToString();
    }

    void loadHighScore()
    {
        int hs = PlayerPrefs.GetInt("hscore");
        if (currScore > hs)
        {
            PlayerPrefs.SetInt("hscore", currScore);
            highScoreText.text = currScore.ToString();
        }
        else
        {
            highScoreText.text = hs.ToString();
        }
    }

    float change;
	void Update () {
        if (Input.GetKeyDown(KeyCode.R) && !nameInput.isFocused)
            StartCoroutine(restartScene());
        if (Input.GetKeyDown(KeyCode.K) && !nameInput.isFocused)
            player.die();

        float targetVal = 0;

        if (!isBossLevel)
            targetVal = Mathf.Clamp((currScore - change) / (bossScore - change),0,1);

        bossSlider.localScale = new Vector3(Mathf.Lerp(bossSlider.localScale.x,targetVal,0.1f), bossSlider.localScale.y, 1);


        if (currScore >= bossScore)
        {
            change = bossScore;
            bossScore = bossScore + bossScoreIncrement;
            currBoss++;

            StartCoroutine(bossLevelEnum());
        }

        if (isBossLevel && !boss.activeInHierarchy)
        {
            stopBossLevel();
        }
	}

    public bool isBossLevel;

    public void startBossLevel()
    {
        boss.transform.position = Vector3.up * 9;
        shaveBullets(1f);
        AudioManager.main.Play("pickup");
        isBossLevel = true;
        boss.SetActive(true);
        boss.GetComponent<EnemyInput>().respawnBoss();
        spawner.enemyPrefab = bossEnemies;
        
    }
    
    IEnumerator bossLevelEnum()
    {
        spawner.enabled = false;
        
        for (int i = 0; i < 5; i++)
        {
            bossLevelText.SetActive(false);

            yield return new WaitForSeconds(0.1f);

            bossLevelText.SetActive(true);

            yield return new WaitForSeconds(0.1f);
        }

        startBossLevel();

        yield return new WaitForSeconds(3);

        spawner.enabled = true;
    }

    public void stopBossLevel()
    {
        spawnNextWeapon();

        change = currScore;
        isBossLevel = false;
        boss.SetActive(false);
        bossLevelText.SetActive(false);
        spawner.enemyPrefab = defaultEnemyPrefab;
    }
}
