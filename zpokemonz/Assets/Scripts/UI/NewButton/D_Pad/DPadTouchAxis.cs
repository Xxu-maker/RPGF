using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
namespace ZButton
{
	public class DPadTouchAxis : MonoBehaviour,
	IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
	{
		public enum ButtonStates { Off, ButtonDown, ButtonPressed, ButtonUp }

		[Header("按下时调用(tap计时)")]
		[Header("绑定")]
		public UnityEvent AxisDown;//按下时调用(tap计时)

		[Header("tap时调用(人物转向)")]
		public UnityEvent<float> AxisTap;//tap时调用(人物转向)

		[Header("按住时调用(人物移动)")]
		public UnityEvent<float> AxisPressed;//按住时调用(人物移动)

		[Header("按键退出调用(重置tap时间, 重置按键动画)")]
        public UnityEvent AxisExit;//按键退出调用(重置tap时间，重置按键动画)

		[Header("按键交互计时器")]
		[SerializeField] DPadInteractionsTime dPadInteractionsTime;

		[Header("按下轴时发送绑定方法的值")]
		[SerializeField] float axisValue;

		[Header("方向 (上0 下1 左2 右3) 不要用enum 要转int 对移动不划算")]
		[SerializeField] int directionValue;
		private ButtonStates CurrentState;

		//如果触摸区被按下, 触发绑定方法
		protected virtual void Update()
	    {
			if (CurrentState == ButtonStates.ButtonPressed)
			{
				//如果没有超过tap计算的时间段
				if(!dPadInteractionsTime.IsTheTapTimeExceeded())
				{
					//没超时Tap
				    AxisTap.Invoke(directionValue);
				}
				else
				{
					//超时正常移动
					AxisPressed.Invoke(axisValue);
				}
			}
	    }

		//每帧结束时, 处理按钮的状态
		protected virtual void LateUpdate()
		{
			if (CurrentState == ButtonStates.ButtonUp)
			{
				AxisExit.Invoke();
				CurrentState = ButtonStates.Off;
			}
			if (CurrentState == ButtonStates.ButtonDown)
			{
				CurrentState = ButtonStates.ButtonPressed;
			}
		}

		//按下
		public virtual void OnPointerDown(PointerEventData data)
	    {
			if(CurrentState != ButtonStates.Off)
			{
				return;
			}
			AxisDown.Invoke();
			CurrentState = ButtonStates.ButtonDown;
	    }

		//松开
		public virtual void OnPointerUp(PointerEventData data)
		{
			if(CurrentState != ButtonStates.ButtonPressed && CurrentState != ButtonStates.ButtonDown)
			{
				return;
			}
			AxisPressed.Invoke(0);
			CurrentState = ButtonStates.ButtonUp;
	    }

		//当触摸进入区域时
		public void OnPointerEnter(PointerEventData data)
		{
			OnPointerDown (data);
		}

		//当触摸超出区域时
		public void OnPointerExit(PointerEventData data)
		{
			OnPointerUp (data);
		}
    }
}