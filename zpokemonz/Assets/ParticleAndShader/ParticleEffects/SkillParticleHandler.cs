using UnityEngine;
using DG.Tweening;
/// <summary>
/// 技能粒子效果程序
/// </summary>
public class SkillParticleHandler : MonoBehaviour
{
    [SerializeField] Transform target;
    public GameObject impactParticle;
    public GameObject projectileParticle;
    public GameObject muzzleParticle;
    public bool test;
    public bool doJump;
    //public GameObject[] trailParticles;
    //[HideInInspector]
    //public Vector3 impactNormal; //Used to rotate impactparticle.

    void Start()
    {
        if(test)
        {
            Invoke("_Start", 1f);
        }
    }
    public void RunMove(Vector3 targetPos)
    {
        Vector3 startPos = transform.position;
        projectileParticle = Instantiate(projectileParticle, startPos, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
		if(muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, startPos, transform.rotation) as GameObject;
            Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
		}

        if(doJump)
        {
            transform.DOJump(targetPos, 2.5f, 1, 0.7f);
        }
        else
        {
            transform.DOMove(targetPos, 0.7f);
        }
        Invoke("End", 0.8f);
    }

    void _Start()
    {
        projectileParticle = Instantiate(projectileParticle, transform.position, transform.rotation) as GameObject;
        projectileParticle.transform.parent = transform;
		if(muzzleParticle)
        {
            muzzleParticle = Instantiate(muzzleParticle, transform.position, transform.rotation) as GameObject;
            Destroy(muzzleParticle, 1.5f); // Lifetime of muzzle effect.
		}
        transform.DOMove(target.position, 1f);
        Invoke("End", 1.2f);
    }

    void End()
    {
        //impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, impactNormal)) as GameObject;
        impactParticle = Instantiate(impactParticle, transform.position, Quaternion.FromToRotation(Vector3.up, Vector3.zero)) as GameObject;

        Destroy(projectileParticle, 3f);
        Destroy(impactParticle, 5f);
        Destroy(gameObject);
    }
}