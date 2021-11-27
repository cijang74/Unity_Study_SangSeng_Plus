using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoxCollider2D col2D;

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioDie;
    public AudioClip audioFinish;

    AudioSource audioSource;


    private void Awake()
    {
        // 맨 처음에는 변수를 초기화 시켜줌
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        col2D = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(string action)
    {
        switch (action){
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
        }
        audioSource.Play();
    }

    private void Update() // 단발적인 키입력은 그냥 업데이트
    {
        // 점프
        if (Input.GetButtonDown("Jump") && !anim.GetBool("IsJumping"))
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // 무게 적용하려면 Impulse
            anim.SetBool("IsJumping", true);
            PlaySound("JUMP");
        }


        // 키에서 손 땠을 때 속도 줄이기
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); // normalized: 백터 크기를 1로 만든 단위백터 (방향구할 때 쓰는거)
        }

        // 방향 전환
        if (Input.GetButton("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // GetAxisRaw(Horizontal)은 좌우 방향키를 눌렀을 때 이동 방향을 나타내며 -1은 왼쪽이다.
        }

        // 걷기 애니메이션 적용
        if (Mathf.Abs(rigid.velocity.x) < 1) // 속도가 0이면 그건 멈춘거니까 애니메이션 제어
        {
            anim.SetBool("IsWalking", false);
        }

        else
        {
            anim.SetBool("IsWalking", true);
        }
    }

    private void FixedUpdate() // 지속적인 키입력은 픽스드 업데이트
    {
        // 키를 입력받아 움직임을 제어
        float h = Input.GetAxisRaw("Horizontal");

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) // Right Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if(rigid.velocity.x < maxSpeed * -1) // Left Max Speed
        {
            rigid.velocity = new Vector2(maxSpeed * -1, rigid.velocity.y);
        }


        if (rigid.velocity.y < 0)
        {
            // 플랫폼에 착지했는가 DrawRay(빔이 시작되는 위치, 쏘는 방향): 에디터 상에서 Ray를 그려주는 함수
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            // 쏜 레이저가 platform과 맞았는지 판단
            RaycastHit2D rayHit = Physics2D.BoxCast(col2D.bounds.center, col2D.bounds.size, 0f, Vector2.down, 0.02f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.5f)
                {
                    anim.SetBool("IsJumping", false);
                    // Debug.Log(rayHit.collider.name);
                }
            }
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                OnAttack(collision.transform);
            }

            else
            {
                OnDamaged(collision.transform.position);
            };
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();
            PlaySound("FINISH");
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        PlaySound("DAMAGED");
        gameManager.HealthDown();

        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 7, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        gameObject.layer = 10;

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void OnDie()
    {
        PlaySound("DIE");
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        col2D.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    void OnAttack(Transform enemy)
    {
        PlaySound("ATTACK");
        rigid.AddForce(Vector2.up * 8, ForceMode2D.Impulse);
        try
        {
            EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
            enemyMove.OnDamaged();
        }catch(NullReferenceException)
        {
            BossMove enemyMove = enemy.GetComponent<BossMove>();
            enemyMove.OnDamaged();
        }
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }
}
