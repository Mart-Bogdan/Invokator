 using System;

namespace SUF.Common.GeneralPurpose
{
    /// <summary>
    /// Хранит ссылку на объект, но позволяет сборщику моусора убрать объект
    /// </summary>
    public class WeakReference<T> where T : class
    {
        /// <summary>
        /// Так а как?
        /// </summary>
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
            var target = Target;
            return target == null ? string.Format("WeakReference<{0}> (empty)", typeof(T).Name) : target.ToString();
        }

        public override int GetHashCode()
        {
            var target = Target;
            return target != null ? target.GetHashCode() : reference.GetHashCode();
        }

        public static implicit operator WeakReference<T>(T t)
        {
            return t == null ? new WeakReference<T>() : new WeakReference<T>(t);
        }
    }
}