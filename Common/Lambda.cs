using System;
using System.Reflection.Metadata.Ecma335;

namespace AdventOfCode.Common
{
	public class Lambda
    {
        public static T Identity<T>(T t) => t;

        public static Action<T> Discard<T>() => _ => { };
	}
}
