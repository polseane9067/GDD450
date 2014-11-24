﻿using UnityEngine;
using System.Collections;

public class BarrelExplode : MonoBehaviour 
{
    public ParticleSystem explosion;
    public ParticleSystem smoke;
    bool isExploding = false;
    bool p1Hit = false;
    bool p2Hit = false;
	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
        if (isExploding)
        {
            GameObject p1 = GameObject.Find("Player1(Clone)");
            GameObject p2 = GameObject.Find("Player2(Clone)");
            if (p1!= null && p1Hit == false && Vector3.Distance(p1.transform.position, gameObject.transform.position) <= 6)
            {
                p1Hit = true;
                p1.GetComponent<Player1MovementController>().health -= 40;
            }
            if (p2!= null && p2Hit == false && Vector3.Distance(p2.transform.position, gameObject.transform.position) <= 6)
            {
                p2Hit = true;
                p2.GetComponent<Player2MovementController>().health -= 40;
            }
        }
	}

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "alienBullet" || other.tag == "robotBullet" || other.name=="Sword(Clone)")
        {
            isExploding = true;
            GetComponent<AudioSource>().Play();
            explosion.Play();
            smoke.Play();
            transform.parent.gameObject.GetComponent<ItemSpawner>().empty = true;
            //GameObject.Find("A*Battle").GetComponent<AstarPath>().Scan();
            ItemSpawner.numExplodingBarrels--;
            StartCoroutine("explode");
            Destroy(this.gameObject, 1.1f);
        }
    }

    IEnumerator explode()
    {
        yield return new WaitForSeconds(.1f);
        this.renderer.enabled = false;
        yield return new WaitForSeconds(.5f);
        smoke.enableEmission = false;
    }
}
