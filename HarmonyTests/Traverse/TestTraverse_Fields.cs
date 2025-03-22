using HarmonyLib;
using HarmonyLibTests.Assets;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace HarmonyLibTests.Tools
{
	[TestFixture, NonParallelizable]
	public class TestTraverse_Fields : TestLogger
	{
		// Traverse.ToString() should return the value of a traversed field
		//
		[Test]
		public void Traverse_Field_ToString()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);

			var trv = Traverse.Create(instance).Field(TraverseFields.fieldNames[0]);
			ClassicAssert.AreEqual(TraverseFields.testStrings[0], trv.ToString());
		}

		// Traverse.GetValue() should return the value of a traversed field
		// regardless of its access modifier
		//
		[Test]
		public void Traverse_Field_GetValue()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);
			var trv = Traverse.Create(instance);

			for (var i = 0; i < TraverseFields.testStrings.Length; i++)
			{
				var name = TraverseFields.fieldNames[i];
				var ftrv = trv.Field(name);
				ClassicAssert.NotNull(ftrv);

				ClassicAssert.AreEqual(TraverseFields.testStrings[i], ftrv.GetValue());
				ClassicAssert.AreEqual(TraverseFields.testStrings[i], ftrv.GetValue<string>());
			}
		}

		// Traverse.Field() should return the value of a traversed static field
		//
		[Test]
		public void Traverse_Field_Static()
		{
			var instance = new Traverse_BaseClass();

			var trv1 = Traverse.Create(instance).Field("staticField");
			ClassicAssert.AreEqual("test1", trv1.GetValue());


			var trv2 = Traverse.Create(typeof(TraverseFields_Static)).Field("staticField");
			ClassicAssert.AreEqual("test2", trv2.GetValue());
		}

		// Traverse.Field().Field() should continue the traverse chain for static and non-static fields
		//
		[Test]
		public void Traverse_Static_Field_Instance_Field()
		{
			var extra = new Traverse_ExtraClass("test1");
			ClassicAssert.AreEqual("test1", Traverse.Create(extra).Field("someString").GetValue());

			ClassicAssert.AreEqual("test2", TraverseFields_Static.extraClassInstance.someString, "direct");

			var trv = Traverse.Create(typeof(TraverseFields_Static));
			var trv2 = trv.Field("extraClassInstance");
			ClassicAssert.AreEqual(typeof(Traverse_ExtraClass), trv2.GetValue().GetType());
			ClassicAssert.AreEqual("test2", trv2.Field("someString").GetValue(), "traverse");
		}

		// Traverse.Field().Field() should continue the traverse chain for static and non-static fields
		//
		[Test]
		public void Traverse_Instance_Field_Static_Field()
		{
			var instance = new Traverse_ExtraClass("test3");
			ClassicAssert.AreEqual(typeof(Traverse_BaseClass), instance.baseClass.GetType());

			var trv1 = Traverse.Create(instance);
			ClassicAssert.NotNull(trv1, "trv1");

			var trv2 = trv1.Field("baseClass");
			ClassicAssert.NotNull(trv2, "trv2");

			var val = trv2.GetValue();
			ClassicAssert.NotNull(val, "val");
			ClassicAssert.AreEqual(typeof(Traverse_BaseClass), val.GetType());

			var trv3 = trv2.Field("baseField");
			ClassicAssert.NotNull(trv3, "trv3");
			ClassicAssert.AreEqual("base-field", trv3.GetValue());
		}

		// Traverse.SetValue() should set the value of a traversed field
		// regardless of its access modifier
		//
		[Test]
		public void Traverse_Field_SetValue()
		{
			var instance = new TraverseFields_AccessModifiers(TraverseFields.testStrings);
			var trv = Traverse.Create(instance);

			for (var i = 0; i < TraverseFields.testStrings.Length; i++)
			{
				var newValue = "newvalue" + i;

				// before
				ClassicAssert.AreEqual(TraverseFields.testStrings[i], instance.GetTestField(i));

				var name = TraverseFields.fieldNames[i];
				var ftrv = trv.Field(name);
				_ = ftrv.SetValue(newValue);

				// after
				ClassicAssert.AreEqual(newValue, instance.GetTestField(i));
				ClassicAssert.AreEqual(newValue, ftrv.GetValue());
				ClassicAssert.AreEqual(newValue, ftrv.GetValue<string>());
			}
		}
	}
}
