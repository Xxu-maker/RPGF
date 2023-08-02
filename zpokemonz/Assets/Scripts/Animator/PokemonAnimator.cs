using System.Text;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
public class PokemonAnimator : MonoBehaviour
{
    [SerializeField] Transform _trans;//播放器位置
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SpriteRenderer shadow;
    [SerializeField] SpriteRenderer skill;
    [SerializeField] SpriteRenderer megaOrCloud;
    [SerializeField] ParticleSystem shiny;
    [SerializeField] ParticleSystem buff;
    [SerializeField] ParticleSystem debuff;
    [SerializeField] ParticleSystem mbuff;
    /// <summary>
    /// 播放bool值
    /// </summary>
    private bool play;
    private Sprite[] sprites;
    private Sprite[] skillSprites;
    private Sprite[] megaIcon;
    private Sprite[] cloud;
    StringBuilder sB = new StringBuilder();
    private bool playDynamaxCloud;
    private int normalCount;

    /// <summary>
    /// 设置动画属性
    /// </summary>
    public async void SetAnimation(Pokemon pokemon, bool isPlayer, bool mega, bool Gigantamax, Vector2 pos)
    {
        if(!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }
        ResetAnimator();

        PokemonSpriteSetData spriteSetData = pokemon.Base.SpriteSetData;
        normalCount = spriteSetData.NormalCount[0];

        //读取图片
        int positionFixIndex = 0;
        switch(isPlayer)
        {
            case true:  sB.Append("Pokemon/B");                       break;
            case false: sB.Append("Pokemon/F"); positionFixIndex = 1; break;
        }

        string pokeIDPath = string.Concat("/", pokemon.Base.ID.ToString());
        if(spriteSetData.FixY.Length > 2)
        {
            if(mega)
            {
                pokeIDPath += "M"; positionFixIndex += 2; normalCount = spriteSetData.NormalCount[1];
            }

            if(Gigantamax)
            {
                pokeIDPath += "G"; positionFixIndex += 4; normalCount = spriteSetData.NormalCount[2];
            }
        }

        if(pokemon.Shiny)
        {
            pokeIDPath += "S";
        }

        sB.Append(pokeIDPath);
        sB.Append(pokeIDPath);//不然会读到别的, 不知道为什么

        sprites = ResM.Instance.LoadAllSprites(sB.ToString());

        //阴影修正
        bool shaded = shadow != null;
        if(shaded)
        {
            shadow.transform.position = spriteSetData.ShadowFix[positionFixIndex];
        }
        //设置宝可梦位置
        pos.y += spriteSetData.FixY[positionFixIndex];
        transform.position = pos;

        sB.Clear();

        //等待上一个动画结束
        await UniTask.WaitUntil(WaitForAnimateEnd);

        //开启动画
        if(sprites != null)
        {
            play = true;
            Animate(pokemon.Shiny, shaded);

            if(mega)
            {
                MegaOrDynamaxAnim(mega);
            }
        }
    }

    /// <summary>
    /// 等待上个动画结束
    /// </summary>
    private bool WaitForAnimateEnd() => isStop == true;

    /// <summary>
    /// 停止动画
    /// </summary>
    public void ResetAnimator()
    {
        play = false;

        if(playDynamaxCloud)
        {
            playDynamaxCloud = false;
        }

        if(_trans.position != Vector3.zero)
        {
            _trans.position = Vector3.zero;
        }

        if(_trans.localScale != Vector3.one)
        {
            _trans.localScale = Vector3.one;
        }

        if(spriteRenderer.color != Color.white)
        {
            spriteRenderer.color = Color.white;
        }

        if(megaOrCloud!= null && megaOrCloud.sprite != null)
        {
            megaOrCloud.sprite = null;
        }
    }

    /// <summary>
    /// 防止详细面板动画没关掉
    /// </summary>
    public void ForcedStopAnimator()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 停止极巨化的云
    /// </summary>
    public void StopCloud()
    {
        if(playDynamaxCloud)
        {
            playDynamaxCloud = false;
        }
    }

