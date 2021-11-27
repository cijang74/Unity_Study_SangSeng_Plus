using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int stageIndex;
    public int health;
    private float respawn_x = 0f;
    private float respawn_y = -0.5f;
    private float respawn_z = 0;

    public Image[] UIhealth;
    public Text UIStage;



    public PlayerMove player;
    public GameObject[] Stages;
    public GameObject RestartBtn;

    private void Update()
    {
        UIStage.text = "STAGE" + stageIndex + 1;
    }

    public void NextStage()
    {
        if (stageIndex < Stages.Length-1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            if (stageIndex == 1)
            {
                respawn_y = 8f;
            }

            else if (stageIndex == 2)
            {
                respawn_y = 0.4f;
            }

            else if (stageIndex == 3)
            {
                respawn_x = -11f;
                respawn_y = -1.5f;
            }

            else if (stageIndex == 4)
            {
                health++;
                respawn_x = 0.0f;
                respawn_y = 0.4f;
            }

            else if (stageIndex == 5)
            {
                respawn_x = -11f;
                respawn_y = -1.5f;
            }

            else if (stageIndex == 6)
            {
                health++;
                respawn_x = 0.0f;
                respawn_y = 0.4f;
            }

            else if (stageIndex == 7)
            {
                respawn_x = -11f;
                respawn_y = -1.5f;
            }
            PlayerReposition(respawn_x, respawn_y, respawn_z);
        }

        else
        {
            Time.timeScale = 0;
            Debug.Log("게임 클리어!");

            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            RestartBtn.SetActive(true);
        }

    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
            UIhealth[health].color = new Color(1, 0, 0, 0.4f);
        }
        else
        {
            UIhealth[0].color = new Color(1, 0, 0, 0.4f);
            player.OnDie();

            Debug.Log("죽었습니다");
            RestartBtn.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {

            if (health > 1)
            {
                PlayerReposition(respawn_x, respawn_y, respawn_z);
            }

            HealthDown();
        }
    }

    void PlayerReposition(float respawn_x, float respawn_y, float respawn_z)
    {
        player.transform.position = new Vector3(respawn_x, respawn_y, respawn_z);
        player.VelocityZero();
    }
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
