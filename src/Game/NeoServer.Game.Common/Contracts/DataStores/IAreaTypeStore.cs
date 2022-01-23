using System.Reflection;

namespace NeoServer.Game.Common.Contracts.DataStores;

public interface IAreaTypeStore
{
    void Add(string name, FieldInfo area);
    byte[,] Get(string name);
}