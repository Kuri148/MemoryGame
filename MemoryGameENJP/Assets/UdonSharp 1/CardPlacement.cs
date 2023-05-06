
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;
using TMPro; 


public class CardPlacement : UdonSharpBehaviour
{
    public PlayersTurnsTopic PlayersTurnsTopic;
    //Mainboard
    public GameObject PlayerTextEmpty;

    public GameObject PlayerJoinButtonsEmpty;
    public GameObject CardTopicEmpty;
    public GameObject startButton;
    public GameObject startPageInstructions;
    public TextMeshProUGUI topicChoice;
    public GameObject topicChoiceGameObject;
    public GameObject[] playerButtons = new GameObject[4];
    public GameObject[] cardTopics = new GameObject[12];
    public GameObject[] playerText = new GameObject[4];
    
    //Gameplay
    public TextMeshProUGUI debugLog;
    public GameObject[] cardDeck = new GameObject[16];
    Vector3[] cardTransforms = new Vector3[16];
    public GameObject cardBox;
    int cardIndex = 0;
    public string[] vocabSet;
    public string faceParts; //= "face, 顔\nかお, ears, 耳\nみみ, nose, 鼻\nはな, hair, 髪\nかみ, mouth, 口\nくち, cheeks, 頬\nほほ, eyes, 目\nめ, eyebrows, 眉毛\nまゆげ";
    public string rawVocab;
    public Vector3 noVectorYet;
    public GameObject placeKeeper;
    public GameObject[] selectedCards = new GameObject[2];
    public GameObject nextButton;

    //Memory decks here
    
    //UdonSynced Varibles below here:
    [UdonSynced] Vector3[] shuffledTransforms;
    [UdonSynced] public int[] selectedCardValues = new int[2];
    [UdonSynced] public int[] foundCards = new int[16];
    [UdonSynced] public int[] notFoundCards = new int[16];
    [UdonSynced] public bool isCorrect;
    [UdonSynced] public string synchronizationSwitch;
    [UdonSynced] public string[] playerDisplayNames = new string[4];

    
    //-----------------------------------------------------------------------------------------
    void Start()
    {
        nextButton.SetActive(false);
        noVectorYet = new Vector3(-13f,0f,0f);
        GatherBoardArrays();
        BuildInitialDeck();
        BuildInitialTransforms();
        TurnOnMenuBoard(true, false);
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
    }
    private void BuildInitialDeck()
    {
        for (int i = 0; i < cardBox.transform.childCount; i++)
        {
            GameObject card = cardBox.transform.GetChild(i).gameObject;
            cardDeck[i] = card;
            Button button = card.GetComponent<Button>();
            //button.interactable = false;
            button.name = cardIndex.ToString();
            cardIndex++;
            card.SetActive(false);
            //button.colors = buttonColors;
        }
    }
    private void BuildInitialTransforms()
    {
        int x = -8;
        int y = 4;
        for (int i = 0; i < 16; i++)
        {
            Vector3 cardPosition = new Vector3 (x, y, 0);
            cardTransforms[i] = cardPosition;
            x += 4;
            if (x == 8)
            {
                x = -8;
                y -= 2;
            }
        } 
    }

    public void TurnOnMenuBoard(bool onAndOff, bool justTheStartButton)
    {
        foreach (GameObject button in playerButtons)
        {
            button.SetActive(onAndOff);
        }
        foreach (GameObject topic in cardTopics)
        {
            int j = 0;
            Debug.Log($"Passing through {j}"); 
            topic.SetActive(onAndOff);
            j++;
        }
        topicChoiceGameObject.SetActive(onAndOff);
        startButton.SetActive(justTheStartButton);
        startPageInstructions.SetActive(onAndOff);
    }
    //--------------------------------------------------------------------------------------

        public void StartButtonPressed()
    {
        TurnOnMenuBoard(false, false);
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        ResetSelectedCards();
        RandomizeTransforms();
        SetFoundAndNotFoundCards();
        PlayersTurnsTopic.RotateTurn();
        synchronizationSwitch = "setBoard";
        RequestSerialization();
        PutVocabularyOntoCards();
        PlaceCardsOnTransforms();
    }

    
    public void BuildVocabArray()
    {   
        rawVocab = PlayersTurnsTopic.setToUse;
        vocabSet = rawVocab.Split(',');

        foreach (string vocab in vocabSet)
        {
            Debug.Log($"<{vocab}>");
        }
    }

