using Hkmp.Networking.Packet.Data;
using JetBrains.Annotations;

namespace ACHKMP;

public class ACClient:ClientAddon
{
    public IClientApi _clientApi;
    public override void Initialize(IClientApi clientApi)
    {
        _clientApi = clientApi;
        
        //_clientApi.CommandManager.RegisterCommand(new SendEffectCommand());
        
        var netReceiver = _clientApi.NetClient.GetNetworkReceiver<ToClientPackets>(this, InstantiatePacket);
        var netSender = _clientApi.NetClient.GetNetworkSender<ToServerPackets>(this);

        netReceiver.RegisterPacketHandler<ToClientRequestEffect>(
            ToClientPackets.RequestAnEffectToBeRun,
            RequestAnEffectToBeRun);
        
        netReceiver.RegisterPacketHandler<SendSettings>(
            ToClientPackets.SendSettings,
            (packet) =>
            {
                Modding.Logger.Log("Received new settings");
                ACHKMP.Instance.EffectUnloadTime = packet.EffectUnloadTime;
                ACHKMP.Instance.KeyPressDownTime = packet.KeyPressDownTime;
                Modding.Logger.Log($"{ACHKMP.Instance.EffectUnloadTime}, {ACHKMP.Instance.KeyPressDownTime}");
            });

        _clientApi.ClientManager.ConnectEvent += () => ModMenu.HKMPMenu.Update();
        _clientApi.ClientManager.ConnectEvent += () =>
        {
            netSender.SendSingleData(ToServerPackets.RequestSettings, new ReliableEmptyData());
        };

    }

    public void SendRequest(ToServerRequestEffect packet)
    {
        var netSender = _clientApi.NetClient.GetNetworkSender<ToServerPackets>(this);
        netSender.SendSingleData(ToServerPackets.RequestAnEffectToBeRun, packet);
    }

    private void RequestAnEffectToBeRun(ToClientRequestEffect packet)
    {
        Logger.Warn(this,$"{packet.effectName}");
        //effectName is parsed and then sent i hope
        AbstractEffects effect;
        try
        {
            effect = AdditionalChallenge.AdditionalChallenge.AllEffects.First(eff => eff.ToggleName == packet.effectName);
        }
        catch (InvalidOperationException e)
        {
            ACHKMP.Instance.LogError(e);
            return;
        }
        
        switch (effect)
        {
            case AbstractCoolDownEffect coolDownEffect:
                coolDownEffect.DoEffect();
                break;
            case AbstractBossAttack bossAttack:
                bossAttack.Attack();
                break;
            case AbstractPersistentEffect persistentEffect:
                StartNonCoolDownEffect(persistentEffect);
                break;
            
            case AbstractEnemyFollow enemyFollow:
                StartNonCoolDownEffect(enemyFollow);
                break;
        }
    }

    private void StartNonCoolDownEffect(AbstractEffects effect)
    {
        IEnumerator UnloadEffectEventually(AbstractEffects ToUnloadeffect)
        {
            yield return new WaitForSeconds(ACHKMP.Instance.EffectUnloadTime);
            ToUnloadeffect.Unload();
        }
        
        effect.Load();
        AdditionalChallenge.AdditionalChallenge.CoroutineSlave.StartCoroutine(UnloadEffectEventually(effect));
    }

    private static IPacketData InstantiatePacket(ToClientPackets Clientpackets)
    {
        switch (Clientpackets)
        {
            case ToClientPackets.RequestAnEffectToBeRun:
                return new ToClientRequestEffect();
            case ToClientPackets.SendSettings:
                return new SendSettings();
            default:
                ACHKMP.Instance.LogError($"{Clientpackets} not found");
                return null;
        }
    }

    protected override string Name { get; } = HKMPInfo.Name;
    protected override string Version { get; } = HKMPInfo.Version;
    public override bool NeedsNetwork { get; } = HKMPInfo.NeedsNetwork;
}