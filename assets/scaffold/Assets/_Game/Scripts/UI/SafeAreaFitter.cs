// SafeAreaFitter.cs
// Put one of these on a "SafeArea" child under every screen-space canvas and author all HUD inside it
// (SOP section 11). Handles all four insets and reapplies on rotation, resize and rect change.
// Why: a settings button once sat almost entirely inside the Dynamic Island because top insets were
// handled nowhere and the offsets were hand-tuned numbers that happened to land near the boundary.
using UnityEngine;

namespace __NAMESPACE__
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public sealed class SafeAreaFitter : MonoBehaviour
    {
        Rect lastSafeArea;
        Vector2Int lastScreen;
        ScreenOrientation lastOrientation;

        void OnEnable() { Apply(); }
        void OnRectTransformDimensionsChange() { Apply(); }

        void Update()
        {
            if (Screen.safeArea != lastSafeArea || Screen.orientation != lastOrientation ||
                Screen.width != lastScreen.x || Screen.height != lastScreen.y) Apply();
        }

        void Apply()
        {
            if (Screen.width <= 0 || Screen.height <= 0) return;
            var rt = (RectTransform)transform;
            Rect sa = Screen.safeArea;
            lastSafeArea = sa;
            lastOrientation = Screen.orientation;
            lastScreen = new Vector2Int(Screen.width, Screen.height);
            Vector2 min = sa.position;
            Vector2 max = sa.position + sa.size;
            min.x /= Screen.width; min.y /= Screen.height;
            max.x /= Screen.width; max.y /= Screen.height;
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }
    }
}
