using UnityEngine;
using System.Collections;

public class ResourceMng : MonoBehaviour {

    private AudioClip[] mySounds;

    public void LoadSounds()
    {
        if (DataConst.ID_SOUNDS_TOTAL == 0)
        {
            Debug.Log("<color=red>Error unload sound!</color>");
            return;
        }
        mySounds = new AudioClip[DataConst.ID_SOUNDS_TOTAL];
        for (int i = 0; i < DataConst.ID_SOUNDS_TOTAL; i++)
        {
            //Debug.Log("--Sound: " + DataConst.ID_SOUNDS_LOADING[i].NAME);
            mySounds[DataConst.ID_SOUNDS_LOADING[i].ID] = Resources.Load<AudioClip>(DataConst.ID_SOUNDS_LOADING[i].NAME);
        }
    }

    public AudioClip[] getMySound()
    {
        return mySounds;
    }
}
