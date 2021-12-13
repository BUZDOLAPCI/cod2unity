using UnityEngine;

using System.IO;
using System.Text;
using System.Collections.Generic;

namespace Cod2Unity
{
    public struct Lump
    {
        public string name;

        public uint offset;
        public uint length;
    };

    public struct TriangleSoup
    {
        public ushort materialID;

        public ushort drawOrder;

        public uint vertexOffset;
        public ushort vertexLength;

        public uint triangleOffset;
        public ushort triangleLength;
    };

    public class Triangle
    {
        public Triangle()
        {
            indices = new ushort[3];
        }

        public ushort[] indices;
    }

    public class Vertex
    {
        public Vertex()
        {
            position = new float[3];
            normal = new float[3];
            rgba = new byte[4];

            uv = new float[2];
            st = new float[2];

            unknown = new float[6];
        }

        public Vector3 PositionToVector3()
        {
            Vector3 pos = new Vector3(
                position[0],
                position[1],
                position[2]
            );

            return pos;
        }

        public Color RGBAtoColor()
        {
            Color pos = new Color(
                rgba[0],
                rgba[1],
                rgba[2],
                rgba[3]
            );

            return pos;
        }

        public Vector2 UVToVector2()
        {
            Vector2 _uv = new Vector2(
                uv[0],
                uv[1]
            );

            return _uv;
        }

        public float[] position;
        public float[] normal;
        public byte[] rgba;

        public float[] uv;
        public float[] st;

        public float[] unknown;
    }

    public class MapMaterial
    {
        public string name;
        public ulong flags;
    }

    // d_beachpebble 0x00000004   00b00000 - materials[currentSoup.materialID].flags
    // d_beachpebble 0x00b00000 0x00000004 - reference from: https://wiki.zeroy.com/index.php?title=Call_of_Duty_2:_d3dbsp
    public class LumpMaterialFlags
    {
        public const ulong none = 0x0000000100000000;
        public const ulong asphalt = 0x0000000101600000;
        public const ulong bark = 0x0000000100100000;
        public const ulong brick = 0x0000000100200000;
        public const ulong carpet = 0x0000000100300000;
        public const ulong cloth = 0x0000000100400000;
        public const ulong concrete = 0x0000000100500000;
        public const ulong dirt = 0x0000000100600000;
        public const ulong flesh = 0x0000000100700000;
        public const ulong foliage = 0x0000000200800000;
        public const ulong glass = 0x0000001000900000;
        public const ulong grass = 0x0000000100a00000;
        public const ulong gravel = 0x0000000100b00000;
        public const ulong ice = 0x0000000100c00000;
        public const ulong metal = 0x0000000100d00000;
        public const ulong mud = 0x0000000100e00000;
        public const ulong paper = 0x0000000100f00000;
        public const ulong plaster = 0x0000000101000000;
        public const ulong rock = 0x0000000101100000;
        public const ulong sand = 0x0000000101200000;
        public const ulong snow = 0x0000000101300000;
        public const ulong water = 0x0000002001400000;
        public const ulong wood = 0x0000000101500000;
        public const ulong missileClip = 0x0000008000000000;
        public const ulong bulletClip = 0x0000200000000000;
        public const ulong playerClip = 0x0001000000000000;
        public const ulong vehicleClip = 0x0000020000000000;
        public const ulong aiClip = 0x0002000000000000;
        public const ulong itemClip = 0x0000040000000000;
        public const ulong canShootClip = 0x0000004100000000;
        public const ulong aiSightClip = 0x0000100000000000;
        public const ulong noFallDamage = 0x0000000100000001;
        public const ulong noSteps = 0x0000000100002000;
        public const ulong noImpact = 0x0000000100000010;
        public const ulong noMarks = 0x0000000100000020;
        public const ulong noDrop = 0x8000000000000000;
        public const ulong slick = 0x0000000100000002;
        public const ulong ladder = 0x0000000100000008;
        public const ulong mantleOn = 0x0100000102000000;
        public const ulong mantleOver = 0x0100000104000000;
        public const ulong noLightmap = 0x0000000100000000;
        public const ulong noDynamicLight = 0x0000000100020000;
        public const ulong noCastShadow = 0x0000000100040000;
        public const ulong noDraw = 0x0000000100000080;
        public const ulong noFog = 0x0000000100000000;
        public const ulong drawToggle = 0x0000000100000000;
        public const ulong sky = 0x0000080000000004;
        public const ulong radialNormals = 0x0000000100000000;
        public const ulong nonColliding = 0x0000000400000000;
        public const ulong nonSolid = 0x0000000000004000;
        public const ulong transparent = 0x2000000100000000;
        public const ulong transparent2 = 0x2800000100000000;
        public const ulong detail = 0x0800000100000000;
        public const ulong structural = 0x1000000100000000;
        public const ulong portal = 0x0000000080000000;
        public const ulong occluder = 0x0000000100000000;
    }

    public class D3DBSP
    {
        Material defaultMaterial;

        Dictionary<int, string> lumpNames;

        List<Lump> lumps;
        List<TriangleSoup> triangleSoups;

