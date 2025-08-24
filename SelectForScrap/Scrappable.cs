using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

namespace SelectForScrap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ItemIcon))]
    public class Scrappable : MonoBehaviour, IPointerClickHandler
    {
        private HGTextMeshProUGUI scrapCount;
        private ItemIcon icon;

        public ItemIndex idx
        {
            get { return icon.itemIndex; }
        }

        public void Awake()
        {
            Debug.Log("[SelectForScrap] Scrappable.Awake()");

            icon = GetComponent<ItemIcon>();

            InitUI();
        }

        public void LateUpdate()
        {
            // Debug.Log("[SelectForScrap] Scrappable.LateUpdate()");

            var scrapCounter = Utils.LocalUser.scrapCounter;
            scrapCount.text = scrapCounter ? scrapCounter[idx].ToString() : "";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("[SelectForScrap] Scrappable.OnPointerClick()");

            // TODO config? do we need a modifier key? how does it play with yeet
            // Only allow selection while holding shift
            if (!Input.GetKey(KeyCode.LeftShift)) return; 

            var scrapCounter = Utils.LocalUser.scrapCounter;
            if (!scrapCounter)
            {
                Debug.Log("[SelectForScrap] Scrappable.OnPointerClick() | No ScrapCounter Found");
                return;
            }

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

        public void InitUI()
        {
            Debug.Log("[SelectForScrap] Scrappable.InitUI()");

            var go = new GameObject("ScrapOverlay", typeof(RectTransform));
            go.transform.SetParent(transform, false);

            var rt = (RectTransform)go.transform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;

            scrapCount = go.AddComponent<HGTextMeshProUGUI>();
            scrapCount.raycastTarget = false;
            scrapCount.enableWordWrapping = false;
            scrapCount.alignment = TextAlignmentOptions.Center;
            scrapCount.fontSize = 18f;
            scrapCount.richText = false;
        }
    }
}
