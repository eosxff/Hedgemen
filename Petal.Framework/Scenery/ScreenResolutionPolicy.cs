namespace Petal.Framework.Scenery;

public enum ScreenResolutionPolicy
{
	/// <summary>
	/// Default. RenderTarget matches the screen size
	/// </summary>
	None,

	/// <summary>
	/// The entire application is visible in the specified area without trying to preserve the original aspect ratio.
	/// Distortion can occur, and the application may appear stretched or compressed.
	/// </summary>
	ExactFit,

	/// <summary>
	/// The entire application fills the specified area, without distortion but possibly with some cropping,
	/// while maintaining the original aspect ratio of the application.
	/// </summary>
	NoBorder,

	/// <summary>
	/// Pixel perfect version of NoBorder. Scaling is limited to integer values.
	/// </summary>
	NoBorderPixelPerfect,

	/// <summary>
	/// The entire application is visible in the specified area without distortion while maintaining the original
	/// aspect ratio of the application. Borders can appear on two sides of the application.
	/// </summary>
	ShowAll,

	/// <summary>
	/// Pixel perfect version of ShowAll. Scaling is limited to integer values.
	/// </summary>
	ShowAllPixelPerfect,

	/// <summary>
	/// The application takes the height of the design resolution size and modifies the width of the internal
	/// canvas so that it fits the aspect ratio of the device.
	/// no distortion will occur however you must make sure your application works on different
	/// aspect ratios
	/// </summary>
	FixedHeight,

	/// <summary>
	/// Pixel perfect version of FixedHeight. Scaling is limited to integer values.
	/// </summary>
	FixedHeightPixelPerfect,

	/// <summary>
	/// The application takes the width of the design resolution size and modifies the height of the internal
	/// canvas so that it fits the aspect ratio of the device.
	/// no distortion will occur however you must make sure your application works on different
	/// aspect ratios
	/// </summary>
	FixedWidth,

	/// <summary>
	/// Pixel perfect version of FixedWidth. Scaling is limited to integer values.
	/// </summary>
	FixedWidthPixelPerfect,

	/// <summary>
	/// The application takes the width and height that best fits the design resolution with optional cropping inside of the "bleed area"
	/// and possible letter/pillar boxing. Works just like ShowAll except with horizontal/vertical bleed (padding). Gives you an area much
	/// like the old TitleSafeArea. Example: if design resolution is 1348x900 and bleed is 148x140 the safe area would be 1200x760 (design
	/// resolution - bleed).
	/// </summary>
	BestFit
}