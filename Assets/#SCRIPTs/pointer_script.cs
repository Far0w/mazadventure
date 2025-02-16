﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class pointer_script : MonoBehaviour
{

    public float defaultLength = 5f;
    public GameObject dot;
    public GameObject menuManager;
    public keyboard_script keyboard;
    public leaderboardsManager lbManager; // Manager qui s'occupe des leaderboards (choix du leaderboard de difficulté X)


    //public Text debugText3;

    private LineRenderer lineRenderer = null;
    private bool isKeyboardOpen = true;
    private field lastSelectedField;
    private bool lastState = false; //Etat : true si à la dernière frame le bouton était enfoncé , false sinon


    private void Awake()
    {
        lineRenderer = transform.GetComponent<LineRenderer>();
        //debugText3.text = "Pointer Init";
    }


    private void Update()
    {
        UpdateLine();

        if (Input.GetButtonDown("Oculus_CrossPlatform_PrimaryThumbstick"))
        {
            if (isKeyboardOpen)
            {
                keyboard.closeKeyboard();
                isKeyboardOpen = false;
            }

            else
            {
                keyboard.openKeyboard();
                isKeyboardOpen = true;
            }
                
        }
    }

    private void UpdateLine()
    {
        float targetLength = defaultLength;

        RaycastHit hit = CreateRaycast(targetLength);

        Vector3 endPosition = transform.position + (transform.forward * targetLength);


        if (hit.collider != null)
        {
            endPosition = hit.point;

            if (hit.collider.gameObject.name.Contains("_c")) // If the collider belong to the keyboard
            {
                //print("pointer on key" + hit.collider.gameObject.name);
                if (hit.collider.gameObject.name.Length == 3) // touche de clavier classique (lettre, nombres ou _)
                {

                    keyboard.onKey(hit.collider.gameObject.name[0].ToString());

                }
                else
                {
                    string[] splittedKeyName = hit.collider.gameObject.name.Split('_');
                    keyboard.onKey(splittedKeyName[0]);

                }
            }

            if (Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") > 0f)

            {
                //debugText3.text = "Button action0";

                if (hit.collider.transform.tag == "field")
                {
                    keyboard.selectedField = hit.collider.gameObject.GetComponent<field>();
                    hit.collider.gameObject.GetComponent<field>().isSelected();
                    if (lastSelectedField != null)
                    {
                        lastSelectedField.isUnselected();
                    }
                    lastSelectedField = hit.collider.transform.GetComponent<field>();
                }


                if (lastState == false)
                {
                    if (hit.collider.gameObject.name.Contains("_c")) // If the collider belong to the keyboard
                    {
                        if (hit.collider.gameObject.name.Length == 3)
                        {
                            keyboard.clickOnKey(hit.collider.gameObject.name[0].ToString());
                        }
                        else
                        {
                            string[] splittedKeyName = hit.collider.gameObject.name.Split('_');
                            keyboard.clickOnKey(splittedKeyName[0]);
                        }
                    }
                    else if (hit.collider.tag == "leaderboard_collider") // colliders for pages switching
                    {
                        lbManager.changePage(hit.collider.name);
                    }
                    else if (hit.collider.name.Contains("collider_diff_")) // colliders for difficulties showed by the leaderboard
                    {
                        lbManager.loadNewLeaderboard(hit.collider.name);
                    }
                    else // Sinon c'est un bouton du menu
                    {
                        //debugText3.text = "Button action1";
                        menuManager.SendMessage("receiveData", hit.collider.gameObject.name);
                        //menuManager.receiveData(hit.collider.gameObject, "click");
                    }
                    lastState = true;
                }
            }
            else if (lastState && Input.GetAxis("Oculus_CrossPlatform_SecondaryIndexTrigger") == 0)
            {
                lastState = false;
                //debugText3.text = "Button Released";
            }
        }
        else if (lastState) // Si le pointeur vient de sortir du bloc après avoir cliqué
        {
            lastState = false;
            
        }
        else if (hit.collider == null)
        {
            keyboard.clearKeys();
        }

        dot.transform.position = endPosition;

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, endPosition);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit rHit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out rHit, defaultLength);

        return rHit;
    }
}
