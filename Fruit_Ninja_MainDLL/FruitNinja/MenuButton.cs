using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class MenuButton : HUDControl3d
{
	public delegate void MenuCallback();

	public bool m_isTrialLockable;

	protected int m_goto;

	protected Entity m_link;

	protected int m_fruitType;

	protected MenuCallback m_callback;

	protected MenuCallback m_clickedCallback;

	protected int m_scaleInOut;

	protected bool m_inRegion;

	protected bool m_inOuterRegion;

	protected bool m_prevInRegion;

	protected bool m_prevInOuterRegion;

	protected int m_touchInRegion;

	protected Vector3 m_touchPos;

	protected float m_scratchRot;

	protected float m_scratchScale;

	protected bool m_scratchFlip;

	protected float m_rotationSpeed;

	protected static Texture m_scratchTexture;

	protected static Texture s_newTexture;

	protected float m_loadingSymbolTime;

	protected float m_newSymbol;

	protected Vector3 m_originalEntityScale;

	public static Texture m_loadingTexture;

	public bool m_growOnPressed;

	public bool m_clearOthers;

	public Vector3 m_originalScale;

	public bool m_hasOwnScale;

	public bool m_enabled;

	public Entity m_entity;

	public bool m_triggerOnBackPress;

	public float m_overallScratchScale;

	public Vector3 m_newSymbolOffset;

	public float m_innerBoundX;

	public float m_innerBoundY;

	public float m_outerBound;

	public float m_shakeTime;

	private static GameVertex[] symbo_tris = new GameVertex[48];

	public static float POP_IN_OUT_TIME => 0.15f;

	public static ushort POP_SINE => Math.DEGREE_TO_IDX(90f);

	public static float SCRATCH_SCALE => 1.125f;

	public static float BOB_TIME => 0.5f;

	public static float BOB_AMT => 6f;

	public static void MenuCallbackClicked()
	{
	}

	public void SetInnerBound(float bound)
	{
		m_innerBoundX = bound;
		m_innerBoundY = bound;
	}

	public void SetInnerBound(float boundX, float boundY)
	{
		m_innerBoundX = boundX;
		m_innerBoundY = boundY;
	}

	private void UpdateTouchPosition()
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		if (m_touchInRegion != -1)
		{
			m_touchPos = Game.game_work.touchPositions[m_touchInRegion];
		}
	}

	public MenuButton(Texture texture, Vector3 pos, MenuCallback call)
		: this(texture, pos, call, -1)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public MenuButton(Texture texture, Vector3 pos, MenuCallback call, int fruitType)
		: this(texture, pos, call, fruitType, Vector3.Zero)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public MenuButton(Texture texture, Vector3 pos, MenuCallback call, int fruitType, Vector3 scale)
		: this(texture, pos, call, fruitType, scale, MenuCallbackClicked)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public MenuButton(Texture texture, Vector3 pos, MenuCallback call, int fruitType, Vector3 scale, MenuCallback clickedCall)
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		m_texture = texture;
		Init(pos, call, fruitType, scale, clickedCall);
	}

	public MenuButton(string texture, Vector3 pos, MenuCallback call)
		: this(texture, pos, call, -1)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)


	public MenuButton(string texture, Vector3 pos, MenuCallback call, int fruitType)
		: this(texture, pos, call, fruitType, Vector3.Zero, localise: false)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public MenuButton(string texture, Vector3 pos, MenuCallback call, int fruitType, Vector3 scale, bool localise)
		: this(texture, pos, call, fruitType, scale, localise, MenuCallbackClicked)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0006: Unknown result type (might be due to invalid IL or missing references)


	public MenuButton(string texture, Vector3 pos, MenuCallback call, int fruitType, Vector3 scale, bool localise, MenuCallback clickedCall)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (localise)
		{
			m_texture = TextureManager.GetInstance().Load(texture, localise);
		}
		else
		{
			string texture2 = "textureswp7/" + texture;
			try
			{
				m_texture = TextureManager.GetInstance().Load(texture2);
			}
			catch
			{
			}
		}
		Init(pos, call, fruitType, scale, clickedCall);
	}

	public void Init(Vector3 pos, MenuCallback call, int fruitType, Vector3 scale, MenuCallback clickedCall)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0075: Unknown result type (might be due to invalid IL or missing references)
		//IL_0467: Unknown result type (might be due to invalid IL or missing references)
		//IL_046c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0488: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0450: Unknown result type (might be due to invalid IL or missing references)
		//IL_0455: Unknown result type (might be due to invalid IL or missing references)
		//IL_016e: Unknown result type (might be due to invalid IL or missing references)
		//IL_016f: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03da: Unknown result type (might be due to invalid IL or missing references)
		//IL_03df: Unknown result type (might be due to invalid IL or missing references)
		//IL_0202: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0314: Unknown result type (might be due to invalid IL or missing references)
		//IL_0319: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_0406: Unknown result type (might be due to invalid IL or missing references)
		//IL_040b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0378: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		m_pos = pos;
		m_fruitType = fruitType;
		m_callback = call;
		m_clickedCallback = clickedCall;
		m_selfCleanUp = false;
		m_clearOthers = true;
		m_triggerOnBackPress = false;
		m_hasOwnScale = Math.Abs(scale.X) + Math.Abs(scale.Y) > 0f;
		m_scale = Vector3.Zero;
		m_originalScale = scale;
		m_link = null;
		m_originalEntityScale = scale;
		m_touchInRegion = -1;
		m_inRegion = false;
		m_prevInRegion = false;
		m_inOuterRegion = false;
		m_prevInOuterRegion = false;
		m_rotationSpeed = 0f;
		m_overallScratchScale = 1f;
		m_growOnPressed = true;
		m_enabled = true;
		m_loadingSymbolTime = -1f;
		m_newSymbol = -1f;
		m_innerBoundX = 5f;
		m_innerBoundY = 5f;
		m_outerBound = 100f;
		m_shakeTime = 0f;
		m_rotation = 0f;
		if (m_fruitType >= 0)
		{
			m_scratchFlip = Math.g_random.Rand32(2) != 0;
			m_scratchRot = Math.g_random.Rand32(40) - 20;
			m_link = ActorManager.GetInstance().Add((m_fruitType >= Fruit.MAX_FRUIT_TYPES) ? EntityTypes.ENTITY_BOMB : EntityTypes.ENTITY_BEGIN);
			m_link.m_pos = pos;
			m_link.m_vel = Vector3.Zero;
			m_link.Init(null, m_fruitType, null);
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_BEFORE_SPLAT;
			m_rotationSpeed = Math.g_random.RandF(4f) + 8f;
			if (Math.g_random.Rand32(2) == 0)
			{
				m_rotationSpeed = 0f - m_rotationSpeed;
			}
			if (m_fruitType < Fruit.MAX_FRUIT_TYPES)
			{
				ref Vector3 reference = ref ((Fruit)m_link).m_rotation_speed[0];
				reference *= 0.2f;
				float num = Math.MAX(Math.Abs(((Fruit)m_link).m_rotation_speed[0].X), 0.75f);
				((Fruit)m_link).m_rotation_speed[0].X = num * (float)Math.MATH_SIGN(((Fruit)m_link).m_rotation_speed[0].X);
				float num2 = Math.MAX(Math.Abs(((Fruit)m_link).m_rotation_speed[0].Y), 0.5f);
				((Fruit)m_link).m_rotation_speed[0].Y = num2 * (float)Math.MATH_SIGN(((Fruit)m_link).m_rotation_speed[0].Y);
				ref Vector3 reference2 = ref ((Fruit)m_link).m_rotation_speed[1];
				reference2 = ((Fruit)m_link).m_rotation_speed[0];
				((Fruit)m_link).EnableGravity(state: false);
				((Fruit)m_link).m_hudControl = this;
				((Fruit)m_link).m_isMenuItem = true;
				((Fruit)m_link).m_z = 150f;
				if (!m_hasOwnScale)
				{
					m_originalScale = m_link.m_cur_scale * 200f;
				}
			}
			else
			{
				((Bomb)m_link).EnableGravity(state: false);
				((Bomb)m_link).SetCallback(call, this);
				((Bomb)m_link).m_z = 150f;
				Entity link = m_link;
				link.m_cur_scale *= 0.85f;
				if (!m_hasOwnScale)
				{
					m_originalScale = Vector3.One * 2f * Game.game_work.bombSize;
				}
			}
		}
		else if (!m_hasOwnScale)
		{
			m_originalScale.X = m_texture.GetWidth() + 1;
			m_originalScale.Y = m_texture.GetHeight() + 1;
			m_scale = m_originalScale;
		}
		m_entity = m_link;
		m_originalEntityScale = Vector3.Zero;
		m_scaleInOut = 0;
		m_newSymbolOffset = new Vector3(0.85f, 0.85f, 0f);
	}

	~MenuButton()
	{
		Release();
	}

	public void SetCallback(MenuCallback call)
	{
		m_callback = call;
	}

	public virtual void Clicked()
	{
	}

	public override void Reset()
	{
		if (m_link != null && m_fruitType < Fruit.MAX_FRUIT_TYPES)
		{
			((Fruit)m_link).m_isLockedMenuButton = m_isTrialLockable;
		}
		if (partOfPopup && m_link != null)
		{
			if (m_fruitType < Fruit.MAX_FRUIT_TYPES)
			{
				((Fruit)m_link).partOfPopup = true;
			}
			else
			{
				((Bomb)m_link).partOfPopup = true;
			}
		}
	}

	public override void Release()
	{
		if (m_entity != null && m_fruitType < Fruit.MAX_FRUIT_TYPES)
		{
			((Fruit)m_entity).m_hudControl = null;
		}
		if (m_entity != null && m_fruitType == Fruit.MAX_FRUIT_TYPES)
		{
			((Bomb)m_entity).m_hudControl = null;
		}
		m_texture = null;
	}

	public override void Init()
	{
		Reset();
	}

	public void TouchReleased()
	{
		if (!PopOverControl.IsInPopup || partOfPopup)
		{
			if (m_fruitType < 0)
			{
				m_callback();
			}
			else if (m_link != null)
			{
				Game.game_work.tutorialControl.ButtonPressedAtPos(this);
			}
			m_clickedCallback();
		}
	}

	public override void Update(float dt)
	{
		//IL_060b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0610: Unknown result type (might be due to invalid IL or missing references)
		//IL_0127: Unknown result type (might be due to invalid IL or missing references)
		//IL_0143: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_0104: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_0114: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_03f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0349: Unknown result type (might be due to invalid IL or missing references)
		//IL_0154: Unknown result type (might be due to invalid IL or missing references)
		//IL_0159: Unknown result type (might be due to invalid IL or missing references)
		//IL_0396: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0329: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_017a: Unknown result type (might be due to invalid IL or missing references)
		//IL_017f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_024d: Unknown result type (might be due to invalid IL or missing references)
		//IL_019f: Unknown result type (might be due to invalid IL or missing references)
		//IL_01af: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0511: Unknown result type (might be due to invalid IL or missing references)
		//IL_0516: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0290: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_05f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_05dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_05e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_05eb: Unknown result type (might be due to invalid IL or missing references)
		if (m_loadingSymbolTime >= 0f)
		{
			m_loadingSymbolTime += dt * 8f;
			if (m_loadingSymbolTime >= 8f)
			{
				m_loadingSymbolTime = 0f;
			}
		}
		if (m_newSymbol >= 0f)
		{
			m_newSymbol += dt / BOB_TIME;
			if (m_loadingSymbolTime >= 1f)
			{
				m_newSymbol = 0f;
			}
		}
		if (m_fruitType >= 0)
		{
			if (dt > 0f)
			{
				m_rotation += m_rotationSpeed * dt;
				if (m_rotation < 0f)
				{
					m_rotation += 360f;
				}
				if (m_rotation > 360f)
				{
					m_rotation -= 360f;
				}
			}
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_BEFORE_SPLAT;
			if (m_link != null)
			{
				if (m_originalEntityScale.X == 0f)
				{
					m_originalEntityScale = m_link.m_cur_scale;
					m_link.m_cur_scale = Vector3.Zero;
				}
				else
				{
					m_link.m_cur_scale = m_originalEntityScale * (m_scale.X / m_originalScale.X);
				}
				m_link.m_pos = m_pos;
				if (m_fruitType < Fruit.MAX_FRUIT_TYPES)
				{
					((Fruit)m_link).m_pos2 = m_pos;
					if (((Fruit)m_link).m_isSliced)
					{
						Vector3 val = m_link.m_vel - ((Fruit)m_link).m_vel2;
						if (val.LengthSquared() > 0.1f)
						{
							m_callback();
							Game.game_work.tutorialControl.ResetTutePos();
							m_link.m_cur_scale = m_originalEntityScale;
							if (m_clearOthers)
							{
								Game.ClearMenuItems();
							}
						}
						m_link = null;
					}
				}
				else
				{
					m_pos.Z = -5f;
					m_link.m_pos.Z = 0f;
					if (!((Bomb)m_link).Enabled())
					{
						m_link.m_cur_scale = m_originalEntityScale;
						m_link = null;
					}
				}
				if (m_scaleInOut < POP_SINE)
				{
					m_scaleInOut = Math.MIN(POP_SINE, (int)((float)m_scaleInOut + dt * ((float)(int)POP_SINE / POP_IN_OUT_TIME)));
					m_scale = m_originalScale * Math.SinIdx((ushort)m_scaleInOut) / Math.SinIdx(POP_SINE);
				}
				else
				{
					m_scale = m_originalScale;
				}
			}
			else
			{
				if (m_entity != null && m_entity.m_vel.X == 0f && m_entity.m_vel.Y == 0f)
				{
					m_entity.m_cur_scale = m_originalEntityScale * (m_scale.X / m_originalScale.X);
				}
				else if (m_entity != null)
				{
					m_entity.m_cur_scale = m_originalEntityScale;
				}
				int num = (int)(dt * ((float)(int)POP_SINE / POP_IN_OUT_TIME));
				m_scaleInOut -= num;
				if (m_scaleInOut <= 0)
				{
					m_scaleInOut = 0;
					m_terminate = true;
				}
				m_pos.Z = -5f;
				m_scale = m_originalScale * Math.SinIdx((ushort)m_scaleInOut) / Math.SinIdx(POP_SINE);
			}
		}
		if (m_enabled)
		{
			if (m_link != null && TheGame.instance.BackButtonWasPressed && m_triggerOnBackPress)
			{
				Vector3 proj = new Vector3(1f, 0f, 0f);
				m_link.CollisionResponse(null, 0u, 0u, ref proj);
			}
			else if (TheGame.instance.BackButtonWasPressed && m_triggerOnBackPress)
			{
				TouchReleased();
			}
			float innerBoundX = m_innerBoundX;
			float innerBoundY = m_innerBoundY;
			float num2 = m_pos.X - m_originalScale.X * 0.5f - innerBoundX;
			float num3 = m_pos.X + m_originalScale.X * 0.5f + innerBoundX;
			float num4 = m_pos.Y - m_originalScale.Y * 0.5f - innerBoundY;
			float num5 = m_pos.Y + m_originalScale.Y * 0.5f + innerBoundY;
			if (m_touchInRegion == -1)
			{
				m_touchInRegion = Game.TouchInRegion(num2, num3, num4, num5);
				if (m_touchInRegion != -1)
				{
					if (Game.IsTouchDown(m_touchInRegion) != 2)
					{
						m_touchInRegion = -1;
					}
				}
				else
				{
					UpdateTouchPosition();
				}
				if (m_fruitType < 0)
				{
					m_scale = m_originalScale;
				}
			}
			else if (Game.IsTouchDown(m_touchInRegion) == 0)
			{
				m_touchInRegion = -1;
				if (m_touchPos.X >= num2 && m_touchPos.X <= num3 && m_touchPos.Y >= num4 && m_touchPos.Y <= num5)
				{
					TouchReleased();
				}
			}
			else
			{
				UpdateTouchPosition();
				if (m_fruitType < 0 && m_growOnPressed)
				{
					if (m_touchPos.X >= num2 && m_touchPos.X <= num3 && m_touchPos.Y >= num4 && m_touchPos.Y <= num5)
					{
						m_scale = m_originalScale * 1.25f;
					}
					else
					{
						m_scale = m_originalScale;
					}
				}
			}
		}
		else if (m_fruitType < 0)
		{
			m_scale = m_originalScale;
		}
		m_scratchScale = m_scale.X * SCRATCH_SCALE * m_overallScratchScale;
		if (m_shakeTime > 0f)
		{
			m_shakeTime -= dt;
			if (m_shakeTime < 0f)
			{
				m_shakeTime = 0f;
			}
		}
	}

	public override void PreDraw(float[] tintChannels)
	{
	}

	public override void Draw(float[] tintChannels)
	{
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_0369: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_0425: Unknown result type (might be due to invalid IL or missing references)
		//IL_042b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0408: Unknown result type (might be due to invalid IL or missing references)
		//IL_040e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00df: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Unknown result type (might be due to invalid IL or missing references)
		//IL_0430: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0884: Unknown result type (might be due to invalid IL or missing references)
		//IL_0887: Unknown result type (might be due to invalid IL or missing references)
		//IL_088c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ea: Unknown result type (might be due to invalid IL or missing references)
		//IL_08f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_0904: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0203: Unknown result type (might be due to invalid IL or missing references)
		//IL_0252: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_0258: Unknown result type (might be due to invalid IL or missing references)
		//IL_0211: Unknown result type (might be due to invalid IL or missing references)
		//IL_0241: Unknown result type (might be due to invalid IL or missing references)
		//IL_0246: Unknown result type (might be due to invalid IL or missing references)
		//IL_024b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0285: Unknown result type (might be due to invalid IL or missing references)
		//IL_028b: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		if (PopOverControl.IsInPopup && !partOfPopup)
		{
			return;
		}
		int num = 255;
		if (m_fruitType >= 0)
		{
			num = Math.CLAMP((int)(256f * (float)m_scaleInOut / (float)(int)POP_SINE), 0, 255);
		}
		if (m_drawOrder == HUD.HUD_ORDER.HUD_ORDER_BEFORE_SPLAT && num > 0)
		{
			m_drawOrder = HUD.HUD_ORDER.HUD_ORDER_AFTER_SPLAT;
			if (m_scratchTexture != null)
			{
				m_scratchTexture.Set();
				MatrixManager.GetInstance().Reset();
				MatrixManager.GetInstance().Scale(new Vector3((float)((!m_scratchFlip) ? 1 : (-1)), 1f, 1f) * m_scratchScale);
				MatrixManager.GetInstance().Translate(new Vector3(m_pos.X, m_pos.Y, -5500f));
				MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
				Color col = HUDControl.TintWhite(tintChannels);
				col.A = (byte)num;
				Mesh.DrawQuad(col, 0f, 1f, 0f, 1f);
			}
			return;
		}
		if (num > 0 && m_texture != null)
		{
			if (m_pos.X < (float)(0L - (long)(240 + m_texture.w)) || m_pos.X > (float)(240 + m_texture.w) || m_pos.Y < (float)(0L - (long)(160 + m_texture.h)) || m_pos.Y > (float)(160 + m_texture.h))
			{
				return;
			}
			m_texture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(m_scale);
			if (m_rotation != 0f)
			{
				MatrixManager.GetInstance().RotZ(m_rotation);
			}
			MatrixManager.GetInstance().Scale(new Vector3(0.9f, 1f, 1f));
			Vector3 val = Vector3.Zero;
			if (m_shakeTime > 0f)
			{
				val += new Vector3(Math.g_random.RandF(6f) - 3f, Math.g_random.RandF(6f) - 3f, 0f);
			}
			MatrixManager.GetInstance().Translate(m_pos + val);
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			Color col2 = (m_enabled ? HUDControl.TintColor(new Color(255, 255, 255, num), tintChannels) : HUDControl.TintColor(new Color(128, 128, 128, num), tintChannels));
			Mesh.DrawQuad(col2, m_uvs[0].X, m_uvs[1].X, m_uvs[0].Y, m_uvs[1].Y);
		}
		if (m_newSymbol >= 0f && !Game.isWP7TrialMode())
		{
			float num2 = m_scale.X / m_originalScale.X;
			s_newTexture.Set();
			MatrixManager.GetInstance().Reset();
			MatrixManager.GetInstance().Scale(new Vector3(64f * num2, 32f * num2, 0f));
			MatrixManager.GetInstance().Translate(m_pos + new Vector3(m_newSymbolOffset.X * m_originalScale.X * 0.5f, m_newSymbolOffset.Y * m_originalScale.Y * 0.5f + Math.ABS(Math.SinIdx(Math.DEGREE_TO_IDX(180f * m_newSymbol))) * BOB_AMT, 0f) * num2);
			MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
			Color col3 = (m_enabled ? HUDControl.TintColor(new Color(255, 255, 255, num), tintChannels) : HUDControl.TintColor(new Color(128, 128, 128, num), tintChannels));
			Mesh.DrawQuad(col3, 0f, 1f, 0f, 1f);
		}
		if (!(m_loadingSymbolTime >= 0f) || m_loadingTexture == null)
		{
			return;
		}
		int num3 = (int)m_loadingSymbolTime;
		num3 %= 8;
		num3 = 7 - num3;
		if (0 == 0)
		{
			bool flag = true;
			float num4 = 0.6f;
			float num5 = 0.075f;
			Vector3 val2 = default(Vector3);
			Vector3 val3 = default(Vector3);
			for (int i = 0; i < 8; i++)
			{
				val2 = new Vector3(Math.SinIdx(Math.DEGREE_TO_IDX(i * 45)) * 0.5f, Math.CosIdx(Math.DEGREE_TO_IDX(i * 45)) * 0.5f, 0f);
				val3 = new Vector3(Math.SinIdx(Math.DEGREE_TO_IDX(i * 45 + 90)) * num5, Math.CosIdx(Math.DEGREE_TO_IDX(i * 45 + 90)) * num5, 0f);
				symbo_tris[i * 6].X = val2.X - val3.X;
				symbo_tris[i * 6].Y = val2.Y - val3.Y;
				symbo_tris[i * 6].u = 0f;
				symbo_tris[i * 6].v = 0f;
				symbo_tris[i * 6 + 1].X = val2.X + val3.X;
				symbo_tris[i * 6 + 1].Y = val2.Y + val3.Y;
				symbo_tris[i * 6 + 1].u = 1f;
				symbo_tris[i * 6 + 1].v = 0f;
				symbo_tris[i * 6 + 2].X = val2.X * num4 - val3.X;
				symbo_tris[i * 6 + 2].Y = val2.Y * num4 - val3.Y;
				symbo_tris[i * 6 + 2].u = 0f;
				symbo_tris[i * 6 + 2].v = 1f;
				symbo_tris[i * 6 + 3].X = val2.X * num4 - val3.X;
				symbo_tris[i * 6 + 3].Y = val2.Y * num4 - val3.Y;
				symbo_tris[i * 6 + 3].u = 0f;
				symbo_tris[i * 6 + 3].v = 1f;
				symbo_tris[i * 6 + 4].X = val2.X + val3.X;
				symbo_tris[i * 6 + 4].Y = val2.Y + val3.Y;
				symbo_tris[i * 6 + 4].u = 1f;
				symbo_tris[i * 6 + 4].v = 0f;
				symbo_tris[i * 6 + 5].X = val2.X * num4 + val3.X;
				symbo_tris[i * 6 + 5].Y = val2.Y * num4 + val3.Y;
				symbo_tris[i * 6 + 5].u = 1f;
				symbo_tris[i * 6 + 5].v = 1f;
				for (int j = 0; j < 6; j++)
				{
					symbo_tris[i * 6 + j].nz = 1f;
					symbo_tris[i * 6 + j].Z = 0f;
				}
			}
		}
		Color val4 = default(Color);
		for (int k = 0; k < 8; k++)
		{
			int num6 = Math.CLAMP((num3 + k) % 8 * 32, 64, 255);
			val4 = new Color(num6, num6, num6, 200);
			val4 = HUDControl.TintColor(val4, tintChannels);
			for (int l = 0; l < 6; l++)
			{
				symbo_tris[k * 6 + l].color = val4;
			}
		}
		m_loadingTexture.Set();
		MatrixManager.GetInstance().Reset();
		MatrixManager.GetInstance().Scale(Vector3.One * m_scale.Y * 0.75f);
		MatrixManager.GetInstance().Translate(m_pos);
		MatrixManager.GetInstance().UploadCurrentMatrices(quick: true);
		Mesh.DrawTriList(symbo_tris, 48, hasAlpha: false);
	}

	public void Shake(float time)
	{
		m_shakeTime = time;
	}

	public override HUD_TYPE GetType()
	{
		return HUD_TYPE.HUD_TYPE_BUTTON;
	}

	public override void Skip()
	{
		m_scaleInOut = POP_SINE;
	}

	public static void LoadContent()
	{
		m_scratchTexture = TextureManager.GetInstance().Load("textureswp7/scratchs.tex");
		m_loadingTexture = null;
		s_newTexture = TextureManager.GetInstance().Load("new_item.tex", localise: true);
	}

	public static void UnLoadContent()
	{
		m_scratchTexture = null;
		m_loadingTexture = null;
		s_newTexture = null;
	}

	public void SetLoadingSymbol(bool loading)
	{
		if (m_loadingSymbolTime >= 0f)
		{
			if (!loading)
			{
				m_loadingSymbolTime = -1f;
			}
		}
		else if (loading)
		{
			m_loadingSymbolTime = 0f;
		}
	}

	public bool IsLoadingSymbol()
	{
		return m_loadingSymbolTime >= 0f;
	}

	public void SetNewSymbol(bool hasNew)
	{
		if (hasNew)
		{
			if (m_newSymbol < 0f)
			{
				m_newSymbol = 0f;
			}
		}
		else if (m_newSymbol >= 0f)
		{
			m_newSymbol = -1f;
		}
	}

	public bool HasNewSymbol()
	{
		return m_newSymbol >= 0f;
	}

	public void Remove()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (m_entity != null && !((Fruit)m_entity).Sliced())
		{
			((Fruit)m_entity).m_isSliced = true;
			((Fruit)m_entity).m_vel = new Vector3(Math.g_random.RandF(10f), 0f - Math.g_random.RandF(5f), 0f);
			((Fruit)m_entity).m_vel2 = m_entity.m_vel;
			((Fruit)m_entity).m_hudControl = null;
			m_entity = null;
			m_link = null;
		}
	}

	public void RemoveNoShow()
	{
		//IL_0060: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0080: Unknown result type (might be due to invalid IL or missing references)
		if (m_entity != null && !((Fruit)m_entity).Sliced())
		{
			((Fruit)m_entity).m_isSliced = true;
			((Fruit)m_entity).m_vel = new Vector3(Math.g_random.RandF(10f), 0f - Math.g_random.RandF(5f), 0f);
			((Fruit)m_entity).m_vel2 = m_entity.m_vel;
			((Fruit)m_entity).m_hudControl = null;
			ref Vector3 pos = ref ((Fruit)m_entity).m_pos;
			pos.Y += 400f;
			m_entity = null;
			m_link = null;
		}
	}
}
