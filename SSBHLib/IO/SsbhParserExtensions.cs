namespace SSBHLib.IO
{
    /// <summary>
    /// Generated code for parsing specific SSBH types.
    /// Changes made to this file will be overridden.    
    /// </summary>
    public static class Parsers
    {
        public static Formats.Hlpb ParseHlpb(this SsbhParser parser)
        {
            return new Formats.Hlpb
            {
                Magic = default,            
                VersionMajor = default,            
                VersionMinor = default,            
                AimEntries = default,            
                InterpolationEntries = default,            
                List1 = default,            
                List2 = default,            
            };
        }

        public static Formats.HlpbRotateAim ParseHlpbRotateAim(this SsbhParser parser)
        {
            return new Formats.HlpbRotateAim
            {
                Name = default,            
                AimBoneName1 = default,            
                AimBoneName2 = default,            
                AimType1 = default,            
                AimType2 = default,            
                TargetBoneName1 = default,            
                TargetBoneName2 = default,            
                Unknown1 = default,            
                Unknown2 = default,            
                Unknown3 = default,            
                Unknown4 = default,            
                Unknown5 = default,            
                Unknown6 = default,            
                Unknown7 = default,            
                Unknown8 = default,            
                Unknown9 = default,            
                Unknown10 = default,            
                Unknown11 = default,            
                Unknown12 = default,            
                Unknown13 = default,            
                Unknown14 = default,            
                Unknown15 = default,            
                Unknown16 = default,            
                Unknown17 = default,            
                Unknown18 = default,            
                Unknown19 = default,            
                Unknown20 = default,            
                Unknown21 = default,            
                Unknown22 = default,            
            };
        }

        public static Formats.HlpbRotateInterpolation ParseHlpbRotateInterpolation(this SsbhParser parser)
        {
            return new Formats.HlpbRotateInterpolation
            {
                Name = default,            
                BoneName = default,            
                RootBoneName = default,            
                ParentBoneName = default,            
                DriverBoneName = default,            
                Type = default,            
                AoIx = default,            
                AoIy = default,            
                AoIz = default,            
                Quat1X = default,            
                Quat1Y = default,            
                Quat1Z = default,            
                Quat1W = default,            
                Quat2X = default,            
                Quat2Y = default,            
                Quat2Z = default,            
                Quat2W = default,            
                MinRangeX = default,            
                MinRangeY = default,            
                MinRangeZ = default,            
                MaxRangeX = default,            
                MaxRangeY = default,            
                MaxRangeZ = default,            
            };
        }

        public static Formats.Skel ParseSkel(this SsbhParser parser)
        {
            return new Formats.Skel
            {
                Magic = default,            
                MajorVersion = default,            
                MinorVersion = default,            
                BoneEntries = default,            
                WorldTransform = default,            
                InvWorldTransform = default,            
                Transform = default,            
                InvTransform = default,            
            };
        }

        public static Formats.SkelBoneEntry ParseSkelBoneEntry(this SsbhParser parser)
        {
            return new Formats.SkelBoneEntry
            {
                Name = default,            
                Id = default,            
                ParentId = default,            
                Type = default,            
            };
        }

        public static Formats.SkelMatrix ParseSkelMatrix(this SsbhParser parser)
        {
            return new Formats.SkelMatrix
            {
                M11 = default,            
                M12 = default,            
                M13 = default,            
                M14 = default,            
                M21 = default,            
                M22 = default,            
                M23 = default,            
                M24 = default,            
                M31 = default,            
                M32 = default,            
                M33 = default,            
                M34 = default,            
                M41 = default,            
                M42 = default,            
                M43 = default,            
                M44 = default,            
            };
        }

        public static Formats.Modl ParseModl(this SsbhParser parser)
        {
            return new Formats.Modl
            {
                Magic = default,            
                MajorVersion = default,            
                MinorVersion = default,            
                ModelFileName = default,            
                SkeletonFileName = default,            
                MaterialFileNames = default,            
                UnknownFileName = default,            
                MeshString = default,            
                ModelEntries = default,            
            };
        }

        public static Formats.ModlMaterialName ParseModlMaterialName(this SsbhParser parser)
        {
            return new Formats.ModlMaterialName
            {
                MaterialFileName = default,            
            };
        }

        public static Formats.ModlEntry ParseModlEntry(this SsbhParser parser)
        {
            return new Formats.ModlEntry
            {
                MeshName = default,            
                SubIndex = default,            
                MaterialName = default,            
            };
        }

        public static Formats.Rendering.Nrpd ParseNrpd(this SsbhParser parser)
        {
            return new Formats.Rendering.Nrpd
            {
                Magic = default,            
                MajorVersion = default,            
                MinorVersion = default,            
                FrameBufferContainers = default,            
                StateContainers = default,            
                RenderPasses = default,            
            };
        }

        public static Formats.Rendering.NrpdFrameBuffer ParseNrpdFrameBuffer(this SsbhParser parser)
        {
            return new Formats.Rendering.NrpdFrameBuffer
            {
                Name = default,            
                Width = default,            
                Height = default,            
                Unk1 = default,            
                Unk2 = default,            
                Unk3 = default,            
            };
        }

        public static Formats.Rendering.NrpdFrameBufferContainer ParseNrpdFrameBufferContainer(this SsbhParser parser)
        {
            return new Formats.Rendering.NrpdFrameBufferContainer
            {
                FrameBuffer = default,            
                Type = default,            
            };
        }

        public static Formats.Rendering.NrpdRenderPass ParseNrpdRenderPass(this SsbhParser parser)
        {
            return new Formats.Rendering.NrpdRenderPass
            {
                Name = default,            
                Offset2 = default,            
                Type2 = default,            
                Offset3 = default,            
                Type3 = default,            
                UnkString = default,            
                Type4 = default,            
                Padding = default,            
            };
        }

        public static Formats.Rendering.NrpdSampler ParseNrpdSampler(this SsbhParser parser)
        {
            return new Formats.Rendering.NrpdSampler
            {
                Name = default,            
                WrapS = default,            
                WrapT = default,            
                WrapR = default,            
                Unk4 = default,            
                Unk5 = default,            
                Unk6 = default,            
                Unk7 = default,            
                Unk8 = default,            
                Unk9 = default,            
                Unk10 = default,            
                Unk11 = default,            
                Unk12 = default,            
                Unk13 = default,            
                Unk14 = default,            
                Unk15 = default,            
                Unk16 = default,            
            };
        }

        public static Formats.Rendering.NrpdStateContainer ParseNrpdStateContainer(this SsbhParser parser)
        {
            return new Formats.Rendering.NrpdStateContainer
            {
                StateObject = default,            
                Type = default,            
            };
        }

        public static Formats.Rendering.Shdr ParseShdr(this SsbhParser parser)
        {
            return new Formats.Rendering.Shdr
            {
                Magic = default,            
                MajorVersion = default,            
                MinorVersion = default,            
                Shaders = default,            
            };
        }

        public static Formats.Rendering.ShdrShader ParseShdrShader(this SsbhParser parser)
        {
            return new Formats.Rendering.ShdrShader
            {
                Name = default,            
                Unk1 = default,            
                Unk2 = default,            
                ShaderBinary = default,            
                ShaderFileSize = default,            
                Padding1 = default,            
                Padding2 = default,            
            };
        }

        public static Formats.Meshes.Mesh ParseMesh(this SsbhParser parser)
        {
            return new Formats.Meshes.Mesh
            {
                Magic = default,            
                VersionMajor = default,            
                VersionMinor = default,            
                ModelName = default,            
                BoundingSphereX = default,            
                BoundingSphereY = default,            
                BoundingSphereZ = default,            
                BoundingSphereRadius = default,            
                MinBoundingBoxX = default,            
                MinBoundingBoxY = default,            
                MinBoundingBoxZ = default,            
                MaxBoundingBoxX = default,            
                MaxBoundingBoxY = default,            
                MaxBoundingBoxZ = default,            
                ObbCenterX = default,            
                ObbCenterY = default,            
                ObbCenterZ = default,            
                M11 = default,            
                M12 = default,            
                M13 = default,            
                M21 = default,            
                M22 = default,            
                M23 = default,            
                M31 = default,            
                M32 = default,            
                M33 = default,            
                ObbSizeX = default,            
                ObbSizeY = default,            
                ObbSizeZ = default,            
                UnkBounding0 = default,            
                Objects = default,            
                BufferSizes = default,            
                PolygonIndexSize = default,            
                VertexBuffers = default,            
                PolygonBuffer = default,            
                RiggingBuffers = default,            
                UnknownOffset = default,            
                UnknownSize = default,            
            };
        }

        public static Formats.Meshes.MeshAttribute ParseMeshAttribute(this SsbhParser parser)
        {
            return new Formats.Meshes.MeshAttribute
            {
                Index = default,            
                DataType = default,            
                BufferIndex = default,            
                BufferOffset = default,            
                Unk4 = default,            
                Unk5 = default,            
                Name = default,            
                AttributeStrings = default,            
            };
        }

        public static Formats.Meshes.MeshAttributeString ParseMeshAttributeString(this SsbhParser parser)
        {
            return new Formats.Meshes.MeshAttributeString
            {
                Name = default,            
            };
        }

        public static Formats.Meshes.MeshBoneBuffer ParseMeshBoneBuffer(this SsbhParser parser)
        {
            return new Formats.Meshes.MeshBoneBuffer
            {
                BoneName = default,            
                Data = default,            
            };
        }

        public static Formats.Meshes.MeshBuffer ParseMeshBuffer(this SsbhParser parser)
        {
            return new Formats.Meshes.MeshBuffer
            {
                Buffer = default,            
            };
        }

        public static Formats.Meshes.MeshObject ParseMeshObject(this SsbhParser parser)
        {
            return new Formats.Meshes.MeshObject
            {
                Name = default,            
                SubMeshIndex = default,            
                ParentBoneName = default,            
                VertexCount = default,            
                IndexCount = default,            
                Unk2 = default,            
                VertexOffset = default,            
                VertexOffset2 = default,            
                FinalBufferOffset = default,            
                BufferIndex = default,            
                Stride = default,            
                Stride2 = default,            
                Unk6 = default,            
                Unk7 = default,            
                ElementOffset = default,            
                Unk8 = default,            
                DrawElementType = default,            
                HasRigging = default,            
                Unk11 = default,            
                UnkBounding0 = default,            
                BoundingSphereX = default,            
                BoundingSphereY = default,            
                BoundingSphereZ = default,            
                BoundingSphereRadius = default,            
                MinBoundingBoxX = default,            
                MinBoundingBoxY = default,            
                MinBoundingBoxZ = default,            
                MaxBoundingBoxX = default,            
                MaxBoundingBoxY = default,            
                MaxBoundingBoxZ = default,            
                ObbCenterX = default,            
                ObbCenterY = default,            
                ObbCenterZ = default,            
                M11 = default,            
                M12 = default,            
                M13 = default,            
                M21 = default,            
                M22 = default,            
                M23 = default,            
                M31 = default,            
                M32 = default,            
                M33 = default,            
                ObbSizeX = default,            
                ObbSizeY = default,            
                ObbSizeZ = default,            
                Attributes = default,            
            };
        }

        public static Formats.Meshes.MeshRiggingGroup ParseMeshRiggingGroup(this SsbhParser parser)
        {
            return new Formats.Meshes.MeshRiggingGroup
            {
                Name = default,            
                SubMeshIndex = default,            
                Flags = default,            
                Buffers = default,            
            };
        }

        public static Formats.Materials.MatlAttribute ParseMatlAttribute(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute
            {
                ParamId = default,            
                OffsetToData = default,            
                DataType = default,            
                DataObject = default,            
            };
        }

        public static Formats.Materials.MatlEntry ParseMatlEntry(this SsbhParser parser)
        {
            return new Formats.Materials.MatlEntry
            {
                MaterialLabel = default,            
                Attributes = default,            
                MaterialName = default,            
            };
        }

        public static Formats.Materials.Matl ParseMatl(this SsbhParser parser)
        {
            return new Formats.Materials.Matl
            {
                Magic = default,            
                MajorVersion = default,            
                MinorVersion = default,            
                Entries = default,            
            };
        }

        public static Formats.Animation.Anim ParseAnim(this SsbhParser parser)
        {
            return new Formats.Animation.Anim
            {
                Magic = default,            
                VersionMajor = default,            
                VersionMinor = default,            
                FrameCount = default,            
                Unk1 = default,            
                Unk2 = default,            
                Name = default,            
                Animations = default,            
                Buffer = default,            
            };
        }

        public static Formats.Animation.AnimGroup ParseAnimGroup(this SsbhParser parser)
        {
            return new Formats.Animation.AnimGroup
            {
                Type = default,            
                Nodes = default,            
            };
        }

        public static Formats.Animation.AnimNode ParseAnimNode(this SsbhParser parser)
        {
            return new Formats.Animation.AnimNode
            {
                Name = default,            
                Tracks = default,            
            };
        }

        public static Formats.Animation.AnimTrack ParseAnimTrack(this SsbhParser parser)
        {
            return new Formats.Animation.AnimTrack
            {
                Name = default,            
                Flags = default,            
                FrameCount = default,            
                Unk3 = default,            
                DataOffset = default,            
                DataSize = default,            
            };
        }

        public static Formats.Materials.MatlAttribute.MatlBlendState ParseMatlBlendState(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute.MatlBlendState
            {
                Unk1 = default,            
                Unk2 = default,            
                BlendFactor1 = default,            
                Unk4 = default,            
                Unk5 = default,            
                BlendFactor2 = default,            
                Unk7 = default,            
                Unk8 = default,            
                Unk9 = default,            
                Unk10 = default,            
                Unk11 = default,            
                Unk12 = default,            
            };
        }

        public static Formats.Materials.MatlAttribute.MatlRasterizerState ParseMatlRasterizerState(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute.MatlRasterizerState
            {
                FillMode = default,            
                CullMode = default,            
                DepthBias = default,            
                Unk4 = default,            
                Unk5 = default,            
                Unk6 = default,            
                Unk7 = default,            
                Unk8 = default,            
            };
        }

        public static Formats.Materials.MatlAttribute.MatlSampler ParseMatlSampler(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute.MatlSampler
            {
                WrapS = default,            
                WrapT = default,            
                WrapR = default,            
                MinFilter = default,            
                MagFilter = default,            
                Unk6 = default,            
                Unk7 = default,            
                Unk8 = default,            
                Unk9 = default,            
                Unk10 = default,            
                Unk11 = default,            
                Unk12 = default,            
                LodBias = default,            
                MaxAnisotropy = default,            
            };
        }

        public static Formats.Materials.MatlAttribute.MatlString ParseMatlString(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute.MatlString
            {
                Text = default,            
            };
        }

        public static Formats.Materials.MatlAttribute.MatlUvTransform ParseMatlUvTransform(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute.MatlUvTransform
            {
                X = default,            
                Y = default,            
                Z = default,            
                W = default,            
                V = default,            
            };
        }

        public static Formats.Materials.MatlAttribute.MatlVector4 ParseMatlVector4(this SsbhParser parser)
        {
            return new Formats.Materials.MatlAttribute.MatlVector4
            {
                X = default,            
                Y = default,            
                Z = default,            
                W = default,            
            };
        }

    }
}
	
   