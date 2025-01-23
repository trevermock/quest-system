=== collectCoinsStart ===
{ CollectCoinsQuestState :
    - "REQUIREMENTS_NOT_MET": -> requirementsNotMet
    - "CAN_START": -> canStart
    - "IN_PROGRESS": -> inProgress
    - "CAN_FINISH": -> canFinish
    - "FINISHED": -> finished
    - else: -> END
}

= requirementsNotMet
// not possible for this quest, but putting something here anyways
Come back once you've leveled up a bit more.
-> END

= canStart
Will you collect 5 coins and bring them to my friend over there?
* [Yes]
    ~ StartQuest(CollectCoinsQuestId)
    Great!
* [No]
    Oh, ok then. Come back if you change your mind.
- -> END

= inProgress
How is collecting those coins going?
-> END

= canFinish
Oh? You collected the coins? Go give them to my friend over there and he'll give you a reward!
-> END

= finished
Thanks for collecting those coins!
-> END