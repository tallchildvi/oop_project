using Godot;
using System;

public partial class BaseMenu : Panel
{
	public string MenuName { get; set; }
	public override void _Ready()
	{
		var style = new StyleBoxFlat();
		style.BgColor = new Color(0.694f, 0.6f, 0.467f, 0.9f); 
		style.BorderColor = new Color(0.431f, 0.302f, 0.196f, 0.9f);    
		style.BorderWidthLeft = 4;
		style.BorderWidthTop = 4;
		style.BorderWidthRight = 4;
		style.BorderWidthBottom = 4;
		style.ContentMarginLeft = 6; 
		style.ContentMarginTop = 6;
		style.ContentMarginRight = 6;
		style.ContentMarginBottom = 6;
		style.CornerRadiusTopLeft = 15;
		style.CornerRadiusTopRight = 15;
		style.CornerRadiusBottomLeft = 15;
		style.CornerRadiusBottomRight = 15;
		this.AddThemeStyleboxOverride("panel", style);
		CallDeferred(nameof(RegisterWithMenuManager));
	}
	
	private void RegisterWithMenuManager()
	{
		if (!string.IsNullOrEmpty(MenuName) && MenuManager.Instance != null)
			MenuManager.Instance.RegisterMenu(MenuName, this);
		else
			GD.PrintErr($"[BaseMenu] Could not register menu '{MenuName}' — MenuManager.Instance is null or name empty.");
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
