using HarmonyLib;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System;

namespace HarmonyLibTests.IL
{
	[TestFixture, NonParallelizable]
	public class TestMethodBodyReader : TestLogger
	{
		public static void WeirdMethodWithGoto()
		{
			LABEL:
			try
			{
				for (; ; )
				{
				}
			}
			catch (Exception)
			{
				goto LABEL;
			}
		}

		[Test]
		public void Test_Read_WeirdMethodWithGoto()
		{
			var method = SymbolExtensions.GetMethodInfo(() => WeirdMethodWithGoto());
			ClassicAssert.NotNull(method);
			var instructions = PatchProcessor.GetOriginalInstructions(method);
			ClassicAssert.NotNull(instructions);
			ClassicAssert.Greater(instructions.Count, 0);
		}
	}
}
