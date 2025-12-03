using Archipelago.MultiClient.Net.Helpers;
using Camp;
using EOAP.Plugin.AP;
using EOAP.Plugin.EO;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace EOAP.Plugin.Behaviours
{
    public class CheckTextBehaviour : MonoBehaviour
    {
        public Text Text { get; private set; }
        private RectTransform _tr;

        private Text _label;
        private Text _value;
        private Text _max;
        public CheckTextBehaviour(IntPtr ptr) : base(ptr)
        {

        }

        private void Awake()
        {
            _tr = GetComponent<RectTransform>();
            _label = _tr.GetChild(0).gameObject.GetComponent<Text>();
            _value = _tr.GetChild(1).gameObject.GetComponent<Text>();
            _max = _tr.GetChild(3).gameObject.GetComponent<Text>();
            _label.text = "Checks";
        }

        public void SetText(string message)
        {
        }

        public void FetchChecksData()
        {
            EOSession sess = APBehaviour.GetSession();

            if (sess == null || !sess.Connected)
            {
                _value.text = "0";
                _max.text = "0";
            }
            else
            {
                string checkText = string.Empty;
                ILocationCheckHelper locs = sess.Session.Locations;
                _value.text = locs.AllLocationsChecked.Count.ToString();
                _max.text = locs.AllLocations.Count.ToString();
            }
        }


        public static CheckTextBehaviour Create()
        {
            var gatherCopy = GameObject.Instantiate(Shinigami.GatherHUD, Shinigami.GatherHUD.parent);
            var tr2 = gatherCopy.GetComponent<RectTransform>();
            Vector2 p = tr2.anchoredPosition - new Vector2(0f, tr2.sizeDelta.y + 10f);
            tr2.anchoredPosition = p;
            gatherCopy.GetComponent<CanvasGroup>().enabled = false;
            GameObject.Destroy(gatherCopy.GetComponent<CampGatheringCountUiController>());
            gatherCopy.transform.GetChild(0).gameObject.SetActive(false);
            gatherCopy.transform.GetChild(1).gameObject.SetActive(false);

            Transform checkRoot = gatherCopy.transform.GetChild(2);
            CheckTextBehaviour bhv = checkRoot.gameObject.AddComponent<CheckTextBehaviour>();
            gatherCopy.transform.SetParent(Shinigami.GatherHUD, true); // apply gatherhud animations 
            return bhv;
        }
    }
}
