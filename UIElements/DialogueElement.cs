using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Component = UnityEngine.Component;

namespace UIElements
{
    public abstract class DialogueElement<T> : MonoBehaviour where T : Component
    {

        [Unity.Collections.ReadOnly] public List<T> dialogueElements;
        protected abstract void Initialize();

        void Awake()
        {
            GetElements();
            Initialize();
        }

        void GetElements()
        {
            dialogueElements = GetComponentsInChildren<T>(true).ToList();
        }

        void DisableAll()
        {
            foreach (var element in dialogueElements)
            {
                element.gameObject.SetActive(false);
            }
        }

        public void ShowElement(MapGenerator.Types type)
        {
            DisableAll();
            dialogueElements[(int) type].gameObject.SetActive(true);
        }
    }
}