    public void PutVocabularyOntoCards()
    {
        for (int i = 0 ; i < cardBox.transform.childCount; i++)
        {
            GameObject button = cardBox.transform.GetChild(i).gameObject;
            GameObject textField = button.transform.GetChild(0).gameObject;
            Debug.Log(textField==null);
            Debug.Log(vocabSet[i]);
            textField.GetComponent<TextMeshProUGUI>().text = vocabSet[i];
            textField.SetActive(false);
        }
    }
    //This needs to the udon synced
    public void RandomizeTransforms()
    {
        //Make sure shuffled transforms is full of (-13f, 0f 0f) vectors
        shuffledTransforms = new Vector3[16];
        for (int i= 0; i < shuffledTransforms.Length; i ++)
        {
            shuffledTransforms[i] = noVectorYet;
            Debug.Log($"i: {i}, {shuffledTransforms[i]}");
        }
        Debug.Log($"The length of shuffledTransforms is {shuffledTransforms.Length}");


        int arraySpotsChecked = 0;
        for (int i = 0; i < shuffledTransforms.Length; i++)
        {
            int entrancePoint = Random.Range(0, shuffledTransforms.Length);
            if (shuffledTransforms[entrancePoint] == noVectorYet)
            {
                shuffledTransforms[entrancePoint] = cardTransforms[i];
                Debug.Log($"Shuffle placement successful first try! Turn {i} {shuffledTransforms[i]}");
            }
            else
            {
                arraySpotsChecked = 0;
                while ( arraySpotsChecked < 20)
                {
                    entrancePoint++;
                    if (entrancePoint == shuffledTransforms.Length)
                    { 
                        entrancePoint = 0;
                    }
                    if (shuffledTransforms[entrancePoint] == noVectorYet)
                    {
                        shuffledTransforms[entrancePoint] = cardTransforms[i];
                        Debug.Log($"Shuffle placement on the {arraySpotsChecked} Turn {i} {shuffledTransforms[i]}");
                        break;
                    }
                    else
                    {
                        arraySpotsChecked++;
                    }
                }        
            }
        }

        Debug.Log($"The length of shuffledTransforms 2nd time is {shuffledTransforms.Length}");
        for (int i = 0; i < shuffledTransforms.Length; i++)
        {
            Debug.Log($"{i} {shuffledTransforms[i]} {shuffledTransforms.Length}");
        }
    }
    private void PlaceCardsOnTransforms()
    {
        for (int i = 0; i < shuffledTransforms.Length; i++)
        {
            cardDeck[i].transform.localPosition = shuffledTransforms[i];
            Debug.Log($"Button number {cardDeck[i].name}");
        }
        foreach (GameObject card in cardDeck)
        {
            card.SetActive(true);
        }
    }
    public void SetFoundAndNotFoundCards()
    {
        for (int i = 0; i < 16; i++)
        {
            foundCards[i] = -1;
            notFoundCards[i] = i;
        }
    }

    public void ResetSelectedCards()
    {
        for (int i = 0; i < selectedCardValues.Length; i++)
        {
            int uniqueLows = -1;
            selectedCardValues[i] = uniqueLows;
            uniqueLows--;
        }
    }

    public void SelectCards(int card)
    {
        for (int i = 0; i < selectedCards.Length; i++)
        {
            if (selectedCardValues[i] < 0 && selectedCardValues[0] != card)
            {
                selectedCardValues[i] = card;
                RevealSelectedCard(card);
                Debug.Log($"Chosen card: {selectedCardValues[i]}");
                break;
            }
        }
        if (selectedCardValues[0] != -1 && selectedCardValues[1] != -1)
        {
            Debug.Log($"Chosen pair: ({selectedCardValues[0]}, {selectedCardValues[1]})");
            CheckCards(selectedCardValues[0], selectedCardValues[1]);
        }
        else
        {
            Debug.Log("Pick another card.");
        }
    }

    public void CheckCards(int firstCard, int secondCard)
    {
        int correctOptionOne = 0;
        int correctOptionTwo = 1;

        for (int i = 0; i < 16; i++)
        {
            if (firstCard == correctOptionOne && secondCard == correctOptionTwo) //((firstCard, secondCard) == (correctOptionOne, correctOptionTwo))
            {
                Debug.Log("Correct!");
                isCorrect = true;
                AdjustFoundAndNotFoundCards(firstCard, secondCard);
                PlayersTurnsTopic.ScoreGoesUp();
                synchronizationSwitch = "showSelectedCards";
                RequestSerialization();
                NextPlayer(isCorrect);
                return;//Do something else here. 
            }
            else
            {
                correctOptionOne += 2; 
                correctOptionTwo += 2;
                if (correctOptionOne == 16 && correctOptionTwo == 17)
                {
                    correctOptionOne = 1; 
                    correctOptionTwo = 0;
                }
                if (correctOptionOne == 17 && correctOptionTwo == 16)
                {
                    isCorrect = false;
                    Debug.Log("Oh dear, it isn't a match.");
                    synchronizationSwitch = "showSelectedCards";
                    RequestSerialization();
                    isCorrect = false;
                    NextButtonAppears();
                }
            }
        }
    }
    public void RevealSelectedCard(int selectedCard)
    {
        foreach (GameObject card in cardDeck)
        {
            if (card.name == selectedCard.ToString())
            {
                GameObject cardText = card.transform.GetChild(0).gameObject;
                cardText.SetActive(true);

            }
        }
    }
    public void RevealSelectedCardsToEveryone()
    {
        foreach (int chosenCardValue in selectedCardValues)
        {
            foreach (GameObject card in cardDeck)
            {
                if (card.name == chosenCardValue.ToString())
                {
                    GameObject cardText = card.transform.GetChild(0).gameObject;
                    cardText.SetActive(true);
                }
            }
        }
    }
    public void NextButtonAppears()
    {
        nextButton.SetActive(true);
    }
    public void NextPlayer(bool isCorrect)
    {
        Networking.SetOwner(Networking.LocalPlayer,gameObject);
        if (isCorrect)
        {
            IntractableCards();
            PlayersTurnsTopic.ScoreGoesUp();
            ResetSelectedCards();
        }
        else
        {
            //You need to turn off the buttons here and add in the next button for single player and other people.
            ResetSelectedCards();
            PlayersTurnsTopic.RotateTurn();
        }
        
        synchronizationSwitch = "goingToTheNextPlayer";
        RequestSerialization();
    }
    public void HideIncorrectPair(int[] incorrectPair)
    {
        foreach (int mistake in incorrectPair)
        {
            foreach (GameObject card in cardDeck)
            {
                if (card.name == mistake.ToString())
                {
                    GameObject cardText = card.transform.GetChild(0).gameObject;
                    cardText.SetActive(false);
                }
            }
        }
    }
    public void DisableCorrectPair(int[] correctPair)
    {
        foreach (int cardValue in correctPair)
        {
            foreach (GameObject card in cardDeck)
            {
                if (card.name == cardValue.ToString())
                {
                    card.GetComponent<Button>().interactable = false;
                }
            }
        }
    }

