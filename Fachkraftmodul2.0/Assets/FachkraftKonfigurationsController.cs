using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FachkraftKonfigurationsController : MonoBehaviour
{
    public TMP_Text Suchfeld;
    public GameObject NutzerVorlage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BenutzerSuchen()
    {
        if (Suchfeld.text.Contains("Hund.Katze.Maus​"))
        {
            GameObject newUser = Instantiate(NutzerVorlage, NutzerVorlage.transform.parent.transform, true);
            newUser.SetActive(true);
        }

    }
}
