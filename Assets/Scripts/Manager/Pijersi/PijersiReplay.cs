using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    public void OnEnterReplay()
    {
        if (save.turns[0].actions.Count > 0)
            UI.replayButtons["Back"].interactable = true;

        int lastSaveTurnId = save.turns.Count - 1;
        int lastReplaySaveTurnId = replaySave.turns.Count - 1;
        if (lastSaveTurnId > lastReplaySaveTurnId || save.turns[lastSaveTurnId].actions.Count > replaySave.turns[lastSaveTurnId].actions.Count)
            return;
        UI.replayButtons["Next"].interactable = true;
    }

    public void OnExitReplay()
    {
        UI.replayButtons["Back"].interactable = false;
        UI.replayButtons["Next"].interactable = false;
    }

    public void OnUpdateReplay()
    {
        if (replayState != ReplayState.Play) return;

        SM.ChangeState(State.Next);
    }
}
