using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.ReversePatching
{
	[TestFixture]
	public class ILManipulatorReversePatches
	{
		[Test]
		public void Test_ILManipulatorReversePatch()
		{
			var class2 = new Class2Reverse();

			ClassicAssert.AreEqual("some string", class2.SomeMethod());

			var original = AccessTools.Method(typeof(Class2Reverse), nameof(Class2Reverse.SomeMethod));
			ClassicAssert.NotNull(original);

			var stub = AccessTools.Method(typeof(Class2ReversePatch), nameof(Class2ReversePatch.SomeMethodReverse));
			ClassicAssert.NotNull(stub);

			var instance = new Harmony("test-ilmanipulator-reverse");
			var reversePatcher = instance.CreateReversePatcher(original, new HarmonyMethod(stub));
			_ = reversePatcher.Patch();

			ClassicAssert.AreEqual("some other string", Class2ReversePatch.SomeMethodReverse());
		}
	}
}
