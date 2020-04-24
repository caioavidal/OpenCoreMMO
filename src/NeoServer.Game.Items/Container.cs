//using NeoServer.Game.Contracts;
//using NeoServer.Game.Contracts.Items;
//using NeoServer.Game.Enums;
//using NeoServer.Game.Enums.Location.Structs;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace NeoServer.Game.Items
//{
//    public class Container : Item, IContainer
//    {
//        public event OnContentAdded OnContentAdded;

//        public event OnContentUpdated OnContentUpdated;

//        public event OnContentRemoved OnContentRemoved;

//        public IList<IItem> Content { get; }

//        public Dictionary<uint, byte> OpenedBy { get; }

//        private readonly object openedByLock;

//        public byte Volume => Convert.ToByte(Attributes.ContainsKey(ItemAttribute.Capacity) ? Attributes[ItemAttribute.Capacity] : 0x08);

//        public new Location Location => Parent?.Location ?? base.Location;

//        public Container(IItemType type)
//            : base(type)
//        {
//            Content = new List<IItem>();
//            OpenedBy = new Dictionary<uint, byte>();
//            openedByLock = new object();

//            //todo
//            //OnContentUpdated += Game.Instance.OnContainerContentUpdated;
//            //OnContentAdded += Game.Instance.OnContainerContentAdded;
//            //OnContentRemoved += Game.Instance.OnContainerContentRemoved;
//        }

//        // ~Container()
//        // {
//        //    OnContentUpdated -= Game.Instance.OnContainerContentUpdated;
//        //    OnContentAdded -= Game.Instance.OnContainerContentAdded;
//        //    OnContentRemoved -= Game.Instance.OnContainerContentRemoved;
//        // }
//        public override void AddContent(IEnumerable<object> contentObjs)
//        {
//            if (contentObjs == null)
//            {
//                throw new ArgumentNullException(nameof(contentObjs));
//            }

//            var content = contentObjs.Cast<CipElement>();

//            foreach (var element in content)
//            {
//                if (element.Data < 0)
//                {
//                    // this is a flag an is unexpected.
//                    // TODO: proper logging.
//                    //if (!ServerConfiguration.SupressInvalidItemWarnings)
//                    //{
//                    //    Console.WriteLine($"Container.AddContent: Unexpected flag {element.Attributes?.First()?.Name}, ignoring.");
//                    //}

//                    continue;
//                }

//                try
//                {
//                    var item = ItemFactory.Create((ushort)element.Data) as IItem;

//                    if (item == null)
//                    {
//                        //todo
//                        //if (!ServerConfiguration.SupressInvalidItemWarnings)
//                        //{
//                        //    Console.WriteLine($"Container.AddContent: Item with id {element.Data} not found in the catalog, skipping.");
//                        //}

//                        continue;
//                    }

//                    // TODO: this is hacky.
//                    ((Item)item).AddAttributes(element.Attributes);

//                    AddContent(item, 0xFF);
//                }
//                catch (ArgumentException)
//                {
//                    // TODO: proper logging.
//                    //if (!ServerConfiguration.SupressInvalidItemWarnings)
//                    //{
//                    //    Console.WriteLine($"Item.AddContent: Invalid item {element.Data} in item contents, skipping.");
//                    //}
//                }
//            }
//        }

//        /// <summary>
//        /// Attempts to add the joined item to this container's content at the default index.
//        /// </summary>
//        /// <param name="otherItem">The item to add.</param>
//        /// <returns>True if the operation was successful, false otherwise</returns>
//        public override bool Join(IItem otherItem)
//        {
//            if (Content.Count >= Volume)
//            {
//                // we can't add the additional item, since we're at capacity.
//                return false;
//            }

//            otherItem.SetParent(this);
//            Content.Add(otherItem);

//            OnContentAdded?.Invoke(this, otherItem);

//            return true;
//        }

//        public bool AddContent(IItem item, byte index)
//        {
//            if (item == null)
//            {
//                throw new ArgumentNullException(nameof(item));
//            }

//            // Validate that the item being added is not a parent of this item.
//            if (IsChildOf(item))
//            {
//                return false;
//            }

//            try
//            {
//                var existingItem = Content[Content.Count - index - 1];

//                if (existingItem != null)
//                {
//                    var joinResult = existingItem.Join(item);

//                    OnContentUpdated?.Invoke(this, index, existingItem);

//                    if (joinResult)
//                    {
//                        return true;
//                    }

