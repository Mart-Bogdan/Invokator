using System;

namespace SUF.Common.GeneralPurpose
{
    public class WeakReference<T> where T : class
    {
        private WeakReference reference;

        public WeakReference(T target)
        {
            if (target != null)
                reference = new WeakReference(target);
        }

        private WeakReference()
        {
        }

        public bool IsAlive
        {
            get
            {
                return reference != null && reference.IsAlive;
            }
        }

        public T Target
        {
            get
            {
                return reference.Target as T;
            }
        }

        public override string ToString()
        {
            return Target == null ? string.Format("WeakReference<{0}> (empty)", typeof(T).Name) : Target.ToString();
        }

        public override int GetHashCode()
        {
            return Target != null ? Target.GetHashCode() : reference.GetHashCode();
        }

        public static implicit operator WeakReference<T>(T t)
        {
            return t == null ? new WeakReference<T>() : new WeakReference<T>(t);
        }
    }
}