using System;
using SSBHLib.IO;

namespace SSBHLib.Formats
{
    public enum MTALParamID
    {
        Diffuse,
        Specular,
        Ambient,
        BlendMap,
        Transparency,
        DiffuseMapLayer1,
        CosinePower,
        SpecularPower,
        Fresnel,
        Roughness,
        EmissiveScale,
        EnableDiffuse,
        EnableSpecular,
        EnableAmbient,
        DiffuseMapLayer2,
        EnableTransparency,
        EnableOpacity,
        EnableCosinePower,
        EnableSpecularPower,
        EnableFresnel,
        EnableRoughness,
        EnableEmissiveScale,
        WorldMatrix,
        ViewMatrix,
        ProjectionMatrix,
        WorldViewMatrix,
        ViewInverseMatrix,
        ViewProjectionMatrix,
        WorldViewProjectionMatrix,
        WorldInverseTransposeMatrix,
        DiffuseMap,
        SpecularMap,
        AmbientMap,
        EmissiveMap,
        SpecularMapLayer1,
        TransparencyMap,
        NormalMap,
        DiffuseCubeMap,
        ReflectionMap,
        ReflectionCubeMap,
        RefractionMap,
        AmbientOcclusionMap,
        LightMap,
        AnisotropicMap,
        RoughnessMap,
        ReflectionMask,
        OpacityMask,
        UseDiffuseMap,
        UseSpecularMap,
        UseAmbientMap,
        UseEmissiveMap,
        UseTranslucencyMap,
        UseTransparencyMap,
        UseNormalMap,
        UseDiffuseCubeMap,
        UseReflectionMap,
        UseReflectionCubeMap,
        UseRefractionMap,
        UseAmbientOcclusionMap,
        UseLightMap,
        UseAnisotropicMap,
        UseRoughnessMap,
        UseReflectionMask,
        UseOpacityMask,
        DiffuseSampler,
        SpecularSampler,
        NormalSampler,
        ReflectionSampler,
        SpecularMapLayer2,
        NormalMapLayer1,
        NormalMapBC5,
        NormalMapLayer2,
        RoughnessMapLayer1,
        RoughnessMapLayer2,
        UseDiffuseUVTransform1,
        UseDiffuseUVTransform2,
        UseSpecularUVTransform1,
        UseSpecularUVTransform2,
        UseNormalUVTransform1,
        UseNormalUVTransform2,
        ShadowDepthBias,
        ShadowMap0,
        ShadowMap1,
        ShadowMap2,
        ShadowMap3,
        ShadowMap4,
        ShadowMap5,
        ShadowMap6,
        ShadowMap7,
        CastShadow,
        ReceiveShadow,
        ShadowMapSampler,
        Texture0,
        Texture1,
        Texture2,
        Texture3,
        Texture4,
        Texture5,
        Texture6,
        Texture7,
        Texture8,
        Texture9,
        Texture10,
        Texture11,
        Texture12,
        Texture13,
        Texture14,
        Texture15,
        Sampler0,
        Sampler1,
        Sampler2,
        Sampler3,
        Sampler4,
        Sampler5,
        Sampler6,
        Sampler7,
        Sampler8,
        Sampler9,
        Sampler10,
        Sampler11,
        Sampler12,
        Sampler13,
        Sampler14,
        Sampler15,
        CustomBuffer0,
        CustomBuffer1,
        CustomBuffer2,
        CustomBuffer3,
        CustomBuffer4,
        CustomBuffer5,
        CustomBuffer6,
        CustomBuffer7,
        CustomMatrix0,
        CustomMatrix1,
        CustomMatrix2,
        CustomMatrix3,
        CustomMatrix4,
        CustomMatrix5,
        CustomMatrix6,
        CustomMatrix7,
        CustomMatrix8,
        CustomMatrix9,
        CustomMatrix10,
        CustomMatrix11,
        CustomMatrix12,
        CustomMatrix13,
        CustomMatrix14,
        CustomMatrix15,
        CustomMatrix16,
        CustomMatrix17,
        CustomMatrix18,
        CustomMatrix19,
        CustomVector0,
        CustomVector1,
        CustomVector2,
        CustomVector3,
        CustomVector4,
        CustomVector5,
        CustomVector6,
        CustomVector7,
        CustomVector8,
        CustomVector9,
        CustomVector10,
        CustomVector11,
        CustomVector12,
        CustomVector13,
        CustomVector14,
        CustomVector15,
        CustomVector16,
        CustomVector17,
        CustomVector18,
        CustomVector19,
        CustomColor0,
        CustomColor1,
        CustomColor2,
        CustomColor3,
        CustomColor4,
        CustomColor5,
        CustomColor6,
        CustomColor7,
        CustomColor8,
        CustomColor9,
        CustomColor10,
        CustomColor11,
        CustomColor12,
        CustomColor13,
        CustomColor14,
        CustomColor15,
        CustomColor16,
        CustomColor17,
        CustomColor18,
        CustomColor19,
        CustomFloat0,
        CustomFloat1,
        CustomFloat2,
        CustomFloat3,
        CustomFloat4,
        CustomFloat5,
        CustomFloat6,
        CustomFloat7,
        CustomFloat8,
        CustomFloat9,
        CustomFloat10,
        CustomFloat11,
        CustomFloat12,
        CustomFloat13,
        CustomFloat14,
        CustomFloat15,
        CustomFloat16,
        CustomFloat17,
        CustomFloat18,
        CustomFloat19,
        CustomInteger0,
        CustomInteger1,
        CustomInteger2,
        CustomInteger3,
        CustomInteger4,
        CustomInteger5,
        CustomInteger6,
        CustomInteger7,
        CustomInteger8,
        CustomInteger9,
        CustomInteger10,
        CustomInteger11,
        CustomInteger12,
        CustomInteger13,
        CustomInteger14,
        CustomInteger15,
        CustomInteger16,
        CustomInteger17,
        CustomInteger18,
        CustomInteger19,
        CustomBoolean0,
        CustomBoolean1,
        CustomBoolean2,
        CustomBoolean3,
        CustomBoolean4,
        CustomBoolean5,
        CustomBoolean6,
        CustomBoolean7,
        CustomBoolean8,
        CustomBoolean9,
        CustomBoolean10,
        CustomBoolean11,
        CustomBoolean12,
        CustomBoolean13,
        CustomBoolean14,
        CustomBoolean15,
        CustomBoolean16,
        CustomBoolean17,
        CustomBoolean18,
        CustomBoolean19,
        UVTransform0,
        UVTransform1,
        UVTransform2,
        UVTransform3,
        UVTransform4,
        UVTransform5,
        UVTransform6,
        UVTransform7,
        UVTransform8,
        UVTransform9,
        UVTransform10,
        UVTransform11,
        UVTransform12,
        UVTransform13,
        UVTransform14,
        UVTransform15,
        DiffuseUVTransform1,
        DiffuseUVTransform2,
        SpecularUVTransform1,
        SpecularUVTransform2,
        NormalUVTransform1,
        NormalUVTransform2,
        DiffuseUVTransform,
        SpecularUVTransform,
        NormalUVTransform,
        UseDiffuseUVTransform,
        UseSpecularUVTransform,
        UseNormalUVTransform,
        BlendState0,
        BlendState1,
        BlendState2,
        BlendState3,
        BlendState4,
        BlendState5,
        BlendState6,
        BlendState7,
        BlendState8,
        BlendState9,
        BlendState10,
        RasterizerState0,
        RasterizerState1,
        RasterizerState2,
        RasterizerState3,
        RasterizerState4,
        RasterizerState5,
        RasterizerState6,
        RasterizerState7,
        RasterizerState8,
        RasterizerState9,
        RasterizerState10,
        ShadowColor,
        EmissiveMapLayer1,
        EmissiveMapLayer2,
        AlphaTestFunc,
        AlphaTestRef,
        Texture16,
        Texture17,
        Texture18,
        Texture19,
        Sampler16,
        Sampler17,
        Sampler18,
        Sampler19,
        CustomVector20,
        CustomVector21,
        CustomVector22,
        CustomVector23,
        CustomVector24,
        CustomVector25,
        CustomVector26,
        CustomVector27,
        CustomVector28,
        CustomVector29,
        CustomVector30,
        CustomVector31,
        CustomVector32,
        CustomVector33,
        CustomVector34,
        CustomVector35,
        CustomVector36,
        CustomVector37,
        CustomVector38,
        CustomVector39,
        CustomVector40,
        CustomVector41,
        CustomVector42,
        CustomVector43,
        CustomVector44,
        CustomVector45,
        CustomVector46,
        CustomVector47,
        CustomVector48,
        CustomVector49,
        CustomVector50,
        CustomVector51,
        CustomVector52,
        CustomVector53,
        CustomVector54,
        CustomVector55,
        CustomVector56,
        CustomVector57,
        CustomVector58,
        CustomVector59,
        CustomVector60,
        CustomVector61,
        CustomVector62,
        CustomVector63,
        UseBaseColorMap,
        UseMetallicMap,
        BaseColorMap,
        BaseColorMapLayer1,
        MetallicMap,
        MetallicMapLayer1,
        DiffuseLightingAOOffset
    }

