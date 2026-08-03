using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mortar;

namespace FruitNinja;

public class PSPParticleManager
{
	public enum PARTICLE_TYPE
	{
		PARTICLE_TYPE_POINT,
		PARTICLE_TYPE_VORTEX,
		PARTICLE_TYPE_DIRECTIONAL,
		PARTICLE_TYPE_ANGULAR
	}

	public enum PARTICLE_COORD
	{
		PARTICLE_COORD_GLOBAL,
		PARTICLE_COORD_LOCAL
	}

	public float RepulsiveRadius;

	public float RepulsiveStrength;

	public Vector3 RepulsivePosition;

	private static PSPParticleManager instance = new PSPParticleManager();

	public PSPParticle[] particles;

	public ushort particleFreeList;

	public ushort rotStart;

	public uint m_activeParticles;

	public LinkedList<PSPParticleEmitter> particleEmitters = new LinkedList<PSPParticleEmitter>();

	public int numTemplates;

	public LinkedList<PSPParticleTemplate> particleTemplates = new LinkedList<PSPParticleTemplate>();

	private LinkedList<PToDraw> drawList = new LinkedList<PToDraw>();

	public int noet;

	public LinkedList<PSPEmitterTemplate> emitterTemplates = new LinkedList<PSPEmitterTemplate>();

	private static VertexPositionColorTexture[] verts = new VertexPositionColorTexture[6000];

	private static Color fcolour = default(Color);

	public static PSPParticleManager GetInstance()
	{
		return instance;
	}

	public PSPEmitterTemplate GetEmitterTemplate(int idx)
	{
		int num = 0;
		foreach (PSPEmitterTemplate emitterTemplate in emitterTemplates)
		{
			if (num == idx)
			{
				return emitterTemplate;
			}
			num++;
		}
		return null;
	}

	public void Update(float update)
	{
		Update(update, paused: false);
	}

	public void Update(float dt, bool paused)
	{
		_Update(dt, paused);
		_Draw(dt, paused, 0, foreal: false);
	}

	private void _Update(float dt, bool paused)
	{
		int num = 0;
		LinkedListNode<PSPParticleEmitter> linkedListNode = particleEmitters.First;
		while (linkedListNode != null)
		{
			PSPParticleEmitter value = linkedListNode.Value;
			if (value.enabled != 0 && value.rateScale != 0f && (!paused || value.updateEvenIfPaused))
			{
				value.Update(dt);
				num++;
			}
			LinkedListNode<PSPParticleEmitter> next = linkedListNode.Next;
			if (value.time >= value.tmplt.life && value.tmplt.life != 0f && (value.tmplt.life > 0f || value.tmplt.Ends()))
			{
				if (value.zeroer != null)
				{
					value.zeroer(null);
				}
				particleEmitters.Remove(linkedListNode);
			}
			linkedListNode = next;
		}
	}

	public void Draw(float dt, bool paused, int withDepth)
	{
		if (drawList.Count <= 0)
		{
			return;
		}
		MatrixManager.instance.Reset();
		MatrixManager.instance.UploadCurrentMatrices(quick: true);
		int num = 0;
		foreach (PToDraw draw in drawList)
		{
			if (draw.depth == withDepth)
			{
				draw.tex.Set();
				Mesh.DrawPrimitivesEx3((PrimitiveType)0, verts, draw.numPoints, draw.numPoints / 3, draw.offset, num++);
			}
		}
	}

