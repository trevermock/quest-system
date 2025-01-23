=== collectCoinsFinish ===
{ CollectCoinsQuestState:
    - "FINISHED": -> finished
    - else: -> default
}

= finished
Thank you!
-> END

= default
Hm? What do you want?
* [Nothing, I guess.]
    -> END
* { CollectCoinsQuestState == "CAN_FINISH" } [Here are some coins.]
    ~ FinishQuest(CollectCoinsQuestId)
    Oh? These coins are for me? Thank you!
-> END