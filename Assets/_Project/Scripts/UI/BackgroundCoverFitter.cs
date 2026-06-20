using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Image))]
[ExecuteAlways]
[DisallowMultipleComponent]
public class BackgroundCoverFitter : MonoBehaviour
{
    private RectTransform _rect;
    private Image _image;

    private void OnEnable()
    {
        Cache();
        Fit();
    }

    private void OnRectTransformDimensionsChange()
    {
        Fit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        Cache();
        UnityEditor.EditorApplication.delayCall += () =>
        {
            if (this != null) Fit();
        };
    }
#endif

    private void Cache()
    {
        if (_rect == null) _rect = GetComponent<RectTransform>();
        if (_image == null) _image = GetComponent<Image>();
    }

    private void Fit()
    {
        if (_rect == null || _image == null || _image.sprite == null) return;

        RectTransform parent = _rect.parent as RectTransform;
        if (parent == null) return;

        Vector2 area = parent.rect.size;
        if (area.x <= 0f || area.y <= 0f) return;

        Vector2 spriteSize = _image.sprite.rect.size;
        if (spriteSize.x <= 0f || spriteSize.y <= 0f) return;

        float scale = Mathf.Max(area.x / spriteSize.x, area.y / spriteSize.y);

        _rect.anchorMin = new Vector2(0.5f, 0.5f);
        _rect.anchorMax = new Vector2(0.5f, 0.5f);
        _rect.pivot = new Vector2(0.5f, 0.5f);
        _rect.anchoredPosition = Vector2.zero;
        _rect.sizeDelta = spriteSize * scale;
    }
}