    private bool isStop = true;
    /// <summary>
    /// 宝可梦动画播放
    /// </summary>
    /// <param name="isShiny">是否闪光</param>
    /// <param name="shaded">是否要阴影</param>
    private async void Animate(bool isShiny, bool shaded)
    {
        isStop = false;
        if(isShiny && shiny != null)
        {
            shiny.Play();
        }

        int turn = 0;
        int count = sprites.Length - 1;
        int frame = 0;
        bool x = normalCount == 0;

        if(shaded)
        {
            while (play)
            {
                spriteRenderer.sprite = sprites[frame];
                shadow.sprite = sprites[frame];
                await UniTask.Delay(60);

                ++frame;
                if(x)
                {
                    if(frame > count)
                    {
                        frame = 0;
                    }
                }
                else
                {
                    if(turn < 3)
                    {
                        if(frame > normalCount)
                        {
                            ++turn;
                            frame = 0;
                        }
                    }
                    else
                    {
                        if(frame > count)
                        {
                            frame = 0;
                            turn = 0;
                        }
                    }
                }
            }

            shadow.sprite = null;
        }
        else
        {
            while (play)
            {
                spriteRenderer.sprite = sprites[frame];
                await UniTask.Delay(60);

                ++frame;
                if(x)
                {
                    if(frame > count)
                    {
                        frame = 0;
                    }
                }
                else
                {
                    if(turn < 3)
                    {
                        if(frame > normalCount)
                        {
                            ++turn;
                            frame = 0;
                        }
                    }
                    else
                    {
                        if(frame > count)
                        {
                            frame = 0;
                            turn = 0;
                        }
                    }
                }
            }
        }

        spriteRenderer.sprite = null;
        isStop = true;
    }
#region 技能动画
    /// <summary>
    /// 技能动画
    /// </summary>
    public async UniTask SingleAnimate(int sid)//float _fps = 0.03f
    {
        skillSprites = ResM.Instance.LoadAllSprites(string.Concat("Skill/", sid.ToString()));

        int skillFrame = 0;
        int skillCount = skillSprites.Length;
        while (skillFrame < skillCount)
        {
            skill.sprite = skillSprites[skillFrame];
            ++skillFrame;
            await UniTask.Delay(20);
        }
        skill.sprite = null;
    }

    /// <summary>
    /// 状态动画
    /// </summary>
    public async UniTask EffectAnimate(byte cid)
    {
        skillSprites = ResM.Instance.LoadAllSprites(string.Concat("Effect/", cid.ToString()));

        int skillFrame = 0;
        int skillCount = skillSprites.Length;
        if(cid == 4)
        {
            skill.color = MyData.half;
        }
        while (skillFrame < skillCount)
        {
            skill.sprite = skillSprites[skillFrame];
            ++skillFrame;
            await UniTask.Delay(70);
        }
        skill.color = Color.white;
        skill.sprite = null;
    }

    public void BuffEffects(byte x)
    {
        switch(x)
        {
            case 0: buff.Play(); break;
            case 1: debuff.Play(); break;
            case 2: mbuff.Play(); break;
        }
    }

    public void SkillMaterial(Material material)
    {
        play = false;
        spriteRenderer.sprite = sprites[0];
        spriteRenderer.material = material;
    }
#endregion
#region 战斗中进化
    public void InjuredColor() => spriteRenderer.color = MyData.hurtPink;//受伤
    public void NormalColor()  => spriteRenderer.color = Color.white    ;//恢复白色
    public void DynamaxColor() => spriteRenderer.color = MyData.dynamax ;

    /// <summary>
    /// Mega进化
    /// </summary>
    public async UniTask MegaEvolutionAnim(Action<bool, bool> changeSpriteAction)
    {
        await PokemonAlphaFade(true);

        changeSpriteAction?.Invoke(true, false);

        await PokemonAlphaFade(false);
    }

    /// <summary>
    /// 超级巨化
    /// </summary>
    public async UniTask GigantamaxEvolutionAnim(Action<bool, bool> changeSpriteAction)
    {
        await PokemonAlphaFade(true);

        changeSpriteAction?.Invoke(false, true);

        await PokemonAlphaFade(false);

        MegaOrDynamaxAnim(false);
    }

    /// <summary>
    /// 结束超极巨化
    /// </summary>
    public async UniTask EndGigantamax(Action<bool, bool> changeSpriteAction)
    {
        await EndGigantamaxAnim();

        changeSpriteAction?.Invoke(false, false);

        await PokemonAlphaFade(false);
    }

