using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState
{
    public abstract void UpdateState(BossControl boss);
    public abstract void EnterState(BossControl boss);
    public abstract void ExitState(BossControl boss);

}
