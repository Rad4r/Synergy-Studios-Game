using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public GameManager GM;
    
    public float inputDelaytime = 1.2f;
    public Color defaultColor = new Color(1, 1, 1);
    public Color highlightColor = new Color(0.7f, 0.5f, 0.2f);
    public Color placeHighlightColor = new Color(0.6f, 0.8f, 0.9f);

    private Object emptyCard;
    private float screenHeight;
    private float screenWidth;

    public float cardSpeed = 5f;
    public float tableWidth = 1.2f;
    public float tableHeight = -0.07f;

    // 0 = default; 1 = dealCards; 2 = selecting; 3 = placing
    private int stage = 0;

    private GameObject[] tableFree = new GameObject[4];
    private GameObject[] tableFreeObjects = new GameObject[4];
    private GameObject[,] tableAces = new GameObject[2, 4];
    private GameObject[,] tableBot = new GameObject[8, 21];

    private GameObject tempCard5;

    private List<string> cardArr = new List<string>{"C1", "C2", "C3", "C4", "C5", "C6", "C7", "C8", "C9", "C10", "C11", "C12", "C13",
                         "H1", "H2", "H3", "H4", "H5", "H6", "H7", "H8", "H9", "H10", "H11", "H12", "H13",
                         "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "D10", "D11", "D12", "D13",
                         "S1", "S2", "S3", "S4", "S5", "S6", "S7", "S8", "S9", "S10", "S11", "S12", "S13"};

    public int cardAmount = 52;
    private int cardsDealt = 0;
    private int[,] endCardPositions;
    private int playableCardAmount = 0;
    private int placeAmount = 0;

    private int selectedCard;
    private int selectedRow = 0;
    private int tempCard7 = 0;
    private bool canEmptyColumn;
    private int selectedPlace;
    private int restartTimer = 0;

    // column 0 = cardX; column 1 = cardY; column 2 = table (0 = bot, 1 = free, 2 = ace); column 3 = following card count
    private int[,] playableCards;

    // column 0 = cardX; column 1 = cardY; column 2 = table (0 = bot, 1 = free, 2 = ace);
    private int[,] places;

    // played cards on the ace table; Returns value of the row
    private int[] aceCards;

    // x,y
    private int[,] tempCard3 = new int[4, 1];

    private Vector3 followingTargetDeviat;

    private void Start()
    {
        GM = FindObjectOfType<GameManager>();
        followingTargetDeviat = new Vector3(0, -tableWidth, tableHeight);
        emptyCard = Resources.Load("Prefabs/freeSpace");
        aceCards = new int[4];
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        dealCards();
    }

    private void Update()
    {
        if (Input.GetKeyDown(GM.primary))
        {
            if (stage == 5)
            {
                StopAllCoroutines();
                prepMoveCard(tempCard3[0, 0], tempCard3[1, 0], tempCard3[2, 0], tempCard3[3, 0], places[0, selectedPlace], places[1, selectedPlace], places[2, selectedPlace]);
            }

            if (stage == 3)
            {
                StopAllCoroutines();

                if (tempCard5 != null)
                {
                    Destroy(tempCard5);
                }

                tempCard3[0, 0] = playableCards[0, selectedCard];
                tempCard3[1, 0] = playableCards[1, selectedCard];
                tempCard3[2, 0] = playableCards[2, selectedCard];
                tempCard3[3, 0] = playableCards[3, selectedCard];
                selectedRow = playableCards[0, selectedCard];

                if (tempCard3[2, 0] == 0)
                {
                    findPlaces(tableBot[tempCard3[0, 0], tempCard3[1, 0]], tempCard3[3, 0]);
                }
                if (tempCard3[2, 0] == 1)
                {
                    findPlaces(tableFree[tempCard3[0, 0]], 0);
                }
                if (tempCard3[2, 0] == 2)
                {
                    findPlaces(tableAces[0, tempCard3[0, 0]], 0);
                }
            }
        }
        if (Input.GetKeyDown(GM.secondary))
        {
            if (stage == 5)
            {
                StopAllCoroutines();
                searchFree();
                if (tempCard3[2, 0] == 0)
                {
                    tableBot[tempCard3[0, 0], tempCard3[1, 0]].GetComponent<SpriteRenderer>().color = defaultColor;
                }
                if (tempCard3[2, 0] == 1)
                {
                    tableFree[tempCard3[0, 0]].GetComponent<SpriteRenderer>().color = defaultColor;
                }
                if (tempCard3[2, 0] == 2)
                {
                    tableAces[tempCard3[0, 0], tempCard3[1, 0]].GetComponent<SpriteRenderer>().color = defaultColor; ;
                }
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    private void dealCards()
    {
        stage = 1;
        cardsDealt = 0;

        Vector3 target;

        for (int free = 0; free < 4; free++)
        {
            GameObject tempCard5 = Instantiate(emptyCard, new Vector3(-5 + tableWidth * free, 3.3f), Quaternion.identity) as GameObject;

            target = new Vector3(-5 + tableWidth * free, 3.3f);

            tableFreeObjects[free] = tempCard5;
            tableFree[free] = tempCard5;
        }

        Object ace1 = Resources.Load("Prefabs/H0");
        tableAces[0, 0] = Instantiate(ace1, new Vector3(-5 + tableWidth * 4, 3.3f), Quaternion.identity) as GameObject;
        Object ace2 = Resources.Load("Prefabs/C0");
        tableAces[0, 1] = Instantiate(ace2, new Vector3(-5 + tableWidth * 5, 3.3f), Quaternion.identity) as GameObject;
        Object ace3 = Resources.Load("Prefabs/D0");
        tableAces[0, 2] = Instantiate(ace3, new Vector3(-5 + tableWidth * 6, 3.3f), Quaternion.identity) as GameObject;
        Object ace4 = Resources.Load("Prefabs/S0");
        tableAces[0, 3] = Instantiate(ace4, new Vector3(-5 + tableWidth * 7, 3.3f), Quaternion.identity) as GameObject;

        for (int c = cardAmount; c > 0; c--)
        {
            int tempRdm = Random.Range(0, c);
            Object tempCard = Resources.Load("Prefabs/Cards/" + cardArr[tempRdm]);
            cardArr.RemoveAt(tempRdm);

        rowLabel:
            int row = Random.Range(0, 8);
            for (int i = 0; i < 21; i++)
            {
                if (tableBot[row, i] == null)
                {
                    GameObject tempObj = Instantiate(tempCard, new Vector3(-8, 5, 0), Quaternion.identity) as GameObject;
                    target = new Vector3(-5 + tableWidth * row, -0.5f * i + 1, -0.2f * i);
                    StartCoroutine(moveCard(tempObj.transform, target, cardSpeed/2));
                    tableBot[row, i] = tempObj;

                    break;
                }

                if (i == 20)
                {
                    goto rowLabel;
                }
            }
        }
    }

    private void searchFree()
    {
        stage = 2;
        endCardPositions = new int[2, 8];
        playableCards = new int[4, 256];
        playableCardAmount = 0;
        bool canPlay = false;
        tempCard7 = 0;

        for (int free = 0; free < 4; free++)
        {
            if (tableFree[free].layer == 4)
            {
                canPlay = true;
            }
        }

        // set cards at the end of each row
        for (int h = selectedRow; h < 8; h++)
        {
            for (int v = 0; v < 21; v++)
            {
                if (tableBot[h, v] == null)
                {
                    if (v != 0)
                    {
                        if (tableBot[h, v - 1] != null)
                        {
                            endCardPositions[0, tempCard7] = h;
                            endCardPositions[1, tempCard7] = v - 1;
                            tempCard7++;
                        }
                    }
                    else
                    {
                        endCardPositions[0, tempCard7] = h;
                        endCardPositions[1, tempCard7] = 99;
                        tempCard7++;
                    }
                }
            }
        }

        if (selectedRow > 0)
        {
            for (int h = 0; h < selectedRow; h++)
            {
                for (int v = 0; v < 21; v++)
                {
                    if (tableBot[h, v] == null)
                    {
                        if (v != 0)
                        {
                            if (tableBot[h, v - 1] != null)
                            {
                                endCardPositions[0, tempCard7] = h;
                                endCardPositions[1, tempCard7] = v - 1;
                                tempCard7++;
                            }
                        }
                        else
                        {
                            endCardPositions[0, tempCard7] = h;
                            endCardPositions[1, tempCard7] = 99;
                            tempCard7++;
                        }
                    }
                }
            }
        }

        // go through the end cards and those behind to find playable cards
        for (int c = 0; c < 8; c++)
        {
            if (endCardPositions[1, c] != 99)
            {
                for (int fc = 1; fc < 12; fc++)
                {
                    if (endCardPositions[1, c] - fc >= 0)
                    {
                        if (tableBot[endCardPositions[0, c], endCardPositions[1, c] - fc] != null)
                        {
                            if (playable(tableBot[endCardPositions[0, c], endCardPositions[1, c] - fc + 1], tableBot[endCardPositions[0, c], endCardPositions[1, c] - fc]))
                            {
                                for (int endFc = 0; endFc < 8; endFc++)
                                {
                                    if (endCardPositions[1, endFc] != 99)
                                    {
                                        if (playable(tableBot[endCardPositions[0, c], endCardPositions[1, c] - fc], tableBot[endCardPositions[0, endFc], endCardPositions[1, endFc]]))
                                        {
                                            playableCards[0, playableCardAmount] = endCardPositions[0, c];
                                            playableCards[1, playableCardAmount] = endCardPositions[1, c] - fc;
                                            playableCards[2, playableCardAmount] = 0;
                                            playableCards[3, playableCardAmount] = fc;

                                            playableCardAmount++;
                                        }
                                    }
                                    else if (tableBot[endCardPositions[0, c], endCardPositions[1, c] - fc].tag == "13" && endCardPositions[1, c] - fc != 0)
                                    {
                                        playableCards[0, playableCardAmount] = endCardPositions[0, c];
                                        playableCards[1, playableCardAmount] = endCardPositions[1, c] - fc;
                                        playableCards[2, playableCardAmount] = 0;
                                        playableCards[3, playableCardAmount] = fc;

                                        playableCardAmount++;
                                    }
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                bool canPlayAce = false;
                GameObject tempCard6 = tableBot[endCardPositions[0, c], endCardPositions[1, c]];

                for (int ace = 0; ace < 4; ace++)
                {
                    if (tableAces[0, ace].layer == tempCard6.layer && tableAces[0, ace].tag == (int.Parse(tempCard6.tag) - 1).ToString())
                    {
                        // is the difference between the values of the cards smaller than 2 then move automatically

                        int t1 = aceCards[0];
                        int t2 = aceCards[1];
                        int t3 = aceCards[2];
                        int t4 = aceCards[3];

                        if (t1 - t2 > 2 && t1 - t2 < -2 && t1 - t3 > 2 && t1 - t3 < -2 && t1 - t4 > 2 && t1 - t4 < -2 && t2 - t3 > 2 && t2 - t3 < -2 && t2 - t4 > 2 && t2 - t4 < -2 && t3 - t4 > 2 && t3 - t4 < -2)
                        {
                            playableCards[0, playableCardAmount] = endCardPositions[0, c];
                            playableCards[1, playableCardAmount] = endCardPositions[1, c];
                            playableCards[2, playableCardAmount] = 0;
                            playableCards[3, playableCardAmount] = 0;

                            playableCardAmount++;
                            canPlayAce = true;

                            break;
                        }
                        else
                        {
                            prepMoveCard(endCardPositions[0, c], endCardPositions[1, c], 0, 0, ace, 0, 2);
                            canPlayAce = true;

                            return;
                        }
                    }
                }

                if (canPlay)
                {
                    playableCards[0, playableCardAmount] = endCardPositions[0, c];
                    playableCards[1, playableCardAmount] = endCardPositions[1, c];
                    playableCards[2, playableCardAmount] = 0;
                    playableCards[3, playableCardAmount] = 0;

                    playableCardAmount++;
                }
                else
                {
                    if (!canPlayAce)
                    {
                        for (int endC = 0; endC < 8; endC++)
                        {
                            if (endCardPositions[1, endC] != 99)
                            {
                                if (playable(tempCard6, tableBot[endCardPositions[0, endC], endCardPositions[1, endC]]))
                                {
                                    playableCards[0, playableCardAmount] = endCardPositions[0, c];
                                    playableCards[1, playableCardAmount] = endCardPositions[1, c];
                                    playableCards[2, playableCardAmount] = 0;
                                    playableCards[3, playableCardAmount] = 0;

                                    playableCardAmount++;
                                }
                            }
                            else if (tempCard6.tag == "13" && endCardPositions[1, c] != 0)
                            {
                                playableCards[0, playableCardAmount] = endCardPositions[0, c];
                                playableCards[1, playableCardAmount] = endCardPositions[1, c];
                                playableCards[2, playableCardAmount] = 0;
                                playableCards[3, playableCardAmount] = 0;

                                playableCardAmount++;
                            }
                        }
                    }
                }
            }
        }

        // go through cards in freecells to find playable cards
        for (int free = 0; free < 4; free++)
        {
            bool canPlayAce = false;
            GameObject tempCard6 = tableFree[free];

            for (int ace = 0; ace < 4; ace++)
            {
                if (tableAces[0, ace].layer == tempCard6.layer && tableAces[0, ace].tag == (int.Parse(tempCard6.tag) - 1).ToString())
                {
                    // is the difference between the values of the cards smaller than 2 then move automatically

                    int t1 = aceCards[0];
                    int t2 = aceCards[1];
                    int t3 = aceCards[2];
                    int t4 = aceCards[3];

                    if (t1 - t2 > 2 && t1 - t2 < -2 && t1 - t3 > 2 && t1 - t3 < -2 && t1 - t4 > 2 && t1 - t4 < -2 && t2 - t3 > 2 && t2 - t3 < -2 && t2 - t4 > 2 && t2 - t4 < -2 && t3 - t4 > 2 && t3 - t4 < -2)
                    {
                        playableCards[0, playableCardAmount] = free;
                        playableCards[1, playableCardAmount] = 0;
                        playableCards[2, playableCardAmount] = 1;
                        playableCards[3, playableCardAmount] = 0;

                        playableCardAmount++;
                        canPlayAce = true;

                        break;
                    }
                    else
                    {
                        prepMoveCard(free, 0, 1, 0, ace, 0, 2);
                        canPlayAce = true;

                        return;
                    }
                }
            }

            if (!canPlayAce)
            {
                for (int endC = 0; endC < 8; endC++)
                {
                    if (endCardPositions[1, endC] != 99)
                    {
                        if (playable(tempCard6, tableBot[endCardPositions[0, endC], endCardPositions[1, endC]]))
                        {
                            playableCards[0, playableCardAmount] = free;
                            playableCards[1, playableCardAmount] = 0;
                            playableCards[2, playableCardAmount] = 1;
                            playableCards[3, playableCardAmount] = 0;

                            playableCardAmount++;
                        }
                    }
                    else if (tempCard6.tag == "13" && endCardPositions[1, endC] != 0)
                    {
                        playableCards[0, playableCardAmount] = free;
                        playableCards[1, playableCardAmount] = 0;
                        playableCards[2, playableCardAmount] = 1;
                        playableCards[3, playableCardAmount] = 0;

                        playableCardAmount++;
                    }
                }
            }
        }

        // go through cards in ace places to find playable cards
        for (int ace = 0; ace < 4; ace++)
        {
            if (tableAces[0, ace].tag != "0" && tableAces[0, ace].tag != "1" && tableAces[0, ace].tag != "2")
            {
                if (canPlay)
                {
                    playableCards[0, playableCardAmount] = ace;
                    playableCards[1, playableCardAmount] = 0;
                    playableCards[2, playableCardAmount] = 2;
                    playableCards[3, playableCardAmount] = 0;

                    playableCardAmount++;
                }
                else
                {
                    GameObject tempCard6 = tableAces[0, ace];

                    for (int endC = 0; endC < 8; endC++)
                    {
                        if (endCardPositions[1, endC] != 99)
                        {
                            if (playable(tempCard6, tableBot[endCardPositions[0, endC], endCardPositions[1, endC]]))
                            {
                                playableCards[0, playableCardAmount] = ace;
                                playableCards[1, playableCardAmount] = 0;
                                playableCards[2, playableCardAmount] = 2;
                                playableCards[3, playableCardAmount] = 0;

                                playableCardAmount++;
                            }
                        }
                    }
                }
            }
        }
        // restart if after 3 tries no card can be played
        if (playableCards[2, 0] == 0 && playableCards[1, 0] == 0 && playableCards[3, 0] == 0)
        {
            if (playableCards[1, 0] != endCardPositions[1, 0])
            {
                restartTimer++;
                Debug.Log(restartTimer);

                if (restartTimer >= 3)
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }

        stage = 3;
        StartCoroutine(selectCard());
    }

    private IEnumerator selectCard()
    {
        for (int c = 0; c < playableCardAmount; c++)
        {
            // tableBot
            if (playableCards[2, c] == 0)
            {
                tableBot[playableCards[0, c], playableCards[1, c]].GetComponent<SpriteRenderer>().color = highlightColor;
                selectedCard = c;
                yield return new WaitForSeconds(inputDelaytime);
                tableBot[playableCards[0, c], playableCards[1, c]].GetComponent<SpriteRenderer>().color = defaultColor;
            }

            // tableFree
            else if (playableCards[2, c] == 1)
            {
                tableFree[playableCards[0, c]].GetComponent<SpriteRenderer>().color = highlightColor;
                selectedCard = c;
                yield return new WaitForSeconds(inputDelaytime);
                tableFree[playableCards[0, c]].GetComponent<SpriteRenderer>().color = defaultColor;
            }

            // tableAce
            else if (playableCards[2, c] == 2)
            {
                tableAces[0, playableCards[0, c]].GetComponent<SpriteRenderer>().color = highlightColor;
                selectedCard = c;
                yield return new WaitForSeconds(inputDelaytime);
                tableAces[0, playableCards[0, c]].GetComponent<SpriteRenderer>().color = defaultColor;
            }
        }

        StartCoroutine(selectCard());
    }

    private void findPlaces(GameObject card, int following)
    {
        stage = 4;
        places = new int[3, 16];
        placeAmount = 0;
        canEmptyColumn = true;

        if (following == 0)
        {
            for (int ace = 0; ace < 4; ace++)
            {
                if (tableAces[0, ace].layer == card.layer)
                {
                    if (tableAces[0, ace].tag == (int.Parse(card.tag) - 1).ToString())
                    {
                        places[0, placeAmount] = ace;
                        places[2, placeAmount] = 2;
                        placeAmount++;
                        break;
                    }
                }
            }
        }

        for (int h = 0; h < 8; h++)
        {
            if (endCardPositions[1, h] != 99)
            {
                if (tableBot[endCardPositions[0, h], endCardPositions[1, h]] != null)
                {
                    if (playable(card, tableBot[endCardPositions[0, h], endCardPositions[1, h]]))
                    {
                        places[0, placeAmount] = endCardPositions[0, h];
                        places[1, placeAmount] = endCardPositions[1, h];
                        places[2, placeAmount] = 0;
                        placeAmount++;
                    }
                }
            }
            else if (card.tag == "13" && canEmptyColumn)
            {
                if (playableCards[2, selectedCard] != 0)
                {
                    places[0, placeAmount] = endCardPositions[0, h];
                    places[1, placeAmount] = 0;
                    places[2, placeAmount] = 0;
                    placeAmount++;
                    canEmptyColumn = false;
                }
                else if (playableCards[1, selectedCard] != 0)
                {
                    places[0, placeAmount] = endCardPositions[0, h];
                    places[1, placeAmount] = 0;
                    places[2, placeAmount] = 0;
                    placeAmount++;
                    canEmptyColumn = false;
                }
            }
        }
        if (following == 0)
        {
            for (int free = 0; free < 4; free++)
            {
                if (tableFree[free].layer == 4)
                {
                    places[0, placeAmount] = free;
                    places[2, placeAmount] = 1;
                    placeAmount++;
                    break;
                }
            }
        }
        stage = 5;

        StartCoroutine(whereToPlace());
    }

    private IEnumerator whereToPlace()
    {
        if (placeAmount > 0)
        {
            for (int p = 0; p < placeAmount; p++)
            {
                // tableBot
                if (places[2, p] == 0)
                {
                    if (tableBot[places[0, p], places[1, p]] != null)
                    {
                        tableBot[places[0, p], places[1, p]].GetComponent<SpriteRenderer>().color = placeHighlightColor;
                        selectedPlace = p;
                        yield return new WaitForSeconds(inputDelaytime);
                        tableBot[places[0, p], places[1, p]].GetComponent<SpriteRenderer>().color = defaultColor;
                    }
                    else
                    {
                        GameObject tempCard8 = Instantiate(emptyCard, new Vector3(-5 + tableWidth * places[0, p], 1), Quaternion.identity) as GameObject;
                        tempCard8.GetComponent<SpriteRenderer>().color = placeHighlightColor;
                        selectedPlace = p;
                        tempCard5 = tempCard8;
                        yield return new WaitForSeconds(inputDelaytime);
                        Destroy(tempCard8);
                    }
                }

                // tableFree
                else if (places[2, p] == 1)
                {
                    tableFree[places[0, p]].GetComponent<SpriteRenderer>().color = placeHighlightColor;
                    selectedPlace = p;
                    yield return new WaitForSeconds(inputDelaytime);
                    tableFree[places[0, p]].GetComponent<SpriteRenderer>().color = defaultColor;
                }

                // tableAce
                else if (places[2, p] == 2)
                {
                    tableAces[0, places[0, p]].GetComponent<SpriteRenderer>().color = placeHighlightColor;
                    selectedPlace = p;
                    yield return new WaitForSeconds(inputDelaytime);
                    tableAces[0, places[0, p]].GetComponent<SpriteRenderer>().color = defaultColor;
                }
            }
            searchFree();
        }
        else
        {
            restartTimer++;
            searchFree();
            Debug.Log("no places found");
        }
    }

    private void prepMoveCard(int x1, int y1, int table1, int following, int x2, int y2, int table2)
    {
        restartTimer = 0;
        stage = 6;
        GameObject tempCard4;
        if (table1 == 0)
        {
            tempCard4 = tableBot[x1, y1];
            tableBot[x1, y1] = null;
            selectedRow = x1;
        }
        else if (table1 == 1)
        {
            tempCard4 = tableFree[x1];
            tableFree[x1] = tableFreeObjects[x1];
            selectedRow = 0;
        }
        else if (table1 == 2)
        {
            tempCard4 = tableAces[0, x1];
            tableAces[1, x1] = tableAces[0, x1];
            aceCards[x1] -= 1;
            selectedRow = 0;
        }
        else
        {
            tempCard4 = tableBot[x1, y1];
            tableBot[x1, y1] = null;
            selectedRow = 0;
            Debug.Log("Error: table direction missing");
        }

        if (tempCard4 != null)
        {
            tempCard4.GetComponent<SpriteRenderer>().color = defaultColor;

            if (table2 == 0)
            {
                Vector3 target = new Vector3(0, 0, 0);

                if (tableBot[x2, y2] != null)
                {
                    tableBot[x2, y2].GetComponent<SpriteRenderer>().color = defaultColor;
                    target.x = tableBot[x2, y2].transform.position.x;
                    target.y = tableBot[x2, y2].transform.position.y - tableHeight;
                    target.z = tableBot[x2, y2].transform.position.z - 3.1f;
                    tableBot[x2, y2 + 1] = tempCard4;
                }
                else
                {
                    target.x = -5 + 1.5f * x2;
                    target.y = 1;
                    target.z = -3;

                    tableBot[x2, y2] = tempCard4;
                    y2 -= 1;
                }
                StartCoroutine(moveCard(tempCard4.transform, target, cardSpeed));
                if (table1 == 0)
                {
                    if (following != 0)
                    {
                        for (int fc = 1; fc < following + 1; fc++)
                        {
                            tempCard4 = tableBot[x1, y1 + fc];
                            target += followingTargetDeviat;
                            // Debug.Log($"move {tempCard4.name} to {target}");
                            tableBot[x1, y1 + fc] = null;
                            tableBot[x2, y2 + 1 + fc] = tempCard4;

                            StartCoroutine(moveCard(tempCard4.transform, target, cardSpeed));
                        }
                    }
                }
            }
            else if (table2 == 1)
            {
                tableFree[x2].GetComponent<SpriteRenderer>().color = defaultColor;
                Vector3 target = new Vector3(0, 0, 0);
                target.x = tableFree[x2].transform.position.x;
                target.y = tableFree[x2].transform.position.y;
                target.z = -3.1f;

                tableFree[x2] = tempCard4;

                StartCoroutine(moveCard(tempCard4.transform, target, cardSpeed));
            }
            else if (table2 == 2)
            {
                tableAces[0, x2].GetComponent<SpriteRenderer>().color = defaultColor;
                Vector3 target = new Vector3(0, 0, 0);
                target.x = tableAces[0, x2].transform.position.x;
                target.y = tableAces[0, x2].transform.position.y;
                target.z = tableAces[0, x2].transform.position.z - 3.2f;

                tableAces[0, x2] = tempCard4;
                tableAces[1, x2] = tableAces[0, x2];

                aceCards[x2] = y1;

                if (aceCards[0] == 13 && aceCards[1] == 13 && aceCards[2] == 13 && aceCards[3] == 13)
                {
                    Debug.Log("Congrats, you won!");
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                }

                StartCoroutine(moveCard(tempCard4.transform, target, cardSpeed));
            }
            else
            {
                Debug.Log("Error: Can't find table");
                searchFree();
            }
        }
    }

    private IEnumerator moveCard(Transform moveMe, Vector3 target, float speed)
    {
        bool isMoving = false;

        if (isMoving)
        {
            yield break;
        }

        isMoving = true;

        while (moveMe.transform.position != target)
        {
            Vector3 currentPosition = moveMe.transform.position;
            Vector3 newPosition = Vector3.MoveTowards(currentPosition, target, speed * Time.deltaTime);
            moveMe.transform.position = newPosition;
            yield return null;
        }
        isMoving = false;
        Vector3 tempPos = new Vector3(moveMe.transform.position.x, moveMe.transform.position.y, moveMe.transform.position.z + 3);
        moveMe.transform.position = tempPos;

        if (stage != 1)
        {
            searchFree();
        }
        else
        {
            cardsDealt++;

            if (cardsDealt >= 52)
            {
                searchFree();
            }
        }
    }

    // can card1 be placed on top of card2
    private bool playable(GameObject card1, GameObject card2)
    {
        // Debug.Log($"card1 tag {card1.tag} and layer {card1.layer} on tag {card2.tag} and layer {card2.layer}?");

        if (card1.tag == (int.Parse(card2.tag) - 1).ToString())
        {
            if (card2.layer == 9 || card2.layer == 11)
            {
                if (card1.layer != 9 && card1.layer != 11)
                {
                    return true;
                }
            }
            if (card2.layer == 8 || card2.layer == 10)
            {
                if (card1.layer != 8 && card1.layer != 10)
                {
                    return true;
                }
            }
        }
        return false;
    }
}