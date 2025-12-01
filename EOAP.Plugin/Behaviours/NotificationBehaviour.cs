using DG.Tweening;
using EOAP.Plugin.AP;
using Il2CppInterop.Runtime;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EOAP.Plugin.Behaviours
{
    public class NotificationBehaviour : MonoBehaviour
    {
        public int Slot { get; internal set; }
        public bool IsComplete { get; internal set; }
        public Text Text { get; private set; }

        private float _Clock;

        private RectTransform _tr;
        private Vector2 _targetPosition;
        private Vector2 _startPosition;
        private bool _FadeOut;
        public NotificationBehaviour(IntPtr ptr) : base(ptr)
        {

        }

        private void Awake()
        {
            _tr = GetComponent<RectTransform>();
            _tr.anchoredPosition = new Vector2(-10f, -10f);
            _targetPosition = _tr.localPosition;
            gameObject.SetActive(false);
        }


        public void SetNotification(APUI.Notification notification)
        {
            Text.text = notification.Text;
        }

        public void ShowNotification(int notificationIndex = 0, float duration = 1f)
        {
            _tr.SetAsLastSibling();
            gameObject.SetActive(true);
            _tr.anchoredPosition = new Vector2(_tr.sizeDelta.x + 10f, -10f - _tr.sizeDelta.y * notificationIndex);
            _startPosition = _tr.localPosition;
            Sequence seq = DOTween.Sequence();
            Vector2 targetPosition = _targetPosition - notificationIndex * Vector2.up * _tr.sizeDelta.y;
            seq.Append(_tr.DOLocalMove(targetPosition, 0.33f).SetEase(Ease.OutCubic));
            IsComplete = false;
            _Clock = duration;
            _FadeOut = false;
            Slot = notificationIndex;
        }
        private void Update()
        {
            if (_Clock > 0f)
            {
                _Clock -= Time.deltaTime;
                if (_Clock <= 0f)
                {
                    if (!_FadeOut)
                    {
                        _Clock = 0.5f;
                        _FadeOut = true;
                    }
                    else
                    {
                        _tr.DOLocalMove(_startPosition, 0.5f).SetEase(Ease.InCubic);
                        IsComplete = true;
                    }
                }
            }
        }

        public static NotificationBehaviour Create()
        {
            GameObject notificationObj = new GameObject("NotificationRoot", Il2CppType.Of<RectTransform>());
            var rectTr = notificationObj.GetComponent<RectTransform>();
            var img = notificationObj.AddComponent<Image>();
            rectTr.SetParent(Shinigami.GameHUD.transform);
            img.sprite = Shinigami.NotificationSprite;
            img.color = Shinigami.NotificationSpriteColor;
            img.SetNativeSize();
            img.type = Image.Type.Sliced;
            rectTr.sizeDelta = new Vector2(600f, 60f);
            GameObject imgText = new GameObject("NotificationText", Il2CppType.Of<RectTransform>());
            var textRectr = imgText.GetComponent<RectTransform>();
            textRectr.SetParent(rectTr);
            textRectr.anchorMin = Vector2.zero;
            textRectr.anchorMax = Vector2.one;
            textRectr.localPosition = Vector3.zero;
            var text = imgText.AddComponent<Text>();
            text.text = string.Empty;
            text.font = Shinigami.Font01;
            text.alignment = TextAnchor.MiddleCenter;
            text.fontSize = 24;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = true;
            rectTr.anchorMax = Vector2.one;
            rectTr.anchorMin = Vector2.one;
            rectTr.pivot = new Vector2(1f, 1f);
            rectTr.anchoredPosition = new Vector2(-10f, -10f);

            NotificationBehaviour bhv = notificationObj.AddComponent<NotificationBehaviour>();
            bhv.Text = text;

            return bhv;
        }
    }
}