	private void _Draw(float dt, bool paused, int withDepth, bool foreal)
	{
		//IL_04a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c0: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c5: Unknown result type (might be due to invalid IL or missing references)
		//IL_036e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0373: Unknown result type (might be due to invalid IL or missing references)
		//IL_0392: Unknown result type (might be due to invalid IL or missing references)
		//IL_0397: Unknown result type (might be due to invalid IL or missing references)
		//IL_0321: Unknown result type (might be due to invalid IL or missing references)
		//IL_0326: Unknown result type (might be due to invalid IL or missing references)
		//IL_0337: Unknown result type (might be due to invalid IL or missing references)
		//IL_033c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0570: Unknown result type (might be due to invalid IL or missing references)
		//IL_0575: Unknown result type (might be due to invalid IL or missing references)
		//IL_058f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0594: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0613: Unknown result type (might be due to invalid IL or missing references)
		//IL_0618: Unknown result type (might be due to invalid IL or missing references)
		//IL_0628: Unknown result type (might be due to invalid IL or missing references)
		//IL_062d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0676: Unknown result type (might be due to invalid IL or missing references)
		//IL_067b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0695: Unknown result type (might be due to invalid IL or missing references)
		//IL_069a: Unknown result type (might be due to invalid IL or missing references)
		//IL_06aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_06af: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_06d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_06f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_06fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0746: Unknown result type (might be due to invalid IL or missing references)
		//IL_074b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0765: Unknown result type (might be due to invalid IL or missing references)
		//IL_076a: Unknown result type (might be due to invalid IL or missing references)
		//IL_077a: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		drawList.Clear();
		int num = 0;
		int num2 = 0;
		m_activeParticles = 0u;
		float num3 = 0f;
		float num4 = 0f;
		foreach (PSPParticleTemplate particleTemplate in particleTemplates)
		{
			int num5 = particleTemplate.first_particle;
			if (num5 == 0)
			{
				continue;
			}
			ushort num6 = 0;
			float life = particleTemplate.life;
			do
			{
				PSPParticle pSPParticle = particles[num5];
				PSPParticle pSPParticle2 = pSPParticle;
				if (pSPParticle2.time <= pSPParticle2.lifeScale)
				{
					ushort num7 = (ushort)pSPParticle2.next_particle;
					if (num6 != 0)
					{
						particles[num6].next_particle = particles[num5].next_particle;
					}
					else
					{
						particleTemplate.first_particle = (ushort)particles[num5].next_particle;
					}
					particles[num5].next_particle = particleFreeList;
					particleFreeList = (ushort)num5;
					num5 = num7;
					continue;
				}
				num3 = (life - pSPParticle2.time) / life;
				float num8;
				if (num3 < 0.5f)
				{
					num4 = num3 * 2f;
					fcolour.R = (byte)((float)(int)pSPParticle2.Cs[0] + (float)pSPParticle2.Ci[0] * num4);
					fcolour.G = (byte)((float)(int)pSPParticle2.Cs[1] + (float)pSPParticle2.Ci[1] * num4);
					fcolour.B = (byte)((float)(int)pSPParticle2.Cs[2] + (float)pSPParticle2.Ci[2] * num4);
					fcolour.A = (byte)((float)(int)pSPParticle2.Cs[3] + (float)pSPParticle2.Ci[3] * num4);
					num8 = (float)(int)pSPParticle2.sizeS + (float)pSPParticle2.sizeInc * num4;
				}
				else
				{
					num4 = (num3 - 0.5f) * 2f;
					fcolour.R = (byte)((float)(pSPParticle2.Cs[0] + pSPParticle2.Ci[0]) + (float)pSPParticle2.Ci2[0] * num4);
					fcolour.G = (byte)((float)(pSPParticle2.Cs[1] + pSPParticle2.Ci[1]) + (float)pSPParticle2.Ci2[1] * num4);
					fcolour.B = (byte)((float)(pSPParticle2.Cs[2] + pSPParticle2.Ci[2]) + (float)pSPParticle2.Ci2[2] * num4);
					fcolour.A = (byte)((float)(pSPParticle2.Cs[3] + pSPParticle2.Ci[3]) + (float)pSPParticle2.Ci2[3] * num4);
					num8 = (float)(int)pSPParticle2.sizeS + (float)pSPParticle2.sizeInc + (float)pSPParticle2.sizeInc2 * num4;
				}
				float num9 = num8 * particleTemplate.xScaleRatio;
				float num10 = num8;
				if (!paused || (pSPParticle2.owner != null && pSPParticle2.owner.updateEvenIfPaused))
				{
					float num11 = Mortar.Math.floatLERP(pSPParticle2.spin_speed_start, pSPParticle2.spin_speed_end, num3);
					if (num11 != 0f)
					{
						pSPParticle.current_spin += (ushort)(int)(num11 * 182f * 360f * dt);
						if (pSPParticle.current_spin == 0)
						{
							pSPParticle.sinz = -1f;
							pSPParticle.cosz = 1f;
							pSPParticle.vecX = new Vector2(1f, 0f);
							pSPParticle.vecY = new Vector2(0f, 1f);
						}
						else
						{
							pSPParticle.vecX = new Vector2(Mortar.Math.SinIdx((ushort)(pSPParticle.current_spin + 16384)), Mortar.Math.CosIdx((ushort)(pSPParticle.current_spin + 16384)));
							pSPParticle.vecY = new Vector2(Mortar.Math.SinIdx(pSPParticle.current_spin), Mortar.Math.CosIdx(pSPParticle.current_spin));
							ushort idx = (ushort)((pSPParticle.current_spin + 57330) % 65520);
							pSPParticle.sinz = Mortar.Math.SinIdx(idx) * 1.41f;
							pSPParticle.cosz = Mortar.Math.CosIdx(idx) * 1.41f;
						}
					}
					float num12 = Mortar.Math.floatLERP(pSPParticle2.cycleX_speed_start, pSPParticle2.cycleX_speed_end, num3);
					if (num12 != 0f)
					{
						pSPParticle.current_x_scale_cycle += (ushort)(int)(num12 * 182f * 360f * dt);
					}
					if (pSPParticle.current_x_scale_cycle != 0)
					{
						num9 *= Mortar.Math.CosIdx(pSPParticle.current_x_scale_cycle);
					}
					float num13 = Mortar.Math.floatLERP(pSPParticle2.cycleY_speed_start, pSPParticle2.cycleY_speed_end, num3);
					if (num13 != 0f)
					{
						pSPParticle.current_y_scale_cycle += (ushort)(int)(num13 * 182f * 360f * dt);
					}
					if (pSPParticle.current_y_scale_cycle != 0)
					{
						num10 *= Mortar.Math.CosIdx(pSPParticle.current_y_scale_cycle);
					}
				}
				_ = pSPParticle2.sinz;
				_ = pSPParticle2.cosz;
				Vector2 val = pSPParticle.vecX * num9;
				Vector2 val2 = pSPParticle.vecY * num10;
				Vector3 pos = pSPParticle2.pos;
				pos.X += ((particleTemplate.coord_type == 1) ? pSPParticle2.owner.pos.X : 0f);
				pos.Y += ((particleTemplate.coord_type == 1) ? pSPParticle2.owner.pos.Y : 0f);
				pos.Z = withDepth;
				verts[num2].Position = new Vector3(pos.X + (val.X + val2.X), pos.Y + (val.Y + val2.Y), pos.Z);
				verts[num2].TextureCoordinate = new Vector2(1f, 0f);
				verts[num2].Color = fcolour;
				num2++;
				verts[num2].Position = new Vector3(pos.X + (0f - val.X + val2.X), pos.Y + (0f - val.Y + val2.Y), pos.Z);
				verts[num2].TextureCoordinate = new Vector2(0f, 0f);
				verts[num2].Color = fcolour;
				num2++;
				verts[num2].Position = new Vector3(pos.X + (val.X - val2.X), pos.Y + (val.Y - val2.Y), pos.Z);
				verts[num2].TextureCoordinate = new Vector2(1f, 1f);
				verts[num2].Color = fcolour;
				num2++;
				ref VertexPositionColorTexture reference = ref verts[num2];
				reference = verts[num2 - 1];
				num2++;
				ref VertexPositionColorTexture reference2 = ref verts[num2];
				reference2 = verts[num2 - 3];
				num2++;
				verts[num2].Position = new Vector3(pos.X + (0f - val.X - val2.X), pos.Y + (0f - val.Y - val2.Y), pos.Z);
				verts[num2].TextureCoordinate = new Vector2(0f, 1f);
				verts[num2].Color = fcolour;
				num2++;
				if (!paused || (pSPParticle2.owner != null && pSPParticle2.owner.updateEvenIfPaused))
				{
					pSPParticle.time = pSPParticle2.time - dt;
					float num14 = dt;
					if (dt > 0.025f)
					{
						dt /= 2f;
					}
					pSPParticle.vel.X = (pSPParticle2.vel.X + pSPParticle2.gravity.X * dt) * Mortar.Math.floatLERP(particleTemplate.friction_start.X, particleTemplate.friction_end.X, num3);
					pSPParticle.vel.Y = (pSPParticle2.vel.Y + pSPParticle2.gravity.Y * dt) * Mortar.Math.floatLERP(particleTemplate.friction_start.Y, particleTemplate.friction_end.Y, num3);
					pSPParticle.vel.Z = (pSPParticle2.vel.Z + pSPParticle2.gravity.Z * dt) * Mortar.Math.floatLERP(particleTemplate.friction_start.Z, particleTemplate.friction_end.Z, num3);
					pSPParticle.pos.X = pSPParticle2.pos.X + pSPParticle.vel.X * dt;
					pSPParticle.pos.Y = pSPParticle2.pos.Y + pSPParticle.vel.Y * dt;
					pSPParticle.pos.Z = pSPParticle2.pos.Z + pSPParticle.vel.Z * dt;
					dt = num14;
				}
				num6 = (ushort)num5;
				num5 = pSPParticle2.next_particle;
			}
			while (num5 != 0);
			int num15 = num2 - num;
			num2++;
			if (num15 != 0)
			{
				int num16 = (num2 - num) / 6;
				m_activeParticles += (uint)num16;
				drawList.AddLast(new PToDraw(num15, num, particleTemplate.tex, particleTemplate.useDepth));
			}
			num = num2;
		}
	}

