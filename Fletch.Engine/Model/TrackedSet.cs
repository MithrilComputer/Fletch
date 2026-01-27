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
    }
}
