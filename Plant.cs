using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Stage
{
    public int requiredPoints = 3;
    public int maxNumEnemies = 3;
    public float spawnCooldown = 1;
    public List<GameObject> stageObjects;
}

public class Plant : MonoBehaviour
{
    public static Plant instance;

    [Header("Menu elements")]
    public GameObject HUD;
    public GameObject pauseMenu;
    public GameObject instructions;
    public GameObject thornTips;
    public GameObject toxinTips;
    private bool paused;
    private bool helping;
    private bool thornCompleted;
    private bool toxinCompleted;

    public Slider healthBar;
    public Slider killCount;
    public GameObject toxinTimer;

    public GameObject growUI;

    [Header("Game trackers")]
    public int stage;
    private int nextStage;
    public int currentPoints;
    private int pointsChanged;
    public int winningPoints;
    public int plant_health;

    [Header("Abilities")]
    public GameObject thornAim;
    public GameObject thorn;
    public GameObject toxins;
    public float toxin_cooldown;
    private float toxin_cooldown_max;
    private bool toxins_used;

    [Header("World Objects")]
    public GameObject enemy;
    public Transform cam_pos;
    public float camSpeed = 0.05f;
    public Vector3 cameraGoal;
    public float cameraSizeGoal;


    //Declaration of List of audiosources and Clips
    [Header("Sound Set Up")]
    public AudioSource genericsfxaudiosource;
    public AudioSource toxinhitaudiosource;
    public AudioSource gametrackaudiosource;
    public AudioClip thornattack, planthit, toxinhit, gametrack, enemyspawn, enemyhit, win, levelup, enemythorndeath, enemytoxindeath, gameover;


    [Header("Stage Set Up")]
    public List<Stage> stages;

    public float spawnCooldown;

    // Start is called before the first frame update
    void Start()
    {
        // Set up basic unity stuff
        if (instance == null)
        {
            instance = this;
        }

        Time.timeScale = 1;

        // Set up parameters
        plant_health = 10;
        stage = 0;
        nextStage = stage;
        currentPoints = 0;
        pointsChanged = -1;
        winningPoints = 50;
        toxin_cooldown_max = 4;
        toxin_cooldown = 0;
        toxins_used = false;

        // Camera
        cameraGoal = cam_pos.transform.localPosition;
        cameraSizeGoal = Camera.main.orthographicSize;

        // Set up game menus
        HUD.SetActive(true);
        instructions.SetActive(false);
        pauseMenu.SetActive(false);
        thornTips.SetActive(false);
        toxinTips.SetActive(false);
        paused = false;
        helping = false;
        thornCompleted = false;
        toxinCompleted = false;

        // Set up HUD elements
        healthBar.maxValue = plant_health;
        healthBar.value = plant_health;
        killCount.maxValue = winningPoints;
        killCount.value = currentPoints;
        toxinTimer.SetActive(false);
        growUI.SetActive(false);

        // Set up sounds
        Plant.instance.gametrackaudiosource.clip = Plant.instance.gametrack;
        Plant.instance.gametrackaudiosource.loop = true;
        Plant.instance.gametrackaudiosource.Play();

        // Set up first level
        // Set all plant stages inactive
        for (int i = 0; i < stages.Count; i++)
        {
            foreach (GameObject obj in stages[i].stageObjects)
            {
                obj.SetActive(false);
            }
        }
        // Set the first level active
        foreach (GameObject obj in stages[stage].stageObjects)
        {
            obj.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Pause Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!helping)
            {
                PauseGame();
            } else
            {
                HelpMenu();
            }
        }


