using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class MeshManager
{
	private static MeshManager instance = new MeshManager();

	public static MeshManager GetInstance()
	{
		return instance;
	}

	public Model Load(string modelFileName)
	{
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0218: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_022b: Unknown result type (might be due to invalid IL or missing references)
		//IL_022d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0232: Unknown result type (might be due to invalid IL or missing references)
		//IL_0237: Unknown result type (might be due to invalid IL or missing references)
		//IL_023c: Unknown result type (might be due to invalid IL or missing references)
		//IL_023f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0243: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0339: Unknown result type (might be due to invalid IL or missing references)
		//IL_0361: Unknown result type (might be due to invalid IL or missing references)
		//IL_0363: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_038c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b3: Unknown result type (might be due to invalid IL or missing references)
		modelFileName = Path.ChangeExtension(modelFileName, ".wp7mesh");
		string directoryName = Path.GetDirectoryName(modelFileName);
		Model model = new Model();
		BinaryReader binaryReader = MortarFile.LoadBinBR(modelFileName);
		if (binaryReader == null)
		{
			return model;
		}
		byte[] d = binaryReader.ReadBytes((int)(binaryReader.BaseStream.Length - binaryReader.BaseStream.Position));
		binaryReader.Close();
		binaryReader = new BinaryReader(new MortarFile.ByteStream(d));
		d = null;
		Matrix val = default(Matrix);
		Vector3 val2 = default(Vector3);
		Quaternion val3 = default(Quaternion);
		Matrix identity = Matrix.Identity;
		val.M11 = binaryReader.ReadSingle();
		val.M12 = binaryReader.ReadSingle();
		val.M13 = binaryReader.ReadSingle();
		val.M14 = binaryReader.ReadSingle();
		val.M21 = binaryReader.ReadSingle();
		val.M22 = binaryReader.ReadSingle();
		val.M23 = binaryReader.ReadSingle();
		val.M24 = binaryReader.ReadSingle();
		val.M31 = binaryReader.ReadSingle();
		val.M32 = binaryReader.ReadSingle();
		val.M33 = binaryReader.ReadSingle();
		val.M34 = binaryReader.ReadSingle();
		val.M41 = binaryReader.ReadSingle();
		val.M42 = binaryReader.ReadSingle();
		val.M43 = binaryReader.ReadSingle();
		val.M44 = binaryReader.ReadSingle();
		val2.X = binaryReader.ReadSingle();
		val2.Y = binaryReader.ReadSingle();
		val2.Z = binaryReader.ReadSingle();
		val3.X = binaryReader.ReadSingle();
		val3.Y = binaryReader.ReadSingle();
		val3.Z = binaryReader.ReadSingle();
		val3.W = binaryReader.ReadSingle();
		identity.M11 = binaryReader.ReadSingle();
		identity.M12 = binaryReader.ReadSingle();
		identity.M13 = binaryReader.ReadSingle();
		identity.M21 = binaryReader.ReadSingle();
		identity.M22 = binaryReader.ReadSingle();
		identity.M23 = binaryReader.ReadSingle();
		identity.M31 = binaryReader.ReadSingle();
		identity.M32 = binaryReader.ReadSingle();
		identity.M33 = binaryReader.ReadSingle();
		Matrix val4 = identity * Matrix.Transpose(Matrix.CreateFromQuaternion(val3)) * Matrix.Transpose(Matrix.CreateTranslation(val2));
		model.amatrix = val * val4;
		int num = binaryReader.ReadInt32();
		Texture[] array = new Texture[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = binaryReader.ReadInt32();
			if (num2 > 0)
			{
				string text = new string(binaryReader.ReadChars(num2));
				array[i] = Texture.Load(directoryName + "\\" + text);
			}
		}
		int num3 = binaryReader.ReadInt32();
		model.meshes = new Model.Submesh[num3];
		for (int j = 0; j < num3; j++)
		{
			int num4 = binaryReader.ReadInt32();
			int num5 = binaryReader.ReadInt32();
			model.meshes[j].tex = array[num4];
			model.meshes[j].vertecies = new VertexPositionColorTexture[num5];
			for (int k = 0; k < num5; k++)
			{
				float num6 = binaryReader.ReadSingle();
				float num7 = binaryReader.ReadSingle();
				float num8 = binaryReader.ReadSingle();
				float num9 = binaryReader.ReadSingle();
				float num10 = binaryReader.ReadSingle();
				uint packedValue = binaryReader.ReadUInt32();
				Color color = default(Color);
				color.PackedValue = packedValue;
				model.meshes[j].vertecies[k].Color = color;
				model.meshes[j].vertecies[k].Position = new Vector3(num6, num7, num8);
				model.meshes[j].vertecies[k].TextureCoordinate = new Vector2(num9, num10);
			}
		}
		return model;
	}

	public void Initialise()
	{
	}

	public void Initialise(int heapsize)
	{
	}
}