	public bool EmitterExists(uint hash)
	{
		foreach (PSPEmitterTemplate emitterTemplate in emitterTemplates)
		{
			if (emitterTemplate.hash == hash)
			{
				return true;
			}
		}
		return false;
	}

	public PSPParticleEmitter AddEmitter(uint hash, Action<PSPParticleEmitter> zeroer)
	{
		PSPEmitterTemplate tmplt = null;
		foreach (PSPEmitterTemplate emitterTemplate in emitterTemplates)
		{
			if (emitterTemplate.hash == hash)
			{
				tmplt = emitterTemplate;
				break;
			}
		}
		PSPParticleEmitter pSPParticleEmitter = new PSPParticleEmitter();
		pSPParticleEmitter.updateEvenIfPaused = false;
		pSPParticleEmitter.tmplt = tmplt;
		pSPParticleEmitter.enabled = 1;
		pSPParticleEmitter.zeroer = zeroer;
		pSPParticleEmitter.cosz = 1f;
		pSPParticleEmitter.scale = 1f;
		pSPParticleEmitter.rateScale = 1f;
		pSPParticleEmitter.lifeScale = 1f;
		pSPParticleEmitter.sizeScale = (Game.IsMultiplayer() ? Game.SPLIT_SCREEN_SCALE : 1f);
		particleEmitters.AddFirst(pSPParticleEmitter);
		return pSPParticleEmitter;
	}

