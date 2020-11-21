using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MonoTek.Graphics
{
    public interface IModel
    {
        List<Vector3> Verticies { get; }
        List<Vector2> Textures { get; }
        List<Vector3> Normals { get; }
        List<IFace> Faces { get; }
        IImage Texture { get; }

        IPixel Diffuse(Vector2 v);
        IPixel Diffuse(int x, int y);
        Vector2 UV(IFace face, int vert);


    }
    public class Model : IModel
    {
        protected List<Vector3> _verticies, _normals;
        protected List<Vector2> _textures;
        protected List<IFace> _faces;
        protected IImage _texture;

        public List<Vector3> Verticies => _verticies;
        public List<Vector2> Textures => _textures;
        public List<Vector3> Normals => _normals;
        public List<IFace> Faces => _faces;
        public IImage Texture => _texture;

        public Model()
        {
            _verticies = new List<Vector3>();
            _textures = new List<Vector2>();
            _normals = new List<Vector3>();
            _faces = new List<IFace>();
            _texture = Image.Create(1, 1);
        }

        public IPixel Diffuse(Vector2 v) => _texture[(int)v.X, (int)v.Y];
        public IPixel Diffuse(int x, int y) => _texture[x, y];

        public Vector2 UV(IFace face, int vert)
        {
            var index = face[vert][1];
            return new Vector2(_textures[index].X * _texture.Width, _textures[index].Y * _texture.Height);
        }

        protected IModel LoadModel(string filename)
        {
            var lines = File.ReadAllLines($"{filename}.obj");
            float v0 = 0.0f, v1 = 0.0f, v2 = 0.0f;
            foreach(var line in lines)
            {
                if (string.IsNullOrEmpty(line) || line.Contains("#")) continue;
                var type = line.Substring(0, 2).TrimEnd();
                var data = line.Substring(2).Trim().Split();
                switch(type)
                {
                    case "v": _verticies.Add(new Vector3
                    (
                        float.TryParse(data[0], out v0) ? v0 : throw new Exception($"Could not format string to float [{data[0]}]"),
                        float.TryParse(data[1], out v1) ? v1 : throw new Exception($"Could not format string to float [{data[1]}]"),
                        float.TryParse(data[2], out v2) ? v2 : throw new Exception($"Could not format string to float [{data[2]}]")
                    )); break;
                    case "vt": _textures.Add(new Vector2
                    (
                        float.TryParse(data[0], out v0) ? v0 : throw new Exception($"Could not format string to float [{data[0]}]"),
                        float.TryParse(data[1], out v1) ? v1 : throw new Exception($"Could not format string to float [{data[1]}]")
                    )); break;
                    case "vn": _normals.Add(new Vector3
                    (
                        float.TryParse(data[0], out v0) ? v0 : throw new Exception($"Could not format string to float [{data[0]}]"),
                        float.TryParse(data[1], out v1) ? v1 : throw new Exception($"Could not format string to float [{data[1]}]"),
                        float.TryParse(data[2], out v2) ? v2 : throw new Exception($"Could not format string to float [{data[2]}]")
                    )); break;
                    case "f": _faces.Add(new Face(data)); break;
                    default: break;
                }
            }
            _texture = Image.Load($"{filename}_diffuse");

            return this;
        }

        public static IModel Load(string filename) => new Model().LoadModel(filename);
    }


    public interface IFace
    {
        IFaceIndex this[int index] { get; set; }
        IEnumerable<IFaceIndex> Indices { get; }
    }
    public class Face : IFace
    {
        protected List<IFaceIndex> _indices;

        public IEnumerable<IFaceIndex> Indices => _indices;
        public IFaceIndex this[int index]
        {
            get => _indices[index];
            set => _indices[index] = value;
        }

        public Face(IEnumerable<string> lines) : this()
        {
            foreach(var line in lines)
            {
                _indices.Add((FaceIndex)line);
            }
        }
        public Face()
        {
            _indices = new List<IFaceIndex>();
        }
    }

    public interface IFaceIndex
    {
        int Vertex { get; }
        int Texture { get; }
        int Normal { get; }

        int this[int index] { get; }
    }
    public struct FaceIndex : IFaceIndex
    {
        private int _v, _vt, _vn;
        public int Vertex => _v;
        public int Texture => _vt;
        public int Normal => _vn;

        public int this[int index]
        {
            get => index == 0 ? _v : index == 1 ? _vt : _vn;
        }

        public FaceIndex(int v, int vt, int vn)
        {
            _v = v;
            _vt = vt;
            _vn = vn;
        }
        public FaceIndex(string[] line)
        {
            if (line.Length < 3) 
                throw new Exception($"Face requires 3 lines (v/vt/vn), Face() was given [{string.Join('/', line)}] instead");
            _v = int.TryParse(line[0], out var v) ? v : throw new Exception($"Could not format string to int [{line[0]}]");
            _vt = int.TryParse(line[1], out var vt) ? vt : throw new Exception($"Could not format string to int [{line[1]}]");
            _vn = int.TryParse(line[2], out var vn) ? vn : throw new Exception($"Could not format string to int [{line[2]}]");
        }
        public FaceIndex(string line)
        {
            if (!line.Contains('/')) 
                throw new Exception($"Line is not formatted properly (v/vt/vn v/vt/vn v/vt/vn), Face() was give [{line}] instead");
            string[] l = line.Split('/');
            if (l.Length < 3)
                throw new Exception($"Face requires 3 lines (v/vt/vn), Face() was given [{string.Join('/', l)}] instead");
            _v = int.TryParse(l[0], out var v) ? v - 1 : throw new Exception($"Could not format string to int [{l[0]}]");
            _vt = int.TryParse(l[1], out var vt) ? vt - 1 : throw new Exception($"Could not format string to int [{l[1]}]");
            _vn = int.TryParse(l[2], out var vn) ? vn - 1 : throw new Exception($"Could not format string to int [{l[2]}]");
        }

        public static implicit operator FaceIndex(string[] line) => new FaceIndex(line);
        public static implicit operator FaceIndex(string line) => new FaceIndex(line);
    }


    
}
