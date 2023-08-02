using System.Diagnostics;
using UnityEngine;
public class Ztest : MonoBehaviour
{
    public void testzzz()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start(); //开始测量
        sw.Stop();
        print($"switch{sw.ElapsedTicks}");
    }
}