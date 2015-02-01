using System;
using System.Collections.Generic;
using System.Runtime;
using System.Runtime.CompilerServices;

namespace SUF.Common.GeneralPurpose
{
    internal static class MyTuple
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public static MyTuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2)
        {
            return new MyTuple<T1, T2>(item1,item2);
        }
    }
    internal struct MyTuple<T1, T2> : IEquatable<MyTuple<T1, T2>>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public MyTuple(T1 item1, T2 item2)
        {
            Item1 = item1;
            Item2 = item2;
        }

        public bool Equals(MyTuple<T1, T2> other)
        {
            return Object.Equals(Item1, other.Item1) && Object.Equals(Item2, other.Item2);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MyTuple<T1, T2> && Equals((MyTuple<T1, T2>)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Item1 == null ? 0 : Item1.GetHashCode() * 397) ^ (Item1 == null ? 0 : Item2.GetHashCode());
            }
        }
    }
    internal struct MyTuple<T1, T2, T3> : IEquatable<MyTuple<T1, T2, T3>>
    {
        public readonly T1 Item1;
        public readonly T2 Item2;
        public readonly T3 Item3;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public MyTuple(T1 item1, T2 item2, T3 item3)
        {
            Item1 = item1;
            Item2 = item2;
            Item3 = item3;
        }

        public bool Equals(MyTuple<T1, T2, T3> other)
        {
            return Object.Equals(Item1, other.Item1) && Object.Equals(Item2, other.Item2) && Object.Equals(Item3, other.Item3);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MyTuple<T1, T2, T3> && Equals((MyTuple<T1, T2, T3>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode =              (Item1 == null ? 0 : EqualityComparer<T1>.Default.GetHashCode(Item1));
                hashCode = (hashCode*397) ^ (Item2 == null ? 0 : EqualityComparer<T2>.Default.GetHashCode(Item2));
                hashCode = (hashCode*397) ^ (Item3 == null ? 0 : EqualityComparer<T3>.Default.GetHashCode(Item3));
                return hashCode;
            }
        }
    }
}