        if (!paused)
        {
            // Check if dead
            if (plant_health <= 0)
            {
                gametrackaudiosource.Pause();
                Loser();
            }


            //Check if enough points for next stage
            if (pointsChanged != currentPoints)
            {
                pointsChanged = currentPoints;

                if (stage + 1 < stages.Count)
                {
                    if (currentPoints >= stages[stage + 1].requiredPoints)
                    {
                        nextStage = stage + 1;
                    }
                }

                //only grow one stage at a time
                if (nextStage > stage + 1)
                {
                    nextStage = stage + 1;
                }
            }

            // Level up (using spacebar)
            if (stage != nextStage)
            {
                // Growth prompt
                if (growUI)
                {
                    growUI.SetActive(true);
                }

                //push space to grow
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    stage = nextStage;

                    // Growth prompt
                    if (growUI)
                    {
                        growUI.SetActive(false);
                    }

                    //reset point checker
                    pointsChanged -= 1;


                    // Have you won?
                    if (currentPoints >= winningPoints)
                    {
                        // Win the game!
                        Winner();
                    }

                    // Set all plant stages inactive
                    for (int i = 0; i < stages.Count; i++)
                    {
                        foreach (GameObject obj in stages[i].stageObjects)
                            obj.SetActive(false);
                    }

                    // Set up new camera parameters
                    cameraSizeGoal = 1.5f * stage + 1;
                    cameraGoal = new Vector3(cam_pos.localPosition.x, 0.5f * stage + 0.25f, cam_pos.localPosition.z);

                    thornAim.transform.localPosition = new Vector3(0, stage, 0);

                    // Set health
                    if (stage >= 1)
                    {
                        plant_health = 20 * stage;
                        healthBar.maxValue = plant_health;
                    }
                    else if (stage <= 0)
                    {
                        plant_health = 1;
                        healthBar.maxValue = plant_health;
                    }
                        
                    // Activate the next stage of plant objects
                    if (stage >= 0)
                    {
                        foreach (GameObject obj in stages[stage].stageObjects)
                            obj.SetActive(true);
                    }

                    // Level up soundtrack
                    genericsfxaudiosource.PlayOneShot(levelup);
                }
            }


