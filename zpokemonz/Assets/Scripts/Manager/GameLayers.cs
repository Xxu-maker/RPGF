using UnityEngine;
public class GameLayers : SingletonMono<GameLayers>
{
    [SerializeField] LayerMask buildLayer;
	[SerializeField] LayerMask npcLayer;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask followLayer;
    [SerializeField] LayerMask fovLayer;
	[SerializeField] LayerMask grassLayer;
    [SerializeField] LayerMask portalLayer;
    public LayerMask NpcLayer => npcLayer;
    public LayerMask PlayerLayer => playerLayer;
    public LayerMask BuildingLayer => buildLayer;
    public LayerMask TalkLayer => npcLayer | followLayer;
    public LayerMask PathClearLayers => buildLayer | playerLayer;// | npcLayer ;
    public LayerMask TriggerableLayers => grassLayer | fovLayer | portalLayer;
}