using HarmonyLib;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using System.Reflection.Emit;
using static HarmonyLib.Code;

namespace HarmonyLibTests.Tools
{
	[TestFixture]
	public class Test_Codes : TestLogger
	{
		[Test]
		public void Test_Basic_Code_Usage()
		{
			var code1 = Ldstr["hello"];
			ClassicAssert.AreEqual(OpCodes.Ldstr, code1.opcode);
			ClassicAssert.AreEqual("hello", code1.operand);
			ClassicAssert.AreEqual(0, code1.labels.Count);
			ClassicAssert.AreEqual(0, code1.blocks.Count);
			ClassicAssert.AreEqual(0, code1.jumpsFrom.Count);
			ClassicAssert.AreEqual(0, code1.jumpsTo.Count);
			ClassicAssert.AreEqual(null, code1.predicate);

			var code2 = Ldarg_0;
			ClassicAssert.AreEqual(OpCodes.Ldarg_0, code2.opcode);
			ClassicAssert.AreEqual(null, code2.operand);
			ClassicAssert.AreEqual(0, code2.labels.Count);
			ClassicAssert.AreEqual(0, code2.blocks.Count);
			ClassicAssert.AreEqual(0, code2.jumpsFrom.Count);
			ClassicAssert.AreEqual(0, code2.jumpsTo.Count);
			ClassicAssert.AreEqual(null, code2.predicate);
		}

		[Test]
		public void Test_CodeMatch_Usage()
		{
			var code = Ldstr["test", "foo"];
			var match = new CodeMatch(OpCodes.Ldstr, "test", "foo");
			ClassicAssert.AreEqual("[foo: opcodes=ldstr operands=test]", match.ToString());
			ClassicAssert.AreEqual("[foo: opcodes=ldstr operands=test]", code.ToString());
		}
	}
}
