using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIElements
{
    public class OptionUI : MonoBehaviour
    {
        [ReadOnly]public Node.Option option;
        [ReadOnly]public Button button;
        [ReadOnly] public TextMeshProUGUI optionText;

        private int checkPoint;

        private void Awake()
        {
            button = GetComponent<Button>();
            optionText = GetComponentInChildren<TextMeshProUGUI>();
        }
       

        public void UpdateOption(Node.Option _option)
        {
            button.interactable = UpgradeHandler.Instance.HasMoney();
            option = _option;
            optionText.text =  "[" + option.stat + "] "
                              + option.price +" Gold: " + option.mainText;
                                //+ option.value + " / "
            
        }

        public void SelectOption()
        {
            EventBus.OnSelectOption?.Invoke();
            Node nextNode = option.nextNode;
            if (nextNode)
            {
                nextNode.UnlockPoint(true);
                checkPoint = nextNode.id - 1;
            }
            UpgradeHandler.Instance.UpgradeStat((int)option.stat, option.value, option.price, checkPoint);
            
        }
        
    }
}