    public void AdjustFoundAndNotFoundCards(int firstValue, int secondValue)
    {
        for (int i = 0; i < 16; i++)
        {
            if (notFoundCards[i] == firstValue || notFoundCards[i] == secondValue)
            {
                notFoundCards[i] = -1;
            }
            foundCards[firstValue] = firstValue;
            foundCards[secondValue] = secondValue;
        }
    }

    public void IntractableCards()
    {
        foreach (int foundCardValue in foundCards)
        {
            foreach (GameObject card in cardDeck)
            {
                if (card.name == foundCardValue.ToString())
                {
                    card.GetComponent<Button>().interactable = false;
                }
            }
        }
        foreach (int notFoundCardValue in notFoundCards)
        {
            foreach (GameObject card in cardDeck)
            {
                if (card.name == notFoundCardValue.ToString())
                {
                    GameObject cardText = card.transform.GetChild(0).gameObject;
                    cardText.SetActive(false);
                }
            }
        }
    }
    public override void OnDeserialization()
    {
        //debugLog.text = "case Deserialization switch is working!";
        switch (synchronizationSwitch)
        {
        case "setBoard":
            TurnOnMenuBoard(false, false);
            ResetSelectedCards();
            //BuildVocabArray();
            PutVocabularyOntoCards();
            PlaceCardsOnTransforms();
            break;

        case "showSelectedCards":
            RevealSelectedCardsToEveryone();
            break;

        case "goingToTheNextPlayer":
            debugLog.text = "Deserialization switch is working!";
            IntractableCards();
            ResetSelectedCards();
            break;

        case "justTesting":
            debugLog.text = "case Deserialization switch is working!";
            break;
        
        default:
            debugLog.text = "Deserialization is firing, but the switch is incorrect";
            break;
            
        }

    }
    //U# WHY U DON"T LET ME USE SYTEM EVENTS?!? LOOK WHAT YOU MADE ME DO.
    public void TellMeYourName0()
    {
        //Right in if statement here, so only one player can access; And cannot steal ownership
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(0);
        }
        Debug.Log("This is button 0");
    }
    public void TellMeYourName1()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(1);
        }
        Debug.Log("This is button 1");
    }

    public void TellMeYourName2()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(2);
        }
        Debug.Log("This is button 2");
    }    
    
    public void TellMeYourName3()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(3);
        }
        Debug.Log("This is button 3");
    }    
    
    public void TellMeYourName4()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        SelectCards(4);
        Debug.Log("This is button 4");
    }    
    
    public void TellMeYourName5()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(5);
        }
        Debug.Log("This is button 5");
    }    
    
    public void TellMeYourName6()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(6);
        }
        Debug.Log("This is button 6");
    }    
    
    public void TellMeYourName7()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(7);
        }
        Debug.Log("This is button 7");
    }    
    
    public void TellMeYourName8()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(8);
        }
        Debug.Log("This is button 8");
    }    
    
    public void TellMeYourName9()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(9);
        }
        Debug.Log("This is button 9");
    }    
    
    public void TellMeYourName10()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(10);
        }
        Debug.Log("This is button 10");
    }    
    
    public void TellMeYourName11()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(11);
        }
        Debug.Log("This is button 11");
    }    
    
    public void TellMeYourName12()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(12);
        }
        Debug.Log("This is button 12");
    }    
    
    public void TellMeYourName13()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(13);
        }
        Debug.Log("This is button 13");
    }    
    
    public void TellMeYourName14()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(14);
        }
        Debug.Log("This is button 14");
    }    
    
    public void TellMeYourName15()
    {
        Networking.SetOwner(Networking.LocalPlayer, gameObject);
        if (Networking.LocalPlayer.playerId == PlayersTurnsTopic.currentPlayerId)
        {
            SelectCards(15);
        }
        Debug.Log("This is button 15");
    }
    
}