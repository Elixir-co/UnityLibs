﻿namespace BasicExtends {
    using System.Linq;
    using System.Text;
    using System.Collections.Generic;
    using System;
    using UnityEngine;

    public class IntPair: Pair<int, int> { }
    public class StringPair: Pair<string, string> { }
    public class CheckedRet<T>: Pair<bool, T>  {
        private static CheckedRet<T>  mFail  = new CheckedRet<T>();
        public new CheckedRet<T> Set(bool flg,T obj ) { Key = flg; Value = obj; return this; }
        public static CheckedRet<T> Fail () { mFail.Key = false; return mFail; }
        public static CheckedRet<T> Succeed () { return new CheckedRet<T>(); }
        public static new CheckedRet<T> Gen ( bool flg, T obj ) { return new CheckedRet<T>().Set(flg,obj); }
        public bool Check { get { return mKey; } }
    }

    public static class StPair {
        public static Dictionary<K, V> GenDic<K, V>
            ( this Pair<K, V> a )
            where V : class {
            var dic = new Dictionary<K, V>();
            dic.TrySet(a.Key, a.Value);
            return dic;
        }

        public static Dictionary<K, V> ToDic<K, V>
            ( this List<Pair<K, V>> list )
            where V : class {
            var dic = new Dictionary<K, V>();
            foreach (var a in list) {
                dic.TrySet(a.Key, a.Value);
            }
            return dic;
        }

        public static Pair<K, V> Gen<K, V> ( K k, V v )
            where V : class {
            return new Pair<K, V>().Set(k, v);
        }

        public static List<Pair<K, V>> Add<K, V> ( this List<Pair<K, V>> p, K k, V v )
            where V : class {
            p.Add(Gen(k, v));
            return p;
        }


        public static List<Pair<K, V>> ToList<K, V> ( this Pair<K, V> p )
            where V : class {
            List<Pair<K, V>> arr = new List<Pair<K, V>> { p };
            return arr;
        }

        public static List<Pair<K, V>> ToList<K, V> ( this Pair<K, V> [] arr )
            where V : class {
            if (arr.Length == 0) { return new List<Pair<K, V>>(); }
            if (arr.Length == 1) { return new List<Pair<K, V>> { arr [0] }; }

            var ret = new List<Pair<K, V>> { arr [0], arr [1] };
            foreach (var i in Enumerable.Range(2, arr.Length)) {
                ret.Add(arr [i]);
            }
            return ret;
        }
    }


    [Serializable]
    public class Pair<K, V>: IDictionaryDataConvertable<K, V>, IJsonable {

        [SerializeField]
        protected K mKey;
        [SerializeField]
        protected V mValue;
        public Pair () { }

        public K Key { set { mKey = value; } get { return mKey; } }
        public V Value { set { mValue = value; } get { return mValue; } }

        public K W { set { mKey = value; } get { return mKey; } }
        public V H { set { mValue = value; } get { return mValue; } }

        public K L { set { mKey = value; } get { return mKey; } }
        public V R { set { mValue = value; } get { return mValue; } }

        public K X { set { mKey = value; } get { return mKey; } }
        public V Y { set { mValue = value; } get { return mValue; } }

        public K Min { set { mKey = value; } get { return mKey; } }
        public V Max { set { mValue = value; } get { return mValue; } }

        public virtual Pair<K, V> Set ( K key, V value ) {
            mKey = key;
            mValue = value;
            return this;
        }

        public static Pair<K, V> Gen (  ) {
            return new Pair<K, V>();
        }
        public static Pair<K, V> Gen ( K key, V value ) {
            return new Pair<K, V>().Set(key, value);
        }


        public Dictionary<K, V> ToDic () {
            return new Dictionary<K, V> { { mKey, mValue } };
        }

        public string ToJson () {
            return Stringify("{", ",", "}");
        }

        public override string ToString () {
            return Stringify("{", ",", "}");
        }

        /// <summary>
        /// 指定したセパレータと括弧を使って、中身を文字列に変換する
        /// </summary>
        public string Stringify (
            string head, string sep1, string tail ) {
            StringBuilder sb = new StringBuilder();
            sb.Append(head).Append(mKey).Append(sep1).Append(mValue).Append(tail);
            return sb.ToString();
        }
    }
}