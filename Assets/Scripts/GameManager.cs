using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int stageIndex;
    public int health;
    public PlayerMove player;

    public void NextStage()
    {
        stageIndex++;
    }

    public void HealthDown()
    {
        if(health > 1)
        {
            health--;
        }
        else
        {
            player.OnDie();

            Debug.Log("죽었습니다");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {

            if (health > 1)
            {
                collision.attachedRigidbody.velocity = Vector2.zero;
                collision.transform.position = new Vector3(-3, 0, -1);
            }

            HealthDown();
        }
    }
}
