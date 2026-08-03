using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

public class Coin : Entity
{
	public delegate void CoinArrivedCallback(Coin sdf);

	public enum COIN_STATE
	{
		COIN_STATE_START,
		COIN_STATE_DEAD,
		COIN_STATE_POPOUT,
		COIN_STATE_WAIT,
		COIN_STATE_HEADH_TO_CORNER
	}

	protected int m_worth;

	protected int m_state;

	protected float m_stateTime;

	protected bool m_useModel;

	protected uint m_trailHash;

	protected uint m_burstHash;

	protected Vector3 m_destination;

	protected CoinArrivedCallback m_coinArrivedCall;

	protected float m_speed;

	protected ushort m_rollyAngle;

	protected PSPParticleEmitter m_trailEmitter;

	protected PSPParticleEmitter m_burstEmitter;

	private static Model s_coinModel = null;

	private static bool s_isContentLoaded = false;

	public static int PiIdx = 32768;

	public static int TwoPiIdx = 65536;

	public static float COIN_INIT_VEL_MIN => 500f;

	public static float COIN_INIT_VEL_MAX => 1050f;

	public static float COIN_INIT_VEL_RANGE => COIN_INIT_VEL_MAX - COIN_INIT_VEL_MIN;

	public static float COIN_INIT_VEL => COIN_INIT_VEL_MIN + Math.g_random.RandF(COIN_INIT_VEL_RANGE);

	public static float COIN_CORNER_POS_X => Game.SCREEN_WIDTH / 2f - 20f;

	public static float COIN_CORNER_POS_Y => (0f - Game.SCREEN_HEIGHT) / 2f + 20f;

	public static Vector3 COIN_CORNER_POS => new Vector3(COIN_CORNER_POS_X, COIN_CORNER_POS_Y, 0f);

	public static float COIN_WAIT_TIME => 0.05f;

	public static float ROT_SPEED => 0.065f;

	public static float SCALE_NORMAL => 0.5f;

	public static float SCALE_POP => 1f;

	public int GetWorth()
	{
		return m_worth;
	}

	public static void LoadContent()
	{
		s_coinModel = MeshManager.GetInstance().Load("models/Fruit/coin.mmd");
		s_isContentLoaded = true;
	}

	public static void UnLoadContent()
	{
		s_coinModel = null;
		s_isContentLoaded = false;
	}

