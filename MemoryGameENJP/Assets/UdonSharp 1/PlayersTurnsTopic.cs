
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
    public string mountains;
    public string testExample = "A, あ\n lo, I, い¥n ho, U, う, E, え, O, お, KA, か, KI, き, KU, くki";
    int rotateTurnCount = 0;
    
    //UdonSynced Varibles below here:

    [UdonSynced] public int[] foundCards = new int[16];
    [UdonSynced] public int[] notFoundCards = new int[16];
    [UdonSynced] public int[] playerIds = new int[4];
    [UdonSynced] public string[] playerDisplayNames = new string[4];
    [UdonSynced] public string setToUse = "empty";
    [UdonSynced] public string deckName;
    [UdonSynced] public int playerCount;
    [UdonSynced] public int currentPlayerId;
    [UdonSynced] public int turnRoller = -1;
    [UdonSynced] public int[] playerScores = new int[4];
    [UdonSynced] public int pairsFound = 0;
    [UdonSynced] public string winnerString = "Wow look at all this stuff";
    [UdonSynced] public string currentPlayerDisplayName;
    /* Testing if this needs to be synced*/ public bool showStartButton = true;
    [UdonSynced] public bool deckSelected = false;


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
        animals = "dog, 犬\nいぬ, cat, 猫\nねこ, elephant, 象\nぞう, bird, 鳥\nとり, bee, 蜂\nはち, fish, 魚\nさかな, snake, 蛇\nへび, cow, 牛\nうし";
        
        fruits = "strawberry, 苺\nいちご, banana, バナナ, apple, 林檎\nりんご, pear, 梨\nなし, grape, 葡萄\nぶどう, cherry, 桜ん坊\nさくらんぼ, lemon, 檸檬\nレモン, watermelon, 西瓜\nスイカ";
        
        jobs = "doctor, 医者\nいしゃ, plumber, 配管工\nはいかんこう, police officer, 警察\nけいさつ, programmer, プログラマー, teacher, 先生\nせんせい, unemployed, 無職\nむしょく, cashier, レジ, retired, 退職\nたいしょく";
        
        familyMembers = "father, 父\nちち, mother, 母\nはは, younger sister, 妹\nいもうと, younger brother, 弟\nおとうと, grandpa, お爺さん\nおじいさん, grandma, お婆さん\nおばあさん, older brother, お兄さん\nおにいさん, older sister, お姉さん\nおねえさん";
        
        questionableActivities = "doing drugs, 薬をする\nくすりをする, arson, 放火\nほうか, graffiti, 落書き\nらくがき, blackmail, 恐喝\nきょうかつ, bribe, 賄賂\nわいろ, forgery, 偽造\nぎぞう, black market, 闇市\nやみいち, catfishing, なりすまし";
        
        schoolSubjects = "science, 科学\nかがく, math, 数学\nすうがく, literature, 文学\nぶんがく, history, 歴史\nれきし, gym class, 体育\nたいいく, home economics, 家庭科\nかていか, art, 美術\nびじゅつ, homeroom, 朝/終礼\nちょう/しゅうれい";
        
        love = "first date, 初デート\nはつデート, hold hands, 手を繋ぐ\nてをつなぐ, confession, 告白\nこくはく, breakup, 別れる\nわかれる, to cheat, 浮気\nうわき, boyfriend, 彼氏\nかれし, girlfriend, 彼女\nかのじょ, ex-, 元\nもと";
        
        fighting = "punch, 殴る\nなぐる, kick, 蹴る\nける, pin, 動けなくする\nうごけなくする, trip, 足を引っ張る\nあしをひっぱる, push, 押す\nおす, sucker punch, いきなり殴る\nいきなりなぐる, body slam, 叩きつける\nたたきつける, choke, 首を絞める\nくびをしめる";    
        
        kansaiben = "what the heck, なんでやねん, no/bad, アカン, really, ほんま, thank you, おおきに, that's right, せやで, wrong, ちゃう, it suits you, におてる, funny, おもろい";
        
        typesOfGovernment = "democracy, 民主主義\nみんしゅしゅぎ, anarchy, 無政府\nむせいふ, communism, 共産主義\nきょうさんしゅぎ, dictatorship, 独裁\nどくさい, oligarchy, 寡頭政\nかとうせい, monarchy, 王政\nおうせい, republic, 共和国\nきょうわこく, socialism, 社会主義\nしゃかいしゅぎ";
        
        mountains = "mountain, 山\nやま, ridge, 尾根\nおね, peak, 頂上\nちょうじょう, foot, 麓\nふもと, ravine, 渓流\nけいりゅう, landside, 地滑り\nじすべり, spring, 泉\nいずみ, lumber road, 林道\nりんどう";
        
        faceParts = "face, 顔\nかお, ears, 耳\nみみ, nose, 鼻\nはな, hair, 髪\nかみ, mouth, 口\nくち, cheeks, 頬\nほほ, eyes, 目\nめ, eyebrows, 眉毛\nまゆげ";
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
        UpdateCurrentTopic();
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
        deckSelected = true;
        RequestSerialization();
        UpdateCurrentTopic();
    }

    public void UpdateCurrentTopic()
    {
        topicChoice.text = deckName;
        if ((deckSelected == true) && (playerCount > 0) && (showStartButton == true))
        {
            Debug.Log($"{setToUse} {playerCount} {showStartButton}");
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
        currentPlayerDisplayName = playerDisplayNames[turnRoller];
        Debug.Log($"Here is the current player {currentPlayerDisplayName}");
        //Update everyone and board
        RequestSerialization();
        UpdateTurnTrackerDisplayBoard();
    }

    public void UpdateTurnTrackerDisplayBoard()
    {
        //turnTrackerDisplayBoard.text = $"The turn roller is at {turnRoller}. The player route is: {playerIds[0]}, {playerIds[1]}, {playerIds[2]}, {playerIds[3]}.";
        turnTrackerDisplayBoard.text = $"{currentPlayerDisplayName}";
        if (pairsFound >= 8)
        {
            playAgainButton.SetActive(true);
            turnTrackerDisplayBoard.text = "Congratulations:" + " " +  winnerString;
        }


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

     void CalculateTheWinner()
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

    public void PlayersAndScoresReset()
    {
        for (int i = 0; i < 4; i++)
        {
            playerIds[i] = -1;
            playerDisplayNames[i] = "nobody";
            playerScores[i] = 0;
            playerText[i].GetComponent<TextMeshProUGUI>().text = "Player " + i.ToString();
        }
        showStartButton = true;
        deckSelected = false;
        pairsFound = 0;
        winnerString = " ";
        turnRoller = -1;
        playerCount = 0;
        currentPlayerDisplayName = " ";
        topicChoice.text = "Choose a topic.";
        playAgainButton.SetActive(false);
        UpdateCurrentPlayers();
        UpdateTurnTrackerDisplayBoard();
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
        SelectTopic("mountains", mountains);
        Debug.Log("This is topic zero.");
    }
}
