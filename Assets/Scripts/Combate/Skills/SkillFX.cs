using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class SkillFX
{
    [Header("Single cell")] 
    public AnimatorOverrideController singleCellFX;
   
    [Header("Many cell")] 
    public AnimatorOverrideController manyCellFX_midway;
    public AnimatorOverrideController manyCellFX_nearTarget;
    public AnimatorOverrideController manyCellFX_nearCaster;

    [Header("Entity")] 
    public AnimatorOverrideController entityCasterFX;
    public AnimatorOverrideController entityTargetFX;

    public bool hasSingleCellFX => singleCellFX is not null;

    public bool hasManyCellFX => manyCellFX_nearCaster is not null
                                 && manyCellFX_nearTarget is not null
                                 && manyCellFX_midway;
    public bool hasCasterFX => entityCasterFX is not null;
    public bool hasTargetFX => entityTargetFX is not null;
}
