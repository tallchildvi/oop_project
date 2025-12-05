/// <file>
/// <summary>
/// BaseMenu.cs - Provides the base class for all UI menus in the game.
/// </summary>
/// <remarks>
/// This abstract class inherits from <see cref="Panel"/> and implements basic
/// functionality required for menu control, including setting a default visual style,
/// registering itself with the <c>MenuManager</c>, and providing virtual methods for
/// opening, closing, and toggling visibility.
/// </remarks>
/// </file>
using Godot;
using System;

/// <summary>
/// The base class for all game menus. It standardizes the visual appearance and
/// integration with the <c>MenuManager</c> for centralized control.
/// It inherits from <see cref="Panel"/> to provide a container for UI elements.
/// </summary>
public partial class BaseMenu : Panel
{
	/// <summary>
    /// Gets or sets the unique string name used to identify this menu instance
    /// within the <c>MenuManager</c>. This must be set by derived classes (e.g., in <see cref="_Ready"/>).
    /// </summary>
	public string MenuName { get; set; }
	/// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// It applies a standard background and border style to the menu panel,
    /// and defers registration with the <c>MenuManager</c>.
    /// </summary>
    public override void _Ready()
    {
        // --- Custom Style Initialization ---
        var style = new StyleBoxFlat();
        
        // Background color (light brown/tan with high opacity)
        style.BgColor = new Color(0.694f, 0.6f, 0.467f, 0.9f); 
        
        // Border color (dark brown/sepia)
        style.BorderColor = new Color(0.431f, 0.302f, 0.196f, 0.9f);     
        
        // Border width for all sides
        style.BorderWidthLeft = 4;
        style.BorderWidthTop = 4;
        style.BorderWidthRight = 4;
        style.BorderWidthBottom = 4;
        
        // Content margins (padding)
        style.ContentMarginLeft = 6; 
        style.ContentMarginTop = 6;
        style.ContentMarginRight = 6;
        style.ContentMarginBottom = 6;
        
        // Rounded corners
        style.CornerRadiusTopLeft = 15;
        style.CornerRadiusTopRight = 15;
        style.CornerRadiusBottomLeft = 15;
        style.CornerRadiusBottomRight = 15;
        
        // Apply the custom style to the panel override theme
        this.AddThemeStyleboxOverride("panel", style);
        
        // Defer registration to ensure MenuManager is ready and initialized first
        CallDeferred(nameof(RegisterWithMenuManager));
    }

    /// <summary>
    /// Registers this menu instance with the global <c>MenuManager</c> singleton.
    /// This method is called deferred to ensure the <c>MenuManager</c> has completed its <see cref="_Ready"/> step.
    /// </summary>
    private void RegisterWithMenuManager()
    {
        if (!string.IsNullOrEmpty(MenuName) && MenuManager.Instance != null)
            // Assumes MenuManager.Instance is the singleton manager
            MenuManager.Instance.RegisterMenu(MenuName, this);
        else
            // Prints an error if registration fails due to a missing name or manager
            GD.PrintErr($"[BaseMenu] Could not register menu '{MenuName}' — MenuManager.Instance is null or name empty.");
    }

    /// <summary>
    /// Opens the menu by setting its <see cref="CanvasItem.Visible"/> property to <c>true</c>.
    /// Derived classes can override this method to include transition effects or custom logic.
    /// </summary>
    public virtual void Open()
    {
        this.Visible = true;
    }

    /// <summary>
    /// Closes the menu by setting its <see cref="CanvasItem.Visible"/> property to <c>false</c>.
    /// Derived classes can override this method to include transition effects or custom logic.
    /// </summary>
    public virtual void Close()
    {
        GD.Print("try to close");
        this.Visible = false;
    }

    /// <summary>
    /// Toggles the visibility state of the menu. If visible, it closes; otherwise, it opens.
    /// </summary>
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