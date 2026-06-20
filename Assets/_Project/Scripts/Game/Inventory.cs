using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FortuneWheel
{
    [DisallowMultipleComponent]
    public class Inventory : MonoBehaviour
    {
        [SerializeField] private WheelView _wheel;
        [SerializeField] private GameObject _rowPrefab;
        [SerializeField] private RectTransform _content;
        [SerializeField] private Canvas _canvas;
        [SerializeField] private float _popScale = 6f;
        [SerializeField] private float _popDuration = 0.3f;
        [SerializeField] private float _holdDuration = 0.3f;
        [SerializeField] private float _flyDuration = 0.5f;

        private readonly Dictionary<RewardData, int> _haul = new Dictionary<RewardData, int>();
        private readonly Dictionary<RewardData, RectTransform> _rows = new Dictionary<RewardData, RectTransform>();

        private void OnEnable()
        {
            if (_wheel != null) _wheel.Landed += OnLanded;
        }

        private void OnDisable()
        {
            if (_wheel != null) _wheel.Landed -= OnLanded;
        }

        private void OnLanded(WheelSlice slice, WheelSlotView slot)
        {
            if (slice.IsBomb || slice.Reward == null) return;

            RewardData reward = slice.Reward;
            _haul.TryGetValue(reward, out int oldTotal);
            int newTotal = oldTotal + slice.Amount;
            _haul[reward] = newTotal;

            bool isNewRow = !_rows.ContainsKey(reward);
            RectTransform row = GetOrCreateRow(reward);

            if (_canvas == null || slot == null || row == null)
            {
                SetRowAmount(row, newTotal);
                return;
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_content);

            CanvasGroup rowGroup = null;
            if (isNewRow)
            {
                rowGroup = row.GetComponent<CanvasGroup>();
                if (rowGroup == null) rowGroup = row.gameObject.AddComponent<CanvasGroup>();
                rowGroup.alpha = 0f;
            }

            Vector3 destination = row.position;

            Transform visual = slot.IconTransform.parent;
            GameObject ghostGo = Instantiate(visual.gameObject, visual.parent);
            ghostGo.transform.SetParent(_canvas.transform, true);

            foreach (Graphic g in ghostGo.GetComponentsInChildren<Graphic>(true))
                g.raycastTarget = false;

            RectTransform ghost = ghostGo.GetComponent<RectTransform>();
            Vector3 baseScale = ghost.localScale;

            Sequence seq = DOTween.Sequence();
            seq.Append(ghost.DOScale(baseScale * _popScale, _popDuration).SetEase(Ease.OutBack));
            seq.AppendInterval(_holdDuration);
            seq.Append(ghost.DOMove(destination, _flyDuration).SetEase(Ease.InQuad));
            seq.Join(ghost.DOScale(baseScale * 0.6f, _flyDuration));
            seq.OnComplete(() =>
            {
                Destroy(ghostGo);
                SetRowAmount(row, newTotal);
                if (rowGroup != null) rowGroup.alpha = 1f;
            });
        }

        private RectTransform GetOrCreateRow(RewardData reward)
        {
            if (!_rows.TryGetValue(reward, out RectTransform row))
            {
                GameObject go = Instantiate(_rowPrefab, _content);
                row = go.GetComponent<RectTransform>();
                Image icon = go.GetComponentInChildren<Image>();
                if (icon != null) icon.sprite = reward.Icon;
                _rows[reward] = row;
            }

            return row;
        }

        private static void SetRowAmount(RectTransform row, int total)
        {
            TMP_Text amount = row.GetComponentInChildren<TMP_Text>();
            if (amount != null) amount.text = $"x{total}";
        }

        public void CollectInto(CurrencyHud hud)
        {
            if (hud != null)
            {
                foreach (KeyValuePair<RewardData, int> entry in _haul)
                    if (entry.Key != null && entry.Key.Kind != RewardKind.Item)
                        hud.Add(entry.Key.Kind, entry.Value);
            }

            Clear();
        }

        public void Clear()
        {
            foreach (RectTransform row in _rows.Values)
                if (row != null) Destroy(row.gameObject);
            _rows.Clear();
            _haul.Clear();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_content == null) _content = transform as RectTransform;
            if (_wheel == null) _wheel = FindObjectOfType<WheelView>();
            if (_canvas == null) _canvas = GetComponentInParent<Canvas>();
        }
#endif
    }
}
