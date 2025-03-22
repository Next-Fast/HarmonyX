using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Collections.Generic;

namespace HarmonyLibTests.Tools
{
	[TestFixture, NonParallelizable]
	public class TestTraverse_Basics : TestLogger
	{
		static readonly List<string> fieldNames = ["_root", "_type", "_info", "_method", "_params"];

		// Basic integrity check for our test class and the field-testvalue relations
		//
		[Test]
		public void Test_Instantiate_TraverseFields_AccessModifiers()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			for (var i = 0; i < TraverseFields.testStrings.Length; i++)
				ClassicAssert.AreEqual(TraverseFields.testStrings[i], instance.GetTestField(i));
		}

		[Test]
		public void Test_Traverse_Has_Expected_Internal_Fields()
		{
			foreach (var name in fieldNames)
			{
				var fInfo = AccessTools.DeclaredField(typeof(Traverse), name);
				ClassicAssert.NotNull(fInfo);
			}
		}

		public static void AssertIsEmpty(Traverse trv)
		{
			foreach (var name in fieldNames)
				ClassicAssert.AreEqual(null, AccessTools.DeclaredField(typeof(Traverse), name).GetValue(trv));
		}

		class FooBar
		{
#pragma warning disable CS0169
			readonly string field;
#pragma warning restore CS0169
		}

		// Traverse should default to an empty instance to avoid errors
		//
		[Test]
		public void Traverse_SilentFailures()
		{
			var trv1 = new Traverse(null);
			AssertIsEmpty(trv1);

			trv1 = Traverse.Create(null);
			AssertIsEmpty(trv1);

			var trv2 = trv1.Type("FooBar");
			AssertIsEmpty(trv2);

			var trv3 = Traverse.Create<FooBar>().Field("field");
			AssertIsEmpty(trv3);

			var trv4 = new Traverse(new FooBar()).Field("field");
			AssertIsEmpty(trv4.Method("", new object[0]));
			AssertIsEmpty(trv4.Method("", [], []));
		}

		// Traverse should handle basic null values
		//
		[Test]
		public void Traverse_Create_With_Null()
		{
			var trv = Traverse.Create(null);

			ClassicAssert.NotNull(trv);
			ClassicAssert.Null(trv.ToString());

			// field access

			var ftrv = trv.Field("foo");
			ClassicAssert.NotNull(ftrv);

			ClassicAssert.Null(ftrv.GetValue());
			ClassicAssert.Null(ftrv.ToString());
			ClassicAssert.AreEqual(0, ftrv.GetValue<int>());
			ClassicAssert.AreSame(ftrv, ftrv.SetValue(123));

			// property access

			var ptrv = trv.Property("foo");
			ClassicAssert.NotNull(ptrv);

			ClassicAssert.Null(ptrv.GetValue());
			ClassicAssert.Null(ptrv.ToString());
			ClassicAssert.Null(ptrv.GetValue<string>());
			ClassicAssert.AreSame(ptrv, ptrv.SetValue("test"));

			// method access

			var mtrv = trv.Method("zee");
			ClassicAssert.NotNull(mtrv);

			ClassicAssert.Null(mtrv.GetValue());
			ClassicAssert.Null(mtrv.ToString());
			ClassicAssert.AreEqual(0, mtrv.GetValue<float>());
			ClassicAssert.AreSame(mtrv, mtrv.SetValue(null));
		}

		// Traverse.ToString() should return a meaningful string representation of its initial value
		//
		[Test]
		public void Test_Traverse_Create_Instance_ToString()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			var trv = Traverse.Create(instance);
			ClassicAssert.AreEqual(instance.ToString(), trv.ToString());
		}

		// Traverse.ToString() should return a meaningful string representation of its initial type
		//
		[Test]
		public void Test_Traverse_Create_Type_ToString()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);
			ClassicAssert.NotNull(instance);

			var type = typeof(TraverseFields_AccessModifiers);
			var trv = Traverse.Create(type);
			ClassicAssert.AreEqual(type.ToString(), trv.ToString());
		}
	}
}
