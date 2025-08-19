using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O_TextHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public string OnPhaseChanged(int phase)
    {
        string text = "Suitable text not found.";
        switch (phase)
        {
            case 0:
                text = "05.00 - Kulnevin ratsuv‰ki saapuu. Etuvartiotaistelu alkaa.";
                break;
            case 1:
                text = "09.00 - Kulnevin jalkav‰ki liittyy taisteluun.";
                break;
            case 2:
                text = "09.30 - H‰lsingen rykmentti puolustaa.";
                break;
            case 3:
                text = "10.00 - J‰‰k‰rit saapuvat tukemaan puolustusta.";
                break;
            case 4:
                text = "10.30 - Kamppailu kuluttaa molempia osapuolia.";
                break;
            case 5:
                text = "11.00 - Ruotsi vet‰ytyy. Von Schwerinin tykkipatteri tukee irtautumista.";
                break;
            case 6:
                text = "11.30 - Uudet puolustusasemat. Karjalan j‰‰k‰rit saapuvat.";
                break;
        }
        return text;            
    }

}
