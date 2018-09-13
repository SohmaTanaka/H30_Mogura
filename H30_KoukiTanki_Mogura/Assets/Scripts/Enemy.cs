﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private CharacterController controller;
    private Animator anim;
    private Vector3 rightDestination;//右端
    private Vector3 leftDestination;//左端
    private float speed = 2;
    private Vector3 velocity;
    private Vector3 direction;//方向
    private bool rightFlag = true;//右に行くか
    private bool leftFlag = false;//左に行くか
    private Transform rayBox;
    private float maxDistance = 10;//Rayの長さ
    bool isHunmer = false;

    // Use this for initialization
    void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        rightDestination = new Vector3(-6, transform.position.y, transform.position.z);
        leftDestination = new Vector3(6, transform.position.y, transform.position.z);
        velocity = Vector3.zero;
        rightFlag = true;
        leftFlag = false;
        rayBox = transform.GetChild(2);
        isHunmer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (CounterTimer.CanStart)
        {

            if (!isHunmer)
            {
                Ray ray = new Ray(rayBox.position, new Vector3(0, 0, -1));//画面手前側にRayを飛ばす
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, maxDistance))
                {
                    if (hit.collider.tag == "Player")
                    {
                        isHunmer = true;
                        StartCoroutine(Attack(hit.collider.gameObject.GetComponent<MoleMotion>()));
                    }
                }
                velocity = Vector3.zero;
                anim.SetFloat("Speed", 2f);
                if (rightFlag)
                {
                    direction = (rightDestination - transform.position).normalized;
                    transform.LookAt(new Vector3(rightDestination.x, transform.position.y, transform.position.z));
                }
                else if (leftFlag)
                {
                    direction = (leftDestination - transform.position).normalized;
                    transform.LookAt(new Vector3(leftDestination.x, transform.position.y, transform.position.z));
                }
                velocity = direction * speed;

                if (Vector3.Distance(transform.position, rightDestination) < 0.5f)
                {
                    rightFlag = false;
                    leftFlag = true;
                }
                else if (Vector3.Distance(transform.position, leftDestination) < 0.5f)
                {
                    leftFlag = false;
                    rightFlag = true;
                }

                controller.Move(velocity * Time.deltaTime);
            }

        }
    }

    IEnumerator Attack(MoleMotion mole)
    {
        float rate = 0;

        while (true)
        {
            rate += Time.deltaTime / 3;
            gameObject.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 180, 0), rate);

            if (rate >= 1)
            {
                anim.SetTrigger("Hunmer");
                yield return new WaitForSeconds(1);
                break;
            }
            yield return null;
        }

        while (mole.Now == Motion.UP || mole.Now == Motion.Top)
        {
            mole.Down();
            yield return null;

            if (mole.Now == Motion.Idle || mole.Now == Motion.Down)
                break;
        }

        while (mole.Now == Motion.Idle || mole.Now == Motion.Down)
        {
            if (mole.Now == Motion.Idle)
            {
                isHunmer = false;
                yield break;
            }
            yield return null;
        }
    }
}

