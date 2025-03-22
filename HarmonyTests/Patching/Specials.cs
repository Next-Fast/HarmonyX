using HarmonyLib;
using HarmonyLib.Tools;
using HarmonyLibTests.Assets;
using HarmonyLibTests.Assets.Methods;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;
#if NET6_0_OR_GREATER
using System.Net.Http;
#else
using System.Net;
#endif
using System.Linq;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class Specials : TestLogger
	{
		[Test]
		public void Test_HttpWebRequestGetResponse()
		{
#if NET6_0_OR_GREATER
			var original = SymbolExtensions.GetMethodInfo(() => new HttpClient().Send(default));
#else
			var t_WebRequest = typeof(HttpWebRequest);
			ClassicAssert.NotNull(t_WebRequest);
			var original = AccessTools.DeclaredMethod(t_WebRequest, nameof(HttpWebRequest.GetResponse));
#endif
			ClassicAssert.NotNull(original);

			var prefix = SymbolExtensions.GetMethodInfo(() => HttpWebRequestPatches.Prefix());
			var postfix = SymbolExtensions.GetMethodInfo(() => HttpWebRequestPatches.Postfix());

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);
			_ = instance.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

			HttpWebRequestPatches.ResetTest();

#if NET6_0_OR_GREATER
			var client = new HttpClient();
			var webRequest = new HttpRequestMessage(HttpMethod.Get, "http://google.com");
			var response = client.Send(webRequest);
#else
			var request = WebRequest.Create("http://google.com");
			ClassicAssert.AreEqual(request.GetType(), t_WebRequest);
			var response = request.GetResponse();
#endif

			ClassicAssert.NotNull(response);
			ClassicAssert.True(HttpWebRequestPatches.prefixCalled, "Prefix not called");
			ClassicAssert.True(HttpWebRequestPatches.postfixCalled, "Postfix not called");
		}

		[Test]
		public void Test_PatchResultRef()
		{
			ResultRefStruct.numbersPrefix = [0, 0];
			ResultRefStruct.numbersPostfix = [0, 0];
			ResultRefStruct.numbersPostfixWithNull = [0];
			ResultRefStruct.numbersFinalizer = [0];
			ResultRefStruct.numbersMixed = [0, 0];

			var test = new ResultRefStruct();

			var instance = new Harmony("result-ref-test");
			ClassicAssert.NotNull(instance);
			var processor = instance.CreateClassProcessor(typeof(ResultRefStruct_Patch));
			ClassicAssert.NotNull(processor, "processor");

			test.ToPrefix() = 1;
			test.ToPostfix() = 2;
			test.ToPostfixWithNull() = 3;
			test.ToMixed() = 5;

			ClassicAssert.AreEqual(new[] { 1, 0 }, ResultRefStruct.numbersPrefix);
			ClassicAssert.AreEqual(new[] { 2, 0 }, ResultRefStruct.numbersPostfix);
			ClassicAssert.AreEqual(new[] { 3 }, ResultRefStruct.numbersPostfixWithNull);
			Assert.Throws<Exception>(() => test.ToFinalizer(), "ToFinalizer method does not throw");
			ClassicAssert.AreEqual(new[] { 5, 0 }, ResultRefStruct.numbersMixed);

			var replacements = processor.Patch();
			ClassicAssert.NotNull(replacements, "replacements");

			test.ToPrefix() = -1;
			test.ToPostfix() = -2;
			test.ToPostfixWithNull() = -3;
			test.ToFinalizer() = -4;
			test.ToMixed() = -5;

			ClassicAssert.AreEqual(new[] { 1, -1 }, ResultRefStruct.numbersPrefix);
			ClassicAssert.AreEqual(new[] { 2, -2 }, ResultRefStruct.numbersPostfix);
			ClassicAssert.AreEqual(new[] { -3 }, ResultRefStruct.numbersPostfixWithNull);
			ClassicAssert.AreEqual(new[] { -4 }, ResultRefStruct.numbersFinalizer);
			ClassicAssert.AreEqual(new[] { 42, -5 }, ResultRefStruct.numbersMixed);
		}

		[Test]
		public void Test_Enumerator_Patch()
		{
			ClassicAssert.Null(EnumeratorPatch.patchTarget);
			ClassicAssert.AreEqual(0, EnumeratorPatch.runTimes);

			var instance = new Harmony("special-case-enumerator-movenext");
			ClassicAssert.NotNull(instance);
			instance.PatchAll(typeof(EnumeratorPatch));

			ClassicAssert.IsNotNull(EnumeratorPatch.patchTarget);
			ClassicAssert.AreEqual("MoveNext", EnumeratorPatch.patchTarget.Name);

			var testObject = new EnumeratorCode();
			ClassicAssert.AreEqual(new[] { 1, 2, 3, 4, 5 }, testObject.NumberEnumerator().ToArray());
			ClassicAssert.AreEqual(6, EnumeratorPatch.runTimes);
		}

		// -----------------------------------------------------

		[Test]
		public void Test_Multiple_Attributes_Overload()
		{
			OverloadedCodePatch.callCount = 0;
			var instance = new Harmony("special-case-overload");
			ClassicAssert.NotNull(instance);
			instance.PatchAll(typeof(OverloadedCodePatch));

			var testObject1 = new OverloadedCode.Class1();
			var testObject2 = new OverloadedCode.Class2();
			ClassicAssert.NotNull(testObject1);
			ClassicAssert.NotNull(testObject2);
			Assert.DoesNotThrow(() => testObject1.Method(), "Method() wasn't patched");
			Assert.DoesNotThrow(() => testObject2.Method("test"), "Method(string) wasn't patched");
			ClassicAssert.AreEqual(2, OverloadedCodePatch.callCount);
		}

		[Test, NonParallelizable]
		public void Test_Patch_With_Module_Call()
		{
			if (AccessTools.IsMonoRuntime)
				Environment.SetEnvironmentVariable("MONOMOD_DMD_TYPE", "cecil");
			var testMethod = ModuleLevelCall.CreateTestMethod();
			ClassicAssert.AreEqual(0, testMethod());

			var instance = new Harmony("special-case-module-call");
			ClassicAssert.NotNull(instance);
			var postfix = AccessTools.Method(typeof(ModuleLevelCall), nameof(ModuleLevelCall.Postfix));
			ClassicAssert.NotNull(postfix);

			instance.Patch(testMethod.Method, postfix: new HarmonyMethod(postfix));
			ClassicAssert.AreEqual(1, testMethod());
			if (AccessTools.IsMonoRuntime)
				Environment.SetEnvironmentVariable("MONOMOD_DMD_TYPE", "");
		}

		[Test]
		public void Test_Type_Patch_Regression()
		{
			var instance = new Harmony("special-case-type-patch");
			ClassicAssert.NotNull(instance);

			var testObject = new MultiAttributePatchCall();
			ClassicAssert.NotNull(testObject);
			MultiAttributePatchCall.returnValue = true;
			ClassicAssert.True(testObject.GetValue());
			MultiAttributePatchCall.returnValue = false;
			ClassicAssert.False(testObject.GetValue());

			instance.PatchAll(typeof(TestMultiAttributePatch));

			MultiAttributePatchCall.returnValue = true;
			ClassicAssert.True(testObject.GetValue());
			MultiAttributePatchCall.returnValue = false;
			ClassicAssert.True(testObject.GetValue());
		}

		[Test]
		public void Test_Optional_Patch()
		{
			var instance = new Harmony("special-case-optional-patch");
			ClassicAssert.NotNull(instance);

			Assert.Throws<InvalidOperationException>(OptionalPatch.Thrower);
			Assert.DoesNotThrow(() => instance.PatchAll(typeof(OptionalPatch)));
			Assert.DoesNotThrow(OptionalPatch.Thrower);

			Assert.Throws<InvalidOperationException>(OptionalPatchNone.Thrower);
			Assert.Throws<HarmonyException>(() => instance.PatchAll(typeof(OptionalPatchNone)));
			Assert.Throws<InvalidOperationException>(OptionalPatchNone.Thrower);
		}

		[Test]
		public void Test_MultiTarget_Class1()
		{
			MultiAttributePatchClass1.callCount = 0;
			var instance = new Harmony("special-case-multi-target-1");
			ClassicAssert.NotNull(instance);

			var processor = instance.CreateClassProcessor(typeof(MultiAttributePatchClass1));
			ClassicAssert.NotNull(processor);
			processor.Patch();

			var testObject = new DeadEndCode();
			ClassicAssert.NotNull(testObject);
			Assert.DoesNotThrow(() => testObject.Method2(), "Test method 2 wasn't patched");
			Assert.DoesNotThrow(() => testObject.Method3(), "Test method 3 wasn't patched");
			ClassicAssert.AreEqual(2, MultiAttributePatchClass1.callCount);
		}

		[Test]
		public void Test_MultiTarget_Class2()
		{
			MultiAttributePatchClass2.callCount = 0;
			var instance = new Harmony("special-case-multi-target-2");
			ClassicAssert.NotNull(instance);

			var processor = instance.CreateClassProcessor(typeof(MultiAttributePatchClass2));
			ClassicAssert.NotNull(processor);
			processor.Patch();

			var testObject = new DeadEndCode();
			ClassicAssert.NotNull(testObject);
			Assert.DoesNotThrow(() => testObject.Method2(), "Test method 2 wasn't patched");
			Assert.DoesNotThrow(() => testObject.Method3(), "Test method 3 wasn't patched");
			ClassicAssert.AreEqual(2, MultiAttributePatchClass2.callCount);
		}

		[Test]
		public void Test_Multiple_Attributes_Partial()
		{
			var instance = new Harmony("special-case-multi-attribute-partial");
			ClassicAssert.NotNull(instance);
			instance.PatchAll(typeof(TypeTargetedPatch));

			var testObject = new DeadEndCode();
			ClassicAssert.NotNull(testObject);
			Assert.DoesNotThrow(() => testObject.Method4(), "Test method wasn't patched");
		}

		[Test]
		public void Test_Wrap_Patch()
		{
			SafeWrapPatch.called = false;
			var instance = new Harmony("special-case-wrap-patch");
			ClassicAssert.NotNull(instance);

			instance.PatchAll(typeof(SafeWrapPatch));

			var testObject = new DeadEndCode();
			ClassicAssert.NotNull(testObject);
			Assert.DoesNotThrow(() => testObject.Method5());
			ClassicAssert.True(SafeWrapPatch.called);
		}

		[Test]
		public void Test_ExceptionPostfixPatch()
		{
			PostfixOnExceptionPatch.called = false;
			PostfixOnExceptionPatch.patched = false;
			var instance = new Harmony("exception-postix-patch-1");
			ClassicAssert.NotNull(instance);

			var processor = instance.CreateClassProcessor(typeof(PostfixOnExceptionPatch));
			ClassicAssert.NotNull(processor);
			processor.Patch();

			ClassicAssert.True(PostfixOnExceptionPatch.patched, "Patch not applied");
			var testObject = new DeadEndCode();
			ClassicAssert.NotNull(testObject);
			Assert.Throws<Exception>(() => testObject.Method6(), "Test method 6 didn't throw");
			ClassicAssert.False(PostfixOnExceptionPatch.called, "Postfix was called");
		}

		[Test]
		public void Test_Patch_Exception_Propagate()
		{
			var instance = new Harmony("special-case-exception-throw");
			ClassicAssert.NotNull(instance);

			var processor = instance.CreateClassProcessor(typeof(ErrorReportTestPatch));
			ClassicAssert.NotNull(processor);
			Assert.Throws<HarmonyException>(() => processor.Patch());
		}

		// -----------------------------------------------------

		[Test]
		public void Test_Patch_ConcreteClass()
		{
			var instance = new Harmony("special-case-1");
			ClassicAssert.NotNull(instance, "instance");
			var processor = instance.CreateClassProcessor(typeof(ConcreteClass_Patch));
			ClassicAssert.NotNull(processor, "processor");

			var someStruct1 = new ConcreteClass().Method("test", new AnotherStruct());
			ClassicAssert.True(someStruct1.accepted, "someStruct1.accepted");

			TestTools.Log($"Patching ConcreteClass_Patch start");
			var replacements = processor.Patch();
			ClassicAssert.NotNull(replacements, "replacements");
			ClassicAssert.AreEqual(1, replacements.Count);
			TestTools.Log($"Patching ConcreteClass_Patch done");

			TestTools.Log($"Running patched ConcreteClass_Patch start");
			var someStruct2 = new ConcreteClass().Method("test", new AnotherStruct());
			ClassicAssert.True(someStruct2.accepted, "someStruct2.accepted");
			TestTools.Log($"Running patched ConcreteClass_Patch done");
		}

		[Test, NonParallelizable]
		public void Test_Patch_Returning_Structs([Values(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20)] int n, [Values("I", "S")] string type)
		{
			var name = $"{type}M{n:D2}";

			var patchClass = typeof(ReturningStructs_Patch);
			ClassicAssert.NotNull(patchClass);

			var prefix = SymbolExtensions.GetMethodInfo(() => ReturningStructs_Patch.Prefix(null));
			ClassicAssert.NotNull(prefix);

			var instance = new Harmony("returning-structs");
			ClassicAssert.NotNull(instance);

			var cls = AccessTools.TypeByName($"HarmonyLibTests.Assets.Methods.ReturningStructs_{type}{n:D2}");
			ClassicAssert.NotNull(cls, "type");
			var method = AccessTools.DeclaredMethod(cls, name);
			ClassicAssert.NotNull(method, "method");

			TestTools.Log($"Test_Returning_Structs: patching {name} start");
			try
			{
				var replacement = instance.Patch(method, new HarmonyMethod(prefix));
				ClassicAssert.NotNull(replacement, "replacement");
			}
			catch (Exception ex)
			{
				TestTools.Log($"Test_Returning_Structs: patching {name} exception: {ex}");
			}
			TestTools.Log($"Test_Returning_Structs: patching {name} done");

			var clsInstance = Activator.CreateInstance(cls);
			try
			{
				TestTools.Log($"Test_Returning_Structs: running patched {name}");

				var original = AccessTools.DeclaredMethod(cls, name);
				ClassicAssert.NotNull(original, $"{name}: original");
				var result = original.Invoke(type == "S" ? null : clsInstance, ["test"]);
				ClassicAssert.NotNull(result, $"{name}: result");
				ClassicAssert.AreEqual($"St{n:D2}", result.GetType().Name);

				TestTools.Log($"Test_Returning_Structs: running patched {name} done");
			}
			catch (Exception ex)
			{
				TestTools.Log($"Test_Returning_Structs: running {name} exception: {ex}");
			}
		}

		[Test]
		public void Test_PatchException()
		{
			var test = new DeadEndCode();

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);
			var original = AccessTools.Method(typeof(DeadEndCode), nameof(DeadEndCode.Method));
			ClassicAssert.NotNull(original);
			var prefix = AccessTools.Method(typeof(DeadEndCode_Patch1), nameof(DeadEndCode_Patch1.Prefix));
			ClassicAssert.NotNull(prefix);
			var postfix = AccessTools.Method(typeof(DeadEndCode_Patch1), nameof(DeadEndCode_Patch1.Postfix));
			ClassicAssert.NotNull(postfix);
			var prefixWithControl =
				AccessTools.Method(typeof(DeadEndCode_Patch1), nameof(DeadEndCode_Patch1.PrefixWithControl));
			ClassicAssert.NotNull(postfix);

			// run original
			try
			{
				_ = test.Method();
				Assert.Fail("expecting format exception");
			}
			catch (FormatException ex)
			{
				ClassicAssert.NotNull(ex);
			}

			// patch: +prefix
			var newMethod = instance.Patch(original, prefix: new HarmonyMethod(prefix));
			ClassicAssert.NotNull(newMethod);

			// run original with prefix
			DeadEndCode_Patch1.prefixCalled = false;
			try
			{
				_ = test.Method();
				Assert.Fail("expecting format exception");
			}
			catch (Exception ex)
			{
				ClassicAssert.NotNull(ex as FormatException);
			}
			ClassicAssert.True(DeadEndCode_Patch1.prefixCalled);

			// patch: +postfix
			_ = instance.Patch(original, postfix: new HarmonyMethod(postfix));
			DeadEndCode_Patch1.prefixCalled = false;
			DeadEndCode_Patch1.postfixCalled = false;
			// run original
			try
			{
				_ = test.Method();
				Assert.Fail("expecting format exception");
			}
			catch (FormatException ex)
			{
				ClassicAssert.NotNull(ex);
				ClassicAssert.True(DeadEndCode_Patch1.prefixCalled);
				ClassicAssert.False(DeadEndCode_Patch1.postfixCalled);
			}

			_ = instance.Patch(original, prefix: new HarmonyMethod(prefixWithControl));
			DeadEndCode_Patch1.prefixCalled = false;
			DeadEndCode_Patch1.postfixCalled = false;
			test.Method();
			ClassicAssert.True(DeadEndCode_Patch1.prefixCalled);
			ClassicAssert.True(DeadEndCode_Patch1.postfixCalled);
		}

		[Test]
		public void Test_PatchingLateThrow1()
		{
			var patchClass = typeof(LateThrowClass_Patch1);
			ClassicAssert.NotNull(patchClass);

			new LateThrowClass1().Method("AB");
			try
			{
				new LateThrowClass1().Method("");
				Assert.Fail("expecting exception");
			}
			catch (ArgumentException ex)
			{
				ClassicAssert.AreEqual(ex.Message, "fail");
			}

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher);
			ClassicAssert.NotNull(patcher.Patch());

			LateThrowClass_Patch1.prefixCalled = false;
			LateThrowClass_Patch1.postfixCalled = false;
			new LateThrowClass1().Method("AB");
			ClassicAssert.True(LateThrowClass_Patch1.prefixCalled);
			ClassicAssert.True(LateThrowClass_Patch1.postfixCalled);

			LateThrowClass_Patch1.prefixCalled = false;
			LateThrowClass_Patch1.postfixCalled = false;
			try
			{
				new LateThrowClass1().Method("");
				Assert.Fail("expecting exception");
			}
			catch (ArgumentException ex)
			{
				ClassicAssert.AreEqual(ex.Message, "fail");
			}
			ClassicAssert.True(LateThrowClass_Patch1.prefixCalled);
			ClassicAssert.False(LateThrowClass_Patch1.postfixCalled);

			LateThrowClass_Patch1.prefixCalled = false;
			LateThrowClass_Patch1.postfixCalled = false;
			new LateThrowClass1().Method("AB");
			ClassicAssert.True(LateThrowClass_Patch1.prefixCalled);
			ClassicAssert.True(LateThrowClass_Patch1.postfixCalled);
		}

		[Test]
		public void Test_PatchingLateThrow2()
		{
			var patchClass = typeof(LateThrowClass_Patch2);
			ClassicAssert.NotNull(patchClass);

			new LateThrowClass2().Method(0);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance);
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher);
			ClassicAssert.NotNull(patcher.Patch());

			LateThrowClass_Patch2.prefixCalled = false;
			LateThrowClass_Patch2.postfixCalled = false;
			new LateThrowClass2().Method(0);
			ClassicAssert.True(LateThrowClass_Patch2.prefixCalled);
			ClassicAssert.True(LateThrowClass_Patch2.postfixCalled);
		}

		[Test]
		public void Test_PatchExceptionWithCleanup2()
		{
			if (AccessTools.IsMonoRuntime is false)
				return; // Assert.Ignore("Only mono allows for detailed IL exceptions. Test ignored.");

			var patchClass = typeof(DeadEndCode_Patch3);
			ClassicAssert.NotNull(patchClass);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "Harmony instance");
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher, "Patch processor");
			try
			{
				_ = patcher.Patch();
			}
			catch (HarmonyException ex)
			{
				ClassicAssert.NotNull(ex.InnerException);
				ClassicAssert.IsInstanceOf(typeof(ArgumentException), ex.InnerException);
				ClassicAssert.AreEqual("Test", ex.InnerException.Message);
				return;
			}
			Assert.Fail("Patch should throw HarmonyException");
		}

		[Test]
		public void Test_PatchExceptionWithCleanup3()
		{
			if (AccessTools.IsMonoRuntime is false)
				return; // Assert.Ignore("Only mono allows for detailed IL exceptions. Test ignored.");

			var patchClass = typeof(DeadEndCode_Patch4);
			ClassicAssert.NotNull(patchClass);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "Harmony instance");
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher, "Patch processor");
			_ = patcher.Patch();
		}

		[Test]
		public void Test_PatchEventHandler()
		{
			Console.WriteLine($"### EventHandlerTestClass TEST");

			var patchClass = typeof(EventHandlerTestClass_Patch);
			ClassicAssert.NotNull(patchClass);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "Harmony instance");
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher, "Patch processor");
			var patched = patcher.Patch();
			ClassicAssert.AreEqual(1, patched.Count);
			ClassicAssert.NotNull(patched[0]);

			Console.WriteLine($"### EventHandlerTestClass BEFORE");
			new EventHandlerTestClass().Run();
			Console.WriteLine($"### EventHandlerTestClass AFTER");
		}

		[Test]
		public void Test_PatchMarshalledClass()
		{
			Console.WriteLine($"### MarshalledTestClass TEST");

			var patchClass = typeof(MarshalledTestClass_Patch);
			ClassicAssert.NotNull(patchClass);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "Harmony instance");
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher, "Patch processor");
			var patched = patcher.Patch();
			ClassicAssert.AreEqual(1, patched.Count);
			ClassicAssert.NotNull(patched[0]);

			Console.WriteLine($"### MarshalledTestClass BEFORE");
			new MarshalledTestClass().Run();
			Console.WriteLine($"### MarshalledTestClass AFTER");
		}

		[Test]
		public void Test_MarshalledWithEventHandler1()
		{
			Console.WriteLine($"### MarshalledWithEventHandlerTest1 TEST");

			var patchClass = typeof(MarshalledWithEventHandlerTest1Class_Patch);
			ClassicAssert.NotNull(patchClass);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "Harmony instance");
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher, "Patch processor");
			var patched = patcher.Patch();
			ClassicAssert.AreEqual(1, patched.Count);
			ClassicAssert.NotNull(patched[0]);

			Console.WriteLine($"### MarshalledWithEventHandlerTest1 BEFORE");
			new MarshalledWithEventHandlerTest1Class().Run();
			Console.WriteLine($"### MarshalledWithEventHandlerTest1 AFTER");
		}

		[Test]
		public void Test_MarshalledWithEventHandler2()
		{
			Console.WriteLine($"### MarshalledWithEventHandlerTest2 TEST");

			var patchClass = typeof(MarshalledWithEventHandlerTest2Class_Patch);
			ClassicAssert.NotNull(patchClass);

			var instance = new Harmony("test");
			ClassicAssert.NotNull(instance, "Harmony instance");
			var patcher = instance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(patcher, "Patch processor");
			var patched = patcher.Patch();
			ClassicAssert.AreEqual(1, patched.Count);
			ClassicAssert.NotNull(patched[0]);

			Console.WriteLine($"### MarshalledWithEventHandlerTest2 BEFORE");
			new MarshalledWithEventHandlerTest2Class().Run();
			Console.WriteLine($"### MarshalledWithEventHandlerTest2 AFTER");
		}
	}
}
