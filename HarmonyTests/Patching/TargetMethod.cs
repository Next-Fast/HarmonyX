using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;

namespace HarmonyLibTests.Patching
{
	[TestFixture, NonParallelizable]
	public class TargetMethod : TestLogger
	{
		[Test]
		public void Test_TargetMethod_Returns_Null()
		{
			var patchClass = typeof(Class15Patch);
			ClassicAssert.NotNull(patchClass);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var processor = harmonyInstance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(processor);

			Exception exception = null;
			try
			{
				ClassicAssert.NotNull(processor.Patch());
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			ClassicAssert.NotNull(exception);
			ClassicAssert.NotNull(exception.InnerException);
			ClassicAssert.True(exception.InnerException.Message.Contains("returned an unexpected result: null"));
		}

		[Test]
		public void Test_TargetMethod_Returns_Wrong_Type()
		{
			var patchClass = typeof(Class16Patch);
			ClassicAssert.NotNull(patchClass);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var processor = harmonyInstance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(processor);

			Exception exception = null;
			try
			{
				ClassicAssert.NotNull(processor.Patch());
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			ClassicAssert.NotNull(exception);
			ClassicAssert.NotNull(exception.InnerException);
			ClassicAssert.True(exception.InnerException.Message.Contains("has wrong return type"));
		}

		[Test]
		public void Test_TargetMethods_Returns_Null()
		{
			var patchClass = typeof(Class17Patch);
			ClassicAssert.NotNull(patchClass);

			var harmonyInstance = new Harmony("test");
			ClassicAssert.NotNull(harmonyInstance);

			var processor = harmonyInstance.CreateClassProcessor(patchClass);
			ClassicAssert.NotNull(processor);

			Exception exception = null;
			try
			{
				ClassicAssert.NotNull(processor.Patch());
			}
			catch (Exception ex)
			{
				exception = ex;
			}
			ClassicAssert.NotNull(exception);
			ClassicAssert.NotNull(exception.InnerException);
			ClassicAssert.True(exception.InnerException.Message.Contains("returned an unexpected result: some element was null"));
		}
	}
}
