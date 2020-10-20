using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler
{
    private Image knobBackground;
    private Image knob;

    private Vector3 inputVector;

    public Vector2 InputVector
    {
        get
        {
            convert.x = inputVector.x;
            convert.y = inputVector.z;
            return convert;
        }
    }
    private Vector2 convert = new Vector2();

    void Start()
    {
        knobBackground = GetComponent<Image>();
        knob = transform.GetChild(0).GetComponent<Image>();
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 pos;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(knobBackground.rectTransform,
                                                                    eventData.position,
                                                                    eventData.pressEventCamera,
                                                                    out pos))
        {

            pos.x = (pos.x / knobBackground.rectTransform.sizeDelta.x);
            pos.y = (pos.y / knobBackground.rectTransform.sizeDelta.y);

            inputVector = new Vector3(pos.x * 2 - 1, 0, pos.y * 2 - 1);
            inputVector = (inputVector.magnitude > 1.0f) ? inputVector.normalized : inputVector;

            knob.rectTransform.anchoredPosition = new Vector3(inputVector.x * (knobBackground.rectTransform.sizeDelta.x * .4f),
                                                                     inputVector.z * (knobBackground.rectTransform.sizeDelta.y * .4f));
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        inputVector = Vector3.zero;
        knob.rectTransform.anchoredPosition = Vector3.zero;
    }


}
