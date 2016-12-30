using UnityEngine;
using System.Collections;

public class ParticleControl : MonoBehaviour {

    public ParticleSystem[] _particleSystem;
    public Light[] _light;

    public void ActiveObj(bool value)
    {
        Debug.Log("ActiveObj: " + gameObject.name);
        for (int i = 0; i < _particleSystem.Length; i++)
        {
            if(_particleSystem[i].gameObject.activeSelf != value)
                _particleSystem[i].gameObject.SetActive(value);
            if (value)
            {
                _particleSystem[i].Stop();
                _particleSystem[i].Play();
            }
        }

        for (int i = 0; i < _light.Length; i++)
        {
            _particleSystem[i].gameObject.SetActive(value);
            
        }
    }
}
