using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UIElements
{
    public class OptionHandler : MonoBehaviour
    {
        [ReadOnly] public Node currentNode;
        [ReadOnly] public List<OptionUI> optionsUI;
        [ReadOnly] public bool optionSelected = false;
        private bool gameEnded = false;


        private void Awake()
        {
            optionsUI = GetComponentsInChildren<OptionUI>().ToList();
            optionsUI.Reverse();
        }
        
        
        private void OnEnable()
        {
            EventBus.OnSelectOption += AfterOptionSelection;
        }

        private void OnDisable()
        {
            EventBus.OnSelectOption -= AfterOptionSelection;
        }

        public void UpdateOptionTexts(Node node)
        {
            currentNode = node;
            SetAnswerAmount();

            for (int i = 0; i < node.options.Count; i++)
            {
                optionsUI[i].UpdateOption(node.options[i]); 
            }
        }
        public void Leave()
        {
            if (!optionSelected)
            {
                if (currentNode.children.Count > 0)
                {
                    currentNode.children[0].UnlockPoint(true);
                    SaveSystem.SaveUpgrade( currentNode.options[0].id-1, UpgradeHandler.Instance.stats);
                }
                else
                {
                    gameEnded = true;
                }
                AfterOptionSelection();
            }
            
            Invoke(nameof(EndDialogue), 0.3f);
        }

        void EndDialogue()
        {
            optionSelected = false;
            EventBus.OnDialogueEnd?.Invoke();
        }

        void AfterOptionSelection()
        {
            optionSelected = true;
            if(!gameEnded)
                currentNode.UnlockPoint(false);
            Invoke( nameof(CloseButtons), 0.3f);
        }

        void CloseButtons()
        {
            optionsUI.ForEach(o=>o.button.gameObject.SetActive(false));
        }
        
        void SetAnswerAmount()
        {
            ResetButtons();
            for (int i = optionsUI.Count - 1; i >= currentNode.options.Count; i--)
            {
                optionsUI[i].button.gameObject.SetActive(false);
            }
        }

        void ResetButtons()
        {
            optionsUI.ForEach(o =>
            {
                o.button.gameObject.SetActive(true);
                o.button.interactable = true;
            });
        }


    }
}