        List<Vertex> vertices;
        List<Triangle> triangles;

        List<MapMaterial> materials;

        MemoryStream fs;
        BinaryReader br;

        MaterialCreator materialCreator;

        //[ContextMenu("Load map")]
        public void LoadMap(string mapName)
        {
            //Utils.ReadZipContentsToDictionaries();
            Load(mapName);
        }

        public bool ParseGameFolder(string gamePath)
        {
            return Utils.ReadZipContentsToDictionaries(gamePath);
        }

        void Load(string mapName)
        {
            lumpNames = new Dictionary<int, string>() {
                { 0, "Materials" },
                { 1, "Lightmaps" },
                { 2, "Light Grid Hash" },
                { 3, "Light Grid Values" },
                { 4, "Planes" },
                { 5, "Brushsides" },
                { 6, "Brushes" },
                { 7, "TriangleSoups" },
                { 8, "Vertices" },
                { 9, "Triangles" },
                { 10, "Cull Groups" },
                { 11, "Cull Group Indexes" },
                { 17, "Portal Verts" },
                { 18, "Occluder" },
                { 19, "Occluder Planes" },
                { 20, "Occluder Edges" },
                { 21, "Occluder Indexes" },
                { 22, "AABB Trees" },
                { 23, "Cells" },
                { 24, "Portals" },
                { 25, "Nodes" },
                { 26, "Leafs" },
                { 27, "Leaf Brushes" },
                { 29, "Collision Verts" },
                { 30, "Collision Edges" },
                { 31, "Collision Tris" },
                { 32, "Collision Borders" },
                { 33, "Collision Parts" },
                { 34, "Collision AABBs" },
                { 35, "Models" },
                { 36, "Visibility" },
                { 37, "Entities" },
                { 38, "Paths" },
            };

            lumps = new List<Lump>();
            materials = new List<MapMaterial>();
            vertices = new List<Vertex>();
            triangles = new List<Triangle>();
            triangleSoups = new List<TriangleSoup>();

            defaultMaterial = Resources.Load<Material>("Default");
            materialCreator = new MaterialCreator(defaultMaterial);

            using (fs = new MemoryStream(Utils.maps[mapName]))
            {
                using (br = new BinaryReader(fs, new ASCIIEncoding()))
                {
                    if (fs.CanRead)
                        StartReading(mapName);
                }
            }
        }

        private void StartReading(string mapName)
        {
            string ident = GetHeaderIdentifier();

            //string mapNameWithExt = fs.Name.Substring(fs.Name.LastIndexOf('/') + 1);
            //string mapName = mapNameWithExt.Substring(0, mapNameWithExt.IndexOf('.'));

            Debug.Log("File format " + ident + " has been detected on " + mapName);

            if (ident != "IBSP4")
                return;

            FillLumpList();
        }

        string GetHeaderIdentifier()
        {
            byte[] chunk;
            chunk = br.ReadBytes(5);

            StringBuilder ident = new StringBuilder();

            for (int i = 0; i < 4; i++)
            {
                ident.Append((char)chunk[i]);
            }
            ident.Append((int)chunk[4]);

            return ident.ToString();
        }

        private void FillLumpList()
        {
            br.BaseStream.Seek(8, SeekOrigin.Begin);

            for (int i = 0; i < 39; i++)
            {
                string lumpName;

                if (!lumpNames.ContainsKey(i))
                {
                    lumpName = "UNKNOWN";
                }
                else
                {
                    lumpName = lumpNames[i];
                }

                Lump l = new Lump();

                l.name = lumpName;
                l.length = br.ReadUInt32();
                l.offset = br.ReadUInt32();

                lumps.Insert(i, l);

                //Debug.Log("Lump[" + l.name + "] Length: " + l.length + " bytes | Offset: " + l.offset + " bytes");
            }

            CreateMeshMagic();
        }

        private void FillMaterialList()
        {
            br.BaseStream.Seek(lumps[0].offset, SeekOrigin.Begin);

            for (int i = 0; i < lumps[0].length; i++)
            {
                MapMaterial m = new MapMaterial();
                m.name = Encoding.ASCII.GetString(br.ReadBytes(64)).Replace("\0", string.Empty).Trim();
                m.flags = br.ReadUInt64();

                materials.Add(m);
            }
        }

        void FillSoupsList()
        {
            br.BaseStream.Seek(lumps[7].offset, SeekOrigin.Begin);

            for (int i = 0; i < lumps[7].length / 16; i++)
            {
                TriangleSoup t = new TriangleSoup();

                t.materialID = br.ReadUInt16();
                t.drawOrder = br.ReadUInt16();

                t.vertexOffset = br.ReadUInt32();
                t.vertexLength = br.ReadUInt16();

                t.triangleLength = br.ReadUInt16();
                t.triangleOffset = br.ReadUInt32();

                triangleSoups.Add(t);
            }
        }

