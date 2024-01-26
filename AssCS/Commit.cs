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
        public T Snapshot { get; }

        public Commit(T snapshot)
        {
            Id = Guid.NewGuid();
            Snapshot = snapshot;
        }
    }

    public interface ICommitable
    {
    }
}
