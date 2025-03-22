using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Linq;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class StaticPatches : TestLogger
	{
		[Test]
		public void Test_Method0()
		{
			var originalClass = typeof(Class0);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method0");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class0Patch);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPostfix(postfix);
			_ = patcher.Patch();

			var result = new Class0().Method0();
			ClassicAssert.AreEqual("patched", result);
		}

		[Test]
		public void Test_Method1()
		{
			var originalClass = typeof(Class1);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method1");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class1Patch);
			var prefix = patchClass.GetMethod("Prefix");
			var postfix = patchClass.GetMethod("Postfix");
			var transpiler = patchClass.GetMethod("Transpiler");
			ClassicAssert.NotNull(prefix);
			ClassicAssert.NotNull(postfix);
			ClassicAssert.NotNull(transpiler);

			Class1Patch.ResetTest();

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPrefix(prefix);
			_ = patcher.AddPostfix(postfix);
			_ = patcher.AddTranspiler(transpiler);

			//var originalMethodStartPre = TestTools.GetMethodStart(originalMethod, out _);
			_ = patcher.Patch();
			//var originalMethodStartPost = TestTools.GetMethodStart(originalMethod, out _);
			//ClassicAssert.AreEqual(originalMethodStartPre, originalMethodStartPost);

			Class1.Method1();
			ClassicAssert.True(Class1Patch.prefixed, "Prefix was not executed");
			ClassicAssert.True(Class1Patch.originalExecuted, "Original was not executed");
			ClassicAssert.True(Class1Patch.postfixed, "Postfix was not executed");
		}

		[Test]
		public void Test_Method2()
		{
			var originalClass = typeof(Class2);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method2");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class2Patch);
			var prefix = patchClass.GetMethod("Prefix");
			var postfix = patchClass.GetMethod("Postfix");
			var transpiler = patchClass.GetMethod("Transpiler");
			ClassicAssert.NotNull(prefix);
			ClassicAssert.NotNull(postfix);
			ClassicAssert.NotNull(transpiler);

			Class2Patch.ResetTest();

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPrefix(prefix);
			_ = patcher.AddPostfix(postfix);
			_ = patcher.AddTranspiler(transpiler);

			// var originalMethodStartPre = TestTools.GetMethodStart(originalMethod, out _);
			_ = patcher.Patch();
			// var originalMethodStartPost = TestTools.GetMethodStart(originalMethod, out _);
			// ClassicAssert.AreEqual(originalMethodStartPre, originalMethodStartPost);

			new Class2().Method2();
			ClassicAssert.True(Class2Patch.prefixed, "Prefix was not executed");
			ClassicAssert.True(Class2Patch.originalExecuted, "Original was not executed");
			ClassicAssert.True(Class2Patch.postfixed, "Postfix was not executed");
		}

		[Test]
		public void Test_Method4()
		{
			var originalClass = typeof(Class4);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method4");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class4Patch);
			var prefix = patchClass.GetMethod("Prefix");
			ClassicAssert.NotNull(prefix);

			Class4Patch.ResetTest();

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPrefix(prefix);

			// var originalMethodStartPre = TestTools.GetMethodStart(originalMethod, out _);
			_ = patcher.Patch();
			// var originalMethodStartPost = TestTools.GetMethodStart(originalMethod, out _);
			// ClassicAssert.AreEqual(originalMethodStartPre, originalMethodStartPost);

			(new Class4()).Method4("foo");
			ClassicAssert.True(Class4Patch.prefixed, "Prefix was not executed");
			ClassicAssert.True(Class4Patch.originalExecuted, "Original was not executed");
			ClassicAssert.AreEqual(Class4Patch.senderValue, "foo");
		}

		[Test]
		public void Test_Method5()
		{
			var originalClass = typeof(Class5);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method5");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class5Patch);
			var prefix = patchClass.GetMethod("Prefix");
			ClassicAssert.NotNull(prefix);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			Class5Patch.ResetTest();

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPrefix(prefix);
			_ = patcher.AddPostfix(postfix);
			_ = patcher.Patch();

			(new Class5()).Method5("foo");
			ClassicAssert.True(Class5Patch.prefixed, "Prefix was not executed");
			ClassicAssert.True(Class5Patch.postfixed, "Postfix was not executed");
		}

		[Test]
		public void Test_PatchUnpatch()
		{
			var originalClass = typeof(Class9);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("ToString");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class9Patch);
			var prefix = patchClass.GetMethod("Prefix");
			ClassicAssert.NotNull(prefix);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var instance = new Harmony("unpatch-all-test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPrefix(prefix);
			_ = patcher.AddPostfix(postfix);
			_ = patcher.Patch();

			var instanceB = new Harmony("test");
			ClassicAssert.NotNull(instanceB);

			Harmony.UnpatchID("unpatch-all-test");
		}

		[Test]
		public void Test_Attributes()
		{
			var originalClass = typeof(AttributesClass);
			ClassicAssert.NotNull(originalClass);

			var originalMethod = originalClass.GetMethod("Method");
			ClassicAssert.NotNull(originalMethod);
			ClassicAssert.AreEqual(originalMethod, AttributesPatch.Patch0());

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patchClass = typeof(AttributesPatch);
			ClassicAssert.NotNull(patchClass);

			AttributesPatch.ResetTest();

			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher);
			ClassicAssert.NotNull(patcher.Patch());

			(new AttributesClass()).Method("foo");
			ClassicAssert.True(AttributesPatch.targeted, "TargetMethod was not executed");
			ClassicAssert.True(AttributesPatch.prefixed, "Prefix was not executed");
			ClassicAssert.True(AttributesPatch.postfixed, "Postfix was not executed");
		}

		[Test]
		public void Test_Method10()
		{
			var originalClass = typeof(Class10);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("Method10");
			ClassicAssert.NotNull(originalMethod);

			var patchClass = typeof(Class10Patch);
			var postfix = patchClass.GetMethod("Postfix");
			ClassicAssert.NotNull(postfix);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patcher = instance.CreateProcessor(originalMethod);
			ClassicAssert.NotNull(patcher);
			_ = patcher.AddPostfix(postfix);
			_ = patcher.Patch();

			_ = new Class10().Method10();
			ClassicAssert.True(Class10Patch.postfixed);
			ClassicAssert.True(Class10Patch.originalResult);
		}

		[Test]
		public void Test_MultiplePatches_One_Class()
		{
			var originalClass = typeof(MultiplePatches1);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);
			var patchClass = typeof(MultiplePatches1Patch);
			ClassicAssert.NotNull(patchClass);

			MultiplePatches1.result = "before";

			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher);
			ClassicAssert.NotNull(patcher.Patch());

			var result = (new MultiplePatches1()).TestMethod("after");
			ClassicAssert.AreEqual("after,prefix2,prefix1", MultiplePatches1.result);
			ClassicAssert.AreEqual("ok,postfix", result);
		}

		[Test]
		public void Test_MultiplePatches_Multiple_Classes()
		{
			var originalClass = typeof(MultiplePatches2);
			ClassicAssert.NotNull(originalClass);
			var originalMethod = originalClass.GetMethod("TestMethod");
			ClassicAssert.NotNull(originalMethod);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);
			var patchClass1 = typeof(MultiplePatchesPatch2_Part1);
			ClassicAssert.NotNull(patchClass1);
			var patchClass2 = typeof(MultiplePatchesPatch2_Part2);
			ClassicAssert.NotNull(patchClass2);
			var patchClass3 = typeof(MultiplePatchesPatch2_Part3);
			ClassicAssert.NotNull(patchClass3);

			MultiplePatches2.result = "before";

			var patcher1 = instance.CreateClassProcessor(patchClass1);
			ClassicAssert.NotNull(patcher1);
			ClassicAssert.NotNull(patcher1.Patch());
			var patcher2 = instance.CreateClassProcessor(patchClass2);
			ClassicAssert.NotNull(patcher2);
			ClassicAssert.NotNull(patcher2.Patch());
			var patcher3 = instance.CreateClassProcessor(patchClass3);
			ClassicAssert.NotNull(patcher3);
			ClassicAssert.NotNull(patcher3.Patch());

			var result = (new MultiplePatches2()).TestMethod("hey");
			ClassicAssert.AreEqual("hey,prefix2,prefix1", MultiplePatches2.result);
			ClassicAssert.AreEqual("patched", result);
		}

		[Test]
		public void Test_Finalizer_Patch_Order()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");
			var processor = instance.CreateClassProcessor(typeof(Finalizer_Patch_Order_Patch));
			ClassicAssert.NotNull(processor, "processor");

			var methods = processor.Patch();
			ClassicAssert.NotNull(methods, "methods");
			ClassicAssert.AreEqual(1, methods.Count);

			Finalizer_Patch_Order_Patch.ResetTest();
			var test = new Finalizer_Patch_Order_Class();
			ClassicAssert.NotNull(test, "test");
			var values = test.Method().ToList();

			ClassicAssert.NotNull(values, "values");
			ClassicAssert.AreEqual(3, values.Count);
			ClassicAssert.AreEqual(11, values[0]);
			ClassicAssert.AreEqual(21, values[1]);
			ClassicAssert.AreEqual(31, values[2]);

			// note that since passthrough postfixes are async, they run AFTER any finalizer
			//
			var actualEvents = Finalizer_Patch_Order_Patch.GetEvents();
			var correctEvents = new string[] {
				"Bool_Prefix",
				"Void_Prefix",
				"Simple_Postfix",
				"NonModifying_Finalizer",
				"ClearException_Finalizer",
				"Void_Finalizer",
				"Passthrough_Postfix2 start",
				"Passthrough_Postfix1 start",
				"Yield 10 [old=1]", "Yield 11 [old=10]",
				"Yield 20 [old=2]", "Yield 21 [old=20]",
				"Yield 30 [old=3]", "Yield 31 [old=30]",
				"Passthrough_Postfix1 end",
				"Passthrough_Postfix2 end"
			};

			ClassicAssert.True(actualEvents.SequenceEqual(correctEvents), "events");
		}

		[Test]
		public void Test_Affecting_Original_Prefixes()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");
			var processor = instance.CreateClassProcessor(typeof(Affecting_Original_Prefixes_Patch));
			ClassicAssert.NotNull(processor, "processor");

			var methods = processor.Patch();
			ClassicAssert.NotNull(methods, "methods");
			ClassicAssert.AreEqual(1, methods.Count);

			Affecting_Original_Prefixes_Patch.ResetTest();
			var test = new Affecting_Original_Prefixes_Class();
			ClassicAssert.NotNull(test, "test");
			var value = test.Method(100);

			ClassicAssert.AreEqual("patched", value);

			// note that since passthrough postfixes are async, they run AFTER any finalizer
			//
			var events = Affecting_Original_Prefixes_Patch.GetEvents();
			ClassicAssert.AreEqual(4, events.Count, "event count");
			ClassicAssert.AreEqual("Prefix1", events[0]);
			ClassicAssert.AreEqual("Prefix2", events[1]);
			ClassicAssert.AreEqual("Prefix3", events[2]);
			ClassicAssert.AreEqual("Prefix5", events[3]);
		}

		[Test]
		public void Test_Class18()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");
			var processor = instance.CreateClassProcessor(typeof(Class18Patch));
			ClassicAssert.NotNull(processor, "processor");

			var methods = processor.Patch();
			ClassicAssert.NotNull(methods, "methods");
			ClassicAssert.AreEqual(1, methods.Count);

			Class18Patch.prefixExecuted = false;
			var color = Class18.GetDefaultNameplateColor(new APIUser());

			ClassicAssert.IsTrue(Class18Patch.prefixExecuted, "prefixExecuted");
			ClassicAssert.AreEqual((float)1, color.r);
		}

		[Test]
		public void Test_Explicit_Attributes()
		{
			var originalClass = typeof(AttributesClass);
			ClassicAssert.NotNull(originalClass);

			var originalMethod = originalClass.GetMethod("Method");
			ClassicAssert.NotNull(originalMethod);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);

			var patchClass = typeof(ExplicitAttributesPatch);
			ClassicAssert.NotNull(patchClass);

			ExplicitAttributesPatch.ResetTest();

			var patcher = instance.CreateClassProcessor(patchClass, true);
			ClassicAssert.NotNull(patcher);
			ClassicAssert.NotNull(patcher.Patch());

			(new AttributesClass()).Method("foo");
			ClassicAssert.True(ExplicitAttributesPatch.prefixed, "Prefix was not executed");
			ClassicAssert.True(ExplicitAttributesPatch.postfixed, "Postfix was not executed");
		}

		[Test]
		public void Test_Class22()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");

			var processor = new PatchClassProcessor(instance, typeof(Class22));
			ClassicAssert.NotNull(processor, "processor");
			_ = processor.Patch();

			Class22.bool1 = null;
			Class22.bool2 = null;
			Class22.bool3 = null;
			Class22.bool4 = null;
			Class22.Method22();

			ClassicAssert.NotNull(Class22.bool1, "bool1");
			ClassicAssert.IsTrue(Class22.bool1.Value, "bool1.Value");

			ClassicAssert.NotNull(Class22.bool2, "bool2");
			ClassicAssert.IsTrue(Class22.bool2.Value, "bool2.Value");

			ClassicAssert.NotNull(Class22.bool3, "bool3");
			ClassicAssert.IsFalse(Class22.bool3.Value, "bool3.Value");

			ClassicAssert.NotNull(Class22.bool4, "bool3");
			ClassicAssert.IsFalse(Class22.bool4.Value, "bool4.Value");
		}

		[Test]
		public void Test_Class22b()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");

			var processor = new PatchClassProcessor(instance, typeof(Class22b));
			ClassicAssert.NotNull(processor, "processor");
			_ = processor.Patch();

			Class22b.prefixResult = false;
			Class22b.originalExecuted = false;
			Class22b.runOriginalPre = null;
			Class22b.runOriginalPost = null;
			Class22b.Method22b();

			ClassicAssert.IsFalse(Class22b.originalExecuted, "originalExecuted 1");
			ClassicAssert.IsTrue(Class22b.runOriginalPre.Value, "runOriginalPre.Value 1");
			ClassicAssert.IsFalse(Class22b.runOriginalPost.Value, "runOriginalPre.Value 1");

			Class22b.prefixResult = true;
			Class22b.originalExecuted = false;
			Class22b.runOriginalPre = null;
			Class22b.runOriginalPost = null;
			Class22b.Method22b();

			ClassicAssert.IsTrue(Class22b.originalExecuted, "originalExecuted 2");
			ClassicAssert.IsTrue(Class22b.runOriginalPre.Value, "runOriginalPre.Value 2");
			ClassicAssert.IsTrue(Class22b.runOriginalPost.Value, "runOriginalPre.Value 2");
		}

		[Test]
		public void Test_Class23()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");

			var processor = new PatchClassProcessor(instance, typeof(Class23));
			ClassicAssert.NotNull(processor, "processor");
			_ = processor.Patch();

			Class23.bool1 = null;
			Class23.Method23();

			ClassicAssert.NotNull(Class23.bool1, "bool1");
			ClassicAssert.IsTrue(Class23.bool1.Value, "bool1.Value");
		}

		[Test]
		public void Test_Class24()
		{
			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "instance");

			var processor = new PatchClassProcessor(instance, typeof(Class24));
			ClassicAssert.NotNull(processor, "processor");
			_ = processor.Patch();

			Class24.bool1 = null;
			Class24.bool2 = null;
			Class24.Method24();

			ClassicAssert.NotNull(Class24.bool1, "bool1");
			ClassicAssert.IsTrue(Class24.bool1.Value, "bool1.Value");

			ClassicAssert.NotNull(Class24.bool2, "bool2");
			ClassicAssert.IsTrue(Class24.bool2.Value, "bool2.Value");
		}
	}
}
