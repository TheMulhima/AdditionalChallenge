namespace ACHKMP;

public class ACClient:ClientAddon
{
    public IClientApi _clientApi;
    public override void Initialize(IClientApi clientApi)
    {
        _clientApi = clientApi;
        
        _clientApi.CommandManager.RegisterCommand(new SendEffectCommand());
        
        var netReceiver = _clientApi.NetClient.GetNetworkReceiver<ToClientPackets>(this, InstantiatePacket);
        var netSender = _clientApi.NetClient.GetNetworkSender<ToServerPackets>(this);

        netReceiver.RegisterPacketHandler<ToClientRequestEffect>(
            ToClientPackets.RequestAnEffectToBeRun,
            RequestAnEffectToBeRun);

        _clientApi.ClientManager.ConnectEvent += () => ModMenu.HKMPMenu.Update();

    }

    private void RequestAnEffectToBeRun(ToClientRequestEffect packet)
    {
        //effectName is parsed and then sent i hope
        AbstractEffects effect;
        try
        {
            effect = AdditionalChallenge.AdditionalChallenge.AllEffects.Find(eff => eff.name == packet.effectName);
        }
        catch (ArgumentNullException e)
        {
            ACHKMP.Instance.LogError(e);
            return;
        }
        
        effect.Load();
        switch (effect)
        {
            case AbstractCoolDownEffect coolDownEffect:
                coolDownEffect.DoEffect();
                break;
            case AbstractBossAttack bossAttack:
                bossAttack.Attack();
                break;
        }

        AdditionalChallenge.AdditionalChallenge.CoroutineSlave.StartCoroutine(UnloadEffectEventually(effect));
    }

    private IEnumerator UnloadEffectEventually(AbstractEffects effect)
    {
        yield return new WaitForSeconds(30f);
        effect.Unload();
    }
    

    private static IPacketData InstantiatePacket(ToClientPackets Clientpackets)
    {
        switch (Clientpackets)
        {
            case ToClientPackets.RequestAnEffectToBeRun:
                return new ToClientRequestEffect();
            default:
                ACHKMP.Instance.LogError($"{Clientpackets} not found");
                return null;
        }
    }

    protected override string Name { get; } = HKMPInfo.Name;
    protected override string Version { get; } = HKMPInfo.Version;
    public override bool NeedsNetwork { get; } = HKMPInfo.NeedsNetwork;
}