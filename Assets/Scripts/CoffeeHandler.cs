//
// Not much to see here. This component just enables dapper individuals to donate dollars at their discretion.
//

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoffeeHandler : MonoBehaviour
{
    public void OnClick()
    {
        Application.OpenURL("http://pouchmouse.com/coffee_page/");
    }
}
