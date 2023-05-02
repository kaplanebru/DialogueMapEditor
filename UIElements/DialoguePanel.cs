using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UIElements
{
    public class DialoguePanel : MonoBehaviour
    {
        private Canvas canvas;
        public TextMeshProUGUI dialogueText, typeText;
        [ReadOnly] public ImageDialogue dialogueImages;
        [ReadOnly] public OptionHandler optionHandler;
        [ReadOnly] public Dialogue panel;

        private void Awake()
        {
            canvas = GetComponent<Canvas>();
            dialogueImages = GetComponentInChildren<ImageDialogue>(true);
            optionHandler = GetComponentInChildren<OptionHandler>(true);
            panel = GetComponentInChildren<Dialogue>(true);
            CloseDialoguePanel();
        }
      

        private void OnEnable()
        {
            EventBus.OnPointSelection += OpenDialoguePanel;
            EventBus.OnDialogueEnd += CloseDialoguePanel; //() => dialogue.gameObject.SetActive(false);
            
        }

        private void OnDisable()
        {
            EventBus.OnPointSelection -= OpenDialoguePanel;
            EventBus.OnDialogueEnd -= CloseDialoguePanel; 
        }


        void OpenDialoguePanel(Point selectedPoint)
        {
            canvas.sortingOrder = 3;
            panel.gameObject.SetActive(true);
            UpdateDialoguePanel(selectedPoint);
            
        }

        void CloseDialoguePanel()
        {
            canvas.sortingOrder = 2;
            panel.gameObject.SetActive(false);
        }

        void UpdateDialoguePanel(Point selectedPoint)
        {
            optionHandler.UpdateOptionTexts(selectedPoint.props);
            dialogueText.text = selectedPoint.props.mainText;
            dialogueImages.ShowElement(selectedPoint.props.type);
            typeText.text = selectedPoint.props.type.ToString();
            
            if (selectedPoint.props.unknown)
                selectedPoint.SetPointIcon(selectedPoint.props.type, false);
            
        }
        
    }

}