    [SSBHFileAttribute("LTAM")]
    public class MTAL : ISSBH_File
    {
        public uint Magic { get; set; }

        public short MajorVersion { get; set; }

        public short MinorVersion { get; set; }

        public MTAL_Entry[] Entries { get; set; }
    }

    public class MTAL_Entry : ISSBH_File
    {
        public string MaterialLabel { get; set; }

        public MTAL_Attribute[] Attributes { get; set; }

        public string MaterialName { get; set; }
    }

    public class MTAL_Attribute : ISSBH_File
    {
        public MTALParamID ParamID { get; set; }

        public SSBHOffset OffsetToData { get; set; }

        public long DataType { get; set; }

        // not part of the entry
        public object DataObject;

        public override void PostProcess(SSBHParser R)
        {
            R.Seek(OffsetToData);
            if (DataType == 0x01)
            {
                DataObject = R.ReadSingle();
            }
            if (DataType == 0x02)
            {
                DataObject = R.ReadUInt32() == 1;
            }
            if (DataType == 0x05)
            {
                DataObject = R.Parse<MTAL_Vector4>();
            }
            if (DataType == 0x0B)
            {
                DataObject = R.Parse<MTAL_String>();
            }
            if (DataType == 0x0E)
            {
                DataObject = R.Parse<MTAL_Sampler>();
            }
            if (DataType == 0x10)
            {
                DataObject = R.Parse<MTAL_UVTransform>();
            }
            if (DataType == 0x11)
            {
                DataObject = R.Parse<MTAL_BlendState>();
            }
            if (DataType == 0x12)
            {
                DataObject = R.Parse<MTAL_RasterizerState>();
            }
        }

