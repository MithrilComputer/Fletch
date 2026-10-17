namespace Fletch.Engine.Model
{
    internal sealed class TrackedSet<T> where T : class
    {
        private readonly List<T> items = new List<T>();
        private readonly HashSet<T> set = new HashSet<T>();
        private readonly HashSet<T> pendingAdds = new HashSet<T>();
        private readonly HashSet<T> pendingRemoves = new HashSet<T>();

        /// <summary>
        /// The current items in the tracked set. Do not modify this list directly; use MarkToAdd and MarkToRemove instead.
        /// </summary>
        public List<T> Items => items;

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
            pendingAdds.Add(item);
        }

        /// <summary>
        /// Marks an item for removal from the tracked set.
        /// </summary>
        public void MarkToRemove(T item)
        {
            if (item == null) return;
            if (!set.Remove(item)) return;
            pendingRemoves.Add(item);
        }

        /// <summary>
        /// Refreshes the tracked set by applying all pending additions and removals.
        /// </summary>
        public void Refresh()
        {
            foreach (T item in pendingAdds)
                items.Add(item);

            foreach (T item in pendingRemoves)
                items.Remove(item);

            pendingAdds.Clear();
            pendingRemoves.Clear();
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

        public void RemoveFromSet(T item)
        {
            if (items.Remove(item))
                return;

            if (pendingAdds.Remove(item))
                return;

            if (pendingRemoves.Remove(item))
                return;
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