	public void ClearEmitters()
	{
		foreach (PSPParticleEmitter particleEmitter in particleEmitters)
		{
			if (particleEmitter.zeroer != null)
			{
				particleEmitter.zeroer(null);
			}
		}
		particleEmitters.Clear();
		if (particles != null)
		{
			int num = 1024;
			for (int i = 1; i < num; i++)
			{
				particles[i].next_particle = i + 1;
			}
			particles[num - 1].next_particle = 0;
		}
		particleFreeList = 1;
		if (particleTemplates == null)
		{
			return;
		}
		foreach (PSPParticleTemplate particleTemplate in particleTemplates)
		{
			particleTemplate.first_particle = 0;
		}
	}

	public void ClearEmitter(PSPParticleEmitter emitter)
	{
		if (emitter != null)
		{
			if (emitter.zeroer != null)
			{
				emitter.zeroer(null);
			}
			particleEmitters.Remove(emitter);
		}
	}

	public void Destroy()
	{
		ClearEmitters();
		particleTemplates.Clear();
		emitterTemplates.Clear();
		Delete.SAFE_DELETE_ARRAY(ref particles);
	}

	private int[] GetNumberListI(string str)
	{
		string[] array = str.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		int[] array2 = new int[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = MParser.ParseInt(array[i]);
		}
		return array2;
	}

