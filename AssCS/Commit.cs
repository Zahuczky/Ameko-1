using System;
using System.Collections.Generic;
using System.Text;

namespace AssCS
{
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
