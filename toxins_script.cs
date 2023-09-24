using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class toxins_script : MonoBehaviour
{
    public GameObject toxins;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(spray_time());
        //transform.localScale = Plant.instance.stages[Plant.instance.stage].stageObjects[0].transform.localScale;
    }


    IEnumerator spray_time()
    {
        yield return new WaitForSeconds(1);
        //Destroy(toxins);
        if (GetComponent<Collider>())
            Destroy(GetComponent<Collider>());

        yield return new WaitForSeconds(5);
        Destroy(toxins);
    }
}
