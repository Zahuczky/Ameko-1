using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AssCS
{
    public class ExtradataManager
    {
        private readonly List<Extradata> extradata;
        private int _id = 0;
        public int NextId
        {
            get => _id++;
            set => _id = value;
        }

        public int Add(string key, string value)
        {
            foreach (var ed in extradata)
                if (ed.Key == key && ed.Value == value)
                    return ed.Id;

            int next = NextId;
            extradata.Add(new Extradata(next, 0, key, value));
            return next;
        }

        public int Add(Extradata e)
        {
            extradata.Add(e);
            return e.Id;
        }

        public List<Extradata> Get(List<int> ids)
        {
            List<Extradata> result = new List<Extradata>();
            return extradata.Where(e =>  ids.Contains(e.Id)).ToList();
        }

        public void SetValue(Event line, string key, string value, bool delete)
        {
            List<int> toErase = new List<int>();
            bool dirty = false;
            bool found = false;

            var entries = Get(line.LinkedExtradatas);
            foreach (var entry in entries)
            {
                if (entry.Key == key)
                {
                    if (!delete && entry.Value == value)
                        found = true;
                } else
                {
                    toErase.Add(entry.Id);
                    dirty = true;
                }
            }

            // The key is already set
            if (found && !dirty) return;

            foreach (var id in toErase)
            {
                line.LinkedExtradatas.Remove(id);
            }

            if (!delete && !found)
                line.LinkedExtradatas.Add(Add(key, value));
        }

        public void Clean(File file)
        {
            if (extradata.Count == 0) return;

            HashSet<int> usedIds = new HashSet<int>();
            foreach (var line in file.EventManager.Ordered)
            {
                if (line.LinkedExtradatas.Count == 0) continue;

                // Find the id for each unique key
                Dictionary<string, int> usedKeys;
                usedKeys = new Dictionary<string, int>(
                    extradata.Where(e => line.LinkedExtradatas.Contains(e.Id)).Select(e => new KeyValuePair<string, int>(e.Key, e.Id))
                );
                usedIds = usedKeys.Values.ToHashSet();

                // Check for duped/missing keys
                if (usedKeys.Count != line.LinkedExtradatas.Count)
                {
                    line.LinkedExtradatas = usedKeys.Select(k => k.Value).ToList();
                    line.LinkedExtradatas.Sort();
                }
            }
            
            foreach (var e in extradata)
            {
                if (!usedIds.Contains(e.Id))
                    e.Expiration = 0;
                else
                    e.Expiration += 1;
            }

            // Erase all unused entries
            if (usedIds.Count != extradata.Count)
                extradata.RemoveAll(e => e.Expiration >= 10);
        }

        public List<Extradata> GetAll()
        {
            return extradata;
        }

        public void Sort()
        {
            extradata.Sort();
        }

        public ExtradataManager()
        {
            extradata = new List<Extradata>();
        }
        public ExtradataManager(File source)
        {
            extradata = new List<Extradata>(source.ExtradataManager.extradata);
        }
    }
}
