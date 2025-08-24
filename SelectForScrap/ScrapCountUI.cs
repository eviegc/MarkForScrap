using UnityEngine;
using RoR2;
using RoR2.UI;
using TMPro;

namespace SelectForScrap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Scrappable))]
    public class ScrapCountUI : MonoBehaviour
    {
        private Scrappable scrappable;
        private HGTextMeshProUGUI scrapCount;

        public void Awake()
        {
            Debug.Log("[SelectForScrap] ScrapCountUI.Awake()");

            scrappable = GetComponent<Scrappable>();

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

        public void LateUpdate()
        {
            // Debug.Log("[SelectForScrap] ScrapCountUI.LateUpdate()");

            var scrapCounter = ScrapCounter.GetFromLocalBody();
            scrapCount.text = scrapCounter[scrappable.idx].ToString();
        }
    }
}