    /// <summary>
    /// 过渡动画
    /// </summary>
    /// <param name="hide">true为隐藏</param>
    public async UniTask PokemonAlphaFade(bool hide)
    {
        if(hide)
        {
            float i = 1f;
            while(i > 0.1)
            {
                i -= 0.05f;
                animColor.a = i;
                spriteRenderer.color = animColor;
                await UniTask.Yield();
            }
            animColor.a = 0f;
            spriteRenderer.color = animColor;
            play = false;
            await UniTask.Delay(100);
        }
        else
        {
            float i = 0;
            while(i < 0.9)
            {
                i += 0.05f;
                animColor.a = i;
                spriteRenderer.color = animColor;
                await UniTask.Yield();
            }
            NormalColor();
            await UniTask.Delay(100);
        }
    }

    /// <summary>
    /// 普通极巨化
    /// </summary>
    /// <returns></returns>
    public async UniTask DynamaxEvolutionAnim()
    {
        Vector3 origin = _trans.position;
        dynamaxPos = origin;
        Vector3 scale = _trans.localScale;
        saveScale = scale;

        float i = 1f;
        while(i < 1.7)
        {
            i += 0.05f;
            //增大
            scale.x = i;
            scale.y = i;
            origin.y += 0.1f;

            _trans.localScale = scale;
            _trans.position = origin;
            await UniTask.Yield();
        }
        MegaOrDynamaxAnim(false);
    }

    /// <summary>
    /// 结束极巨化
    /// </summary>
    public void EndDynamaxAnim()
    {
        StopCloud();
        _trans.position = dynamaxPos;
        _trans.localScale = saveScale;
    }

    /// <summary>
    /// 结束超极巨化时的动画
    /// </summary>
    /// <returns></returns>
    public async UniTask EndGigantamaxAnim()
    {
        StopCloud();
        Vector3 scale = _trans.localScale;

        float i = 1f;
        while(i > 0.2)
        {
            i -= 0.05f;
            //缩小
            scale.x = i;
            scale.y = i;
            //透明度
            animColor.a = i;

            spriteRenderer.color = animColor;
            _trans.localScale = scale;
            await UniTask.Yield();
        }
        animColor.a = 0f;
        spriteRenderer.color = animColor;
    }

    /// <summary>
    /// Mega和极巨化特效
    /// </summary>
    /// <param name="mega">true为Mega</param>
    public async void MegaOrDynamaxAnim(bool mega)
    {
        int evoFrame = 0;//当前帧数
        switch(mega)
        {
            case true:

                if(megaIcon == null || megaIcon.Length == 0)
                {
                    megaIcon = ResM.Instance.LoadAllSprites("Mega");
                }
                int megaCount = megaIcon.Length;
                byte turn = 0;
                float i = 0;
                bool c = true;
                Color color = Color.white;
                while (turn < 3)
                {
                    evoFrame++;
                    if (evoFrame >= megaCount)
                    {
                        evoFrame = 0;
                        turn++;
                    }
                    if(c)
                    {
                        if(i < 0.85f)
                        {
                            i += 0.06f;
                            color.a = i;
                            megaOrCloud.color = color;
                        }
                        else
                        {
                            c = false;
                        }
                    }
                    else
                    {
                        i -= 0.08f;
                        color.a = i;
                        megaOrCloud.color = color;
                    }

                    megaOrCloud.sprite = megaIcon[evoFrame];
                    await UniTask.Delay(30);
                }

            break;

            case false:

                if(cloud == null || cloud.Length == 0)
                {
                    cloud = ResM.Instance.LoadAllSprites("dCloud");
                }

                int cloudCount = cloud.Length;
                megaOrCloud.color = Color.white;

                playDynamaxCloud = true;
                while(playDynamaxCloud)
                {
                    evoFrame++;
                    if (evoFrame >= cloudCount)
                    {
                        evoFrame = 0;
                    }
                    megaOrCloud.sprite = cloud[evoFrame];
                    await UniTask.Delay(70);
                }

            break;
        }

        if(megaOrCloud.sprite != null)
        {
            megaOrCloud.sprite = null;
        }
    }
#endregion
#region 战斗事件动画
    /// <summary>
    /// 存储的位置
    /// </summary>
    private Vector3 savePos;
    /// <summary>
    /// 储存的scale
    /// </summary>
    private Vector3 saveScale;
    /// <summary>
    /// 极巨化存的位置
    /// </summary>
    private Vector3 dynamaxPos;
    /// <summary>
    /// 用于处理动画的color
    /// </summary>
    private Color animColor = Color.white;

