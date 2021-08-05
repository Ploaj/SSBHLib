using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SSBHLib.Test.UltimateVertexAttributeTests
{
    [TestClass]
    public class GetAttributeFromName
    {
        [TestMethod]
        public void Position0()
        {
            Assert.AreSame(UltimateVertexAttribute.Position0, UltimateVertexAttribute.GetAttributeDefaultsFromName("Position0"));
        }

        [TestMethod]
        public void Normal0()
        {
            Assert.AreSame(UltimateVertexAttribute.Normal0, UltimateVertexAttribute.GetAttributeDefaultsFromName("Normal0"));
        }

        [TestMethod]
        public void Tangent0()
        {
            Assert.AreSame(UltimateVertexAttribute.Tangent0, UltimateVertexAttribute.GetAttributeDefaultsFromName("Tangent0"));
        }

        [TestMethod]
        public void Map1()
        {
            Assert.AreSame(UltimateVertexAttribute.Map1, UltimateVertexAttribute.GetAttributeDefaultsFromName("map1"));
        }


        [TestMethod]
        public void UvSet()
        {
            Assert.AreSame(UltimateVertexAttribute.UvSet, UltimateVertexAttribute.GetAttributeDefaultsFromName("uvSet"));
        }

        [TestMethod]
        public void UvSet1()
        {
            Assert.AreSame(UltimateVertexAttribute.UvSet1, UltimateVertexAttribute.GetAttributeDefaultsFromName("uvSet1"));
        }

        [TestMethod]
        public void UvSet2()
        {
            Assert.AreSame(UltimateVertexAttribute.UvSet2, UltimateVertexAttribute.GetAttributeDefaultsFromName("uvSet2"));
        }

        [TestMethod]
        public void ColorSet1()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet1, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet1"));
        }

        [TestMethod]
        public void ColorSet2()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet2, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet2"));
        }

        [TestMethod]
        public void ColorSet21()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet21, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet2_1"));
        }

        [TestMethod]
        public void ColorSet22()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet22, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet2_2"));
        }

        [TestMethod]
        public void ColorSet23()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet23, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet2_3"));
        }

        [TestMethod]
        public void ColorSet3()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet3, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet3"));
        }

        [TestMethod]
        public void ColorSet4()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet4, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet4"));
        }

        [TestMethod]
        public void ColorSet5()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet5, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet5"));
        }

        [TestMethod]
        public void ColorSet6()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet6, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet6"));
        }

        [TestMethod]
        public void ColorSet7()
        {
            Assert.AreSame(UltimateVertexAttribute.ColorSet7, UltimateVertexAttribute.GetAttributeDefaultsFromName("colorSet7"));
        }

        [TestMethod]
        public void NullName()
        {
            var e = Assert.ThrowsException<NotImplementedException>(() => UltimateVertexAttribute.GetAttributeDefaultsFromName(null));
        }

        [TestMethod]
        public void EmptyName()
        {
            var e = Assert.ThrowsException<NotImplementedException>(() => UltimateVertexAttribute.GetAttributeDefaultsFromName(""));
        }

        [TestMethod]
        public void InvalidName()
        {
            var e = Assert.ThrowsException<NotImplementedException>(() => UltimateVertexAttribute.GetAttributeDefaultsFromName("position0"));
        }
    }
}
