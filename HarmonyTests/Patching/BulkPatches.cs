using HarmonyLib;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class BulkPatches : TestLogger
	{
		[Test]
		public void Test_HarmonyPatchAll()
		{
			var harmony = new Harmony("test");
			var processor = new PatchClassProcessor(harmony, typeof(Assets.BulkPatchClassPatch));
			Assets.BulkPatchClassPatch.transpileCount = 0;
			var patches = processor.Patch();
			ClassicAssert.NotNull(patches, "patches");
			ClassicAssert.AreEqual(3, patches.Count);
			ClassicAssert.AreEqual(3, Assets.BulkPatchClassPatch.transpileCount, "transpileCount");

			var instance = new Assets.BulkPatchClass();
			ClassicAssert.AreEqual("TEST1+", instance.Method1());
			ClassicAssert.AreEqual("TEST2+", instance.Method2());
		}
	}
}
