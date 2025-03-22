using HarmonyLib;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class ValueTypes : TestLogger
	{
		[Test]
		public void ValueTypeInstance()
		{
			var originalClass = typeof(Assets.Struct1);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Assets.Struct1Patch);

			ClassicAssert.NotNull(patchClass);
			var prefix = patchClass.GetMethod("Prefix");
			ClassicAssert.NotNull(prefix);

			ClassicAssert.NotNull(patchClass);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var instance = new Assets.Struct1() { s = "before", n = 1 };

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var result = harmonyInstance.Patch(originalMethod, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
			ClassicAssert.NotNull(result);

			Assets.Struct1.Reset();
			instance.TestMethod("new");
			ClassicAssert.AreEqual(2, instance.n);
			ClassicAssert.AreEqual("new", instance.s);
		}

		[Test]
		public void Test_StructInstanceNoRef()
		{
			var originalClass = typeof(Assets.Struct2NoRef);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Assets.Struct2NoRefObjectPatch);

			ClassicAssert.NotNull(patchClass);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var result = harmonyInstance.Patch(originalMethod, null, new HarmonyMethod(postfix));
			ClassicAssert.NotNull(result);

			var instance = new Assets.Struct2NoRef() { s = "before" };
			Assets.Struct2NoRefObjectPatch.s = "";
			instance.TestMethod("original");
			ClassicAssert.AreEqual("original", Assets.Struct2NoRefObjectPatch.s);
		}

		[Test]
		public void Test_StructInstanceByRef()
		{
			var originalClass = typeof(Assets.Struct2Ref);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Assets.Struct2RefPatch);

			ClassicAssert.NotNull(patchClass);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var result = harmonyInstance.Patch(originalMethod, null, new HarmonyMethod(postfix));
			ClassicAssert.NotNull(result);

			var instance = new Assets.Struct2Ref() { s = "before" };
			instance.TestMethod("original");
			ClassicAssert.AreEqual("patched", instance.s);
		}

		[Test]
		public void Test_StructInstanceNoRefObject()
		{
			var originalClass = typeof(Assets.Struct3NoRefObject);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Assets.Struct3NoRefObjectPatch);

			ClassicAssert.NotNull(patchClass);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var result = harmonyInstance.Patch(originalMethod, null, new HarmonyMethod(postfix));
			ClassicAssert.NotNull(result);

			var instance = new Assets.Struct3NoRefObject() { s = "before" };
			Assets.Struct3NoRefObjectPatch.s = "";
			instance.TestMethod("original");
			ClassicAssert.AreEqual("original", Assets.Struct3NoRefObjectPatch.s);
		}

		[Test]
		public void Test_StructInstanceByRefObject()
		{
			var originalClass = typeof(Assets.Struct3RefObject);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Assets.Struct3RefObjectPatch);

			ClassicAssert.NotNull(patchClass);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var result = harmonyInstance.Patch(originalMethod, null, new HarmonyMethod(postfix));
			ClassicAssert.NotNull(result);

			var instance = new Assets.Struct3RefObject() { s = "before" };
			instance.TestMethod("original");
			ClassicAssert.AreEqual("patched", instance.s);
		}
	}
}
