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
        private ItemIndex idx;

        private static Dictionary<ItemIndex, int> scrapCount = new Dictionary<ItemIndex, int>();

        public int ScrapCount
        {
            get
            {
                scrapCount.TryGetValue(idx, out var count);
                return Mathf.Clamp(count, 0, icon.itemCount);
            }
            set { scrapCount[idx] = Mathf.Clamp(value, 0, icon.itemCount); }
        }

        public void Awake()
        {
            Debug.Log("[SelectForScrap] Scrappable.Awake()");

            icon = GetComponent<ItemIcon>();
            idx = icon.itemIndex;
            icon.gameObject.AddComponent<InventoryItemScrapCount>();
        }

        public void OnDestroy()
        {
            // I *THINK* this will mean that we don't need to periodically clean up
            // the static dict, but I also think I'm being too smart for my own good
            scrapCount.Remove(idx);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("[SelectForScrap] Scrappable.OnPointerClick()");

            // TODO config? do we need a modifier key? how does it play with yeet
            if (!Input.GetKey(KeyCode.LeftShift)) return; // Only allow selection while holding shift

            var delta = 0;
            switch (eventData.button)
            {
                case PointerEventData.InputButton.Left:
                    delta = 1;
                    break;
                case PointerEventData.InputButton.Middle:
                    delta = -ScrapCount;
                    break;
                case PointerEventData.InputButton.Right:
                    delta = -1;
                    break;
            }

            ScrapCount += delta;

            Debug.Log($"[SelectForScrap] Scrappable.OnPointerClick() | Item: {idx}, scrapCount: {scrapCount}, ScrapCount: {ScrapCount}, Delta: {delta}");
        }
    }
}
