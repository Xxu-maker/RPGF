using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class Character : MonoBehaviour
{
    [SerializeField] Transform _trans;
    public float moveSpeed;
    //[SerializeField] Follow follow;
    [SerializeField] CharacterAnimator animator;
    private bool IsMoving;

    /// <summary>
    /// Async Move
    /// </summary>
    public async UniTask IsFinishMoving(Vector2 moveVec, Action<bool> OnFinishMoving, Action OnMoveOver = null)
    {
        Vector3 oldPos = _trans.position;
        Vector3 targetPos = oldPos;
        targetPos.x += moveVec.x;
        targetPos.y += moveVec.y;
        if(!IsPathClear(targetPos))
        {
            animator.UpdateMoveXY(moveVec.x, moveVec.y);
            OnFinishMoving?.Invoke(false);
            return;
        }
        animator.SetMoveXYValue(moveVec.x, moveVec.y);
        IsMoving = true;
        while ((targetPos - _trans.position).sqrMagnitude > Mathf.Epsilon)
        {
            _trans.position = Vector3.MoveTowards(_trans.position, targetPos, moveSpeed * Time.deltaTime);
            await UniTask.Yield();
        }
        _trans.position = targetPos;
        IsMoving = false;
        OnFinishMoving?.Invoke(true);
        OnMoveOver?.Invoke();
    }

    public void HandleUpdate()
    {
        animator.HandleUpdate(IsMoving);
    }

    /// <summary>
    /// 检查路径通畅
    /// </summary>
    /// <param name="targetPos">目的地</param>
    /// <returns>是否通畅</returns>
    private bool IsPathClear(Vector3 targetPos)
    {
        //Physics2D.BoxCast
        //origin   盒体在2D空间中的起点   size      盒体的大小
        //angle    盒体的角度(以度为单位) direction 表示盒体方向的矢量。
        //distance 盒体的最大投射距离     layerMask 过滤器,用于仅在特定层上检测碰撞体

        Vector3 diff = targetPos - transform.position;
        Vector3 dir = diff.normalized;
        Vector2 origin = transform.position + dir;
        float distance = diff.magnitude - 1;
        //先检测一遍移动人物，减少人物穿越
        if(Physics2D.BoxCast(origin, MyData.rectangularCast, 0f, dir, distance, GameLayers.Instance.NpcLayer))
        {
            return false;
        }
        return !Physics2D.BoxCast(origin, MyData.boxCast, 0f, dir, distance, GameLayers.Instance.PathClearLayers);
    }

    public void LookTowards(Vector3 targetPos)
    {
        float xdiff = Mathf.Floor(targetPos.x) - Mathf.Floor(_trans.position.x);
        float ydiff = Mathf.Floor(targetPos.y) - Mathf.Floor(_trans.position.y);
        if(xdiff == 0 || ydiff == 0)
        {
            animator.UpdateMoveXY(xdiff, ydiff);
        }
    }
    public CharacterAnimator Animator => animator;
}