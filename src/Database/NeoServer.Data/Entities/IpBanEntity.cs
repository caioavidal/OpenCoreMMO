using System;

namespace NeoServer.Data.Entities;

public class IpBanEntity
{
    public uint Ip { get; set; }
    public string Reason { get; set; }
    public TimeSpan BannedAt { get; set; }
    public TimeSpan ExpiresAt { get; set; }
    public ushort BannedBy { get; set; }
}