using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flytrap_script : MonoBehaviour
{
    public float coolDown = 3;
    public float waiting = 1;

    public List<enemy_script> enemiesDetected;
    public enemy_script currentTarget;

    public Vector3 goalPos;
    private GameObject aim;

    public int biteCycle = 0;
    public List<GameObject> biteCycleObjects;
    public int frameNow = 0;
    public int frameWait = 10;

    private Quaternion startRotation;
    // Start is called before the first frame update
    void Start()
    {
        aim = new GameObject();
        aim.name = transform.name + " aim";
        aim.transform.parent = transform.parent;
        aim.transform.localPosition = Vector3.zero;
        aim.transform.localRotation = transform.localRotation;
        aim.transform.localRotation = transform.localRotation;
       startRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        //recharge
        if (waiting > 0)
        {
            waiting -= Time.deltaTime;

        }
        //ready
        else
        {
            if (currentTarget)
            {
                //eat enemy
                currentTarget.hp = 0;
                enemiesDetected.Remove(currentTarget);
                currentTarget = null;
                waiting = coolDown;

                biteCycle = 0;
            }
        }

        //find enemy position
        if (currentTarget)
        {
            goalPos = currentTarget.transform.position;
            aim.transform.LookAt(goalPos);
            aim.transform.localEulerAngles = new Vector3(0, aim.transform.localEulerAngles.y, 0);
        }
        else
        {
            aim.transform.rotation = Quaternion.Lerp(aim.transform.rotation, startRotation, 0.1f);
        }

        //look at enemy
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, aim.transform.rotation, 0.1f);
        }

        for(int i = 0; i < enemiesDetected.Count; i++)
        {
            if (enemiesDetected[i] != null)
            {
                bool alreadyTargeted = false;
                if (enemiesDetected[i].targeted)
                    if (enemiesDetected[i].targeted.gameObject.activeInHierarchy)
                        alreadyTargeted = true;

                if (enemiesDetected[i].hp > 0 &! alreadyTargeted)
                {
                    if (currentTarget == null)
                    {
                        currentTarget = enemiesDetected[i];
                        currentTarget.targeted = this;
                    }
                    //find enemy with highest hp
                    else if (enemiesDetected[i].hp > currentTarget.hp)
                    {
                        if (currentTarget.targeted)
                            if (currentTarget.targeted == this)
                                currentTarget.targeted = null;

                        currentTarget = enemiesDetected[i];
                        currentTarget.targeted = this;
                    }
                }
            }
        }

        if (biteCycle < biteCycleObjects.Count)
        {
            foreach (GameObject bc in biteCycleObjects)
            {
                bc.SetActive(false);
            }
            biteCycleObjects[biteCycle].SetActive(true);
            if (frameNow > frameWait)
            {
                frameNow = 0;
                biteCycle++;
            }
            else
            {
                frameNow++;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            if (other.GetComponent<enemy_script>())
                enemiesDetected.Add(other.GetComponent<enemy_script>());
        }
    }
}
