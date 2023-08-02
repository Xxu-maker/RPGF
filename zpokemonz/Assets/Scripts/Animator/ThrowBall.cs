using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Cysharp.Threading.Tasks;
/// <summary>
/// 投掷&抓捕 精灵球动画
/// </summary>
public class ThrowBall : MonoBehaviour
{
	[SerializeField] Transform _trans;
	[SerializeField] SpriteRenderer spriteRenderer;
	[SerializeField] SpriteRenderer highLightRenderer;
	[SerializeField] ParticleSystem catchStars;
	List<Sprite> roll  = new List<Sprite>();
	List<Sprite> open  = new List<Sprite>();
	List<Sprite> close = new List<Sprite>();
	List<Sprite> sL    = new List<Sprite>();
	List<Sprite> sR    = new List<Sprite>();
	private int saveN = 0;
	private Color color = Color.white;
#region 获取精灵球图片
    public void SetBallSprite(int n)//球从1开始记
	{
		if(saveN == n)
		{
			return;
		}
		else
		{
			roll.Clear(); open.Clear(); close.Clear();
			sL.Clear(); sR.Clear();
		}
		saveN = n;
        Sprite[] resSprite = ResM.Instance.LoadAllSprites(string.Concat("Ball/", n.ToString()));

		for(int i = 0; i < 8; ++i)
		{
			roll.Add(resSprite[i]);
		}
		roll.Add(resSprite[0]);

		open.Add(resSprite[8]);open.Add(resSprite[9]);open.Add(resSprite[10]);

		close.Add(resSprite[9]);close.Add(resSprite[0]);

		sL.Add(resSprite[0]);
		sL.Add(resSprite[11]);sL.Add(resSprite[12]);sL.Add(resSprite[13]);
		sL.Add(resSprite[12]);sL.Add(resSprite[11]);sL.Add(resSprite[0]);

		sR.Add(resSprite[0]);
		sR.Add(resSprite[14]);sR.Add(resSprite[15]);sR.Add(resSprite[16]);
		sR.Add(resSprite[15]);sR.Add(resSprite[14]);sR.Add(resSprite[0]);
		//if(saveN == n) { return; }
		//saveN = n;

        //Sprite[] resSprite = ResM.Instance.LoadAllSprites(string.Concat("Ball/", n.ToString()));

		//if(roll == null)
		//{
		//	List<Sprite> roll = new List<Sprite>();
	    //    List<Sprite> open = new List<Sprite>();
	    //    List<Sprite> close = new List<Sprite>();
	    //    List<Sprite> sL = new List<Sprite>();
	    //    List<Sprite> sR = new List<Sprite>();
        //    for(int i = 0; i < 8; ++i)
		//    {
		//    	roll.Add(resSprite[i]);
		//    }
		//    roll.Add(resSprite[0]);

    	//    open.Add(resSprite[8]);open.Add(resSprite[9]);open.Add(resSprite[10]);

    	//    close.Add(resSprite[9]);close.Add(resSprite[0]);

    	//    sL.Add(resSprite[0]);
    	//    sL.Add(resSprite[11]);sL.Add(resSprite[12]);sL.Add(resSprite[13]);
    	//    sL.Add(resSprite[12]);sL.Add(resSprite[11]);sL.Add(resSprite[0]);

		//    sR.Add(resSprite[0]);
		//    sR.Add(resSprite[14]);sR.Add(resSprite[15]);sR.Add(resSprite[16]);
		//    sR.Add(resSprite[15]);sR.Add(resSprite[14]);sR.Add(resSprite[0]);
		//}
		//else
		//{
		//    for(int i = 0; i < 8; ++i)
		//    {
		//    	roll[i] = resSprite[i];
		//    }
		//    roll[8] = resSprite[0];

		//    open[0] = resSprite[8]; open[1] = resSprite[9]; open[2] = resSprite[10];

		//    close[0] = resSprite[9]; close[1] = resSprite[0];

		//    sL[0] = resSprite[0] ;
		//    sL[1] = resSprite[11]; sL[2] = resSprite[12]; sL[3] = resSprite[13];
		//    sL[4] = resSprite[12]; sL[5] = resSprite[11]; sL[6] = resSprite[0] ;

		//    sR[0] = resSprite[0] ;
		//    sR[1] = resSprite[14]; sR[2] = resSprite[15]; sR[3] = resSprite[16];
		//    sR[4] = resSprite[15]; sR[5] = resSprite[14]; sR[6] = resSprite[0] ;
		//}
	}
#endregion
#region 投掷动画
	public async UniTask ThrowBallAnim(Vector3 targetPos, int n)
	{
		SetBallSprite(n);//设置球种图片
		gameObject.SetActive(true);
		targetPos.y += 3;

		AudioManager.Instance.ThrowingAudio();//投掷音效

		//抛物线
		//var sequence = DOTween.Sequence();
		_trans.DOJump(targetPos, 4f, 1, 1.5f);
		//空中旋转
		byte num = 0;//轮次
		int frame = 0;
		int count = 9;
		while(num < 5)
		{
		    frame++;
            if (frame >= count)
            {
                frame = 0;
			    num++;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(20);
		}
		#pragma warning disable 4014//不需要等待
		Animation(open, 0, false);
	}

	/// <summary>
	/// 关球及之后动画
	/// </summary>
	/// <returns></returns>
	public async UniTask CloseBall()
	{
		int frame = 0;
		int count = close.Count - 1;

        while (frame < count)//关球
        {
            frame++;
            spriteRenderer.sprite = close[frame];
			await UniTask.Delay(30);
        }

		//球亮一下每帧delay0.02或0.03
		for(float hra = 0f; hra < 0.9f; hra += 0.1f)//亮
		{
			color.a = hra;
			highLightRenderer.color = color;
			await UniTask.Delay(30);
		}
		highLightRenderer.color = Color.white;
		await UniTask.Delay(30);

		for(float hra = 1f; hra > 0.1f; hra -= 0.1f)//暗
		{
			color.a = hra;
			highLightRenderer.color = color;
			await UniTask.Delay(30);
		}
		highLightRenderer.color = MyData.hyaline;
		await UniTask.Delay(30);

		frame = 0;
		Vector3 nowPos = _trans.position;
		float i = nowPos.y - 3;
		float a = 0.01f;
		count = 9;
		//掉落
		while(i < nowPos.y)
		{
            nowPos.y -= a;
			nowPos.x += 0.01f;
			a += 0.02f;
			_trans.position = nowPos;
			frame++;
            if (frame >= count)
            {
                frame = 0;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(10);
		}

		i = nowPos.y + 1;
		a = 0.15f;
		//弹起
		while(i > nowPos.y)
		{
            nowPos.y += a;
			if(a > 0.03f)
			{
                a -= 0.01f;
			}
			nowPos.x += 0.01f;
			_trans.position = nowPos;
			frame++;
            if (frame >= count)
            {
                frame = 0;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(10);
		}

		//弹起空转
		while(frame == 0)
		{
            frame++;
            if (frame >= count)
            {
                frame = 0;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(10);
		}

		i = nowPos.y - 1;
		a = 0.01f;
		//下落
		while(i < nowPos.y)
		{
			nowPos.y -= a;
			a += 0.01f;
			nowPos.x += 0.01f;
			_trans.position = nowPos;
			frame++;
            if (frame >= count)
            {
                frame = 0;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(10);
		}

        //落地归0
		while(frame != 0)
		{
            ++frame;
            if (frame >= count)
            {
                frame = 0;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(20);
		}
	}

	public async UniTask Animation(List<Sprite> sprites, int sheet, bool forever)
	{
        int frame = sheet;
		int count = sprites.Count;
		if(forever)
		{
            while (true)
            {
                ++frame;
                if (frame >= count)
                {
                    frame = 0;
                }
                spriteRenderer.sprite = sprites[frame];
				await UniTask.Delay(30);
            }
		}
		else
		{
			count--;
            while (frame < count)
            {
                ++frame;
                spriteRenderer.sprite = sprites[frame];
				await UniTask.Delay(30);
            }
		}
	}
#endregion
#region 颜色变化
	public void CatchPokemon()
	{
		catchStars.Play();
		spriteRenderer.color = MyData.gray;
	}
	public void Normal()
	{
		spriteRenderer.color = Color.white;
	}
#endregion
#region 随机左右摇动
	public async UniTask ShakeThis()
	{
		await Animation(Random.value > 0.5f? sL : sR, 0, false);
	}
#endregion

#region 放出精灵丢球动画
    //public IEnumerator ReleaseThePokemon(bool player, Vector3 startPos, Vector3 targetPos, float power = 4f)
	public async UniTask ReleaseThePokemon(Vector3 targetPos, float duration = 1f)
	{
		//投掷音效
		AudioManager.Instance.ThrowingAudio();

		gameObject.SetActive(true);

		SetBallSprite(1);//设置球种图片
		targetPos.y += 2;
		_trans.DOJump(targetPos, 4f, 1, duration);

		byte num = 0;//轮次
		int frame = 0;//当前帧
		int count = 9;//球图片总帧数
		while(num < 4)
		{
		    frame++;
            if (frame >= count)
            {
                frame = 0;
			    num++;
            }
            spriteRenderer.sprite = roll[frame];
			await UniTask.Delay(20);
		}

		//开球
		await Animation(open, 0, false);
		await UniTask.Delay(20);

		gameObject.SetActive(false);
	}
#endregion
}