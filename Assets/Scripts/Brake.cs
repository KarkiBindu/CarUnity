using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class Brake : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static bool Braking;
    private Image _image;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        // Executed when mouse/finger starts touching the steering wheel
        Braking = true;
        _image.transform.rotation = Quaternion.Euler(0, 15, 0); 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Braking = false;
        _image.transform.rotation = Quaternion.Euler(0, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
