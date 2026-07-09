using System.Collections.Generic;
using UnityEngine;

public class PerformEffectGA : GameAction
{
    public Effect Effect { get; set; }
    public List<CombatantView> Targets { get; set; }
    public PerformEffectGA(Effect effect, List<CombatantView> targets)
    // public PerformEffectGA(Effect effect)
    {
        Effect = effect;
        Targets = targets == null ? null : new(targets);
    }
}
