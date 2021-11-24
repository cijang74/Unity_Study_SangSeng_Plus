using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    public int nextMove;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CircleCollider2D circlecollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circlecollider = GetComponent<CircleCollider2D>();
        Think();

        Invoke("Think", 5);
    }

    private void FixedUpdate()
    {
        // 몬스터의 움직임
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        // 앞 지형이 낭떨어지인지 플랫폼 체크
        Vector2 frontVec = new Vector2(rigid.position.x + (nextMove*0.3f), rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (rayHit.collider == null)
        {
            // 방향 틀기
            Turn();
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);

        anim.SetInteger("RunSpeed", nextMove);

        // 방향에 따라 스프라이트 플립해주기
        if(nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }

        float nextThinkTime = Random.Range(2f, 5f);
        Invoke("Think", nextThinkTime);
    }

    void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 2);
    }

    public void OnDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        circlecollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        Invoke("DeActive", 5);
    }

    void DeActive()
    {
        gameObject.SetActive(false);
    }
}
