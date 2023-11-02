using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 3.0f;
    [SerializeField] private float stoppingDistance = 2.0f;
    [SerializeField] private float retreatSpeed = 2.0f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float health = 50f;
    [SerializeField] private float timeToReactivate = 5f;
    [SerializeField] private Image canvasImage; // Asegúrate de asignar esta variable desde el editor Unity

    private float Originalspeed;
    private bool isDead = false;
    private Transform player;
    private Vector3 initialPosition;
    private bool isReturning = false;
    private Animator ownAnimator;
    private bool isStunned = false;
    private float stunTimer = 0f;
    private GameManager gameManager;

    [SerializeField] private AudioSource walkSound;
    [SerializeField] private AudioSource wakeUpSound;
    [SerializeField] private AudioSource deathSound;
    [SerializeField] private AudioSource attackSound;
    [SerializeField] private AudioSource playerSound;
    [SerializeField] private AudioSource hurtSound;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        canvasImage.DOFade(0, .2f);
        initialPosition = transform.position;
        ownAnimator = GetComponent<Animator>();
        Originalspeed = speed;
        gameManager = GameManager.Instance;
    }

    void Update()
    {
        
        if (isStunned)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= 1.5f) // Cambia el tiempo de estuneo según tus necesidades
            {
                isStunned = false;
                speed = Originalspeed; // Cambia la velocidad de recuperación según tus necesidades
                ownAnimator.SetFloat("speed", 1);
                stunTimer = 0f;
            }
        }
        else
        {
            walkSound.Play();
        }

        if (!isDead)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < stoppingDistance && !isReturning)
            {
                Vector3 direction = player.position - transform.position;
                transform.LookAt(player);
                if (distance > attackRange)
                {
                    transform.Translate(Vector3.forward * Time.deltaTime * speed);
                    ownAnimator.SetFloat("speed", speed);
                    // Reproducir sonido de caminar
                }
                else
                {
                    
                    ownAnimator.SetTrigger("attack" + Random.Range(1, 4));
                    StartCoroutine(AttackCoroutine());
                }
            }
            else
            {
                isReturning = true;
                ReturnToInitialPosition();
                if (Vector3.Distance(transform.position, initialPosition) < 0.05f)
                {
                    isReturning = false;
                    ownAnimator.SetFloat("speed", 0);
                    
                }
                else
                {
                    transform.LookAt(initialPosition);
                    transform.Translate(Vector3.forward * Time.deltaTime * retreatSpeed);
                    ownAnimator.SetFloat("speed", retreatSpeed);
                    // Reproducir sonido de caminar
                }
            }
        }
    }

    IEnumerator AttackCoroutine()
    {
        yield return new WaitForSeconds(0.6f); // Ajusta el tiempo según tus necesidades

        if (!isDead && !isStunned)
        {
            yield return new WaitForSeconds(0.6f); // Espera a que se complete el fade out
            // lógica de ataque aquí
            Debug.Log("Golpe");
            if (gameManager.IncreaseHit())
            {
                // Reproducir sonido de quejido del personaje
                playerSound.Play();
            }
            // Realiza el fade out y el fade in de la imagen del canvas

            // Reproducir sonido de disparo
            attackSound.Play();

            canvasImage.DOFade(1, 1f).OnComplete(() =>
            {
                canvasImage.DOFade(0, 0.2f);
            });
        }
    }


    public void TakeDamage(float amount)
    {
        if (!isDead)
        {
            if (!isStunned)
            {
                isStunned = true;
                ownAnimator.SetTrigger("stunned");
                
                ownAnimator.SetFloat("speed", 0); // Asegurarse de que el enemigo permanezca quieto durante la duración especificada
                speed = 0;
            }
            health -= amount;
            if (health <= 0)
            {
                Die();
            }
            else
            {
                ownAnimator.SetTrigger("damage");
                // Reproducir sonido de herida
                hurtSound.Play();
            }
        }
    }

    public void Die()
    {
        isDead = true;
        
        initialPosition = transform.position; // Establece la nueva posición inicial como la posición actual
        ownAnimator.SetTrigger("death2");
        StartCoroutine(ReactivateAfterTime(timeToReactivate));
        // Reproducir sonido de muerte
        deathSound.Play();
    }

    void ReturnToInitialPosition()
    {
        if (!isDead)
        {
            transform.position = Vector3.MoveTowards(transform.position, initialPosition, retreatSpeed * Time.deltaTime);
        }
    }

    IEnumerator ReactivateAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        isDead = false;
        health = 50f; // Reinicia la salud
        ownAnimator.SetTrigger("wake");
        // Reproducir sonido de despertar
        wakeUpSound.Play();
    }
}
