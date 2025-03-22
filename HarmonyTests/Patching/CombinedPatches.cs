using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Collections.Generic;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class CombinedPatches : TestLogger
	{
		[Test]
		public void Test_ManyFinalizers()
		{
			var originalClass = typeof(CombinedPatchClass);
			ClassicAssert.NotNull(originalClass);
			var patchClass = typeof(CombinedPatchClass_Patch_1);
			ClassicAssert.NotNull(patchClass);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var processor = harmonyInstance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(processor);
			ClassicAssert.NotNull(processor.Patch());

			CombinedPatchClass_Patch_1.counter = 0;
			var instance = new CombinedPatchClass();
			instance.Method1();
			ClassicAssert.AreEqual("tested", instance.Method2("test"));
			instance.Method3(123);
			ClassicAssert.AreEqual(1111, CombinedPatchClass_Patch_1.counter);
		}

		[Test]
		public static void Test_Method11()
		{
			var originalClass = typeof(Class14);
			ClassicAssert.NotNull(originalClass);
			var patchClass = typeof(Class14Patch);
			ClassicAssert.NotNull(patchClass);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var processor = harmonyInstance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(processor);
			ClassicAssert.NotNull(processor.Patch());

			_ = new Class14().Test("Test1", new KeyValuePair<string, int>("1", 1));
			_ = new Class14().Test("Test2", new KeyValuePair<string, int>("1", 1), new KeyValuePair<string, int>("2", 2));

			ClassicAssert.AreEqual("Prefix0", Class14.state[0]);
			ClassicAssert.AreEqual("Test1", Class14.state[1]);
			ClassicAssert.AreEqual("Postfix0", Class14.state[2]);
			ClassicAssert.AreEqual("Prefix1", Class14.state[3]);
			ClassicAssert.AreEqual("Test2", Class14.state[4]);
			ClassicAssert.AreEqual("Postfix1", Class14.state[5]);
		}
	}
}
