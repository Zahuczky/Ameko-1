using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
    /// <summary>
    /// A Commit is a snapshot of history.
    /// </summary>
    /// <typeparam name="T">Type being committed</typeparam>
    public class Commit<T> where T : ICommitable
    {
        public Guid Id { get; }
        public List<Snapshot<T>> Snapshots { get; }

        public Commit(List<Snapshot<T>> snapshots)
        {
            Id = Guid.NewGuid();
            Snapshots = snapshots;
        }
    }

    public class Snapshot<T> where T : ICommitable
    {
        public readonly List<T> snapshot;
        public readonly Action action;

        public Snapshot(List<T> snapshot, Action action)
        {
            this.snapshot = snapshot;
            this.action = action;
        }
    }

    public enum Action
    {
        EDIT,
        INSERT,
        DELETE
    }

    public interface ICommitable
    {
    }
}
