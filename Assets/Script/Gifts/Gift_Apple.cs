using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gift_Apple : GiftBase
{
    public override void Acquire()
    {
        base.Acquire();
        GameManager.Inst.player.mhp += 10;
    }
}
