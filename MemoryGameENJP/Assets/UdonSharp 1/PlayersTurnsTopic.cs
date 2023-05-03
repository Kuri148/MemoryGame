
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

public class PlayersTurnsTopic : UdonSharpBehaviour
{
    public CardPlacement CardPlacement;
        //Mainboard
    public GameObject PlayerTextEmpty;
    public GameObject PlayerJoinButtonsEmpty;
    public GameObject CardTopicEmpty;
    public GameObject startButton;
    public GameObject startPageInstructions;
    public TextMeshProUGUI topicChoice;
    public GameObject[] playerButtons = new GameObject[4];
    public GameObject[] cardTopics = new GameObject[12];
    public GameObject[] playerText = new GameObject[4];
    
    //Gameplay
    public GameObject shuffleButton;
    public TextMeshProUGUI debugLog;
    public GameObject cardBox;
    public string[] vocabSet;
    public GameObject nextButton;

    //Memory decks here
    public string faceParts;
    public string animals;
    public string fruits;
    public string jobs;
    public string familyMembers;
    public string questionableActivities;
    public string schoolSubjects;
    public string love;
    public string fighting;
    public string kansaiben;
    public string typesOfGovernment;
    public string leftovers;
    public string testExample = "A, あ\n lo, I, い¥n ho, U, う, E, え, O, お, KA, か, KI, き, KU, くki";
    
    //UdonSynced Varibles below here:

    [UdonSynced] public int[] foundCards = new int[16];
    [UdonSynced] public int[] notFoundCards = new int[16];
    [UdonSynced] public string synchronizationSwitch;
    [UdonSynced] public int[] playerIds = new int[4];
    [UdonSynced] public string[] playerDisplayNames = new string[4];
    [UdonSynced] public string setToUse;
    [UdonSynced] public string deckName;
    [UdonSynced] public int playerCount;

    void Start()
    {
        GatherBoardArrays();
    }

    public void GatherBoardArrays()
    {
        for (int i = 0; i < playerButtons.Length; i++)
        {
            GameObject button = PlayerJoinButtonsEmpty.transform.GetChild(i).gameObject;
            playerButtons[i] = button;

            GameObject text = PlayerTextEmpty.transform.GetChild(i).gameObject;
            playerText[i] = text;
        }
        for (int j = 0; j < cardTopics.Length; j++)
        {
            GameObject topic = CardTopicEmpty.transform.GetChild(j).gameObject;
            cardTopics[j] = topic;
            GameObject textField = topic.transform.GetChild(0).gameObject;
            textField.GetComponent<TextMeshProUGUI>().text = topic.name;
        }
        for (int i = 0; i < 4; i++)
        {
            playerIds[i] = -1;
            playerDisplayNames[i] = "nobody";
        }
    }

        private void InitializeVocabSets()
    {
        faceParts = "face, 顔\nかお, ears, 耳\nみみ, nose, 鼻\nはな, hair, 髪\nかみ, mouth, 口\nくち, cheeks, 頬\nほほ, eyes, 目\nめ, eyebrows, 眉毛\nまゆげ";
        testExample = "A, あ\n lo, I, い¥n ho, U, う, E, え, O, お, KA, か, KI, き, KU, く";
    }
    public void SelectPlayerNumber(int playerNumber)
    {
        playerCount++;
        debugLog.text = Networking.GetOwner(PlayerJoinButtonsEmpty).playerId.ToString();
        playerDisplayNames[playerNumber - 1] = Networking.LocalPlayer.displayName;
        playerIds[playerNumber - 1] = Networking.LocalPlayer.playerId;
        UpdateCurrentPlayers();
        foreach (GameObject button in playerButtons)
        {
            button.SetActive(false);
        }
        synchronizationSwitch = "playerAdded";
        RequestSerialization();
    }
    
    public void UpdateCurrentPlayers()
    {
        string printOut = ""; 
        for (int i = 0; i < 4; i++)
        {
            if (playerIds[i] != -1)
            {
                playerButtons[i].SetActive(false);
                playerText[i].GetComponent<TextMeshProUGUI>().text = playerDisplayNames[i];
            }
        printOut = printOut + " " + playerDisplayNames[i];
        }
        Debug.Log(printOut);
        //debugLog.text = "Everything all right here?";
    }

    public void SelectTopic(string topicName, string deckContents)
    {
        deckName = topicName;
        setToUse = deckContents;
        RequestSerialization();
        UpdateCurrentTopic();
    }

    public void UpdateCurrentTopic()
    {
        topicChoice.text = deckName;
        if (setToUse != null && playerCount > 0)
        {
            startButton.SetActive(true);
        }
        CardPlacement.BuildVocabArray(deckName);
    }

    public override void OnDeserialization()
    {
        UpdateCurrentPlayers();
        UpdateCurrentTopic();
    }
    public void PlayerOneButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(1);
        Debug.Log("This is player 1");
    }

    public void PlayerTwoButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(2);
        Debug.Log("This is player 2");
    }

    public void PlayerThreeButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(3);
        Debug.Log("This is player 3");
    }

    public void PlayerFourButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(4);
        Debug.Log("This is player 4");
    }

    public void TopicZero()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("Face Parts", faceParts);
        Debug.Log("This is topic zero.");
    }
        public void TopicOne()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("animals", animals);
        Debug.Log("This is topic zero.");
    }
        public void TopicTwo()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("fruits", fruits);
        Debug.Log("This is topic zero.");
    }
        public void TopicThree()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("jobs", jobs);
        Debug.Log("This is topic zero.");
    }
        public void TopicFour()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("familyMembers", familyMembers);
        Debug.Log("This is topic zero.");
    }    public void TopicFive()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("questionableActivities", questionableActivities);
        Debug.Log("This is topic zero.");
    }    public void TopicSix()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("schoolSubjects", schoolSubjects);
        Debug.Log("This is topic zero.");
    }    public void TopicSeven()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("love", love);
        Debug.Log("This is topic zero.");
    }    public void TopicEight()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("fighting", fighting);
        Debug.Log("This is topic zero.");
    }    public void TopicNine()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("kansaiben", kansaiben);
        Debug.Log("This is topic zero.");
    }    public void TopicTen()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("typesOfGovernment", typesOfGovernment);
        Debug.Log("This is topic zero.");
    }    public void TopicEleven()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectTopic("leftovers", leftovers);
        Debug.Log("This is topic zero.");
    }
}
