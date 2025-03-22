using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.ReversePatching
{
	[TestFixture, NonParallelizable]
	public class AttributeReversePatchesWithTargetMethod : TestLogger
	{


		[Test]
		public void Test_ReversePatchingWithAttributesWithTargetMethod()
		{
			var getExtraMethodInfo = AccessTools.Method(typeof(Class1Reverse), "GetExtra");
			ClassicAssert.NotNull(getExtraMethodInfo);

			var result1 = getExtraMethodInfo.Invoke(null, [123]);
			ClassicAssert.AreEqual("Extra123", result1);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var processor = instance.CreateClassProcessor(typeof(Class1ReversePatchWithTargetMethod));
			ClassicAssert.NotNull(processor);
			ClassicAssert.NotNull(processor.Patch());

			var result2 = Class1ReversePatchWithTargetMethod.GetExtra(123);
			ClassicAssert.AreEqual(result1, result2);
		}
	}
}
