using System.Collections.Generic;
using UnityEngine;

internal static class YieldInstructionCache
{
    public static readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();
    public static readonly WaitForFixedUpdate waitForFixedUpdate = new WaitForFixedUpdate();
    private static readonly Dictionary<float, WaitForSeconds> waitForSeconds = new Dictionary<float, WaitForSeconds>();
    // Dictionary는 Map<key, value>:Tree 과 유사

    public static WaitForSeconds WaitForSeconds(float seconds)
    {
        WaitForSeconds wfs;
        if (!waitForSeconds.TryGetValue(seconds, out wfs))
            waitForSeconds.Add(seconds, wfs = new WaitForSeconds(seconds));
        return wfs;
    }
}
