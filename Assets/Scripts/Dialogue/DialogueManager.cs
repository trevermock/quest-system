using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Runtime;

public class DialogueManager : MonoBehaviour
{
    [Header("Ink Story")]
    [SerializeField] private TextAsset inkJson;

    private Story story;
    private int currentChoiceIndex = -1;

    private bool dialoguePlaying = false;

    private InkExternalFunctions inkExternalFunctions;
    private InkDialogueVariables inkDialogueVariables;

    private void Awake() 
    {
        story = new Story(inkJson.text);
        inkExternalFunctions = new InkExternalFunctions();
        inkExternalFunctions.Bind(story);
        inkDialogueVariables = new InkDialogueVariables(story);
    }

    private void OnDestroy() 
    {
        inkExternalFunctions.Unbind(story);
    }

    private void OnEnable() 
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue += EnterDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed += SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex += UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable += UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange += QuestStateChange;
    }

    private void OnDisable() 
    {
        GameEventsManager.instance.dialogueEvents.onEnterDialogue -= EnterDialogue;
        GameEventsManager.instance.inputEvents.onSubmitPressed -= SubmitPressed;
        GameEventsManager.instance.dialogueEvents.onUpdateChoiceIndex -= UpdateChoiceIndex;
        GameEventsManager.instance.dialogueEvents.onUpdateInkDialogueVariable -= UpdateInkDialogueVariable;
        GameEventsManager.instance.questEvents.onQuestStateChange -= QuestStateChange;
    }

    private void QuestStateChange(Quest quest) 
    {
        GameEventsManager.instance.dialogueEvents.UpdateInkDialogueVariable(
            quest.info.id + "State",
            new StringValue(quest.state.ToString())
        );
    }

    private void UpdateInkDialogueVariable(string name, Ink.Runtime.Object value) 
    {
        inkDialogueVariables.UpdateVariableState(name, value);
    }

    private void UpdateChoiceIndex(int choiceIndex) 
    {
        this.currentChoiceIndex = choiceIndex;
    }

    private void SubmitPressed(InputEventContext inputEventContext) 
    {
        // if the context isn't dialogue, we never want to register input here
        if (!inputEventContext.Equals(InputEventContext.DIALOGUE)) 
        {
            return;
        }

        ContinueOrExitStory();
    }

    private void EnterDialogue(string knotName) 
    {
        // don't enter dialogue if we've already entered
        if (dialoguePlaying) 
        {
            return;
        }

        dialoguePlaying = true;

        // inform other parts of our system that we've started dialogue
        GameEventsManager.instance.dialogueEvents.DialogueStarted();

        // freeze player movement
        GameEventsManager.instance.playerEvents.DisablePlayerMovement();

        // input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DIALOGUE);
        
        // jump to the knot
        if (!knotName.Equals(""))
        {
            story.ChoosePathString(knotName);
        }
        else 
        {
            Debug.LogWarning("Knot name was the empty string when entering dialogue.");
        }

        // start listening for variables
        inkDialogueVariables.SyncVariablesAndStartListening(story);

        // kick off the story
        ContinueOrExitStory();
    }

    private void ContinueOrExitStory() 
    {
        // make a choice, if applicable
        if (story.currentChoices.Count > 0 && currentChoiceIndex != -1)
        {
            story.ChooseChoiceIndex(currentChoiceIndex);
            // reset choice index for next time
            currentChoiceIndex = -1;
        }

        if (story.canContinue)
        {
            string dialogueLine = story.Continue();

            // handle the case where there's an empty line of dialogue
            // by continuing until we get a line with content
            while (IsLineBlank(dialogueLine) && story.canContinue) 
            {
                dialogueLine = story.Continue();
            }
            // handle the case where the last line of dialogue is blank
            // (empty choice, external function, etc...)
            if (IsLineBlank(dialogueLine) && !story.canContinue) 
            {
                ExitDialogue();
            }
            else 
            {
                GameEventsManager.instance.dialogueEvents.DisplayDialogue(dialogueLine, story.currentChoices);
            }
        }
        else if (story.currentChoices.Count == 0)
        {
            ExitDialogue();
        }
    }

    private void ExitDialogue()
    {
        dialoguePlaying = false;

        // inform other parts of our system that we've finished dialogue
        GameEventsManager.instance.dialogueEvents.DialogueFinished();

        // let player move again
        GameEventsManager.instance.playerEvents.EnablePlayerMovement();

        // input event context
        GameEventsManager.instance.inputEvents.ChangeInputEventContext(InputEventContext.DEFAULT);

        // stop listening for dialogue variables
        inkDialogueVariables.StopListening(story);

        // reset story state
        story.ResetState();
    }

    private bool IsLineBlank(string dialogueLine)
    {
        return dialogueLine.Trim().Equals("") || dialogueLine.Trim().Equals("\n");
    }
}