	public void InitCoin(Vector3 pos, Vector3 dest, ushort angle, int worth, uint trailHash, uint burstHash)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitCoin(pos, dest, angle, worth, trailHash, burstHash, CoinArrived);
	}

	public void InitCoin(Vector3 pos, Vector3 dest, ushort angle, int worth, uint trailHash, uint burstHash, CoinArrivedCallback call)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitCoin(pos, dest, angle, worth, trailHash, burstHash, call, 0f);
	}

	public void InitCoin(Vector3 pos, Vector3 dest, ushort angle, int worth, uint trailHash, uint burstHash, CoinArrivedCallback call, float wait)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		InitCoin(pos, dest, angle, worth, trailHash, burstHash, call, wait, useModel: true);
	}

	public void InitCoin(Vector3 pos, Vector3 dest, ushort angle, int worth, uint trailHash, uint burstHash, CoinArrivedCallback call, float wait, bool useModel)
	{
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		base.m_destroy = false;
		base.m_dormant = false;
		m_speed = COIN_INIT_VEL;
		m_pos = pos;
		m_destination = dest;
		m_dir_angle = angle;
		m_worth = worth;
		m_state = 0;
		m_stateTime = 0f - wait;
		m_vel = Vector3.Zero;
		m_cur_scale = Vector3.One * SCALE_NORMAL;
		m_trailEmitter = null;
		m_burstEmitter = null;
		m_trailHash = trailHash;
		m_burstHash = burstHash;
		m_useModel = useModel;
		m_coinArrivedCall = call;
		m_rollyAngle = 0;
	}

	public Coin()
	{
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!s_isContentLoaded)
		{
			LoadContent();
		}
		m_worth = 1;
		m_state = 0;
		m_stateTime = 0f;
		m_pos = Vector3.Zero;
		m_vel = Vector3.Zero;
		base.m_destroy = false;
		base.m_dormant = false;
		m_speed = 0f;
		m_useModel = true;
		m_destination = COIN_CORNER_POS;
	}

	public override void Release()
	{
		if (m_trailEmitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_trailEmitter);
			m_trailEmitter = null;
		}
	}

	private static int GetSmallestDelta(int to, int from)
	{
		int num = to - from;
		if (Math.ABS(num) > PiIdx)
		{
			num = ((to > from) ? (num - TwoPiIdx) : (to + TwoPiIdx - from));
		}
		return num;
	}

	public override void Update(float dt)
	{
		float dt2 = Game.game_work.dt;
		Game.game_work.dt = 1f / 60f;
		_Update(1f / 60f);
		if (!base.m_dormant)
		{
			_Update(1f / 60f);
		}
		Game.game_work.dt = dt2;
	}

	public void _Update(float dt)
	{
		//IL_010c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_011b: Unknown result type (might be due to invalid IL or missing references)
		//IL_047a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_048f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0494: Unknown result type (might be due to invalid IL or missing references)
		//IL_0499: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_04b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0465: Unknown result type (might be due to invalid IL or missing references)
		//IL_046a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0354: Unknown result type (might be due to invalid IL or missing references)
		//IL_0359: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0201: Unknown result type (might be due to invalid IL or missing references)
		dt *= 0.25f;
		if (m_burstEmitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_burstEmitter);
			m_burstEmitter = null;
		}
		switch (m_state)
		{
		case 0:
			m_stateTime -= Game.game_work.dt;
			if (!(m_stateTime <= 0f))
			{
				break;
			}
			m_state = 2;
			m_vel = new Vector3(Math.SinIdx(m_dir_angle) * m_speed, Math.CosIdx(m_dir_angle) * m_speed, 0f) * 1.5f;
			if (!m_useModel)
			{
				m_stateTime = 0f;
				m_state = 4;
				if (PSPParticleManager.GetInstance().EmitterExists(m_trailHash))
				{
					m_trailEmitter = PSPParticleManager.GetInstance().AddEmitter(m_trailHash, null);
				}
			}
			break;
		case 2:
		{
			m_vel *= 0.7f;
			float num6 = m_vel.LengthSquared();
			if (num6 < 900f)
			{
				m_state = 3;
				if (PSPParticleManager.GetInstance().EmitterExists(m_trailHash))
				{
					m_trailEmitter = PSPParticleManager.GetInstance().AddEmitter(m_trailHash, null);
				}
			}
			break;
		}
		case 3:
		{
			float stateTime = m_stateTime;
			m_stateTime += Game.game_work.dt;
			if (stateTime <= 0.01f && m_stateTime > 0.01f && (ActorManager.GetInstance().GetNumEntities(2) < 20 || Math.g_random.Rand32(3) == 0) && PSPParticleManager.GetInstance().EmitterExists(m_burstHash))
			{
				m_burstEmitter = PSPParticleManager.GetInstance().AddEmitter(m_burstHash, null);
				if (m_burstEmitter != null)
				{
					m_burstEmitter.pos = m_pos;
				}
			}
			if (m_stateTime >= COIN_WAIT_TIME)
			{
				m_stateTime = 0f;
				m_state = 4;
				float y = m_destination.X - m_pos.X;
				float x = m_destination.Y - m_pos.Y;
				ushort to2 = Math.Atan2Idx(y, x);
				m_dir_angle += (ushort)((float)GetSmallestDelta(to2, m_dir_angle) * 0.25f);
			}
			break;
		}
		case 4:
		{
			m_stateTime += Game.game_work.dt;
			float num = m_destination.X - m_pos.X;
			float num2 = m_destination.Y - m_pos.Y;
			float num3 = Math.Sqrt(num * num + num2 * num2);
			if (num3 < 40f || m_stateTime > 1.5f)
			{
				if ((ActorManager.GetInstance().GetNumEntities(2) < 20 || Math.g_random.Rand32(3) == 0) && PSPParticleManager.GetInstance().EmitterExists(m_burstHash))
				{
					m_burstEmitter = PSPParticleManager.GetInstance().AddEmitter(m_burstHash, null);
					if (m_burstEmitter != null)
					{
						m_burstEmitter.pos = m_pos;
					}
				}
				m_state = 1;
				return;
			}
			ushort to = Math.Atan2Idx(num, num2);
			float num4 = Math.MIN(ROT_SPEED * (1f + m_stateTime * 5f + Math.MAX(100f - num3, 0f) / 5f), 0.85f + Math.MAX(100f - num3, 0f) * 0.15f / 100f);
			m_dir_angle += (ushort)(int)((float)GetSmallestDelta(to, m_dir_angle) * num4);
			float num5 = Math.MIN((m_stateTime + 0.05f) * 2f, 1f);
			m_rollyAngle += (ushort)(int)(num5 * (float)(int)Math.DEGREE_TO_IDX(180f) * dt * 500f);
			num5 *= m_speed * 2f;
			m_vel = new Vector3(Math.SinIdx(m_dir_angle) * num5, Math.CosIdx(m_dir_angle) * num5, 0f);
			break;
		}
		case 1:
			Arrived();
			return;
		}
		m_pos += m_vel * Game.game_work.dt;
		if (m_trailEmitter != null)
		{
			m_trailEmitter.pos = m_pos;
			m_trailEmitter.sinz = Math.SinIdx(m_dir_angle);
			m_trailEmitter.cosz = Math.CosIdx(m_dir_angle);
		}
	}

	public static void CoinArrived(Coin theCoin)
	{
		Game.AddCoins(theCoin.GetWorth());
	}

	public override void Draw()
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (m_useModel && s_coinModel != null && m_state > 1)
		{
			Matrix val = Matrix.CreateScale(m_cur_scale);
			val *= Matrix.CreateRotationY(Math.IDX_TO_RADIANS(m_rollyAngle));
			val *= Matrix.CreateRotationZ(Math.IDX_TO_RADIANS(m_dir_angle));
			val *= Matrix.CreateTranslation(m_pos);
			s_coinModel.Draw(val);
		}
	}

	public override void DrawUpdate(float dt)
	{
	}

	public void Arrived()
	{
		m_coinArrivedCall(this);
		if (m_trailEmitter != null)
		{
			PSPParticleManager.GetInstance().ClearEmitter(m_trailEmitter);
			m_trailEmitter = null;
		}
		base.m_dormant = true;
		base.m_destroy = true;
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, null);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, dest, 0.02f);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest, float delay)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, dest, delay, 0.15f);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest, float delay, float maxTotalDelay)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, dest, delay, maxTotalDelay, null);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest, float delay, float maxTotalDelay, string trailEmmiter)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, dest, delay, maxTotalDelay, trailEmmiter, null);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest, float delay, float maxTotalDelay, string trailEmmiter, string burstEmmiter)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, dest, delay, maxTotalDelay, trailEmmiter, burstEmmiter, CoinArrived);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest, float delay, float maxTotalDelay, string trailEmmiter, string burstEmmiter, CoinArrivedCallback call)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		MakeCoins(numberOfCoins, coinWorth, pos, direction, range, dest, delay, maxTotalDelay, trailEmmiter, burstEmmiter, call, useModel: true);
	}

	public static void MakeCoins(int numberOfCoins, int coinWorth, Vector3 pos, ushort direction, ushort range, Vector3? dest, float delay, float maxTotalDelay, string trailEmmiter, string burstEmmiter, CoinArrivedCallback call, bool useModel)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		if (numberOfCoins <= 0)
		{
			return;
		}
		int num = numberOfCoins / coinWorth + 1;
		if ((float)num * delay > maxTotalDelay)
		{
			delay = maxTotalDelay / (float)num;
		}
		Vector3 dest2 = (dest.HasValue ? dest.Value : COIN_CORNER_POS);
		uint trailHash = StringFunctions.StringHash((trailEmmiter != null) ? trailEmmiter : "coin_fly");
		uint burstHash = StringFunctions.StringHash((burstEmmiter != null) ? burstEmmiter : "coin_collect");
		int num2 = 0;
		for (int i = 0; i < numberOfCoins; i += coinWorth)
		{
			Coin coin = (Coin)ActorManager.GetInstance().Add(EntityTypes.ENTITY_COIN);
			int num3 = Math.g_random.Rand32(range) - (range >> 1);
			ushort num4 = (ushort)(direction + num3);
			float num5 = pos.X + Math.SinIdx(num4) * 100f;
			float num6 = pos.Y + Math.CosIdx(num4) * 100f;
			int num7 = 1;
			while ((num5 < (0f - Game.SCREEN_WIDTH) / 2f || num5 > Game.SCREEN_WIDTH / 2f || num6 < (0f - Game.SCREEN_HEIGHT) / 2f || num6 > Game.SCREEN_HEIGHT / 2f) && num7 < 10)
			{
				num7++;
				num3 = Math.g_random.Rand32(range) - (range >> 1);
				num4 = (ushort)(direction + num3);
				num5 = pos.X + Math.SinIdx(num4) * 100f;
				num6 = pos.Y + Math.CosIdx(num4) * 100f;
			}
			coin.InitCoin(pos, dest2, num4, Math.MIN(coinWorth, numberOfCoins - i), trailHash, burstHash, call, (float)num2 * delay, useModel);
			num2++;
		}
	}

	public static void ClearCoins(bool callDelegate)
	{
		LinkedListNode<Entity> iterator = null;
		for (Coin coin = (Coin)ActorManager.GetInstance().GetEntityFirst(EntityTypes.ENTITY_COIN, ref iterator); coin != null; coin = (Coin)ActorManager.GetInstance().GetEntityNext(EntityTypes.ENTITY_COIN, ref iterator))
		{
			if (callDelegate)
			{
				coin.Arrived();
			}
			else
			{
				coin.m_dormant = true;
				coin.m_destroy = true;
			}
		}
	}
}
