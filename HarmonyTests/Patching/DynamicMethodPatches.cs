using HarmonyLib;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class DynamicMethodPatches : TestLogger
	{
		[Test]
		public void Test_ByRefResultPrefix()
		{
			var originalClass = typeof(Assets.Class11);
			ClassicAssert.NotNull(originalClass);

			var originalMethod = originalClass.GetMethod(nameof(Assets.Class11.TestMethod));
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Assets.Class11Patch);
			ClassicAssert.NotNull(patchClass);

			var prefix = patchClass.GetMethod(nameof(Assets.Class11Patch.Prefix));
			ClassicAssert.NotNull(prefix);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var patchResult = harmonyInstance.Patch(
				original: originalMethod,
				prefix: new HarmonyMethod(prefix));

			ClassicAssert.NotNull(patchResult);

			var instance = new Assets.Class11();
			var result = instance.TestMethod(0);

			ClassicAssert.False(instance.originalMethodRan);
			ClassicAssert.True(Assets.Class11Patch.prefixed);

			ClassicAssert.AreEqual("patched", result);
		}
	}
}
