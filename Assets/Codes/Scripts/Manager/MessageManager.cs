using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;
    
    public GameObject hintText;

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;


    public GameObject choiceButtonPrefab;

    public Transform choiceHolder;

    public GameObject promptInput;

    public MessageSO currentMessage;

    public float inputCDTime = 5;

    public bool triggerEnd;

    public bool hasResponse;

    private float _floatCDTime;
    private int messageIndex;
    private int messageCount;
    private int choiceCount;

    private bool inputKeyDownCD;
    private bool isString;

    public void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    public void Start()
    {
        _floatCDTime = inputCDTime;
    }

    public void Update()
    {
        WaitForResponse();
        InputKeyDownCD();
    }

    public void StartMessage(string text)
    {
        choiceHolder.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.None;

        isString = true;

        inputKeyDownCD = true;
        _floatCDTime = inputCDTime;

        DisplayString(text);
    }

    public void StartMessage(MessageSO passInMessage)
    {
        choiceHolder.gameObject.SetActive(false);

        GameManager.Instance.TriggerUnlock();

        // Get the messageSO
        if (passInMessage == null)
            throw new Exception("Dude, there's no MessageSO.");
        else
            currentMessage = passInMessage;

        isString = false;

        // Get how many narrative is inside the Scriptable Object
        messageCount = currentMessage.narratives.Count;
        messageIndex = 0;
        inputKeyDownCD = true;

        // Display the mssage here
        DisplayMessage();


    }

    private void DisplayMessage()
    {
        // If got choice
        if (currentMessage.narratives[messageIndex].choices.Count > 0)
        {
            foreach (Transform child in choiceHolder)
            {
                Destroy(child.gameObject);
            }

            hasResponse = true;
            // Set the panel to inactive first
            promptInput.SetActive(false);
            messagePanel.SetActive(false);

            // Set the text
            messageText.text = currentMessage.narratives[messageIndex].message;

            // Get how many choice is there
            choiceCount = currentMessage.narratives[messageIndex].choices.Count;

            // Instantiate Button depend on how many choice it has
            for (int i = 0; i < choiceCount; i++)
            {
                ChoiceButtonComponent button = Instantiate(choiceButtonPrefab, choiceHolder).GetComponent<ChoiceButtonComponent>();
                button.reply.text = currentMessage.narratives[messageIndex].choices[i].choicesText;

                // Replace button font to "MessageAfterChoice"'s text
                if (currentMessage.narratives[messageIndex].choices[i].messageAfterChoice != null)
                {
                    button.messageSO = currentMessage.narratives[messageIndex].choices[i].messageAfterChoice;
                }

                // If there is no more messageSO, set it to close message.
                if (button.messageSO == null)
                {
                    // Need to improve at the futre. Code an If here, check if the dialogue has next one.
                    // If got, go to next message
                    // If no, closeMessage.
                    button.thisButton.onClick.AddListener(() =>
                    {
                        CloseMessage();
                    });
                }
                else
                {

                    button.thisButton.onClick.AddListener(() => { StartMessage(button.messageSO); });

                }

            }
            // Set the panel to active
            choiceHolder.gameObject.SetActive(true);
            messagePanel.SetActive(true);

        }
        // If there's no choice...
        else
        {
            // to control Input.AnyKeyDown
            hasResponse = false;
            inputKeyDownCD = true;
            // Press any key to continue call
            promptInput.SetActive(true);
            // Set the Message Panel gone
            messagePanel.SetActive(false);
            // Load the message into it
            messageText.text = currentMessage.narratives[messageIndex].message;
            // Set the message panel on
            messagePanel.SetActive(true);

        }
    }
    
    // If the function called NOT messageSO
    private void DisplayString(string text)
    {
        messageText.text = text;
        messagePanel.SetActive(true);
        promptInput.SetActive(true);

    }

    // When player Press Any Key Down
    private void WaitForResponse()
    {
        if (hasResponse)
        {
            return;
        }
        else
        {
            if (messagePanel.gameObject.activeSelf)
            {
                if (!inputKeyDownCD)
                {
                    if (Input.anyKey)
                    {
                        if (isString)
                        {
                            _floatCDTime = inputCDTime;
                            CloseMessage();

                        }
                        else
                        {
                            if (messageIndex >= messageCount - 1)
                            {
                                _floatCDTime = inputCDTime;
                                CloseMessage();
                            }
                            else
                            {
                                messageIndex++;
                                _floatCDTime = inputCDTime;
                                DisplayMessage();
                            }
                        }
                    }
                }
            }

        }
    }

    // Get Any Key Down to load next message
    private void InputKeyDownCD()
    {
        if (inputKeyDownCD)
        {
            _floatCDTime -= Time.deltaTime;

            if (_floatCDTime <= 0)
            {
                inputKeyDownCD = false;
            }
        }
    }

    // Close the message at end
    public void CloseMessage()
    {
        hasResponse = false;

        isString = false;

        promptInput.SetActive(false);

        messagePanel.SetActive(false);

        currentMessage = null;

        GameManager.Instance.TriggerLock(); ;
    }

}
