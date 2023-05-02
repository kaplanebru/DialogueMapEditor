using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Point : MonoBehaviour
{
    
    [ReadOnly] public SpriteRenderer mainSprite;
    [ReadOnly]public SpriteRenderer[] sprites;
    [ReadOnly]public Node props;
    private Vector2 startLineOffset = new Vector2(0, 0.4f);
    private Vector2 endLineOffset = new Vector2(0, 0.5f);
   

    private void Awake()
    {
        mainSprite = GetComponent<SpriteRenderer>();
        sprites = GetComponentsInChildren<SpriteRenderer>(true).Where(s=>s != mainSprite).ToArray();
    }

    private void Start()
    {
        SetPointIcon(props.type,true);
        DrawLines();
    }
    

    private void OnMouseDown()
    {
        if (!props.unlocked) return;
        EventBus.OnPointSelection?.Invoke(this);
    }

    void DisableAllSprites()
    {
        foreach (var sprite in sprites)
        {
            sprite.gameObject.SetActive(false);
        }
    }

    public void SetPointIcon(MapGenerator.Types type, bool unknown)
    {
        DisableAllSprites();
        if (props.unknown)
        {
            sprites[2].gameObject.SetActive(unknown);
        }
        switch (type)
        {
            case MapGenerator.Types.Merchant:
                sprites[0].gameObject.SetActive(true);
                break;
            case MapGenerator.Types.Treasure:
                sprites[1].gameObject.SetActive(true);
                break;
        }
    }
    
    void DrawLines()
    {
        foreach (var child in props.children)
        {
            LineRenderer lr = Instantiate(GameManager.Instance.line, transform);
            
            Vector2 startPos = props.pos + startLineOffset;
            Vector2 endPos = child.pos - endLineOffset;
            Vector2 middle = (startPos + endPos) / 2;
            
            new Curve(startPos, CurveTangent(middle), endPos).CreateLine(lr);
        }
    }
    
    public void ChangeColor(bool enable)
    {
        mainSprite.color = enable ? GameManager.Instance.unlockedColor : GameManager.Instance.lockedColor;
    }

    float RandomValueExcept(float min, float max)
    {
        float number;
        do
        {
            number = Random.Range (min, max);
        } while (number == 0);
        return number;
    }
    Vector2 CurveTangent(Vector2 middle)
    {
        int dir = 1;
        float multiplier;

        if (middle.x == 0)
        {
            multiplier = RandomValueExcept(-0.6f, 0.6f);
        }
        else
        {
            multiplier = Random.Range(0.5f, 0.8f);
            dir = Mathf.RoundToInt(middle.x / Mathf.Abs(middle.x));
        }
        
        return middle + Vector2.right * dir * multiplier;
       
    }
    
    
    
}
