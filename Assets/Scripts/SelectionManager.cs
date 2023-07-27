using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InventoryManager;

//Manages what happens when a slot is hovered over/selected/etc
public class SelectionManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public delegate void ItemDrag(PointerEventData eventData); //Possibly not needed
    public static event ItemDrag onItemDrag;

    public delegate void ItemDrop(PointerEventData eventData);
    public static event ItemDrop onItemDrop;

    private Canvas canvas;

    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float moveTime = 0.1f;
    private RectTransform imageRectTransform;
    private Vector2 itemOriginalPosition;

    private Vector3 startScale;
    // Start is called before the first frame update
    void Start() {
        startScale = transform.localScale;

        //Assumes image sprite is first child
        imageRectTransform = transform.GetChild(0).GetComponent<RectTransform>();

        canvas = transform.parent.parent.parent.parent.GetComponent<Canvas>(); //???
    }

    private IEnumerator MoveCard(bool selected)
    {
        Vector3 endScale;

        float timeElapsed = 0f;
        while (timeElapsed < moveTime) {
            timeElapsed += Time.deltaTime;

            if (selected)
            {
                endScale = startScale * scaleMultiplier;
            }
            else {
                endScale = startScale;
            }

            //Lerp to scale
            Vector3 lerpedScale = Vector3.Lerp(transform.localScale, endScale, timeElapsed / moveTime);

            transform.localScale = lerpedScale;

            yield return null;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(MoveCard(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(MoveCard(false));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        itemOriginalPosition = imageRectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!transform.GetChild(0).GetComponent<Image>().IsActive()) return;
        //Keeps image on cursor
        //Currently, scrolling while dragging makes the dragged item scroll up/down as well, when it shouldn't
        imageRectTransform.anchoredPosition += (eventData.delta / canvas.scaleFactor);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        imageRectTransform.anchoredPosition = itemOriginalPosition;
        Debug.Log("End Drag");
    }

    public void OnDrop(PointerEventData eventData)
    {
        onItemDrop?.Invoke(eventData);
    }
}
