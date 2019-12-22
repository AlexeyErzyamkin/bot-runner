//using System;
//using System.Collections.Generic;
//using System.Diagnostics.CodeAnalysis;
//using System.Text;

//namespace Backend.Domain
//{
//    readonly struct Width : IEquatable<Width>
//    {
//        public readonly int Value;

//        public bool Equals([AllowNull] Width other)
//        {
//            return Value == other.Value;
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(Value);
//        }
//    }

//    readonly struct Height : IEquatable<Height>
//    {
//        public readonly int Value;

//        public bool Equals([AllowNull] Height other)
//        {
//            return Value == other.Value;
//        }

//        public override int GetHashCode()
//        {
//            return HashCode.Combine(Value);
//        }
//    }

//    class Universe
//    {
//        public Width Width { get; }

//        public Height Height { get; }


//    }

//    class Planet
//    {

//    }
//}
