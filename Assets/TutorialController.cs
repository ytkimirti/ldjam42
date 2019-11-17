using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialController : MonoBehaviour {


    public TextMeshPro tutorialText;
    int currPhase;

    public float sinSpeed;
    public float sinAmount;
    public Transform arrow;

    Transform target;

	void Start () {
        if (!PlayerPrefs.HasKey("tutorial"))
        {
            StartCoroutine(tutorialEnum());
        }
	}
	
    IEnumerator tutorialEnum()
    {
        for (int i = 0; i < 5; i++)
        {
            nextPhase();
            yield return new WaitForSeconds(6f);
        }
    }

    public void nextPhase()
    {
        currPhase++;

        if(currPhase == 1)
        {
            target = GameManager.main.player.transform;
            tutorialText.text = "Use WASD to move";
        }else if (currPhase == 2)
        {
            target = GameManager.main.currEnemy[GameManager.main.currEnemy.Count - 1].transform;
            tutorialText.text = "Left click to shoot, but dont miss";
        }
        else if (currPhase == 3)
        {
            target = GameManager.main.currEnemy[GameManager.main.currEnemy.Count - 1].transform;
            tutorialText.text = "Rigth click to throw grenades";
        }
        else if (currPhase == 4)
        {
            target = GameManager.main.player.transform;
            tutorialText.text = "MouseWheel or Q or '1,2,3...' to change weapons";
        }
        else if (currPhase == 5)
        {
            PlayerPrefs.SetInt("tutorial", 1);
            gameObject.SetActive(false);
        }
    }

	void Update () {
        if(target)
            transform.position = target.position;

        arrow.transform.localPosition = new Vector3(0, Mathf.Sin(sinSpeed * Time.time) * sinAmount, 0);
	}
}
