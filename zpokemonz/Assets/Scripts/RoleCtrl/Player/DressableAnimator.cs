using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 换装类型多项Sprite动画器
/// </summary>
public class DressableAnimator
{
    List<SpriteRenderer> spriteRenderers;
    List<List<Sprite>> frames;
    float frameRate;
    int currentFrame;
    float timer;
    int count;
    int framesCount;
    public DressableAnimator(ref List<List<Sprite>> frames, ref List<SpriteRenderer> spriteRenderers, float frameRate = 0.25f)
    {
        this.frames = frames;
        this.spriteRenderers = spriteRenderers;
        this.frameRate = frameRate;
        count = spriteRenderers.Count;
        framesCount = frames[0].Count;
    }

    public void Start()
    {
        timer = 0f;
        currentFrame = 1;
        for(int i = 0; i < count; i++)
        {
            spriteRenderers[i].sprite = frames[i][0];
        }
    }

    public void Reset()
    {
        currentFrame = 0;
        timer = 0.25f;
    }

    public void HandleUpdate()
    {
        timer += Time.deltaTime;
        if(timer > frameRate)
        {
            if(currentFrame == framesCount)
            {
                currentFrame = 0;
            }
            for(int i = 0; i < count; i++)
            {
                spriteRenderers[i].sprite = frames[i][currentFrame];
            }
            currentFrame++;
            timer = 0f;
        }
    }
}