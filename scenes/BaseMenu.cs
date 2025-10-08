using Godot;
using System;

public partial class BaseMenu : Panel
{
	public string MenuName { get; set; }
	public override void _Ready()
	{
		var style = new StyleBoxFlat();  // простий прямокутний стиль
		style.BgColor = new Color(0.694f, 0.6f, 0.467f, 0.9f); // колір фону
		style.BorderColor = new Color(0.431f, 0.302f, 0.196f, 0.9f);      // колір рамки
		style.BorderWidthLeft = 4;
		style.BorderWidthTop = 4;
		style.BorderWidthRight = 4;
		style.BorderWidthBottom = 4;
		style.ContentMarginLeft = 6;  // внутрішні відступи
		style.ContentMarginTop = 6;
		style.ContentMarginRight = 6;
		style.ContentMarginBottom = 6;
		style.CornerRadiusTopLeft = 15;
		style.CornerRadiusTopRight = 15;
		style.CornerRadiusBottomLeft = 15;
		style.CornerRadiusBottomRight = 15;
		this.AddThemeStyleboxOverride("panel", style);
		MenuManager.Instance.RegisterMenu(MenuName, this);
	}
	public virtual void Open()
	{
		this.Visible = true;
	}
	public virtual void Close()
	{
		GD.Print("try to close");
		this.Visible = false;
	}
	public virtual void Toggle()
	{
		if (Visible)
		{
			this.Visible = false;
		}
		else
		{
			this.Visible = true;
		}
	}
}
