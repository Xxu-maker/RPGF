using UnityEngine;
public class IVItem : MonoBehaviour
{
    [Header("个体值")]
    [SerializeField] bool all;
    public void UseIVItem(Pokemon pokemon, int i = 0)
    {
        if(all)
        {
            //全31
        }
        else
        {
            //i项31
        }
    }
}