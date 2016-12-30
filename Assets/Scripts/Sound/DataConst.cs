using UnityEngine;
using System.Collections;

public class IDConst
{
    public int ID;
    public string NAME;
    public int TYPE;
    public int FX;

    public IDConst(int id, string name, int type, int fx)
    {
        ID = id;
        NAME = name;
        TYPE = type;
        FX = fx;
    }
}

public class DataConst{

    public static readonly IDConst ID_SOUND_INGAME_BG_MUSIC = new IDConst(0, "Sounds/Jazz_In_Paris", -1, -1);
    public static readonly IDConst ID_SOUND_INGAME_BTNCLICK = new IDConst(1, "Sounds/052-Cannon01", -1, -1);
    public const int ID_SOUNDS_TOTAL = 2;

    public static readonly IDConst[] ID_SOUNDS_LOADING = {
        ID_SOUND_INGAME_BG_MUSIC
        ,ID_SOUND_INGAME_BTNCLICK
	};
}