            // Move camera if levelled up
            if (Vector3.Distance(cam_pos.transform.localPosition, cameraGoal) > 0.01)
            {
                Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, cameraSizeGoal, camSpeed);
                cam_pos.transform.localPosition = Vector3.Lerp(cam_pos.transform.localPosition, cameraGoal, camSpeed);
            }
            else
            {
                Camera.main.orthographicSize = cameraSizeGoal;
                cam_pos.localPosition = cameraGoal;
            }


            // Check where the mouse has clicked for basic attack
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit raycastHit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out raycastHit, 100f, 1 << 6))
                {
                    if (raycastHit.transform != null)
                    {
                        BasicAttack(raycastHit.point);
                    }
                }
            }

            

            // Spawn toxins if the cooldown is off and the stage is high enough
            if (stage > 1)
            {
                // Toxin ability
                toxinTimer.SetActive(true);
                toxinTimer.GetComponent<Slider>().value = toxin_cooldown_max - toxin_cooldown;
                toxinTimer.GetComponent<Slider>().transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("{0}", Mathf.Ceil(toxin_cooldown));

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    if (!toxins_used)
                    {
                        toxins_used = true;
                        // spawn toxins
                        toxin_cooldown = toxin_cooldown_max;
                        Vector3 pos = stages[stage].stageObjects[0].transform.GetChild(0).transform.position;
                        ToxinSpray(pos);
                        //StartCoroutine(ToxinCooldown());
                    }
                }
            }
            else
            {
                toxinTimer.SetActive(false);
            }

            // Check if toxins are on cooldown
            if (toxin_cooldown > 0)
            {
                toxin_cooldown -= Time.deltaTime;
            }
            else
            {
                toxins_used = false;
                toxinTimer.GetComponent<Slider>().transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>().text = "";
            }


            // Spawn new enemies if needed
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < stages[stage].maxNumEnemies)
            {
                if (spawnCooldown < 0)
                {
                    spawnCooldown = stages[stage].spawnCooldown;
                    SpawnEnemy();
                }
                else
                {
                    spawnCooldown -= Time.deltaTime;
                }
            }


            // Show thorn tutorial
            if (stage == 0 && !thornCompleted) {
                ThornTutorial();
            } 

            // Show toxin tutorial
            if (stage == 2 && !toxinCompleted)
            {
                ToxinTutorial();
            }


            // Update HUD elements
            healthBar.value = plant_health;
            killCount.value = currentPoints;
        }

    }

    public void BasicAttack (Vector3 pos)
    {
        genericsfxaudiosource.PlayOneShot(thornattack);
        GameObject newThorn = Instantiate(thorn, pos, Quaternion.identity);
        //newThorn.transform.localScale *= (stage + 1);
        newThorn.transform.localScale *= (stage * 1.6f) + 1;
    }

    public void ToxinSpray(Vector3 pos)
    {
        genericsfxaudiosource.PlayOneShot(toxinhit);
        GameObject newSpray = Instantiate(toxins, pos, Quaternion.identity);
        newSpray.transform.localScale *= (stage * 2);
        newSpray.transform.GetChild(0).transform.localScale *= (stage * 2);

        if (newSpray.transform.GetChild(0).GetComponent<ParticleSystem>())
        {
            ParticleSystem ps = newSpray.transform.GetChild(0).GetComponent<ParticleSystem>();
            ps.Stop();

            var m = ps.main;
            m.startLifetime = stage;
            m.duration = stage * 0.25f;
            var s = ps.shape;
            s.radius *= (1.0f / stage);

            ps.Play();
        }
    }

    public void SpawnEnemy ()
    {
        int spawns = stages[stage].stageObjects[1].transform.childCount;
        int spawn_num = Random.Range(0, spawns);
        Vector3 pos = stages[stage].stageObjects[1].transform.GetChild(spawn_num).transform.position;
        int ran = 3 + stage * 2;
        pos = new Vector3 (pos.x + Random.Range(-ran,ran), 1, pos.z + Random.Range(-ran, ran));
        Instantiate(enemy, pos, Quaternion.identity);
    }

    public void PauseGame()
    {
        if (paused)
        {
            Time.timeScale = 1;
            Plant.instance.paused = false;
            // Set menu inactive, unpause music
            Plant.instance.pauseMenu.SetActive(false);
            Plant.instance.gametrackaudiosource.clip = gametrack;
            Plant.instance.gametrackaudiosource.loop = true;
            Plant.instance.gametrackaudiosource.Play();
        }
        else
        {
            Time.timeScale = 0;
            Plant.instance.paused = true;
            // Set pause menu active, pause music
            Plant.instance.pauseMenu.SetActive(true);
            Plant.instance.gametrackaudiosource.Pause();
        }
    }

    public void HelpMenu ()
    {
        if (!helping)
        {
            instructions.SetActive(true);
            pauseMenu.SetActive(false);
            HUD.SetActive(false);
            helping = true;
        } else 
        {
            helping = false;
            instructions.SetActive(false);
            pauseMenu.SetActive(true);
            HUD.SetActive(true);
        }
    }

    public void QuitGame ()
    {
        SceneManager.LoadScene("menu screens");
    }

    public void Winner ()
    {
        SceneManager.LoadScene("win screen");
    }

    public void Loser ()
    {
        SceneManager.LoadScene("lose screen");
    }

    public void StartGame()
    {
        SceneManager.LoadScene("plant stages");
    }

    public void ThornTutorial()
    {
        // Show the thorn tips, wait until thorn is spawned
        Time.timeScale = 0.01f;
        thornTips.SetActive(true);

        if (GameObject.FindGameObjectsWithTag("Thorn").Length > 0)
        {
            Time.timeScale = 1;
            thornTips.SetActive(false);
            thornCompleted = true;
        }
    }

    public void ToxinTutorial()
    {
        // Show the thorn tips, wait until thorn is spawned
        Time.timeScale = 0.01f;
        toxinTips.SetActive(true);

        if (GameObject.FindGameObjectsWithTag("Toxins").Length > 0)
        {
            Time.timeScale = 1;
            toxinTips.SetActive(false);
            toxinCompleted = true;
        }
    }
}
