using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserProfilePrefab : MonoBehaviour
{

    public TMPro.TMP_Text UserName;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayUserData(){
        UserName.text = AppState.CurrentUser.Mnemonic_token;
    }

}
