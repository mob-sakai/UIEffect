using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

namespace Coffee.UIEffectInternal
{
    internal class ObjectRepository<T> where T : Object
    {
        private readonly Dictionary<Hash128, Entry> _cache = new Dictionary<Hash128, Entry>(8);
        private readonly Dictionary<int, Hash128> _objectKey = new Dictionary<int, Hash128>(8);
        private readonly string _name;
        private readonly Action<T> _onRelease;
        private readonly Stack<Entry> _pool = new Stack<Entry>(8);

        public ObjectRepository(Action<T> onRelease = null)
        {
            _name = $"{typeof(T).Name}Repository";
            if (onRelease == null)
            {
                _onRelease = x =>
                {
#if UNITY_EDITOR
                    if (!Application.isPlaying)
                    {
                        Object.DestroyImmediate(x, false);
                    }
                    else
#endif
                    {
                        Object.Destroy(x);
                    }
                };
            }
            else
            {
                _onRelease = onRelease;
            }

            for (var i = 0; i < 8; i++)
            {
                _pool.Push(new Entry());
            }
        }

        public int count => _cache.Count;

        public void Clear()
        {
            foreach (var kv in _cache)
            {
                var entry = kv.Value;
                if (entry == null) continue;

                entry.Release(_onRelease);
                _pool.Push(entry);
            }

            _cache.Clear();
            _objectKey.Clear();
        }

        public bool Valid(Hash128 hash, T obj)
        {
            return _cache.TryGetValue(hash, out var entry) && entry.storedObject == obj;
        }

        /// <summary>
        /// Adds or retrieves a cached object based on the hash.
        /// </summary>
        public void Get(Hash128 hash, ref T obj, Func<T> onCreate)
        {
            if (GetFromCache(hash, ref obj)) return;
            Add(hash, ref obj, onCreate());
        }

        /// <summary>
        /// Adds or retrieves a cached object based on the hash.
        /// </summary>
        public void Get<TS>(Hash128 hash, ref T obj, Func<TS, T> onCreate, TS source)
        {
            if (GetFromCache(hash, ref obj)) return;
            Add(hash, ref obj, onCreate(source));
        }

        private bool GetFromCache(Hash128 hash, ref T obj)
        {
            // Find existing entry.
            Profiler.BeginSample("(COF)[ObjectRepository] GetFromCache");
            if (_cache.TryGetValue(hash, out var entry))
            {
                if (!entry.storedObject)
                {
                    Release(ref entry.storedObject);
                    Profiler.EndSample();
                    return false;
                }

                if (entry.storedObject != obj)
                {
                    // if the object is different, release the old one.
                    Release(ref obj);
                    ++entry.reference;
                    obj = entry.storedObject;
                    Logging.Log(_name, $"Get(total#{count}): {entry}");
                }

                Profiler.EndSample();
                return true;
            }

            Profiler.EndSample();
            return false;
        }

        private void Add(Hash128 hash, ref T obj, T newObject)
        {
            if (!newObject)
            {
                Release(ref obj);
                obj = newObject;
                return;
            }

            // Create and add a new entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Add");
            var newEntry = 0 < _pool.Count ? _pool.Pop() : new Entry();
            newEntry.storedObject = newObject;
            newEntry.hash = hash;
            newEntry.reference = 1;
            _cache[hash] = newEntry;
            _objectKey[newObject.GetInstanceID()] = hash;
            Logging.Log(_name, $"<color=#03c700>Add</color>(total#{count}): {newEntry}");
            Release(ref obj);
            obj = newObject;
            Profiler.EndSample();
        }

        /// <summary>
        /// Release a object.
        /// </summary>
        public void Release(ref T obj)
        {
            if (ReferenceEquals(obj, null)) return;

            // Find and release the entry.
            Profiler.BeginSample("(COF)[ObjectRepository] Release");
            var id = obj.GetInstanceID();
            if (_objectKey.TryGetValue(id, out var hash)
                && _cache.TryGetValue(hash, out var entry))
            {
                entry.reference--;
                if (entry.reference <= 0 || !entry.storedObject)
                {
                    Remove(entry);
                }
                else
                {
                    Logging.Log(_name, $"Release(total#{_cache.Count}): {entry}");
                }
            }
            else
            {
                Logging.Log(_name, $"Release(total#{_cache.Count}): <color=red>Already released: {obj}</color>");
            }

            obj = null;
            Profiler.EndSample();
        }

        private void Remove(Entry entry)
        {
            if (ReferenceEquals(entry, null)) return;

            Profiler.BeginSample("(COF)[ObjectRepository] Remove");
            _cache.Remove(entry.hash);
            _objectKey.Remove(entry.storedObject.GetInstanceID());
            _pool.Push(entry);
            entry.reference = 0;
            Logging.Log(_name, $"<color=#f29e03>Remove</color>(total#{_cache.Count}): {entry}");
            entry.Release(_onRelease);
            Profiler.EndSample();
        }

        private class Entry
        {
            public Hash128 hash;
            public int reference;
            public T storedObject;

            public void Release(Action<T> onRelease)
            {
                reference = 0;
                if (storedObject)
                {
                    onRelease?.Invoke(storedObject);
                }

                storedObject = null;
            }

            public override string ToString()
            {
                return $"h{(uint)hash.GetHashCode()} (refs#{reference}), {storedObject}";
            }
        }
    }
}
