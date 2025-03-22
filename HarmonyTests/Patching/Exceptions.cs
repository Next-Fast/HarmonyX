using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Patching
{
	// TODO: DynamicMethod does not support 'catch .. when' and
	//       MonoMod.Core has still a bug so for now we cannot enable

	public class TestExceptionFilterBlock
	{
		[Test]
		[Ignore("Filter exceptions are currently not supported in DynamicMethods")]
		public void TestExceptionsWithFilter()
		{
			var originalClass = typeof(ClassExceptionFilter);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method1");
			ClassicAssert.NotNull(originalMethod);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = new PatchProcessor(instance, originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.Patch();

			ClassExceptionFilter.Method1();
		}

		[Test]
		[Ignore("Filter exceptions are currently not supported in DynamicMethods")]
		public void TestPlainMethodExceptions()
		{
			var originalClass = typeof(ClassExceptionFilter);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method2");
			ClassicAssert.NotNull(originalMethod);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = new PatchProcessor(instance, originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.Patch();

			var result = ClassExceptionFilter.Method2(null);
			ClassicAssert.AreEqual(100, result);
		}
	}
}
