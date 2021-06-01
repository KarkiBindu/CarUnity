using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class ImageControl : MonoBehaviour, IPointerClickHandler, IDragHandler, IEndDragHandler
{
    private Image _image;   
    public RectTransform Steering;

    Vector2 _initialPosition;
    Vector2 _newPosition;

    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        ImageClick();
    }
    public void ImageClick()
    {
        _initialPosition = _image.transform.position;
        MonoBehaviour.print("Image clicked");
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _newPosition = Input.mousePosition;
        MonoBehaviour.print("Image dragged");          
        if (Mathf.Atan2(_initialPosition.y, _initialPosition.x) < Mathf.Atan2(_newPosition.y, _newPosition.x))
        {
            Steering.rotation = Quaternion.Euler(0, 0, -30);
        }
        else if (Mathf.Atan2(_initialPosition.y, _initialPosition.x) > Mathf.Atan2(_newPosition.y, _newPosition.x))
        {
            Steering.rotation = Quaternion.Euler(0, 0, 30);
        }
    }

    public float SignedAngleTo(Vector2 a, Vector2 b)
    {
        return Mathf.Atan2(a.x * b.y - a.y * b.x, a.x * b.x + a.y * b.y) * Mathf.Rad2Deg;
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        MonoBehaviour.print("Image released");
    }
}
