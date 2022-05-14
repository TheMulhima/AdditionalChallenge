namespace ACHKMP;

public class ToClientRequestEffect:IPacketData
{
    public string effectName { get; set; }
    public void WriteData(IPacket packet)
    {
        packet.Write(effectName);
    }

    public void ReadData(IPacket packet)
    {
        effectName = packet.ReadString();
    }

    public bool IsReliable { get; } = true;
    public bool DropReliableDataIfNewerExists { get; } = true;
}

public class ToServerRequestEffect:IPacketData
{
    public bool targetEveryone { get; set; }
    public ushort targetPlayerId { get; set; } //will be ignored if targetEveryone set to true
    public string effectName { get; set; }

    public void WriteData(IPacket packet)
    {
        packet.Write(targetEveryone);
        packet.Write(targetPlayerId);
        packet.Write(effectName);
    }

    public void ReadData(IPacket packet)
    {
        targetEveryone = packet.ReadBool();
        targetPlayerId = packet.ReadUShort();
        effectName = packet.ReadString();
    }

    public bool IsReliable { get; } = true;
    public bool DropReliableDataIfNewerExists { get; } = true;
}

public enum ToServerPackets
{
    RequestAnEffectToBeRun,
}
public enum ToClientPackets
{ 
    RequestAnEffectToBeRun,
}