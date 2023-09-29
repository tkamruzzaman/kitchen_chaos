//https://docs-multiplayer.unity3d.com/netcode/current/components/networkanimator/

using Unity.Netcode.Components;

public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}