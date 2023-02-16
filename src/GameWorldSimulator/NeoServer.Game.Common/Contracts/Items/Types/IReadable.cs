using System;
using NeoServer.Game.Common.Contracts.Creatures;
using NeoServer.Game.Common.Results;

namespace NeoServer.Game.Common.Contracts.Items.Types;

public interface IReadable : IItem
{
    string Text { get; }
    bool CanWrite { get; }
    ushort MaxLength { get; }
    string WrittenBy { get; } //todo: change to id and then query the database to get the current name
    DateTime? WrittenOn { get; set; }
    Result Write(string text, IPlayer writtenBy);
}