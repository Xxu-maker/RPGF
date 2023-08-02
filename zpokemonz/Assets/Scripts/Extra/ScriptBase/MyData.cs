using UnityEngine;
public static class MyData
{
#region 颜色
    /// <summary>
    /// 透明
    /// </summary>
    public static readonly Color hyaline = new Color(0, 0, 0, 0);

    /// <summary>
    /// 50%透明度白
    /// </summary>
    public static readonly Color half = new Color(1f, 1f, 1f, 0.5f);

    /// <summary>
    /// 血条绿
    /// </summary>
    public static readonly Color hp_green = new Color(0.09f, 0.75f, 0.12f);
    public static readonly Color hp_red = new Color(0.97f, 0.34f, 0.15f);
    public static readonly Color hp_orange = new Color(0.97f, 0.69f, 0f);

    public static readonly Color gray = new Color(0.46f, 0.49f, 0.53f);
    public static readonly Color hurtPink = new Color(1f, 0.43f, 0.43f);
    public static readonly Color ballPink = new Color(0.85f, 0.31f, 0.29f);
    public static readonly Color dynamax = new Color(1f, 0.55f, 0.77f);
    public static readonly Color purple = new Color(1f, 0.3f, 1f);
    public static readonly Color deepPurple = new Color(0.6f, 0.1f, 0.3f);

    /// <summary>
    /// 烧伤的颜色
    /// </summary>
    public static readonly Color oRed = new Color(1f, 0.5f, 0f);

    public static readonly Color hpPurple = new Color(0.5f, 0f, 1f);
    public static readonly Color hpDeepPurple = new Color(0.74f, 0f, 1f);
    public static readonly Color hpORed = new Color(1f, 0.23f, 0f);
    public static readonly Color iceBlue = new Color(0f, 0.69f, 1f);
    public static readonly Color parYellow = new Color(1f, 0.84f, 0f);

    /// <summary>
    /// 建造检测grid绿色
    /// </summary>
    public static readonly Color girdGreen = new Color(0.34f, 0.92f, 0.16f, 0.29f);
    /// <summary>
    /// 建造检测grid红色
    /// </summary>
    public static readonly Color girdRed = new Color(0.92f, 0.29f, 0.16f, 0.6f);
#endregion

#region 其它数据
    //一次60帧yield return null 约 0.01f
    //public static readonly WaitForSeconds delayHalf = new WaitForSeconds(0.5f);

    public static readonly Vector3 plane = new Vector3(90, 0, 0);
    public static readonly Vector2 boxCast = new Vector2(0.2f, 0.2f);
    public static readonly Vector3 rectangularCast = new Vector2(0.8f, 0.8f);

    /// <summary>
    /// {1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f}
    /// </summary>
    public static readonly float[] BoostValues = new float[]{ 1f, 1.5f, 2f, 2.5f, 3f, 3.5f, 4f };
#endregion

    public static Vector3 ctrl = new Vector3(380f, 350f, 0);
    public static Vector3 aButton = new Vector3(1995f, -345f, 0);
    public static bool change = false;

#region string
    public static readonly string[] statsString =
    {
        "", "的物攻", "的物防", "的特攻", "的特防", "的速度", "的命中", "的闪避"
    };
    public static readonly string miniSprite = "Pokemon/Mini/";
#endregion
}