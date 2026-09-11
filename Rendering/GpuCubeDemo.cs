using System.Numerics;
using Raylib_cs;

namespace GTron.Rendering;

public sealed unsafe class GpuCubeDemo : IDisposable
{
	private Mesh _mesh;
	private readonly Material _material;
	private readonly Shader _shader;

	public GpuCubeDemo(string vertexShaderPath, string fragmentShaderPath)
	{
		_shader = Raylib.LoadShader(vertexShaderPath, fragmentShaderPath);
		_mesh = Raylib.GenMeshCube(2.0f, 2.0f, 2.0f);

		Raylib.UploadMesh(ref _mesh, false);

		_material = Raylib.LoadMaterialDefault();
		_material.Shader = _shader;
		_material.Maps[(int)MaterialMapIndex.Diffuse].Color = Color.Lime;
	}

	public void Draw(Matrix4x4 transform)
	{
		Raylib.DrawMesh(_mesh, _material, transform);
	}

	public void Dispose()
	{
		Raylib.UnloadMaterial(_material);
		Raylib.UnloadShader(_shader);
		Raylib.UnloadMesh(_mesh);
	}
}