using Microsoft.Xna.Framework;
using Mortar;

namespace FruitNinja;

internal class LeaderboardItem : ScrollingMenuItem
{
	protected int m_score;

	protected int m_rank;

	public LeaderboardItem()
	{
	}

	public LeaderboardItem(string name, int rank, int score)
	{
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		m_text = rank + ". " + name.ToUpper();
		m_rank = rank;
		m_score = score;
		m_height = 25f;
		m_colour = new Color(116, 93, 59);
	}

	public override void Draw()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_001c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0117: Unknown result type (might be due to invalid IL or missing references)
		//IL_011d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0122: Unknown result type (might be due to invalid IL or missing references)
		//IL_0128: Unknown result type (might be due to invalid IL or missing references)
		//IL_0132: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0184: Unknown result type (might be due to invalid IL or missing references)
		//IL_0189: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		if (m_text != null)
		{
			Vector3 val = m_pos + m_textOffset;
			MortarRectangleDec? rect = null;
			MortarRectangleDec value = default(MortarRectangleDec);
			value.left = 0f;
			value.right = 0f;
			if (m_parentList != null)
			{
				value.top = m_parentList.m_pos.Y + m_parentList.GetHeight() / 2f;
				value.bottom = m_parentList.m_pos.Y - m_parentList.GetHeight() / 2f;
				value.left = m_parentList.m_pos.X - m_parentList.GetWidth() / 2f;
				value.right = m_parentList.m_pos.X + m_parentList.GetWidth() / 2f;
				rect = value;
			}
			Game.game_work.pGameFont.DrawString(m_text, new Vector3(value.left, val.Y, 0f) + m_textOffset, m_colour, 18f, Vector2.Zero, (ALIGNMENT_TYPE)13, 1f, rect);
			Game.game_work.pGameFont.DrawString(m_score.ToString(), new Vector3(value.right - m_parentList.GetWidth() * 0.1f, val.Y, 0f) + m_textOffset, m_colour, 18f, Vector2.Zero, (ALIGNMENT_TYPE)14, 1f, rect);
		}
	}
}
