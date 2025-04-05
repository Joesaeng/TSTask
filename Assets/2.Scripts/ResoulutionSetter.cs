using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResoulutionSetter : MonoBehaviour
{
    private void Awake()
    {
#if UNITY_STANDALONE
        Screen.SetResolution(540, 960, false);
        Screen.fullScreen = false;
#endif
    }
}
