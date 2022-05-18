using System.IO;
using Hkmp.Api.Server;
using Hkmp.Networking.Packet;
using System.Threading.Tasks;
using Hkmp.Networking.Packet.Data;

namespace ACHKMP;

public class ACServer:ServerAddon
{
    internal IServerApi _serverApi;
    private FileSystemWatcher _fileSystemWatcher;
    private static readonly string DllDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    private const string SETTINGFILENAME = "ACSettings.json";
    private float EffectUnloadTime = HKMPInfo.DefaultEffectUnloadTime;
    private float KeyPressDownTime = HKMPInfo.DefaultKeyPressDownTime;

    public override void Initialize(IServerApi serverApi)
    {
        _serverApi = serverApi;
        
        var netReceiver = _serverApi.NetServer.GetNetworkReceiver<ToServerPackets>(this,InstantiatePacket);
        var netSender = _serverApi.NetServer.GetNetworkSender<ToClientPackets>(this);
        
        netReceiver.RegisterPacketHandler<ToServerRequestEffect>(ToServerPackets.RequestAnEffectToBeRun,
            RequestEffectToBeRun);
        netReceiver.RegisterPacketHandler<ReliableEmptyData>(ToServerPackets.RequestSettings,
            (id,_) =>
            {
                netSender.SendSingleData(ToClientPackets.SendSettings, new SendSettings()
                {
                    KeyPressDownTime = this.KeyPressDownTime,
                    EffectUnloadTime = this.EffectUnloadTime,
                }, id);
            });
        
        _fileSystemWatcher = new FileSystemWatcher(DllDirectory ?? string.Empty);
        _fileSystemWatcher.IncludeSubdirectories = false;
        _fileSystemWatcher.Changed += OnFileChanged;
        _fileSystemWatcher.Created += OnFileChanged;
        _fileSystemWatcher.Renamed += OnFileChanged;
        _fileSystemWatcher.EnableRaisingEvents = true;
        HandleConfigChange();
    }
    
     private async void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            // just do a best attempt at getting the file before passing it off to readers
            const int maxAttempts = 50;
            var attempts = 0;
            while (IsFileLocked(new FileInfo(e.FullPath)) && attempts <= maxAttempts)
            {
                // We don't want to mess around with half copied files, wait for the lock to end
                await Task.Delay(10);
                attempts++;
            }

            if (attempts > maxAttempts)
            {
                // We'll call this unreadable
                return;
            }

            HandleConfigChange();
        }

         private void HandleConfigChange()
        {
            if (!File.Exists(Path.Combine(DllDirectory, SETTINGFILENAME)))
            {
                return;
            }

            var fileContents = ReadAllTextNoLock(Path.Combine(DllDirectory, SETTINGFILENAME));
            if (fileContents == null)
            {
                return;
            }

            Dictionary<string, float> settings;
            settings = JsonConvert.DeserializeObject<Dictionary<string, float>>(fileContents);
            
            KeyPressDownTime = settings![nameof(KeyPressDownTime)];
            EffectUnloadTime = settings![nameof(EffectUnloadTime)];
            
            if (_serverApi.NetServer is { IsStarted: true })
            {
                var netSender = _serverApi.NetServer.GetNetworkSender<ToClientPackets>(this);

                netSender.BroadcastSingleData(ToClientPackets.SendSettings, new SendSettings()
                {
                    KeyPressDownTime = this.KeyPressDownTime,
                    EffectUnloadTime = this.EffectUnloadTime,
                });
            }
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
            if (packet.targetPlayerId != 60001)
            {
                netSender.SendSingleData(ToClientPackets.RequestAnEffectToBeRun, request, packet.targetPlayerId);
            }
        }
    }
    
    private IPacketData InstantiatePacket(ToServerPackets Serverpackets)
    {
        switch (Serverpackets) 
        {
            case ToServerPackets.RequestAnEffectToBeRun:
                return new ToServerRequestEffect();
            case ToServerPackets.RequestSettings:
                return new ReliableEmptyData();
            default:
                return null;
        }
    }
    
    private static bool IsFileLocked(FileInfo file)
    {
        try
        {
            using (var _ = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None)) { }
        }
        catch (IOException)
        {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        }

        //file is not locked
        return false;
    }
    
    private string ReadAllTextNoLock(string path)
    {
        try
        {
            using (var file = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(file))
            {
                return sr.ReadToEnd();
            }
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected override string Name { get; } = HKMPInfo.Name;
    protected override string Version { get; } = HKMPInfo.Version;
    public override bool NeedsNetwork { get; } = HKMPInfo.NeedsNetwork;
}