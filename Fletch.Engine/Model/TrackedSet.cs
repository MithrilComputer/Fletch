namespace Fletch.Engine.Model
{
    internal sealed class TrackedSet<T> where T : class
    {
        private readonly List<T> items = new List<T>();
        private readonly HashSet<T> set = new HashSet<T>();
        private readonly Queue<T> pendingAdds = new Queue<T>();
        private readonly Queue<T> pendingRemoves = new Queue<T>();

        /// <summary>
        /// The current items in the tracked set.
        /// </summary>
        public IReadOnlyList<T> Items => items;

        /// <summary>
        /// The Items that will be removed on refresh.
        /// </summary>
        public IReadOnlyList<T> PendingRemoves => pendingRemoves.ToList();

        /// <summary>
        /// The items that will be added on refresh.
        /// </summary>
        public IReadOnlyList<T> PendingAdds => pendingAdds.ToList();

        /// <summary>
        /// Marks an item for addition to the tracked set.
        /// </summary>
        public void MarkToAdd(T item)
        {
            if (item == null) return;
            if (!set.Add(item)) return;
            pendingAdds.Enqueue(item);
        }

        /// <summary>
        /// Marks an item for removal from the tracked set.
        /// </summary>
        public void MarkToRemove(T item)
        {
            if (item == null) return;
            if (!set.Remove(item)) return;
            pendingRemoves.Enqueue(item);
        }

        /// <summary>
        /// Refreshes the tracked set by applying all pending additions and removals.
        /// </summary>
        public void Refresh()
        {
            while (pendingAdds.Count > 0)
                items.Add(pendingAdds.Dequeue());

            while (pendingRemoves.Count > 0)
                items.Remove(pendingRemoves.Dequeue());
        }

        /// <summary>
        /// Sorts the internal list of items from a comparison method.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public void Sort(Comparison<T> comparison)
        {
            if (comparison == null) throw new ArgumentNullException(nameof(comparison));
            items.Sort(comparison);
        }

        // Clears the collections
        public void Clear()
        {
            items.Clear();
            pendingAdds.Clear();
            pendingRemoves.Clear();
            set.Clear();
        }
    }
}
