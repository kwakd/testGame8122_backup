using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class PermanentUI : MonoBehaviour
{
    //Player Stats
    public int             apples    = 0;
    public int             health;
    
    public TextMeshProUGUI appleText;

    public Text           healthAmount;

    public static PermanentUI perm;

    private void Start()
    {

        DontDestroyOnLoad(gameObject);

        //singleton
        if(!perm)
        {
            perm = this;
        }
        else
        {
            Destroy(gameObject);    
        }
    }

    public void Reset()
    {
        apples = 0;
        appleText.text = apples.ToString();
    }

}
