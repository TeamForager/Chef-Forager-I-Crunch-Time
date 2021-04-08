using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class TutorialController_Return : MonoBehaviour
{
    public GameObject[] instructionTriggers;
    public GameObject SpawnFoodLocation;
    public TutorialManager TutorialManager;
    public GameObject PossibleSeats;
    public Animator textAnimator;
    public GameObject customerTextUI;
    public GameObject clownerCust;
    public DialogueLoader dialogueLoader;
    public Restaurant_CustomerController customerController;
    // CookingStation to disable movement when speaking to ClownerCust
    public CookingStation movementController;
    public InventoryUI inventoryUI;
    public List<Item> startingItems;

    public AudioMixer audio;          // to fade sounds
    public LocateCamera cameraScript; // for shaking
    public GameObject blackscreen;

    bool cookedDish = false;
    bool pickedUpDish = false;
    bool firstCustArrived = false;
    bool firstCustLeft = false;
    bool spawnedClownerCust = false;

    GameObject firstCustSeat = null;

    private float shakeTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        inventoryUI.PopulateUI(startingItems);
        StartCoroutine("StartWelcome");
    }

    IEnumerator StartWelcome()
    {
        yield return new WaitForSeconds(1); // Allow scene to load before triggering welcome msg

        instructionTriggers[0].GetComponent<InstructionTrigger>().TriggerInstruction();
    }

    private void Update()
    {
        if (cookedDish == false)
        {
            if (SpawnFoodLocation.transform.childCount != 0) // food cooked and on counter
            {
                instructionTriggers[1].GetComponent<InstructionTrigger>().TriggerInstruction();
                cookedDish = true;
                StartCoroutine(AdvanceInstructions()); // Auto advance for this particular instruction
            }          
        }

        if (pickedUpDish == false)
        {
            if ( (SpawnFoodLocation.transform.childCount == 0) && (cookedDish == true) ) // food cooked and on counter
            {
                instructionTriggers[2].GetComponent<InstructionTrigger>().TriggerInstruction();
                pickedUpDish = true;
                StartCoroutine(AdvanceInstructions()); // Auto advance for this particular instruction
            }
        }

        if (firstCustArrived == false)
        {
            foreach (Transform seat in PossibleSeats.transform)
            {
                if (seat.childCount == 1) // our first customer arrived
                {
                    firstCustSeat = seat.gameObject; 
                    firstCustArrived = true;
                }
            }
        }
        
        if (!spawnedClownerCust && (firstCustLeft == true) && (textAnimator.GetBool("isOpen") == false)) // once instruction textbox goes away
        {
            //StartCoroutine(LoadGameScene()); // Previously used to immediately load next scene

            // Replace by triggering the dialogue, then transition to initial clown boss scene
            // DialogueDatabase.GetDialogue(15);
            spawnedClownerCust = true;
            Debug.Log("spawn clowner");
            StartCoroutine("SpawnClownerCust");
        }        
    }

    // Will be called by Return_CustCounter.cs when it tracks that one customer already left 
    public void ShowLeaveTutorialMsg()
    {
        instructionTriggers[3].GetComponent<InstructionTrigger>().TriggerInstruction();
        firstCustLeft = true; // mark that first cust has left, final msg shld be showing alr
    }

    IEnumerator SpawnClownerCust()
    {
        yield return new WaitForSeconds(2);

        clownerCust.SetActive(true);
        TutFindPlayer findPlayerAi = clownerCust.GetComponent<TutFindPlayer>();

        if (findPlayerAi) {
            findPlayerAi.SetReachedPlayerCallback(() => {
                movementController.DisableMovementOfPlayer();
                Dialogue clownerDialogue = DialogueParser.Parse(
                    "{[R]1}Rumour has it that the one behind this chaos is a clown-" +
                    "{[R]1}Wait... What's that sound?");
                dialogueLoader.LoadAndRun(clownerDialogue, customerController);
                dialogueLoader.SetDialogueEndCallback(() => {
                    Debug.Log("Meet Donald McRonald in 2.5 seconds");
                    StartCoroutine(LoadGameScene());
                });
            });
        }
    }

    IEnumerator ShakeCamera(float duration)
    {
        while (shakeTime < duration) {
            shakeTime = shakeTime + Time.deltaTime;
            cameraScript.SetShake(shakeTime / duration * 0.5f);
            yield return null;
        }
    }

    IEnumerator LoadGameScene()
    {
        float duration = 2.5f;
        StartCoroutine(AudioHelper.FadeAudio(audio, "Master_Vol", duration));
        // Add shaking effect to camera
        StartCoroutine(ShakeCamera(duration));
        blackscreen.SetActive(true);
        Animator blackscreenAnimator = blackscreen.GetComponent<Animator>();
        blackscreenAnimator.SetBool("TurnBlack", true); // Fade to black

        yield return new WaitForSeconds(duration); // Have some time for player to process everything before loading the game scene

        SceneManager.LoadScene((int)SceneIndexes.TUT_BOSS); // CHANGE TO INITIAL BOSS SCENE HERE
    }

    IEnumerator AdvanceInstructions()
    {
        yield return new WaitForSeconds(2); // Allow players to try out move before next sentence shows

        TutorialManager.DisplayNextSentence();
    }
}
