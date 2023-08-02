using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static InventoryManager;

//Manages what happens when a slot is hovered over/selected/dragged/dropped
public class SelectionManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void SlotSelected(GameObject selectedObject);
    public static event SlotSelected onSlotSelected;


    [SerializeField] private float scaleMultiplier = 1.1f;
    [SerializeField] private float moveTime = 0.1f;

    private Vector3 startScale;

    void Start() {
        startScale = transform.localScale;
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

    //For scaling an item slot up if hovered over
    public void OnPointerEnter(PointerEventData eventData)
    {
        
        StartCoroutine(MoveCard(true));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(MoveCard(false));
    }

    //Picking up the item
    public void OnSelect()
    {
        onSlotSelected?.Invoke(this.gameObject);
    }

    
}
