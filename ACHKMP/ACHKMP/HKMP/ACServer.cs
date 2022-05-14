using Hkmp.Api.Server;
using Hkmp.Networking.Packet;

namespace ACHKMP;

public class ACServer:ServerAddon
{
    internal IServerApi _serverApi;
    public override void Initialize(IServerApi serverApi)
    {
        _serverApi = serverApi;
        
        var netReceiver = _serverApi.NetServer.GetNetworkReceiver<ToServerPackets>(this,InstantiatePacket);
        var netSender = _serverApi.NetServer.GetNetworkSender<ToClientPackets>(this);
        
        netReceiver.RegisterPacketHandler<ToServerRequestEffect>(ToServerPackets.RequestAnEffectToBeRun,
            RequestEffectToBeRun);
    }

    private void RequestEffectToBeRun(ushort fromPlayerid, ToServerRequestEffect packet)
    {
        var netSender = _serverApi.NetServer.GetNetworkSender<ToClientPackets>(this);

        var request = new ToClientRequestEffect
        {
            effectName = packet.effectName
        };
        
        if (packet.targetEveryone)
        {
            netSender.BroadcastSingleData(ToClientPackets.RequestAnEffectToBeRun, request);
        }
        else if (!packet.targetEveryone)
        {
            netSender.SendSingleData(ToClientPackets.RequestAnEffectToBeRun, request, packet.targetPlayerId);
        }
    }
    
    private IPacketData InstantiatePacket(ToServerPackets Serverpackets)
    {
        switch (Serverpackets) 
        {
            case ToServerPackets.RequestAnEffectToBeRun:
                return new ToServerRequestEffect();
            default:
                return null;
        }
    }

    protected override string Name { get; } = HKMPInfo.Name;
    protected override string Version { get; } = HKMPInfo.Version;
    public override bool NeedsNetwork { get; } = HKMPInfo.NeedsNetwork;
}