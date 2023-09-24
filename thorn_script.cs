using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class thorn_script : MonoBehaviour
{
    //public GameObject thorn;
    public float lifeTime = 1;
    public GameObject lookAt;

    private Vector3 startPos;
    private Vector3 posGoal;
    private Vector3 startScale;
    private Vector3 scaleGoal;

    private float speed = 0.1f;

    public GameObject thornSprite;

    private void Start()
    {

        startScale = transform.localScale;
        startPos = transform.localPosition;

        scaleGoal = new Vector3(startScale.x, startScale.y * 2, startScale.z);
        posGoal = transform.localPosition;
        //posGoal = transform.localPosition + (transform.up * startScale.y);
        //transform.localPosition = new Vector3(startPos.x, startPos.y - startScale.y, startPos.z);

        if (Plant.instance.thornAim)
        {
            lookAt = Plant.instance.thornAim;
            transform.LookAt(lookAt.transform);

            thornSprite.transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y * -1, 0);
        }

        //transform.localPosition -= transform.up * -startScale.y;
        transform.Translate(Vector3.down * startScale.y);

        Invoke("Shrink", lifeTime * 0.5f);
        //Destroy(thorn, lifeTime);
        Destroy(this.gameObject, lifeTime);
    }
    // Update is called once per frame
    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, posGoal, speed);
        transform.localScale = Vector3.Lerp(transform.localScale, scaleGoal, speed);

        //if (lookAt)
        //    transform.LookAt(lookAt.transform);

        if (speed == 0.11f)
            if (Vector3.Distance(transform.localPosition, posGoal) < 0.01f)
            {
                CancelInvoke("Shrink");
                Shrink();
            }
    }

    public void Shrink()
    {
        //posGoal = new Vector3(startPos.x, startPos.y - startScale.y, startPos.z);
        //posGoal = transform.localPosition - (transform.up * startScale.y * 1);
        scaleGoal = new Vector3(startScale.x, startScale.y, startScale.z);


        transform.Translate(Vector3.down * startScale.y * 0.5f);
        posGoal = transform.localPosition;
        transform.Translate(Vector3.up * startScale.y * 0.5f);
    }
    public void Spike()
    {
        speed = 0.11f;
    }
}
