using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.ReversePatching
{
	[TestFixture, NonParallelizable]
	public class AttributeReversePatches : TestLogger
	{
		[Test]
		public void Test_ReversePatchingWithAttributes()
		{
			var test = new Class1Reverse();

			var result1 = test.Method("Foo", 123);
			ClassicAssert.AreEqual("FooExtra123", result1);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var processor = instance.CreateClassProcessor(typeof(Class1ReversePatch));
			ClassicAssert.NotNull(processor);
			ClassicAssert.NotNull(processor.Patch());

			var result2 = test.Method("Bar", 456);
			ClassicAssert.AreEqual("PrefixedExtra456Bar", result2);
		}
	}
}
