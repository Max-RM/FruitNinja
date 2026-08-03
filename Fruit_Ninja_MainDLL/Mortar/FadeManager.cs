using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Mortar;

public class FadeManager
{
	private enum FadeColour
	{
		None,
		Black,
		White
	}

	private enum FadeType
	{
		None,
		Colour,
		Normal
	}

	private const float MINIMUM_FADE_TIME = 17f;

	private Texture2D m_black;

	private Texture2D m_white;

	private FadeColour m_colour;

	private FadeState m_state;

	private FadeType m_type;

	private float m_alpha;

	private float m_time;

	private float m_step;

	private bool m_fading;

	public bool IsFading
	{
		get
		{
			lock (this)
			{
				return m_fading;
			}
		}
	}

	public bool IsFadeVisible => m_alpha != 0f;

	public FadeManager(Texture2D black, Texture2D white)
	{
		Reset();
		m_black = black;
		m_white = white;
		m_colour = FadeColour.None;
	}

	public void Start(FadeState state, float time)
	{
		lock (this)
		{
			if (m_state == FadeState.None)
			{
				switch (state)
				{
				case FadeState.ToBlack:
					m_colour = FadeColour.Black;
					m_time = time;
					m_fading = true;
					m_step = m_time / 255f;
					m_type = FadeType.Colour;
					break;
				case FadeState.ToWhite:
					m_colour = FadeColour.White;
					m_time = time;
					m_fading = true;
					m_step = m_time / 255f;
					m_type = FadeType.Colour;
					break;
				case FadeState.ToNormal:
					m_time = time * 2f;
					m_fading = true;
					m_step = m_time / 255f;
					m_type = FadeType.Normal;
					break;
				default:
					throw new ArgumentException("Invald fade state");
				}
			}
		}
	}

	public void Update(GameTime gameTime)
	{
		if (m_fading)
		{
			m_time -= gameTime.ElapsedGameTime.Milliseconds;
			if (m_time < 0f)
			{
				m_fading = false;
				m_time = 0f;
				m_step = 0f;
				m_alpha = ((m_type == FadeType.Colour) ? 255f : 0f);
			}
			else
			{
				float num = m_time / m_step;
				m_alpha = ((m_type == FadeType.Colour) ? (255f - num) : num);
			}
		}
	}

	public void Draw(GraphicsDevice device, SpriteBatch batch)
	{
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0045: Unknown result type (might be due to invalid IL or missing references)
		//IL_0065: Unknown result type (might be due to invalid IL or missing references)
		if (m_alpha > 0f)
		{
			Texture2D val = ((m_colour == FadeColour.Black) ? m_black : m_white);
			Viewport viewport = device.Viewport;
			int width = viewport.Width;
			Viewport viewport2 = device.Viewport;
			batch.Draw(val, new Rectangle(0, 0, width, viewport2.Height), new Color(1f, 1f, 1f, m_alpha / 255f));
		}
	}

	private void Reset()
	{
		m_colour = FadeColour.None;
		m_state = FadeState.None;
		m_type = FadeType.None;
		m_alpha = 0f;
		m_time = 0f;
		m_step = 0f;
		m_fading = false;
	}

	public void DrawFade(GraphicsDevice device, SpriteBatch batch)
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_002a: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		Color white = Color.White;
		white.A = 128;
		Texture2D black = m_black;
		Viewport viewport = device.Viewport;
		int width = viewport.Width;
		Viewport viewport2 = device.Viewport;
		batch.Draw(black, new Rectangle(0, 0, width, viewport2.Height), white);
	}

	public void SetBlack()
	{
		lock (this)
		{
			m_state = FadeState.None;
			m_colour = FadeColour.Black;
			m_alpha = 255f;
			m_fading = false;
		}
	}
}
