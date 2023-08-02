using UnityEngine;
namespace ZUI.BagToggle
{
    public class BagToggleGroup : MonoBehaviour
    {
        [SerializeField] BagToggle[] toggles;
        private int current = 1;

        public void SwitchCurrentToggle(int n)
        {
            if(n != current)
            {
                toggles[current].OnClose();
                current = n;
                toggles[current].OnOpen();
            }
        }

        public void Reset()
        {
            toggles[current].OnClose();
            current = 1;
        }
    }
}