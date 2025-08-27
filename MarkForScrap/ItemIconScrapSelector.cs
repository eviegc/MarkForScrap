using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MarkForScrap
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ItemIcon))]
    public class ItemIconScrapSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private ItemIcon icon;
        private static Sprite markSprite;
        private Image image;
        private Image imageShadow;
        private bool isHover = false;

        public ItemIndex idx
        {
            get { return icon.itemIndex; }
        }

        public void Awake()
        {
            // MarkForScrapPlugin.Log.LogDebug("Scrappable.Awake()");

            icon = GetComponent<ItemIcon>();

            InitUI();
        }

        public void Update()
        {
            if (!isHover)
                return; // Mouse isn't hovering over
            if (!Utils.PatchedKeyboardShortcut.IsDown(PluginConfig.ToggleScrapKey.Value))
                return; // Scrap key isn't pressed
            if (!Utils.ItemUtils.IsScrappable(idx))
                return; // Item isn't scrappable

            // MarkForScrapPlugin.Log.LogDebug("Scrappable.Update()");

            var scrapCounter = Utils.LocalUser.scrapCounter;
            if (!scrapCounter)
                return;

            scrapCounter.FlipMark(idx);

            MarkForScrapPlugin.Log.LogDebug(
                $"Scrappable.Update() | Item: {idx}, IsMarked: {scrapCounter.IsMarked(idx)}"
            );
        }

        public void LateUpdate()
        {
            // MarkForScrapPlugin.Log.LogDebug("Scrappable.LateUpdate()");

            var scrapCounter = Utils.LocalUser.scrapCounter;
            bool isMarked = scrapCounter ? scrapCounter.IsMarked(idx) : false;
            image.enabled = isMarked;
            imageShadow.enabled = isMarked;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHover = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHover = false;
        }

        public void EnsureSprite()
        {
            if (markSprite)
                return;
            markSprite = Resources.Assets.Load<Sprite>("Assets/icon-scrapper.png");
        }

        public void InitUI()
        {
            // MarkForScrapPlugin.Log.LogDebug("Scrappable.InitUI()");

            EnsureSprite();

            GameObject overlay = new GameObject("ScrapOverlay");
            overlay.transform.SetParent(transform, false);

            RectTransform overlayRT = overlay.AddComponent<RectTransform>();
            overlayRT.anchorMin = new Vector2(0.05f, 0.05f);
            overlayRT.anchorMax = new Vector2(0.95f, 0.95f);
            overlayRT.offsetMin = Vector2.zero;
            overlayRT.offsetMax = Vector2.zero;

            GameObject imageShadowOverlay = new GameObject("imageShadowOverlay");
            imageShadowOverlay.transform.SetParent(overlay.transform, false);

            RectTransform imageShadowOverlayRT = imageShadowOverlay.AddComponent<RectTransform>();
            imageShadowOverlayRT.anchorMin = Vector2.zero;
            imageShadowOverlayRT.anchorMax = Vector2.one;
            imageShadowOverlayRT.offsetMin = Vector2.zero;
            imageShadowOverlayRT.offsetMax = Vector2.zero;
            imageShadowOverlayRT.localPosition = new Vector3(2.0f, -1.0f - 8.0f, 0.0f);

            imageShadow = imageShadowOverlay.AddComponent<Image>();
            imageShadow.sprite = markSprite;
            imageShadow.color = new Color(0.0f, 0.0f, 0.0f, 1.0f);

            GameObject imageOverlay = new GameObject("imageOverlay");
            imageOverlay.transform.SetParent(overlay.transform, false);

            RectTransform imageOverlayRT = imageOverlay.AddComponent<RectTransform>();
            imageOverlayRT.anchorMin = Vector2.zero;
            imageOverlayRT.anchorMax = Vector2.one;
            imageOverlayRT.offsetMin = Vector2.zero;
            imageOverlayRT.offsetMax = Vector2.zero;
            imageOverlayRT.localPosition = new Vector3(0.0f, -8.0f, 0.1f);

            image = imageOverlay.AddComponent<Image>();
            image.sprite = markSprite;

            imageShadow.enabled = false;
            image.enabled = false;
        }
    }
}