	private float[] GetNumberListF(string str)
	{
		string[] array = str.Split(new char[1] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		float[] array2 = new float[array.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array2[i] = MParser.ParseFloat(array[i]);
		}
		return array2;
	}

	public void LoadFile(string texturedir, string filename)
	{
		//IL_0328: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0819: Unknown result type (might be due to invalid IL or missing references)
		//IL_081e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0825: Unknown result type (might be due to invalid IL or missing references)
		//IL_082a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0763: Unknown result type (might be due to invalid IL or missing references)
		//IL_0768: Unknown result type (might be due to invalid IL or missing references)
		//IL_076f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0774: Unknown result type (might be due to invalid IL or missing references)
		if (particles == null)
		{
			int num = 1024;
			particles = ArrayInit.CreateFilledArray<PSPParticle>(num);
			for (int i = 1; i < num; i++)
			{
				particles[i].next_particle = i + 1;
			}
			particles[num - 1].next_particle = 0;
			particleFreeList = 1;
		}
		bool flag = false;
		do
		{
			XDocument xDocument = MortarXml.Load(filename);
			if (xDocument == null)
			{
				continue;
			}
			flag = true;
			XElement element = xDocument.FirstChildElement("particle_file");
			element = element.FirstChildElement("body");
			XElement xElement = element.FirstChildElement("particleTemplate");
			numTemplates = 0;
			uint[] array = new uint[1024];
			while (xElement != null)
			{
				XElement xElement2 = xElement;
				PSPParticleTemplate pSPParticleTemplate = new PSPParticleTemplate();
				particleTemplates.AddLast(pSPParticleTemplate);
				array[numTemplates] = StringFunctions.StringHash(xElement2.Attribute("name").Value);
				xElement2.QueryIntAttribute("useDepth", ref pSPParticleTemplate.useDepth);
				string texture = string.Format("{0}/{1}.tex", texturedir, xElement2.FirstChildElement("texture").Attribute("name").Value);
				try
				{
					pSPParticleTemplate.tex = TextureManager.GetInstance().Load(texture);
				}
				catch (Exception)
				{
				}
				pSPParticleTemplate.xScaleRatio = (float)pSPParticleTemplate.tex.GetWidth() / (float)pSPParticleTemplate.tex.GetHeight();
				pSPParticleTemplate.life = MParser.ParseFloat(xElement2.FirstChildElement("life").Value) / 60f;
				if (xElement2.FirstChildElement("type") != null)
				{
					switch (xElement2.FirstChildElement("type").Value)
					{
					case "Point":
						pSPParticleTemplate.particle_type = 0;
						break;
					case "Vortex":
						pSPParticleTemplate.particle_type = 1;
						break;
					case "Direction":
						pSPParticleTemplate.particle_type = 2;
						break;
					case "Angular":
						pSPParticleTemplate.particle_type = 3;
						break;
					default:
						pSPParticleTemplate.particle_type = 0;
						break;
					}
				}
				else
				{
					pSPParticleTemplate.particle_type = 0;
				}
				if (xElement2.FirstChildElement("system") != null)
				{
					string value = xElement2.FirstChildElement("system").Value;
					if (value == "Local")
					{
						pSPParticleTemplate.coord_type = 1;
					}
					else if (value == "Global")
					{
						pSPParticleTemplate.coord_type = 0;
					}
					else
					{
						pSPParticleTemplate.coord_type = 0;
					}
				}
				else
				{
					pSPParticleTemplate.coord_type = 0;
				}
				int[] numberListI = GetNumberListI(xElement2.FirstChildElement("gravity").Value);
				pSPParticleTemplate.gravity_min.X = numberListI[0];
				pSPParticleTemplate.gravity_min.Y = numberListI[1];
				pSPParticleTemplate.gravity_min.Z = numberListI[2];
				if (xElement2.FirstChildElement("gravity_max") != null)
				{
					numberListI = GetNumberListI(xElement2.FirstChildElement("gravity_max").Value);
					pSPParticleTemplate.gravity_max.X = numberListI[0];
					pSPParticleTemplate.gravity_max.Y = numberListI[1];
					pSPParticleTemplate.gravity_max.Z = numberListI[2];
				}
				else
				{
					pSPParticleTemplate.gravity_max = pSPParticleTemplate.gravity_min;
				}
				XElement xElement3 = xElement2.FirstChildElement("color");
				numberListI = GetNumberListI(xElement3.Attribute("startMin").Value);
				int num2 = numberListI[0];
				int num3 = numberListI[1];
				int num4 = numberListI[2];
				int num5 = numberListI[3];
				pSPParticleTemplate.color_start_max[0] = (byte)((float)num2 * 8.226f);
				pSPParticleTemplate.color_start_max[1] = (byte)((float)num3 * 8.226f);
				pSPParticleTemplate.color_start_max[2] = (byte)((float)num4 * 8.226f);
				pSPParticleTemplate.color_start_max[3] = (byte)((float)num5 * 8.226f);
				numberListI = GetNumberListI(xElement3.Attribute("startMax").Value);
				num2 = numberListI[0];
				num3 = numberListI[1];
				num4 = numberListI[2];
				num5 = numberListI[3];
				pSPParticleTemplate.color_start_min[0] = (byte)((float)num2 * 8.226f);
				pSPParticleTemplate.color_start_min[1] = (byte)((float)num3 * 8.226f);
				pSPParticleTemplate.color_start_min[2] = (byte)((float)num4 * 8.226f);
				pSPParticleTemplate.color_start_min[3] = (byte)((float)num5 * 8.226f);
				if (xElement3.Attribute("endMin") != null && xElement3.Attribute("endMax") != null)
				{
					numberListI = GetNumberListI(xElement3.Attribute("endMin").Value);
					num2 = numberListI[0];
					num3 = numberListI[1];
					num4 = numberListI[2];
					num5 = numberListI[3];
					pSPParticleTemplate.color_end_max[0] = (byte)((float)num2 * 8.226f);
					pSPParticleTemplate.color_end_max[1] = (byte)((float)num3 * 8.226f);
					pSPParticleTemplate.color_end_max[2] = (byte)((float)num4 * 8.226f);
					pSPParticleTemplate.color_end_max[3] = (byte)((float)num5 * 8.226f);
					numberListI = GetNumberListI(xElement3.Attribute("endMax").Value);
					num2 = numberListI[0];
					num3 = numberListI[1];
					num4 = numberListI[2];
					num5 = numberListI[3];
					pSPParticleTemplate.color_end_min[0] = (byte)((float)num2 * 8.226f);
					pSPParticleTemplate.color_end_min[1] = (byte)((float)num3 * 8.226f);
					pSPParticleTemplate.color_end_min[2] = (byte)((float)num4 * 8.226f);
					pSPParticleTemplate.color_end_min[3] = (byte)((float)num5 * 8.226f);
				}
				else
				{
					pSPParticleTemplate.color_end_min = pSPParticleTemplate.color_start_min;
					pSPParticleTemplate.color_end_max = pSPParticleTemplate.color_start_max;
				}
				if (xElement3.Attribute("midMax") != null && xElement3.Attribute("midMin") != null)
				{
					numberListI = GetNumberListI(xElement3.Attribute("midMin").Value);
					num2 = numberListI[0];
					num3 = numberListI[1];
					num4 = numberListI[2];
					num5 = numberListI[3];
					pSPParticleTemplate.color_mid_max[0] = (byte)((float)num2 * 8.226f);
					pSPParticleTemplate.color_mid_max[1] = (byte)((float)num3 * 8.226f);
					pSPParticleTemplate.color_mid_max[2] = (byte)((float)num4 * 8.226f);
					pSPParticleTemplate.color_mid_max[3] = (byte)((float)num5 * 8.226f);
					numberListI = GetNumberListI(xElement3.Attribute("midMax").Value);
					num2 = numberListI[0];
					num3 = numberListI[1];
					num4 = numberListI[2];
					num5 = numberListI[3];
					pSPParticleTemplate.color_mid_min[0] = (byte)((float)num2 * 8.226f);
					pSPParticleTemplate.color_mid_min[3] = (byte)((float)num5 * 8.226f);
					pSPParticleTemplate.color_mid_min[1] = (byte)((float)num3 * 8.226f);
					pSPParticleTemplate.color_mid_min[2] = (byte)((float)num4 * 8.226f);
				}
				else
				{
					for (int j = 0; j < 4; j++)
					{
						pSPParticleTemplate.color_mid_min[j] = (byte)((pSPParticleTemplate.color_start_min[j] + pSPParticleTemplate.color_end_min[j]) / 2);
						pSPParticleTemplate.color_mid_max[j] = (byte)((pSPParticleTemplate.color_start_max[j] + pSPParticleTemplate.color_end_max[j]) / 2);
					}
				}
				pSPParticleTemplate.first_particle = 0;
				xElement3 = xElement2.FirstChildElement("friction");
				if (xElement3 != null && xElement3.Attribute("start") != null && xElement3.Attribute("end") != null)
				{
					pSPParticleTemplate.friction_start = Vector3.One;
					pSPParticleTemplate.friction_end = Vector3.One;
					float[] numberListF = GetNumberListF(xElement3.Attribute("start").Value);
					pSPParticleTemplate.friction_start.X = numberListF[0];
					pSPParticleTemplate.friction_start.Y = numberListF[1];
					pSPParticleTemplate.friction_start.Z = numberListF[2];
					numberListF = GetNumberListF(xElement3.Attribute("end").Value);
					pSPParticleTemplate.friction_end.X = numberListF[0];
					pSPParticleTemplate.friction_end.Y = numberListF[1];
					pSPParticleTemplate.friction_end.Z = numberListF[2];
				}
				else
				{
					pSPParticleTemplate.friction_start = Vector3.One;
					pSPParticleTemplate.friction_end = Vector3.One;
				}
				xElement3 = xElement2.FirstChildElement("size");
				pSPParticleTemplate.size_start_min = (byte)MParser.ParseInt(xElement3.Attribute("startMin").Value);
				pSPParticleTemplate.size_start_max = (byte)MParser.ParseInt(xElement3.Attribute("startMax").Value);
				pSPParticleTemplate.size_end_min = (byte)MParser.ParseInt(xElement3.Attribute("endMin").Value);
				pSPParticleTemplate.size_end_max = (byte)MParser.ParseInt(xElement3.Attribute("endMax").Value);
				if (xElement3.Attribute("midMax") != null && xElement3.Attribute("midMin") != null)
				{
					pSPParticleTemplate.size_mid_max = (byte)MParser.ParseInt(xElement3.Attribute("midMax").Value);
					pSPParticleTemplate.size_mid_min = (byte)MParser.ParseInt(xElement3.Attribute("midMin").Value);
				}
				else
				{
					pSPParticleTemplate.size_mid_max = (byte)((pSPParticleTemplate.size_start_max + pSPParticleTemplate.size_end_max) / 2);
					pSPParticleTemplate.size_mid_min = (byte)((pSPParticleTemplate.size_start_min + pSPParticleTemplate.size_end_min) / 2);
				}
				xElement3 = xElement2.FirstChildElement("cycleX");
				if (xElement3 != null && xElement3.Attribute("startMin") != null && xElement3.Attribute("startMax") != null)
				{
					pSPParticleTemplate.cycleX_start_min = (short)MParser.ParseInt(xElement3.Attribute("startMin").Value);
					pSPParticleTemplate.cycleX_start_max = (short)MParser.ParseInt(xElement3.Attribute("startMax").Value);
					if (xElement3 != null && xElement3.Attribute("endMin") != null && xElement3.Attribute("endMax") != null)
					{
						pSPParticleTemplate.cycleX_end_min = (short)MParser.ParseInt(xElement3.Attribute("endMin").Value);
						pSPParticleTemplate.cycleX_end_max = (short)MParser.ParseInt(xElement3.Attribute("endMax").Value);
					}
					else
					{
						pSPParticleTemplate.cycleX_end_min = pSPParticleTemplate.cycleX_start_min;
						pSPParticleTemplate.cycleX_end_max = pSPParticleTemplate.cycleX_start_max;
					}
				}
				else
				{
					pSPParticleTemplate.cycleX_start_min = 0;
					pSPParticleTemplate.cycleX_start_max = 0;
					pSPParticleTemplate.cycleX_end_min = 0;
					pSPParticleTemplate.cycleX_end_max = 0;
				}
				xElement3 = xElement2.FirstChildElement("cycleY");
				if (xElement3 != null && xElement3.Attribute("startMin") != null && xElement3.Attribute("startMax") != null)
				{
					pSPParticleTemplate.cycleY_start_min = (short)MParser.ParseInt(xElement3.Attribute("startMin").Value);
					pSPParticleTemplate.cycleY_start_max = (short)MParser.ParseInt(xElement3.Attribute("startMax").Value);
					if (xElement3 != null && xElement3.Attribute("endMin") != null && xElement3.Attribute("endMax") != null)
					{
						pSPParticleTemplate.cycleY_end_min = (short)MParser.ParseInt(xElement3.Attribute("endMin").Value);
						pSPParticleTemplate.cycleY_end_max = (short)MParser.ParseInt(xElement3.Attribute("endMax").Value);
					}
					else
					{
						pSPParticleTemplate.cycleY_end_min = pSPParticleTemplate.cycleY_start_min;
						pSPParticleTemplate.cycleY_end_max = pSPParticleTemplate.cycleY_start_max;
					}
				}
				else
				{
					pSPParticleTemplate.cycleY_start_min = 0;
					pSPParticleTemplate.cycleY_start_max = 0;
					pSPParticleTemplate.cycleY_end_min = 0;
					pSPParticleTemplate.cycleY_end_max = 0;
				}
				xElement3 = xElement2.FirstChildElement("angle");
				if (xElement3 != null && xElement3.Attribute("min") != null && xElement3.Attribute("max") != null)
				{
					pSPParticleTemplate.angleMin = MParser.ParseInt(xElement3.Attribute("min").Value);
					pSPParticleTemplate.angleMax = MParser.ParseInt(xElement3.Attribute("max").Value);
				}
				else
				{
					pSPParticleTemplate.angleMin = 0;
					pSPParticleTemplate.angleMax = 0;
				}
				xElement3 = xElement2.FirstChildElement("spin");
				if (xElement3 != null && xElement3.Attribute("startMin") != null && xElement3.Attribute("startMax") != null)
				{
					pSPParticleTemplate.spin_start_min = (short)MParser.ParseInt(xElement3.Attribute("startMin").Value);
					pSPParticleTemplate.spin_start_max = (short)MParser.ParseInt(xElement3.Attribute("startMax").Value);
					if (xElement3 != null && xElement3.Attribute("endMin") != null && xElement3.Attribute("endMax") != null)
					{
						pSPParticleTemplate.spin_end_min = (short)MParser.ParseInt(xElement3.Attribute("endMin").Value);
						pSPParticleTemplate.spin_end_max = (short)MParser.ParseInt(xElement3.Attribute("endMax").Value);
					}
					else
					{
						pSPParticleTemplate.spin_end_min = pSPParticleTemplate.spin_start_min;
						pSPParticleTemplate.spin_end_max = pSPParticleTemplate.spin_start_max;
					}
				}
				else
				{
					pSPParticleTemplate.spin_start_min = 0;
					pSPParticleTemplate.spin_start_max = 0;
					pSPParticleTemplate.spin_end_min = 0;
					pSPParticleTemplate.spin_end_max = 0;
				}
				_ = xElement2.FirstChildElement("SourceBlend").Value;
				xElement = xElement.NextSiblingElement("particleTemplate");
				numTemplates++;
			}
			noet = 0;
			for (xElement = element.FirstChildElement("emitter"); xElement != null; xElement = xElement.NextSiblingElement("emitter"))
			{
				XElement xElement4 = xElement;
				PSPEmitterTemplate pSPEmitterTemplate = new PSPEmitterTemplate();
				emitterTemplates.AddLast(pSPEmitterTemplate);
				pSPEmitterTemplate.hash = StringFunctions.StringHash(xElement4.Attribute("name").Value);
				pSPEmitterTemplate.name = xElement4.Attribute("name").Value;
				pSPEmitterTemplate.life = MParser.ParseFloat(xElement4.FirstChildElement("life").Value) / 60f;
				int num6 = 0;
				XElement xElement5 = xElement4.FirstChildElement("particleSet");
				while (xElement5 != null)
				{
					XElement xElement6 = xElement5;
					PSPParticleSet pSPParticleSet = new PSPParticleSet();
					pSPEmitterTemplate.sets.Add(pSPParticleSet);
					uint num7 = StringFunctions.StringHash(xElement6.Attribute("name").Value);
					int num8 = 0;
					foreach (PSPParticleTemplate particleTemplate in particleTemplates)
					{
						if (array[num8] == num7)
						{
							pSPParticleSet.template_idx = particleTemplate;
							break;
						}
						num8++;
					}
					XElement xElement7 = xElement6.FirstChildElement("time");
					pSPParticleSet.time_start = MParser.ParseFloat(xElement7.Attribute("start").Value);
					pSPParticleSet.time_end = MParser.ParseFloat(xElement7.Attribute("stop").Value);
					xElement7 = xElement6.FirstChildElement("particleNumber");
					pSPParticleSet.number_start = MParser.ParseByte(xElement7.Attribute("init").Value);
					pSPParticleSet.number_per_second = MParser.ParseByte(xElement7.Attribute("perSec").Value);
					float num9 = (int)pSPParticleSet.number_start;
					float num10 = (int)pSPParticleSet.number_per_second;
					num9 *= 0.75f;
					num10 *= 0.75f;
					pSPParticleSet.number_start = (byte)num9;
					pSPParticleSet.number_per_second = (byte)num10;
					xElement7 = xElement6.FirstChildElement("velocity");
					int[] numberListI2 = GetNumberListI(xElement7.Attribute("min").Value);
					pSPParticleSet.vel_min.X = numberListI2[0];
					pSPParticleSet.vel_min.Y = numberListI2[1];
					pSPParticleSet.vel_min.Z = numberListI2[2];
					numberListI2 = GetNumberListI(xElement7.Attribute("max").Value);
					pSPParticleSet.vel_max.X = numberListI2[0];
					pSPParticleSet.vel_max.Y = numberListI2[1];
					pSPParticleSet.vel_max.Z = numberListI2[2];
					xElement5 = xElement5.NextSiblingElement("particleSet");
					num6++;
				}
				pSPEmitterTemplate.particle_set_num = (byte)num6;
				noet++;
			}
		}
		while (!flag);
	}
}
