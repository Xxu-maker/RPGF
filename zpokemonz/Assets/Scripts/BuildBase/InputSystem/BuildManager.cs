using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BuildManager : MonoBehaviour
{
    [SerializeField] BuildInputSystem buildInputSystem;
    [SerializeField] PlacementSystem placementSystem;
    [SerializeField] MoveObject moveObject;
    [SerializeField] BuildListCanvas buildListCanvas;
    [SerializeField] CameraMovement cameraMovement;

    [Header("控制Toggle")]
    [SerializeField] Toggle objectMoving;
    [SerializeField] Toggle selectObject;
    [SerializeField] Toggle cameraMoving;

    [SerializeField] SpriteRenderer gridSprite;

    public PlacementSystem PlacementSystem => placementSystem;
    public BuildInputSystem BuildInputSystem => buildInputSystem;

    public void Start()
    {
        objectMoving.onValueChanged.AddListener((bool value) => ObjectMovingHandler(value));
        selectObject.onValueChanged.AddListener((bool value) => SelectObject(value));
        cameraMoving.onValueChanged.AddListener((bool value) => CameraMovingHandler(value));
        buildInputSystem.EndInput += OnDragActive;
        ObjectMovingHandler(true);
    }

    /// <summary>
    /// 清除建造输入
    /// </summary>
    /// <param name="nullSet"></param>
    public void ClearInputActions(bool nullSet = false)
    {
        buildInputSystem.n_OnMouseClick = null;
        buildInputSystem.n_OnMouseHold  = null;
        buildInputSystem.n_OnMouseUp    = null;
        buildInputSystem.v_OnMouseHold  = null;
    }

    public void OnDragActive()
    {
        buildListCanvas.gameObject.SetActive(true);
        buildInputSystem.gameObject.SetActive(false);
    }

    public void InputSystemActive()
    {
        if(backOnDragActive == null)
        {
            backOnDragActive = OnDragActive;
        }

        buildListCanvas.gameObject.SetActive(false);
        buildInputSystem.gameObject.SetActive(true);
        if(!gridSprite.enabled)
        {
            gridSprite.enabled = true;
        }
    }

    private event System.Action backOnDragActive;

    /// <summary>
    /// 放置普通建筑
    /// </summary>
    public void ObjectMovingHandler(bool isOn)
    {
        if(isOn)
        {
            ClearInputActions();

            backOnDragActive?.Invoke();

            buildInputSystem.state = BuildingState.Moving;
            buildInputSystem.v_OnMouseHold += placementSystem.MovingObject;
            if(!gridSprite.enabled)
            {
                gridSprite.enabled = true;
            }
        }
    }

    /// <summary>
    /// 选择移动
    /// </summary>
    public void SelectObject(bool isOn)
    {
        if(isOn)
        {
            ClearInputActions();
            InputSystemActive();

            buildInputSystem.state = BuildingState.Select;
            buildInputSystem.n_OnMouseClick += moveObject.RecordTime;
            buildInputSystem.n_OnMouseHold  += moveObject.Select;
            buildInputSystem.n_OnMouseUp    += moveObject.EndInput;
            buildInputSystem.gameObject.SetActive(true);
            buildListCanvas.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 选择物体完成后转移输入
    /// </summary>
    public void BackObjectMovingHandler()
    {
        backOnDragActive = null;
        objectMoving.isOn = true;
    }

    /// <summary>
    /// 摄像机控制
    /// </summary>
    public void CameraMovingHandler(bool isOn)
    {
        if(isOn)
        {
            ClearInputActions();
            InputSystemActive();

            buildInputSystem.state = BuildingState.Camera;
            buildInputSystem.n_OnMouseHold += cameraMovement.MoveCamera;
            if(gridSprite.enabled)
            {
                gridSprite.enabled = false;
            }
        }
    }
}