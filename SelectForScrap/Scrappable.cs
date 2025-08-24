using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace SelectForScrap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ItemIcon))]
    public class Scrappable : MonoBehaviour, IPointerClickHandler
    {
        private ItemIcon icon;
        public ItemIndex idx;


        public void Awake()
        {
            Debug.Log("[SelectForScrap] Scrappable.Awake()");

            icon = GetComponent<ItemIcon>();
            idx = icon.itemIndex;
            icon.gameObject.AddComponent<ScrapCountUI>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("[SelectForScrap] Scrappable.OnPointerClick()");

            // TODO config? do we need a modifier key? how does it play with yeet
            if (!Input.GetKey(KeyCode.LeftShift)) return; // Only allow selection while holding shift

            var scrapCounter = ScrapCounter.GetFromLocalBody();

            var delta = 0;
            switch (eventData.button)
            {   
                case PointerEventData.InputButton.Left:
                    delta = 1;
                    break;
                case PointerEventData.InputButton.Middle:
                    delta = -scrapCounter[idx];
                    break;
                case PointerEventData.InputButton.Right:
                    delta = -1;
                    break;
            }

            scrapCounter[idx] += delta;

            Debug.Log($"[SelectForScrap] Scrappable.OnPointerClick() | Item: {idx}, ScrapCount: {scrapCounter[idx]}, Delta: {delta}");
        }
    }
}
