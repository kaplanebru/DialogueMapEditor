using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventBus
{
    public static Action<Point> OnGameStart;
    public static Action<Point> OnPointSelection;
    public static Action OnDialogueEnd;
    public static Action OnSelectOption;
    

}
