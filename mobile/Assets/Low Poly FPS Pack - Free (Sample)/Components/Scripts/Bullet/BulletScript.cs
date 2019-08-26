﻿using UnityEngine;
using System.Collections;
using Photon.Pun;

// ----- Low Poly FPS Pack Free Version -----
public class BulletScript : MonoBehaviourPun
{

    [Range(5, 100)]
    [Tooltip("After how long time should the bullet prefab be destroyed?")]
    public float destroyAfter;
    [Tooltip("If enabled the bullet destroys on impact")]
    public bool destroyOnImpact = false;
    [Tooltip("Minimum time after impact that the bullet is destroyed")]
    public float minDestroyTime;
    [Tooltip("Maximum time after impact that the bullet is destroyed")]
    public float maxDestroyTime;

    [Header("Impact Effect Prefabs")]
    public Transform[] metalImpactPrefabs;

    public GameObject bullet;

    public Vector3 bulletSpawn;

    /// <summary>
    /// 伺服器刪除物件
    /// </summary>
    private void DelayDestroy()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    private void Start()
    {
        //Start destroy timer
        StartCoroutine(DestroyAfter());
    }

    //If the bullet collides with anything
    private void OnCollisionEnter(Collision collision)
    {

            //If destroy on impact is false, start 
            //coroutine with random destroy timer
            if (!destroyOnImpact)
        {
            Invoke("DelayDestroy", 0.5f);
            StartCoroutine(DestroyTimer());
        }
        //Otherwise, destroy bullet on impact
        else
        {
            Invoke("DelayDestroy", 0.5f);
            PhotonNetwork.Destroy(gameObject);
        }

        //If bullet collides with "Metal" tag
        if (collision.transform.tag == "Metal")
        {
            //Instantiate random impact prefab from array
            Instantiate(metalImpactPrefabs[Random.Range
                (0, metalImpactPrefabs.Length)], transform.position,
                Quaternion.LookRotation(collision.contacts[0].normal));
            //Destroy bullet object
            PhotonNetwork.Destroy(gameObject);
            Invoke("DelayDestroy", 0.5f);

        }

        //If bullet collides with "Target" tag
        if (collision.transform.tag == "Target")
        {
            //Toggle "isHit" on target object
            collision.transform.gameObject.GetComponent
                <TargetScript>().isHit = true;
            //Destroy bullet object
            PhotonNetwork.Destroy(gameObject);
            Invoke("DelayDestroy", 0.5f);

        }

        //If bullet collides with "ExplosiveBarrel" tag
        if (collision.transform.tag == "ExplosiveBarrel")
        {
            //Toggle "explode" on explosive barrel object
            collision.transform.gameObject.GetComponent
                <ExplosiveBarrelScript>().explode = true;
            //Destroy bullet object
            PhotonNetwork.Destroy(gameObject);
            Invoke("DelayDestroy", 0.5f);

        }
    }

    private IEnumerator DestroyTimer()
    {
        //Wait random time based on min and max values
        yield return new WaitForSeconds
            (Random.Range(minDestroyTime, maxDestroyTime));
        //Destroy bullet object
        PhotonNetwork.Destroy(gameObject);
        Invoke("DelayDestroy", 0.5f);

    }

    private IEnumerator DestroyAfter()
    {
        //Wait for set amount of time
        yield return new WaitForSeconds(destroyAfter);
        //Destroy bullet object
        PhotonNetwork.Destroy(gameObject);
        Invoke("DelayDestroy", 0.5f);

    }
}
// ----- Low Poly FPS Pack Free Version -----