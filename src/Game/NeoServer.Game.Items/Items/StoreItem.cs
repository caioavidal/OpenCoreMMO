using NeoServer.Game.Common;
using NeoServer.Game.Contracts;
using NeoServer.Game.Contracts.Bases;
using NeoServer.Game.Contracts.Items;
using NeoServer.Game.Contracts.Items.Types;

namespace NeoServer.Game.Items.Items
{
    //todo: this class is a workaround. need to fix this
    public class ContainerStore : Store
    {
        private readonly IContainer container;
        public ContainerStore(IContainer container)
        {
            this.container = container;
        }

        public override Result<OperationResult<IItem>> AddItem(IItem thing, byte? position = null) => container.AddItem(thing, position);
        public override Result CanAddItem(IItem item, byte amount = 1, byte? slot = null) => container.CanAddItem(item, amount, slot);

        public override bool CanRemoveItem(IItem item) => container.CanRemoveItem(item);

        public override int PossibleAmountToAdd(IItem thing, byte? toPosition = null) => container.PossibleAmountToAdd(thing, toPosition);

        public override Result<OperationResult<IItem>> ReceiveFrom(IStore source, IItem thing, byte? toPosition) => base.ReceiveFrom(source, thing, toPosition);


        public override Result<OperationResult<IItem>> RemoveItem(IItem thing, byte amount, byte fromPosition, out IItem removedThing) => container.RemoveItem(thing, amount, fromPosition, out removedThing);

        public override Result<OperationResult<IItem>> SendTo(IStore destination, IItem thing, byte amount, byte fromPosition, byte? toPosition) => base.SendTo(destination, thing, amount, fromPosition, toPosition);

    }
}
