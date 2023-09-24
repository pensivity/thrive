using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy_script : MonoBehaviour
{
    // Set up variables
    public int score_increase;
    public int hp;
    public float speed;
    public GameObject enemy_go;
    public Rigidbody enemy;
    public Plant plant_script;
    public Transform plant;
    public Vector3 plant_dir;
    public bool can_atk;
    public bool attacking;

    //[HideInInspector]
    public flytrap_script targeted;

    public GameObject walkAnimation;
    public GameObject attackAnimation;

    public List<GameObject> enemy1Objects;
    public List<GameObject> enemy2Objects;
    public List<GameObject> enemy3Objects;
    private int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        // Assign variables
        score_increase = 1;
        hp = 2;
        can_atk = false;
        attacking = false;
        speed = 7f;
        enemy = GetComponent<Rigidbody>();

        plant_script = Plant.instance;
        plant = plant_script.transform;
        plant_dir = plant.position - enemy.position;
        plant_dir = new Vector3(plant_dir.x, 0, plant_dir.z).normalized;

        if(plant_script.stage >= 2)
        {
            //chance to upgrade enemy
            if (enemy2Objects.Count > 0)
            {
                if (Random.Range(0f, 1f) < 0.2f + plant_script.stage * 0.05f)
                {
                    level = 2;
                    hp = 3;

                    foreach (GameObject obj in enemy1Objects)
                        obj.SetActive(false);
                    foreach (GameObject obj in enemy2Objects)
                        obj.SetActive(true);
                    foreach (GameObject obj in enemy3Objects)
                        obj.SetActive(false);
                }
            }
            if (plant_script.stage >= 3)
                if (enemy3Objects.Count > 0)
                {
                    if (Random.Range(0f, 1f) < plant_script.stage * 0.05f)
                    {
                        level = 3;
                        hp = 4;

                        foreach (GameObject obj in enemy1Objects)
                            obj.SetActive(false);
                        foreach (GameObject obj in enemy2Objects)
                            obj.SetActive(false);
                        foreach (GameObject obj in enemy3Objects)
                            obj.SetActive(true);
                    }
                }
        }

        //flip
        if (transform.position.x < plant.position.x)
        {
            Transform flip = transform.GetChild(0);
            //flip.transform.localEulerAngles = new Vector3(0, 180, 0);
            flip.localScale = new Vector3(flip.localScale.x * -1, flip.transform.localScale.y, flip.transform.localScale.z);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // For weird spawns
        if (enemy_go.transform.position.y < 0)
        {
            Destroy(enemy_go);
        }

        // Kill the enemy
        if (hp <= 0)
        {
            plant_script.currentPoints += score_increase;
            Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.enemythorndeath);
            Destroy(enemy_go);
        }

        //check range
        if(Vector3.Distance(transform.position, plant.position) > 0.55f + (plant_script.stage * 0.45f))
        {
            can_atk = false;
        }
        else
        {
            can_atk = true;
        }

        if (!attacking)
        {
            if (!can_atk)
            {
                // Make the enemy move towards the plant
                //enemy.AddForce(plant_dir * speed);
                if (Vector3.Distance(transform.position, plant.position) > plant_script.stage * 2)
                {
                    enemy.transform.position = Vector3.Lerp(enemy.transform.position, plant.transform.position, 0.01f);
                }
                else if (Vector3.Distance(transform.position, plant.position) > plant_script.stage * 1)
                {
                    enemy.transform.position = Vector3.Lerp(enemy.transform.position, plant.transform.position, 0.02f);
                }
                else if (Vector3.Distance(transform.position, plant.position) > plant_script.stage * 0.3f)
                {
                    enemy.transform.position = Vector3.Lerp(enemy.transform.position, plant.transform.position, 0.05f);
                }
            }
            else
            {
                attacking = true;
                can_atk = false;
                StartCoroutine(Attack());
            }
        }

        //animations
        if (attacking)
        {
            walkAnimation.SetActive(false);
            attackAnimation.SetActive(true);
        }
        else
        {
            walkAnimation.SetActive(true);
            attackAnimation.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Thorns
        if (other.CompareTag("Thorn"))
        {
            if (plant_script.stage > 3)
                hp -= 2;
            else
                hp -= 1;

            if (hp <= 0)
            {
                Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.enemythorndeath);
            }
            else
            {
                Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.enemyhit);
            }
            Destroy(other.gameObject);
        }

        // Toxic gas attack
        if (other.CompareTag("Toxins"))
        {
            if (plant_script.stage > 3)
                hp -= 3;
            else
                hp -= 2;

            if (hp <= 0)
            {
                Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.enemytoxindeath);
            }
            else
            {
                Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.enemyhit);
            }

        }

        if (other.CompareTag("Plant"))
        {
            // If close enough to plant, start attacking
            speed = 6.3f;
            can_atk = true;
        }
    }

    IEnumerator Attack ()
    {
        if (level == 1)
        {
            yield return new WaitForSeconds(2);
            // Reduce plant health, do attack animation?
            plant_script.plant_health -= 1;
            Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.planthit);
            attacking = false;
            yield return new WaitForSeconds(2);
            can_atk = true;
            yield return new WaitForSeconds(0);
        }
        else if (level == 2)
        {
            yield return new WaitForSeconds(1);
            // Reduce plant health, do attack animation?
            plant_script.plant_health -= 2;
            Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.planthit);
            attacking = false;
            if(plant_script.stage < 3)
                yield return new WaitForSeconds(2);
            else
                yield return new WaitForSeconds(1);
            can_atk = true;
            yield return new WaitForSeconds(0);
        }
        else if (level == 3)
        {
            yield return new WaitForSeconds(0.5f);
            // Reduce plant health, do attack animation?
            plant_script.plant_health -= 2;
            Plant.instance.genericsfxaudiosource.PlayOneShot(Plant.instance.planthit);
            attacking = false;
            yield return new WaitForSeconds(0.5f);
            can_atk = true;
            yield return new WaitForSeconds(0);
        }
    }
}
