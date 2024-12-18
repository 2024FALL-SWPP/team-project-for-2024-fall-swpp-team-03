#region

using System;

#endregion

namespace SWPPT3.Main.Utility.Singleton
{
    public abstract class Singleton<T> where T: new()
    {
        private static readonly Lazy<T> s_instance = new(() => new T());
        public static T Instance => s_instance.Value;
    }
}
