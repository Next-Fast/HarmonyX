using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Patching
{
	[TestFixture]
	class ILManipulators : TestLogger
	{
		[Test]
		public void Test_ILManipulator()
		{
			var original = AccessTools.Method(typeof(ILManipulatorClass), nameof(ILManipulatorClass.SomeMethod));
			ClassicAssert.NotNull(original);

			var manipulator = AccessTools.Method(typeof(ILManipulatorClassPatch), nameof(ILManipulatorClassPatch.ILManipulator));
			ClassicAssert.NotNull(manipulator);

			string a = "a";
			string b = "string";

			ClassicAssert.AreEqual("a something string", ILManipulatorClass.SomeMethod(a, b));

			var instance = new Harmony("test-ilmanipulator");
			_ = instance.Patch(original, ilmanipulator: new HarmonyMethod(manipulator));

			ClassicAssert.AreEqual("a string", ILManipulatorClass.SomeMethod(a, b));
		}

		[Test]
		public void Test_ILManipulatorsWithOtherPatches()
		{
			var original = AccessTools.Method(typeof(ILManipulatorsAndOthersClass), nameof(ILManipulatorsAndOthersClass.SomeMethod));
			ClassicAssert.NotNull(original);

			var prefix = AccessTools.Method(typeof(ILManipulatorsAndOthersClassPatch), nameof(ILManipulatorsAndOthersClassPatch.Prefix));
			ClassicAssert.NotNull(prefix);

			var manipulator = AccessTools.Method(typeof(ILManipulatorsAndOthersClassPatch), nameof(ILManipulatorsAndOthersClassPatch.ILManipulator));
			ClassicAssert.NotNull(manipulator);

			var transpiler = AccessTools.Method(typeof(ILManipulatorsAndOthersClassPatch), nameof(ILManipulatorsAndOthersClassPatch.Transpiler));
			ClassicAssert.NotNull(transpiler);

			ClassicAssert.AreEqual(14, ILManipulatorsAndOthersClass.SomeMethod(4));

			var instance = new Harmony("test-ilmanipulators-and-other-patches");
			_ = instance.Patch(original, prefix: new HarmonyMethod(prefix), ilmanipulator: new HarmonyMethod(manipulator), transpiler: new HarmonyMethod(transpiler));

			ClassicAssert.AreEqual(18, ILManipulatorsAndOthersClass.SomeMethod(4));
		}

		[Test]
		public void Test_ILManipulatorName()
		{
			ClassicAssert.AreEqual("string1", ILManipulatorNameClass.SomeMethod("string"));

			var instance = new Harmony("test-ilmanipulators-name");
			instance.PatchAll(typeof(ILManipulatorNameClassPatch));

			ClassicAssert.AreEqual("string2", ILManipulatorNameClass.SomeMethod("string"));
		}

		[Test]
		public void Test_ILManipulatorAttribute()
		{
			ClassicAssert.AreEqual(2, ILManipulatorAttributeClass.SomeMethod(6, 3));

			var instance = new Harmony("test-ilmanipulators-attribute");
			instance.PatchAll(typeof(ILManipulatorAttributeClassPatch));

			ClassicAssert.AreEqual(8, ILManipulatorAttributeClass.SomeMethod(2, 4));
		}

		[Test]
		public void Test_ILManipulatorReturnLabel()
		{
			var original = AccessTools.Method(typeof(ILManipulatorReturnLabelClass), nameof(ILManipulatorReturnLabelClass.SomeMethod));
			ClassicAssert.NotNull(original);

			var postfix = AccessTools.Method(typeof(ILManipulatorReturnLabelClassPatch), nameof(ILManipulatorReturnLabelClassPatch.Postfix));
			ClassicAssert.NotNull(postfix);

			var manipulator = AccessTools.Method(typeof(ILManipulatorReturnLabelClassPatch), nameof(ILManipulatorReturnLabelClassPatch.ILManipulator));
			ClassicAssert.NotNull(manipulator);

			ClassicAssert.AreEqual(3, ILManipulatorReturnLabelClass.SomeMethod(2));

			var instance = new Harmony("test-ilmanipulators-return-label");
			var a = instance.Patch(original, postfix: new HarmonyMethod(postfix), ilmanipulator: new HarmonyMethod(manipulator));

			ClassicAssert.AreEqual(7, ILManipulatorReturnLabelClass.SomeMethod(5));
		}
	}
}