//                    // attempt to add to the parent of this item.
//                    if (existingItem.Parent != null && existingItem.Parent.Join(item))
//                    {
//                        return true;
//                    }
//                }
//            }
//            catch
//            {
//                // ignored
//            }

//            return Join(item);
//        }

//        public bool RemoveContent(ushort itemId, byte index, byte count, out IItem splitItem)
//        {
//            splitItem = null;

//            // see if item at index is cummulative, and if it is the same type we're adding.
//            IItem existingItem = null;

//            try
//            {
//                existingItem = Content[Content.Count - index - 1];
//            }
//            catch
//            {
//                // ignored
//            }

//            if (existingItem == null || existingItem.Type.TypeId != itemId || existingItem.Count < count)
//            {
//                return false;
//            }

//            var separateResult = existingItem.Separate(count, out splitItem);

//            if (separateResult)
//            {
//                if (existingItem.Amount == 0)
//                {
//                    existingItem.SetAmount(count); // restore the "removed" count, since we removed "all" of the item.

//                    existingItem.SetParent(null);
//                    Content.RemoveAt(Content.Count - index - 1);
//                    OnContentRemoved?.Invoke(this, index);
//                }
//                else
//                {
//                    OnContentUpdated?.Invoke(this, index, existingItem);
//                }
//            }

//            return separateResult;
//        }

//        /// <summary>
//        /// Gets the Count of the item in the specified index of the <see cref="Content"/> of this container.
//        /// </summary>
//        /// <param name="fromIndex">The index to check at this container's <see cref="Container.Content"/>.</param>
//        /// <param name="itemIdExpected">The id of the item to expect at that index.</param>
//        /// <returns>Returns an <see cref="sbyte"/> with a value between 1 and 100 with the Count of the item at the index <paramref name="fromIndex"/>, 0 if the item does not match the type of the <paramref name="itemIdExpected"/>, or -1 if there is no item at that index.</returns>
//        public sbyte CountContentAmountAt(byte fromIndex, ushort itemIdExpected = 0)
//        {
//            IItem existingItem = null;

//            try
//            {
//                existingItem = Content[Content.Count - fromIndex - 1];
//            }
//            catch
//            {
//                // ignored
//            }

//            if (existingItem == null)
//            {
//                return -1;
//            }

//            if (existingItem.Type.TypeId != itemIdExpected)
//            {
//                return 0;
//            }

//            return (sbyte)Math.Min(existingItem.Count, (byte)100);
//        }

//        /// <summary>
//        /// Adds the supplied creature id and container id to this container's tracking list of openers.
//        /// </summary>
//        /// <param name="creatureOpeningId">The id of the creature opening this container.</param>
//        /// <param name="containerId">The id of this container in creature's view.</param>
//        /// <returns>The containerId that this container knows for this creature.</returns>
//        /// <remarks>The id returned may not match the one supplied if the container was already opened by this creature before.</remarks>
//        public byte Open(uint creatureOpeningId, byte containerId)
//        {
//            lock (openedByLock)
//            {
//                if (!OpenedBy.ContainsKey(creatureOpeningId))
//                {
//                    OpenedBy.Add(creatureOpeningId, containerId);
//                }

//                return OpenedBy[creatureOpeningId];
//            }
//        }

//        /// <summary>
//        /// Removes the supplied <see cref="ICreature"/> from this container's tracking list of openers.
//        /// </summary>
//        /// <param name="creatureClosingId">The id of the creature closing this container.</param>
//        public void Close(uint creatureClosingId)
//        {
//            lock (openedByLock)
//            {
//                if (OpenedBy.ContainsKey(creatureClosingId))
//                {
//                    OpenedBy.Remove(creatureClosingId);
//                }
//            }
//        }

//        /// <summary>
//        /// Gets the current id known for this container to a supplied creature.
//        /// </summary>
//        /// <param name="creatureId">The id of the creature to check for.</param>
//        /// <returns>A non-negative number if an id was found, -1 otherwise.</returns>
//        public sbyte GetIdFor(uint creatureId)
//        {
//            lock (openedByLock)
//            {
//                if (OpenedBy.ContainsKey(creatureId))
//                {
//                    return (sbyte)OpenedBy[creatureId];
//                }
//            }

//            return -1;
//        }

//        private bool IsChildOf(IItem item)
//        {
//            var itemAsContainer = item as IContainer;

//            while (itemAsContainer != null)
//            {
//                if (this == itemAsContainer)
//                {
//                    return true;
//                }

//                itemAsContainer = itemAsContainer.Parent;
//            }

//            return false;
//        }
//    }
//}
