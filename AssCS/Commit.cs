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
        public List<T> Snapshot { get; }
        public Action Action { get; }

        public Commit(List<T> snapshot, Action action)
        {
            Id = Guid.NewGuid();
            Snapshot = snapshot;
            Action = action;
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