        private static string GetPropertyValues(System.Type type, object obj)
        {
            string result = "(";
            foreach (var property in type.GetProperties())
            {
                result += property.GetValue(obj).ToString() + ", ";
            }
            result = result.TrimEnd(',', ' ');
            result += ")";
            return result;
        }

        public class MTAL_Vector4 : ISSBH_File
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z}, {W})";
            }
        }

        public class MTAL_UVTransform : ISSBH_File
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float Z { get; set; }
            public float W { get; set; }
            public float V { get; set; }

            public override string ToString()
            {
                return $"({X}, {Y}, {Z}, {W}, {V})";
            }
        }

        public class MTAL_String : ISSBH_File
        {
            public string Text { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        public class MTAL_Sampler : ISSBH_File
        {
            public int WrapS { get; set; }
            public int WrapT { get; set; }
            public int WrapR { get; set; }
            public int Unk4 { get; set; }
            public int Unk5 { get; set; }
            public int Unk6 { get; set; }
            public int Unk7 { get; set; }
            public int Unk8 { get; set; }
            public int Unk9 { get; set; }
            public int Unk10 { get; set; }
            public int Unk11 { get; set; }
            public int Unk12 { get; set; }
            public float Unk13 { get; set; }
            public int Unk14 { get; set; }
            public int Unk15 { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }


        public class MTAL_BlendState : ISSBH_File
        {
            public int Unk1 { get; set; }

            public int Unk2 { get; set; }

            public int Unk3 { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int Unk6 { get; set; }

            public int Unk7 { get; set; }

            public int Unk8 { get; set; }

            public int Unk9 { get; set; }

            public int Unk10 { get; set; }

            public int Unk11 { get; set; }

            public int Unk12 { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }

        public class MTAL_RasterizerState : ISSBH_File
        {
            public int Unk1 { get; set; }

            public int Unk2 { get; set; }

            public int Unk3 { get; set; }

            public int Unk4 { get; set; }

            public int Unk5 { get; set; }

            public int Unk6 { get; set; }

            public int Unk7 { get; set; }

            public int Unk8 { get; set; }

            public override string ToString()
            {
                return GetPropertyValues(GetType(), this);
            }
        }
    }
}