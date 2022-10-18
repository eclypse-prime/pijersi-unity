using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Pijersi
{
    public void OnEnterReplay()
    {
        if (save.turns[0].actions.Count > 0)
            UI.replayButtons["Back"].interactable = true;
    }

    public void OnExitReplay() { }

    public void OnUpdateReplay()
    {
        if (replayState != ReplayState.Play) return;

        SM.ChangeState(State.Next);
    }
}
