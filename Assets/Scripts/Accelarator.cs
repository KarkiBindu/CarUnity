using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Accelarator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public static bool Accelerate;    
   

    // Start is called before the first frame update
    void Start()
    {
        Accelerate = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Executed when mouse/finger starts touching the steering wheel
        Accelerate = true;
        transform.rotation = Quaternion.Euler(0, 15, 0);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Accelerate = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }
   
   
}