        void FillVerticesList()
        {
            br.BaseStream.Seek(lumps[8].offset, SeekOrigin.Begin);

            for (int i = 0; i < lumps[8].length / 68; i++)
            {
                Vertex v = new Vertex();

                v.position[0] = br.ReadSingle();
                v.position[2] = br.ReadSingle(); // switch Y and Z, different engine
                v.position[1] = br.ReadSingle();

                v.normal[0] = br.ReadSingle();
                v.normal[1] = br.ReadSingle();
                v.normal[2] = br.ReadSingle();

                //BGRA
                v.rgba[2] = br.ReadByte();
                v.rgba[1] = br.ReadByte();
                v.rgba[0] = br.ReadByte();
                v.rgba[3] = br.ReadByte();

                v.uv[0] = br.ReadSingle();
                v.uv[1] = br.ReadSingle();

                v.st[0] = br.ReadSingle();
                v.st[1] = br.ReadSingle();

                // Unknown.. skip. Texture rotation?
                br.BaseStream.Seek(24, SeekOrigin.Current);

                vertices.Add(v);
            }
        }

        void FillTrianglesList()
        {
            br.BaseStream.Seek(lumps[9].offset, SeekOrigin.Begin);

            for (int i = 0; i < lumps[9].length / 6; i++)
            {
                Triangle t = new Triangle();

                t.indices[0] = br.ReadUInt16();
                t.indices[1] = br.ReadUInt16();
                t.indices[2] = br.ReadUInt16();

                triangles.Add(t);
            }
        }

        void CreateMeshMagic()
        {

            FillSoupsList();
            FillVerticesList();
            FillTrianglesList();
            FillMaterialList();

            List<Vector3> vertices = new List<Vector3>();
            List<Color> colors = new List<Color>();
            List<Vector2> uvs = new List<Vector2>();

            List<int> triangleIndices = new List<int>();

            // 1 soup per material/mesh
            // Each mesh has a triangle_length and triangle_offset
            // Triangle_length is how many triangles the mesh uses
            // Triangle_offset is the offset into the triangle array

            // Each soup also defines a vertex_offset
            // First you look up the current triangle
            // Then you use the index pointed to by the triangle, plus the vertex_offset, to find the correct vertex

            GameObject root = new GameObject("root_");
            for (int i = 0; i < this.triangleSoups.Count; i++)
            {
                TriangleSoup currentSoup = this.triangleSoups[i];

                GameObject go = new GameObject("_UNNAMED_");
                go.AddComponent<MeshRenderer>();
                go.AddComponent<MeshFilter>();

                go.transform.parent = root.transform;

                Mesh m = new Mesh();
                go.GetComponent<MeshFilter>().mesh = m;

                Vector3 boundMin = new Vector3(float.MinValue, float.MinValue, float.MinValue);
                Vector3 boundMax = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

                int tri_count = (int)currentSoup.triangleLength / 3;

                for (int j = 0; j < tri_count; j++)
                {
                    Triangle tri = this.triangles[(int)currentSoup.triangleOffset / 3 + j];

                    for (int vert_loop = 0; vert_loop < 3; vert_loop++)
                    {
                        int offset = (int)tri.indices[vert_loop];

                        Vector3 pos = this.vertices[(int)currentSoup.vertexOffset + offset].PositionToVector3();
                        pos *= 0.03f; // to correct scale
                        boundMin = Vector3.Max(boundMin, pos);
                        boundMax = Vector3.Min(boundMax, pos);
                        Vector2 uv = this.vertices[(int)currentSoup.vertexOffset + offset].UVToVector2();
                        Color col = this.vertices[(int)currentSoup.vertexOffset + offset].RGBAtoColor();

                        triangleIndices.Add(vertices.Count);
                        vertices.Add(pos);
                        colors.Add(col);
                        uvs.Add(uv);
                    }
                }

                Vector3 boundsCenter = (boundMin + boundMax) / 2;
                for (int v = 0; v < vertices.Count; v++)
                {
                    vertices[v] -= boundsCenter;
                }
                go.transform.position += boundsCenter;

                if (vertices.Count > 0)
                {
                    // Load required material here
                    Material newMat = materialCreator.CreateOrGetMaterial(materials[currentSoup.materialID].name);

                    go.name = newMat.name;
                    {
                        ulong curFlags = materials[currentSoup.materialID].flags;
                        if ((curFlags & LumpMaterialFlags.noDraw) != 0)
                        {
                            go.name += " _noDraw";
                            //go.SetActive(false);
                        }

                        if ((curFlags & LumpMaterialFlags.sky) != 0)
                        {
                            go.name += " _sky";
                            go.SetActive(false);
                        }
                    }


                    go.GetComponent<Renderer>().material = newMat;

                    m.vertices = vertices.ToArray();
                    m.colors = colors.ToArray();
                    m.triangles = triangleIndices.ToArray();
                    m.uv = uvs.ToArray();

                    m.RecalculateNormals();

                    vertices.Clear();
                    colors.Clear();
                    triangleIndices.Clear();
                    uvs.Clear();
                }
                else
                {
                    GameObject.DestroyImmediate(go);
                }
            }

            //Destroy(gameObject);
            UnityEditor.AssetDatabase.Refresh();
        }
    }
}