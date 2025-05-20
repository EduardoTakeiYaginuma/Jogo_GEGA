using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(CircleCollider2D))]
public class XPOrb : MonoBehaviour
{
    public int   xpAmount        = 1;
    public float attractionRange = 4f;
    public float speed           = 6f;

    /* ----------- SFX (única mudança) ----------- */
    [Header("Áudio de coleta")]
    [SerializeField] AudioClip pickupSfx;          // arraste seu .wav/.ogg aqui
    [SerializeField, Range(0f,1f)] float sfxVolume = 0.8f;
    /* ------------------------------------------- */

    Transform player;
    bool      follow;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void Update()
    {
        if (!player) return;

        float d = Vector2.Distance(transform.position, player.position);
        if (!follow && d < attractionRange) follow = true;

        if (follow)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            transform.position += (Vector3)dir * speed * Time.deltaTime;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        var xp = col.GetComponent<PlayerExperience>() ??
                 col.GetComponentInChildren<PlayerExperience>();
        if (xp != null) xp.AddXP(xpAmount);

        /* --------- toca o som de pickup --------- */
        if (pickupSfx)
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, sfxVolume);
        /* ---------------------------------------- */

        Destroy(gameObject);
    }
}
