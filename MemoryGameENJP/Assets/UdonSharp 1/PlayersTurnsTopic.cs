
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using TMPro;

public class PlayersTurnsTopic : UdonSharpBehaviour
{
    //Reference Scripts
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
    public bool showStartButton = true;
    public TextMeshProUGUI winnerBoard;
    public GameObject playAgainButton;
    
    //Gameplay
    public GameObject shuffleButton;
    public TextMeshProUGUI debugLog;
    public GameObject cardBox;
    public string[] vocabSet;
    public GameObject nextButton;
    public TextMeshProUGUI turnTrackerDisplayBoard;
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
    int rotateTurnCount = 0;
    
    //UdonSynced Varibles below here:

    [UdonSynced] public int[] foundCards = new int[16];
    [UdonSynced] public int[] notFoundCards = new int[16];
    [UdonSynced] public int[] playerIds = new int[4];
    [UdonSynced] public string[] playerDisplayNames = new string[4];
    [UdonSynced] public string setToUse;
    [UdonSynced] public string deckName;
    [UdonSynced] public int playerCount;
    [UdonSynced] public int currentPlayerId;
    [UdonSynced] public int turnRoller = 0;
    [UdonSynced] public int[] playerScores = new int[4];
    [UdonSynced] public int pairsFound = 0;
    [UdonSynced] public string winnerString = "Wow look at all this stuff";


    void Start()
    {
        GatherBoardArrays();
        InitializeVocabSets();
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
            playerScores[i] = 0;
        }
    }

        private void InitializeVocabSets()
    {
        faceParts = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        animals = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        fruits = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        jobs = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        familyMembers = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        questionableActivities = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        schoolSubjects = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        love = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        fighting = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";    
        kansaiben = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        typesOfGovernment = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        leftovers = "1, one\nred, 2, two\norange, 3, three\nyellow, 4, four\ngreen, 5, five\nblue, 6, six\npurple, 7, seven\nblack, 8, eight\nwhite";
        faceParts = "face, 顔\nかお, ears, 耳\nみみ, nose, 鼻\nはな, hair, 髪\nかみ, mouth, 口\nくち, cheeks, 頬\nほほ, eyes, 目\nめ, eyebrows, 眉毛\nまゆげ";
        testExample = "A, あ\n lo, I, い¥n ho, U, う, E, え, O, お, KA, か, KI, き, KU, く";
    }
    public void SelectPlayerNumber(int playerNumber)
    {
        playerCount++;
        //debugLog.text = Networking.GetOwner(PlayerJoinButtonsEmpty).playerId.ToString();
        playerDisplayNames[playerNumber] = Networking.LocalPlayer.displayName;
        playerIds[playerNumber] = Networking.LocalPlayer.playerId;
        UpdateCurrentPlayers();
        foreach (GameObject button in playerButtons)
        {
            button.SetActive(false);
        }
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
                playerText[i].GetComponent<TextMeshProUGUI>().text = playerDisplayNames[i] + " " + playerScores[i];
            }
        printOut = printOut + " " + playerDisplayNames[i] + " " + playerScores[i];
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
        if (setToUse != null && playerCount > 0 && showStartButton == true)
        {
            startButton.SetActive(true);
            showStartButton = false;
        }
        CardPlacement.BuildVocabArray();
    }

    public void RotateTurn()
    {
        Networking.SetOwner(Networking.LocalPlayer,gameObject);

        //Debug rotation
        debugLog.text = $"RotateTurn Fired {rotateTurnCount} times";
        rotateTurnCount++;
        
        //Plain roller
        turnRoller++;
        if (turnRoller == 4)
        {
            turnRoller = 0;
        }
        currentPlayerId = playerIds[turnRoller];

        //Player not there, or roller is out of bounds
        while (currentPlayerId == -1)
        {

            currentPlayerId = playerIds[turnRoller];
            if (currentPlayerId == -1)
            {
                turnRoller++;
                if (turnRoller == 4)
                {
                turnRoller = 0;
                }
            }
            //Roller the result
            currentPlayerId = playerIds[turnRoller];
        }
        
        //Update everyone and board
        RequestSerialization();
        UpdateTurnTrackerDisplayBoard();
    }

    public void UpdateTurnTrackerDisplayBoard()
    {
        turnTrackerDisplayBoard.text = $"The turn roller is at {turnRoller}. The current player id is {currentPlayerId}. The player route is: {playerIds[0]}, {playerIds[1]}, {playerIds[2]}, {playerIds[3]}.";
        winnerBoard.text = winnerString;
    }

    public void ScoreGoesUp()
    {
        Networking.SetOwner(Networking.LocalPlayer,gameObject);
        playerScores[turnRoller] ++;
        pairsFound++;
        Debug.Log($"The score goes up {playerScores[turnRoller]}. Total pairs found {pairsFound}.");
        if (pairsFound >= 8)
        {
            Debug.Log("if 8 passed");
            CalculateTheWinner();
            playAgainButton.SetActive(true);
        }
        RequestSerialization();
        UpdateCurrentPlayers();
        UpdateTurnTrackerDisplayBoard();
    }

    public void CalculateTheWinner()
    {
        winnerString = " ";
        int maxElement = playerScores[0];
        for (int index = 1; index < playerScores.Length; index++)
        {
            if (playerScores[index] > maxElement)
            maxElement = playerScores[index];
        }
        for (int i = 0; i < playerDisplayNames.Length; i++)
        {
            if (playerScores[i] == maxElement)
            {
                winnerString = winnerString + "   " + playerDisplayNames[i];
            }
        }
    }

    public override void OnDeserialization()
    {
        UpdateCurrentPlayers();
        UpdateCurrentTopic();
        UpdateTurnTrackerDisplayBoard();
    }

    
    public void PlayerOneButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(0);
        Debug.Log("This is player 1");
    }

    public void PlayerTwoButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(1);
        Debug.Log("This is player 2");
    }

    public void PlayerThreeButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(2);
        Debug.Log("This is player 3");
    }

    public void PlayerFourButton()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectPlayerNumber(3);
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
