using System.Collections;
using UnityEngine;

public class ClubSinger : EnemyController
{
    [SerializeField] private GameObject aoeAttackPrefab;
    [SerializeField] private GameObject aoeWarningPrefab;
    [SerializeField] private GameObject aoeWarningBorderPrefab;
    private GameObject aoeAttack;
    private GameObject aoeWarning;
    private GameObject aoeWarningBorder;
    
    public AudioClip warningSFX;
    public AudioClip aoeStartSFX;
    public AudioClip idleSFX;
    private AudioSource audioSource;

    float aoeWarningTime = 2;
    float aoeTimer = 2;
    float attackTime = 1;
    bool aoeAttackFlag = false;
    
    protected override void Start()
    {
        base.Start();
        scoreValue = 5000;
        //audioSource = GetComponent<AudioSource>();
    }
    
    protected override void Update()
    {
        base.Update();
        if(moveSpeed == 0){
            aoeWarningTime -= Time.deltaTime;
            
            if(aoeWarningTime < 0 && aoeAttack == null){
                Destroy(aoeWarningBorder);
                Destroy(aoeWarning);
                animator.SetTrigger("attacking");
                aoeAttack = Instantiate(aoeAttackPrefab, transform.position, Quaternion.identity);
                aoeAttack.GetComponent<SingerAOE>().InitializeDamage(attack);
                //audioSource.PlayOneShot(aoeStartSFX);
                //Debug.Log("Disabled");
                aoeAttackFlag = true;
            }
            
            if(aoeAttackFlag){
                attackTime -= Time.deltaTime;
            }
            
            if(aoeAttackFlag && attackTime < 0){
                animator.SetTrigger("ending");
                Destroy(aoeAttack);
                //audioSource.PlayOneShot(idleSFX);
                moveSpeed = 2;
                aoeAttackFlag = false;
                aoeWarningTime = aoeTimer;
                attackTime = 1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.CompareTag("Player")){
            if(aoeWarning == null){
                moveSpeed = 0;
                animator.SetTrigger("charging");
                aoeWarningBorder = Instantiate(aoeWarningBorderPrefab, transform.position, Quaternion.identity);
                aoeWarning = Instantiate(aoeWarningPrefab, transform.position, Quaternion.identity);
                StartCoroutine(FillWarning());
                //audioSource.PlayOneShot(warningSFX);
            }
        }
    }
    
    IEnumerator FillWarning()
    {
        float elapsed = 0f;
        while (elapsed < aoeTimer)
        {
            float t = elapsed / aoeTimer;
            aoeWarning.transform.localScale = Vector3.Lerp(Vector3.zero, aoeWarningPrefab.transform.localScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        aoeWarning.transform.localScale = aoeWarningPrefab.transform.localScale;
    }
}