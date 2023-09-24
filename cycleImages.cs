using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cycleImages : MonoBehaviour
{
    public List<GameObject> cycleObjects;
    public bool loop = true;

    public int cycle = 0;
    public int frameNow = 0;
    public int frameWait = 10;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (cycle < cycleObjects.Count)
        {
            foreach (GameObject bc in cycleObjects)
            {
                bc.SetActive(false);
            }
            cycleObjects[cycle].SetActive(true);

            if (frameNow > frameWait)
            {
                frameNow = 0;
                cycle++;
            }
            else
            {
                frameNow++;
            }
        }
        else if (loop)
        {
            cycle = 0;
            foreach (GameObject bc in cycleObjects)
            {
                bc.SetActive(false);
            }
            cycleObjects[cycle].SetActive(true);
        }
    }
}
