﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace NetMFAPatcher.Utils
{
    public class BitDict
    {
        private String[] Keys;
        public UInt32 flag { get; set; }

        public BitDict(String[] keys)
        {
            Keys = keys;
        }

        public bool this[String key]
        {
            get => GetFlag(key);
        }

        public bool GetFlag(String key)
        {
            Int32 pos = Array.IndexOf(Keys, key);
            if (pos >= 0)
            {
                return (flag & ((UInt32) Math.Pow(2, pos))) != 0;
            }

            return false;
        }

        public static string ToDebugString<TKey, TValue>(IDictionary<TKey, TValue> dictionary)
        {
            return  string.Join(";", dictionary.Select(kv => kv.Key + "=" + kv.Value).ToArray());
        }

        public override string ToString()
        {
            Dictionary<String, bool> actualKeys = new Dictionary<string, bool>();
            foreach (var key in Keys)
            {
                actualKeys[key] = this[key];
            }

            return ToDebugString(actualKeys);
        }
    }
}