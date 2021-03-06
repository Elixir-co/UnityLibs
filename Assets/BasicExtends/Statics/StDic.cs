﻿namespace BasicExtends {
    using System.Collections.Generic;
    using System.Text;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class StringDict : Dictionary<string, string> { }
    public class GameObjDict : Dictionary<string, GameObject> { }
    public class ObjDict : Dictionary<string, object> { }

    public interface IDictionaryDataConvertable<K,V> {
        K Key { get; set; }
        V Value { get; set; }
    }

    /// <summary>
    /// 辞書型コレクションに対する
    /// スタンダードな拡張関数を提供しています。
    /// </summary>
    public static class StDic {
        public static string basicHead = "{";
        public static string basicTail = "}";
        public static string basicSeparator1 = ":";
        public static string basicSeparator2 = ",";

        public static Dictionary<K,V> FromJson<K, V> ( this JsonNode node ) {
            throw new System.Exception();
        }

        public static string ToJson<K, V> (
            this Dictionary<K, V> dic , string markK = "\"", string markV = "\"" ) {
            string head = basicHead;
            string sep1 = basicSeparator1;
            string sep2 = basicSeparator2;
            string tail = basicTail;
            StringBuilder sb = new StringBuilder(dic.Values.Count);
            sb.Append(head);
            foreach (var val in dic) {
                sb.Append(markK)
                    .Append(val.Key)
                    .Append(markK)
                    .Append(sep1)
                    .Append(markV)
                    .Append(val.Value)
                    .Append(markV)
                    .Append(sep2);
            }
            var ret = sb.ToString();
            return ret.Substring(0, ret.Length - sep2.Length) + tail;
        }

        /// <summary>
        /// 指定したセパレータと括弧を使って、辞書の中身を文字列に変換する
        /// </summary>
        public static string Stringify<K, V> (
            this Dictionary<K, V> dic,
            string head, string sep1,string sep2, string tail ) {
            StringBuilder sb = new StringBuilder(dic.Values.Count);
            sb.Append(head);
            foreach (var val in dic) {
                sb.Append(val.Key)
                    .Append(sep1)
                    .Append(val.Value)
                    .Append(sep2);
            }
            var ret = sb.ToString();
            return ret.Substring(0, ret.Length - sep2.Length) + tail;
        }

        public static string Stringify<K, V> (
            this Dictionary<K, V> dic, string sep1, string sep2 ) {
            return dic.Stringify(basicHead, sep1, sep2, basicTail);
        }

        public static string Stringify<K, V> (
            this Dictionary<K, V> dic ) {
            return dic.Stringify(basicHead,
                basicSeparator1,
                basicSeparator2,
                basicTail);
        }

        public static bool KeyNotFound<K,V>( this Dictionary<K, V> dic ,K key) {
            return !dic.ContainsKey(key);
        }

        public static Dictionary<K,V> TrySet<K, V> (this Dictionary<K, V> dic, K key, V val) {
            if (dic.ContainsKey(key)) {
                dic [key] = val;
                return dic;
            }
            dic.Add(key, val);
            return dic;
        }

        public static Dictionary<K, V> TrySet<K, V> ( this Dictionary<K, V> dic, IDictionaryDataConvertable<K,V> data ) {
            return dic.TrySet(data.Key, data.Value) ;
        }

        public static CheckedRet<V> TryGet<K,V> (this Dictionary<K,V> dic ,K key) {
            var r1 = default(V);
            var r2 = dic.TryGetValue(key, out r1);
            return CheckedRet<V>.Gen(r2, r1);
        }
    }
}
