// external functions
EXTERNAL StartQuest(questId)
EXTERNAL AdvanceQuest(questId)
EXTERNAL FinishQuest(questId)

// quest ids (questId + "Id" for variable name)
VAR CollectCoinsQuestId = "CollectCoinsQuest"

// quest states (questId + "State" for variable name)
VAR CollectCoinsQuestState = "REQUIREMENTS_NOT_MET"

// ink files
INCLUDE collect_coins_start_npc.ink
INCLUDE collect_coins_finish_npc.ink