    /// <summary>
    /// 替换动画
    /// </summary>
    public async UniTask ZoomInPokemonAnim()
    {
        spriteRenderer.color = MyData.hurtPink;

        Vector3 origin = _trans.position;
        savePos = origin;
        Vector3 scale = _trans.localScale;
        saveScale = scale;

        float i = 1f;
        while(i > 0.1)
        {
            i -= 0.05f;
            //缩小
            scale.x = i;
            scale.y = i;
            //向下
            origin.y += 0.1f;

            _trans.localScale = scale;
            _trans.position = origin;
            await UniTask.Yield();
        }
        _trans.localScale = Vector3.zero;
    }

    /// <summary>
    /// 濒死动画
    /// </summary>
    public async UniTask Fainted()
    {
        Vector3 origin = _trans.position;
        float i = 1f;

        AudioManager.Instance.FaintSource();
        while(i > 0.2f)
        {
            i -= 0.1f;
            animColor.a = i;
            origin.y -= 0.1f;

            spriteRenderer.color = animColor;
            shadow.color = animColor;
            _trans.position = origin;
            await UniTask.Yield();
        }
        animColor.a = 0f;
        spriteRenderer.color = animColor;
        play = false;
    }

    /// <summary>
    /// 捕捉时宝可梦缩小动画
    /// </summary>
    /// <returns></returns>
    public async UniTask NarrowPokemonAnim()
    {
        spriteRenderer.color = MyData.ballPink;
        Vector3 origin = _trans.position;
        Vector3 scale = _trans.localScale;
        savePos = origin;
        saveScale  = scale;

        float i = 1f;
        while(i > 0.1)
        {
            i -= 0.05f;
            //缩小
            scale.x = i;
            scale.y = i;
            //向下
            origin.y += 0.1f;

            _trans.localScale = scale;
            _trans.position = origin;
            await UniTask.Yield();
        }
        _trans.localScale = Vector3.zero;
    }

    /// <summary>
    /// 用于恢复捕捉缩小(及收回宝可梦)
    /// </summary>
    public void ResetColorAndTrans()//恢复原状
    {
        spriteRenderer.color = Color.white;
        _trans.localScale = saveScale;
        _trans.position = savePos;
    }

    /// <summary>
    /// 突进动画
    /// </summary>
    /// <returns></returns>
    public async UniTask AttackAnim(bool isPlayerUnit)//突进动画
    {
        Vector3 origin = _trans.position;
        savePos = origin;

        float a = 0.4f;

        if(!isPlayerUnit)
        {
            for(byte i = 0; i < 5; ++i)
            {
                origin.x -= a; origin.y -= a;
                _trans.position = origin;
                await UniTask.Yield();
            }
        }
        else
        {
            for(byte i = 0; i < 5; ++i)
            {
                origin.x += a; origin.y += a;
                _trans.position = origin;
                await UniTask.Yield();
            }
        }
    }

    /// <summary>
    /// 后退动画
    /// </summary>
    /// <returns></returns>
    public async UniTask EndAttackAnim(bool isPlayerUnit)
    {
        Vector3 origin = _trans.position;
        float a = 0.4f;

        if(!isPlayerUnit)
        {
            for(byte i = 5; i > 0; --i)
            {
                origin.x += a; origin.y += a;
                _trans.position = origin;
                await UniTask.Yield();
            }
        }
        else
        {
            for(byte i = 5; i > 0; --i)
            {
                origin.x -= a; origin.y -= a;
                _trans.position = origin;
                await UniTask.Yield();
            }
        }
        _trans.position = savePos;
        await UniTask.Yield();
    }
#endregion
#region 状态变色
    public async UniTask ConditionColor(Condition status)
    {
        if(status == null)//混乱
        {
            //变化
            return;
        }

        ConditionID conditionID = status.ConditionID;
        switch(conditionID)
        {
            //中毒
            case ConditionID.psn:

                spriteRenderer.color = MyData.purple;

            break;

            //剧毒
            case ConditionID.hyp:

                spriteRenderer.color = MyData.deepPurple;

            break;

            //烧伤
            case ConditionID.brn:

                spriteRenderer.color = MyData.oRed;

            break;

            //case 3: break;

            //冰冻
            case ConditionID.frz:

                spriteRenderer.color = Color.blue;
                await EffectAnimate(4);

            break;

            //麻痹
            case ConditionID.par:

                spriteRenderer.color = Color.yellow;
                await EffectAnimate(5);

            break;
            //yield return stage.EffectAnimate(pokemon.Status.Cid, 0.2f);
        }
        //抖动动画
        await UniTask.Delay(1000);
        NormalColor();
    }
#endregion
}