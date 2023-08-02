using UnityEngine;
public enum AudioPlayType{ADES, BGM, BattleAD, NormalEffectAS, BgmAS, BattleEffectAS}
public partial class AudioManager : SingletonMono<AudioManager>
{
    [Header("播放器")]
    public AudioSource ADES;
    public AudioSource BGM;
    public AudioSource BattleAD;
    private AudioClip[] damage = new AudioClip[3];
    private AudioClip[] stat = new AudioClip[2];
    private bool isEnd = true;
    AudioClip bumb, walkGrass, faint;
    void Start()
    {
        NormalBgm();
        bumb = GetAudio("SoundEffect/Bump");
        walkGrass = GetAudio("SoundEffect/walkGrass");
        damage[0] = GetAudio("BattleEffect/Hit0");
        damage[1] = GetAudio("BattleEffect/Hit1");
        damage[2] = GetAudio("BattleEffect/Hit2");
        stat[0] = GetAudio("BattleEffect/StatUp");
        stat[1] = GetAudio("BattleEffect/StatDown");
        faint = GetAudio("BattleEffect/Faint");
    }

    /// <summary>
    /// 播放Audio
    /// </summary>
    /// <param name="path">"Music/"之后的路径</param>
    /// <param name="type">使用哪个播放器</param>
    public void PlayAudio(string path, AudioPlayType type)
    {
        switch(type)
        {
            case AudioPlayType.ADES    : ADES.clip     = GetAudio(path); ADES.Play()    ; break;
            case AudioPlayType.BGM     : BGM.clip      = GetAudio(path); BGM.Play()     ; break;
            case AudioPlayType.BattleAD: BattleAD.clip = GetAudio(path); BattleAD.Play(); break;
        }
    }

    //人物动作音效
    public void NormalBgm()
    {
        BGM.loop = true;
        BGM.clip = GetAudio("Bgm/6");
        BGM.Play();
    }
    public void ChangeBGM()
    {
        //
    }

    public void CantMoveAudio()
    {
        if(isEnd)
        {
            ADES.clip = bumb;
            ADES.Play();
            isEnd = false;
            Invoke("WaitForBumB", 0.5f);
        }
    }
    private void WaitForBumB() { isEnd = true; }

    private bool grass = true;
    public void WalkGrass()
    {
        if(grass)
        {
            ADES.clip = walkGrass;
            ADES.Play();
            grass = false;
            Invoke("WaitForWalkGrassAudio", 0.3f);
        }
    }
    private void WaitForWalkGrassAudio() { grass = true; }

    //战斗音效
    public void HitSource(int effect)
    {
        BattleAD.clip = damage[effect];
        BattleAD.Play();
    }

    /// <summary>
    /// 战斗濒死声音
    /// </summary>
    public void FaintSource()
    {
        BattleAD.clip = faint;
        BattleAD.Play();
    }

    public void RunSound()
    {
        ADES.clip = GetAudio("SoundEffect/Flee");
        ADES.Play();
    }

    //BGM
    public void BattleBGM()
    {
        BGM.clip = GetAudio("BattleBgm/battlebgm");
        BGM.Play();
    }
    public void ReStartBAD()
    {
        BattleAD.clip = null;
    }

    //点击音效
    public void ViewPlayer()
    {
        BGM.clip = GetAudio("Bgm/View");
        BGM.Play();
    }

    public void StatChangeAudio(bool isUp)
    {
        switch(isUp)
        {
            case true: BattleAD.clip = stat[0]; break;
            case false: BattleAD.clip = stat[1]; break;
        }
        BattleAD.Play();
    }

    //获得道具
    public void GetItemsAudio()
    {
        ADES.clip = GetAudio("SoundEffect/GetItem");
        ADES.Play();
    }

    public void CurePokemon()
    {
        ADES.clip = GetAudio("SoundEffect/healParty");
        ADES.Play();
    }

    public void ChangeTheSceneAudio()
    {
        ADES.clip = GetAudio("SoundEffect/Exit");
        ADES.Play();
    }

    //扔球
    public void ThrowingAudio()
    {
        BattleAD.clip = GetAudio("BattleEffect/PokeballThrow");
        BattleAD.Play();
    }
    public void BallBreakAudio()
    {
        BattleAD.clip = GetAudio("BattleEffect/PokeballOpen");
        BattleAD.Play();
    }
    //电脑
    public void OpenComputer()
    {
        ADES.clip = GetAudio("SoundEffect/ComputerOn");
        ADES.Play();
    }
    public void TapPC()
    {
        ADES.clip = GetAudio("SoundEffect/ComputerAccess");
        ADES.Play();
    }
    public void CloseComputer()
    {
        ADES.clip = GetAudio("SoundEffect/ComputerOff");
        ADES.Play();
    }

    private bool healPlaying;
    //治疗音效
    public void HealPokemon()
    {
        if(!healPlaying)
        {
            healPlaying = true;
            ADES.clip = GetAudio("SoundEffect/Heal");
            ADES.Play();
            Invoke("WaitForHealAudio", 0.5f);
        }
    }
    public void WaitForHealAudio()
    {
        healPlaying = false;
    }

    private AudioClip GetAudio(string n)
    {
        return ResM.Instance.Load<AudioClip>(string.Concat("Music/", n));
    }
}