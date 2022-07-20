using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Constants
{
    public static readonly int JOINTCOUNT = 17;
    public static readonly int TARGETFRAME = 30;
    public static readonly int SCALEFACTOR = 1000;

    public enum TargetPositionIndex : int
    {
        Cha_Hips = 0,
        Cha_Spine,
        Cha_Chest,
        Cha_Cape_0,
        Cha_Cape_1,
        Cha_Cape_2,

        Cha_UpperArmL,
        Cha_LowerArmL,
        Cha_HandL,
        BodyHandL,

        Cha_UpperArmR,
        Cha_LowerArmR,
        Cha_HandR,
        BodyHandR,

        Cha_Wing_L,
        Cha_Wing_R,

        Face,
        Cha_Antenna_L,
        Cha_Antenna_R,
        Cha_HeadDummy,

        Cha_UpperLegL,
        Cha_LowerLegL,
        Cha_FootL,
        Cha_ToeL,

        Cha_UpperLegR,
        Cha_LowerLegR,
        Cha_FootR,
        Cha_ToeR,

        Count,
        None,
    }

    public enum SourcePositionIndex : int
    {
        bottom_torso = 0,

        right_hip,
        right_knee,
        right_foot,

        left_hip,
        left_knee,
        left_foot,

        center_torso,
        upper_torso,
        neck_base,
        center_head,

        left_shoulder,
        left_elbow,
        left_hand,

        right_shoulder,
        right_elbow,
        right_hand,

        Count,
        None,
    }
}
