using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class ZoneBar : MonoBehaviour
    {
        [SerializeField] private RectTransform _indicator;
        [SerializeField, HideInInspector] private Image _indicatorImage;
        [SerializeField] private RectTransform _cells;
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _safeSprite;
        [SerializeField] private Sprite _superSprite;
        [SerializeField] private Color _normalColor = Color.white;
        [SerializeField] private Color _safeColor = new Color(0.46f, 0.89f, 0.17f);
        [SerializeField] private Color _superColor = new Color(1f, 0.78f, 0.16f);
        [SerializeField, Range(0f, 1f)] private float _passedAlpha = 0.3f;
        [SerializeField] private float _currentScale = 1.25f;
        [SerializeField] private float _slideDuration = 0.4f;
        [SerializeField] private float _rollDuration = 0.5f;
        [SerializeField] private float _rollShift = 140f;

        private CanvasGroup _cellsGroup;
        private CanvasGroup _indicatorGroup;
        private float _baseX;
        private int _lastStart = -1;

        private void Awake()
        {
            if (_cells != null)
            {
                _baseX = _cells.anchoredPosition.x;
                _cellsGroup = Group(_cells.gameObject);
            }
            if (_indicator != null) _indicatorGroup = Group(_indicator.gameObject);
        }

        public void SetZone(int zone)
        {
            if (_cells == null || _cells.childCount == 0) return;

            int window = _cells.childCount;
            int start = ((zone - 1) / window) * window + 1;

            bool rolling = _lastStart != -1 && start != _lastStart;
            bool forward = start > _lastStart;
            _lastStart = start;

            if (rolling)
            {
                RollTo(start, zone, forward);
            }
            else
            {
                ApplyWindow(start, zone);
                ApplyHighlight(start, zone, false);
            }
        }

        private void RollTo(int start, int zone, bool forward)
        {
            float dir = forward ? 1f : -1f;
            float outX = _baseX - _rollShift * dir;
            float inX = _baseX + _rollShift * dir;
            float half = _rollDuration * 0.5f;

            _cells.DOKill();
            _cellsGroup.DOKill();
            if (_indicatorGroup != null) { _indicator.DOKill(); _indicatorGroup.DOKill(); }

            Sequence seq = DOTween.Sequence();
            seq.Append(_cells.DOAnchorPosX(outX, half).SetEase(Ease.InQuad));
            seq.Join(_cellsGroup.DOFade(0f, half));
            if (_indicatorGroup != null) seq.Join(_indicatorGroup.DOFade(0f, half));
            seq.AppendCallback(() =>
            {
                ApplyWindow(start, zone);
                _cells.anchoredPosition = new Vector2(inX, _cells.anchoredPosition.y);
            });
            seq.Append(_cells.DOAnchorPosX(_baseX, half).SetEase(Ease.OutQuad));
            seq.Join(_cellsGroup.DOFade(1f, half));
            seq.AppendCallback(() => ApplyHighlight(start, zone, true));
            if (_indicatorGroup != null) seq.Append(_indicatorGroup.DOFade(1f, 0.15f));
        }

        private void ApplyWindow(int start, int zone)
        {
            int window = _cells.childCount;
            for (int i = 0; i < window; i++)
            {
                int number = start + i;
                ZoneType type = Zones.Classify(number);
                RectTransform cell = _cells.GetChild(i) as RectTransform;
                if (cell == null) continue;

                TMP_Text text = cell.GetComponentInChildren<TMP_Text>(true);
                if (text != null)
                {
                    text.text = number.ToString();
                    Color c = ColorFor(type);
                    if (number < zone) c.a = _passedAlpha;
                    text.color = c;
                }
                cell.localScale = (number == zone) ? Vector3.one * _currentScale : Vector3.one;
            }
        }

        private void ApplyHighlight(int start, int zone, bool instant)
        {
            if (_indicator != null)
            {
                int idx = zone - start;
                if (idx >= 0 && idx < _cells.childCount)
                {
                    RectTransform current = _cells.GetChild(idx) as RectTransform;
                    if (current != null)
                    {
                        Canvas.ForceUpdateCanvases();
                        _indicator.DOKill();
                        if (instant)
                            _indicator.position = new Vector3(current.position.x, _indicator.position.y, _indicator.position.z);
                        else
                            _indicator.DOMoveX(current.position.x, _slideDuration).SetEase(Ease.OutCubic);
                    }
                }
            }

            if (_indicatorImage != null)
            {
                Sprite s = SpriteFor(Zones.Classify(zone));
                if (s != null) _indicatorImage.sprite = s;
            }
        }

        private CanvasGroup Group(GameObject go)
        {
            CanvasGroup g = go.GetComponent<CanvasGroup>();
            return g != null ? g : go.AddComponent<CanvasGroup>();
        }

        private Color ColorFor(ZoneType type)
        {
            switch (type)
            {
                case ZoneType.Safe: return _safeColor;
                case ZoneType.Super: return _superColor;
                default: return _normalColor;
            }
        }

        private Sprite SpriteFor(ZoneType type)
        {
            switch (type)
            {
                case ZoneType.Safe: return _safeSprite;
                case ZoneType.Super: return _superSprite;
                default: return _normalSprite;
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_indicator == null)
            {
                Transform t = transform.Find("ui_image_zone_current_value");
                if (t != null) _indicator = t as RectTransform;
            }
            if (_indicatorImage == null && _indicator != null)
                _indicatorImage = _indicator.GetComponent<Image>();
            if (_cells == null)
            {
                Transform t = transform.Find("ui_zonebar_cells");
                if (t != null) _cells = t as RectTransform;
            }
        }
#endif
    }
}
