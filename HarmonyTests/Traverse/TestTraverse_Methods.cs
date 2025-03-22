using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;

namespace HarmonyLibTests.Tools
{
	[TestFixture, NonParallelizable]
	public class TestTraverse_Methods : TestLogger
	{
		[Test]
		public void Traverse_Method_Instance()
		{
			var instance = new TraverseMethods_Instance();
			var trv = Traverse.Create(instance);

			instance.Method1_called = false;
			var mtrv1 = trv.Method("Method1");
			ClassicAssert.AreEqual(null, mtrv1.GetValue());
			ClassicAssert.AreEqual(true, instance.Method1_called);

			var mtrv2 = trv.Method("Method2", ["arg"]);
			ClassicAssert.AreEqual("argarg", mtrv2.GetValue());
		}

		[Test]
		public void Traverse_Method_Static()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Static));
			var mtrv = trv.Method("StaticMethod", [6, 7]);
			ClassicAssert.AreEqual(42, mtrv.GetValue<int>());
		}

		[Test]
		public void Traverse_Method_VariableArguments()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_VarArgs));

			ClassicAssert.AreEqual(30, trv.Method("Test1", 10, 20).GetValue<int>());
			ClassicAssert.AreEqual(60, trv.Method("Test2", 10, 20, 30).GetValue<int>());

			// Calling varargs methods directly won't work. Use parameter array instead
			// ClassicAssert.AreEqual(60, trv.Method("Test3", 100, 10, 20, 30).GetValue<int>());
			ClassicAssert.AreEqual(6000, trv.Method("Test3", 100, new int[] { 10, 20, 30 }).GetValue<int>());
		}

		[Test]
		public void Traverse_Method_RefParameters()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Parameter));

			string result = null;
			var parameters = new object[] { result };
			var types = new Type[] { typeof(string).MakeByRefType() };
			var mtrv1 = trv.Method("WithRefParameter", types, parameters);
			ClassicAssert.AreEqual("ok", mtrv1.GetValue<string>());
			ClassicAssert.AreEqual("hello", parameters[0]);
		}

		[Test]
		public void Traverse_Method_OutParameters()
		{
			var trv = Traverse.Create(typeof(TraverseMethods_Parameter));

			string result = null;
			var parameters = new object[] { result };
			var types = new Type[] { typeof(string).MakeByRefType() };
			var mtrv1 = trv.Method("WithOutParameter", types, parameters);
			ClassicAssert.AreEqual("ok", mtrv1.GetValue<string>());
			ClassicAssert.AreEqual("hello", parameters[0]);
		}

		[Test]
		public void Traverse_Method_Overloads()
		{
			var instance = new TraverseMethods_Overloads();
			var trv = Traverse.Create(instance);

			var mtrv1 = trv.Method("SomeMethod", [typeof(string), typeof(bool)]);
			ClassicAssert.AreEqual(true, mtrv1.GetValue<bool>("test", false));
			ClassicAssert.AreEqual(false, mtrv1.GetValue<bool>("test", true));
		}
	}
}
