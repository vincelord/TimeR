using System;
using System.Collections.Generic;

namespace VTility.Logic
{
    public abstract partial class Singleton<T> where T : Singleton<T>, new()
    {
        private static readonly T s_instance = new T();

        protected Singleton()
        {
            if (s_instance != null)
            {
                string s = string.Format(
                    "An instance of {0} already exists at {0}.instance. " +
                    "That's what \"Singleton\" means. You can't create another.",
                    typeof(T));
                throw new System.Exception(s);
            }
        }

        public static T Instance { get { return s_instance; } }
    }

    [Serializable]
    public abstract partial class Multiton<T> where T : Multiton<T>, new()
    {
        private static T s_current = null;

        public static List<T> All = new List<T>();

        protected Multiton()
        {
            if (All == null)
                All = new List<T>();
            s_current = (T)this;
            Multiton<T>.Current = (T)this;

            All.Add(s_current);
        }

        //static Multiton()
        //{
        //    All = new List<T>();
        //}

        public static T Current
        {
            get => s_current ?? Last ?? new T();
            set => s_current = value;
        }

        public static T Last => All.Count > 0 ? All[All.Count - 1] : null;
    }
}