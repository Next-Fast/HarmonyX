using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Extras
{
	[TestFixture, NonParallelizable]
	public class TestMethodInvoker : TestLogger
	{
		[Test]
		public void Test_MethodInvokerGeneral()
		{
			for (var i = 0; i < 2; i++)
			{
				var directBoxValueAccess = i == 0;

				var type = typeof(MethodInvokerClass);
				ClassicAssert.NotNull(type);
				var method = type.GetMethod("Method1");
				ClassicAssert.NotNull(method);

				var handler = MethodInvoker.GetHandler(method, directBoxValueAccess);
				ClassicAssert.NotNull(handler);

				var testStruct = new TestMethodInvokerStruct();
				var boxedTestStruct = (object)testStruct;
				var args = new object[] { 0, 0, 0, /*out*/ null, /*ref*/ boxedTestStruct };
				for (var a = 0; a < 100; a++)
				{
					args[0] = a;
					var b = (int)args[1];
					_ = handler(null, args);
					ClassicAssert.AreEqual(a, args[0], "@a={0}", a);
					ClassicAssert.AreEqual(b + 1, args[1], "@a={0}", a);
					ClassicAssert.AreEqual((b + 1) * 2, args[2], "@a={0}", a);
					ClassicAssert.AreEqual(a, ((TestMethodInvokerObject)args[3])?.Value, "@a={0}", a);
					ClassicAssert.AreEqual(a, ((TestMethodInvokerStruct)args[4]).Value, "@a={0}", a);
					ClassicAssert.AreEqual(0, testStruct.Value, "@a={0}", a);
					ClassicAssert.AreEqual(directBoxValueAccess ? a : 0, ((TestMethodInvokerStruct)boxedTestStruct).Value, "@a={0}", a);
				}
			}
		}

		[Test]
		public void Test_MethodInvokerSelfObject()
		{
			var type = typeof(TestMethodInvokerObject);
			ClassicAssert.NotNull(type);
			var method = type.GetMethod("Method1");
			ClassicAssert.NotNull(method);

			var handler = MethodInvoker.GetHandler(method);
			ClassicAssert.NotNull(handler);

			var instance = new TestMethodInvokerObject
			{
				Value = 1
			};

			var args = new object[] { 2 };
			_ = handler(instance, args);
			ClassicAssert.AreEqual(3, instance.Value);
		}
	}
}
